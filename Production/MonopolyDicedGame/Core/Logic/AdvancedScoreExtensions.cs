namespace MonopolyDicedGame.Core.Logic;
public static class AdvancedScoreExtensions
{
    extension (MonopolyDicedGameSaveInfo saveRoot)
    {
        public int GetTotalScoreInRound()
        {
            saveRoot.HasAtLeastOnePropertyMonopoly = false;
            if (saveRoot.NumberOfCops == 3)
            {
                return 0; //because you already have 3 cops.
            }
            int highestUsed = 0;
            int output = 0;
            int proposed;
            8.Times(x =>
            {
                if (saveRoot.HasMonopoly(x))
                {
                    output += x.MonopolyValue;
                    saveRoot.HasAtLeastOnePropertyMonopoly = true;
                }
                else
                {
                    proposed = saveRoot.GetPropertyIncompleteScore(x);
                    if (proposed > highestUsed)
                    {
                        highestUsed = proposed;
                    }
                }
            });
            EnumBasicType other = EnumBasicType.Railroad;
            if (saveRoot.HasTrainCompleteSet)
            {
                output += other.MonopolyValue;
            }
            else
            {
                proposed = saveRoot.GetTrainIncompleteScore;
                if (proposed > highestUsed)
                {
                    highestUsed = proposed;
                }
            }
            other = EnumBasicType.Utility;
            if (saveRoot.HasUtilityCompleteSet)
            {
                output += other.MonopolyValue;
            }
            else
            {
                proposed = saveRoot.GetUtilityIncompleteScore;
                if (proposed > highestUsed)
                {
                    highestUsed = proposed;
                }
            }
            output += highestUsed;
            int goValues = saveRoot.TotalGos * 200;
            output += goValues;
            if (saveRoot.HasHotel)
            {
                output += 9000;
            }
            else
            {
                int totalHouses = saveRoot.NumberOfHouses * 1000; //i am guessing that a hotel plus 4 houses is equal to 9000 extra points.
                output += totalHouses;
            }
            return output;
        }
        public int GetPropertyIncompleteScore(int group)
        {
            return saveRoot.Owns.Where(x => x.Group == group).Sum(x => x.IncompleteValue);
        }
        public bool HasMonopoly(int group)
        {
            int needed;
            if (group == 1 || group == 8)
            {
                needed = 2;
            }
            else
            {
                needed = 3;
            }
            int count = saveRoot.Owns.Count(x => x.Group == group);
            if (count > needed)
            {
                throw new CustomBasicException("Too many properties was used for monopoly");
            }
            return count == needed;
        }
        public int GetTrainIncompleteScore => saveRoot.Owns.Where(x => x.UsedOn == EnumBasicType.Railroad).Sum(x => x.IncompleteValue);
        public int GetUtilityIncompleteScore => saveRoot.Owns.Where(x => x.UsedOn == EnumBasicType.Utility).Sum(x => x.IncompleteValue);
        public bool HasTrainCompleteSet
        {
            get
            {
                int count = saveRoot.Owns.Count(x => x.UsedOn == EnumBasicType.Railroad);
                if (count > 4)
                {
                    throw new CustomBasicException("Cannot have more than 4 trains");
                }
                return count == 4;
            }
            
        }
        public bool HasUtilityCompleteSet
        {
            get
            {
                int count = saveRoot.Owns.Count(x => x.UsedOn == EnumBasicType.Utility);
                if (count > 2)
                {
                    throw new CustomBasicException("Cannot have more than 2 utilities");
                }
                return count == 2;
            }
        }
    }
    
}