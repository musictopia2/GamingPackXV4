namespace ThinkTwice.Core.Logic;
[SingletonGame]
[AutoReset]
public class Multiplier : ICompleteSingleDice<int>, ISerializable
{
    public int Value { get; set; } = -1;
    public bool Visible { get; set; }
    public int Index { get; set; }
    readonly ThinkTwiceGameContainer _gameContainer;
    public int HeightWidth { get; } = 60;
    public void LoadSavedGame()
    {
        if (_gameContainer.SaveRoot!.WhichMulti == -1)
        {
            return;
        }
        Value = _gameContainer.SaveRoot.WhichMulti;
        Visible = true;
    }
    public void NewTurn()
    {
        Value = -1;
        Visible = false;
        _gameContainer.SaveRoot!.WhichMulti = -1;
    }
    public IGamePackageResolver? MainContainer { get; set; }
    public Multiplier(ThinkTwiceGameContainer gameContainer)
    {
        MainContainer = gameContainer.Resolver;
        _gameContainer = gameContainer;
    }
    public BasicList<int> GetPossibleList
    {
        get
        {
            WeightedAverageLists<int> thisWeight = new();
            thisWeight.AddSubItem(0, 200).AddSubItem(5, 5);
            BasicList<int> sixList = thisWeight.GetSubList();
            thisWeight.AddSubItem(0, 80).AddSubItem(2, 20);
            BasicList<int> fiveList = thisWeight.GetSubList();
            thisWeight.AddWeightedItem(0, 20, 30).AddWeightedItem(1, 35)
            .AddWeightedItem(2, 20, 25)
            .AddWeightedItem(3, 10, 20).AddWeightedItem(4, 3).AddWeightedItem(5, fiveList)
            .AddWeightedItem(6, sixList);
            return thisWeight.GetWeightedList();
        }
    }
    public IGamePackageGeneratorDI? GeneratorContainer { get; set; }
    public async Task SendMessageAsync(string category, BasicList<int> thisList)
    {
        await _gameContainer.Network!.SendAllAsync(category, thisList);
    }
    public async Task ShowRollingAsync(BasicList<int> thisCol)
    {
        Visible = true;
        await thisCol.ForEachAsync(async items =>
        {
            Populate(items);
            _gameContainer.Command.UpdateAll();
            await _gameContainer.Delay.DelaySeconds(.07);
        });
    }
    public async Task<BasicList<int>> GetDiceList(string content)
    {
        return await js.DeserializeObjectAsync<BasicList<int>>(content);
    }
    public BasicList<int> RollDice(int howManySections = 7)
    {
        return GetSingleRolledDice(howManySections, this);
    }
    public void Populate(int chosen)
    {
        Value = chosen;
    }
}