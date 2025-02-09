namespace ThinkTwice.Core.Logic;
[SingletonGame]
[AutoReset]
public class Multiplier(ThinkTwiceGameContainer gameContainer) : ICompleteSingleDice<int>, ISerializable
{
    public int Value { get; set; } = -1;
    public bool Visible { get; set; }
    public int Index { get; set; }
    public int HeightWidth { get; } = 60;
    public void LoadSavedGame()
    {
        if (gameContainer.SaveRoot!.WhichMulti == -1)
        {
            return;
        }
        Value = gameContainer.SaveRoot.WhichMulti;
        Visible = true;
    }
    public void NewTurn()
    {
        Value = -1;
        Visible = false;
        gameContainer.SaveRoot!.WhichMulti = -1;
    }
    public IGamePackageResolver? MainContainer { get; set; } = gameContainer.Resolver;
    public IGamePackageGeneratorDI? GeneratorContainer { get; set; }
    public async Task SendMessageAsync(string category, BasicList<int> thisList)
    {
        await gameContainer.Network!.SendAllAsync(category, thisList);
    }
    public async Task ShowRollingAsync(BasicList<int> thisCol)
    {
        Visible = true;
        await thisCol.ForEachAsync(async items =>
        {
            Populate(items);
            gameContainer.Command.UpdateAll();
            await gameContainer.Delay.DelaySeconds(.07);
        });
    }
    public async Task<BasicList<int>> GetDiceList(string content)
    {
        return await js1.DeserializeObjectAsync<BasicList<int>>(content);
    }
    public BasicList<int> RollDice(int howManySections = 7)
    {
        return GetSingleRolledDice(howManySections, this);
    }
    public void Populate(int chosen)
    {
        Value = chosen;
    }
    int IGenerateDice<int>.GetRandomDiceValue(bool isLastItem)
    {
        if (isLastItem == false)
        {
            BasicList<int> list =
                [
                0, 0, 0, 1, 1, 1, 2, 2, 2, 3, 3, 4, 4 ,5, 6
                ];
            return list.GetRandomItem();
        }
        WeightedAverageLists<int> thisWeight = new();
        thisWeight.AddWeightedItem(0, 10, 30)
            .AddWeightedItem(1, 35)
            .AddWeightedItem(2, 20, 40)
            .AddWeightedItem(3, 10, 20)
            .AddWeightedItem(4, 3)
            .AddWeightedItemWithChance(5, 10, 1, 2)
            .AddWeightedItemWithChance(6, 30, 1, 1)
            ;
        return thisWeight.GetRandomWeightedItem();
    }

    Task IRollSingleDice<int>.ShowRollingAsync(CommonBasicLibraries.CollectionClasses.BasicList<int> thisCol)
    {
        throw new NotImplementedException();
    }

    Task IRollSingleDice<int>.SendMessageAsync(string category, CommonBasicLibraries.CollectionClasses.BasicList<int> thisList)
    {
        throw new NotImplementedException();
    }

    Task<CommonBasicLibraries.CollectionClasses.BasicList<int>> IRollSingleDice<int>.GetDiceList(string content)
    {
        throw new NotImplementedException();
    }

    CommonBasicLibraries.CollectionClasses.BasicList<int> IRollSingleDice<int>.RollDice(int howManySections)
    {
        throw new NotImplementedException();
    }

    void IGenerateDice<int>.StartRoll()
    {
        
    }
}