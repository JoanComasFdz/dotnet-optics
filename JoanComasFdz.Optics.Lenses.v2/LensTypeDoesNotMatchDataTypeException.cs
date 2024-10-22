namespace JoanComasFdz.Optics.Lenses.v2;

public class LensTypeDoesNotMatchDataTypeException(string lensType, string dataType)
    : Exception($"This lens was declared with a type ({lensType}) that does not match the type of the nested property specified ({dataType}). Make sure to declare the Lens with the same types as the data structure.")
{
}
