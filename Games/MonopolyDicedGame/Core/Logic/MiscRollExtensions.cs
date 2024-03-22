namespace MonopolyDicedGame.Core.Logic;
internal static class MiscRollExtensions
{
    private static int NewCopsFound(this MonopolyDicedGameSaveInfo saveRoot, IRandomGenerator randoms)
    {
        //if the roll number is 1, the maximum would be 2.
        //int ask1;
        //int ask2;
        //int ask3;
        bool ask;
        int chances;

        //ask = randoms.NextBool(90);
        //if (ask)
        //{
        //    return 3; //for now until i figure out the new bug.
        //}
        int rollNumber = saveRoot.RollNumber - 1;
        ask = randoms.NextBool(10);
        if (ask)
        {
            return 0; //10 percent chances of no cops no matter what.
        }
        if (rollNumber > 10)
        {
            ask = randoms.NextBool(20);
            if (ask == true)
            {
                return 3; //will reduce as needed anyways.
            }
            ask = randoms.NextBool(10);
            if (ask == true)
            {
                return 2;
            }
            ask = randoms.NextBool(40);
            if (ask)
            {
                return 1;
            }
            return 0;
        }
        if (rollNumber > 3)
        {
            ask = randoms.NextBool(10);
            if (ask)
            {
                return 2; //if you passed 3 rolls, 10 percent chances will have 2 cops
            }
        }
        if (rollNumber == 1)
        {
            chances = 10;
        }
        else if (saveRoot.NumberOfCops == 2)
        {
            chances = 30;
        }
        else if (rollNumber == 2)
        {
            chances = 15;
        }
        else if (rollNumber == 3)
        {
            chances = 20;
        }
        else if (rollNumber == 4)
        {
            chances = 40;
        }
        else
        {
            chances = 50;
        }
        ask = randoms.NextBool(chances);
        if (ask == false)
        {
            return 0;
        }
        if (saveRoot.NumberOfCops == 2)
        {
            return 1;
        }
        if (rollNumber == 1)
        {
            chances = 3;
        }
        else if (saveRoot.NumberOfCops == 1)
        {
            chances = 10;
        }
        else if (rollNumber == 2)
        {
            chances = 15;
        }
        else
        {
            chances = 25;
        }
        ask = randoms.NextBool(chances);
        if (ask == false || saveRoot.NumberOfCops == 2)
        {
            return 1;
        }
        if (rollNumber == 1)
        {
            return 2;
        }
        if (rollNumber == 2)
        {
            chances = 5;
        }
        else if (rollNumber == 3)
        {
            chances = 10;
        }
        else if (rollNumber > 6)
        {
            chances = 90;
        }
        else
        {
            chances = 20;
        }
        ask = randoms.NextBool(chances);
        if (ask == false)
        {
            return 2;
        }
        return 3;
    }
    public static BasicList<EnumMiscType> GetMiscResults(this MonopolyDicedGameSaveInfo saveRoot, IRandomGenerator randoms)
    {
        BasicList<EnumMiscType> output = [];
        if (saveRoot.NumberOfCops > 3)
        {
            throw new CustomBasicException("Can never have more than 3 cops");
        }
        if (saveRoot.NumberOfCops == 3)
        {
            return [];
        }

        int cops = saveRoot.NewCopsFound(randoms);
        if (cops > 3)
        {
            cops = 3;
            //throw new CustomBasicException("Too many cops");
        }
        if (cops + saveRoot.NumberOfCops > 3)
        {
            cops = 3 - saveRoot.NumberOfCops;
            //throw new CustomBasicException("Returned too many cops");
        }
        //cops come first.

        cops.Times(x =>
        {
            output.Add(EnumMiscType.Police);
        });
        if (cops + saveRoot.NumberOfCops == 3)
        {
            return output;
        }
        bool ask1;
        if (saveRoot.NumberOfCops > 0)
        {
            ask1 = randoms.NextBool(10);
            if (ask1)
            {
                output.Add(EnumMiscType.Free);
            }
        }
        if (output.Count + saveRoot.NumberOfCops == 3)
        {
            return output;
        }
        ask1 = randoms.NextBool(5);
        if (ask1)
        {
            output.Add(EnumMiscType.Go);
        }
        return output;



    }
}
