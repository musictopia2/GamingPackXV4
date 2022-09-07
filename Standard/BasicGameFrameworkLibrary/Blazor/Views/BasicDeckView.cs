namespace BasicGameFrameworkLibrary.Blazor.Views;
public class BasicDeckView<V> : BasicGameView<V>
    where V : ScreenViewModel, IBlankGameVM
{
    [CascadingParameter]
    private int TargetHeight { get; set; }
    protected string HeightString => $"{TargetHeight}vh";
}