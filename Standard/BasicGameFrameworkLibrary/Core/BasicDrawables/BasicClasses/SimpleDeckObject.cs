namespace BasicGameFrameworkLibrary.Core.BasicDrawables.BasicClasses;
[UseStateChanged]
public abstract partial class SimpleDeckObject : IEquatable<SimpleDeckObject>
{
    protected virtual void ChangeDeck() { }

    [StateProperty]
    public partial int Deck { get; set; }
    partial void OnDeckChanged(int value)
    {
        ChangeDeck(); //somebody can do something else when the deck change.
    }



    [StateProperty]
    public partial bool Drew { get; set; }
    [StateProperty]
    public partial bool IsUnknown { get; set; }
    [StateProperty]
    public partial bool Visible { get; set; } = true;
    [StateProperty]
    public partial bool IsSelected { get; set; }
    [StateProperty]
    public partial bool IsEnabled { get; set; } = true;
    public virtual BasicDeckRecordModel GetRecord => new(Deck, IsSelected, Drew, IsUnknown, IsEnabled, Visible);
    public virtual string GetKey() => Guid.NewGuid().ToString(); //most of the time, had to use a guid.
    public SizeF DefaultSize { get; set; }
    public bool Rotated { get; set; }
    public override bool Equals(object? obj)
    {
        if (obj is not SimpleDeckObject temps)
        {
            return false;
        }
        return Deck.Equals(temps.Deck);
    }
    public bool Equals(SimpleDeckObject? other)
    {
        return other != null &&
               Deck == other.Deck;
    }
    public override int GetHashCode()
    {
        return Deck.GetHashCode();
    }
}