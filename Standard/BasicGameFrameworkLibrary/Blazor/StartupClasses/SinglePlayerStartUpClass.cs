using BasicGameFrameworkLibrary.Core.StandardImplementations.AutoResumeNativeFileAccessClasses; //has to be here because not common enough.
namespace BasicGameFrameworkLibrary.Blazor.StartupClasses;
public class SinglePlayerStartUpClass : IStartUp
{
    public static bool? IsWasm { get; set; }
    //hopefully has a way to do another way of doing autoresume for desktop (to help in testing).  besides, that is flexible.
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