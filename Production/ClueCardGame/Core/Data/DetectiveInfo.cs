namespace ClueCardGame.Core.Data;
public class DetectiveInfo
{
    public string Name { get; set; } = "";
    public EnumCardType Category { get; set; }
    public bool IsChecked { get; set; }
    public bool WasGiven { get; set; } //so can have some automation.  obviously if new game, then wiped out.
    //if this is given, then should not be able to change it.   but you can manually check/uncheck other items.
    //if you want to do some manually, then may open another screen.

}