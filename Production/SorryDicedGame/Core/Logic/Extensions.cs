namespace SorryDicedGame.Core.Logic;
public static class Extensions
{
    extension (BasicList<SorryDiceModel> list)
    {
        public SorryDiceModel? SelectedDice => list.GetSelectedItems().FirstOrDefault();
    }
}