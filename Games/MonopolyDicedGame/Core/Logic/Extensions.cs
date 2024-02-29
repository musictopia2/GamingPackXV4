namespace MonopolyDicedGame.Core.Logic;
public static class Extensions
{
    public static int GetMonopolyValue(this int group)
    {
        if (group == 1)
        {
            return 600;
        }
        if (group == 2)
        {
            return 1000;
        }
        if (group == 3)
        {
            return 1500;
        }
        if (group == 4)
        {
            return 1800;
        }
        if (group == 5)
        {
            return 2200;
        }
        if (group == 6)
        {
            return 2700;
        }
        if (group == 7)
        {
            return 3000;
        }
        if (group == 8)
        {
            return 3500;
        }
        throw new CustomBasicException("Unable to get the property value");
    }

    public static int GetMonopolyValue(this EnumBasicType whatCard)
    {
        if (whatCard == EnumBasicType.Railroad)
        {
            return 2500;
        }
        if (whatCard == EnumBasicType.Utility)
        {
            return 800;
        }
        throw new CustomBasicException("Only Railroads and utilities are supported in this mode");
    }

    public static string GetColor(this int group)
    {
        if (group <= 0)
        {
            throw new CustomBasicException("No need for color because no group found");
        }
        if (group == 1)
        {
            return cc1.Brown.ToWebColor();
        }
        if (group == 2)
        {
            return cc1.Cyan.ToWebColor();
        }
        if (group == 3)
        {
            return cc1.MediumVioletRed.ToWebColor();
        }
        if (group == 4)
        {
            return cc1.DarkOrange.ToWebColor();
        }
        if (group == 5)
        {
            return cc1.Red.ToWebColor();
        }
        if (group == 6)
        {
            return cc1.Yellow.ToWebColor();
        }
        if (group == 7)
        {
            return cc1.Green.ToWebColor();
        }
        if (group == 8)
        {
            return cc1.DarkBlue.ToWebColor();
        }
        throw new CustomBasicException("Only 1 to 8 are supported");
    }
    private static int NewCopsFound(this MonopolyDicedGameSaveInfo saveRoot, IRandomGenerator randoms)
    {
        //if the roll number is 1, the maximum would be 2.
        //int ask1;
        //int ask2;
        //int ask3;
        bool ask;
        int chances;

        ask = randoms.NextBool(10);
        if (ask)
        {
            return 0; //10 percent chances of no cops no matter what.
        }
        if (saveRoot.RollNumber > 10)
        {
            ask = randoms.NextBool(50);
            if (ask)
            {
                return 3;
            }
            return 0; //after roll 10 make it all or nothing for the rest.
        }
        if (saveRoot.RollNumber > 3)
        {
            ask = randoms.NextBool(10);
            if (ask)
            {
                return 2; //if you passed 3 rolls, 10 percent chances will have 2 cops
            }   
        }
        if (saveRoot.RollNumber == 1)
        {
            chances = 15;
        }
        else if (saveRoot.NumberOfCops == 2)
        {
            chances = 40;
        }
        else if (saveRoot.RollNumber == 2)
        {
            chances = 20;
        }
        else if (saveRoot.RollNumber == 3)
        {
            chances = 25;
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
        if (saveRoot.RollNumber == 1)
        {
            chances = 5;
        }
        else if (saveRoot.NumberOfCops == 1)
        {
            chances = 10;
        }
        else if (saveRoot.RollNumber == 2)
        {
            chances = 15;
        }
        else
        {
            chances = 25;
        }
        ask = randoms.NextBool(chances);
        if (ask == false)
        {
            return 2;
        }
        if (saveRoot.RollNumber == 1 || saveRoot.NumberOfCops == 2)
        {
            return 2;
        }
        if (saveRoot.RollNumber == 2)
        {
            chances = 5;
        }
        else if (saveRoot.RollNumber == 3)
        {
            chances = 10;
        }
        else if (saveRoot.RollNumber > 8)
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
            ask1 = randoms.NextBool(20);
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
