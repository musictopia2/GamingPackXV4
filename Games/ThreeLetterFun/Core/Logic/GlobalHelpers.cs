namespace ThreeLetterFun.Core.Logic;
[SingletonGame]
[AutoReset]
public class GlobalHelpers
{
    public BasicList<ThreeLetterFunWordModel>? SavedWords;
    public BasicList<char>? SavedTiles { get; set; }
    public BasicList<SavedCard>? SavedCardList { get; set; }
    internal CustomStopWatchCP? Stops { get; set; }
    private readonly BasicData _basicData;
    private readonly ThreeLetterFunVMData _model;
    private readonly CommandContainer _command;
    private readonly ISpellingLogic _logic;
    private bool _processing;
    #region "Delegates to stop overflow problems"
    public Func<bool, Task>? SelfGiveUpAsync { get; set; } //keep this is fine.
    #endregion
    public GlobalHelpers(BasicData basicData, ThreeLetterFunVMData model, CommandContainer command, ISpellingLogic logic)
    {
        _processing = true;
        _basicData = basicData;
        _model = model;
        _command = command;
        _logic = logic;
        LoadItems();
        LoadSavedWordsAsync();
    }
    public async void LoadSavedWordsAsync()
    {
        SavedCardList = await Resources.SavedCardList.GetResourceAsync<BasicList<SavedCard>>();
        var firstList = await Resources.SavedTileList.GetResourceAsync<BasicList<SavedTile>>();
        SavedTiles = PrivateGetTiles(firstList);
        SavedWords = await GetSavedWordsAsync();
        _processing = false;
    }
    public async Task WaitAsync()
    {
        do
        {
            if (_processing == false)
            {
                return;
            }
            await Task.Delay(20);
        } while (true);
    }
    private void LoadItems()
    {
        if (_basicData.MultiPlayer)
        {
            Stops = new CustomStopWatchCP();
            Stops.TimeUp += Stops_TimeUp;
            Stops.MaxTime = 120000;
        }
        _model.TileBoard1 = new TileBoardObservable(_command);
    }
    private async void Stops_TimeUp()
    {
        if (SelfGiveUpAsync == null)
        {
            throw new CustomBasicException("Nobody is handling the self giving up.  Rethink");
        }
        await SelfGiveUpAsync.Invoke(false);
    }
    private async Task<BasicList<ThreeLetterFunWordModel>> GetSavedWordsAsync()
    {
        BasicList<ThreeLetterFunWordModel> output = new();
        var firstList = await _logic.GetWordsAsync(null, 3);
        firstList.ForEach(firstWord =>
        {
            ThreeLetterFunWordModel newWord = new();
            newWord.Word = firstWord.Word;
            newWord.IsEasy = firstWord.Difficulty == EnumDifficulty.Easy;
            output.Add(newWord);
        });
        return output;
    }
    private static BasicList<char> PrivateGetTiles(BasicList<SavedTile> thisList)
    {
        BasicList<char> finList = new();
        thisList.ForEach(thisTile =>
        {
            int x;
            var loopTo = thisTile.HowMany;
            for (x = 1; x <= loopTo; x++)
            {
                finList.Add(thisTile.Letter.Single());
            }
        });
        return finList;
    }
    public BasicList<TileInformation> GetTiles()
    {
        var thisList = Enumerable.Range(1, 100).ToBasicList();
        return thisList.Select(items => GetTile(items)).ToBasicList();
    }
    public TileInformation GetTile(int deck)
    {
        if (SavedTiles!.Count == 0)
        {
            throw new CustomBasicException("No saved tiles");
        }
        var tempTile = SavedTiles[deck - 1];
        TileInformation output = new();
        output.Deck = deck;
        output.Letter = tempTile;
        return output;
    }
    public void PauseContinueTimer()
    {
        if (_basicData.MultiPlayer == false)
        {
            return; //because multiplayer has no timer.
        }
        if (Stops!.IsRunning == true)
        {
            Stops.PauseTimer();
        }
        else
        {
            Stops.ContinueTimer();
        }
    }
}
