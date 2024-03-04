namespace ClueCardGame.Blazor;
public partial class BasicNotebookComponent
{
    [Parameter]
    [EditorRequired]
    public Dictionary<int, DetectiveInfo> PersonalNotebook { get; set; } = [];


    [Parameter]
    [EditorRequired]
    public ICustomCommand? PredictCommand { get; set; }
    [CascadingParameter]
    public int TargetHeight { get; set; } = 15;
    private string RealHeight => $"{TargetHeight}vh";

    private BasicList<ClueCardGameCardInformation> GetCardsInCategory(EnumCardType category)
    {
        var list = PersonalNotebook.Where(x => x.Value.Category == category).ToBasicList();
        BasicList<ClueCardGameCardInformation> output = [];
        foreach (var item in list)
        {
            ClueCardGameCardInformation card = new();
            card.Populate(item.Key);
            if (item.Value.IsChecked || item.Value.WasGiven)
            {
                card.IsSelected = true;
            }
            output.Add(card);
        }
        output = output.OrderBy(x => x.Deck).ToBasicList();
        return output;
    }





}