namespace BasicGameFrameworkLibrary.Core.Dice;
public static class SharedDiceRoutines
{
    public static D GetDiceInfo<D>(int chosen, int upTo) where D : IBasicDice<int>, new()
    {
        D output = new();
        output.Index = upTo;
        output.Populate(chosen); 
        return output;
    }
    public static BasicList<BasicList<T>> GetMultipleRolledDice<T>(int howManySections, IGenerateDice<T> dice) where T : IConvertible
    {
        BasicList<BasicList<T>> output = new();
        BasicList<T> thisList = new();
        howManySections.Times(items =>
        {
            thisList.Add(dice.GetRandomDiceValue(items == howManySections));
        });
        output.Add(thisList);
        return output;
    }
    public static BasicList<T> GetSingleRolledDice<T>(int howManySections, IGenerateDice<T> dice) where T : IConvertible
    {
        BasicList<T> thisList = new();
        howManySections.Times(items =>
        {
            thisList.Add(dice.GetRandomDiceValue(items == howManySections));
        });
        return thisList;
    }
}