namespace SorryDicedGame.Core.Data;
[SingletonGame]
[UseLabelGrid]
public partial class SorryDicedGameVMData : IInstructionData
{
    [LabelColumn]
    public partial string NormalTurn { get; set; } = "";
    [LabelColumn]
    public partial string Status { get; set; } = "";
    [LabelColumn]
    public partial string Instructions { get; set; } = "";
}