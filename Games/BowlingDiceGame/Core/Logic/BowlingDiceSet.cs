namespace BowlingDiceGame.Core.Logic;
[SingletonGame]
[AutoReset]
public class BowlingDiceSet(IGameNetwork thisNet) : IRollMultipleDice<bool>, ISerializable
{
    public IGamePackageResolver? MainContainer { get; set; }
    public IGamePackageGeneratorDI? GeneratorContainer { get; set; }

    private IAsyncDelayer? _delay;
    public async Task<BasicList<BasicList<bool>>> GetDiceList(string payLoad)
    {
        return await js1.DeserializeObjectAsync<BasicList<BasicList<bool>>>(payLoad);
    }
    public async Task LoadGameAsync(string payLoad)
    {
        if (DiceList.Count != 10)
        {
            throw new CustomBasicException("You have to already have 10 dice");
        }
        BasicList<bool> list = await js1.DeserializeObjectAsync<BasicList<bool>>(payLoad);
        if (list.Count != 10)
        {
            throw new CustomBasicException("You had to saved 10 items");
        }
        int x = 0;
        list.ForEach(items =>
        {
            DiceList[x].DidHit = items;
            DiceList[x].Value = items; //i think this was needed too.
            x++;
        });
    }
    public async Task<string> SaveGameAsync()
    {
        BasicList<bool> thisList = DiceList.Select(Items => Items.DidHit).ToBasicList();
        return await js1.SerializeObjectAsync(thisList);
    }
    public BasicList<BasicList<bool>> RollDice(int howManySections = 7)
    {
        if (DiceList.Count == 0)
        {
            throw new CustomBasicException("There are no dice to even roll.  Try FirstLoad");
        }
        int counts = DiceList.Count(Items => Items.Value == false);
        BasicList<BasicList<bool>> output = [];
        AsyncDelayer.SetDelayer(this, ref _delay!);
        IDiceContainer<bool> thisG = MainContainer!.Resolve<IDiceContainer<bool>>();
        thisG.MainContainer = MainContainer;
        howManySections.Times(x =>
        {
            BasicList<bool> firsts = [];
            counts.Times(() =>
            {
                bool lasts = x == howManySections;
                firsts.Add(thisG.GetRandomDiceValue(lasts));
            });
            output.Add(firsts);
        });
        return output;
    }

    public readonly BasicList<SingleDiceInfo> DiceList = [];

    public async Task SendMessageAsync(BasicList<BasicList<bool>> thisList)
    {
        await SendMessageAsync("rolled", thisList);
    }
    public async Task SendMessageAsync(string category, BasicList<BasicList<bool>> thisList)
    {
        await thisNet.SendAllAsync(category, thisList); //i think
    }
    public Task ShowRollingAsync(BasicList<BasicList<bool>> thisCol)
    {
        bool isLast = false;
        AsyncDelayer.SetDelayer(this, ref _delay!); //has to be here to learn lesson from other dice games.
        thisCol.ForEach(firstList =>
        {
            if (thisCol.Last() == firstList)
            {
                isLast = true;
            }
            int x; //this had no animations so should be okay this time.
            x = 0;
            firstList.ForEach(items =>
            {
                SingleDiceInfo thisDice = FindNextPin(ref x);
                thisDice.Populate(items);
                thisDice.Index = DiceList.IndexOf(thisDice) + 1;
                if (isLast == true)
                {
                    thisDice.DidHit = items;
                }
            });
        });
        if (isLast == false)
        {
            throw new CustomBasicException("Was never last for showing rolling.  Rethink");
        }
        return Task.CompletedTask;
    }
    public void ClearDice()
    {
        if (DiceList.Count != 10)
        {
            throw new CustomBasicException("You had to have 10 dice.  Otherwise, can't clear");
        }
        DiceList.ForEach(Items =>
        {
            Items.DidHit = false;
            Items.Value = false;
        });
    }
    public void FirstLoad()
    {
        10.Times(() =>
        {
            DiceList.Add(new SingleDiceInfo());
        });
    }
    public int HowManyHit()
    {
        return DiceList.Count(items => items.DidHit == true);
    }
    private SingleDiceInfo FindNextPin(ref int previous)
    {
        if (previous > 10)
        {
            throw new CustomBasicException($"Cannot find the next pin because its already upto 10.  The number chosen is {previous}");
        }
        if (previous < 0)
        {
            throw new CustomBasicException($"Must be at least 0 to find the next pin");
        }
        int starts = previous + 1;
        for (int y = starts; y <= 10; y++)
        {
            var thisDice = DiceList[y - 1];
            if (thisDice.DidHit == false)
            {
                previous = y;
                return thisDice;
            }
        }
        throw new CustomBasicException("There was no other pins that has not been hit.  Therefore, there must be a problem");
    }
}