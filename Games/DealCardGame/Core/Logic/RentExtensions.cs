namespace DealCardGame.Core.Logic;
public static class RentExtensions
{
    public static int RentOwed(this RentModel rent, DealCardGamePlayerItem player)
    {
        int output;
        var list = player!.SetData.GetCards(rent.Color);
        bool hasHouse = list.HasHouse();
        bool hasHotel = list.HasHotel();
        output = list.RentForSet(rent.Color, hasHouse, hasHotel);
        if (rent.RentCategory == EnumRentCategory.SingleDouble)
        {
            output *= 2;
        }
        else if (rent.RentCategory == EnumRentCategory.DoubleDouble)
        {
            output *= 4;
        }
        return output;
    }
}