namespace BasicGameFrameworkLibrary.Core.GamePieceModels;

[UseStateChanged]
public partial class NumberModel : ISimpleValueObject<int>, ISelectableObject, IEnabledObject
{
    public int NumberValue { get; set; } = -1; //defaults to -1 which means nothing
    [StateProperty]
    public partial bool IsEnabled { get; set; }
    [StateProperty]
    public partial bool IsSelected { get; set; }
    int ISimpleValueObject<int>.ReadMainValue => NumberValue;
}