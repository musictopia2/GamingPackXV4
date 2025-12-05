namespace DealCardGame.Core.Logic;
public static class RentExtensions
{
    extension (RentModel rent)
    {
        public int RentOwed(DealCardGamePlayerItem player)
        {
            int output;
            var list = player!.SetData.GetCards(rent.Color);
            bool hasHouse = list.HasHouse;
            bool hasHotel = list.HasHotel;
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
}