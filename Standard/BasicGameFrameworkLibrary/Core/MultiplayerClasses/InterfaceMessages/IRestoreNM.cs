namespace BasicGameFrameworkLibrary.Core.MultiplayerClasses.InterfaceMessages;
public interface IRestoreNM
{
    Task RestoreMessageAsync(string payLoad);
}