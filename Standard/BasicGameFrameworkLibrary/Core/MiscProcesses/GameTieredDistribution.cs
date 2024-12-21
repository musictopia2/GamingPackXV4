namespace BasicGameFrameworkLibrary.Core.MiscProcesses;
public class GameTieredDistribution<T>
    where T : notnull
{
    private IRandomNumberList? _randomSource;
    private readonly Dictionary<string, int> _tierProbabilities = [];
    private readonly Dictionary<string, Dictionary<T, int>> _tieredDistributions = [];
    public void SendRandoms(IRandomNumberList? rs)
    {
        _randomSource = rs;
    }
    private void CaptureRandoms()
    {
        _randomSource ??= RandomHelpers.GetRandomGenerator();
    }
    // Set up the probabilities for each tier with a more fluent approach.
    public GameTieredDistribution<T> AddTier(string tierName, int weight)
    {
        if (_tierProbabilities.ContainsKey(tierName))
        {
            throw new InvalidOperationException($"Tier '{tierName}' already exists.");
        }
        _tierProbabilities[tierName] = weight;
        return this; // To allow method chaining.
    }
    private string _existingTier = "";

    public GameTieredDistribution<T> SelectTier(string tierName, Action<GameTieredDistribution<T>> action)
    {
        _existingTier = tierName;
        action.Invoke(this);
        return this;
    }
    private void CheckTier()
    {
        if (string.IsNullOrWhiteSpace(_existingTier))
        {
            throw new CustomBasicException("No tier was selected  Try calling SelectTier and then using a delegate");
        }
        if (!_tierProbabilities.ContainsKey(_existingTier))
        {
            throw new InvalidOperationException($"Tier '{_existingTier}' has not been defined. Please define it first.");
        }

        if (!_tieredDistributions.ContainsKey(_existingTier))
        {
            _tieredDistributions[_existingTier] = new();
        }
    }
    public GameTieredDistribution<T> AddWeightedItem(BasicList<T> items, int weight)
    {
        CheckTier();
        foreach (var item in items)
        {
            _tieredDistributions[_existingTier][item] = weight;
        }
        return this;
    }
    public GameTieredDistribution<T> AddWeightedItem(BasicList<T> items, int lowRange, int highRange)
    {
        CheckTier();
        CaptureRandoms();
        foreach(var item in items)
        {
            int weight = _randomSource!.GetRandomNumber(highRange, lowRange);
            _tieredDistributions[_existingTier][item] = weight;
        }
        return this;
    }
    public GameTieredDistribution<T> AddWeightedItem(T item, int weight)
    {
        CheckTier(); 
        _tieredDistributions[_existingTier][item] = weight;
        return this;
    }
    public GameTieredDistribution<T> AddWeightedItem(T item, int lowRange, int highRange)
    {
        CheckTier();
        CaptureRandoms();
        int weight = _randomSource!.GetRandomNumber(highRange, lowRange);
        _tieredDistributions[_existingTier][item] = weight;
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
    private string SelectTier()
    {
        int totalWeight = _tierProbabilities.Values.Sum();
        int randomChoice = _randomSource!.GetRandomNumber(totalWeight);
        int cumulativeWeight = 0;

        foreach (var tier in _tierProbabilities)
        {
            cumulativeWeight += tier.Value;
            if (randomChoice <= cumulativeWeight)
            {
                return tier.Key;
            }
        }

        // This should never happen if the probabilities sum to a positive number.
        throw new InvalidOperationException("Unable to select a tier.");
    }

    // Select a random item from the currently selected tier.
    public T GetRandomItem()
    {
        WeightedAverageLists<T> weightedItems = new();

        string selectedTier = SelectTier();
        var selectedDistribution = _tieredDistributions[selectedTier];
        foreach (var item in selectedDistribution)
        {
            weightedItems.AddWeightedItem(item.Key, item.Value);
        }
        return weightedItems.GetRandomWeightedItem();
    }
}