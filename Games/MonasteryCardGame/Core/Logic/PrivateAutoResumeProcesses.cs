﻿namespace MonasteryCardGame.Core.Logic;
[SingletonGame]
public class PrivateAutoResumeProcesses(IPrivateSaveGame save) : ISerializable
{
    private string _resumeData = "";
    public async Task<bool> HasAutoResumeAsync(MonasteryCardGameGameContainer container)
    {
        _resumeData = await save.SavedDataAsync<BasicList<OrganizeModel>>();
        if (string.IsNullOrWhiteSpace(_resumeData))
        {
            container.TempSets.Clear(); //later will populate.
            return false;
        }
        return true;
    }
    public async Task RestoreStateAsync(MonasteryCardGameGameContainer container)
    {
        if (string.IsNullOrWhiteSpace(_resumeData))
        {
            throw new CustomBasicException("No resume data.  Try calling HasAutoResumeAsync first");
        }
        BasicList<OrganizeModel> item = await js1.DeserializeObjectAsync<BasicList<OrganizeModel>>(_resumeData);
        container.TempSets = item;
    }
    private static BasicList<OrganizeModel> GetSaveTempSets(MonasteryCardGameVMData model)
    {
        BasicList<OrganizeModel> output = [];
        int x = 0;
        var list = model.TempSets.SetList;
        foreach (var item in list)
        {
            x++;
            OrganizeModel organize = new();
            organize.SetNumber = x;
            organize.Cards = item.HandList.GetDeckListFromObjectList();
            output.Add(organize);
        }
        return output;
    }
    public async Task SaveStateAsync(MonasteryCardGameVMData model)
    {
        var item = GetSaveTempSets(model);
        await save.SaveStateAsync(item);
    }
}