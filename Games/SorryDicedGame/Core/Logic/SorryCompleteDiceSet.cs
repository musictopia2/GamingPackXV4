namespace SorryDicedGame.Core.Logic;
[SingletonGame]
[AutoReset]
public class SorryCompleteDiceSet(IGamePackageResolver resolver, SorryDicedGameGameContainer gameContainer)
    : IRollMultipleDice<SorryDiceModel>, ISerializable
{
    public IGamePackageResolver? MainContainer { get; set; } = resolver;
    public IGamePackageGeneratorDI? GeneratorContainer { get; set; } //did not need before so hopefully okay.
    public async Task<BasicList<BasicList<SorryDiceModel>>> GetDiceList(string content)
    {
        return await js1.DeserializeObjectAsync<BasicList<BasicList<SorryDiceModel>>>(content);
    }
    public BasicList<BasicList<SorryDiceModel>> RollDice(int howManySections = 6)
    {
        IGenerateDice<int> thisG = MainContainer!.Resolve<IGenerateDice<int>>();
        //thisG.MainContainer = MainContainer;
        BasicList<BasicList<SorryDiceModel>> output = [];
        howManySections.Times(() =>
        {
            GlobalDiceHelpers.HowManyWilds = 0;
            GlobalDiceHelpers.HowManySlides = 0;
            GlobalDiceHelpers.HowManySorrys = 0;
            BasicList<SorryDiceModel> tempCol = [];
            3.Times(() =>
            {
                var list = thisG.GetPossibleList;
                var item = list.GetRandomItem();
                SorryDiceModel dice = new();
                dice.Populate(item);
                if (dice.Category == EnumDiceCategory.Sorry)
                {
                    GlobalDiceHelpers.HowManySorrys++;
                }
                if (dice.Category == EnumDiceCategory.Slide)
                {
                    GlobalDiceHelpers.HowManySlides++;
                }
                if (dice.Category == EnumDiceCategory.Wild)
                {
                    GlobalDiceHelpers.HowManyWilds++;
                }
                tempCol.Add(dice);
            });
            output.Add(tempCol);
        });
        return output;
    }
    public async Task SendMessageAsync(string category, BasicList<BasicList<SorryDiceModel>> thisList)
    {
        await gameContainer.Network!.SendAllAsync(category, thisList); //i think
    }
    public async Task ShowRollingAsync(BasicList<BasicList<SorryDiceModel>> thisCol)
    {
        await thisCol.ForEachAsync(async firsts =>
        {
            gameContainer.SaveRoot.DiceList.ReplaceRange(firsts);
            gameContainer.Command.UpdateSpecificAction("sorrydice"); //i think.
            if (gameContainer.Test.NoAnimations == false)
            {
                await gameContainer.Delay.DelayMilli(50);
            }
        });
    }
}