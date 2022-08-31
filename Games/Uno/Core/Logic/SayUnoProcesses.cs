namespace Uno.Core.Logic;
[SingletonGame]
[AutoReset]
public class SayUnoProcesses : ISayUnoProcesses
{
    private readonly UnoGameContainer _gameContainer;
    private readonly IToast _toast;
    public SayUnoProcesses(UnoGameContainer gameContainer, IToast toast)
    {
        _gameContainer = gameContainer;
        _toast = toast;
    }
    async Task ISayUnoProcesses.ProcessUnoAsync(bool saiduno)
    {
        _gameContainer.AlreadyUno = true;
        if (_gameContainer.SingleInfo!.CanSendMessage(_gameContainer.BasicData!) == true)
        {
            await _gameContainer.Network!.SendAllAsync("uno", saiduno);
        }
        if (saiduno == false)
        {
            if (_gameContainer.SingleInfo!.PlayerCategory == EnumPlayerCategory.Self)
            {
                _toast.ShowWarningToast("You had one card left.  However, you did not say uno.  Therefore, you have to draw 2 cards");
            }
            _gameContainer.LeftToDraw = 2;
            await _gameContainer.DrawAsync!.Invoke();
            return;
        }
        if (_gameContainer.SingleInfo!.PlayerCategory != EnumPlayerCategory.Self)
        {
            _toast.ShowInfoToast($"Uno From {_gameContainer.SingleInfo.NickName}");
        }
        if (_gameContainer.DoFinishAsync == null)
        {
            throw new CustomBasicException("Nobody is handling the dofinishasync.  Rethink");
        }
        if (_gameContainer.CloseSaidUnoAsync == null)
        {
            throw new CustomBasicException("Nobody is closing uno screen.  Rethink");
        }
        await _gameContainer.CloseSaidUnoAsync.Invoke();
        await _gameContainer.DoFinishAsync.Invoke();
    }
}