namespace BladesOfSteel.Core.Data;
public class ScoringGuideClassCP
{
    public string OffenseText { get; set; } = "";
    public string DefenseText { get; set; } = "";
    public string BorderText { get; set; } = "";
    public static BasicList<string> OffenseList()
    {
        return new() { "The Great One:  2 red nines", "Breakaway:  Red Ace plus card of same suit", "One-Timer:  2 red cards of same rank", "3 card flush:  3 red cards of same suit", "High Card:  Any 3 red cards" };
    }
    public static BasicList<string> DefenseList()
    {
        return new() { "(The Great One is an automatic goal)", "Star goalie:  1 black ace", "Star defense:  2 black cards of same rank", "3 card flush:  3 black cards", "High Card:  Any 3 black cards" };
    }
    public ScoringGuideClassCP()
    {
        OffenseText = "Offense Levels:";
        DefenseText = "Defense Levels:";
    }
}