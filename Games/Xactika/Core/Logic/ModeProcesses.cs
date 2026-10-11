namespace Xactika.Core.Logic;
[SingletonGame]
[AutoReset]
public class ModeProcesses(XactikaVMData model,
    XactikaDelegates delegates,
    XactikaGameContainer gameContainer,
    IBidProcesses bidProcesses,
    IShapeProcesses shapeProcesses
        ) : IModeProcesses
{
    public async Task EnableOptionsAsync()
    {
        await gameContainer.SaveStateAsync!.Invoke();

        if (delegates.LoadModeAsync == null)
        {
            throw new CustomBasicException("Nobody is loading the mode.  Rethink");
        }
        await delegates.LoadModeAsync();

        if (gameContainer.BasicData!.MultiPlayer)
        {
            if (gameContainer.BasicData.Client)
            {
                gameContainer.Network!.IsEnabled = true; //has to wait for host to choose game option.
                return;
            }
        }
        if (gameContainer.ShowHumanCanPlayAsync == null)
        {
            throw new CustomBasicException("Nobody is showing human can play.  Rethink");
        }
        await gameContainer.ShowHumanCanPlayAsync!.Invoke(); //hopefully this simple.
    }
    private void LoadBid()
    {
        if (gameContainer.LoadBidding == null)
        {
            throw new CustomBasicException("Nobody is loading the bidding screen.  Rethink");
        }
        gameContainer.LoadBidding.Invoke();
    }
    public async Task ProcessGameOptionChosenAsync(EnumGameMode optionChosen, bool doShow)
    {
        if (doShow)
        {
            model!.ModeChoose1!.SelectSpecificItem((int)optionChosen);
            if (gameContainer.Test!.NoAnimations == false)
            {
                await gameContainer.Delay!.DelaySeconds(.5);
            }
            gameContainer.SaveRoot!.GameMode = optionChosen;
        }
        else if (optionChosen != gameContainer.SaveRoot!.GameMode)
        {
            throw new CustomBasicException("Had to show");
        }
        if (delegates.CloseModeAsync == null)
        {
            throw new CustomBasicException("Nobody is closing the game mode.  Rethink");
        }
        await delegates.CloseModeAsync.Invoke();
        if (gameContainer.SaveRoot.GameStatus != EnumStatusList.ChooseGameType)
        {
            if (gameContainer.SaveRoot.GameStatus == EnumStatusList.Bidding)
            {
                model.TrickArea1!.Visible = false;
                LoadBid();
                await bidProcesses.BeginBiddingAsync();
                return;
            }
            if (gameContainer.SaveRoot.GameStatus == EnumStatusList.CallShape)
            {
                model!.ShapeChoose1!.Visible = true;
                await shapeProcesses.FirstCallShapeAsync();
                return;
            }
            if (gameContainer.SaveRoot.ClearTricks)
            {
                gameContainer.SaveRoot.ClearTricks = false;
                await bidProcesses.EndBidAsync();
                return;
            }
            if (gameContainer.SaveRoot.ShapeChosen != EnumShapes.None)
            {
                model!.ShapeChoose1!.ChoosePiece(gameContainer.SaveRoot.ShapeChosen);
            }
            gameContainer.SingleInfo = gameContainer.PlayerList!.GetWhoPlayer();
            if (gameContainer.ShowTurn == null)
            {
                throw new CustomBasicException("Nobody is showing turn.  Rethink");
            }
            gameContainer.ShowTurn();
            await gameContainer.ContinueTurnAsync!.Invoke();
            return;
        }
        if (optionChosen == EnumGameMode.ToBid)
        {
            gameContainer.SingleInfo = gameContainer.PlayerList!.GetWhoPlayer();
            gameContainer.SaveRoot.GameStatus = EnumStatusList.Bidding;
            LoadBid();
            await bidProcesses.PopulateBidAmountsAsync();
            return;
        }
        ((IAdvancedTrickProcesses)model.TrickArea1).LoadGame();
        model.TrickArea1.Visible = true; //try this too.
        await bidProcesses.EndBidAsync();
    }
}
