namespace DealCardGame.Core.Logic;
public static class Extensions
{
    public static bool CanUseHouseOrHotel(this EnumColor color)
    {
        if (color == EnumColor.Black || color == EnumColor.Lime)
        {
            return false;
        }
        return true;
    }
    public static int RentForGroup(this EnumColor color, int howManyInSet)
    {
        //this will not consider house or hotel.
        if (color == EnumColor.Brown)
        {
            if (howManyInSet == 1)
            {
                return 1;
            }
            if (howManyInSet == 2)
            {
                return 2;
            }
            throw new CustomBasicException("Failed to calculate rent for brown");
        }
        if (color == EnumColor.Cyan)
        {
            if (howManyInSet == 1)
            {
                return 1;
            }
            if (howManyInSet == 2)
            {
                return 2;
            }
            if (howManyInSet == 3)
            {
                return 3;
            }
            throw new CustomBasicException("Failed to calculate rent for cyan");
        }
        if (color == EnumColor.MediumVioletRed)
        {
            if (howManyInSet == 1)
            {
                return 1;
            }
            if (howManyInSet == 2)
            {
                return 2;
            }
            if (howManyInSet == 3)
            {
                return 4;
            }
            throw new CustomBasicException($"Failed to calculate rent for {color}");
        }
        if (color == EnumColor.DarkOrange)
        {
            if (howManyInSet == 1)
            {
                return 1;
            }
            if (howManyInSet == 2)
            {
                return 3;
            }
            if (howManyInSet == 3)
            {
                return 5;
            }
            throw new CustomBasicException($"Failed to calculate rent for {color}");
        }
        if (color == EnumColor.Red)
        {
            if (howManyInSet == 1)
            {
                return 2;
            }
            if (howManyInSet == 2)
            {
                return 3;
            }
            if (howManyInSet == 3)
            {
                return 6;
            }
            throw new CustomBasicException($"Failed to calculate rent for {color}");
        }
        if (color == EnumColor.Yellow)
        {
            if (howManyInSet == 1)
            {
                return 2;
            }
            if (howManyInSet == 2)
            {
                return 4;
            }
            if (howManyInSet == 3)
            {
                return 6;
            }
            throw new CustomBasicException($"Failed to calculate rent for {color}");
        }
        if (color == EnumColor.Green)
        {
            if (howManyInSet == 1)
            {
                return 2;
            }
            if (howManyInSet == 2)
            {
                return 4;
            }
            if (howManyInSet == 3)
            {
                return 7;
            }
            throw new CustomBasicException($"Failed to calculate rent for {color}");
        }
        if (color == EnumColor.DarkBlue)
        {
            if (howManyInSet == 1)
            {
                return 3;
            }
            if (howManyInSet == 2)
            {
                return 8;
            }
            throw new CustomBasicException($"Failed to calculate rent for {color}");
        }
        if (color == EnumColor.Lime)
        {
            if (howManyInSet == 1)
            {
                return 1;
            }
            if (howManyInSet == 2)
            {
                return 2;
            }
            throw new CustomBasicException($"Failed to calculate rent for {color}");
        }
        if (color == EnumColor.Black)
        {
            if (howManyInSet == 1)
            {
                return 1;
            }
            if (howManyInSet == 2)
            {
                return 2;
            }
            if (howManyInSet == 3)
            {
                return 3;
            }
            if (howManyInSet == 4)
            {
                return 4;
            }
            throw new CustomBasicException($"Failed to calculate rent for {color}");
        }
        throw new CustomBasicException("Unable to calculate rent");
    }
}