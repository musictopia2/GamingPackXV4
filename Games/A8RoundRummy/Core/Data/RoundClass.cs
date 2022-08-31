﻿namespace A8RoundRummy.Core.Data;
public class RoundClass
{
    public string Description { get; set; } = "";
    public EnumCategory Category { get; set; }
    public int Points { get; set; }
    public EnumRummyType Rummy { get; set; }
}