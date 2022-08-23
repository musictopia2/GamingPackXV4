namespace BasicGameFrameworkLibrary.Core.ChooserClasses;

public class BoardGamesColorPicker<E, P> : SimpleEnumPickerVM<E>
    where E : struct, IFastEnumColorSimple
    where P : class, IPlayerBoardGame<E>, new()
{
    public PlayerCollection<P>? PlayerList;
    public void FillInRestOfColors() //this means fill rest of colors is no problem.
    {
        var thisList = ColorsLeft();
        thisList.ShuffleList();
        if (PlayerList!.Any(x => x.DidChooseColor == false && x.InGame == true))
        {
            throw new CustomBasicException("There is at least one player who did not choose color who is in game.  That is wrong");
        }
        BasicList<P> tempList = PlayerList!.Where(xx => xx.DidChooseColor == false).ToBasicList();
        tempList.ShuffleList();
        tempList.ForEach(thisPlayer =>
        {
            var item = thisList[tempList.IndexOf(thisPlayer)];
            thisPlayer.Color = item.EnumValue;
        });
    }
    private BasicList<BasicPickerData<E>> ColorsLeft()
    {
        var firstList = PrivateGetList(); //start with entire list
        BasicList<BasicPickerData<E>> output = new();
        firstList.ForEach(items =>
        {
            if (AlreadyTaken(items!.EnumValue!) == false)
            {
                output.Add(items);
            }
        });
        return output;
    }
    private bool AlreadyTaken(E color)
    {
        return PlayerList!.Any(x => x.Color!.Equals(color));
    }
    public void LoadColors()
    {
        var tempList = ColorsLeft();
        ItemList.ReplaceRange(tempList);
        if (ItemList.Count == 0)
        {
            throw new CustomBasicException("There are no pieces.  Should have already continued to the next step of the game");
        }
        UnselectAll();
    }
    public BoardGamesColorPicker(CommandContainer command, IEnumListClass<E> choice) : base(command, choice) { }
}