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
    partial void CreateCommands(CommandContainer command);
    public bool CanRoll => MainGame.SaveRoot.DiceList.Count == 0;
    [Command(EnumCommandCategory.Game)]
    public async Task RollAsync()
    {
        await MainGame.RollAsync();
    }
    public bool CanChoseStartPiece(EnumColorChoice color)
    {
        if (SorryDicedGameGameContainer.SelectedDice is null)
        {
            return false; //because you did not roll or choose dice yet.
        }
        if (MainGame.SaveRoot.BoardList.Any(x => x.Color == color && x.At == EnumBoardCategory.Start))
        {
            if (SorryDicedGameGameContainer.SelectedDice.Category == EnumDiceCategory.Wild)
            {
                return true;
            }
            if (SorryDicedGameGameContainer.SelectedDice.Category != EnumDiceCategory.Color)
            {
                return false;
            }
            if (SorryDicedGameGameContainer.SelectedDice.Color == color)
            {
                return true;
            }
            return false;
        }
        return false;
    }
    [Command(EnumCommandCategory.Game)]
    public async Task ChoseStartPiece(EnumColorChoice color)
    {
        if (MainGame.BasicData.MultiPlayer)
        {
            await MainGame.Network!.SendAllAsync("start", color);
        }
        await MainGame.StartPieceAsync(color);
    }
    public static bool CanSelectDice(SorryDiceModel dice) => dice.IsEnabled; //not sure if i can do static or not (?)
    [Command(EnumCommandCategory.Game)]
    public async Task SelectDiceAsync(SorryDiceModel dice)
    {
        int index = MainGame.SaveRoot.DiceList.IndexOf(dice);
        if (index == -1)
        {
            throw new CustomBasicException("Not found on the dice list");
        }
        if (MainGame.BasicData.MultiPlayer)
        {
            await MainGame.Network!.SendAllAsync("selectdice", index);
        }
        await MainGame.SelectUnselectDiceAsync(index);
    }
    public bool CanHome(SorryDicedGamePlayerItem player)
    {
        if (SorryDicedGameGameContainer.SelectedDice is null)
        {
            return false; //because you did not roll or choose dice yet.
        }
        if (SorryDicedGameGameContainer.SelectedDice.Category != EnumDiceCategory.Sorry)
        {
            return false; //because you have to roll a sorry for this.
        }
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
            return true;
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
    public static bool CanWaiting(WaitingModel wait)
    {
        if (SorryDicedGameGameContainer.SelectedDice is null)
        {
            return false; //because you did not roll or choose dice yet.
        }
        if (SorryDicedGameGameContainer.SelectedDice.Category == EnumDiceCategory.Slide)
        {
            return true; //i think.
        }
        if (SorryDicedGameGameContainer.SelectedDice.Category == EnumDiceCategory.Sorry)
        {
            return false;
        }
        if (SorryDicedGameGameContainer.SelectedDice.Category == EnumDiceCategory.Wild)
        {
            return true;
        }
        if (SorryDicedGameGameContainer.SelectedDice.Category != EnumDiceCategory.Color)
        {
            return false;
        }
        return SorryDicedGameGameContainer.SelectedDice.Color == wait.ColorUsed;
    }
    [Command(EnumCommandCategory.Game)]
    public async Task WaitingAsync(WaitingModel wait)
    {
        if (SorryDicedGameGameContainer.SelectedDice!.Category == EnumDiceCategory.Slide)
        {
            await SlideAsync(wait);
            return;
        }
        var player = MainGame.PlayerList.GetWhoPlayer();
        if (player.Id == wait.Player)
        {
            _toast.ShowUserErrorToast("Cannot click on your own wait area when there is no slide");
            return;
        }
        var chosen = MainGame.PlayerList[wait.Player];
        var rets = MainGame.SaveRoot.BoardList.Any(x => x.Color == wait.ColorUsed && x.At == EnumBoardCategory.Start);
        if (rets)
        {
            _toast.ShowUserErrorToast("Cannot click on wait area because must use one from start first");
            return;
        }
        rets = MainGame.SaveRoot.BoardList.Any(x => x.Color == wait.ColorUsed && x.PlayerOwned == wait.Player && x.At == EnumBoardCategory.Waiting);
        if (rets == false)
        {
            _toast.ShowUserErrorToast($"{chosen.NickName} has no color {wait.ColorUsed} in their waiting area");
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
        var player = MainGame.PlayerList[wait.Player];
        if (wait.ColorUsed == player.SlideColor)
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
    public override bool CanEndTurn()
    {
        return MainGame.SaveRoot.DiceList.Count > 0 && MainGame.SaveRoot.DiceList.All(x => x.IsEnabled == false);
    }
}