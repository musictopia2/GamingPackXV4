namespace BasicGameFrameworkLibrary.Core.ChooserClasses;
public interface IListViewPicker
{
    ICustomCommand ItemSelectedCommand { get; }
    BasicList<ListPieceModel> TextList { get; }
}