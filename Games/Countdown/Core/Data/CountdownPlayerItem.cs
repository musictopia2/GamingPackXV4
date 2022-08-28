namespace Countdown.Core.Data;
[UseScoreboard]
public partial class CountdownPlayerItem : SimplePlayer
{
    public BasicList<SimpleNumber> NumberList { get; set; } = new();
}