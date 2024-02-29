﻿namespace MonopolyDicedGame.Core.Logic;
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
}
