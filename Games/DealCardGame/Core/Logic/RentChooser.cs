namespace DealCardGame.Core.Logic;
public class RentChooser(DealCardGameGameContainer gameContainer) : IEnumListClass<EnumRentCategory>
{
    BasicList<EnumRentCategory> IEnumListClass<EnumRentCategory>.GetEnumList()
    {
        var player = gameContainer.PlayerList!.GetWhoPlayer();
        int count = player.MainHandList.Count(x => x.ActionCategory == EnumActionCategory.DoubleRent);
        if (count == 0)
        {
            return [EnumRentCategory.Alone];
        }
        if (count == 1)
        {
            return [EnumRentCategory.Alone, EnumRentCategory.SingleDouble];
        }
        return [EnumRentCategory.Alone, EnumRentCategory.SingleDouble, EnumRentCategory.DoubleDouble];
    }
}