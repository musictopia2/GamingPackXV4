namespace BowlingDiceGame.Core.Logic;
public static class BowlingRollFactory
{
    public static RollDistribution GetRollsForTotal(int total)
    {
        if (total < 0 || total > 30)
        {
            throw new ArgumentOutOfRangeException(nameof(total), "Total must be between 0 and 30.");
        }

        var weightedCombos = new WeightedAverageLists<RollDistribution>();

        if (total <= 10)
        {
            // No third roll combos
            for (int first = 0; first <= total; first++)
            {
                for (int second = 0; second <= total - first; second++)
                {
                    int third = total - first - second;
                    if (third == 0)
                    {
                        // Weight combos where first roll is 10 higher (strike chance)
                        int weight = first == 10 ? 5 : 1;
                        weightedCombos.AddWeightedItem(RollDistribution.Create(first, second, third, false), weight);
                    }
                }
            }
        }
        else if (total <= 20)
        {
            // Strike first roll combos (first=10)
            int leftoverAfterStrike = total - 10;
            if (leftoverAfterStrike >= 0 && leftoverAfterStrike <= 20)
            {
                for (int second = 0; second <= Math.Min(10, leftoverAfterStrike); second++)
                {
                    int third = leftoverAfterStrike - second;
                    if (third >= 0 && third <= 10)
                    {
                        // Weight strikes a bit higher
                        int weight = 7;
                        weightedCombos.AddWeightedItem(RollDistribution.Create(10, second, third, true), weight);
                    }
                }
            }

            // Spare combos (first + second == 10)
            int thirdRoll = total - 10;
            if (thirdRoll >= 0 && thirdRoll <= 10)
            {
                for (int first = 0; first <= 10; first++)
                {
                    int second = 10 - first;
                    // Slightly less weight than strikes
                    int weight = 3;
                    weightedCombos.AddWeightedItem(RollDistribution.Create(first, second, thirdRoll, true), weight);
                }
            }
        }
        else
        {
            // total > 20: must be two strikes first (10,10), third leftover
            int thirdRoll = total - 20;
            if (thirdRoll >= 0 && thirdRoll <= 10)
            {
                // High weight for two strikes combo
                int weight = 10;
                weightedCombos.AddWeightedItem(RollDistribution.Create(10, 10, thirdRoll, true), weight);
            }
        }

        if (weightedCombos.GetExpectedCount == 0)
        {
            throw new InvalidOperationException("No valid roll distributions found for the total.");
        }

        return weightedCombos.GetRandomWeightedItem();
    }
}