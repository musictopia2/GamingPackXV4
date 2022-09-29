namespace DominosMexicanTrain.Core.Data;
[SingletonGame]
[UseLabelGrid]
[AutoReset]
public partial class DominosMexicanTrainVMData : IDominoGamesData<MexicanDomino>
{
    public DominosMexicanTrainVMData(CommandContainer command,
            IGamePackageResolver resolver,
            DominosBasicShuffler<MexicanDomino> shuffle,
            GlobalClass global,
            TrainStationBoardProcesses trainStation,
            IEventAggregator aggregator
            )
    {
        PlayerHand1 = new HandObservable<MexicanDomino>(command);
        BoneYard = new DominosBoneYardClass<MexicanDomino>(this, command, resolver, shuffle);

        PlayerHand1.ObjectClickedAsync = PlayerHand1_ObjectClickedAsync;
        PlayerHand1.BoardClickedAsync = PlayerHand1_BoardClickedAsync;
        _global = global;
        _global.BoneYard = BoneYard;
        TrainStation1 = trainStation;
        _aggregator = aggregator;
        PrivateTrain1 = new HandObservable<MexicanDomino>(command);
        PlayerHand1.AutoSelect = EnumHandAutoType.None;
        PrivateTrain1.AutoSelect = EnumHandAutoType.None;
        PrivateTrain1.BoardClickedAsync = PrivateTrain1_BoardClickedAsync;
        PrivateTrain1.ObjectClickedAsync = PrivateTrain1_ObjectClickedAsync;
    }
    public Func<MexicanDomino, int, Task>? PrivateTrainObjectClickedAsync { get; set; }
    public Func<Task>? PrivateTrainBoardClickedAsync { get; set; }
    private Task PrivateTrain1_ObjectClickedAsync(MexicanDomino payLoad, int index)
    {
        if (PrivateTrainObjectClickedAsync == null)
        {
            throw new CustomBasicException("Private train click not set.  Rethink");
        }
        return PrivateTrainObjectClickedAsync.Invoke(payLoad, index);
    }
    private Task PrivateTrain1_BoardClickedAsync()
    {
        if (PrivateTrainBoardClickedAsync == null)
        {
            throw new CustomBasicException("Private board click not set.  Rethink");
        }
        return PrivateTrainBoardClickedAsync.Invoke();
    }
    internal void UpdateCount(DominosMexicanTrainPlayerItem player)
    {
        UpdateCountEventModel thisC = new();
        thisC.ObjectCount = player.LongestTrainList.Count;
        _aggregator.Publish(thisC);
    }
    private Task PlayerHand1_BoardClickedAsync()
    {
        if (PlayerBoardClickedAsync == null)
        {
            throw new CustomBasicException("Board clicked was never created.  Rethink");
        }
        return PlayerBoardClickedAsync.Invoke();
    }
    private Task PlayerHand1_ObjectClickedAsync(MexicanDomino domino, int index)
    {
        if (HandClickedAsync == null)
        {
            throw new CustomBasicException("The hand clicked was never done.  Rethink");
        }
        return HandClickedAsync.Invoke(domino, index);
    }
    [LabelColumn]
    public string NormalTurn { get; set; } = "";
    [LabelColumn]
    public string Status { get; set; } = "";
    [LabelColumn]
    public int UpTo { get; set; }
    public Func<MexicanDomino, Task>? DrewDominoAsync { get; set; }
    public HandObservable<MexicanDomino> PlayerHand1 { get; set; }
    public DominosBoneYardClass<MexicanDomino> BoneYard { get; set; }
    public Func<Task>? PlayerBoardClickedAsync { get; set; }
    public Func<MexicanDomino, int, Task>? HandClickedAsync { get; set; }
    public bool CanEnableBasics()
    {
        return true;
    }
    public bool CanEnableAlways()
    {
        return true;
    }
    private readonly GlobalClass _global;
    public TrainStationBoardProcesses TrainStation1;
    private readonly IEventAggregator _aggregator;
    public HandObservable<MexicanDomino> PrivateTrain1;
}