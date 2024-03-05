namespace MonopolyCardGame.Core.Data;
public class PrivateModel : IMappable
{
    public Dictionary<int, DetectiveInfo> PersonalNotebook { get; set; } = [];
    public PredictionInfo? CurrentPrediction { get; set; }
    public BasicList<PrivatePlayer> ComputerData { get; set; } = []; //so the computer can make smarter decisions.
    public bool StartAccusation { get; set; } //this is if you are starting to make accusation.  other players don't need to know about this.
    public SolutionInfo Accusation { get; set; } = new();
    //if you are not in game, that means you failed.

    //public bool HumanFailed { get; set; } //if you fail, then cannot make accusation anymore obviously because you failed.
}