namespace MonopolyDicedGame.Core.Logic;
[SingletonGame]
[AutoReset]
public class MonopolyDiceSet(IGamePackageResolver resolver, MonopolyDicedGameGameContainer gameContainer) : IRollMultipleDice<BasicDiceModel>, ISerializable
{
    public IGamePackageResolver? MainContainer { get; set; } = resolver;
    public IGamePackageGeneratorDI? GeneratorContainer { get; set; }
    public async Task<BasicList<BasicList<BasicDiceModel>>> GetDiceList(string content)
    {
        return await js1.DeserializeObjectAsync<BasicList<BasicList<BasicDiceModel>>>(content);
    }
    public BasicList<BasicList<BasicDiceModel>> RollDice(int howManySections = 6)
    {
        int newNum;
        newNum = 8; //go ahead and try 8 for this.
        newNum -= gameContainer.SaveRoot.Owns.Count;
        IDiceContainer<int> thisG = MainContainer!.Resolve<IDiceContainer<int>>();
        thisG.MainContainer = MainContainer;
        GlobalDiceHelpers.OwnedOnBoard = gameContainer.SaveRoot.Owns;
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
    //public async Task SendMessageAsync(BasicList<BasicList<BasicDiceModel>> thisList)
    //{
    //    await SendMessageAsync("rolled", thisList); //well see.
    //}
    public async Task SendMessageAsync(string category, BasicList<BasicList<BasicDiceModel>> thisList)
    {
        await gameContainer.Network!.SendAllAsync(category, thisList); //i think
    }
    public async Task ShowRollingAsync(BasicList<BasicList<BasicDiceModel>> thisCol)
    {
        await thisCol.ForEachAsync(async firsts =>
        {
            gameContainer.SaveRoot.DiceList.ReplaceRange(firsts);
            gameContainer.Command.UpdateSpecificAction("monopolydice"); //i think.
            if (gameContainer.Test.NoAnimations == false)
            {
                await gameContainer.Delay.DelayMilli(50);
            }
        });
        gameContainer.SaveRoot.DiceList.Sort();
    }
    public void SelectDice(int whichOne)
    {
        gameContainer.SaveRoot.DiceList[whichOne].IsSelected = !gameContainer.SaveRoot.DiceList[whichOne].IsSelected;
    }
    public async Task SelectDiceAsync(BasicDiceModel dice)
    {
        if (gameContainer.Command.IsExecuting)
        {
            return;
        }
        gameContainer.Command.IsExecuting = true; //if i don't have as command, has to do this way.
        var list = gameContainer.SaveRoot.DiceList;
        int index = list.IndexOf(dice);
        if (index == -1)
        {
            throw new CustomBasicException("had problems hooking up.  Rethink");
        }
        if (gameContainer.BasicData.MultiPlayer == true)
        {
            await gameContainer.Network!.SendAllAsync("diceclicked", index); //i think
        }
        await gameContainer.SelectOneMainAsync!.Invoke(index); //same idea from rummy dice game.
    }
    public bool HasSelectedDice()
    {
        return gameContainer.SaveRoot.DiceList.Any(xx => xx.IsSelected == true);
    }
}