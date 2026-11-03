namespace BladesOfSteel.Core.CustomPiles;
public class PlayerDefenseCP(CommandContainer command) : HandObservable<RegularSimpleCard>(command)
{
    private EnumPlayerCategory PlayerCategory { get; set; }
    protected override bool CanEverEnable()
    {
        return PlayerCategory == EnumPlayerCategory.Self;
    }
    public void LoadBoard(BladesOfSteelPlayerItem thisPlayer)
    {
        PlayerCategory = thisPlayer.PlayerCategory;
        ReplaceHandList(thisPlayer.DefenseList);
        Maximum = 3;
        AutoSelect = EnumHandAutoType.None;
        Text = thisPlayer.NickName + " Defense";
    }
    private void AddObjectToHand(RegularSimpleCard payLoad)
    {
        payLoad.Drew = false;
        if (PlayerCategory == EnumPlayerCategory.Self)
        {
            payLoad.IsUnknown = false;
        }
        else
        {
            payLoad.IsUnknown = true;
            payLoad.IsSelected = false;
        }
    }
    protected override void AfterPopulateObjects()
    {
        HandList.ForEach(AddObjectToHand);
    }
}