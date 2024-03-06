namespace SorryDicedGame.Blazor;
public partial class CompleteStartComponent : IDisposable
{
    [Parameter]
    public string ImageHeight { get; set; } = "";
    [Parameter]
    public PlayerCollection<SorryDicedGamePlayerItem> Players { get; set; } = [];
    [Parameter]
    [EditorRequired]
    public BasicList<BoardModel> BoardList { get; set; } = [];
    [CascadingParameter]
    public SorryDicedGameMainViewModel? DataContext { get; set; }
    private static string Columns => gg1.RepeatAuto(2);
    [Parameter]
    [EditorRequired]
    public BasicGameCommand? StartPieceChosen { get; set; }
    private async Task PrivateChoseColorAsync(EnumColorChoice color)
    {
        if (StartPieceChosen is null)
        {
            return;
        }
        if (StartPieceChosen.CanExecute(color))
        {
            await StartPieceChosen.ExecuteAsync(color);
            return;
        }
    }
    private int HowMany(SorryDicedGamePlayerItem player) => BoardList.Count(x => x.PlayerOwned == player.Id && x.At == EnumBoardCategory.Start);
    protected override void OnInitialized()
    {
        DataContext!.CommandContainer.AddAction(ShowChange, "sorrystart");
        base.OnInitialized();
    }
    private bool _canRender;
    private void ShowChange()
    {
        _canRender = true;
        InvokeAsync(StateHasChanged);
    }
    protected override void OnAfterRender(bool firstRender)
    {
        _canRender = false;
        base.OnAfterRender(firstRender);
    }
#pragma warning disable CA1816 // Dispose methods should call SuppressFinalize
    public void Dispose()
#pragma warning restore CA1816 // Dispose methods should call SuppressFinalize
    {
        DataContext!.CommandContainer.RemoveAction("sorrystart");
    }


}