namespace Fluxx.Blazor.Views;
public partial class FluxxMainView
{
    private readonly BasicList<LabelGridModel> _labels = new();
    private readonly BasicList<ScoreColumnModel> _scores = new();
    private FluxxVMData? _vmData;
    private FluxxGameContainer? _gameContainer;
    protected override void OnInitialized()
    {
        _vmData = aa.Resolver!.Resolve<FluxxVMData>();
        _gameContainer = aa.Resolver.Resolve<FluxxGameContainer>();
        _scores.Clear();
        _scores.AddColumn("# In Hand", false, nameof(FluxxPlayerItem.ObjectCount))
            .AddColumn("# Keepers", false, nameof(FluxxPlayerItem.NumberOfKeepers))
            .AddColumn("Bread", false, nameof(FluxxPlayerItem.Bread), category: EnumScoreSpecialCategory.TrueFalse)
            .AddColumn("Chocolate", false, nameof(FluxxPlayerItem.Chocolate), category: EnumScoreSpecialCategory.TrueFalse)
            .AddColumn("Cookies", false, nameof(FluxxPlayerItem.Cookies), category: EnumScoreSpecialCategory.TrueFalse)
            .AddColumn("Death", false, nameof(FluxxPlayerItem.Death), category: EnumScoreSpecialCategory.TrueFalse)
            .AddColumn("Dreams", false, nameof(FluxxPlayerItem.Dreams), category: EnumScoreSpecialCategory.TrueFalse)
            .AddColumn("Love", false, nameof(FluxxPlayerItem.Love), category: EnumScoreSpecialCategory.TrueFalse)
            .AddColumn("Milk", false, nameof(FluxxPlayerItem.Milk), category: EnumScoreSpecialCategory.TrueFalse)
            .AddColumn("Money", false, nameof(FluxxPlayerItem.Money), category: EnumScoreSpecialCategory.TrueFalse)
            .AddColumn("Peace", false, nameof(FluxxPlayerItem.Peace), category: EnumScoreSpecialCategory.TrueFalse)
            .AddColumn("Sleep", false, nameof(FluxxPlayerItem.Sleep), category: EnumScoreSpecialCategory.TrueFalse)
            .AddColumn("TV", false, nameof(FluxxPlayerItem.Television), category: EnumScoreSpecialCategory.TrueFalse)
            .AddColumn("Brain", false, nameof(FluxxPlayerItem.TheBrain), category: EnumScoreSpecialCategory.TrueFalse)
            .AddColumn("Moon", false, nameof(FluxxPlayerItem.TheMoon), category: EnumScoreSpecialCategory.TrueFalse)
            .AddColumn("Rocket", false, nameof(FluxxPlayerItem.TheRocket), category: EnumScoreSpecialCategory.TrueFalse)
            .AddColumn("Sun", false, nameof(FluxxPlayerItem.TheSun), category: EnumScoreSpecialCategory.TrueFalse)
            .AddColumn("Toaster", false, nameof(FluxxPlayerItem.TheToaster), category: EnumScoreSpecialCategory.TrueFalse)
            .AddColumn("Time", false, nameof(FluxxPlayerItem.Time), category: EnumScoreSpecialCategory.TrueFalse)
            .AddColumn("War", false, nameof(FluxxPlayerItem.War), category: EnumScoreSpecialCategory.TrueFalse);
        _labels.Clear();
        _labels.AddLabel("Plays Left", nameof(FluxxVMData.PlaysLeft))
            .AddLabel("Hand Limit", nameof(FluxxVMData.HandLimit))
            .AddLabel("Keeper Limit", nameof(FluxxVMData.KeeperLimit))
            .AddLabel("Play Limit", nameof(FluxxVMData.PlayLimit))
            .AddLabel("Another Turn", nameof(FluxxVMData.AnotherTurn))
            .AddLabel("Current Turn", nameof(FluxxVMData.NormalTurn))
            .AddLabel("Other Turn", nameof(FluxxVMData.OtherTurn))
            .AddLabel("Draw Bonus", nameof(FluxxVMData.DrawBonus))
            .AddLabel("Play Bonus", nameof(FluxxVMData.PlayBonus))
            .AddLabel("Cards Drawn", nameof(FluxxVMData.CardsDrawn))
            .AddLabel("Cards Played", nameof(FluxxVMData.CardsPlayed))
            .AddLabel("Draw Rules", nameof(FluxxVMData.DrawRules))
            .AddLabel($"Previous {Constants.VBCrLf}Bonus", nameof(FluxxVMData.PreviousBonus));
        base.OnInitialized();
    }
    private ICustomCommand EndCommand => DataContext!.EndTurnCommand!;
    private ICustomCommand DiscardCommand => DataContext!.DiscardCommand!;
    private ICustomCommand UnselectCommand => DataContext!.UnselectHandCardsCommand!;
    private ICustomCommand SelectCommand => DataContext!.SelectHandCardsCommand!;
}