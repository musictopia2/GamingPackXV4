namespace Fluxx.Blazor;
public class CompleteContainerClass
{
    public CompleteContainerClass()
    {
        GameContainer = Resolver!.Resolve<FluxxGameContainer>();
        KeeperContainer = Resolver.Resolve<KeeperContainer>();
        ActionContainer = Resolver.Resolve<ActionContainer>();
        GameData = Resolver.Resolve<FluxxVMData>();
    }
    public FluxxGameContainer GameContainer { get; set; }
    public KeeperContainer KeeperContainer { get; set; }
    public ActionContainer ActionContainer { get; set; }
    public FluxxVMData GameData { get; set; }
}