namespace Rook.Blazor;
public partial class TrumpComponent
{
    [Parameter]
    [EditorRequired]
    public RookMainViewModel? DataContext { get; set; }
    private static string GetColor(BasicPickerData<EnumColorTypes> piece) => piece.EnumValue.Color;
    private ICustomCommand TrumpCommand => DataContext!.TrumpCommand!;
}