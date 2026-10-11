namespace Candyland.Core.Data;
[SingletonGame]
[UseLabelGrid]
public partial class CandylandVMData : IViewModelData
{
    [LabelColumn]
    public partial string NormalTurn { get; set; } = "";
    [LabelColumn]
    public partial string Status { get; set; } = "";

    //any other ui related properties will be here.
    //can copy/paste for the actual view model.

}