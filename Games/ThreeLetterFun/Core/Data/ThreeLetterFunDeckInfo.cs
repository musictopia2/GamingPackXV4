namespace ThreeLetterFun.Core.Data;
public class ThreeLetterFunDeckInfo : IDeckCount
{
    internal BasicList<SavedCard> PrivateSavedList { get; set; } = new();
    internal void InitCards()
    {
        ThreeLetterFunSaveInfo saveRoot = aa1.Resolver!.Resolve<ThreeLetterFunSaveInfo>();
        GlobalHelpers global = aa1.Resolver!.Resolve<GlobalHelpers>();
        if (saveRoot.Level == EnumLevel.None)
        {
            throw new CustomBasicException("Must choose the level before you can initialize the cards");
        }
        PrivateSavedList = global.SavedCardList!.Where(items => items.Level == saveRoot.Level).ToBasicList();
    }
    public int GetDeckCount()
    {
        InitCards();
        return PrivateSavedList.Count;
    }
}