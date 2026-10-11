namespace RummyDice.Core.Data;
[SingletonGame]
[UseLabelGrid]
public partial class RummyDiceVMData : IViewModelData
{
    [LabelColumn]
    public partial string NormalTurn { get; set; } = "";
    [LabelColumn]
    public partial string Status { get; set; } = "";
    [LabelColumn]
    public partial int RollNumber { get; set; }
    [LabelColumn]
    public partial string CurrentPhase { get; set; } = "None";
    [LabelColumn]
    public partial int Score { get; set; }
}