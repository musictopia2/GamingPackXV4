namespace MonopolyDicedGame.Core.Data;
[SingletonGame]
[UseLabelGrid]
public partial class MonopolyDicedGameVMData : IViewModelData
{
    [LabelColumn]
    public string NormalTurn { get; set; } = "";
    [LabelColumn]
    public string Status { get; set; } = "";

    [LabelColumn]
    public int RollNumber { get; set; } = 1;


    //any other ui related properties will be here.
    //can copy/paste for the actual view model.

}