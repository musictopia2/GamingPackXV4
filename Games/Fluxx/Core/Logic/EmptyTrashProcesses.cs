namespace Fluxx.Core.Logic;
[SingletonGame]
[AutoReset]
public class EmptyTrashProcesses : IEmptyTrashProcesses
{
    private readonly FluxxGameContainer _gameContainer;
    private readonly FluxxVMData _model;
    private readonly IAnalyzeProcesses _processes;
    private readonly IToast _toast;
    public EmptyTrashProcesses(FluxxGameContainer gameContainer, FluxxVMData model, IAnalyzeProcesses processes, IToast toast)
    {
        _gameContainer = gameContainer;
        _model = model;
        _processes = processes;
        _toast = toast;
    }
    public async Task EmptyTrashAsync()
    {
        _toast.ShowInfoToast("Empty the trash was played.  Therefore; the cards are being reshuffled");
        if (_gameContainer.BasicData!.MultiPlayer && _gameContainer.SingleInfo!.CanSendMessage(_gameContainer.BasicData) == false)
        {
            _gameContainer.Network!.IsEnabled = true;
            return;
        }
        var thisList = _model.Pile1!.DiscardList();
        thisList.AddRange(_model.Deck1!.DeckList());
        thisList.ShuffleList();
        if (_gameContainer.BasicData.MultiPlayer)
        {
            await _gameContainer.Network!.SendAllAsync("emptytrash", thisList.GetDeckListFromObjectList());
        }
        await FinishEmptyTrashAsync(thisList);
    }
    public async Task FinishEmptyTrashAsync(IEnumerableDeck<FluxxCardInformation> cardList)
    {
        _model!.Deck1!.OriginalList(cardList);
        _model.Pile1!.CardsReshuffled();
        await _processes.AnalyzeQueAsync();
    }
}
