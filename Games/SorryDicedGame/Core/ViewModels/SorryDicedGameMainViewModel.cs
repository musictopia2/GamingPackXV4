namespace SorryDicedGame.Core.ViewModels;
[InstanceGame]
public partial class SorryDicedGameMainViewModel : BasicMultiplayerMainVM
{
    public readonly SorryDicedGameMainGameClass MainGame; //if we don't need, delete.
    private readonly IToast _toast;
    public SorryDicedGameVMData VMData { get; set; }
    public SorryDicedGameMainViewModel(CommandContainer commandContainer,
        SorryDicedGameMainGameClass mainGame,
        BasicData basicData,
        TestOptions test,
        IGamePackageResolver resolver,
        IEventAggregator aggregator,
        SorryDicedGameVMData data,
        IToast toast
        )
        : base(commandContainer, mainGame, basicData, test, resolver, aggregator)
    {
        MainGame = mainGame;
        VMData = data;
        _toast = toast;
        CreateCommands(commandContainer);
    }
    //anything else needed is here.
    partial void CreateCommands(CommandContainer command);

    [Command(EnumCommandCategory.Game)]
    public async Task RollAsync()
    {
        //will be a command now to roll the dice (getting closer to reals).

        await MainGame.RollAsync();
    }
    public bool CanChoseStartPiece(EnumColorChoice color)
    {
        if (MainGame.SaveRoot.BoardList.Any(x => x.Color == color && x.At == EnumBoardCategory.Start))
        {
            return true;
        }
        return false;
    }
    [Command(EnumCommandCategory.Game)]
    public async Task ChoseStartPiece(EnumColorChoice color)
    {


        //the view model handles validations.
        if (MainGame.BasicData.MultiPlayer)
        {
            //send to other players.
            await MainGame.Network!.SendAllAsync("start", color);
        }
        await MainGame.StartPieceAsync(color);
        //_toast.ShowInfoToast(color.ToString());
        //await Task.Delay(1);
    }
    public static bool CanSelectDice(SorryDiceModel dice) => dice.IsEnabled; //not sure if i can do static or not (?)
    [Command(EnumCommandCategory.Game)]
    public async Task SelectDiceAsync(SorryDiceModel dice)
    {
        await Task.Delay(1);
        if (dice.IsSelected)
        {
            dice.IsSelected = false;
            return;
        }
        foreach (var item in MainGame.SaveRoot.DiceList)
        {
            item.IsSelected = false;
        }
        dice.IsSelected = true;
    }
    public bool CanHome(SorryDicedGamePlayerItem player)
    {
        if (player.PlayerCategory != EnumPlayerCategory.Self)
        {
            return true; //for now. can change later.
        }
        if (MainGame.BasicData.MultiPlayer == false)
        {
            if (player.Id == MainGame.WhoTurn)
            {
                return false;
            }
            return true; //for now.  can change later.
        }
        return false;
    }
    [Command(EnumCommandCategory.Game)]
    public async Task HomeAsync(SorryDicedGamePlayerItem player)
    {
        if (MainGame.SaveRoot.BoardList.Any(x => x.PlayerOwned == player.Id && x.At == EnumBoardCategory.Home) == false)
        {
            _toast.ShowUserErrorToast("This player has none at home");
            return;
        }
        if (MainGame.BasicData.MultiPlayer)
        {
            await MainGame.Network!.SendAllAsync("sorry", player.Id);
        }
        await MainGame.SorryAsync(player);
    }
    [Command(EnumCommandCategory.Game)]
    public async Task WaitingAsync(WaitingModel wait)
    {

        //for now, will always slide.
        //later will do something else.
        bool slide = false;
        if (slide)
        {
            await SlideAsync(wait);
            return;
        }
        var player = MainGame.PlayerList.GetWhoPlayer();
        if (player.Id == wait.Player!.Id)
        {
            _toast.ShowUserErrorToast("Cannot click on your own wait area when there is no slide");
            return;
        }
        //if there is one of that color at start, must use that one first.
        var rets = MainGame.SaveRoot.BoardList.Any(x => x.Color == wait.ColorUsed && x.At == EnumBoardCategory.Start);
        if (rets)
        {
            _toast.ShowUserErrorToast("Cannot click on wait area because must use one from start first");
            return;
        }
        rets = MainGame.SaveRoot.BoardList.Any(x => x.Color == wait.ColorUsed && x.PlayerOwned == wait.Player.Id && x.At == EnumBoardCategory.Waiting);
        if (rets == false)
        {
            _toast.ShowUserErrorToast($"{wait.Player.NickName} has no color {wait.ColorUsed} in their waiting area");
            return;
        }
        if (MainGame.BasicData.MultiPlayer)
        {
            await MainGame.Network!.SendAllAsync("wait", wait);
        }
        await MainGame.WaitAsync(wait);
    }
    private async Task SlideAsync(WaitingModel wait)
    {
        if (wait.ColorUsed == wait.Player!.SlideColor)
        {
            _toast.ShowUserErrorToast("Must choose a different slide color");
            return;
        }
        if (MainGame.BasicData.MultiPlayer)
        {
            await MainGame.Network!.SendAllAsync("slide", wait);
        }
        await MainGame.SlideAsync(wait);
    }

}