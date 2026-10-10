namespace BasicGameFrameworkLibrary.Core.MultiplayerClasses.BasicPlayerClasses;

[UseScoreboard]
public partial class SimplePlayer : IPlayerItem, IEquatable<SimplePlayer>
{
    public int Id { get; set; }
    [ScoreColumn]
    public partial string NickName { get; set; } = "";
    [ScoreColumn] //there can be games where it needs to display who is out of the game.
    public partial bool InGame { get; set; }
    public bool IsReady { get; set; }
    [ScoreColumn]
    public partial bool MissNextTurn { get; set; }
    public EnumPlayerCategory PlayerCategory { get; set; }
    public bool IsHost { get; set; }
    public virtual bool CanStartInGame => true; //clue will have exceptions.
    public override bool Equals(object? obj)
    {
        if (obj is not SimplePlayer Temps)
        {
            return false;
        }
        return NickName.Equals(Temps.NickName);
    }
    public bool Equals(SimplePlayer? other)
    {
        return NickName.Equals(other!.NickName);
    }
    public override int GetHashCode()
    {
        return NickName.GetHashCode();
    }
}