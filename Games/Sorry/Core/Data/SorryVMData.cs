namespace Sorry.Core.Data;
[SingletonGame]
[UseLabelGrid]
public partial class SorryVMData : ISimpleBoardGamesData
{
    [LabelColumn]
    public string NormalTurn { get; set; } = "";
    [LabelColumn]
    public string Status { get; set; } = "";
    [LabelColumn]
    public string Instructions { get; set; } = "";
    public string CardDetails { get; set; } = "";
}