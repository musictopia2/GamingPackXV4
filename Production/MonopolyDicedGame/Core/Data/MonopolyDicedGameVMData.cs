namespace MonopolyDicedGame.Core.Data;
[SingletonGame]
[UseLabelGrid]
public partial class MonopolyDicedGameVMData : IViewModelData
{
    [LabelColumn]
    public partial string NormalTurn { get; set; } = "";
    [LabelColumn]
    public partial string Status { get; set; } = "";

    [LabelColumn]
    public partial int RollNumber { get; set; } = 1;
    [LabelColumn]
    public partial int CurrentScore { get; set; }

    public StateList<EnumMiscType> OtherActions { get;} = [];
    public HouseDice? HouseDice { get; set; }
    //any other ui related properties will be here.
    //can copy/paste for the actual view model.

}