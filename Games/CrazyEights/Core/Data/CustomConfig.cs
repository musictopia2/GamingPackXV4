namespace CrazyEights.Core.Data;
[SingletonGame]
public class CustomConfig : IRegularCardsSortCategory
{
    public EnumRegularCardsSortCategory SortCategory => EnumRegularCardsSortCategory.NumberSuit;
}
