namespace Fluxx.Blazor;
public class CompleteContainerClass
{
    public CompleteContainerClass()
    {
        GameContainer = aa.Resolver!.Resolve<FluxxGameContainer>();
        KeeperContainer = aa.Resolver.Resolve<KeeperContainer>();
        ActionContainer = aa.Resolver.Resolve<ActionContainer>();
        GameData = aa.Resolver.Resolve<FluxxVMData>();
    }
    public FluxxGameContainer GameContainer { get; set; }
    public KeeperContainer KeeperContainer { get; set; }
    public ActionContainer ActionContainer { get; set; }
    public FluxxVMData GameData { get; set; }
}