namespace BladesOfSteel.Core.Logic;
[SingletonGame]
[AutoReset]
public class FaceoffProcesses : IFaceoffProcesses
{
    private readonly BladesOfSteelGameContainer _gameContainer;
    private readonly BladesOfSteelVMData _model;
    private readonly BladesOfSteelScreenDelegates _delegates;
    private readonly IToast _toast;
    public FaceoffProcesses(BladesOfSteelGameContainer gameContainer,
        BladesOfSteelVMData model, 
        BladesOfSteelScreenDelegates delegates,
        IToast toast
        )
    {
        _gameContainer = gameContainer;
        _model = model;
        _delegates = delegates;
        _toast = toast;
    }
    async Task IFaceoffProcesses.FaceOffCardAsync(RegularSimpleCard card)
    {
        _gameContainer.SingleInfo!.FaceOff = card;
        if (_gameContainer.SingleInfo.PlayerCategory == EnumPlayerCategory.Self)
        {
            _model!.YourFaceOffCard!.AddCard(card);
        }
        else
        {
            _model!.OpponentFaceOffCard!.AddCard(card);
        }
        _gameContainer.Command.UpdateAll(); //try this way.
        if (_gameContainer.PlayerList!.Any(items => items.FaceOff == null))
        {
            await _gameContainer.EndTurnAsync!.Invoke();
            return;
        }
        await AnalyzeFaceOffAsync();
    }
    private async Task AnalyzeFaceOffAsync()
    {
        int tempTurn = WhoWonFaceOff();
        if (tempTurn == 0)
        {
            _toast.ShowInfoToast("There was a tie during the faceoff.  Therefore; the faceoff is being done again");
            await Task.Delay(2000);
            ClearFaceOff();
            await _gameContainer.EndTurnAsync!.Invoke();
            return;
        }
        _gameContainer.WhoTurn = tempTurn;
        _gameContainer.SingleInfo = _gameContainer.PlayerList!.GetWhoPlayer();
        _toast.ShowInfoToast($"{_gameContainer.SingleInfo.NickName} has won the face off");
        await Task.Delay(2000);
        ClearFaceOff();
        _gameContainer.SaveRoot!.IsFaceOff = false;
        if (_delegates.LoadMainGameAsync == null)
        {
            throw new CustomBasicException("Nobody is handling the load main screen.  Rethink");
        }
        await _delegates.LoadMainGameAsync.Invoke();
        await _gameContainer.StartNewTurnAsync!.Invoke();
    }
    private int WhoWonFaceOff()
    {
        if (_gameContainer.Test!.DoubleCheck == true)
        {
            return _gameContainer.PlayerList!.First(items => items.PlayerCategory == EnumPlayerCategory.Self).Id;
        }
        if (_gameContainer.PlayerList!.First().FaceOff!.Value > _gameContainer.PlayerList!.Last().FaceOff!.Value)
        {
            return 1;
        }
        if (_gameContainer.PlayerList!.Last().FaceOff!.Value > _gameContainer.PlayerList!.First().FaceOff!.Value)
        {
            return 2;
        }
        return 0;
    }
    private void ClearFaceOff()
    {
        _gameContainer.PlayerList!.First().FaceOff = null;
        _gameContainer.PlayerList!.Last().FaceOff = null;
    }
}