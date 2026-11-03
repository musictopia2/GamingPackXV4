namespace MonopolyCardGame.Core.Logic;
[SingletonGame]
public class PrivateAutoResumeProcesses(IPrivateSaveGame save) : ISerializable
{
    private string _resumeData = "";
    public async Task<bool> HasAutoResumeAsync(MonopolyCardGameGameContainer container, MonopolyCardGameVMData model)
    {
        _resumeData = await save.SavedDataAsync<PrivateModel>();
        if (string.IsNullOrWhiteSpace(_resumeData))
        {
            container.TempSets.Clear(); //later will populate.
            model.Calculator1.ClearCalculator();
            return false;
        }
        return true;
    }
    public async Task RestoreStateAsync(MonopolyCardGameGameContainer container, MonopolyCardGameVMData model)
    {
        if (string.IsNullOrWhiteSpace(_resumeData))
        {
            throw new CustomBasicException("No resume data.  Try calling HasAutoResumeAsync first");
        }
        PrivateModel item = await js1.DeserializeObjectAsync<PrivateModel>(_resumeData);
        model.Calculator1.RestoreCalculator(item);
        container.TempSets = item.TempSets;
    }
    public async Task SaveStateAsync(MonopolyCardGameVMData model)
    {
        //this will go ahead and populate the save state.
        PrivateModel item = new();
        item.Calculations = model.Calculator1.GetTotalCalculations;
        //make the set number one based.
        item.TempSets = model.TempSets1.SaveTempSets();
        await save.SaveStateAsync(item);
    }
}