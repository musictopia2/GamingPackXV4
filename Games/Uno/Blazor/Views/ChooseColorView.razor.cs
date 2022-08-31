namespace Uno.Blazor.Views;
public partial class ChooseColorView
{
    [CascadingParameter]
    public ChooseColorViewModel? DataContext { get; set; }
    private UnoVMData? _data;
    protected override void OnParametersSet()
    {
        _data = DataContext!.Model;
        base.OnParametersSet();
    }
    private static string GetColor(BasicPickerData<EnumColorTypes> piece) => piece.EnumValue.Color; //i think.
}