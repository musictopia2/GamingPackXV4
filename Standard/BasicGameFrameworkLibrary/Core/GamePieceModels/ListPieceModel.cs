namespace BasicGameFrameworkLibrary.Core.GamePieceModels;

public class ListPieceModel : ISimpleValueObject<int>, ISelectableObject, IEnabledObject
{
    public int Index { get; set; }
    public string DisplayText { get; set; } = "";
    public bool IsEnabled { get; set; }
    public bool IsSelected { get; set; }
    int ISimpleValueObject<int>.ReadMainValue => Index;
}