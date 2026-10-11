namespace BowlingDiceGame.Core.Data;

[UseStateChanged]
public partial class FrameInfoCP
{
    public Dictionary<int, SectionInfoCP> SectionList { get; set; } = [];
    [StateProperty]
    public partial int Score { get; set; } = -1;
    public void Update()
    {
        StateChanged?.Invoke();
    }
}