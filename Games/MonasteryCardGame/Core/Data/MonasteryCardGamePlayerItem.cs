namespace MonasteryCardGame.Core.Data;
[UseScoreboard]
public partial class MonasteryCardGamePlayerItem : PlayerRummyHand<MonasteryCardInfo>
{//anything needed is here
    public BasicList<int> IndexList { get; set; } = [];
    public void UpdateIndexes()
    {
        IndexList = Enumerable.Range(0, 9).ToBasicList();
        Mission1Completed = false;
        Mission2Completed = false;
        Mission3Completed = false;
        Mission4Completed = false;
        Mission5Completed = false;
        Mission6Completed = false;
        Mission7Completed = false;
        Mission8Completed = false;
        Mission9Completed = false;
        FinishedCurrentMission = false;
    }
    [ScoreColumn]
    public partial bool FinishedCurrentMission { get; set; }
    private void UpdateValue(int index, bool completed)
    {
        if (completed == false)
        {
            return;
        }
        if (IndexList.Contains(index))
        {
            IndexList.RemoveSpecificItem(index);
        }
    }
    public void CompleteMissionIndex(int index)
    {
        FinishedCurrentMission = true;
        switch (index)
        {
            case 0:
                {
                    Mission1Completed = true;
                    break;
                }

            case 1:
                {
                    Mission2Completed = true;
                    break;
                }

            case 2:
                {
                    Mission3Completed = true;
                    break;
                }

            case 3:
                {
                    Mission4Completed = true;
                    break;
                }

            case 4:
                {
                    Mission5Completed = true;
                    break;
                }

            case 5:
                {
                    Mission6Completed = true;
                    break;
                }

            case 6:
                {
                    Mission7Completed = true;
                    break;
                }

            case 7:
                {
                    Mission8Completed = true;
                    break;
                }

            case 8:
                {
                    Mission9Completed = true;
                    break;
                }

            default:
                {
                    throw new CustomBasicException("Not Supported");
                }
        }
    }
    //long format is always needed because you have to update the value so it knows what to show up on the ui.
    [ScoreColumn]
    public partial bool Mission1Completed { get; set; }


    partial void OnMission1CompletedChanged(bool value)
    {
        UpdateValue(0, value);
    }
   

    [ScoreColumn]
    public partial bool Mission2Completed { get; set; }

    partial void OnMission2CompletedChanged(bool value)
    {
        UpdateValue(1, value);
    }

    [ScoreColumn]
    public partial bool Mission3Completed { get; set; }

    partial void OnMission3CompletedChanged(bool value)
    {
        UpdateValue(2, value);
    }

    [ScoreColumn]
    public partial bool Mission4Completed { get; set; }

    partial void OnMission4CompletedChanged(bool value)
    {
        UpdateValue(3, value);
    }

    [ScoreColumn]
    public partial bool Mission5Completed { get; set; }

    partial void OnMission5CompletedChanged(bool value)
    {
        UpdateValue(4, value);
    }

    [ScoreColumn]
    public partial bool Mission6Completed { get; set; }

    partial void OnMission6CompletedChanged(bool value)
    {
        UpdateValue(5, value);
    }

    [ScoreColumn]
    public partial bool Mission7Completed { get; set; }

    partial void OnMission7CompletedChanged(bool value)
    {
        UpdateValue(6, value);
    }

    [ScoreColumn]
    public partial bool Mission8Completed { get; set; }

    partial void OnMission8CompletedChanged(bool value)
    {
        UpdateValue(7, value);
    }

    [ScoreColumn]
    public partial bool Mission9Completed { get; set; }

    partial void OnMission9CompletedChanged(bool value)
    {
        UpdateValue(8, value);
    }
}