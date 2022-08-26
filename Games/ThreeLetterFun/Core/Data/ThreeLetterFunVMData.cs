namespace ThreeLetterFun.Core.Data;
[SingletonGame]
[UseLabelGrid]
public partial class ThreeLetterFunVMData : IViewModelData
{
    [LabelColumn]
    public string NormalTurn { get; set; } = "";
    [LabelColumn]
    public string Status { get; set; } = "";
    public INewCard? NewUI;
    public TileBoardObservable? TileBoard1 { get; set; }
    public Action? CalculateVisible { get; set; }
    private string _playerWon = "";
    public string PlayerWon
    {
        get { return _playerWon; }
        set
        {
            if (SetProperty(ref _playerWon, value))
            {
                CalculateVisible?.Invoke();
            }
        }
    }
    private ThreeLetterFunCardData? _currentCard;
    public ThreeLetterFunCardData? CurrentCard
    {
        get { return _currentCard; }
        set
        {
            if (SetProperty(ref _currentCard, value))
            {
                CalculateVisible?.Invoke();
            }
        }
    }
}