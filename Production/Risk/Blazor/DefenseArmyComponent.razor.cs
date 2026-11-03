namespace Risk.Blazor;
public partial class DefenseArmyComponent
{
    [Parameter]
    public int HowManyArmies { get; set; }
    [Parameter]
    public string Color { get; set; } = "";
}