namespace BasicGameFrameworkLibrary.Blazor.BasicControls.SimpleControls.Labels;
public partial class TopBarLabel
{
    [Parameter]
    [EditorRequired]
    public string Display { get; set; } = "";
    [Parameter]
    public string MarginLeft { get; set; } = "";
    private string GetExtras() => string.IsNullOrWhiteSpace(MarginLeft) ? "" : $"margin-left: {MarginLeft};";
}