namespace Minesweeper.Blazor.Views;
public partial class MinesweeperMainView
{
    private readonly BasicList<LabelGridModel> _labels = new();
    private string GetDisplay()
    {
        if (DataContext!.IsFlagging)
        {
            return "Flag Mines";
        }
        return "Unflip Mines";
    }
    private string BackgroundColor()
    {
        if (DataContext!.IsFlagging)
        {
            return cc.Yellow;
        }
        return cc.Aqua;
    }
    protected override void OnInitialized()
    {
        _labels.Clear();
        _labels.AddLabel("Mines Needed", nameof(MinesweeperMainViewModel.HowManyMinesNeeded))
            .AddLabel("Mines Left", nameof(MinesweeperMainViewModel.NumberOfMinesLeft))
            .AddLabel("Level Chosen", nameof(MinesweeperMainViewModel.LevelChosen));
        base.OnInitialized();
    }
}