namespace MilkRun.Core.Cards;
public class MilkRunCardInformation : SimpleDeckObject, IDeckObject, IComparable<MilkRunCardInformation>
{
    public MilkRunCardInformation()
    {
        DefaultSize = new SizeF(55, 72);
    }
    public EnumMilkType MilkCategory { get; set; }
    public int Points { get; set; }
    public static int HowManyKinds => 71;
    public EnumCardCategory CardCategory { get; set; }
    public void Populate(int chosen)
    {
        if (chosen <= HowManyKinds)
        {
            ModifyData(chosen, EnumMilkType.Strawberry);
        }
        else
        {
            ModifyData(chosen, EnumMilkType.Chocolate);
        }
    }
    private void ModifyData(int chosen, EnumMilkType tempMilk)
    {
        MilkCategory = tempMilk;
        Deck = chosen;
        int tempDeck;
        //66
        if (Deck <= HowManyKinds)
        {
            tempDeck = chosen;
        }
        else
        {
            tempDeck = chosen - HowManyKinds;
        }
        int c;
        int z = 0;
        for (int x = 1; x <= 12; x++)
        {
            c = GetPointFreq(x);
            for (int y = 1; y <= c; y++)
            {
                z++;
                if (z == tempDeck)
                {
                    CardCategory = EnumCardCategory.Points;
                    Points = x;
                    return;
                }
            }
        }
        //7 to 20
        //starts at 50

        for (int x = 1; x <= 14; x++)
        {
            z++;
            if (z == tempDeck)
            {
                CardCategory = EnumCardCategory.Go;
                return;
            }
        }
        //starts at 64
        //3 to 6
        for (int x = 1; x <= 5; x++)
        {
            z++;
            if (z == tempDeck)
            {
                CardCategory = EnumCardCategory.Stop;
                return;
            }
        }
        //starts at 69
        for (int x = 1; x <= 3; x++)
        {
            z++;
            if (z == tempDeck)
            {
                CardCategory = EnumCardCategory.Joker;
                return;
            }
        }
        throw new CustomBasicException("cannot find deck.  rethink");
    }
    private static int GetPointFreq(int upTo)
    {
        if (upTo <= 3)
        {
            return 3; //9
        }
        if (upTo <= 8)
        {
            return 6; //30
        }
        if (upTo <= 10)
        {
            return 3; //6
        }
        if (upTo == 11 || upTo == 12)
        {
            return 2; //4
        }
        return 0;
    }
    public void Reset() { }
    int IComparable<MilkRunCardInformation>.CompareTo(MilkRunCardInformation? other)
    {
        if (MilkCategory != other!.MilkCategory)
        {
            return MilkCategory.CompareTo(other.MilkCategory);
        }
        if (CardCategory != other.CardCategory)
        {
            return CardCategory.CompareTo(other.CardCategory);
        }
        return Points.CompareTo(other.Points);
    }
    public override string GetKey()
    {
        return Guid.NewGuid().ToString();
    }
    public override BasicDeckRecordModel GetRecord => new(Deck, IsSelected, Drew, IsUnknown, IsEnabled, Visible, $"{MilkCategory} {CardCategory} {Points}");
}