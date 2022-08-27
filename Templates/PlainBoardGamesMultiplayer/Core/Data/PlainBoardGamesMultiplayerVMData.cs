namespace PlainBoardGamesMultiplayer.Core.Data;
[SingletonGame]
[UseLabelGrid]
public partial class PlainBoardGamesMultiplayerVMData : ISimpleBoardGamesData
{
    [LabelColumn]
    public string NormalTurn { get; set; } = "";
    [LabelColumn]
    public string Status { get; set; } = "";
    [LabelColumn]
    public string Instructions { get; set; } = "";
    //any other ui related properties will be here.
    //can copy/paste for the actual view model.

}