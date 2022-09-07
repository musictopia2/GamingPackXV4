namespace BasicGameFrameworkLibrary.Core.GamePieceModels;
public class NumberModel : ISimpleValueObject<int>, ISelectableObject, IEnabledObject
{
    public int NumberValue { get; set; } = -1; //defaults to -1 which means nothing
    public bool IsEnabled { get; set; }
    public bool IsSelected { get; set; }
    int ISimpleValueObject<int>.ReadMainValue => NumberValue;
}