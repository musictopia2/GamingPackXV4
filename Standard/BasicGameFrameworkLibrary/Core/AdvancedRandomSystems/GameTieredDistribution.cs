namespace BasicGameFrameworkLibrary.Core.AdvancedRandomSystems;
public class GameTieredDistribution<T>
        where T : notnull
{
    private IRandomNumberList? _randomSource;
    private readonly BasicList<Tier<T>> _tiers = [];
    private Tier<T>? _existingTier;
    public void SendRandoms(IRandomNumberList? rs)
    {
        _randomSource = rs;
    }

    private void CaptureRandoms()
    {
        _randomSource ??= RandomHelpers.GetRandomGenerator();
    }

    public GameTieredDistribution<T> AddTier(string tierName, int lowRange, int highRange)
    {
        if (_tiers.Any(x => x.Name == tierName))
        {
            throw new InvalidOperationException($"Tier '{tierName}' already exists.");
        }
        CaptureRandoms();
        int weight = _randomSource!.GetRandomNumber(highRange, lowRange);
        _tiers.Add(new(tierName, weight));
        return this;
    }
    public bool HasTier(string name)
    {
        return _tiers.Any(x => x.Name == name);
    }
    public GameTieredDistribution<T> AddTier(string tierName, int weight)
    {
        if (_tiers.Any(x => x.Name == tierName))
        {
            throw new InvalidOperationException($"Tier '{tierName}' already exists.");
        }
        _tiers.Add(new(tierName, weight));
        return this;
    }
    public GameTieredDistribution<T> SelectTier(string tierName, Action<GameTieredDistribution<T>> action)
    {
        _existingTier = _tiers.Single(x => x.Name == tierName);
        action.Invoke(this);
        return this;
    }

    private void CheckTier()
    {
        if (_existingTier is null)
        {
            throw new CustomBasicException("No tier was selected. Try calling SelectTier and then using a delegate.");
        }
    }

    public GameTieredDistribution<T> AddWeightedItem(BasicList<T> items, int weight)
    {
        CheckTier();
        foreach (var item in items)
        {
            _existingTier!.AddItem(item, weight);
        }
        return this;
    }

    public GameTieredDistribution<T> AddWeightedItem(BasicList<T> items, int lowRange, int highRange)
    {
        CheckTier();
        CaptureRandoms();
        foreach (var item in items)
        {
            int weight = _randomSource!.GetRandomNumber(highRange, lowRange);
            _existingTier!.AddItem(item, weight);
        }
        return this;
    }
    public GameTieredDistribution<T> AddWeightedItem(T item, int weight)
    {
        CheckTier();
        _existingTier!.AddItem(item, weight);
        return this;
    }

    public GameTieredDistribution<T> AddWeightedItem(T item, int lowRange, int highRange)
    {
        CheckTier();
        CaptureRandoms();
        int weight = _randomSource!.GetRandomNumber(highRange, lowRange);
        _existingTier!.AddItem(item, weight);
        return this;
    }

    public GameTieredDistribution<T> AddWeightedItemWithChance(T thisItem, int notPassingWeight, int firstWeight, int desiredWeight)
    {
        CheckTier();
        CaptureRandoms();
        int totalWeight = notPassingWeight + firstWeight;
        int ask = _randomSource!.GetRandomNumber(totalWeight);
        if (ask <= notPassingWeight)
        {
            return this;
        }
        return AddWeightedItem(thisItem, desiredWeight);
    }

    // Randomly select a tier based on the probabilities.
    private Tier<T> SelectTier()
    {
        int totalWeight = _tiers.Sum(x => x.Probability);
        int randomChoice = _randomSource!.GetRandomNumber(totalWeight);
        int cumulativeWeight = 0;

        foreach (var tier in _tiers)
        {
            cumulativeWeight += tier.Probability;
            if (randomChoice <= cumulativeWeight)
            {
                return tier;
            }
        }

        throw new InvalidOperationException("Unable to select a tier.");
    }

    // Select a random item from the currently selected tier.
    public T GetRandomItem()
    {
        WeightedAverageLists<T> weightedItems = new();
        Tier<T> selectedTier = SelectTier();
        //var selectedDistribution = _tieredDistributions[selectedTier];

        // Add the items and their weights to the weighted items list
        foreach (var item in selectedTier.Items)
        {
            weightedItems.AddWeightedItem(item.Item, item.Weight);
            //weightedItems.AddWeightedItem(item.Key, item.Value);
        }

        // Get a random weighted item
        return weightedItems.GetRandomWeightedItem();
    }
}