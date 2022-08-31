namespace Racko.Core.Data;
[UseScoreboard]
public partial class RackoPlayerItem : PlayerSingleHand<RackoCardInformation>
{
    [ScoreColumn]
    public int ScoreRound { get; set; }
    [ScoreColumn]
    public int TotalScore { get; set; }
    [ScoreColumn]
    public bool CanShowValues { get; set; }
    [ScoreColumn]
    public int Value1 { get; set; }
    [ScoreColumn]
    public int Value2 { get; set; }
    [ScoreColumn]
    public int Value3 { get; set; }
    [ScoreColumn]
    public int Value4 { get; set; }
    [ScoreColumn]
    public int Value5 { get; set; }
    [ScoreColumn]
    public int Value6 { get; set; }
    [ScoreColumn]
    public int Value7 { get; set; }
    [ScoreColumn]
    public int Value8 { get; set; }
    [ScoreColumn]
    public int Value9 { get; set; }
    [ScoreColumn]
    public int Value10 { get; set; }
    public void UpdateAllValues()
    {
        if (MainHandList.Count == 0)
        {
            ResetValues();
            return;
        }
        if (MainHandList.Count != 10)
        {
            throw new CustomBasicException("Must have 10 cards left in order to update values");
        }
        int x;
        for (x = 0; x <= 9; x++)
        {
            UpdateSingleValue(x);
        }
    }
    private void UpdateSingleValue(int index)
    {
        var newValue = MainHandList[index].Value;
        switch (index)
        {
            case 0:
                {
                    Value1 = newValue;
                    break;
                }

            case 1:
                {
                    Value2 = newValue;
                    break;
                }

            case 2:
                {
                    Value3 = newValue;
                    break;
                }

            case 3:
                {
                    Value4 = newValue;
                    break;
                }

            case 4:
                {
                    Value5 = newValue;
                    break;
                }

            case 5:
                {
                    Value6 = newValue;
                    break;
                }

            case 6:
                {
                    Value7 = newValue;
                    break;
                }

            case 7:
                {
                    Value8 = newValue;
                    break;
                }

            case 8:
                {
                    Value9 = newValue;
                    break;
                }

            case 9:
                {
                    Value10 = newValue;
                    break;
                }

            default:
                {
                    throw new CustomBasicException("Not Supported");
                }
        }
    }
    public void ResetValues()
    {
        Value1 = 0;
        Value2 = 0;
        Value3 = 0;
        Value4 = 0;
        Value5 = 0;
        Value6 = 0;
        Value7 = 0;
        Value8 = 0;
        Value9 = 0;
        Value10 = 0;
    }
}