namespace MonopolyDicedGame.Blazor;
public partial class MonopolyListBlazor
{
    [CascadingParameter]
    public MonopolyDicedGameMainViewModel? DataContext { get; set; }

    [Parameter]
    [EditorRequired]
    public BasicList<BasicDiceModel> DiceList { get; set; } = [];

    [Parameter]
    [EditorRequired]
    public string ImageHeight { get; set; } = "";
    [Parameter]
    public EventCallback<BasicDiceModel> OnDiceClick { get; set; }

    //won't allow selecting/unselecting dice yet.

    protected override void OnInitialized()
    {
        DataContext!.CommandContainer.AddAction(ShowChange, "monopolydice");
        base.OnInitialized();
    }
    private void ShowChange()
    {
        InvokeAsync(StateHasChanged);
    }
}