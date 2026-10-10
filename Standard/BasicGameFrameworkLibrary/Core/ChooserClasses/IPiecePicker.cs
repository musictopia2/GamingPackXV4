namespace BasicGameFrameworkLibrary.Core.ChooserClasses;
public interface IPiecePicker
{
    bool CanHighlight { get; }
    string TextHeight { get; }
    float TextWidth { get; }
    float ViewBoxHeight { get; }
}