namespace ThreeLetterFun.Core.ViewModels;
[InstanceGame]
public partial class ThreeLetterFunMainViewModel : BasicMultiplayerMainVM
{
    public readonly ThreeLetterFunMainGameClass MainGame;
    private readonly BasicData _basicData;
    private readonly GiveUpClass _giveUp;
    public readonly GameBoard GameBoard;
    private readonly GlobalHelpers _global;
    private readonly IToast _toast;
    public ThreeLetterFunVMData VMData { get; set; }
    public ThreeLetterFunMainViewModel(CommandContainer commandContainer,
        ThreeLetterFunMainGameClass mainGame,
        ThreeLetterFunVMData viewModel,
        BasicData basicData,
        TestOptions test,
        IGamePackageResolver resolver,
        IEventAggregator aggregator,
        GiveUpClass giveUp,
        GameBoard board,
        GlobalHelpers global,
        IToast toast
        )
        : base(commandContainer, mainGame, basicData, test, resolver, aggregator)
    {
        MainGame = mainGame;
        _basicData = basicData;
        _giveUp = giveUp;
        GameBoard = board;
        _global = global;
        _toast = toast;
        VMData = viewModel;
        VMData.CalculateVisible = CalculateVisible;
        CreateCommands(commandContainer);
    }
    partial void CreateCommands(CommandContainer command);
    [Command(EnumCommandCategory.Game)]
    public async Task PlayAsync()
    {
        _global.PauseContinueTimer();
        if (_basicData.MultiPlayer == true && MainGame!.Network!.IsEnabled == true)
        {
            _toast.ShowUserErrorToast("Should not have enabled the network helpers since you had to take your turn.");
            return;
        }
        var thisCard = GameBoard.GetCompletedCard();
        if (thisCard == null)
        {
            _global.PauseContinueTimer();
            _toast.ShowUserErrorToast("You must pick the tiles before playing");
            return;
        }
        if (thisCard.IsValidWord() == false)
        {
            var thisWord = thisCard.GetWord();
            if (MainGame.SaveRoot!.Level == EnumLevel.Easy)
            {
                GameBoard.UnDo();
                _toast.ShowUserErrorToast($"{thisWord} is not a word or is too hard. Please try again");
                _global.PauseContinueTimer();
                return;
            }
            if (_basicData.MultiPlayer == false)
            {
                _toast.ShowWarningToast($"{thisWord} does not exist.  Therefore; its going to the next one");
                await _giveUp.SelfGiveUpAsync(true);
                return;
            }
            _toast.ShowWarningToast($"{thisWord} does not exist.  Therefore; waiting for other players to decide if they have a word");
            await _giveUp.SelfGiveUpAsync(true);
            return;
        }
        if (_basicData.MultiPlayer == true)
        {
            MainGame.SingleInfo = MainGame.PlayerList!.GetSelf();
            TempWord thisWord = new();
            thisWord.Player = MainGame.SingleInfo.Id;
            thisWord.CardUsed = thisCard.Deck;
            thisWord.TileList = MainGame.SingleInfo.TileList;
            if (thisWord.TileList.Count == 0)
            {
                throw new CustomBasicException("Must have tiles to form a word to send");
            }
            MainGame.SingleInfo.TimeToGetWord = (int)_global.Stops!.TimeTaken();
            _global.Stops.ManualStop(false);
            thisWord.TimeToGetWord = MainGame.SingleInfo.TimeToGetWord;
            if (MainGame.SingleInfo.TimeToGetWord == 0)
            {
                throw new CustomBasicException("Time Taken Cannot Be 0");
            }
            await MainGame.Network!.SendAllAsync("playword", thisWord);
            MainGame.SaveRoot!.PlayOrder.WhoTurn = MainGame.SingleInfo.Id;
        }
        await MainGame!.PlayWordAsync(thisCard.Deck);
    }
    [Command(EnumCommandCategory.Game)]
    public async Task GiveUpAsync()
    {
        await _giveUp.SelfGiveUpAsync(true);
    }
    [Command(EnumCommandCategory.Game)]
    public void TakeBack()
    {
        GameBoard.UnDo();
    }
    private void CalculateVisible()
    {
        if (VMData.CurrentCard != null)
        {
            GameBoard.Visible = false;
            return;
        }
        if (VMData.PlayerWon != "")
        {
            GameBoard.Visible = false;
            return;
        }
        GameBoard.Visible = true;
    }
}