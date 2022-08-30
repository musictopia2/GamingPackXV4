namespace BladesOfSteel.Core.Logic;
[SingletonGame]
public class BladesOfSteelScreenDelegates
{
    internal Func<Task>? ReloadFaceoffAsync { get; set; }
    internal Func<Task>? LoadMainGameAsync { get; set; }
}