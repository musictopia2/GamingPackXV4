namespace SorryDicedGame.Core.Data;
public class SorryDiceModel : IBasicDice<int>, IGenerateDice<int>
{
    public int HeightWidth => 40;
    public int Value { get; set; }
    public int Index { get; set; }
    public bool Visible { get; set; }
    public EnumDiceCategory Category { get; set; } = EnumDiceCategory.None;
    public EnumColorChoice Color { get; set; } //would be none if category is not color.
    private static int GetFrequency(int index)
    {
        if (index == 5)
        {
            return 2 - GlobalDiceHelpers.HowManyWilds; //only 2 at the most;
        }
        if (index == 6)
        {
            return 2 - GlobalDiceHelpers.HowManySlides;
        }
        if (index == 7)
        {
            return 2 - GlobalDiceHelpers.HowManySorrys;
        }
        throw new CustomBasicException("Only 5, 6, 7 for frequency is supported");
    }
    public BasicList<int> GetPossibleList
    {
        get
        {
            WeightedAverageLists<int> weights = new();
            4.Times(x =>
            {
                weights.AddWeightedItem(x, 3);
            });
            3.Times(x =>
            {
                int y = x + 4;
                int howOften = GetFrequency(y);
                if (howOften > 0)
                {
                    weights.AddWeightedItem(y, howOften);
                }
            });
            return weights.GetWeightedList();
        }
    }

    public void Populate(int chosen)
    {
        Index = chosen;
        Color = EnumColorChoice.None;
        if (chosen < 1)
        {
            throw new CustomBasicException("Must choose at least 1");
        }
        if (chosen <= 4)
        {
            Category = EnumDiceCategory.Color;
            Color = EnumColorChoice.FromValue(chosen);
            return;
        }
        if (chosen == 5)
        {
            Category = EnumDiceCategory.Wild;
            return;
        }
        if (chosen == 6)
        {
            Category = EnumDiceCategory.Slide;
            return;
        }
        if (chosen == 7)
        {
            Category = EnumDiceCategory.Sorry;
            return;
        }
        throw new CustomBasicException("Only 1 to 7 for dice is supported");
    }
}