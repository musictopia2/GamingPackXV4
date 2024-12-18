namespace MonopolyDicedGame.Core.Logic;
[SingletonGame]
[AutoReset]
public class HouseDice(MonopolyDicedGameGameContainer gameContainer) : ICompleteSingleDice<EnumMiscType>, ISerializable
{
    public IGamePackageResolver? MainContainer { get; set; }
    public IGamePackageGeneratorDI? GeneratorContainer { get; set; }
    public int HeightWidth { get; }
    public EnumMiscType Value { get; set; }
    public int Index { get; set; }
    public bool Visible { get; set; }
    EnumMiscType IGenerateDice<EnumMiscType>.GetRandomDiceValue(bool isLastItem)
    {
        WeightedAverageLists<EnumMiscType> weights = new();
        weights.AddWeightedItem(EnumMiscType.RegularHouse, 13)
            .AddWeightedItem(EnumMiscType.BrokenHouse, 2)
            .AddWeightedItem(EnumMiscType.Hotel, 3)
            .AddWeightedItem(EnumMiscType.Free, 4);
        return weights.GetRandomWeightedItem();
    }
    public async Task<BasicList<EnumMiscType>> GetDiceList(string content)
    {
        return await js1.DeserializeObjectAsync<BasicList<EnumMiscType>>(content);
    }
    public void Populate(EnumMiscType chosen)
    {
        Value = chosen; //hopefully this simple.
    }
    public BasicList<EnumMiscType> RollDice(int howManySections = 6)
    {
        return GetSingleRolledDice(howManySections, this);
    }
    public async Task SendMessageAsync(string category, BasicList<EnumMiscType> thisList)
    {
        await gameContainer.Network!.SendAllAsync(category, thisList);
    }
    public async Task ShowRollingAsync(BasicList<EnumMiscType> thisCol)
    {
        Visible = true;
        await thisCol.ForEachAsync(async category =>
        {
            Populate(category);
            gameContainer.Command.UpdateSpecificAction("housedice"); //i think.
            //gameContainer.Command.UpdateAll();
            await gameContainer.Delay.DelayMilli(30);
        });
    }
}