namespace Bingo.Core.Data;
[SingletonGame]
[UseLabelGrid]
[UseStateChanged]
public partial class BingoVMData : IViewModelData
{
    public string NormalTurn { get; set; } = "";
    [LabelColumn]
    public partial string Status { get; set; } = "";
    [StateProperty]
    public partial string NumberCalled { get; set; } = "";
}