namespace BladesOfSteel.Core.CustomPiles;
public class PlayerAttackCP(CommandContainer command) : HandObservable<RegularSimpleCard>(command)
{
    private EnumPlayerCategory PlayerCategory { get; set; }
    protected override bool CanEverEnable()
    {
        return PlayerCategory == EnumPlayerCategory.Self;
    }
    public void LoadBoard(BladesOfSteelPlayerItem thisPlayer)
    {
        PlayerCategory = thisPlayer.PlayerCategory;
        ReplaceHandList(thisPlayer.AttackList);
        Maximum = 3;
        AutoSelect = EnumHandAutoType.None;
        Text = thisPlayer.NickName + " Attack";
    }
}