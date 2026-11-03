namespace MonopolyDicedGame.Blazor;
public class TempSpace
{
    public int Row { get; set; }
    public int Column { get; set; }
    public OwnedModel Own { get; set; } = new();
    //public bool Owned { get; set; }
}