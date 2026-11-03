namespace LottoDominos.Core.Data;
[SingletonGame]
[UseLabelGrid]
public partial class LottoDominosVMData : IViewModelData
{
    [LabelColumn]
    public partial string NormalTurn { get; set; } = "";
    [LabelColumn]
    public partial string Status { get; set; } = "";
    internal DominosBasicShuffler<SimpleDominoInfo> DominosList { get; set; } = [];
}