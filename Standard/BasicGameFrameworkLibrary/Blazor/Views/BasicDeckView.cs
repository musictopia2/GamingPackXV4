namespace BasicGameFrameworkLibrary.Blazor.Views;
public class BasicDeckView<V> : BasicGameView<V>
    where V : ScreenViewModel, IBlankGameVM
{
    [CascadingParameter]
    private int TargetHeight { get; set; }
    protected string HeightString => $"{TargetHeight}vh"; //if everything works out well then don't need the event aggrevation anymore.
}