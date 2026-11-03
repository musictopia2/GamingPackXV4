namespace Sorry.Core.Data;
[SingletonGame]
[UseLabelGrid]
public partial class SorryVMData : ISimpleBoardGamesData
{
    [LabelColumn]
    public partial string NormalTurn { get; set; } = "";
    [LabelColumn]
    public partial string Status { get; set; } = "";
    [LabelColumn]
    public partial string Instructions { get; set; } = "";
    public string CardDetails { get; set; } = "";
}