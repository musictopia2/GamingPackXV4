namespace ThreeLetterFun.Core.Data;
public class SavedCard
{
    public BasicList<char> CharacterList { get; set; } = new();
    public EnumLevel Level { get; set; }
}