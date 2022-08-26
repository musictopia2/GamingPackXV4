namespace Mancala.Core.Data;
[SingletonGame]
[UseLabelGrid]
public partial class MancalaVMData : IViewModelData
{
    [LabelColumn]
    public string NormalTurn { get; set; } = "";
    [LabelColumn]
    public string Status { get; set; } = "";
    [LabelColumn]
    public string Instructions { get; set; } = "";
    public int PiecesAtStart { get; set; }
    public int PiecesLeft { get; set; }
    public int SpaceSelected { get; set; }
    public int SpaceStarted { get; set; }
    internal Dictionary<int, SpaceInfo> SpaceList { get; set; } = new(); //this should be the main list.
}