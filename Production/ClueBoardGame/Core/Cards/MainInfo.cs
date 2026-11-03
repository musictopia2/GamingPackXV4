namespace ClueBoardGame.Core.Cards;
public abstract class MainInfo : SimpleDeckObject, IDeckObject
{
    public string Name { get; set; } = "";
    public abstract void Populate(int Chosen);
    public abstract void Reset();
}