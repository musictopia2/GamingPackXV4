namespace SixtySix2Player.Core.Cards;
//if you don't need, remove.
[SingletonGame]
public class CustomConfig : IRegularCardsSortCategory
{
    public EnumRegularCardsSortCategory SortCategory => EnumRegularCardsSortCategory.SuitNumber;
}