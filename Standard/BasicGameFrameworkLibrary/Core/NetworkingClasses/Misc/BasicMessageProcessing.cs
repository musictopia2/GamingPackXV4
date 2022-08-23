namespace BasicGameFrameworkLibrary.Core.NetworkingClasses.Misc;
public class BasicMessageProcessing : IMessageProcessor
{
    private readonly IGamePackageResolver _thisContainer;
    private readonly TestOptions _thisTest;
    private readonly ISystemError _error;
    private readonly IToast _toast;
    public BasicMessageProcessing(IGamePackageResolver thisContainer,
        TestOptions thisTest,
        ISystemError error,
        IToast toast
        ) //we need the isimpleui for the errors if any.
    {
        _thisContainer = thisContainer;
        _thisTest = thisTest;
        _error = error;
        _toast = toast;
    }
    public Task ProcessErrorAsync(string errorMessage) //done.
    {
        _error.ShowSystemError(errorMessage);
        return Task.CompletedTask;
    }
    public async Task ProcessMessageAsync(SentMessage thisMessage)
    {
        //if there are any exceptions, rethink.
        if (thisMessage.Status != "newgame" && thisMessage.Status != "restoregame" && InProgressHelpers.Reconnecting)
        {
            IGameNetwork network = _thisContainer.Resolve<IGameNetwork>();
            network.IsEnabled = true;
            return; //can't process because you are reconnecting.
        }
        if (InProgressHelpers.Reconnecting)
        {
            if (InProgressHelpers.MoveInProgress)
            {
                _toast.ShowInfoToast("Move is in progress.  Waiting to finish move to receive message");
            }
            //if reconnecting, then has to do this loop part to finish up.
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
                    IReadyNM thisReady = _thisContainer.Resolve<IReadyNM>();
                    await thisReady.ProcessReadyAsync(thisMessage.Body);
                    break;
                case "loadgame":
                    ILoadGameNM thisLoad = _thisContainer.Resolve<ILoadGameNM>();
                    await thisLoad.LoadGameAsync(thisMessage.Body);
                    break;
                case "savedgame":
                    _error.ShowSystemError("savedgame is obsolete.  Try to use loadgame now");
                    break;
                case "reshuffledcards":
                    IReshuffledCardsNM thisReshuffle = _thisContainer.Resolve<IReshuffledCardsNM>();
                    await thisReshuffle.ReshuffledCardsReceived(thisMessage.Body);
                    break;
                case "newgame":
                    INewGameNM thisGame = _thisContainer.Resolve<INewGameNM>();
                    InProgressHelpers.Reconnecting = false;
                    await thisGame.NewGameReceivedAsync(thisMessage.Body);
                    break;
                case "restoregame":
                    IRestoreNM thisRestore = _thisContainer.Resolve<IRestoreNM>();
                    InProgressHelpers.Reconnecting = false;
                    await thisRestore.RestoreMessageAsync(thisMessage.Body);
                    break;
                case "endturn":
                    IEndTurnNM thisEnd = _thisContainer.Resolve<IEndTurnNM>();
                    await thisEnd.EndTurnReceivedAsync(thisMessage.Body);
                    break;
                case "drawcard":
                    IDrawCardNM thisDrawCard = _thisContainer.Resolve<IDrawCardNM>();
                    await thisDrawCard.DrawCardReceivedAsync(thisMessage.Body);
                    break;
                case "chosepiece":
                    IChoosePieceNM thisPiece = _thisContainer.Resolve<IChoosePieceNM>();
                    await thisPiece.ChoosePieceReceivedAsync(thisMessage.Body);
                    break;
                case "pickup":
                    IPickUpNM thisPick = _thisContainer.Resolve<IPickUpNM>();
                    await thisPick.PickUpReceivedAsync(thisMessage.Body);
                    break;
                case "discard":
                    IDiscardNM thisDicard = _thisContainer.Resolve<IDiscardNM>();
                    await thisDicard.DiscardReceivedAsync(thisMessage.Body);
                    break;
                case "move":
                    IMoveNM thisMove = _thisContainer.Resolve<IMoveNM>();
                    await thisMove.MoveReceivedAsync(thisMessage.Body);
                    break;
                case "rolled":
                    IRolledNM thisRoll = _thisContainer.Resolve<IRolledNM>();
                    await thisRoll.RollReceivedAsync(thisMessage.Body);
                    break;
                case "processhold":
                    IProcessHoldNM thisHold = _thisContainer.Resolve<IProcessHoldNM>();
                    await thisHold.ProcessHoldReceivedAsync(int.Parse(thisMessage.Body)); //if you send something not int, will get casting errors.
                    break;
                case "selectdice":
                    ISelectDiceNM thisDice = _thisContainer.Resolve<ISelectDiceNM>();
                    await thisDice.SelectDiceReceivedAsync(int.Parse(thisMessage.Body));
                    break;
                case "drew":
                    _error.ShowSystemError("drew is obsolete.  Try drawdomino");
                    break;
                case "drawdomino":
                    IDrewDominoNM thisDomino = _thisContainer.Resolve<IDrewDominoNM>();
                    await thisDomino.DrewDominoReceivedAsync(int.Parse(thisMessage.Body));
                    break;
                case "playdomino":
                    IPlayDominoNM playDomino = _thisContainer.Resolve<IPlayDominoNM>();
                    await playDomino.PlayDominoMessageAsync(int.Parse(thisMessage.Body));
                    break;
                case "trickplay":
                    ITrickNM thisTrick = _thisContainer.Resolve<ITrickNM>();
                    await thisTrick.TrickPlayReceivedAsync(int.Parse(thisMessage.Body));
                    break;
                default:
                    IMiscDataNM thisMisc = _thisContainer.Resolve<IMiscDataNM>();
                    await thisMisc.MiscDataReceived(thisMessage.Status, thisMessage.Body);
                    break;
            }
            InProgressHelpers.MoveInProgress = false;
        }
        catch (Exception ex)
        {
            if (_thisTest.ShowErrorMessageBoxes == true)
            {
                _error.ShowSystemError(ex.Message); //which will close out but at least you get a messagebox.
            }
            else
            {
                throw;
            }
        }
    }
}