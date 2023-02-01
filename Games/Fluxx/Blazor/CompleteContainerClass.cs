namespace Fluxx.Blazor;
public class CompleteContainerClass
{
    public CompleteContainerClass()
    {
        GameContainer = aa1.Resolver!.Resolve<FluxxGameContainer>();
        KeeperContainer = aa1.Resolver.Resolve<KeeperContainer>();
        ActionContainer = aa1.Resolver.Resolve<ActionContainer>();
        GameData = aa1.Resolver.Resolve<FluxxVMData>();
    }
    public FluxxGameContainer GameContainer { get; set; }
    public KeeperContainer KeeperContainer { get; set; }
    public ActionContainer ActionContainer { get; set; }
    public FluxxVMData GameData { get; set; }
}