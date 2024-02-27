namespace BasicGameFrameworkLibrary.Core.NetworkingClasses.Misc;
public class BasicMessageProcessing(IGamePackageResolver thisContainer,
    TestOptions thisTest,
    ISystemError error,
    IToast toast
        ) : IMessageProcessor
{
    public Task ProcessErrorAsync(string errorMessage) //done.
    {
        error.ShowSystemError(errorMessage);
        return Task.CompletedTask;
    }
    public async Task ProcessMessageAsync(SentMessage thisMessage)
    {
        if (thisMessage.Status == "newround")
        {
            if (GlobalDelegates.DeleteOldPrivateGames is not null)
            {
                await GlobalDelegates.DeleteOldPrivateGames.Invoke(thisMessage.Body);
            }
            IGameNetwork network = thisContainer.Resolve<IGameNetwork>();
            network.IsEnabled = true; //should be okay because am expecting more messages
            return;
        }
        if (thisMessage.Status != "newgame" && thisMessage.Status != "restoregame" && InProgressHelpers.Reconnecting)
        {
            IGameNetwork network = thisContainer.Resolve<IGameNetwork>();
            network.IsEnabled = true;
            return; //can't process because you are reconnecting.
        }
        if (InProgressHelpers.Reconnecting)
        {
            if (InProgressHelpers.MoveInProgress)
            {
                toast.ShowInfoToast("Move is in progress.  Waiting to finish move to receive message");
            }
            do
            {
                if (InProgressHelpers.MoveInProgress == false)
                {
                    break;
                }
                await Task.Delay(100);
            } while (true);
        }
        try
        {
            InProgressHelpers.MoveInProgress = true;
            switch (thisMessage.Status.ToLower())
            {
                case "ready":
                    IReadyNM thisReady = thisContainer.Resolve<IReadyNM>();
                    await thisReady.ProcessReadyAsync(thisMessage.Body);
                    break;
                case "loadgame":
                    ILoadGameNM thisLoad = thisContainer.Resolve<ILoadGameNM>();
                    await thisLoad.LoadGameAsync(thisMessage.Body);
                    break;
                case "savedgame":
                    error.ShowSystemError("savedgame is obsolete.  Try to use loadgame now");
                    break;
                case "reshuffledcards":
                    IReshuffledCardsNM thisReshuffle = thisContainer.Resolve<IReshuffledCardsNM>();
                    await thisReshuffle.ReshuffledCardsReceived(thisMessage.Body);
                    break;
                case "newgame":
                    INewGameNM thisGame = thisContainer.Resolve<INewGameNM>();
                    InProgressHelpers.Reconnecting = false;
                    await thisGame.NewGameReceivedAsync(thisMessage.Body);
                    break;
                case "restoregame":
                    IRestoreNM thisRestore = thisContainer.Resolve<IRestoreNM>();
                    InProgressHelpers.Reconnecting = false;
                    await thisRestore.RestoreMessageAsync(thisMessage.Body);
                    break;
                case "endturn":
                    IEndTurnNM thisEnd = thisContainer.Resolve<IEndTurnNM>();
                    await thisEnd.EndTurnReceivedAsync(thisMessage.Body);
                    break;
                case "drawcard":
                    IDrawCardNM thisDrawCard = thisContainer.Resolve<IDrawCardNM>();
                    await thisDrawCard.DrawCardReceivedAsync(thisMessage.Body);
                    break;
                case "chosepiece":
                    IChoosePieceNM thisPiece = thisContainer.Resolve<IChoosePieceNM>();
                    await thisPiece.ChoosePieceReceivedAsync(thisMessage.Body);
                    break;
                case "pickup":
                    IPickUpNM thisPick = thisContainer.Resolve<IPickUpNM>();
                    await thisPick.PickUpReceivedAsync(thisMessage.Body);
                    break;
                case "discard":
                    IDiscardNM thisDicard = thisContainer.Resolve<IDiscardNM>();
                    await thisDicard.DiscardReceivedAsync(thisMessage.Body);
                    break;
                case "move":
                    IMoveNM thisMove = thisContainer.Resolve<IMoveNM>();
                    await thisMove.MoveReceivedAsync(thisMessage.Body);
                    break;
                case "rolled":
                    IRolledNM thisRoll = thisContainer.Resolve<IRolledNM>();
                    await thisRoll.RollReceivedAsync(thisMessage.Body);
                    break;
                case "processhold":
                    IProcessHoldNM thisHold = thisContainer.Resolve<IProcessHoldNM>();
                    await thisHold.ProcessHoldReceivedAsync(int.Parse(thisMessage.Body)); //if you send something not int, will get casting errors.
                    break;
                case "selectdice":
                    ISelectDiceNM thisDice = thisContainer.Resolve<ISelectDiceNM>();
                    await thisDice.SelectDiceReceivedAsync(int.Parse(thisMessage.Body));
                    break;
                case "drew":
                    error.ShowSystemError("drew is obsolete.  Try drawdomino");
                    break;
                case "drawdomino":
                    IDrewDominoNM thisDomino = thisContainer.Resolve<IDrewDominoNM>();
                    await thisDomino.DrewDominoReceivedAsync(int.Parse(thisMessage.Body));
                    break;
                case "playdomino":
                    IPlayDominoNM playDomino = thisContainer.Resolve<IPlayDominoNM>();
                    await playDomino.PlayDominoMessageAsync(int.Parse(thisMessage.Body));
                    break;
                case "trickplay":
                    ITrickNM thisTrick = thisContainer.Resolve<ITrickNM>();
                    await thisTrick.TrickPlayReceivedAsync(int.Parse(thisMessage.Body));
                    break;
                default:
                    IMiscDataNM thisMisc = thisContainer.Resolve<IMiscDataNM>();
                    await thisMisc.MiscDataReceived(thisMessage.Status, thisMessage.Body);
                    break;
            }
            InProgressHelpers.MoveInProgress = false;
        }
        catch (Exception ex)
        {
            if (thisTest.ShowErrorMessageBoxes == true)
            {
                error.ShowSystemError(ex.Message); //which will close out but at least you get a messagebox.
            }
            else
            {
                throw;
            }
        }
    }
}