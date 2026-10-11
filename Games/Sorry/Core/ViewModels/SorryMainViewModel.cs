namespace Sorry.Core.ViewModels;
[InstanceGame]
public partial class SorryMainViewModel(CommandContainer commandContainer,
        SorryMainGameClass mainGame,
        SorryVMData model,
        BasicData basicData,
        TestOptions test,
        IGamePackageResolver resolver,
        IEventAggregator aggregator,
        SorryGameContainer gameContainer,
        GameBoardProcesses gameBoard) : BasicMultiplayerMainVM(commandContainer, mainGame, basicData, resolver, aggregator)
{
    public SorryVMData VMData { get; set; } = model;
    public override bool CanEndTurn()
    {
        bool rets = base.CanEndTurn();
        if (rets == false)
        {
            return false;
        }
        return !gameBoard.HasRequiredMove;
    }
    protected override void AdditionalInitialize()
    {
        gameContainer.SpaceClickedAsync = MakeMoveAsync;
        gameContainer.DrawClickAsync = DrawAsync;
        gameContainer.HomeClickedAsync = HomeAsync;
        CreateCommands(CommandContainer);
        base.AdditionalInitialize();
    }
    partial void CreateCommands(CommandContainer command);
    public bool DidDraw => mainGame.SaveRoot.DidDraw;
    private async Task MakeMoveAsync(int space)
    {

        if (test!.AdvancedTestOptions)
        {
            AdvancedPlacePiece(space);
            await mainGame.ContinueTurnAsync();
            //_command.StopExecuting();
            return;
        }



        if (gameBoard.IsValidMove(space) == false)
        {
            gameContainer.Command.StopExecuting(); //since command was not used now, this now has to be done.
            return;
        }
        if (gameContainer.CanSendMessage())
        {
            await gameContainer.Network!.SendMoveAsync(space);
        }
        await mainGame.MakeMoveAsync(space);
    }
    public int AdvancedSelectedPiece { get; set; }
    private void AdvancedPlacePiece(int space)
    {
        SorryPlayerItem player = mainGame.SingleInfo!;

        if (AdvancedSelectedPiece == 0)
        {
            if (!player.PieceList.Contains(space))
            {
                return;
            }

            AdvancedSelectedPiece = space;
            return;
        }
        if (!gameBoard.CanAdvancedPlace(player, space))
        {
            return;
        }
        player.PieceList.RemoveSpecificItem(AdvancedSelectedPiece);
        player.PieceList.Add(space);
        AdvancedSelectedPiece = 0;
    }
    private async Task DrawAsync()
    {
        if (gameContainer.CanSendMessage())
        {
            await gameContainer.Network!.SendDrawAsync();
        }
        await mainGame.DrawCardAsync();
    }
    private async Task HomeAsync(EnumColorChoice color)
    {
        if (gameBoard.CanGoHome(color) == false)
        {
            gameContainer.Command.StopExecuting();
            return;
        }
        if (gameContainer.CanSendMessage())
        {
            await gameContainer.Network!.SendMoveAsync(100);
        }
        await mainGame.MakeMoveAsync(100);
    }
    [Command(EnumCommandCategory.Game)]
    public async Task LockDrawCardAsync()
    {
        if (test.AdvancedTestOptions == false)
        {
            throw new CustomBasicException("Should only be able to lock draw card if advanced test options is enabled");
        }
        mainGame.SaveRoot.DidDraw = true;
        mainGame.SaveRoot.AdvancedCardPreview = false;
        await mainGame.ContinueTurnAsync();
    }
}