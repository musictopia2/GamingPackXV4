namespace BasicGameFrameworkLibrary.Core.Dice;
public interface IStandardDice : IBasicDice<int>, IHoldDice, ICommonObject, IStateChanged
{
    string FillColor { get; }
    string DotColor { get; }
    EnumDiceStyle Style { get; }
}