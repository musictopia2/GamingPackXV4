namespace DominosMexicanTrain.Core.Data;
public class TrainInfo
{
    public DeckRegularDict<MexicanDomino> DominoList { get; set; } = new ();
    public int Index { get; set; }
    public bool TrainUp { get; set; }
    public bool IsPublic { get; set; }
}