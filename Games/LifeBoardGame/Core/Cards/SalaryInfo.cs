namespace LifeBoardGame.Core.Cards;
public class SalaryInfo : LifeBaseCard
{
    public SalaryInfo()
    {
        CardCategory = EnumCardCategory.Salary;
    }
    public EnumPayScale WhatGroup { get; set; }
    public decimal PayCheck { get; set; }
    public decimal TaxesDue { get; set; }
}