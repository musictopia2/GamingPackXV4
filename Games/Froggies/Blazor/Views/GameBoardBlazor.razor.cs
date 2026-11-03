
namespace Froggies.Blazor.Views;
public partial class GameBoardBlazor
{
    private BasicList<LilyPadModel> Lilies { get; set; } = [];
    public static Point GetPoint(LilyPadModel model)
    {
        return new Point(model.Column * SpaceSize, model.Row * SpaceSize);
    }
    protected override void OnInitialized()
    {
        DataContext!.StateHasChanged = () =>
        {
            Lilies = DataContext!.MainGame.GetCompleteLilyList();
            InvokeAsync(StateHasChanged);
        };
        base.OnInitialized();
    }
    protected override void OnParametersSet()
    {
        Lilies = DataContext!.MainGame.GetCompleteLilyList();
        base.OnParametersSet();
    }

    private static int BoardSize => SpaceSize * 10;

    private static int SpaceSize
    {
        get
        {
            if (ll1.LayoutMode == EnumGameLayoutMode.Compact)
            {
                return 50;
            }
            if (ll1.LayoutMode == EnumGameLayoutMode.Tablet)
            {
                return 58;
            }
            return 64;
        }
    }

}