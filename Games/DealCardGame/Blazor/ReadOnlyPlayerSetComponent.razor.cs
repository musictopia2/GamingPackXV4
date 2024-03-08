using System.Reflection;

namespace DealCardGame.Blazor;
public partial class ReadOnlyPlayerSetComponent
{
    [Parameter]
    [EditorRequired]
    public int Completed { get; set; } //from 0 to 4.
    [Parameter]
    [EditorRequired]
    public int Rent { get; set; } //this is the rent charged (at the bottom)
    [Parameter]
    [EditorRequired]
    public EnumColor Color { get; set; }
    [Parameter]
    [EditorRequired]
    public bool HasHouse { get; set; }
    [Parameter]
    [EditorRequired]
    public bool HasHotel { get; set; }
    [Parameter]
    [EditorRequired]
    public int PlayerId { get; set; }
    [Parameter]
    public EventCallback<SetPlayerModel> OnSetClicked { get; set; }
    private static string Rows => gg1.RepeatSpreadOut(3);
    private Assembly GetAssembly => Assembly.GetAssembly(GetType())!;
    private void PrivateClicked()
    {
        SetPlayerModel set = new()
        {
            PlayerId = PlayerId,
            Color = Color,
        };
        OnSetClicked.InvokeAsync(set);
    }
}