namespace ThinkTwice.Blazor.Views;
public partial class ScoreView
{
    [CascadingParameter]
    public ScoreViewModel? DataContext { get; set; }
    private string GetButtonColor(string text)
    {
        if (DataContext!.VMData!.ItemSelected == -1)
        {
            return cc1.Aqua;
        }
        string selected = DataContext!.VMData.TextList[DataContext!.VMData.ItemSelected];
        if (selected.Equals(text))
        {
            return cc1.LimeGreen;
        }
        return cc1.Aqua;
    }
    private ICustomCommand ChangeCommand => DataContext!.ChangeSelectionCommand!;
    private ICustomCommand ScoreCommand => DataContext!.CalculateScoreCommand!;
    private ICustomCommand DescriptionCommand => DataContext!.ScoreDescriptionCommand!;
}