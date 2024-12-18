namespace BowlingDiceGame.Core.Data;
[SingletonGame]
public class SingleDiceInfo : IBasicDice<bool>, IDiceContainer<bool>
{
    public static bool Beginning { get; set; }
    public int HeightWidth { get; } = 60;
    public bool Value { get; set; }
    public bool DidHit { get; set; }
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
        //could rethink further (?)
        //WeightedAverageLists<bool> weight = new WeightedAverageLists<bool>();
        //weight.MainContainer = MainContainer!;
        //weight.AddSubItem(60, 20).AddSubItem(30, 10);
        //weight.FillExtraSubItems(5, 30);
        //weight.AddWeightedItem(false);
        //weight.AddSubItem(700, 100).AddSubItem(400, 50).AddSubItem(70, 40);
        //weight.FillExtraSubItems(70, 100);
        //weight.AddWeightedItem(true);
        //return weight.GetWeightedList();
        IRandomGenerator rs = MainContainer!.Resolve<IRandomGenerator>(); //i need this one.
        if (isLastItem == false)
        {
            return rs.NextBool();
        }
        if (Beginning)
        {
            return rs.NextBool(85); //later rethink
        }
        //later rethink.
        return rs.NextBool(30);
    }
}