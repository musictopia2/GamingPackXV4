namespace Opetong.Core.ViewModels;
[InstanceGame]
public partial class OpetongMainViewModel : BasicCardGamesVM<RegularRummyCard>
{
    private readonly OpetongMainGameClass _mainGame;
    private readonly OpetongVMData _model;
    private readonly IToast _toast;
    private readonly PrivateAutoResumeProcesses _privateAutoResume;
    private readonly OpetongGameContainer _gameContainer;
    public OpetongMainViewModel(CommandContainer commandContainer,
        OpetongMainGameClass mainGame,
        OpetongVMData viewModel,
        BasicData basicData,
        TestOptions test,
        IGamePackageResolver resolver,
        IEventAggregator aggregator,
        IToast toast,
        PrivateAutoResumeProcesses privateAutoResume,
        OpetongGameContainer gameContainer
        )
        : base(commandContainer, mainGame, viewModel, basicData, test, resolver, aggregator, toast)
    {
        _mainGame = mainGame;
        _model = viewModel;
        _toast = toast;
        _privateAutoResume = privateAutoResume;
        _gameContainer = gameContainer;
        var player = _mainGame.PlayerList.GetSelf();
        player.DoInit();
        _model.Deck1.NeverAutoDisable = true;
        _model.TempSets.Init(this);
        _model.TempSets.ClearBoard();
        PossibleAutoResume();
        _model.MainSets.SendEnableProcesses(this, () => false);
        _model.PlayerHand1.AutoSelect = EnumHandAutoType.SelectAsMany;
        _model.TempSets.SetClickedAsync = TempSets_SetClickedAsync;
        CreateCommands(commandContainer);
    }
    partial void CreateCommands(CommandContainer command);
    private void PossibleAutoResume()
    {
        if (_gameContainer.TempSets.Count > 0)
        {
            var player = _gameContainer.PlayerList!.GetSelf();
            bool hadAny = false;
            foreach (var item in _gameContainer.TempSets)
            {
                var current = _model.TempSets.SetList[item.SetNumber - 1];
                var cards = item.Cards.GetNewObjectListFromDeckList(_gameContainer.DeckList);
                DeckRegularDict<RegularRummyCard> toAdd = [];
                foreach (var card in cards)
                {
                    if (player.MainHandList.ObjectExist(card.Deck))
                    {
                        player.MainHandList.RemoveObjectByDeck(card.Deck);
                        player.AdditionalCards.Add(card); //if i remove from hand, must add to additional cards so sends to other players properly.
                        hadAny = true;
                        toAdd.Add(card);
                    }
                }
                current.AddCards(toAdd);
            }
            if (hadAny)
            {
                _model.TempSets.PublicCount();
            }
        }
    }
    private bool _isProcessing;
    private async Task TempSets_SetClickedAsync(int index)
    {
        if (_isProcessing == true)
        {
            return;
        }
        _isProcessing = true;
        var tempList = _model.PlayerHand1!.ListSelectedObjects(true);
        _model.TempSets!.AddCards(index, tempList);
        await _privateAutoResume.SaveStateAsync(_model);
        _isProcessing = false;
    }
    protected override bool CanEnableDeck()
    {
        return true;
    }
    protected override bool CanEnablePile1()
    {
        return false;
    }
    protected override Task ProcessDiscardClickedAsync()
    {
        throw new CustomBasicException("Discard should never be used this time.");
    }
    public override bool CanEnableAlways()
    {
        return true;
    }
    [Command(EnumCommandCategory.Game)]
    public async Task PlaySetAsync()
    {
        int nums = _mainGame!.FindValidSet();
        if (nums == 0)
        {
            _toast.ShowUserErrorToast("Sorry, there is no valid set here");
            return;
        }
        var thisCol = _model.TempSets.ObjectList(nums).ToRegularDeckDict();
        _model.TempSets.ClearBoard(nums);
        if (_mainGame.BasicData!.MultiPlayer)
        {
            var tempCol = thisCol.GetDeckListFromObjectList();
            await _mainGame.Network!.SendAllAsync("newset", tempCol);
        }
        await _mainGame.PlaySetAsync(thisCol);
    }
}