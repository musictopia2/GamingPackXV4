namespace BasicGameFrameworkLibrary.Core.GamePieceModels;

[UseStateChanged]
public partial class ListPieceModel : ISimpleValueObject<int>, ISelectableObject, IEnabledObject
{
    
    public int Index { get; set; }

    [StateProperty]
    public partial string DisplayText { get; set; } = "";
    [StateProperty]
    public partial bool IsEnabled { get; set; }
    [StateProperty]
    public partial bool IsSelected { get; set; }
    //public string DisplayText
    //{
    //    get => _displayText;
    //    set
    //    {
    //        if (_displayText == value)
    //        {
    //            return;
    //        }

    //        _displayText = value;
    //        StateChanged?.Invoke();
    //    }
    //}

    //public bool IsEnabled
    //{
    //    get => _isEnabled;
    //    set
    //    {
    //        if (_isEnabled == value)
    //        {
    //            return;
    //        }

    //        _isEnabled = value;
    //        StateChanged?.Invoke();
    //    }
    //}

    //public bool IsSelected
    //{
    //    get => _isSelected;
    //    set
    //    {
    //        if (_isSelected == value)
    //        {
    //            return;
    //        }

    //        _isSelected = value;
    //        StateChanged?.Invoke();
    //    }
    //}

    //public event Action? StateChanged;

    int ISimpleValueObject<int>.ReadMainValue => Index;
}