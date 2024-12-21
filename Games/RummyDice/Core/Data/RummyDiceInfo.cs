namespace RummyDice.Core.Data;
public class RummyDiceInfo : IRummmyObject<EnumColorType, EnumColorType>,
    IBasicDice<int>, IGenerateDice<int>, IComparable<RummyDiceInfo>, ISelectableObject
{
    public int HeightWidth { get; set; } = 44;
    public int Value { get; set; }
    public int Index { get; set; }
    public string Display { get; set; } = "";
    public bool IsSelected { get; set; }
    public bool Visible { get; set; } = true;
    public EnumColorType Color { get; set; }
    public bool IsWild { get; set; }
    int IRummmyObject<EnumColorType, EnumColorType>.GetSecondNumber => 0;
    int ISimpleValueObject<int>.ReadMainValue => Value;
    bool IWildObject.IsObjectWild => IsWild;
    bool IIgnoreObject.IsObjectIgnored => false;
    EnumColorType ISuitObject<EnumColorType>.GetSuit => Color;
    EnumColorType IColorObject<EnumColorType>.GetColor => Color;
    public void Populate(int chosen)
    {
        if (chosen == 0)
        {
            throw new CustomBasicException("Cannot choose 0");
        }
        int z = 0;
        for (int x = 1; x <= 4; x++)
        {
            for (int y = 1; y <= 12; y++)
            {
                z++;
                if (z == chosen)
                {
                    Index = chosen;
                    Color = EnumColorType.FromValue(x);
                    IsWild = false;
                    Value = y;
                    Display = Value.ToString();
                    return;
                }
            }
        }
        //1 to 40 is 1 to 10
        //41 to 48 is 11 and 12
        //49 to 52 are wilds.

        for (int x = 1; x <= 4; x++)
        {
            z++;
            if (z == chosen)
            {
                IsWild = true;
                Value = 0;
                Display = "W";
                Color = EnumColorType.FromValue(x);
                return;
            }
        }
        throw new CustomBasicException($"Nothing found for {chosen}");
    }
    int IComparable<RummyDiceInfo>.CompareTo(RummyDiceInfo? other) //probably important to sort the dice this time.
    {
        if (IsWild.Equals(other!.IsWild) == false)
        {
            return IsWild.CompareTo(other.IsWild);
        }
        if (Value.Equals(other.Value) == false)
        {
            return Value.CompareTo(other.Value);
        }
        return Color.CompareTo(other.Color);
    }
    int IGenerateDice<int>.GetRandomDiceValue(bool isLastItem)
    {
        BasicList<int> list;
        if (isLastItem == false)
        {
            list = Enumerable.Range(1, 52).ToBasicList();
            return list.GetRandomItem();
        }
        WeightedAverageLists<int> weights = new();
        //first 4 are most likely.
        // Higher weight for numbers 1-16
        list = Enumerable.Range(1, 16).ToBasicList();
        if (RummyDiceHandVM.Phase != 7)
        {
            weights.AddWeightedItem(list, 100);
        }
        else
        {
            weights.AddWeightedItem(list, 200);
        }
        // Lower weight for 17-28
        list = Enumerable.Range(17, 12).ToBasicList();
        weights.AddWeightedItem(list, 80);


        if (RummyDiceHandVM.Phase != 7)
        {
            // Even lower weight for 29-40

            list = Enumerable.Range(29, 12).ToBasicList();
            weights.AddWeightedItem(list, 50);

            // Even more rare for 41-48
            list = Enumerable.Range(41, 8).ToBasicList();
            weights.AddWeightedItem(list, 30);

        }
        else
        {
            list = Enumerable.Range(29, 12).ToBasicList();
            weights.AddWeightedItem(list, 20);

            // Even more rare for 41-48
            list = Enumerable.Range(41, 8).ToBasicList();
            weights.AddWeightedItem(list, 10);
        }

        list = Enumerable.Range(49, 4).ToBasicList();
        weights.AddWeightedItem(list, GetWildWeights());
        return weights.GetRandomWeightedItem();
    }
    private static int GetWildWeights()
    {
        BasicList<int> list;
        if (RummyDiceHandVM.Phase == 0)
        {
            throw new CustomBasicException("Must specify the phase now");
        }
        if (RummyDiceHandVM.Phase == 4)
        {
            return 130;
        }
        if (RummyDiceHandVM.Phase == 5)
        {
            return 140;
        }
        if (RummyDiceHandVM.Phase == 6)
        {
            return 150;
        }
        if (RummyDiceHandVM.Phase == 8)
        {
            return 20;
        }
        if (RummyDiceHandVM.Phase == 1)
        {
            list = Enumerable.Range(20, 60).ToBasicList();
            return list.GetRandomItem(); //may get lower weight but could get higher weight.
        }
        if (RummyDiceHandVM.Phase == 7)
        {
            list = Enumerable.Range(40, 70).ToBasicList();
            return list.GetRandomItem(); //you get a higher chance of wild but could be lower.
        }
        if (RummyDiceHandVM.Phase == 10)
        {
            list = Enumerable.Range(80, 120).ToBasicList();
            return list.GetRandomItem();
        }
        list = Enumerable.Range(70, 100).ToBasicList();
        return list.GetRandomItem();
    }
}