namespace RummyDice.Core.Logic;
[SingletonGame]
[AutoReset]
public class RummyBoardCP
{
    readonly IAsyncDelayer _delay;
    private readonly BasicData _basicData;
    private readonly CommandContainer _command;
    private readonly TestOptions _test;
    readonly IGenerateDice<int> _gens;
    public async Task SelectDiceAsync(RummyDiceInfo dice)
    {
        if (_command.IsExecuting)
        {
            return;
        }
        _command.IsExecuting = true;
        var list = SaveRoot!.Invoke().DiceList;
        int index = list.IndexOf(dice);
        if (index == -1)
        {
            throw new CustomBasicException("had problems hooking up.  Rethink");
        }
        if (_basicData.MultiPlayer == true)
        {
            await _network!.SendAllAsync("diceclicked", index); //i think
        }
        await SelectOneMainAsync!.Invoke(index);
    }
    private readonly IGameNetwork? _network;
    public RummyBoardCP(TestOptions test,
        IGenerateDice<int> gens,
        IAsyncDelayer delay,
        BasicData basicData,
        CommandContainer command
        )
    {
        _test = test;
        _gens = gens; //hopefully putting here is acceptable
        _delay = delay;
        _basicData = basicData;
        _command = command;
        _network = _basicData.GetNetwork();
    }
    #region "Delegates"
    public Func<RummyDiceSaveInfo>? SaveRoot { get; set; } //keep delegates in here.
    public Func<int, Task>? SelectOneMainAsync { get; set; }
    #endregion
    public void EndTurn()
    {
        _doStart = true;
        SaveRoot!.Invoke().DiceList.Clear();
    }
    private bool _doStart = true;
    public void SelectDice(int whichOne)
    {
        SaveRoot!.Invoke().DiceList[whichOne].IsSelected = !SaveRoot!.Invoke().DiceList[whichOne].IsSelected;
    }
    private void UnselectAll()
    {
        SaveRoot!.Invoke().DiceList.UnselectAllObjects();
    }
    public BasicList<BasicList<RummyDiceInfo>> RollDice()
    {
        int newNum;
        if (SaveRoot!.Invoke().RollNumber == 1)
        {
            newNum = 10;
            SaveRoot!.Invoke().DiceList.Clear();
        }
        else
        {
            newNum = SaveRoot!.Invoke().DiceList.Count;
        }
        BasicList<BasicList<RummyDiceInfo>> output = new();
        BasicList<RummyDiceInfo> tempCol;
        BasicList<int> possibleList = _gens.GetPossibleList;
        RummyDiceInfo thisDice;
        7.Times(x =>
        {
            tempCol = new();
            newNum.Times(y =>
            {
                thisDice = new RummyDiceInfo();
                thisDice.Populate(possibleList.GetRandomItem());
                tempCol.Add(thisDice);
            });
            output.Add(tempCol);
        });
        return output;
    }
    public async Task ShowRollingAsync(BasicList<BasicList<RummyDiceInfo>> diceCollection)
    {
        int delay;
        if (_doStart)
        {
            delay = 100;
        }
        else
        {
            delay = 50;
        }
        await diceCollection.ForEachAsync(async thisList =>
        {
            SaveRoot!.Invoke().DiceList.ReplaceRange(thisList);
            _command.UpdateSpecificAction("rummydice");
            if (_test.NoAnimations == false)
            {
                await _delay.DelayMilli(delay);
            }
        });
        SaveRoot!.Invoke().DiceList.Sort();
    }
    public void AddBack(BasicList<RummyDiceInfo> thisList)
    {
        SaveRoot!.Invoke().DiceList.AddRange(thisList);
        if (thisList.Count == 0)
        {
            return;
        }
        UnselectAll();
        SaveRoot!.Invoke().DiceList.Sort();
    }
    public BasicList<RummyDiceInfo> GetSelectedList()
    {
        BasicList<RummyDiceInfo> output = SaveRoot!.Invoke().DiceList.RemoveAllAndObtain(xx => xx.IsSelected == true).ToBasicList();
        SaveRoot!.Invoke().DiceList.Sort();
        return output;
    }
    public bool HasSelectedDice()
    {
        return SaveRoot!.Invoke().DiceList.Any(xx => xx.IsSelected == true);
    }
}