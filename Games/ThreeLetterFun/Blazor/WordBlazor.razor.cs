namespace ThreeLetterFun.Blazor;
public partial class WordBlazor
{
    [Parameter]
    public ThreeLetterFunMainGameClass? MainGame { get; set; }
    [Parameter]
    public ThreeLetterFunCardData? DataContext { get; set; }
    private string GetFillColor()
    {
        if (MainGame!.SaveRoot!.Level == EnumLevel.Easy)
        {
            return cc1.LimeGreen.ToWebColor;
        }
        return cc1.DarkOrange.ToWebColor;
    }
    private readonly BasicList<int> _lefts = new() { 3, 25, 47 };
    private async Task WordClickedAsync(EnumClickPosition position)
    {
        if (MainGame!.GameBoard.ObjectCommand!.CanExecute(DataContext) == false)
        {
            return;
        }
        if (MainGame.SaveRoot.Level == EnumLevel.Easy)
        {
            DataContext!.ClickLocation = EnumClickPosition.Left; //always left for easy mode.
        }
        else
        {
            DataContext!.ClickLocation = position;
        }
        await MainGame.GameBoard.ObjectCommand.ExecuteAsync(DataContext);
    }
}