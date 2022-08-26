namespace ThreeLetterFun.Blazor.Views;
public partial class ThreeLetterFunMainView : INewCard
{
    private ThreeLetterFunCardData? _tempCard;
    [CascadingParameter]
    public TestOptions? TestData { get; set; }
    private readonly BasicList<ScoreColumnModel> _scores = new();
    protected override void OnInitialized()
    {
        _scores.AddColumn("Cards Won", true, nameof(ThreeLetterFunPlayerItem.CardsWon));
        DataContext!.VMData.NewUI = this; //try this.
        base.OnInitialized();
    }
    protected void RunTest() //just to make the suggestion message go away.
    {
        if (Board is null)
        {

        }
    }
    private TileBoardBlazor? Board { get; set; }
    void INewCard.ShowNewCard()
    {
        _tempCard = DataContext!.VMData.CurrentCard; //hopefully this simple.
        ShowChange(); //try this way.
    }
}