namespace BasicGameFrameworkLibrary.Core.ColorCards;
public interface IColorCard : IDeckObject, ISimpleValueObject<int>
    , IColorObject<EnumColorTypes>
{
    EnumColorTypes Color { get; set; }
    string Display { get; set; }
}