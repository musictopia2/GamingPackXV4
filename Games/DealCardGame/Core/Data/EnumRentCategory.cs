namespace DealCardGame.Core.Data;
public readonly partial record struct EnumRentCategory
{
    private enum EnumInfo
    {
        NA = -1,
        NeedChoice = 0,
        Alone = 1,
        SingleDouble = 2,
        DoubleDouble = 3
    }
}

//public enum EnumRentCategory
//{
//    Alone,
//    SingleDouble,
//    DoubleDouble
//}