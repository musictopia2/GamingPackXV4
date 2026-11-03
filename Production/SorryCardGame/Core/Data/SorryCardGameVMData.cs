namespace SorryCardGame.Core.Data;
[SingletonGame]
[UseLabelGrid]
[AutoReset]
public partial class SorryCardGameVMData : IBasicCardGamesData<SorryCardGameCardInformation>, IStateChanged
{
    [LabelColumn]
    public partial string NormalTurn { get; set; } = "";
    [LabelColumn]
    public partial string Status { get; set; } = "";
    [LabelColumn]
    public partial int UpTo { get; set; }
    partial void OnUpToChanged(int value)
    {
        StateChanged?.Invoke();
    }

    [LabelColumn]
    public partial string Instructions { get; set; } = "";
    public SorryCardGameVMData(CommandContainer command)
    {
        Deck1 = new(command);
        Pile1 = new(command);
        PlayerHand1 = new(command);
        OtherPile = new(command);
        OtherPile.Text = "Play Pile";
        OtherPile.FirstLoad(new SorryCardGameCardInformation());
    }
    public DeckObservablePile<SorryCardGameCardInformation> Deck1 { get; set; }
    public SingleObservablePile<SorryCardGameCardInformation> Pile1 { get; set; }
    public HandObservable<SorryCardGameCardInformation> PlayerHand1 { get; set; }
    public SingleObservablePile<SorryCardGameCardInformation>? OtherPile { get; set; }

    public event Action? StateChanged;
}