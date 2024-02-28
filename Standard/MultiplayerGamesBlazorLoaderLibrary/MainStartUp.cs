using BasicGameFrameworkLibrary.Core.MultiplayerClasses.SavedGameClasses;

namespace MultiplayerGamesBlazorLoaderLibrary;
public class MainStartUp : IStartUp
{
    public static bool? IsWasm { get; set; }
    void IStartUp.RegisterCustomClasses(IGamePackageDIContainer container, bool multiplayer, BasicData data)
    {
        if (IsWasm.HasValue == false)
        {
            throw new CustomBasicException("Must figure out if its wasm since autoresume classes are different for wasm");
        }
        if (data.GamePackageMode == EnumGamePackageMode.Debug)
        {
            throw new CustomBasicException("Only production is supported.");
        }
        if (multiplayer)
        {
            if (IsWasm.Value == true)
            {
                container.RegisterType<MultiplayerReleaseAutoResume>();
                container.RegisterType<PrivateAutoResume>(); //hopefully this still work (?)
            }
            else
            {
                container.RegisterType<MultiPlayerReleaseNativeFileAccessAutoResume>();
                container.RegisterType<NoPrivateSave>();
            }
            container.RegisterType<NetworkStartUp>(); //i think i need this here too (?)
        }
        else
        {
            if (IsWasm.Value == true)
            {
                container.RegisterType<SinglePlayerNoSave>();
            }
            else
            {
                container.RegisterType<SinglePlayerReleaseNativeFileAccessAutoResume>();
            }
        }
        Core.DIFinishProcesses.GlobalDIFinishClass.FinishDIRegistrations(container); //had to have this twice.  if it worked fine for test game package, should work fine here too.
    }
    void IStartUp.StartVariables(BasicData data)
    {
        if (GlobalDataModel.DataContext == null)
        {
            throw new CustomBasicException("Must have the data filled out in order to get the nick names");
        }
        //can still be iffy for now though (?)
        data.NickName = GlobalDataModel.DataContext.NickName; //looks like needs this.
        if (OS == EnumOS.Wasm)
        {
            PrivateAutoResumeLocalStorageHelpers.RegisterPrivateAutoResumeLocalStorage(); //only wasm will have this now.
        }
    }
}