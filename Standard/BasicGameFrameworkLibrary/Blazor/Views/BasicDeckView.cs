namespace BasicGameFrameworkLibrary.Blazor.Views;
public class BasicDeckView<V> : CascadingGameView<V>
{
    [CascadingParameter]
    private int TargetHeight { get; set; }
    protected string HeightString => $"{TargetHeight}vh";
}