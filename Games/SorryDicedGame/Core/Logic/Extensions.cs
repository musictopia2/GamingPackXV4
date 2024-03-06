namespace SorryDicedGame.Core.Logic;
public static class Extensions
{
    public static SorryDiceModel? GetSelectedDice(this BasicList<SorryDiceModel> list)
    {
        return list.GetSelectedItems().FirstOrDefault();
    }
}