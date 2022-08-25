
namespace Froggies.Blazor.Views;
public partial class GameBoardBlazor
{
    private BasicList<LilyPadModel> Lilies { get; set; } = new();
    public static Point GetPoint(LilyPadModel model)
    {
        return new Point(model.Column * 64, model.Row * 64);
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
}