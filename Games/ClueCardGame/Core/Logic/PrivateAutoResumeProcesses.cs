namespace ClueCardGame.Core.Logic;
[SingletonGame]
public class PrivateAutoResumeProcesses(IPrivateSaveGame save) : ISerializable
{
    private string _resumeData = "";
    public async Task<bool> HasAutoResumeAsync()
    {
        _resumeData = await save.SavedDataAsync<PrivateModel>();
        if (string.IsNullOrWhiteSpace(_resumeData))
        {
            return false;
        }
        return true;
    }
    public async Task RestoreStateAsync(ClueCardGameGameContainer container)
    {
        if (string.IsNullOrWhiteSpace(_resumeData))
        {
            throw new CustomBasicException("No resume data.  Try calling HasAutoResumeAsync first");
        }
        PrivateModel item = await js1.DeserializeObjectAsync<PrivateModel>(_resumeData);
        container.DetectiveDetails = item;
    }
    public async Task SaveStateAsync(ClueCardGameGameContainer container)
    {
        //this will go ahead and populate the save state.
        if (container.DetectiveDetails is null)
        {
            throw new CustomBasicException("No detective information was even populated");
        }
        await save.SaveStateAsync(container.DetectiveDetails);
    }
}