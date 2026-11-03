namespace Fluxx.Blazor;
public class SimpleActionView<VM> : ParameterGameView<VM>
    where VM : BasicActionScreen
{
    [CascadingParameter]
    public CompleteContainerClass? CompleteContainer { get; set; }
}