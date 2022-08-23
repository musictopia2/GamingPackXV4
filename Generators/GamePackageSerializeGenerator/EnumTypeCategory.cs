using System;
using System.Collections.Generic;
using System.Text;

namespace GamePackageSerializeGenerator;

internal enum EnumTypeCategory
{
    //will risk doing all as one.  because with the new way of doing things, i think this could work (?)
    None,
    CustomEnum,
    StandardEnum,
    Int,
    String,
    Bool,
    Decimal,
    Char,
    Vector,
    PointF,
    SizeF,
    Complex,
    SingleList,
    DoubleList,
    Dictionary,
    NullableInt
}