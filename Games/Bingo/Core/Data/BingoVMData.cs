namespace Bingo.Core.Data;
[SingletonGame]
[UseLabelGrid]
public partial class BingoVMData : IViewModelData
{
    public string NormalTurn { get; set; } = "";
    [LabelColumn]
    public string Status { get; set; } = "";
    public string NumberCalled { get; set; } = "";
}