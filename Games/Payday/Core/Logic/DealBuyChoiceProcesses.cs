namespace Payday.Core.Logic;
[SingletonGame]
[AutoReset]
public class DealBuyChoiceProcesses(PaydayGameContainer gameContainer, PaydayVMData model, GameBoardProcesses gameBoard) : IDealBuyChoiceProcesses
{
    async Task IDealBuyChoiceProcesses.ChoseDealOrBuyAsync()
    {
        await gameContainer.StartProcessPopUpAsync(model);
        gameContainer.SaveRoot.OutCards.Add(gameContainer.SaveRoot.CurrentMail!);
        gameContainer.SaveRoot.CurrentMail = new MailCard();
        int nextSpace;
        if (model.PopUpChosen == "Buy")
        {
            nextSpace = gameBoard!.NextBuyerSpace();
        }
        else
        {
            nextSpace = gameBoard!.NextDealSpace();
        }
        if (gameContainer.BasicData!.MultiPlayer == false)
        {
            await gameBoard.AnimateMoveAsync(nextSpace);
            return;
        }
        if (gameContainer.SingleInfo!.CanSendMessage(gameContainer.BasicData) == true)
        {
            await gameBoard.AnimateMoveAsync(nextSpace);
            return;
        }
        gameContainer.Network!.IsEnabled = true;
    }
}