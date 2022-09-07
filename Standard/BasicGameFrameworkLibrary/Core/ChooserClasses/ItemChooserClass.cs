﻿namespace BasicGameFrameworkLibrary.Core.ChooserClasses;
public class ItemChooserClass<O>
     where O : ISimpleValueObject<int>
{
    private readonly IRandomGenerator _rs;
    public BasicList<O>? ValueList;
    public int ItemToChoose(bool requiredToChoose = true, bool useHalf = true)
    {
        if (requiredToChoose == false)
        {
            int ask1; //decided to not use weighted average after all.
            if (useHalf == true)
            {
                ask1 = _rs.GetRandomNumber(2);
                if (ask1 == 1)
                {
                    return -1;
                }
            }
            else
            {
                ask1 = _rs.GetRandomNumber(ValueList!.Count + 1);
                if (ask1 == ValueList.Count + 1)
                {
                    return -1; //0 based.
                }
            }
        }
        return ValueList!.GetRandomItem().ReadMainValue;
    }
    public ItemChooserClass(IGamePackageResolver resolver)
    {
        _rs = resolver.Resolve<IRandomGenerator>();
    }
}