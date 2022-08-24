namespace BasicGameFrameworkLibrary.Blazor.Views;
public abstract partial class BasicCustomButtonView<V>
    where V : ScreenViewModel, IBlankGameVM
{
    protected abstract ICustomCommand Command { get; }
    protected abstract string DisplayName { get; }
}