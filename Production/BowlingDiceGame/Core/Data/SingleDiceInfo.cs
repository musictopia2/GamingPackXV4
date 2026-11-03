namespace BowlingDiceGame.Core.Data;
[SingletonGame]
public class SingleDiceInfo : IBasicDice<bool>, IDiceContainer<bool>
{
    public int HeightWidth { get; } = 60;
    public bool Value { get; set; }
    public bool DidHit { get; set; }
    public bool IsAnimatingHit { get; set; }
    public bool Visible { get; set; }
    public int Index { get; set; }
    public IGamePackageResolver? MainContainer { get; set; }
    public IGamePackageGeneratorDI? GeneratorContainer { get; set; }
    public void Populate(bool chosen)
    {
        Value = chosen;
    }
    bool IGenerateDice<bool>.GetRandomDiceValue(bool isLastItem)
    {
        IRandomGenerator rs = MainContainer!.Resolve<IRandomGenerator>(); //i need this one.
        if (isLastItem == false)
        {
            return rs.NextBool();
        }
        if (_commitList.Count == 0)
        {
            throw new CustomBasicException("No committed dice values were set before rolling the final section.");
        }
        bool value = _commitList.First();
        _commitList.RemoveFirstItem();
        return value;
    }
    private readonly BasicList<bool> _commitList = [];
    void IGenerateDice<bool>.StartRoll()
    {
        _commitList.Clear();
        _commitList.AddRange(GetDistribution());
    }
    private static BasicList<bool> GetDistribution()
    {
        if (RollContext.CurrentDistribution is null)
        {
            throw new CustomBasicException("Must now have distribution so we know which pins are knocked down");
        }
        int valueRequested = RollContext.CurrentRollNumber switch
        {
            1 => RollContext.CurrentDistribution.First,
            2 => RollContext.CurrentDistribution.Second,
            3 => RollContext.CurrentDistribution.Third,
            _ => throw new CustomBasicException("CurrentRollNumber must be between 1 and 3")
        };
        int standingPins = RollContext.HowManyPins;
        if (valueRequested > standingPins)
        {
            throw new CustomBasicException("Cannot knock down more pins than are standing.");
        }

        // Create list of all remaining pins (false = up)
        BasicList<bool> pins = Enumerable.Repeat(false, standingPins).ToBasicList();
        // Randomly choose which pins to knock down (set to true)
        BasicList<int> indexes = Enumerable.Range(0, standingPins).ToBasicList();
        indexes.ShuffleList();
        foreach (var i in indexes.Take(valueRequested))
        {
            pins[i] = true;
        }
        return pins;


    }
}