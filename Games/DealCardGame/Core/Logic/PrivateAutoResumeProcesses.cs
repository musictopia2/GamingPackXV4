namespace DealCardGame.Core.Logic;
[SingletonGame]
public class PrivateAutoResumeProcesses(IPrivateSaveGame save, TestOptions test) : ISerializable
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
    public async Task RestoreStateAsync(DealCardGameGameContainer container)
    {
        if (string.IsNullOrWhiteSpace(_resumeData))
        {
            throw new CustomBasicException("No resume data.  Try calling HasAutoResumeAsync first");
        }
        PrivateModel item = await js1.DeserializeObjectAsync<PrivateModel>(_resumeData);
        container.PersonalInformation = item;
    }
    public async Task SaveStateAsync(DealCardGameGameContainer container)
    {
        if (test.SaveOption == EnumTestSaveCategory.RestoreOnly)
        {
            return; //can't save anymore
        }
        //this will go ahead and populate the save state.
        
        await save.SaveStateAsync(container.PersonalInformation);
    }
}