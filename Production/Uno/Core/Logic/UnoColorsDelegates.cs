namespace Uno.Core.Logic;
[SingletonGame]
public class UnoColorsDelegates
{
    internal Func<Task>? CloseColorAsync { get; set; }
    internal Func<Task>? OpenColorAsync { get; set; }
}