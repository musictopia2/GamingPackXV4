namespace ClueCardGame.Core.Data;
[SingletonGame]
public class ClueCardGameSaveInfo : BasicSavedCardClass<ClueCardGamePlayerItem, ClueCardGameCardInformation>, IMappable, ISaveInfo
{
    public PredictionInfo? CurrentPrediction { get; set; }
    internal void LoadMod(ClueCardGameVMData model)
    {
        if (CurrentPrediction is not null)
        {
            model.FirstName = CurrentPrediction.FirstName;
            model.SecondName = CurrentPrediction.SecondName;
        }
    }
    public SolutionInfo Solution { get; set; } = new();
    public int PreviousClue { get; set; } //this means if given, then whoevers turn it is can show that clue.
    public EnumClueStatusList GameStatus { get; set; }
    public string WhoGaveClue { get; set; } = "";
}