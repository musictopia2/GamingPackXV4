namespace SorryCardGame.Blazor;
public partial class BoardBlazor
{
    [Parameter]
    public SorryCardGamePlayerItem? Player { get; set; }
    [CascadingParameter]
    public SorryCardGameGameContainer? GameData { get; set; }
    [CascadingParameter]
    public int TargetHeight { get; set; } = 15;
    private bool IsEnabled
    {
        get
        {
            if (GameData!.Command.IsExecuting)
            {
                return false;
            }
            return Player!.CanSorryPlayer();
        }
    }
    private async Task SorryPlayerAsync()
    {
        if (IsEnabled == false)
        {
            return;
        }
        GameData!.Command.StartExecuting();
        await Player!.SorryPlayerAsync();
    }
    private readonly BasicList<SorryCardGameCardInformation> _list = new();
    private SorryCardGameCardInformation GetCard(int index)
    {
        SorryCardGameCardInformation output = new();
        output.Sorry = EnumSorry.OnBoard;
        output.Color = Player!.Color.Color;
        if (index <= Player!.HowManyAtHome)
        {
            output.Category = EnumCategory.Home;
        }
        else
        {
            output.Category = EnumCategory.Start;
        }
        output.Deck = 1; //so it can draw.  otherwise, does not work.
        return output;
    }
    protected override void OnParametersSet()
    {
        _list.Clear();
        for (int i = 1; i <= 4; i++)
        {
            var card = GetCard(i);
            _list.Add(card);
        }
        base.OnParametersSet();
    }
    private string RealHeight => $"{TargetHeight}vh";
}