namespace MonopolyDicedGame.Core.Logic;
[SingletonGame]
[AutoReset]
public class MonopolyDiceSet(IGamePackageResolver resolver, IGameNetwork network, TestOptions test, CommandContainer command) : IRollMultipleDice<BasicDiceModel>, ISerializable
{
    public IGamePackageResolver? MainContainer { get; set; } = resolver;
    public IGamePackageGeneratorDI? GeneratorContainer { get; set; }
    public BasicList<BasicDiceModel> DiceList { get; set; } = [];
    public Func<MonopolyDicedGameSaveInfo>? SaveRoot { get; set; }


    //for now, use its dicelist.
    //later will rethink.
    private IAsyncDelayer? _delay;

    public async Task<BasicList<BasicList<BasicDiceModel>>> GetDiceList(string content)
    {
        return await js1.DeserializeObjectAsync<BasicList<BasicList<BasicDiceModel>>>(content);
    }
    public BasicList<BasicList<BasicDiceModel>> RollDice(int howManySections = 6)
    {
        int newNum;
        
        DiceList.Clear(); //has to clear until i make more progress.
        newNum = 8; //go ahead and try 8 for this.

        newNum -= SaveRoot!.Invoke().Owns.Count;

        AsyncDelayer.SetDelayer(this, ref _delay!);
        IDiceContainer<int> thisG = MainContainer!.Resolve<IDiceContainer<int>>();
        thisG.MainContainer = MainContainer;
        GlobalDiceHelpers.OwnedOnBoard = SaveRoot!.Invoke().Owns;
        BasicList<BasicList<BasicDiceModel>> output = [];
        howManySections.Times(() =>
        {
            GlobalDiceHelpers.OwnWhenRolling.Clear();
            BasicList<BasicDiceModel> tempCol = [];
            newNum.Times(() =>
            {
                var list = thisG.GetPossibleList;
                var item = list.GetRandomItem();
                BasicDiceModel dice = new();
                dice.Populate(item);
                OwnedModel owner = new();
                if (dice.WhatDice == EnumBasicType.Chance)
                {
                    owner.WasChance = true;
                }
                else if (dice.Index == 9)
                {
                    owner.UsedOn = EnumBasicType.Utility;
                    owner.Utility = EnumUtilityType.Water;
                }
                else if (dice.Index == 10)
                {
                    owner.UsedOn = EnumBasicType.Utility;
                    owner.Utility = EnumUtilityType.Electric;
                }
                else if (dice.Index == 11)
                {
                    owner.UsedOn = EnumBasicType.Railroad;
                }
                else if (dice.Index <= 8)
                {
                    owner.Group = dice.Group;
                }
                GlobalDiceHelpers.OwnWhenRolling.Add(owner);
                tempCol.Add(dice);
            });
            output.Add(tempCol);
        });
        return output;
    }
    public async Task SendMessageAsync(BasicList<BasicList<BasicDiceModel>> thisList)
    {
        await SendMessageAsync("rolled", thisList); //well see.
    }
    public async Task SendMessageAsync(string category, BasicList<BasicList<BasicDiceModel>> thisList)
    {
        await network.SendAllAsync(category, thisList); //i think
    }
    public async Task ShowRollingAsync(BasicList<BasicList<BasicDiceModel>> thisCol)
    {
        AsyncDelayer.SetDelayer(this, ref _delay!);
        await thisCol.ForEachAsync(async firsts =>
        {
            DiceList.ReplaceRange(firsts);
            command.UpdateSpecificAction("monopolydice"); //i think.
            if (test.NoAnimations == false)
            {
                await _delay.DelayMilli(50);
            }
        });
        DiceList.Sort();
    }
}