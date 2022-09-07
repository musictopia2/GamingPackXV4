namespace BasicGameFrameworkLibrary.Core.SpecializedGameTypes.TrickClasses;
public interface ITrickDummyHand<SU, TR>
    where SU : IFastEnumSimple
    where TR : ITrickCard<SU>, new()
{
    DeckRegularDict<TR> GetCurrentHandList();
    int CardSelected();
    void RemoveCard(int deck); 
}