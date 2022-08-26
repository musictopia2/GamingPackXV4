namespace LottoDominos.Core.Data;
[SingletonGame]
[UseLabelGrid]
public partial class LottoDominosVMData : IViewModelData
{
    [LabelColumn]
    public string NormalTurn { get; set; } = "";
    [LabelColumn]
    public string Status { get; set; } = "";
    internal DominosBasicShuffler<SimpleDominoInfo> DominosList { get; set; }
    //looks like the shuffler has to be here.
    public LottoDominosVMData()
    {
        DominosList = new();
    }
}