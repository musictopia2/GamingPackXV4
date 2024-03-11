namespace DealCardGame.Core.Data;
public class StealPropertyModel
{
    public int PlayerId { get; set; } //this is the player you are stealing from.
    public int CardPlayed { get; set; } //this is the card played so later can be removed like normal.
    public int CardChosen { get; set; } //this is the card being stolen.
    public EnumColor Color { get; set; } //this will help filter down the colors for stealing.
    public bool StartStealing { get; set; } //this means you are starting to steal.
}