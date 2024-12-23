namespace ThinkTwice.Core.Logic;
[SingletonGame]
[AutoReset]
public class CategoriesDice : ICompleteSingleDice<string>, IHoldDice, ISerializable
{
    private bool _hold;
    public bool Hold
    {
        get { return _hold; }
        set
        {
            if (SetProperty(ref _hold, value))
            {
                _gameContainer.SaveRoot!.CategoryHeld = value;
            }
        }
    }
    public CategoriesDice(ThinkTwiceGameContainer gameContainer)
    {
        _gameContainer = gameContainer;
        MainContainer = _gameContainer.Resolver;
    }
    readonly ThinkTwiceGameContainer _gameContainer;
    public int HeightWidth { get; } = 60;
    public IGamePackageResolver? MainContainer { get; set; }
    private string _value = "";
    public string Value
    {
        get { return _value; }
        set
        {
            if (SetProperty(ref _value, value))
            {
                switch (value)
                {
                    case "":
                        _gameContainer.SaveRoot!.CategoryRolled = -1;
                        return;
                    case "D":
                        _gameContainer.SaveRoot!.CategoryRolled = 1;
                        return;
                    case "E":
                        _gameContainer.SaveRoot!.CategoryRolled = 2;
                        return;
                    case "H":
                        _gameContainer.SaveRoot!.CategoryRolled = 3;
                        return;
                    case "L":
                        _gameContainer.SaveRoot!.CategoryRolled = 4;
                        return;
                    case "O":
                        _gameContainer.SaveRoot!.CategoryRolled = 5;
                        return;
                    case "S":
                        _gameContainer.SaveRoot!.CategoryRolled = 6;
                        return;
                    default:
                        throw new CustomBasicException("Value Not supported");
                }
            }
        }
    }
    public bool Visible { get; set; }
    public int Index { get; set; }
    private void FillText(int whichValue)
    {
        if (whichValue == 1)
        {
            Value = "D";
        }
        else if (whichValue == 2)
        {
            Value = "E";
        }
        else if (whichValue == 3)
        {
            Value = "H";
        }
        else if (whichValue == 4)
        {
            Value = "L";
        }
        else if (whichValue == 5)
        {
            Value = "O";
        }
        else if (whichValue == 6)
        {
            Value = "S";
        }
        else
        {
            Value = "U"; //hopefully obvious something is wrong.
        }
    }
    public void LoadSavedGame()
    {
        if (_gameContainer.SaveRoot!.CategoryRolled == -1)
        {
            return;
        }
        FillText(_gameContainer.SaveRoot.CategoryRolled);
        if (_gameContainer.SaveRoot.CategoryHeld)
        {
            Hold = true;
        }
        Visible = true;
    }
    public void NewTurn()
    {
        Value = "";
        Visible = false;
        _gameContainer.SaveRoot!.CategorySelected = -1;
    }
    public IGamePackageGeneratorDI? GeneratorContainer { get; set; }

    public async Task ShowRollingAsync(BasicList<string> thisCol)
    {
        if (Hold == true)
        {
            throw new CustomBasicException("Can't show it rolling because you held on to the dice");
        }
        Visible = true;
        await thisCol.ForEachAsync(async category =>
        {
            Populate(category);
            _gameContainer.Command.UpdateAll();
            await _gameContainer.Delay.DelaySeconds(.07);
        });
    }
    public async Task SendMessageAsync(string category, BasicList<string> thisList)
    {
        await _gameContainer.Network!.SendAllAsync(category, thisList);
    }
    public async Task<BasicList<string>> GetDiceList(string content)
    {
        return await js1.DeserializeObjectAsync<BasicList<string>>(content);
    }
    public BasicList<string> RollDice(int HowManySections = 6)
    {
        return GetSingleRolledDice(HowManySections, this);
    }
    public void Populate(string chosen)
    {
        Value = chosen;
    }
    string IGenerateDice<string>.GetRandomDiceValue(bool isLastItem)
    {
        WeightedAverageLists<string> thisWeight = new();
        thisWeight.AddWeightedItem("D", 40, 50).AddWeightedItem("E", 20)
        .AddWeightedItem("O", 25, 30)
        .AddWeightedItem("S", 30).AddWeightedItem("H", 5, 10).AddWeightedItem("L", 35, 40);
        return thisWeight.GetRandomWeightedItem();
    }

    Task IRollSingleDice<string>.ShowRollingAsync(CommonBasicLibraries.CollectionClasses.BasicList<string> thisCol)
    {
        throw new NotImplementedException();
    }

    Task IRollSingleDice<string>.SendMessageAsync(string category, CommonBasicLibraries.CollectionClasses.BasicList<string> thisList)
    {
        throw new NotImplementedException();
    }

    Task<CommonBasicLibraries.CollectionClasses.BasicList<string>> IRollSingleDice<string>.GetDiceList(string content)
    {
        throw new NotImplementedException();
    }

    CommonBasicLibraries.CollectionClasses.BasicList<string> IRollSingleDice<string>.RollDice(int howManySections)
    {
        throw new NotImplementedException();
    }

    void IGenerateDice<string>.StartRoll()
    {
        
    }
}