using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BasicGameFrameworkLibrary.Core.AdvancedRandomSystems;
public class Tier<T>(string name, int probability)
    where T: notnull
{
    public string Name { get; set; } = name;
    public BasicList<WeightedItem<T>> Items { get; set; } = [];
    public int Probability { get; set; } = probability;
    public void AddItem(T item, int weight)
    {
        Items.Add(new WeightedItem<T>(item, weight));
    }
}