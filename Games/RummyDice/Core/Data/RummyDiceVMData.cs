namespace RummyDice.Core.Data;
[SingletonGame]
[UseLabelGrid]
public partial class RummyDiceVMData : IViewModelData
{
    [LabelColumn]
    public string NormalTurn { get; set; } = "";
    [LabelColumn]
    public string Status { get; set; } = "";
    [LabelColumn]
    public int RollNumber { get; set; }
    [LabelColumn]
    public string CurrentPhase { get; set; } = "None";
    [LabelColumn]
    public int Score { get; set; }
}