namespace BasicGameFrameworkLibrary.Core.BasicDrawables.BasicClasses;
public abstract class SimpleDeckObject : IEquatable<SimpleDeckObject>
{
    protected virtual void ChangeDeck() { }
    private int _deck;
    public int Deck
    {
        get { return _deck; }
        set
        {
            if (SetProperty(ref _deck, value))
            {
                ChangeDeck(); //so games like fluxx can change another property in response to this.
            }
        }
    }
    public bool Drew { get; set; }
    public bool IsUnknown { get; set; }
    public bool Visible { get; set; } = true;
    private bool _isSelected;
    public bool IsSelected
    {
        get => _isSelected;
        set
        {
            if (SetProperty(ref _isSelected, value))
            {
                //decide what to do now (?)
                ChangeSelectAction?.Invoke();
            }
        }
    }
    [JsonIgnore]
    public Action? ChangeSelectAction { get; set; }
    public bool IsEnabled { get; set; } = true;
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