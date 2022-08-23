namespace GamePackageSerializeGenerator;
internal static class SerializeExtensions
{
    public static void SerializeChar(this ICodeBlock w, TypeModel model, bool property)
    {
        if (model.SpecialCategory == EnumSpecialCategory.Ignore || model.TypeCategory != EnumTypeCategory.Char)
        {
            return;
        }
        if (property)
        {
            w.WriteLine("writer.WriteString(property, value.ToString());");
        }
        else
        {
            w.WriteLine("writer.WriteStringValue(value.ToString());");
        }
    }
    public static void SerializePointF(this ICodeBlock w, TypeModel model, bool property)
    {
        if (model.SpecialCategory == EnumSpecialCategory.Ignore)
        {
            return;
        }
        if (model.TypeCategory != EnumTypeCategory.PointF)
        {
            return;
        }
        w.WriteLine(w =>
        {
            w.Write("string text = $")
            .AppendDoubleQuote("{value.X} {value.Y}")
            .Write(";");
        });
        if (property)
        {
            w.WriteLine("writer.WriteString(property, text);");
        }
        else
        {
            w.WriteLine("writer.WriteStringValue(text);");
        }
    }
    public static void SerializeCustomEnum(this ICodeBlock w, TypeModel model, bool property)
    {
        if (model.SpecialCategory == EnumSpecialCategory.Ignore)
        {
            return;
        }
        if (model.TypeCategory != EnumTypeCategory.CustomEnum)
        {
            return;
        }
        if (property)
        {
            w.WriteLine("writer.WriteString(property, value.Name);");
        }
        else
        {
            w.WriteLine("writer.WriteStringValue(value.Name);");
        }
    }
    public static void SerializeStandardEnum(this ICodeBlock w, TypeModel model, bool property)
    {
        if (model.SpecialCategory == EnumSpecialCategory.Ignore)
        {
            return;
        }
        if (model.TypeCategory != EnumTypeCategory.StandardEnum)
        {
            return;
        }
        if (model.EnumNames.Count == 0)
        {
            return; //you need at least one to even bother.
        }
        foreach (var item in model.EnumNames)
        {
            w.PopulateStandardValue(model, item, property);
        }
    }
    private static void PopulateStandardValue(this ICodeBlock w, TypeModel model, string customValue, bool property)
    {

        w.WriteLine(w =>
        {
            w.Write("if (value == ")
            .PopulateFullClassName(model)
            .Write(".")
            .Write(customValue)
            .Write(")");
        })
        .WriteCodeBlock(w =>
        {
            if (property)
            {
                w.WriteLine(w =>
                {
                    w.Write("writer.WriteString(property, ")
                    .AppendDoubleQuote(customValue)
                    .Write(");");
                });
            }
            else
            {
                w.WriteLine(w =>
                {
                    w.Write("writer.WriteStringValue(")
                    .AppendDoubleQuote(customValue)
                    .Write(");");
                });
            }
        });
    }
    private static void PrivateStringStyle(this ICodeBlock w, bool property)
    {
        if (property)
        {
            w.WriteLine("writer.WriteString(property, value);");
        }
        else
        {
            w.WriteLine("writer.WriteStringValue(value);");
        }
    }
    public static void SerializeBool(this ICodeBlock w, TypeModel model, bool property)
    {
        if (model.SpecialCategory == EnumSpecialCategory.Ignore)
        {
            return;
        }
        if (model.TypeCategory != EnumTypeCategory.Bool)
        {
            return;
        }
        if (property)
        {
            w.WriteLine("writer.WriteBoolean(property, value);");
        }
        else
        {
            w.WriteLine("writer.WriteBooleanValue(value);");
        }
    }
    public static void SerializeString(this ICodeBlock w, TypeModel model, bool property)
    {
        if (model.SpecialCategory == EnumSpecialCategory.Ignore)
        {
            return;
        }
        if (model.TypeCategory != EnumTypeCategory.String)
        {
            return;
        }
        w.PrivateStringStyle(property);
    }
    public static void SerializeVector(this ICodeBlock w, TypeModel model, bool property)
    {
        if (model.SpecialCategory == EnumSpecialCategory.Ignore)
        {
            return;
        }
        if (model.TypeCategory != EnumTypeCategory.Vector)
        {
            return;
        }
        w.WriteLine(w =>
        {
            w.Write("string text = $")
            .AppendDoubleQuote("{value.Row} {value.Column}")
            .Write(";");
        });
        if (property)
        {
            w.WriteLine("writer.WriteString(property, text);");
        }
        else
        {
            w.WriteLine("writer.WriteStringValue(text);");
        }
    }
    private static bool CanPopulateNull(this TypeModel model)
    {
        if (model.SpecialCategory == EnumSpecialCategory.Save)
        {
            return false;
        }
        if (model.SymbolUsed!.IsValueType)
        {
            return false;
        }
        return true;
    }
    public static void SerializeComplex(this ICodeBlock w, TypeModel model, BasicList<IPropertySymbol> ignores, bool property)
    {
        if (model.SpecialCategory == EnumSpecialCategory.Ignore)
        {
            return;
        }
        if (model.TypeCategory != EnumTypeCategory.Complex)
        {
            return;
        }
        if (model.SpecialCategory == EnumSpecialCategory.Save && property)
        {
            return;
        }
        if (model.CanPopulateNull())
        {
            w.PopulateWriteNull(property);
        }
        w.PopulateStartObject(property);
        var properties = model.SymbolUsed!.GetAllPublicProperties();
        properties.RemoveAllOnly(xx =>
        {
            if (xx.SetMethod is null)
            {
                return true;
            }
            if (xx.SetMethod.DeclaredAccessibility == Accessibility.Private)
            {
                return true; //to remove.
            }
            return xx.IsReadOnly ||
            xx.CanBeReferencedByName == false ||
            xx.Name == "Assembly";
        });
        foreach (var p in properties)
        {
            if (p.PropertyIgnored(ignores))
            {
                continue;
            }
            string subs = p.GetSubName();
            w.WriteLine(w =>
            {
                w.Write(subs)
                .Write("SerializeHandler(writer, ")
                .AppendDoubleQuote(p.Name)
                .Write(", value.")
                .Write(p.Name)
                .Write("!);");
            });
        }
        w.PopulateEndObject();
    }
    public static void SerializeDecimal(this ICodeBlock w, TypeModel model, bool property)
    {
        if (model.SpecialCategory == EnumSpecialCategory.Ignore || model.TypeCategory != EnumTypeCategory.Decimal)
        {
            return;
        }
        if (property)
        {
            w.WriteLine("writer.WriteNumber(property, value);");
        }
        else
        {
            w.WriteLine("writer.WriteNumberValue(value);");
        }
    }
    public static void SerializeNullableInt(this ICodeBlock w, TypeModel model, bool property)
    {
        if (model.SpecialCategory == EnumSpecialCategory.Ignore || model.TypeCategory != EnumTypeCategory.NullableInt)
        {
            return;
        }
        w.PopulateWriteNull(property);
        if (property)
        {
            w.WriteLine("writer.WriteNumber(property, value.Value);");
        }
        else
        {
            w.WriteLine("writer.WriteNumberValue(value.Value);");
        }
    }
    public static void SerializeInt(this ICodeBlock w, TypeModel model, bool property)
    {
        if (model.SpecialCategory == EnumSpecialCategory.Ignore)
        {
            return;
        }
        if (model.TypeCategory != EnumTypeCategory.Int)
        {
            return;
        }
        if (property)
        {
            w.WriteLine("writer.WriteNumber(property, value);");
        }
        else
        {
            w.WriteLine("writer.WriteNumberValue(value);");
        }
    }
    public static void SerializeDoubleList(this ICodeBlock w, TypeModel model, bool property)
    {
        if (model.SpecialCategory == EnumSpecialCategory.Ignore)
        {
            return;
        }
        if (model.TypeCategory != EnumTypeCategory.DoubleList)
        {
            return;
        }
        if (model.LoopCategory != EnumLoopCategory.Standard)
        {
            return;
        }
        w.PopulateStartArray(property)
        .WriteLine("for (int i = 0; i < value.Count; i++)")
        .WriteCodeBlock(w =>
        {
            w.WriteLine(w =>
            {
                w.Write(model.SubName)
                .Write("SerializeHandler(writer, value[i]);");
            });
        })
        .PopulateEndArray();
    }
    public static void SerializeDictionary(this ICodeBlock w, TypeModel model, bool property)
    {
        if (model.SpecialCategory == EnumSpecialCategory.Ignore)
        {
            return;
        }
        if (model.TypeCategory != EnumTypeCategory.Dictionary)
        {
            return;
        }
        var temps = (INamedTypeSymbol)model.SymbolUsed!;
        var pairs = temps.GetDictionarySymbols();
        w.PopulateStartArray(property)
        .WriteLine("foreach (var item in value)")
        .WriteCodeBlock(w =>
        {
            w.WriteLine("writer.WriteStartObject();");
            //i don't think dictionaries will deal with lists.  hopefully can just do names alone (?)
            w.WriteLine(w =>
            {
                w.Write(pairs.Key.Name)
                .Write("SerializeHandler(writer, ")
                .AppendDoubleQuote("key")
                .Write(", item.Key);");
            })
            .WriteLine(w =>
            {
                w.Write(pairs.Value.Name)
                .Write("SerializeHandler(writer, ")
                .AppendDoubleQuote("value")
                .Write(", item.Value);");
            })
            .WriteLine("writer.WriteEndObject();");
        })
        .PopulateEndArray();
    }
    public static void SerializeSimpleList(this ICodeBlock w, TypeModel model, bool property)
    {
        if (model.SpecialCategory == EnumSpecialCategory.Ignore)
        {
            return;
        }
        if (model.TypeCategory != EnumTypeCategory.SingleList)
        {
            return;
        }
        //this is for simple lists.
        if (model.LoopCategory != EnumLoopCategory.Standard)
        {
            return;
        }
        w.PopulateStartArray(property)
        .WriteLine("for (int i = 0; i < value.Count; i++)")
        .WriteCodeBlock(w =>
        {
            w.WriteLine(w =>
            {
                w.Write(model.SubName)
                .Write("SerializeHandler(writer, value[i]);");
            });
        })
        .PopulateEndArray();
    }
    public static void SerializeCustomList(this ICodeBlock w, TypeModel model, bool property)
    {
        if (model.SpecialCategory == EnumSpecialCategory.Ignore)
        {
            return;
        }
        if (model.LoopCategory != EnumLoopCategory.Custom)
        {
            return;
        }
        w.PopulateStartArray(property)
        .WriteLine("foreach (var item in value)")
        .WriteCodeBlock(w =>
        {
            w.WriteLine(w =>
            {
                w.Write(model.SubName)
                .Write("SerializeHandler(writer, item);");
            });
        })
        .PopulateEndArray();
    }
}