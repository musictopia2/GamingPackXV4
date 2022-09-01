
namespace Rook.Blazor.Views;
public partial class RookColorView
{
    private static string GetColor(BasicPickerData<EnumColorTypes> piece) => piece.EnumValue.Color;
    private ICustomCommand TrumpCommand => DataContext!.TrumpCommand!;
}