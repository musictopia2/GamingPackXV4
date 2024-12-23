namespace RummyDice.Core.Logic;
internal static class RandomExtensions
{
    private static float Multiplier { get; set; } = 3f; // You can change this value for trial and error (1 = no change, 2 = double, 0.1 = wilds reduced significantly)
    public static GameTieredDistribution<int> Initialize(this GameTieredDistribution<int> tiers, int phase, int repeats)
    {
        if (repeats > 3)
        {
            if (phase == 6)
            {
                Multiplier = 1.0f;
            }
            else
            {
                Multiplier = 1.1f; //has to have the best chances for wilds all around.
            }
            return tiers;
        }
        if (phase == 4 || phase == 5)
        {
            Multiplier = 1.5f;
            if (repeats > 1)
            {
                Multiplier = 1.3f;
            }
        }
        else if (phase == 6)
        {
            Multiplier = 1.2f;
            if (repeats > 1)
            {
                Multiplier = 1.1f;
            }
        }
        else if (phase == 8)
        {
            Multiplier = 2.0f;
        }
        else if (phase == 9)
        {
            Multiplier = 2.5f;
            if (repeats > 1)
            {
                Multiplier = 2f;
            }
        }
        else if (phase == 10)
        {
            Multiplier = 1.8f;
            if (repeats > 1)
            {
                Multiplier = 1.5f;
            }
        }
        else if (phase == 7)
        {
            Multiplier = 2f;
            if (repeats > 1)
            {
                Multiplier = 1.6f;
            }
        }
        return tiers;
    }

    public static GameTieredDistribution<int> PopulateVeryLuckTier(this GameTieredDistribution<int> tiers, int phase, int repeats)
    {
        if (phase == 8)
        {
            tiers.PopulateColorsVeryLucky(repeats);
            return tiers;
        }
        if (phase == 4 || phase == 5 || phase == 6)
        {
            tiers.PopulateRunsVeryLucky(repeats);
            return tiers;
        }
        if (phase == 7)
        {
            tiers.PopulateTwoSetsOfFourVeryLucky(repeats);
            return tiers;
        }
        if (phase == 9 || phase == 10)
        {
            tiers.PopulateLastPhasesVeryLucky(repeats);
            return tiers;
        }
        tiers.PopulateRegularVeryLucky(repeats);
        return tiers;
    }

    public static GameTieredDistribution<int> PopulateSomewhatLuckyTier(this GameTieredDistribution<int> tiers, int phase, int repeats)
    {
        if (phase == 8)
        {
            tiers.PopulateColorsSomewhatLucky(repeats);
            return tiers;
        }
        if (phase == 4 || phase == 5 || phase == 6)
        {
            tiers.PopulateRunsSomewhatLucky(repeats);
            return tiers;
        }
        if (phase == 7)
        {
            tiers.PopulateTwoSetsOfFourSomewhatLucky(repeats);
            return tiers;
        }
        if (phase == 10)
        {
            tiers.PopulateLastPhasesSomewhatLucky(repeats);
            return tiers;
        }
        tiers.PopulateRegularSomewhatLucky(repeats);
        return tiers;
    }

    public static GameTieredDistribution<int> PopulateAverageTier(this GameTieredDistribution<int> tiers, int phase, int repeats)
    {
        if (phase == 8)
        {
            tiers.PopulateColorsAverage(repeats);
            return tiers;
        }
        if (phase == 4 || phase == 5 || phase == 6)
        {
            tiers.PopulateRunsAverage(repeats);
            return tiers;
        }
        if (phase == 7)
        {
            tiers.PopulateTwoSetsOfFourAverage(repeats);
            return tiers;
        }
        if (phase == 10)
        {
            tiers.PopulateLastPhasesAverage(repeats);
            return tiers;
        }
        tiers.PopulateRegularAverage(repeats);
        return tiers;
    }

    public static GameTieredDistribution<int> PopulateBadLuckTier(this GameTieredDistribution<int> tiers, int phase, int repeats)
    {
        if (tiers.HasTier(LuckStaticClass.Bad) == false)
        {
            return tiers;
        }
        if (phase == 8)
        {
            tiers.PopulateColorsBadLuck(repeats);
            return tiers;
        }
        if (phase == 4 || phase == 5 || phase == 6)
        {
            tiers.PopulateRunsBadLuck(repeats);
            return tiers;
        }
        if (phase == 7)
        {
            tiers.PopulateTwoSetsOfFourBadLuck(repeats);
            return tiers;
        }
        if (phase == 10)
        {
            tiers.PopulateLastPhasesBadLuck(repeats);
            return tiers;
        }
        tiers.PopulateRegularBadLuck(repeats);
        return tiers;
    }
    private static BasicList<int> GetWilds => Enumerable.Range(49, 4).ToBasicList();
    private static BasicList<int> GetNumberDistributionsForRange(int low, int high)
    {
        var list = Enumerable.Range(low, high - low + 1).ToBasicList();
        BasicList<int> output = [];
        foreach (var item in list)
        {
            output.AddRange(GetDistributionForNumber(item));
        }
        if (output.Any(x => x > 48))
        {
            throw new CustomBasicException("Nothing found for after 48");
        }
        return output;
    }
    private static BasicList<int> GetDistributionForNumber(int number)
    {
        if (number <= 0 || number > 12)
        {
            throw new CustomBasicException($"Must be between 1 and 12, not {number}");
        }
        BasicList<int> result = [];
        // Calculate the starting number for the group
        int startNumber = (number - 1) * 4 + 1;

        // Add the four numbers (startNumber to startNumber + 3)
        for (int i = 0; i < 4; i++)
        {
            result.Add(startNumber + i);
        }

        return result;
    }
    private static void PopulateColorsVeryLucky(this GameTieredDistribution<int> tiers, int repeats)
    {
        tiers.SelectTier(LuckStaticClass.Lucky, tiers =>
        {
            var list = GetWilds;
            int wildWeight;

            // Adjust wild weight based on the number of repeats (more repeats = better luck)
            if (repeats > 2)
            {
                wildWeight = 100;  // Maximum wild chance after 2 repeats
            }
            else if (repeats == 2)
            {
                wildWeight = 80;  // Slightly lower than max
            }
            else if (repeats == 1)
            {
                wildWeight = 60;  // Moderate chance
            }
            else
            {
                wildWeight = 40;  // Still some chance, but low
            }

            // Add wilds to the distribution (wildWeight is unaffected by the multiplier)
            tiers.AddWeightedItem(list, wildWeight);

            // Now for the numbers:
            if (repeats > 2)
            {
                // Best chances for high numbers (10-12), very high chance
                list = GetNumberDistributionsForRange(10, 12);
                tiers.AddWeightedItem(list, (int)(40 * Multiplier), (int)(80 * Multiplier));  // Apply multiplier to number ranges

                // Still a decent chance for lower numbers (1-9), but less than high numbers
                list = GetNumberDistributionsForRange(1, 9);
                tiers.AddWeightedItem(list, (int)(10 * Multiplier), (int)(20 * Multiplier));  // Apply multiplier to number ranges
            }
            else if (repeats == 2)
            {
                // Good chance for 10-12
                list = GetNumberDistributionsForRange(10, 12);
                tiers.AddWeightedItem(list, (int)(30 * Multiplier), (int)(60 * Multiplier));  // Apply multiplier to number ranges

                // Moderate chance for 1-9
                list = GetNumberDistributionsForRange(1, 9);
                tiers.AddWeightedItem(list, (int)(10 * Multiplier), (int)(20 * Multiplier));  // Apply multiplier to number ranges
            }
            else if (repeats == 1)
            {
                // Balanced chance for 10-12 and 1-9, but still higher focus on 10-12
                list = GetNumberDistributionsForRange(10, 12);
                tiers.AddWeightedItem(list, (int)(20 * Multiplier), (int)(40 * Multiplier));  // Apply multiplier to number ranges

                list = GetNumberDistributionsForRange(1, 9);
                tiers.AddWeightedItem(list, (int)(10 * Multiplier), (int)(20 * Multiplier));  // Apply multiplier to number ranges
            }
            else
            {
                list = GetNumberDistributionsForRange(10, 12);
                tiers.AddWeightedItem(list, (int)(15 * Multiplier), (int)(30 * Multiplier));  // Apply multiplier to number ranges
                list = GetNumberDistributionsForRange(1, 9);
                tiers.AddWeightedItem(list, (int)(10 * Multiplier), (int)(20 * Multiplier));  // Apply multiplier to number ranges

                
            }
        });
    }

    private static void PopulateColorsSomewhatLucky(this GameTieredDistribution<int> tiers, int repeats)
    {
        tiers.SelectTier(LuckStaticClass.SomewhatLucky, tiers =>
        {
            var list = GetWilds;
            int wildWeight;

            // Adjust wild weight based on the number of repeats (more repeats = better luck)
            if (repeats > 2)
            {
                wildWeight = 80;  // Moderate chance for wilds after 2 repeats
            }
            else if (repeats == 2)
            {
                wildWeight = 60;  // Slightly lower wild chance
            }
            else if (repeats == 1)
            {
                wildWeight = 50;  // Still moderate chance for wilds
            }
            else
            {
                wildWeight = 40;  // Lowest chance for wilds
            }

            // Add wilds to the distribution (wildWeight is unaffected by the multiplier)
            tiers.AddWeightedItem(list, wildWeight);

            // For the number distributions:
            // Now for the numbers:
            if (repeats > 2)
            {
                // Best chances for high numbers (10-12), very high chance
                list = GetNumberDistributionsForRange(10, 12);
                tiers.AddWeightedItem(list, (int)(10 * Multiplier), (int)(20 * Multiplier));
                list = GetNumberDistributionsForRange(7, 9);
                tiers.AddWeightedItem(list, (int)(40 * Multiplier), (int)(80 * Multiplier));  // Apply multiplier to number ranges

                // Still a decent chance for lower numbers (1-9), but less than high numbers
                list = GetNumberDistributionsForRange(1, 6);
                tiers.AddWeightedItem(list, (int)(10 * Multiplier), (int)(20 * Multiplier));  // Apply multiplier to number ranges
            }
            else if (repeats == 2)
            {
                // Good chance for 10-12
                list = GetNumberDistributionsForRange(10, 12);
                tiers.AddWeightedItem(list, (int)(10 * Multiplier), (int)(20 * Multiplier));


                tiers.AddWeightedItem(list, (int)(30 * Multiplier), (int)(60 * Multiplier));  // Apply multiplier to number ranges
                list = GetNumberDistributionsForRange(7, 9);
                // Moderate chance for 1-9
                list = GetNumberDistributionsForRange(1, 6);
                tiers.AddWeightedItem(list, (int)(10 * Multiplier), (int)(20 * Multiplier));  // Apply multiplier to number ranges
            }
            else if (repeats == 1)
            {
                // Balanced chance for 10-12 and 1-9, but still higher focus on 10-12
                list = GetNumberDistributionsForRange(10, 12);
                tiers.AddWeightedItem(list, (int)(10 * Multiplier), (int)(20 * Multiplier));

                list = GetNumberDistributionsForRange(7, 9);
                tiers.AddWeightedItem(list, (int)(20 * Multiplier), (int)(40 * Multiplier));  // Apply multiplier to number ranges

                list = GetNumberDistributionsForRange(1, 6);
                tiers.AddWeightedItem(list, (int)(10 * Multiplier), (int)(20 * Multiplier));  // Apply multiplier to number ranges
            }
            else
            {
                list = GetNumberDistributionsForRange(10, 12);
                tiers.AddWeightedItem(list, (int)(10 * Multiplier), (int)(20 * Multiplier));
                list = GetNumberDistributionsForRange(7, 9);
                tiers.AddWeightedItem(list, (int)(15 * Multiplier), (int)(30 * Multiplier));  // Apply multiplier to number ranges
                list = GetNumberDistributionsForRange(1, 6);
                tiers.AddWeightedItem(list, (int)(10 * Multiplier), (int)(20 * Multiplier));  // Apply multiplier to number ranges


            }
        });
    }

    private static void PopulateColorsAverage(this GameTieredDistribution<int> tiers, int repeats)
    {
        tiers.SelectTier(LuckStaticClass.Average, tiers =>
        {
            var list = GetWilds;
            int wildWeight;

            // Adjust wild weight based on the number of repeats (more repeats = better luck)
            if (repeats > 2)
            {
                wildWeight = 70;  // Cap the wild chance at 70 for average luck
            }
            else if (repeats == 2)
            {
                wildWeight = 50;  // Lower wild chance with fewer repeats
            }
            else if (repeats == 1)
            {
                wildWeight = 40;  // Lower wild chance with fewer repeats
            }
            else
            {
                wildWeight = 30;  // Very low wild chance for no repeats
            }

            // Add wilds to the distribution (wildWeight is unaffected by the multiplier)
            tiers.AddWeightedItem(list, wildWeight);

            // For the number distributions:
            if (repeats > 2)
            {
                // Balanced distribution across all ranges (lower, middle, higher)
                list = GetNumberDistributionsForRange(1, 3);
                tiers.AddWeightedItem(list, (int)(10 * Multiplier), (int)(20 * Multiplier));  // Apply multiplier to number ranges

                list = GetNumberDistributionsForRange(4, 6);
                tiers.AddWeightedItem(list, (int)(60 * Multiplier), (int)(90 * Multiplier));  // Apply multiplier to number ranges

                list = GetNumberDistributionsForRange(7, 12);
                tiers.AddWeightedItem(list, (int)(30 * Multiplier), (int)(40 * Multiplier));  // Apply multiplier to number ranges
            }
            else if (repeats == 2)
            {
                // Focus on 4-8 as a balanced middle ground
                list = GetNumberDistributionsForRange(4, 6);
                tiers.AddWeightedItem(list, (int)(60 * Multiplier), (int)(90 * Multiplier));  // Apply multiplier to number ranges

                // Smaller chances for extremes (1-3 and 9-12)
                list = GetNumberDistributionsForRange(1, 3);
                tiers.AddWeightedItem(list, (int)(10 * Multiplier), (int)(20 * Multiplier));  // Apply multiplier to number ranges

                list = GetNumberDistributionsForRange(7, 12);
                tiers.AddWeightedItem(list, (int)(25 * Multiplier), (int)(35 * Multiplier));  // Apply multiplier to number ranges
            }
            else if (repeats == 1)
            {
                // More balanced, but slight preference for middle range (5-8)
                list = GetNumberDistributionsForRange(4, 6);
                tiers.AddWeightedItem(list, (int)(50 * Multiplier), (int)(80 * Multiplier));  // Apply multiplier to number ranges

                list = GetNumberDistributionsForRange(1, 3);
                tiers.AddWeightedItem(list, (int)(10 * Multiplier), (int)(20 * Multiplier));  // Apply multiplier to number ranges

                list = GetNumberDistributionsForRange(7, 12);
                tiers.AddWeightedItem(list, (int)(20 * Multiplier), (int)(30 * Multiplier));  // Apply multiplier to number ranges
            }
            else
            {
                list = GetNumberDistributionsForRange(4, 6);
                tiers.AddWeightedItem(list, (int)(50 * Multiplier), (int)(80 * Multiplier));  // Apply multiplier to number ranges

                list = GetNumberDistributionsForRange(1, 3);
                tiers.AddWeightedItem(list, (int)(30 * Multiplier), (int)(40 * Multiplier));  // Apply multiplier to number ranges

                list = GetNumberDistributionsForRange(7, 12);
                tiers.AddWeightedItem(list, (int)(10 * Multiplier), (int)(20 * Multiplier));  // Apply multiplier to number ranges
            }
        });
    }

    private static void PopulateColorsBadLuck(this GameTieredDistribution<int> tiers, int repeats)
    {
        tiers.SelectTier(LuckStaticClass.Bad, tiers =>
        {
            var list = GetWilds;
            int wildWeight;

            // Adjust wild weight based on the number of repeats (more repeats = better luck)
            if (repeats > 2)
            {
                wildWeight = 50;  // Still very low wild chance after 2 repeats
            }
            else if (repeats == 2)
            {
                wildWeight = 30;  // Low wild chance
            }
            else if (repeats == 1)
            {
                wildWeight = 20;  // Very low wild chance
            }
            else
            {
                wildWeight = 10;  // Barely any chance for wilds
            }

            // Add wilds to the distribution (wildWeight is unaffected by the multiplier)
            tiers.AddWeightedItem(list, wildWeight);

            // For the number distributions:
            if (repeats > 2)
            {
                // Focus on the lowest numbers (1-4) while still accounting for all numbers
                list = GetNumberDistributionsForRange(1, 3);
                tiers.AddWeightedItem(list, (int)(40 * Multiplier), (int)(60 * Multiplier));  // Apply multiplier to number ranges

                list = GetNumberDistributionsForRange(4, 8);
                tiers.AddWeightedItem(list, (int)(25 * Multiplier), (int)(40 * Multiplier));  // Apply multiplier to number ranges

                list = GetNumberDistributionsForRange(9, 12);
                tiers.AddWeightedItem(list, (int)(15 * Multiplier), (int)(20 * Multiplier));  // Apply multiplier to number ranges
            }
            else if (repeats == 2)
            {
                // Strong bias toward low numbers (1-4), moderate chance for mid-range (5-8), and very low chance for high numbers
                list = GetNumberDistributionsForRange(1, 3);
                tiers.AddWeightedItem(list, (int)(45 * Multiplier), (int)(50 * Multiplier));  // Apply multiplier to number ranges

                list = GetNumberDistributionsForRange(4, 6);
                tiers.AddWeightedItem(list, (int)(20 * Multiplier), (int)(30 * Multiplier));  // Apply multiplier to number ranges

                list = GetNumberDistributionsForRange(7, 12);
                tiers.AddWeightedItem(list, (int)(10 * Multiplier), (int)(20 * Multiplier));  // Apply multiplier to number ranges
            }
            else if (repeats == 1)
            {
                // Focus on low numbers (1-3), and some small chance for higher numbers (9-12)
                list = GetNumberDistributionsForRange(1, 3);
                tiers.AddWeightedItem(list, (int)(50 * Multiplier), (int)(70 * Multiplier));  // Apply multiplier to number ranges

                list = GetNumberDistributionsForRange(4, 6);
                tiers.AddWeightedItem(list, (int)(20 * Multiplier), (int)(30 * Multiplier));  // Apply multiplier to number ranges

                list = GetNumberDistributionsForRange(7, 9);
                tiers.AddWeightedItem(list, (int)(10 * Multiplier), (int)(15 * Multiplier));  // Apply multiplier to number ranges

                list = GetNumberDistributionsForRange(10, 12);
                tiers.AddWeightedItem(list, (int)(5 * Multiplier), (int)(10 * Multiplier));  // Apply multiplier to number ranges
            }
            else
            {
                // Worst luck: Focus completely on very low numbers (1-2), very minimal chance for higher numbers
                list = GetNumberDistributionsForRange(1, 3);
                tiers.AddWeightedItem(list, (int)(80 * Multiplier), (int)(100 * Multiplier));  // Apply multiplier to number ranges

                list = GetNumberDistributionsForRange(4, 7);
                tiers.AddWeightedItem(list, (int)(10 * Multiplier), (int)(20 * Multiplier));  // Apply multiplier to number ranges

                list = GetNumberDistributionsForRange(8, 12);
                tiers.AddWeightedItem(list, (int)(5 * Multiplier), (int)(10 * Multiplier));  // Apply multiplier to number ranges
            }
        });
    }
    private static void PopulateRunsVeryLucky(this GameTieredDistribution<int> tiers, int repeats)
    {
        tiers.SelectTier(LuckStaticClass.Lucky, tiers =>
        {
            var list = GetWilds;
            int wildWeight;

            // Wild weight unaffected by Multiplier
            if (repeats > 2)
            {
                wildWeight = 350;
            }
            else if (repeats == 2)
            {
                wildWeight = 300;
            }
            else if (repeats == 1)
            {
                wildWeight = 250;
            }
            else
            {
                wildWeight = 200;
            }

            // Add wilds with high weight for very lucky scenarios
            tiers.AddWeightedItem(list, wildWeight);

            // Apply multiplier to number distributions
            if (repeats > 2)
            {
                // Very high chance for runs starting with 6-12 (7 numbers)
                list = GetNumberDistributionsForRange(6, 12);
                tiers.AddWeightedItem(list, (int)(100 * Multiplier), (int)(140 * Multiplier));  // Apply multiplier to ranges 6-12

                // Low chance for runs starting with 1-5 (5 numbers)
                list = GetNumberDistributionsForRange(1, 5);
                tiers.AddWeightedItem(list, (int)(10 * Multiplier), (int)(30 * Multiplier));  // Apply multiplier to ranges 1-5
            }
            else if (repeats == 2)
            {
                // High chance for runs starting with 6-12 (7 numbers)
                list = GetNumberDistributionsForRange(6, 12);
                tiers.AddWeightedItem(list, (int)(80 * Multiplier), (int)(120 * Multiplier));  // Apply multiplier to ranges 6-12

                // Moderate chance for runs starting with 1-5 (5 numbers)
                list = GetNumberDistributionsForRange(1, 5);
                tiers.AddWeightedItem(list, (int)(20 * Multiplier), (int)(40 * Multiplier));  // Apply multiplier to ranges 1-5
            }
            else if (repeats == 1)
            {
                // Moderate chance for runs starting with 6-12 (7 numbers)
                list = GetNumberDistributionsForRange(6, 12);
                tiers.AddWeightedItem(list, (int)(60 * Multiplier), (int)(80 * Multiplier));  // Apply multiplier to ranges 6-12

                // Higher chance for runs starting with 1-5 (5 numbers)
                list = GetNumberDistributionsForRange(1, 5);
                tiers.AddWeightedItem(list, (int)(20 * Multiplier), (int)(30 * Multiplier));  // Apply multiplier to ranges 1-5
            }
            else
            {
                // Lower chance for runs starting with 6-12 (7 numbers)
                list = GetNumberDistributionsForRange(6, 12);
                tiers.AddWeightedItem(list, (int)(50 * Multiplier), (int)(70 * Multiplier));  // Apply multiplier to ranges 6-12

                // Highest chance for runs starting with 1-5 (5 numbers)
                list = GetNumberDistributionsForRange(1, 5);
                tiers.AddWeightedItem(list, (int)(20 * Multiplier), (int)(20 * Multiplier));  // Apply multiplier to ranges 1-5
            }
        });
    }
    private static void PopulateRunsSomewhatLucky(this GameTieredDistribution<int> tiers, int repeats)
    {
        tiers.SelectTier(LuckStaticClass.SomewhatLucky, tiers =>
        {
            var list = GetWilds;
            int wildWeight;

            // Wild weight unaffected by Multiplier
            if (repeats > 2)
            {
                wildWeight = 250;
            }
            else if (repeats == 2)
            {
                wildWeight = 200;
            }
            else if (repeats == 1)
            {
                wildWeight = 150;
            }
            else
            {
                wildWeight = 120;
            }

            // Add wilds with a reasonable weight for somewhat lucky scenarios
            tiers.AddWeightedItem(list, wildWeight);

            // Apply multiplier to number distributions
            if (repeats > 2)
            {
                // High chance for runs starting with 5-11 (7 numbers)
                list = GetNumberDistributionsForRange(5, 11);
                tiers.AddWeightedItem(list, (int)(80 * Multiplier), (int)(100 * Multiplier));  // Apply multiplier to ranges 5-11

                // Lower chance for runs starting with 12, 1-4 (4 numbers)
                list = GetNumberDistributionsForRange(12, 12);
                tiers.AddWeightedItem(list, (int)(5 * Multiplier), (int)(15 * Multiplier));  // Apply multiplier to range 12

                list = GetNumberDistributionsForRange(1, 4);
                tiers.AddWeightedItem(list, (int)(10 * Multiplier), (int)(20 * Multiplier));  // Apply multiplier to ranges 1-4
            }
            else if (repeats == 2)
            {
                // Moderate chance for runs starting with 5-11 (7 numbers)
                list = GetNumberDistributionsForRange(5, 11);
                tiers.AddWeightedItem(list, (int)(70 * Multiplier), (int)(80 * Multiplier));  // Apply multiplier to ranges 5-11

                // Lower chance for runs starting with 12, 1-4 (4 numbers)
                list = GetNumberDistributionsForRange(12, 12);
                tiers.AddWeightedItem(list, (int)(20 * Multiplier), (int)(30 * Multiplier));  // Apply multiplier to range 12

                list = GetNumberDistributionsForRange(1, 4);
                tiers.AddWeightedItem(list, (int)(20 * Multiplier), (int)(30 * Multiplier));  // Apply multiplier to ranges 1-4
            }
            else if (repeats == 1)
            {
                // Balanced chance for runs starting with 5-11 (7 numbers)
                list = GetNumberDistributionsForRange(5, 11);
                tiers.AddWeightedItem(list, (int)(60 * Multiplier), (int)(80 * Multiplier));  // Apply multiplier to ranges 5-11

                // Higher chance for runs starting with 12, 1-4 (4 numbers)
                list = GetNumberDistributionsForRange(12, 12);
                tiers.AddWeightedItem(list, (int)(20 * Multiplier), (int)(30 * Multiplier));  // Apply multiplier to range 12

                list = GetNumberDistributionsForRange(1, 4);
                tiers.AddWeightedItem(list, (int)(20 * Multiplier), (int)(30 * Multiplier));  // Apply multiplier to ranges 1-4
            }
            else
            {
                // Lower chance for runs starting with 5-11 (7 numbers)
                list = GetNumberDistributionsForRange(5, 11);
                tiers.AddWeightedItem(list, (int)(50 * Multiplier), (int)(70 * Multiplier));  // Apply multiplier to ranges 5-11

                // Highest chance for runs starting with 12, 1-4 (4 numbers)
                list = GetNumberDistributionsForRange(12, 12);
                tiers.AddWeightedItem(list, (int)(20 * Multiplier), (int)(20 * Multiplier));  // Apply multiplier to range 12

                list = GetNumberDistributionsForRange(1, 4);
                tiers.AddWeightedItem(list, (int)(20 * Multiplier), (int)(20 * Multiplier));  // Apply multiplier to ranges 1-4
            }
        });
    }
    private static void PopulateRunsAverage(this GameTieredDistribution<int> tiers, int repeats)
    {
        tiers.SelectTier(LuckStaticClass.Average, tiers =>
        {
            var list = GetWilds;
            int wildWeight;

            // Wild weight unaffected by Multiplier
            if (repeats > 2)
            {
                wildWeight = 150;
            }
            else if (repeats == 2)
            {
                wildWeight = 120;
            }
            else if (repeats == 1)
            {
                wildWeight = 100;
            }
            else
            {
                wildWeight = 80;
            }

            // Add wilds with a reasonable weight for average luck scenarios
            tiers.AddWeightedItem(list, wildWeight);

            // Apply multiplier to number distributions
            if (repeats > 2)
            {
                // Moderate chance for runs starting with 3-9 (7 numbers)
                list = GetNumberDistributionsForRange(1, 2);
                tiers.AddWeightedItem(list, (int)(10 * Multiplier), (int)(20 * Multiplier));

                // Lowest chance for runs starting with 3-9 (7 numbers)
                list = GetNumberDistributionsForRange(3, 9);
                tiers.AddWeightedItem(list, (int)(100 * Multiplier), (int)(150 * Multiplier));  // Apply multiplier to ranges 3-9

                // Highest chance for runs starting with 10-12 (3 numbers)
                list = GetNumberDistributionsForRange(10, 12);
                tiers.AddWeightedItem(list, (int)(30 * Multiplier), (int)(40 * Multiplier));
            }
            else if (repeats == 2)
            {
                list = GetNumberDistributionsForRange(1, 2);
                tiers.AddWeightedItem(list, (int)(10 * Multiplier), (int)(20 * Multiplier));

                // Lowest chance for runs starting with 3-9 (7 numbers)
                list = GetNumberDistributionsForRange(3, 9);
                tiers.AddWeightedItem(list, (int)(80 * Multiplier), (int)(100 * Multiplier));  // Apply multiplier to ranges 3-9

                // Highest chance for runs starting with 10-12 (3 numbers)
                list = GetNumberDistributionsForRange(10, 12);
                tiers.AddWeightedItem(list, (int)(20 * Multiplier), (int)(40 * Multiplier));
            }
            else if (repeats == 1)
            {
                list = GetNumberDistributionsForRange(1, 2);
                tiers.AddWeightedItem(list, (int)(20 * Multiplier), (int)(30 * Multiplier));

                // Lowest chance for runs starting with 3-9 (7 numbers)
                list = GetNumberDistributionsForRange(3, 9);
                tiers.AddWeightedItem(list, (int)(70 * Multiplier), (int)(90 * Multiplier));  // Apply multiplier to ranges 3-9

                // Highest chance for runs starting with 10-12 (3 numbers)
                list = GetNumberDistributionsForRange(10, 12);
                tiers.AddWeightedItem(list, (int)(10 * Multiplier), (int)(20 * Multiplier));
            }
            else
            {

                list = GetNumberDistributionsForRange(1, 2);
                tiers.AddWeightedItem(list, (int)(30 * Multiplier), (int)(40 * Multiplier));

                // Lowest chance for runs starting with 3-9 (7 numbers)
                list = GetNumberDistributionsForRange(3, 9);
                tiers.AddWeightedItem(list, (int)(50 * Multiplier), (int)(70 * Multiplier));  // Apply multiplier to ranges 3-9

                // Highest chance for runs starting with 10-12 (3 numbers)
                list = GetNumberDistributionsForRange(10, 12);
                tiers.AddWeightedItem(list, (int)(10 * Multiplier), (int)(20 * Multiplier));  // Apply multiplier to ranges 10-12
            }
        });
    }
    private static void PopulateRunsBadLuck(this GameTieredDistribution<int> tiers, int repeats)
    {
        tiers.SelectTier(LuckStaticClass.Bad, tiers =>
        {
            var list = GetWilds;
            int wildWeight;

            // Wild weight unaffected by Multiplier
            if (repeats > 2)
            {
                wildWeight = 100;
            }
            else if (repeats == 2)
            {
                wildWeight = 80;
            }
            else if (repeats == 1)
            {
                wildWeight = 60;
            }
            else
            {
                wildWeight = 40;
            }

            // Add wilds with a lower weight for bad luck scenarios
            tiers.AddWeightedItem(list, wildWeight);

            // Apply multiplier to number distributions
            if (repeats > 2)
            {
                // Low chance for runs starting with 3-8 (6 numbers)
                list = GetNumberDistributionsForRange(1, 7);
                tiers.AddWeightedItem(list, (int)(80 * Multiplier), (int)(100 * Multiplier));

                // Lowest chance for runs starting with 3-9 (7 numbers)
                list = GetNumberDistributionsForRange(8, 12);
                tiers.AddWeightedItem(list, (int)(20 * Multiplier), (int)(30 * Multiplier));  // Apply multiplier to ranges 3-9
            }
            else if (repeats == 2)
            {
                list = GetNumberDistributionsForRange(1, 7);
                tiers.AddWeightedItem(list, (int)(70 * Multiplier), (int)(90 * Multiplier));

                // Lowest chance for runs starting with 3-9 (7 numbers)
                list = GetNumberDistributionsForRange(8, 12);
                tiers.AddWeightedItem(list, (int)(20 * Multiplier), (int)(30 * Multiplier));  // Apply multiplier to ranges 3-9
            }
            else if (repeats == 1)
            {
                list = GetNumberDistributionsForRange(1, 7);
                tiers.AddWeightedItem(list, (int)(60 * Multiplier), (int)(80 * Multiplier));

                // Lowest chance for runs starting with 3-9 (7 numbers)
                list = GetNumberDistributionsForRange(8, 12);
                tiers.AddWeightedItem(list, (int)(20 * Multiplier), (int)(30 * Multiplier));  // Apply multiplier to ranges 3-9

               
            }
            else
            {
                list = GetNumberDistributionsForRange(1, 7);
                tiers.AddWeightedItem(list, (int)(50 * Multiplier), (int)(70 * Multiplier));

                // Lowest chance for runs starting with 3-9 (7 numbers)
                list = GetNumberDistributionsForRange(8, 9);
                tiers.AddWeightedItem(list, (int)(20 * Multiplier), (int)(30 * Multiplier));  // Apply multiplier to ranges 3-9

                // Highest chance for runs starting with 10-12 (3 numbers)
                list = GetNumberDistributionsForRange(10, 12);
                tiers.AddWeightedItem(list, (int)(10 * Multiplier), (int)(20 * Multiplier));  // Apply multiplier to ranges 10-12
            }
        });
    }
    private static void PopulateTwoSetsOfFourVeryLucky(this GameTieredDistribution<int> tiers, int repeats)
    {
        tiers.SelectTier(LuckStaticClass.Lucky, tiers =>
        {
            // Wild weight based on repeats for Very Lucky
            int wildWeight;

            if (repeats > 2)
            {
                wildWeight = 300;  // Very high chance for wilds after 2 repeats
            }
            else if (repeats == 2)
            {
                wildWeight = 200;  // Still very high, but slightly less than after 3 repeats
            }
            else if (repeats == 1)
            {
                wildWeight = 150;  // Still high, but lower than for 2 or more repeats
            }
            else
            {
                wildWeight = 100;  // Moderate chance for wilds when not repeated yet
            }

            // Add wilds with the calculated weight (no multiplier applied)
            var list = GetWilds;
            tiers.AddWeightedItem(list, wildWeight);

            // Apply multiplier to the number distributions for Very Lucky
            if (repeats > 2)
            {
                list = GetNumberDistributionsForRange(11, 12); // Range: 11-12
                tiers.AddWeightedItem(list, (int)(80 * Multiplier), (int)(100 * Multiplier));  // High weight with multiplier

                list = GetNumberDistributionsForRange(8, 10); // Range: 8 to 10
                tiers.AddWeightedItem(list, (int)(30 * Multiplier), (int)(50 * Multiplier));  // Moderate weight with multiplier

                list = GetNumberDistributionsForRange(1, 7); // Range: 1-7
                tiers.AddWeightedItem(list, (int)(5 * Multiplier), (int)(10 * Multiplier));  // Very low weight with multiplier
            }
            else if (repeats == 2)
            {
                list = GetNumberDistributionsForRange(11, 12); // Range: 11-12
                tiers.AddWeightedItem(list, (int)(60 * Multiplier), (int)(80 * Multiplier));  // High weight with multiplier

                list = GetNumberDistributionsForRange(8, 10); // Range: 8 to 10
                tiers.AddWeightedItem(list, (int)(40 * Multiplier), (int)(60 * Multiplier));  // Moderate weight with multiplier

                list = GetNumberDistributionsForRange(1, 7); // Range: 1-7
                tiers.AddWeightedItem(list, (int)(10 * Multiplier), (int)(20 * Multiplier));  // Low weight with multiplier
            }
            else if (repeats == 1)
            {
                list = GetNumberDistributionsForRange(11, 12); // Range: 11-12
                tiers.AddWeightedItem(list, (int)(50 * Multiplier), (int)(70 * Multiplier));  // High weight with multiplier

                list = GetNumberDistributionsForRange(8, 9); // Range: 8 to 10
                tiers.AddWeightedItem(list, (int)(30 * Multiplier), (int)(50 * Multiplier));  // Moderate weight with multiplier

                list = GetNumberDistributionsForRange(1, 7); // Range: 1-7
                tiers.AddWeightedItem(list, (int)(20 * Multiplier), (int)(30 * Multiplier));  // Low weight with multiplier
            }
            else
            {
                list = GetNumberDistributionsForRange(1, 7); // Range: 1-7
                tiers.AddWeightedItem(list, (int)(10 * Multiplier), (int)(20 * Multiplier));  // Very low weight with multiplier

                list = GetNumberDistributionsForRange(11, 12); // Range: 11-12
                tiers.AddWeightedItem(list, (int)(100 * Multiplier), (int)(120 * Multiplier));  // Very high weight with multiplier

                list = GetNumberDistributionsForRange(8, 10); // Range: 8 to 10
                tiers.AddWeightedItem(list, (int)(10 * Multiplier), (int)(20 * Multiplier));  // Very low weight with multiplier
            }
        });
    }
    private static void PopulateTwoSetsOfFourSomewhatLucky(this GameTieredDistribution<int> tiers, int repeats)
    {
        tiers.SelectTier(LuckStaticClass.SomewhatLucky, tiers =>
        {
            // Wild weight based on repeats for Somewhat Lucky
            int wildWeight;

            if (repeats > 2)
            {
                wildWeight = 200;  // High chance for wilds after 2 repeats
            }
            else if (repeats == 2)
            {
                wildWeight = 150;  // Still high, but slightly less than after 3 repeats
            }
            else if (repeats == 1)
            {
                wildWeight = 130;  // Moderate high chance for wilds on the first repeat
            }
            else
            {
                wildWeight = 90;   // Moderate chance for wilds when not repeated yet
            }

            // Add wilds with the calculated weight (no multiplier applied)
            var list = GetWilds;
            tiers.AddWeightedItem(list, wildWeight);

            // Apply multiplier to the number distributions for Somewhat Lucky
            if (repeats > 2)
            {
                list = GetNumberDistributionsForRange(7, 8); // Range: 7-8
                tiers.AddWeightedItem(list, (int)(70 * Multiplier), (int)(90 * Multiplier));  // High weight with multiplier

                list = GetNumberDistributionsForRange(9, 12); // Range: 9-12
                tiers.AddWeightedItem(list, (int)(10 * Multiplier), (int)(20 * Multiplier));  // Low weight with multiplier

                list = GetNumberDistributionsForRange(1, 6); // Range: 1-6
                tiers.AddWeightedItem(list, (int)(10 * Multiplier), (int)(20 * Multiplier));  // Low weight with multiplier
            }
            else if (repeats == 2)
            {
                list = GetNumberDistributionsForRange(7, 8); // Range: 7-8
                tiers.AddWeightedItem(list, (int)(60 * Multiplier), (int)(80 * Multiplier));  // Moderate weight with multiplier

                list = GetNumberDistributionsForRange(9, 12); // Range: 9-12
                tiers.AddWeightedItem(list, (int)(10 * Multiplier), (int)(20 * Multiplier));  // Low weight with multiplier

                list = GetNumberDistributionsForRange(1, 6); // Range: 1-6
                tiers.AddWeightedItem(list, (int)(20 * Multiplier), (int)(40 * Multiplier));  // Moderate weight with multiplier
            }
            else if (repeats == 1)
            {
                list = GetNumberDistributionsForRange(7, 8); // Range: 7-8
                tiers.AddWeightedItem(list, (int)(50 * Multiplier), (int)(70 * Multiplier));  // Balanced weight with multiplier

                list = GetNumberDistributionsForRange(9, 12); // Range: 9-12
                tiers.AddWeightedItem(list, (int)(10 * Multiplier), (int)(20 * Multiplier));  // Low weight with multiplier

                list = GetNumberDistributionsForRange(1, 6); // Range: 1-6
                tiers.AddWeightedItem(list, (int)(30 * Multiplier), (int)(50 * Multiplier));  // High weight with multiplier
            }
            else
            {
                list = GetNumberDistributionsForRange(1, 6); // Range: 1-6
                tiers.AddWeightedItem(list, (int)(20 * Multiplier), (int)(30 * Multiplier));  // Low weight with multiplier

                list = GetNumberDistributionsForRange(7, 8); // Range: 7-8
                tiers.AddWeightedItem(list, (int)(60 * Multiplier), (int)(60 * Multiplier));  // High weight with multiplier

                list = GetNumberDistributionsForRange(9, 12); // Range: 9-12
                tiers.AddWeightedItem(list, (int)(10 * Multiplier), (int)(20 * Multiplier));  // Low weight with multiplier
            }
        });
    }
    private static void PopulateTwoSetsOfFourAverage(this GameTieredDistribution<int> tiers, int repeats)
    {
        tiers.SelectTier(LuckStaticClass.Average, tiers =>
        {
            // Wild weight based on repeats for Average
            int wildWeight;

            if (repeats > 2)
            {
                wildWeight = 140;  // Slightly higher chance for wilds after 2 repeats
            }
            else if (repeats == 2)
            {
                wildWeight = 120;  // Still moderate, but slightly less than after 3 repeats
            }
            else if (repeats == 1)
            {
                wildWeight = 110;  // Moderate chance for wilds on the first repeat
            }
            else
            {
                wildWeight = 100;  // Standard moderate chance for wilds when not repeated yet
            }

            // Add wilds with the calculated weight (no multiplier applied)
            var list = GetWilds;
            tiers.AddWeightedItem(list, wildWeight);

            // Apply multiplier to the number distributions for Somewhat Lucky
            if (repeats > 2)
            {
                list = GetNumberDistributionsForRange(5, 6); // Range: 5-6
                tiers.AddWeightedItem(list, (int)(70 * Multiplier), (int)(90 * Multiplier));  // High weight with multiplier

                list = GetNumberDistributionsForRange(7, 12); // Range: 7-12
                tiers.AddWeightedItem(list, (int)(10 * Multiplier), (int)(20 * Multiplier));  // Low weight with multiplier

                list = GetNumberDistributionsForRange(1, 4); // Range: 1-4
                tiers.AddWeightedItem(list, (int)(10 * Multiplier), (int)(20 * Multiplier));  // Low weight with multiplier
            }
            else if (repeats == 2)
            {
                list = GetNumberDistributionsForRange(5, 6); // Range: 5-6
                tiers.AddWeightedItem(list, (int)(80 * Multiplier), (int)(80 * Multiplier));  // Moderate weight with multiplier

                list = GetNumberDistributionsForRange(7, 12); // Range: 7-12
                tiers.AddWeightedItem(list, (int)(10 * Multiplier), (int)(30 * Multiplier));  // Low weight with multiplier

                list = GetNumberDistributionsForRange(1, 4); // Range: 1-4
                tiers.AddWeightedItem(list, (int)(20 * Multiplier), (int)(30 * Multiplier));  // Moderate weight with multiplier
            }
            else if (repeats == 1)
            {
                list = GetNumberDistributionsForRange(5, 6); // Range: 5-6
                tiers.AddWeightedItem(list, (int)(70 * Multiplier), (int)(70 * Multiplier));  // Balanced weight with multiplier

                list = GetNumberDistributionsForRange(7, 12); // Range: 7-12
                tiers.AddWeightedItem(list, (int)(10 * Multiplier), (int)(20 * Multiplier));  // Low weight with multiplier

                list = GetNumberDistributionsForRange(1, 4); // Range: 1-4
                tiers.AddWeightedItem(list, (int)(30 * Multiplier), (int)(30 * Multiplier));  // High weight with multiplier
            }
            else
            {
                list = GetNumberDistributionsForRange(1, 4); // Range: 1-4
                tiers.AddWeightedItem(list, (int)(20 * Multiplier), (int)(30 * Multiplier));  // Low weight with multiplier

                list = GetNumberDistributionsForRange(5, 6); // Range: 7-12
                tiers.AddWeightedItem(list, (int)(60 * Multiplier), (int)(60 * Multiplier));  // High weight with multiplier

                list = GetNumberDistributionsForRange(7, 12); // Range: 7-12
                tiers.AddWeightedItem(list, (int)(10 * Multiplier), (int)(20 * Multiplier));  // Low weight with multiplier
            }
        });
    }
    private static void PopulateTwoSetsOfFourBadLuck(this GameTieredDistribution<int> tiers, int repeats)
    {
        tiers.SelectTier(LuckStaticClass.Bad, tiers =>
        {
            // Wild weight based on repeats for Bad Luck
            int wildWeight;

            if (repeats > 2)
            {
                wildWeight = 150;  // Low chance for wilds after 2 repeats (still higher than initial)
            }
            else if (repeats == 2)
            {
                wildWeight = 130;  // Still relatively low, but a bit higher than after 1 repeat
            }
            else if (repeats == 1)
            {
                wildWeight = 120;  // Moderate low chance for wilds
            }
            else
            {
                wildWeight = 100;  // Moderate low chance for wilds when not repeated yet
            }

            // Add wilds with the calculated weight (no multiplier applied)
            var list = GetWilds;
            tiers.AddWeightedItem(list, wildWeight);

            // Apply multiplier to the number distributions for Bad Luck
            if (repeats > 2)
            {
                list = GetNumberDistributionsForRange(1, 2); // Range: 1-2
                tiers.AddWeightedItem(list, (int)(90 * Multiplier), (int)(110 * Multiplier));  // High weight with multiplier

                list = GetNumberDistributionsForRange(3, 6); // Range: 3-6
                tiers.AddWeightedItem(list, (int)(20 * Multiplier), (int)(40 * Multiplier));  // Low weight with multiplier

                list = GetNumberDistributionsForRange(7, 12); // Range: 7-12
                tiers.AddWeightedItem(list, (int)(10 * Multiplier), (int)(20 * Multiplier));  // Very low weight with multiplier
            }
            else if (repeats == 2)
            {
                list = GetNumberDistributionsForRange(1, 2); // Range: 1-2
                tiers.AddWeightedItem(list, (int)(70 * Multiplier), (int)(90 * Multiplier));  // Moderate-high weight with multiplier

                list = GetNumberDistributionsForRange(3, 6); // Range: 3-6
                tiers.AddWeightedItem(list, (int)(40 * Multiplier), (int)(60 * Multiplier));  // Moderate weight with multiplier

                list = GetNumberDistributionsForRange(7, 12); // Range: 7-12
                tiers.AddWeightedItem(list, (int)(20 * Multiplier), (int)(40 * Multiplier));  // Low weight with multiplier
            }
            else if (repeats == 1)
            {
                list = GetNumberDistributionsForRange(1, 2); // Range: 1-2
                tiers.AddWeightedItem(list, (int)(60 * Multiplier), (int)(80 * Multiplier));  // Moderate weight with multiplier

                list = GetNumberDistributionsForRange(3, 6); // Range: 3-6
                tiers.AddWeightedItem(list, (int)(50 * Multiplier), (int)(70 * Multiplier));  // Moderate-high weight with multiplier

                list = GetNumberDistributionsForRange(7, 12); // Range: 7-12
                tiers.AddWeightedItem(list, (int)(30 * Multiplier), (int)(50 * Multiplier));  // Low weight with multiplier
            }
            else
            {
                list = GetNumberDistributionsForRange(1, 2); // Range: 1-2
                tiers.AddWeightedItem(list, (int)(50 * Multiplier), (int)(70 * Multiplier));  // High weight with multiplier

                list = GetNumberDistributionsForRange(3, 6); // Range: 3-6
                tiers.AddWeightedItem(list, (int)(60 * Multiplier), (int)(80 * Multiplier));  // Moderate-high weight with multiplier

                list = GetNumberDistributionsForRange(7, 12); // Range: 7-12
                tiers.AddWeightedItem(list, (int)(30 * Multiplier), (int)(50 * Multiplier));  // Moderate weight with multiplier
            }
        });
    }
    private static void PopulateLastPhasesSomewhatLucky(this GameTieredDistribution<int> tiers, int repeats)
    {
        tiers.SelectTier(LuckStaticClass.SomewhatLucky, tiers =>
        {
            // Wild weight based on repeats for Somewhat Lucky (no multiplier)
            int wildWeight;

            if (repeats > 2)
            {
                wildWeight = 210;  // High chance for wilds after 2 repeats
            }
            else if (repeats == 2)
            {
                wildWeight = 190;  // Moderate-high chance
            }
            else if (repeats == 1)
            {
                wildWeight = 170;  // Moderate chance for wilds on the first repeat
            }
            else
            {
                wildWeight = 140;  // Lower chance for wilds when not repeated yet
            }

            // Add wilds with the calculated weight (without multiplier)
            var list = GetWilds;
            tiers.AddWeightedItem(list, wildWeight);

            // Randomly pick a number between 7 and 10 (lean towards that number)
            BasicList<int> range = Enumerable.Range(7, 4).ToBasicList(); // Numbers 7, 8, 9, 10
            int chosenNumber = range.GetRandomItem();  // Randomly select one of these numbers

            // Lean towards the chosen number (7, 8, 9, or 10)
            if (repeats > 2)
            {
                // Set of 5: Lean towards the chosen number (7, 8, 9, or 10)
                list = GetDistributionForNumber(chosenNumber);  // Chosen number (either 7, 8, 9, or 10)
                tiers.AddWeightedItem(list, (int)(100 * Multiplier), (int)(120 * Multiplier));  // Leaned weight for the chosen number

                // Add other numbers (1-6, 11-12), but with lower chances (with multiplier)
                for (int i = 1; i <= 6; i++)
                {
                    list = GetDistributionForNumber(i);  // Numbers 1-6
                    tiers.AddWeightedItem(list, (int)(40 * Multiplier), (int)(60 * Multiplier));  // Lower weight for numbers 1-6
                }

                for (int i = 11; i <= 12; i++)
                {
                    list = GetDistributionForNumber(i);  // Numbers 11-12
                    tiers.AddWeightedItem(list, (int)(40 * Multiplier), (int)(60 * Multiplier));  // Lower weight for numbers 11-12
                }
            }
            else if (repeats == 2)
            {
                // Set of 5: Lean towards the chosen number (7, 8, 9, or 10)
                list = GetDistributionForNumber(chosenNumber);  // Chosen number (either 7, 8, 9, or 10)
                tiers.AddWeightedItem(list, (int)(80 * Multiplier), (int)(100 * Multiplier));  // Leaned weight for the chosen number

                // Add other numbers (1-6, 11-12), but with moderate-low chances (with multiplier)
                for (int i = 1; i <= 6; i++)
                {
                    list = GetDistributionForNumber(i);  // Numbers 1-6
                    tiers.AddWeightedItem(list, (int)(50 * Multiplier), (int)(70 * Multiplier));  // Moderate-low weight for numbers 1-6
                }

                for (int i = 11; i <= 12; i++)
                {
                    list = GetDistributionForNumber(i);  // Numbers 11-12
                    tiers.AddWeightedItem(list, (int)(50 * Multiplier), (int)(70 * Multiplier));  // Moderate-low weight for numbers 11-12
                }
            }
            else if (repeats == 1)
            {
                // Set of 5: Lean towards the chosen number (7, 8, 9, or 10)
                list = GetDistributionForNumber(chosenNumber);  // Chosen number (either 7, 8, 9, or 10)
                tiers.AddWeightedItem(list, (int)(70 * Multiplier), (int)(90 * Multiplier));  // Leaned weight for the chosen number

                // Add other numbers (1-6, 11-12), but with moderate chances (with multiplier)
                for (int i = 1; i <= 6; i++)
                {
                    list = GetDistributionForNumber(i);  // Numbers 1-6
                    tiers.AddWeightedItem(list, (int)(60 * Multiplier), (int)(80 * Multiplier));  // Moderate weight for numbers 1-6
                }

                for (int i = 11; i <= 12; i++)
                {
                    list = GetDistributionForNumber(i);  // Numbers 11-12
                    tiers.AddWeightedItem(list, (int)(60 * Multiplier), (int)(80 * Multiplier));  // Moderate weight for numbers 11-12
                }
            }
            else
            {
                // Set of 5: Lean towards the chosen number (7, 8, 9, or 10)
                list = GetDistributionForNumber(chosenNumber);  // Chosen number (either 7, 8, 9, or 10)
                tiers.AddWeightedItem(list, (int)(60 * Multiplier), (int)(80 * Multiplier));  // Leaned weight for the chosen number

                // Add other numbers (1-6, 11-12), but with moderate-high chances (with multiplier)
                for (int i = 1; i <= 6; i++)
                {
                    list = GetDistributionForNumber(i);  // Numbers 1-6
                    tiers.AddWeightedItem(list, (int)(70 * Multiplier), (int)(90 * Multiplier));  // Moderate-high weight for numbers 1-6
                }

                for (int i = 11; i <= 12; i++)
                {
                    list = GetDistributionForNumber(i);  // Numbers 11-12
                    tiers.AddWeightedItem(list, (int)(70 * Multiplier), (int)(90 * Multiplier));  // Moderate-high weight for numbers 11-12
                }
            }
        });
    }
    private static void PopulateLastPhasesBadLuck(this GameTieredDistribution<int> tiers, int repeats)
    {
        tiers.SelectTier(LuckStaticClass.Bad, tiers =>
        {
            // Wild weight based on repeats for Bad Luck (no multiplier)
            int wildWeight;

            if (repeats > 2)
            {
                wildWeight = 150;  // High chance for wilds after 2 repeats (but much lower than Very Lucky)
            }
            else if (repeats == 2)
            {
                wildWeight = 130;  // High chance, but slightly lower
            }
            else if (repeats == 1)
            {
                wildWeight = 110;  // Moderate chance for wilds on the first repeat
            }
            else
            {
                wildWeight = 90;  // Lower chance for wilds when not repeated yet
            }

            // Add wilds with the calculated weight (without multiplier)
            var list = GetWilds;
            tiers.AddWeightedItem(list, wildWeight);

            // Randomly pick a number between 1 and 3
            BasicList<int> listRange = Enumerable.Range(1, 3).ToBasicList();
            int chosenNumber = listRange.GetRandomItem();

            // Lean towards the randomly chosen number (1, 2, or 3)
            if (repeats > 2)
            {
                // Set of 5: Lean towards the chosen number (1, 2, or 3)
                list = GetDistributionForNumber(chosenNumber);  // Chosen number
                tiers.AddWeightedItem(list, (int)(80 * Multiplier), (int)(100 * Multiplier));  // Leaned weight for the chosen number

                // Add other numbers (1-12), but with lower chances (with multiplier)
                for (int i = 1; i <= 12; i++)
                {
                    if (i != chosenNumber)
                    {
                        list = GetDistributionForNumber(i);  // Other numbers
                        tiers.AddWeightedItem(list, (int)(30 * Multiplier), (int)(50 * Multiplier));  // Lower weight for other numbers
                    }
                }
            }
            else if (repeats == 2)
            {
                // Set of 5: Lean towards the chosen number (1, 2, or 3)
                list = GetDistributionForNumber(chosenNumber);  // Chosen number
                tiers.AddWeightedItem(list, (int)(60 * Multiplier), (int)(80 * Multiplier));  // Leaned weight for the chosen number

                // Add other numbers (1-12), but with moderate-low chances (with multiplier)
                for (int i = 1; i <= 12; i++)
                {
                    if (i != chosenNumber)
                    {
                        list = GetDistributionForNumber(i);  // Other numbers
                        tiers.AddWeightedItem(list, (int)(50 * Multiplier), (int)(70 * Multiplier));  // Moderate-low weight for other numbers
                    }
                }
            }
            else if (repeats == 1)
            {
                // Set of 5: Lean towards the chosen number (1, 2, or 3)
                list = GetDistributionForNumber(chosenNumber);  // Chosen number
                tiers.AddWeightedItem(list, (int)(50 * Multiplier), (int)(70 * Multiplier));  // Leaned weight for the chosen number

                // Add other numbers (1-12), but with moderate chances (with multiplier)
                for (int i = 1; i <= 12; i++)
                {
                    if (i != chosenNumber)
                    {
                        list = GetDistributionForNumber(i);  // Other numbers
                        tiers.AddWeightedItem(list, (int)(50 * Multiplier), (int)(70 * Multiplier));  // Moderate weight for other numbers
                    }
                }
            }
            else
            {
                // Set of 5: Lean towards the chosen number (1, 2, or 3)
                list = GetDistributionForNumber(chosenNumber);  // Chosen number
                tiers.AddWeightedItem(list, (int)(40 * Multiplier), (int)(60 * Multiplier));  // Leaned weight for the chosen number

                // Add other numbers (1-12), but with moderate-high chances (with multiplier)
                for (int i = 1; i <= 12; i++)
                {
                    if (i != chosenNumber)
                    {
                        list = GetDistributionForNumber(i);  // Other numbers
                        tiers.AddWeightedItem(list, (int)(60 * Multiplier), (int)(80 * Multiplier));  // Moderate-high weight for other numbers
                    }
                }
            }
        });
    }
    private static void PopulateLastPhasesVeryLucky(this GameTieredDistribution<int> tiers, int repeats)
    {
        tiers.SelectTier(LuckStaticClass.Lucky, tiers =>
        {
            // Wild weight based on repeats for Very Lucky
            int wildWeight;

            if (repeats > 2)
            {
                wildWeight = 250;  // High chance for wilds after 2 repeats
            }
            else if (repeats == 2)
            {
                wildWeight = 230;  // High chance, but slightly lower
            }
            else if (repeats == 1)
            {
                wildWeight = 200;  // Moderate chance for wilds on the first repeat
            }
            else
            {
                wildWeight = 170;  // Lower chance for wilds when not repeated yet
            }

            // Add wilds with the calculated weight (without multiplying)
            var list = GetWilds;
            tiers.AddWeightedItem(list, wildWeight);

            // Lean towards number 12 for Very Lucky last phase (Set of 5 focus)
            if (repeats > 2)
            {
                // Set of 5: Lean towards 12 but not overwhelming
                list = GetDistributionForNumber(12); // Number 12 only
                tiers.AddWeightedItem(list, (int)(130 * Multiplier), (int)(150 * Multiplier));  // Moderate-high weight for number 12 (leaning but not too much)

                // Add other numbers (1 through 11) with moderate chances
                for (int i = 1; i <= 11; i++)
                {
                    list = GetDistributionForNumber(i);  // Distribution for a specific number
                    tiers.AddWeightedItem(list, (int)(50 * Multiplier), (int)(70 * Multiplier));  // Moderate chance for numbers 1-11
                }
            }
            else if (repeats == 2)
            {
                // Set of 5: Lean towards 12 but not overwhelming
                list = GetDistributionForNumber(12); // Number 12 only
                tiers.AddWeightedItem(list, (int)(110 * Multiplier), (int)(130 * Multiplier));  // Moderate weight for number 12

                // Add other numbers (1 through 11) with moderate chances
                for (int i = 1; i <= 11; i++)
                {
                    list = GetDistributionForNumber(i);  // Distribution for a specific number
                    tiers.AddWeightedItem(list, (int)(60 * Multiplier), (int)(80 * Multiplier));  // Moderate chance for numbers 1-11
                }
            }
            else if (repeats == 1)
            {
                // Set of 5: Lean towards 12 but not overwhelming
                list = GetDistributionForNumber(12); // Number 12 only
                tiers.AddWeightedItem(list, (int)(90 * Multiplier), (int)(110 * Multiplier));  // Moderate-high weight for number 12

                // Add other numbers (1 through 11) with moderate chances
                for (int i = 1; i <= 11; i++)
                {
                    list = GetDistributionForNumber(i);  // Distribution for a specific number
                    tiers.AddWeightedItem(list, (int)(70 * Multiplier), (int)(90 * Multiplier));  // Moderate chance for numbers 1-11
                }
            }
            else
            {
                // Set of 5: Lean towards 12 but not overwhelming
                list = GetDistributionForNumber(12); // Number 12 only
                tiers.AddWeightedItem(list, (int)(70 * Multiplier), (int)(90 * Multiplier));  // Moderate weight for number 12

                // Add other numbers (1 through 11) with moderate chances
                for (int i = 1; i <= 11; i++)
                {
                    list = GetDistributionForNumber(i);  // Distribution for a specific number
                    tiers.AddWeightedItem(list, (int)(80 * Multiplier), (int)(100 * Multiplier));  // Moderate-high chance for numbers 1-11
                }
            }
        });
    }

    private static void PopulateLastPhasesAverage(this GameTieredDistribution<int> tiers, int repeats)
    {
        tiers.SelectTier(LuckStaticClass.Average, tiers =>
        {
            // Wild weight based on repeats for Average (moderate chance compared to Very Lucky)
            int wildWeight;

            if (repeats > 2)
            {
                wildWeight = 180;  // High chance for wilds after 2 repeats (but lower than Very Lucky)
            }
            else if (repeats == 2)
            {
                wildWeight = 160;  // Moderate-high chance
            }
            else if (repeats == 1)
            {
                wildWeight = 140;  // Moderate chance for wilds on the first repeat
            }
            else
            {
                wildWeight = 110;  // Lower chance for wilds when not repeated yet
            }

            // Add wilds with the calculated weight (without multiplying)
            var list = GetWilds;
            tiers.AddWeightedItem(list, wildWeight);

            // Randomly pick a number between 4 and 6 (lean towards that number)
            BasicList<int> range = Enumerable.Range(4, 3).ToBasicList(); // Numbers 4, 5, and 6
            int chosenNumber = range.GetRandomItem();  // Randomly select one of these numbers

            // Lean towards the chosen number (4, 5, or 6)
            if (repeats > 2)
            {
                // Set of 5: Lean towards the chosen number (4, 5, or 6)
                list = GetDistributionForNumber(chosenNumber);  // Chosen number (either 4, 5, or 6)
                tiers.AddWeightedItem(list, (int)(80 * Multiplier), (int)(100 * Multiplier));  // Leaned weight for the chosen number

                // Add other numbers (1-3, 7-12), but with lower chances
                for (int i = 1; i <= 3; i++)
                {
                    list = GetDistributionForNumber(i);  // Numbers 1-3
                    tiers.AddWeightedItem(list, (int)(30 * Multiplier), (int)(50 * Multiplier));  // Lower weight for numbers 1-3
                }

                for (int i = 7; i <= 12; i++)
                {
                    list = GetDistributionForNumber(i);  // Numbers 7-12
                    tiers.AddWeightedItem(list, (int)(30 * Multiplier), (int)(50 * Multiplier));  // Lower weight for numbers 7-12
                }
            }
            else if (repeats == 2)
            {
                // Set of 5: Lean towards the chosen number (4, 5, or 6)
                list = GetDistributionForNumber(chosenNumber);  // Chosen number (either 4, 5, or 6)
                tiers.AddWeightedItem(list, (int)(60 * Multiplier), (int)(80 * Multiplier));  // Leaned weight for the chosen number

                // Add other numbers (1-3, 7-12), but with moderate-low chances
                for (int i = 1; i <= 3; i++)
                {
                    list = GetDistributionForNumber(i);  // Numbers 1-3
                    tiers.AddWeightedItem(list, (int)(40 * Multiplier), (int)(60 * Multiplier));  // Moderate-low weight for numbers 1-3
                }

                for (int i = 7; i <= 12; i++)
                {
                    list = GetDistributionForNumber(i);  // Numbers 7-12
                    tiers.AddWeightedItem(list, (int)(40 * Multiplier), (int)(60 * Multiplier));  // Moderate-low weight for numbers 7-12
                }
            }
            else if (repeats == 1)
            {
                // Set of 5: Lean towards the chosen number (4, 5, or 6)
                list = GetDistributionForNumber(chosenNumber);  // Chosen number (either 4, 5, or 6)
                tiers.AddWeightedItem(list, (int)(50 * Multiplier), (int)(70 * Multiplier));  // Leaned weight for the chosen number

                // Add other numbers (1-3, 7-12), but with moderate chances
                for (int i = 1; i <= 3; i++)
                {
                    list = GetDistributionForNumber(i);  // Numbers 1-3
                    tiers.AddWeightedItem(list, (int)(50 * Multiplier), (int)(70 * Multiplier));  // Moderate weight for numbers 1-3
                }

                for (int i = 7; i <= 12; i++)
                {
                    list = GetDistributionForNumber(i);  // Numbers 7-12
                    tiers.AddWeightedItem(list, (int)(50 * Multiplier), (int)(70 * Multiplier));  // Moderate weight for numbers 7-12
                }
            }
            else
            {
                // Set of 5: Lean towards the chosen number (4, 5, or 6)
                list = GetDistributionForNumber(chosenNumber);  // Chosen number (either 4, 5, or 6)
                tiers.AddWeightedItem(list, (int)(50 * Multiplier), (int)(70 * Multiplier));  // Leaned weight for the chosen number

                // Add other numbers (1-3, 7-12), but with moderate chances
                for (int i = 1; i <= 3; i++)
                {
                    list = GetDistributionForNumber(i);  // Numbers 1-3
                    tiers.AddWeightedItem(list, (int)(50 * Multiplier), (int)(70 * Multiplier));  // Moderate weight for numbers 1-3
                }

                for (int i = 7; i <= 12; i++)
                {
                    list = GetDistributionForNumber(i);  // Numbers 7-12
                    tiers.AddWeightedItem(list, (int)(50 * Multiplier), (int)(70 * Multiplier));  // Moderate weight for numbers 7-12
                }
            }
        });
    }

    private static void PopulateRegularVeryLucky(this GameTieredDistribution<int> tiers, int repeats)
    {
        tiers.SelectTier(LuckStaticClass.Lucky, tiers =>
        {
            var list = GetWilds;
            int wildWeight;

            // Adjust wild weight based on the number of repeats (more repeats = better luck)
            if (repeats > 2)
            {
                wildWeight = 150;  // High chance for wilds after 2 repeats
            }
            else if (repeats == 2)
            {
                wildWeight = 140;  // Moderate-high chance for wilds
            }
            else if (repeats == 1)
            {
                wildWeight = 130;  // Moderate chance for wilds
            }
            else
            {
                wildWeight = 110;  // Lower chance for wilds when not repeated yet
            }

            // Add wilds to the distribution (no multiplier applied)
            tiers.AddWeightedItem(list, wildWeight);

            // Grouping by repeats now:

            if (repeats > 2)
            {
                // Very high chance for 11-12 (with multiplier)
                list = GetNumberDistributionsForRange(11, 12);
                tiers.AddWeightedItem(list, (int)(80 * Multiplier), (int)(100 * Multiplier));  // Very high chance for 11-12

                // High chance for 9-10 (with multiplier)
                list = GetNumberDistributionsForRange(9, 10);
                tiers.AddWeightedItem(list, (int)(60 * Multiplier), (int)(80 * Multiplier));

                // Moderate-high chance for 7-8 (with multiplier)
                list = GetNumberDistributionsForRange(7, 8);
                tiers.AddWeightedItem(list, (int)(50 * Multiplier), (int)(70 * Multiplier));

                // Moderate chance for 5-6 (with multiplier)
                list = GetNumberDistributionsForRange(5, 6);
                tiers.AddWeightedItem(list, (int)(40 * Multiplier), (int)(60 * Multiplier));

                // Lower chance for 3-4 (with multiplier)
                list = GetNumberDistributionsForRange(3, 4);
                tiers.AddWeightedItem(list, (int)(20 * Multiplier), (int)(40 * Multiplier));

                // Very low chance for 1-2 (with multiplier)
                list = GetNumberDistributionsForRange(1, 2);
                tiers.AddWeightedItem(list, (int)(10 * Multiplier), (int)(20 * Multiplier));
            }
            else if (repeats == 2)
            {
                // High chance for 11-12 (slightly lower than repeats > 2, with multiplier)
                list = GetNumberDistributionsForRange(11, 12);
                tiers.AddWeightedItem(list, (int)(70 * Multiplier), (int)(90 * Multiplier));  // High chance for 11-12

                // High chance for 9-10 (with multiplier)
                list = GetNumberDistributionsForRange(9, 10);
                tiers.AddWeightedItem(list, (int)(60 * Multiplier), (int)(80 * Multiplier));

                // Moderate-high chance for 7-8 (with multiplier)
                list = GetNumberDistributionsForRange(7, 8);
                tiers.AddWeightedItem(list, (int)(50 * Multiplier), (int)(70 * Multiplier));

                // Moderate chance for 5-6 (with multiplier)
                list = GetNumberDistributionsForRange(5, 6);
                tiers.AddWeightedItem(list, (int)(40 * Multiplier), (int)(60 * Multiplier));

                // Lower chance for 3-4 (with multiplier)
                list = GetNumberDistributionsForRange(3, 4);
                tiers.AddWeightedItem(list, (int)(20 * Multiplier), (int)(40 * Multiplier));

                // Very low chance for 1-2 (with multiplier)
                list = GetNumberDistributionsForRange(1, 2);
                tiers.AddWeightedItem(list, (int)(10 * Multiplier), (int)(20 * Multiplier));
            }
            else if (repeats == 1)
            {
                // High chance for 11-12 (slightly lower than repeats > 2 and == 2, with multiplier)
                list = GetNumberDistributionsForRange(11, 12);
                tiers.AddWeightedItem(list, (int)(60 * Multiplier), (int)(80 * Multiplier));  // High chance for 11-12

                // High chance for 9-10 (with multiplier)
                list = GetNumberDistributionsForRange(9, 10);
                tiers.AddWeightedItem(list, (int)(50 * Multiplier), (int)(70 * Multiplier));

                // Moderate-high chance for 7-8 (with multiplier)
                list = GetNumberDistributionsForRange(7, 8);
                tiers.AddWeightedItem(list, (int)(60 * Multiplier), (int)(80 * Multiplier));

                // Moderate chance for 5-6 (with multiplier)
                list = GetNumberDistributionsForRange(5, 6);
                tiers.AddWeightedItem(list, (int)(40 * Multiplier), (int)(60 * Multiplier));

                // Lower chance for 3-4 (with multiplier)
                list = GetNumberDistributionsForRange(3, 4);
                tiers.AddWeightedItem(list, (int)(30 * Multiplier), (int)(50 * Multiplier));

                // Very low chance for 1-2 (with multiplier)
                list = GetNumberDistributionsForRange(1, 2);
                tiers.AddWeightedItem(list, (int)(10 * Multiplier), (int)(20 * Multiplier));
            }
            else
            {
                // High chance for 11-12, higher than all other ranges (with multiplier)
                list = GetNumberDistributionsForRange(11, 12);
                tiers.AddWeightedItem(list, (int)(50 * Multiplier), (int)(70 * Multiplier));  // High chance for 11-12 even when no repeats

                // Moderate chance for 9-10 (with multiplier)
                list = GetNumberDistributionsForRange(9, 10);
                tiers.AddWeightedItem(list, (int)(40 * Multiplier), (int)(60 * Multiplier));

                // Moderate chance for 7-8 (with multiplier)
                list = GetNumberDistributionsForRange(7, 8);
                tiers.AddWeightedItem(list, (int)(50 * Multiplier), (int)(70 * Multiplier));

                // Moderate chance for 5-6 (with multiplier)
                list = GetNumberDistributionsForRange(5, 6);
                tiers.AddWeightedItem(list, (int)(40 * Multiplier), (int)(60 * Multiplier));

                // Lower chance for 3-4 (with multiplier)
                list = GetNumberDistributionsForRange(3, 4);
                tiers.AddWeightedItem(list, (int)(30 * Multiplier), (int)(50 * Multiplier));

                // Very low chance for 1-2 (with multiplier)
                list = GetNumberDistributionsForRange(1, 2);
                tiers.AddWeightedItem(list, (int)(10 * Multiplier), (int)(20 * Multiplier));
            }
        });
    }
    private static void PopulateRegularSomewhatLucky(this GameTieredDistribution<int> tiers, int repeats)
    {
        tiers.SelectTier(LuckStaticClass.SomewhatLucky, tiers =>
        {
            var list = GetWilds;
            int wildWeight;

            // Adjust wild weight based on the number of repeats (no multiplier applied here)
            if (repeats > 2)
            {
                wildWeight = 130;  // High chance for wilds after 2 repeats
            }
            else if (repeats == 2)
            {
                wildWeight = 120;  // Moderate chance for wilds
            }
            else if (repeats == 1)
            {
                wildWeight = 110;  // Lower chance for wilds
            }
            else
            {
                wildWeight = 100;  // Minimal chance for wilds
            }

            // Add wilds to the distribution (no multiplier applied)
            tiers.AddWeightedItem(list, wildWeight);

            // Number distribution for Somewhat Lucky tier, with the added 4-6 range
            if (repeats > 2)
            {
                // Strongly favor 7-9 (with multiplier)
                list = GetNumberDistributionsForRange(7, 9);
                tiers.AddWeightedItem(list, (int)(60 * Multiplier), (int)(80 * Multiplier));  // Strong weight for 7-9

                // Slight chance for 10-12 (with multiplier)
                list = GetNumberDistributionsForRange(10, 12);
                tiers.AddWeightedItem(list, (int)(15 * Multiplier), (int)(25 * Multiplier));  // Lower weight for 10-12

                // Moderate chance for 4-6 (with multiplier)
                list = GetNumberDistributionsForRange(4, 6);
                tiers.AddWeightedItem(list, (int)(10 * Multiplier), (int)(20 * Multiplier));  // Moderate weight for 4-6

                // Very low chance for 1-3 (with multiplier)
                list = GetNumberDistributionsForRange(1, 3);
                tiers.AddWeightedItem(list, (int)(5 * Multiplier), (int)(10 * Multiplier));  // Very low weight for 1-3
            }
            else if (repeats == 2)
            {
                // Favor 7-9 with a high chance (with multiplier)
                list = GetNumberDistributionsForRange(7, 9);
                tiers.AddWeightedItem(list, (int)(50 * Multiplier), (int)(70 * Multiplier));  // High weight for 7-9

                // Moderate chance for 10-12 (with multiplier)
                list = GetNumberDistributionsForRange(10, 12);
                tiers.AddWeightedItem(list, (int)(20 * Multiplier), (int)(30 * Multiplier));  // Moderate weight for 10-12

                // Moderate chance for 4-6 (with multiplier)
                list = GetNumberDistributionsForRange(4, 6);
                tiers.AddWeightedItem(list, (int)(15 * Multiplier), (int)(25 * Multiplier));  // Moderate weight for 4-6

                // Lower chance for 1-3 (with multiplier)
                list = GetNumberDistributionsForRange(1, 3);
                tiers.AddWeightedItem(list, (int)(10 * Multiplier), (int)(20 * Multiplier));  // Lower weight for 1-3
            }
            else if (repeats == 1)
            {
                // Favor 7-9 with a high chance (with multiplier)
                list = GetNumberDistributionsForRange(7, 9);
                tiers.AddWeightedItem(list, (int)(40 * Multiplier), (int)(50 * Multiplier));  // High weight for 7-9

                // Moderate chance for 10-12 (with multiplier)
                list = GetNumberDistributionsForRange(10, 12);
                tiers.AddWeightedItem(list, (int)(20 * Multiplier), (int)(30 * Multiplier));  // Moderate weight for 10-12

                // Moderate chance for 4-6 (with multiplier)
                list = GetNumberDistributionsForRange(4, 6);
                tiers.AddWeightedItem(list, (int)(20 * Multiplier), (int)(30 * Multiplier));  // Moderate weight for 4-6

                // Lower chance for 1-3 (with multiplier)
                list = GetNumberDistributionsForRange(1, 3);
                tiers.AddWeightedItem(list, (int)(10 * Multiplier), (int)(20 * Multiplier));  // Lower weight for 1-3
            }
            else
            {
                // Lean towards 7-9 with a good chance (with multiplier)
                list = GetNumberDistributionsForRange(7, 9);
                tiers.AddWeightedItem(list, (int)(30 * Multiplier), (int)(40 * Multiplier));  // Moderate weight for 7-9

                // Slight chance for 10-12 (with multiplier)
                list = GetNumberDistributionsForRange(10, 12);
                tiers.AddWeightedItem(list, (int)(20 * Multiplier), (int)(30 * Multiplier));  // Lower weight for 10-12

                // Moderate chance for 4-6 (with multiplier)
                list = GetNumberDistributionsForRange(4, 6);
                tiers.AddWeightedItem(list, (int)(20 * Multiplier), (int)(30 * Multiplier));  // Moderate weight for 4-6

                // Very low chance for 1-3 (with multiplier)
                list = GetNumberDistributionsForRange(1, 3);
                tiers.AddWeightedItem(list, (int)(10 * Multiplier), (int)(20 * Multiplier));  // Very low weight for 1-3
            }
        });
    }


    private static void PopulateRegularAverage(this GameTieredDistribution<int> tiers, int repeats)
    {
        tiers.SelectTier(LuckStaticClass.Average, tiers =>
        {
            var list = GetWilds;
            int wildWeight;

            // Adjust wild weight based on the number of repeats (no multiplier applied here)
            if (repeats > 2)
            {
                wildWeight = 120;  // Slightly lower chance for wilds after 2 repeats
            }
            else if (repeats == 2)
            {
                wildWeight = 110;  // Lower but still moderate chance for wilds
            }
            else if (repeats == 1)
            {
                wildWeight = 100;  // Lower chance for wilds
            }
            else
            {
                wildWeight = 90;  // Minimal chance for wilds when not repeated yet
            }

            // Add wilds to the distribution (no multiplier applied)
            tiers.AddWeightedItem(list, wildWeight);

            // Number distribution for "Average" luck (3 allocations: 1-4, 5-8, 9-12)
            if (repeats > 2)
            {
                // Strongly favor 5-8 range (with multiplier)
                list = GetNumberDistributionsForRange(5, 8);  // Range: 5-8
                tiers.AddWeightedItem(list, (int)(50 * Multiplier), (int)(70 * Multiplier));  // High weight for 5-8

                // Moderate chance for 9-12 range (with multiplier)
                list = GetNumberDistributionsForRange(9, 12);  // Range: 9-12
                tiers.AddWeightedItem(list, (int)(20 * Multiplier), (int)(30 * Multiplier));  // Moderate weight for 9-12

                // Lower chance for 1-4 range (with multiplier)
                list = GetNumberDistributionsForRange(1, 4);  // Range: 1-4
                tiers.AddWeightedItem(list, (int)(10 * Multiplier), (int)(20 * Multiplier));  // Lower weight for 1-4
            }
            else if (repeats == 2)
            {
                // Favor 5-8 range with a high chance (with multiplier)
                list = GetNumberDistributionsForRange(5, 8);  // Range: 5-8
                tiers.AddWeightedItem(list, (int)(40 * Multiplier), (int)(50 * Multiplier));  // High weight for 5-8

                // Moderate chance for 9-12 range (with multiplier)
                list = GetNumberDistributionsForRange(9, 12);  // Range: 9-12
                tiers.AddWeightedItem(list, (int)(20 * Multiplier), (int)(30 * Multiplier));  // Moderate weight for 9-12

                // Lower chance for 1-4 range (with multiplier)
                list = GetNumberDistributionsForRange(1, 4);  // Range: 1-4
                tiers.AddWeightedItem(list, (int)(10 * Multiplier), (int)(20 * Multiplier));  // Lower weight for 1-4
            }
            else if (repeats == 1)
            {
                // Favor 5-8 range with a good chance (with multiplier)
                list = GetNumberDistributionsForRange(5, 8);  // Range: 5-8
                tiers.AddWeightedItem(list, (int)(30 * Multiplier), (int)(40 * Multiplier));  // Moderate weight for 5-8

                // Slight chance for 9-12 range (with multiplier)
                list = GetNumberDistributionsForRange(9, 12);  // Range: 9-12
                tiers.AddWeightedItem(list, (int)(20 * Multiplier), (int)(30 * Multiplier));  // Moderate weight for 9-12

                // Lower chance for 1-4 range (with multiplier)
                list = GetNumberDistributionsForRange(1, 4);  // Range: 1-4
                tiers.AddWeightedItem(list, (int)(10 * Multiplier), (int)(20 * Multiplier));  // Lower weight for 1-4
            }
            else
            {
                // Lean towards 5-8 with a good chance (with multiplier)
                list = GetNumberDistributionsForRange(5, 8);  // Range: 5-8
                tiers.AddWeightedItem(list, (int)(20 * Multiplier), (int)(30 * Multiplier));  // Balanced weight for 5-8

                // Moderate chance for 9-12 range (with multiplier)
                list = GetNumberDistributionsForRange(9, 12);  // Range: 9-12
                tiers.AddWeightedItem(list, (int)(20 * Multiplier), (int)(30 * Multiplier));  // Balanced weight for 9-12

                // Lower chance for 1-4 range (with multiplier)
                list = GetNumberDistributionsForRange(1, 4);  // Range: 1-4
                tiers.AddWeightedItem(list, (int)(10 * Multiplier), (int)(20 * Multiplier));  // Lower weight for 1-4
            }
        });
    }
    private static void PopulateRegularBadLuck(this GameTieredDistribution<int> tiers, int repeats)
    {
        tiers.SelectTier(LuckStaticClass.Bad, tiers =>
        {
            var list = GetWilds;
            int wildWeight;

            // Adjust wild weight based on the number of repeats (no multiplier applied here)
            if (repeats > 2)
            {
                wildWeight = 110;  // Slight chance for wilds after 2 repeats
            }
            else if (repeats == 2)
            {
                wildWeight = 100;  // Minimal chance for wilds
            }
            else if (repeats == 1)
            {
                wildWeight = 90;   // Very low chance for wilds
            }
            else
            {
                wildWeight = 70;   // Almost no chance for wilds when not repeated yet
            }

            // Add wilds to the distribution (no multiplier applied)
            tiers.AddWeightedItem(list, wildWeight);

            // Number distribution for "Bad Luck" (6 allocations: 1-2, 3-4, 5-6, 7-8, 9-10, 11-12)
            if (repeats > 2)
            {
                // Very high chance for 1-2 (with multiplier)
                list = GetNumberDistributionsForRange(1, 2);  // Range: 1-2
                tiers.AddWeightedItem(list, (int)(50 * Multiplier), (int)(70 * Multiplier));  // High weight for 1-2

                // High chance for 3-4 (with multiplier)
                list = GetNumberDistributionsForRange(3, 4);  // Range: 3-4
                tiers.AddWeightedItem(list, (int)(40 * Multiplier), (int)(60 * Multiplier));  // High weight for 3-4

                // Moderate chance for 5-6 (with multiplier)
                list = GetNumberDistributionsForRange(5, 6);  // Range: 5-6
                tiers.AddWeightedItem(list, (int)(30 * Multiplier), (int)(40 * Multiplier));  // Moderate weight for 5-6

                // Lower chance for 7-8 (with multiplier)
                list = GetNumberDistributionsForRange(7, 8);  // Range: 7-8
                tiers.AddWeightedItem(list, (int)(20 * Multiplier), (int)(30 * Multiplier));  // Lower weight for 7-8

                // Very low chance for 9-10 (with multiplier)
                list = GetNumberDistributionsForRange(9, 10);  // Range: 9-10
                tiers.AddWeightedItem(list, (int)(10 * Multiplier), (int)(20 * Multiplier));  // Very low weight for 9-10

                // Minimal chance for 11-12 (with multiplier)
                list = GetNumberDistributionsForRange(11, 12);  // Range: 11-12
                tiers.AddWeightedItem(list, (int)(4 * Multiplier), (int)(5 * Multiplier));  // Minimal weight for 11-12
            }
            else if (repeats == 2)
            {
                // High chance for 1-2 (with multiplier)
                list = GetNumberDistributionsForRange(1, 2);  // Range: 1-2
                tiers.AddWeightedItem(list, (int)(40 * Multiplier), (int)(50 * Multiplier));  // High weight for 1-2

                // Moderate chance for 3-4 (with multiplier)
                list = GetNumberDistributionsForRange(3, 4);  // Range: 3-4
                tiers.AddWeightedItem(list, (int)(30 * Multiplier), (int)(50 * Multiplier));  // Moderate weight for 3-4

                // Balanced chance for 5-6 (with multiplier)
                list = GetNumberDistributionsForRange(5, 6);  // Range: 5-6
                tiers.AddWeightedItem(list, (int)(20 * Multiplier), (int)(30 * Multiplier));  // Balanced weight for 5-6

                // Lower chance for 7-8 (with multiplier)
                list = GetNumberDistributionsForRange(7, 8);  // Range: 7-8
                tiers.AddWeightedItem(list, (int)(10 * Multiplier), (int)(20 * Multiplier));  // Lower weight for 7-8

                // Very low chance for 9-10 (with multiplier)
                list = GetNumberDistributionsForRange(9, 10);  // Range: 9-10
                tiers.AddWeightedItem(list, (int)(5 * Multiplier), (int)(10 * Multiplier));  // Very low weight for 9-10

                // Minimal chance for 11-12 (with multiplier)
                list = GetNumberDistributionsForRange(11, 12);  // Range: 11-12
                tiers.AddWeightedItem(list, (int)(3 * Multiplier), (int)(4 * Multiplier));  // Minimal weight for 11-12
            }
            else if (repeats == 1)
            {
                // Moderate chance for 1-2 (with multiplier)
                list = GetNumberDistributionsForRange(1, 2);  // Range: 1-2
                tiers.AddWeightedItem(list, (int)(30 * Multiplier), (int)(40 * Multiplier));  // Moderate weight for 1-2

                // High chance for 3-4 (with multiplier)
                list = GetNumberDistributionsForRange(3, 4);  // Range: 3-4
                tiers.AddWeightedItem(list, (int)(30 * Multiplier), (int)(50 * Multiplier));  // High weight for 3-4

                // Balanced chance for 5-6 (with multiplier)
                list = GetNumberDistributionsForRange(5, 6);  // Range: 5-6
                tiers.AddWeightedItem(list, (int)(20 * Multiplier), (int)(30 * Multiplier));  // Balanced weight for 5-6

                // Lower chance for 7-8 (with multiplier)
                list = GetNumberDistributionsForRange(7, 8);  // Range: 7-8
                tiers.AddWeightedItem(list, (int)(10 * Multiplier), (int)(20 * Multiplier));  // Lower weight for 7-8

                // Very low chance for 9-10 (with multiplier)
                list = GetNumberDistributionsForRange(9, 10);  // Range: 9-10
                tiers.AddWeightedItem(list, (int)(5 * Multiplier), (int)(10 * Multiplier));  // Very low weight for 9-10

                // Minimal chance for 11-12 (with multiplier)
                list = GetNumberDistributionsForRange(11, 12);  // Range: 11-12
                tiers.AddWeightedItem(list, (int)(2 * Multiplier), (int)(3 * Multiplier));  // Minimal weight for 11-12
            }
            else
            {
                // Very high chance for 1-2 (with multiplier)
                list = GetNumberDistributionsForRange(1, 2);  // Range: 1-2
                tiers.AddWeightedItem(list, (int)(50 * Multiplier), (int)(70 * Multiplier));  // Very high weight for 1-2

                // High chance for 3-4 (with multiplier)
                list = GetNumberDistributionsForRange(3, 4);  // Range: 3-4
                tiers.AddWeightedItem(list, (int)(30 * Multiplier), (int)(50 * Multiplier));  // High weight for 3-4

                // Moderate chance for 5-6 (with multiplier)
                list = GetNumberDistributionsForRange(5, 6);  // Range: 5-6
                tiers.AddWeightedItem(list, (int)(10 * Multiplier), (int)(20 * Multiplier));  // Moderate weight for 5-6

                // Lower chance for 7-8 (with multiplier)
                list = GetNumberDistributionsForRange(7, 8);  // Range: 7-8
                tiers.AddWeightedItem(list, (int)(5 * Multiplier), (int)(10 * Multiplier));  // Lower weight for 7-8

                // Very low chance for 9-10 (with multiplier)
                list = GetNumberDistributionsForRange(9, 10);  // Range: 9-10
                tiers.AddWeightedItem(list, (int)(2 * Multiplier), (int)(5 * Multiplier));  // Very low weight for 9-10

                // Minimal chance for 11-12 (with multiplier)
                list = GetNumberDistributionsForRange(11, 12);  // Range: 11-12
                tiers.AddWeightedItem(list, (int)(2 * Multiplier), (int)(3 * Multiplier));  // Minimal weight for 11-12
            }
        });
    }
}