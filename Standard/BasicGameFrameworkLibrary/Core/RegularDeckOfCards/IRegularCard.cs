namespace BasicGameFrameworkLibrary.Core.RegularDeckOfCards;
public interface IRegularCard : IDeckObject, ISimpleValueObject<int>, IWildObject
    , ISuitObject<EnumSuitList>, IColorObject<EnumRegularColorList>, IAdvancedDIContainer
{
    EnumRegularColorList Color { get; set; }
    EnumSuitList Suit { get; set; }
    EnumRegularCardValueList Value { get; set; }
    EnumSuitList DisplaySuit { get; set; }
    EnumRegularCardValueList DisplayNumber { get; set; }
    int Section { get; }
    EnumRegularCardTypeList CardType { get; set; }
}