namespace BasicGameFrameworkLibrary.Core.BasicDrawables.Interfaces;

public interface IDeckShuffler<D> : IDeckLookUp<D>, IDeckCount where D : IDeckObject, new()
{
    bool NeedsToRedo { get; set; }
    Task<DeckRegularDict<D>> GetListFromJsonAsync(string jsonData);
    void ClearObjects();
    void OrderedObjects();
    void ShuffleObjects();
    void ReshuffleFirstObjects(IDeckDict<D> thisList, int startAt, int endAt);
    void PutCardOnTop(int deck);
}