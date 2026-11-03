namespace BowlingDiceGame.Core.Logic;
public static class ScoreGenerator
{
    public static int GetFrameScoreBasedOnProfile(EnumLuckProfile profile)
    {
        var weights = new WeightedAverageLists<int>();
        void AddRange(int from, int to, int weight)
        {
            for (int i = from; i <= to; i++)
            {
                weights.AddWeightedItem(i, weight);
            }
        }

        switch (profile.Name)
        {
            case nameof(EnumLuckProfile.HotestStreak):
                AddRange(28, 30, 12); // max bias
                AddRange(24, 27, 4);
                AddRange(18, 23, 1);
                break;

            case nameof(EnumLuckProfile.SomewhatHotStreak):
                AddRange(25, 28, 10);
                AddRange(20, 24, 5);
                AddRange(15, 19, 2);
                AddRange(10, 14, 1);
                break;

            case nameof(EnumLuckProfile.VeryStreaky):
                AddRange(0, 5, 10);
                AddRange(6, 12, 2);
                AddRange(13, 17, 1);
                AddRange(18, 22, 1);
                AddRange(23, 27, 2);
                AddRange(28, 30, 10);
                break;



            case nameof(EnumLuckProfile.VeryBalanced):
                AddRange(6, 7, 4);     // some chance of lower rolls
                AddRange(8, 9, 6);     // more likely near-spares, encouraging close rolls
                AddRange(15, 19, 5);   // better spares more likely than low spares
                AddRange(3, 5, 1);     // occasional low rolls
                break;

            case nameof(EnumLuckProfile.SomewhatUnlucky):
                AddRange(5, 9, 5);
                AddRange(10, 12, 2);
                AddRange(0, 4, 4);
                AddRange(13, 14, 1);
                break;

            case nameof(EnumLuckProfile.VeryUnluck):
                AddRange(0, 1, 8);
                AddRange(2, 3, 6);
                AddRange(4, 5, 4);
                AddRange(6, 7, 2);
                break;
        }

        return weights.GetRandomWeightedItem();
    }
}