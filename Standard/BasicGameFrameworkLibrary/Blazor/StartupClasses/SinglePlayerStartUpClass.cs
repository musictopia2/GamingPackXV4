namespace BasicGameFrameworkLibrary.Blazor.StartupClasses;
public class SinglePlayerStartUpClass : IStartUp
{
    public static bool? IsWasm { get; set; }
    void IStartUp.RegisterCustomClasses(IGamePackageDIContainer container, bool multiplayer, BasicData data)
    {
        if (multiplayer == true)
        {
            throw new CustomBasicException("Only single player games are supported for this implementation");
        }
        if (IsWasm.HasValue == false)
        {
            throw new CustomBasicException("Must specify whether its wasm or not");
        }
        if (IsWasm.Value == true)
        {
            container.RegisterType<SinglePlayerNoSave>();
        }
        else
        {
            container.RegisterType<SinglePlayerReleaseNativeFileAccessAutoResume>();
        }
    }
    void IStartUp.StartVariables(BasicData data) { }
}