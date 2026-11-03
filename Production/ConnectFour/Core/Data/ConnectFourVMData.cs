namespace ConnectFour.Core.Data;
[SingletonGame]
[UseLabelGrid]
public partial class ConnectFourVMData : ISimpleBoardGamesData
{
    [LabelColumn]
    public partial string NormalTurn { get; set; } = "";
    [LabelColumn]
    public partial string Status { get; set; } = "";
    [LabelColumn]
    public partial string Instructions { get; set; } = "";
    //any other ui related properties will be here.
    //can copy/paste for the actual view model.

}