namespace DominosMexicanTrain.Core.ViewModels;
[InstanceGame]
public partial class DominosMexicanTrainMainViewModel : DominoGamesVM<MexicanDomino>
{
    private readonly DominosMexicanTrainMainGameClass _mainGame; //if we don't need, delete.
    public DominosMexicanTrainVMData VMData { get; set; }
    private readonly TestOptions _test;
    private readonly IToast _toast;
    public DominosMexicanTrainMainViewModel(CommandContainer commandContainer,
            DominosMexicanTrainMainGameClass mainGame,
            DominosMexicanTrainVMData viewModel,
            BasicData basicData,
            TestOptions test,
            IGamePackageResolver resolver,
            IEventAggregator aggregator,
            IToast toast
        )
        : base(commandContainer, mainGame, viewModel, basicData, test, resolver, aggregator)
    {
        _mainGame = mainGame;
        VMData = viewModel;
        _test = test;
        _toast = toast;
        VMData.PrivateTrainObjectClickedAsync = PrivateTrain1_ObjectClickedAsync;
        VMData.PrivateTrainBoardClickedAsync = PrivateTrain1_BoardClickedAsync;
        CreateCommands(commandContainer);
    }
    partial void CreateCommands(CommandContainer command);
    public HandObservable<MexicanDomino> PlayerHand => VMData.PlayerHand1;
    public HandObservable<MexicanDomino> PrivateTrain => VMData.PrivateTrain1;
    public DominosBoneYardClass<MexicanDomino> BoneYard => VMData.BoneYard;
    public TrainStationBoardProcesses GameBoard => VMData.TrainStation1;
    public PlayerCollection<DominosMexicanTrainPlayerItem> GetPlayerList => _mainGame.SaveRoot.PlayerList;
    public override bool CanEndTurn()
    {
        if (_test.AllowAnyMove)
        {
            return false;
        }
        bool sats = _mainGame!.ForceSatisfy();
        if (VMData.BoneYard!.IsEnabled == false && sats == false)
        {
            return true;
        }
        if (VMData.BoneYard.HasDrawn() == false)
        {
            return false;
        }
        return !sats;
    }
    private async Task PrivateTrain1_ObjectClickedAsync(MexicanDomino payLoad, int index)
    {
        int decks = VMData.PlayerHand1!.ObjectSelected();
        if (decks > 0)
        {
            PutBackHand(decks);
            return;
        }
        if (payLoad.IsSelected == true)
        {
            payLoad.IsSelected = false;
            return;
        }
        VMData.PrivateTrain1!.UnselectAllObjects();
        payLoad.IsSelected = true;
        await Task.CompletedTask;
    }
    private async Task PrivateTrain1_BoardClickedAsync()
    {
        int decks = VMData.PlayerHand1!.ObjectSelected();
        if (decks > 0)
        {
            PutBackHand(decks);
        }
        await Task.CompletedTask;
    }
    public async Task TrainClickedAsync(int index)
    {
        if (CommandContainer.IsExecuting)
        {
            return; //has to do manually.
        }
        if (VMData.TrainStation1!.CanSelectSpace(index) == false)
        {
            return;
        }
        CommandContainer.StartExecuting();
        int decks = DominoSelected(out bool train);
        if (decks == 0)
        {
            _toast.ShowUserErrorToast("Sorry, must have one domino selected to put on the pile");
            CommandContainer.StopExecuting();
            CommandContainer.ManualReport();
            return;
        }
        MexicanDomino thisDomino;
        if (train)
        {
            thisDomino = VMData.PrivateTrain1!.HandList.GetSpecificItem(decks);
        }
        else
        {
            thisDomino = VMData.PlayerHand1.HandList.GetSpecificItem(decks);
        }
        if (VMData.TrainStation1!.CanPlacePiece(thisDomino, index) == false)
        {
            _toast.ShowUserErrorToast("Illegal Move");
            CommandContainer.StopExecuting();
            return;
        }
        if (_mainGame.BasicData!.MultiPlayer)
        {
            SendPlay output = new();
            output.Deck = decks;
            output.Section = index;
            await _mainGame.Network!.SendAllAsync("dominoplayed", output);
        }
        if (train)
        {
            _mainGame!.SingleInfo!.LongestTrainList.RemoveObjectByDeck(decks);
            VMData.PrivateTrain1!.HandList.RemoveObjectByDeck(decks);
        }
        else
        {
            _mainGame!.SingleInfo!.MainHandList.RemoveObjectByDeck(decks);
        }
        VMData.UpdateCount(_mainGame.SingleInfo!);
        CommandContainer.UpdateAll();
        await VMData.TrainStation1.AnimateShowSelectedDominoAsync(index, thisDomino, _mainGame); //hopefully this simple.
        CommandContainer.StopExecuting(); //try this as well.
    }
    private void PutBackTrain(int decks)
    {
        var thisDomino = _mainGame!.SingleInfo!.LongestTrainList.GetSpecificItem(decks);
        _mainGame.SingleInfo.LongestTrainList.RemoveObjectByDeck(decks);
        VMData.PrivateTrain1!.HandList.RemoveObjectByDeck(decks);
        _mainGame.SingleInfo.MainHandList.Add(thisDomino);
        thisDomino.IsSelected = false;
        _mainGame.SingleInfo.MainHandList.Sort();
        VMData.UpdateCount(_mainGame.SingleInfo);
    }
    private void PutBackHand(int decks)
    {
        var thisDomino = _mainGame!.SingleInfo!.MainHandList.GetSpecificItem(decks);
        VMData.PrivateTrain1!.HandList.Add(thisDomino);
        _mainGame.SingleInfo.LongestTrainList.Add(thisDomino);
        thisDomino.IsSelected = false;
        _mainGame.SingleInfo.MainHandList.RemoveObjectByDeck(decks);
        VMData.UpdateCount(_mainGame.SingleInfo);
    }
    protected override bool CanEnableBoneYard()
    {
        if (_test.AllowAnyMove)
        {
            return false;
        }
        bool sats = _mainGame!.ForceSatisfy();
        if (VMData.BoneYard!.HasDrawn())
        {
            return false;
        }
        return !sats;
    }
    private int DominoSelected(out bool isTrain)
    {
        int hands = VMData.PlayerHand1!.ObjectSelected();
        isTrain = false;
        int trains = VMData.PrivateTrain1!.ObjectSelected();
        if (hands > 0 && trains > 0)
        {
            throw new CustomBasicException("Can only have one selected from the train or hand but not both");
        }
        if (hands > 0)
        {
            return hands;
        }
        if (trains == 0)
        {
            return 0;
        }
        isTrain = true;
        return trains;
    }
    protected override async Task HandClicked(MexicanDomino domino, int index)
    {
        int decks = VMData.PrivateTrain1!.ObjectSelected();
        if (decks > 0)
        {
            PutBackTrain(decks);
            return;
        }
        if (domino.IsSelected == true)
        {
            domino.IsSelected = false;
            return;
        }
        VMData.PlayerHand1!.UnselectAllObjects();
        domino.IsSelected = true;
        _mainGame!.SingleInfo = _mainGame.PlayerList!.GetWhoPlayer();
        await Task.CompletedTask;
    }
    protected override async Task PlayerBoardClickedAsync()
    {
        int decks = VMData.PrivateTrain1!.ObjectSelected();
        if (decks > 0)
        {
            PutBackTrain(decks);
        }
        await Task.CompletedTask;
    }
    [Command(EnumCommandCategory.Game)]
    public void LongestTrain()
    {
        VMData.PlayerHand1.HandList.AddRange(_mainGame!.SingleInfo!.LongestTrainList);
        VMData.PlayerHand1.HandList.Sort(); //maybe i have to sort first.
        int locals = VMData.TrainStation1!.DominoNeeded(_mainGame.WhoTurn); //try that way.
        var output = LongestTrainClass.GetTrainList(_mainGame.SingleInfo.MainHandList, locals);
        _mainGame.SingleInfo.LongestTrainList.ReplaceRange(output);
        VMData.PrivateTrain1!.HandList.ReplaceRange(output);
        VMData.UpdateCount(_mainGame.SingleInfo); //hopefully this simple this time (?)
    }
}