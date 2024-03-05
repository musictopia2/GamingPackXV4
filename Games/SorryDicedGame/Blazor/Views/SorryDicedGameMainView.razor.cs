namespace SorryDicedGame.Blazor.Views;
public partial class SorryDicedGameMainView
{
    [CascadingParameter]
    public TestOptions? TestData { get; set; }
    private readonly BasicList<LabelGridModel> _labels = new();

    //private BasicList<SorryDiceModel> _dice = [];

    protected override void OnInitialized()
    {
        _labels.Clear();
        _labels.AddLabel("Turn", nameof(SorryDicedGameVMData.NormalTurn))
                .AddLabel("Instructions", nameof(SorryDicedGameVMData.Instructions))
                .AddLabel("Status", nameof(SorryDicedGameVMData.Status));

        //7.Times(x =>
        //{
        //    SorryDiceModel dice = new();
        //    dice.Populate(x);
        //    _dice.Add(dice);
        //});

        base.OnInitialized();
    }
    private ICustomCommand EndCommand => DataContext!.EndTurnCommand!;
    private ICustomCommand RollCommand => DataContext!.RollCommand!;
    private BasicGameCommand ChooseStartCommand => DataContext!.ChoseStartPieceCommand!;
}