namespace Uno.Core.Logic;
[SingletonGame]
[AutoReset]
public class SayUnoProcesses(UnoGameContainer gameContainer, IToast toast) : ISayUnoProcesses
{
    async Task ISayUnoProcesses.ProcessUnoAsync(bool saiduno)
    {
        gameContainer.AlreadyUno = true;
        if (gameContainer.SingleInfo!.CanSendMessage(gameContainer.BasicData!) == true)
        {
            await gameContainer.Network!.SendAllAsync("uno", saiduno);
        }
        if (saiduno == false)
        {
            if (gameContainer.SingleInfo!.PlayerCategory == EnumPlayerCategory.Self)
            {
                toast.ShowWarningToast("You had one card left.  However, you did not say uno.  Therefore, you have to draw 2 cards");
            }
            gameContainer.LeftToDraw = 2;
            await gameContainer.DrawAsync!.Invoke();
            return;
        }
        if (gameContainer.SingleInfo!.PlayerCategory != EnumPlayerCategory.Self)
        {
            toast.ShowInfoToast($"Uno From {gameContainer.SingleInfo.NickName}");
        }
        if (gameContainer.DoFinishAsync == null)
        {
            throw new CustomBasicException("Nobody is handling the dofinishasync.  Rethink");
        }
        if (gameContainer.CloseSaidUnoAsync == null)
        {
            throw new CustomBasicException("Nobody is closing uno screen.  Rethink");
        }
        await gameContainer.CloseSaidUnoAsync.Invoke();
        await gameContainer.DoFinishAsync.Invoke();
    }
}