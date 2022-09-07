namespace BasicGameFrameworkLibrary.Core.Dice;
public interface IBasicDice<T> : IVisibleObject, IPopulateObject<T> where T : IConvertible
{
    int HeightWidth { get; }
    T Value { get; set; }
    int Index { get; set; }
}