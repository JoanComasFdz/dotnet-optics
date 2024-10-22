using System.Linq.Expressions;
using System.Reflection;
using System.Text.RegularExpressions;

namespace JoanComasFdz.Optics.Lenses.v2;

public record Lens<TWhole, TPart>(Func<TWhole, TPart> Get, Func<TWhole, TPart, TWhole> Set)
{
    // Create method with depth and type checks
    public static Lens<TWhole, TPart> Create(Expression<Func<TWhole, TPart>> expression)
    {
        // Ensure only 1 level of depth is allowed
        if (GetExpressionDepth(expression.Body) > 1)
        {
            throw new InvalidOperationException("Only 1 level of depth is allowed. Use Compose to chain lenses.");
        }

        // Check if the property is not a reference type or struct (i.e., avoid primitive types)
        var propertyType = typeof(TPart);
        if (IsPrimitiveType(propertyType))
        {
            throw new InvalidOperationException($"Lenses for primitive types like {propertyType.Name} are not allowed. Use the 'with' operator instead.");
        }

        var getter = expression.Compile();
        var setter = CreateMutator(expression);

        return new Lens<TWhole, TPart>(getter, setter);
    }

    // New Create method for collections, which allows returning a specific item in the collection
    public static Lens<TWhole, TPart> Create<TCollection>(
          Expression<Func<TWhole, IEnumerable<TPart>>> collectionProperty,
          Expression<Func<TPart, bool>> predicate)
    {
        // Infer the actual collection type (e.g., IReadOnlyList<TPart>, List<TPart>)
        var actualCollectionType = GetActualCollectionType(typeof(TWhole), collectionProperty);

        // Rewrite the original expression to have the correct collection type
        var rewriter = new CollectionTypeRewriter(actualCollectionType);
        var rewrittenCollectionProperty = (LambdaExpression)rewriter.Visit(collectionProperty);

        // Make the correct Lens2<TWhole, TCollection> type
        var wholeToCollectionLens = typeof(Lens<,>).MakeGenericType(typeof(TWhole), actualCollectionType);

        // Use reflection to dynamically call the correct Create method with the right collection type
        var createWholeToCollectionMethod = wholeToCollectionLens
            .GetMethods(BindingFlags.Public | BindingFlags.Static)
            .First(m => m.Name == "Create" && m.GetParameters().Length == 1); // Disambiguate by parameter count

        // Invoke the Create method to generate the collection lens
        var collectionLens = createWholeToCollectionMethod.Invoke(null, [rewrittenCollectionProperty]);

        // Create the itemLens with the correct collection type
        var collectionToItemLens = typeof(Lens<,>).MakeGenericType(actualCollectionType, typeof(TPart));
        var createCollectionToItemMethod = collectionToItemLens
            .GetMethods(BindingFlags.Public | BindingFlags.Static)
            .First(m => m.Name == "Create" && m.GetParameters().Length == 1); // Disambiguate by parameter count

        var itemLens = CreateItemLens(actualCollectionType, predicate);

        var composeMethod = typeof(LensExtensions) // Replace 'LensExtensions' with the class that contains the extension
        .GetMethods(BindingFlags.Public | BindingFlags.Static)
        .Single(m => m.Name == "Compose" && m.IsGenericMethod);

        // Make it generic with the appropriate types
        var genericComposeMethod = composeMethod.MakeGenericMethod(typeof(TWhole), actualCollectionType, typeof(TPart));

        // Invoke the Compose extension method
        var composedLens = genericComposeMethod.Invoke(null, [collectionLens, itemLens]);

        return (Lens<TWhole, TPart>)composedLens;
    }

    private class CollectionTypeRewriter(Type targetCollectionType) : ExpressionVisitor
    {
        protected override Expression VisitLambda<T>(Expression<T> node)
        {
            // We replace the lambda with a new one that has the correct return type
            var newBody = Visit(node.Body);
            var delegateType = typeof(Func<,>).MakeGenericType(node.Parameters[0].Type, targetCollectionType);
            return Expression.Lambda(delegateType, newBody, node.Parameters);
        }
    }

    // Helper method to determine the actual collection type
    private static Type GetActualCollectionType(Type wholeType, LambdaExpression collectionProperty)
    {
        var propertyName = ((MemberExpression)collectionProperty.Body).Member.Name;
        var property = wholeType.GetProperty(propertyName);

        if (property == null)
            throw new InvalidOperationException($"Property '{propertyName}' not found on type {wholeType}");

        return property.PropertyType;
    }

    private static object CreateItemLens(Type actualCollectionType, Expression<Func<TPart, bool>> predicate)
    {
        var compiledPredicate = predicate.Compile();

        // Getter: Find the item in the collection
        var collectionParameter = Expression.Parameter(actualCollectionType, "collection");
        var enumerableCollection = Expression.Convert(collectionParameter, typeof(IEnumerable<TPart>));

        var callSingleOrDefault = Expression.Call(
            typeof(Enumerable),
            nameof(Enumerable.SingleOrDefault),
            new Type[] { typeof(TPart) },
            enumerableCollection,
            Expression.Constant(compiledPredicate));

        var itemGetterLambda = Expression.Lambda(callSingleOrDefault, collectionParameter);
        var itemGetter = itemGetterLambda.Compile();

        // Setter: Replace the item in the collection
        var elementParameter = Expression.Parameter(typeof(TPart), "item");
        var updatedItemParameter = Expression.Parameter(typeof(TPart), "updatedItem");

        var predicateInvoke = Expression.Invoke(predicate, elementParameter);

        var replacementCondition = Expression.Condition(
            predicateInvoke,
            updatedItemParameter,
            elementParameter);

        var selectLambda = Expression.Lambda<Func<TPart, TPart>>(replacementCondition, elementParameter);

        // Find the correct Select method using reflection
        var selectMethod = typeof(Enumerable)
            .GetMethods(BindingFlags.Public | BindingFlags.Static)
            .First(m => m.Name == "Select"
                        && m.GetParameters().Length == 2
                        && m.GetParameters()[1].ParameterType.GetGenericTypeDefinition() == typeof(Func<,>))
            .MakeGenericMethod(typeof(TPart), typeof(TPart));

        var updatedCollection = Expression.Call(
            selectMethod,
            enumerableCollection,
            selectLambda);

        var toListCall = Expression.Call(
            typeof(Enumerable),
            nameof(Enumerable.ToList),
            new Type[] { typeof(TPart) },
            updatedCollection);

        var convertToCollectionType = Expression.Convert(toListCall, actualCollectionType);
        var itemSetterLambda = Expression.Lambda(convertToCollectionType, collectionParameter, updatedItemParameter);
        var itemSetter = itemSetterLambda.Compile();

        // Dynamically create the Lens2 object for the item lens
        var lensType = typeof(Lens<,>).MakeGenericType(actualCollectionType, typeof(TPart));
        return Activator.CreateInstance(lensType, itemGetter, itemSetter);
    }

    // Helper method to check if the type is a primitive or a string
    private static bool IsPrimitiveType(Type type)
    {
        return type.IsPrimitive || type == typeof(string) || type.IsValueType && !type.IsEnum;
    }

    // Helper method to calculate the depth of the expression
    private static int GetExpressionDepth(Expression expression)
    {
        int depth = 0;
        var current = expression;

        // Traverse the tree and increase the depth count until reaching a non-member expression
        while (current is MemberExpression memberExpression)
        {
            depth++;
            current = memberExpression.Expression;
        }

        return depth;
    }

    // Helper method for creating the setter (same as before)
    private static Func<TWhole, TPart, TWhole> CreateMutator(Expression<Func<TWhole, TPart>> expression)
    {
        var typeParam = expression.Parameters.First();
        var valueParam = Expression.Parameter(typeof(TPart), "v");

        var variables = new List<ParameterExpression>();
        var blockExpressions = new List<Expression>();

        var property = (MemberExpression)expression.Body;
        Expression currentValue = valueParam;
        var index = 0;

        while (property != null)
        {
            var variable = Expression.Variable(property.Expression.Type, $"v_{index}");
            variables.Add(variable);

            var cloneMethod = property.Expression.Type.GetMethod("<Clone>$");
            if (cloneMethod is null) throw new Exception($"CalcMutator: No Clone method on {property.Expression.Type}");
            var cloneCall = Expression.Call(property.Expression, cloneMethod);

            var assignClonedToVariable = Expression.Assign(variable, cloneCall);
            var accessVariableProperty = Expression.MakeMemberAccess(variable, property.Member);
            BinaryExpression? assignVariablePropertyValue;
            try
            {
                assignVariablePropertyValue = Expression.Assign(accessVariableProperty, currentValue);
            }
            catch (ArgumentException ae)
            {
                var regex = new Regex("'([^']*)'");
                var matches = regex.Matches(ae.Message);
                if (matches.Count < 2)
                {
                    throw;
                }

                string type1 = matches[0].Groups[1].Value;
                string type2 = matches[1].Groups[1].Value;
                throw new LensTypeDoesNotMatchDataTypeException(type1, type2);
            }

            blockExpressions.Add(assignClonedToVariable);
            blockExpressions.Add(assignVariablePropertyValue);

            property = property.Expression as MemberExpression;
            currentValue = variable;
            index++;
        }

        blockExpressions.Add(currentValue);

        var block = Expression.Block(variables, blockExpressions);
        var assignLambda = (Expression<Func<TWhole, TPart, TWhole>>)Expression.Lambda(block, typeParam, valueParam);
        return assignLambda.Compile();
    }
}
