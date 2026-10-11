namespace BladesOfSteel.Core.Logic;
[SingletonGame]
[AutoReset]
public class FaceoffProcesses(BladesOfSteelGameContainer gameContainer,
    BladesOfSteelVMData model,
    BladesOfSteelScreenDelegates delegates,
    IToast toast
        ) : IFaceoffProcesses
{
    async Task IFaceoffProcesses.FaceOffCardAsync(RegularSimpleCard card)
    {
        gameContainer.SingleInfo!.FaceOff = card;
        if (gameContainer.SingleInfo.PlayerCategory == EnumPlayerCategory.Self)
        {
            model!.YourFaceOffCard!.AddCard(card);
        }
        else
        {
            model!.OpponentFaceOffCard!.AddCard(card);
        }
        if (gameContainer.PlayerList!.Any(items => items.FaceOff == null))
        {
            await gameContainer.EndTurnAsync!.Invoke();
            return;
        }
        await AnalyzeFaceOffAsync();
    }
    private async Task AnalyzeFaceOffAsync()
    {
        int tempTurn = WhoWonFaceOff();
        if (tempTurn == 0)
        {
            toast.ShowInfoToast("There was a tie during the faceoff.  Therefore; the faceoff is being done again");
            await Task.Delay(2000);
            ClearFaceOff();
            await gameContainer.EndTurnAsync!.Invoke();
            return;
        }
        gameContainer.WhoTurn = tempTurn;
        gameContainer.SingleInfo = gameContainer.PlayerList!.GetWhoPlayer();
        toast.ShowInfoToast($"{gameContainer.SingleInfo.NickName} has won the face off");
        await Task.Delay(2000);
        ClearFaceOff();
        gameContainer.SaveRoot!.IsFaceOff = false;
        if (delegates.LoadMainGameAsync == null)
        {
            throw new CustomBasicException("Nobody is handling the load main screen.  Rethink");
        }
        await delegates.LoadMainGameAsync.Invoke();
        await gameContainer.StartNewTurnAsync!.Invoke();
    }
    private int WhoWonFaceOff()
    {
        if (gameContainer.Test!.DoubleCheck == true)
        {
            return gameContainer.PlayerList!.First(items => items.PlayerCategory == EnumPlayerCategory.Self).Id;
        }
        if (gameContainer.PlayerList!.First().FaceOff!.Value > gameContainer.PlayerList!.Last().FaceOff!.Value)
        {
            return 1;
        }
        if (gameContainer.PlayerList!.Last().FaceOff!.Value > gameContainer.PlayerList!.First().FaceOff!.Value)
        {
            return 2;
        }
        return 0;
    }
    private void ClearFaceOff()
    {
        gameContainer.PlayerList!.First().FaceOff = null;
        gameContainer.PlayerList!.Last().FaceOff = null;
    }
}