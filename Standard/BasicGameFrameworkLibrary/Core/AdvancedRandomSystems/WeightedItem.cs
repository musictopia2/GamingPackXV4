namespace BasicGameFrameworkLibrary.Core.AdvancedRandomSystems;
public class WeightedItem<T>(T item, int weight)
    where T : notnull
{
    public T Item { get; set; } = item;
    public int Weight { get; set; } = weight;
}