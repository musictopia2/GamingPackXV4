namespace MonopolyDicedGame.Core.Data;
public class OwnedModel
{
    public int Group { get; set; } //this means you owned the group.
    public EnumBasicType UsedOn { get; set; } = EnumBasicType.None;
    public EnumUtilityType Utility { get; set; } = EnumUtilityType.None;
    //public int Index { get; set; } //this will tell whether its water or utility.
    public bool WasChance { get; set; } //has to determine whether the one you used was a chance or not.
}