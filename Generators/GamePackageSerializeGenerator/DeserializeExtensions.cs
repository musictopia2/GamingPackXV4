using System.Xml.Linq;

namespace GamePackageSerializeGenerator;
internal static class DeserializeExtensions
{
    public static void DeserializeChar(this ICodeBlock w, TypeModel model, bool property)
    {
        if (model.SpecialCategory == EnumSpecialCategory.Ignore || model.TypeCategory != EnumTypeCategory.Char)
        {
            return;
        }
        if (property)
        {
            w.WriteLine("string temps = element.GetProperty(property).GetString()!;");
        }
        else
        {
            w.WriteLine("string temps = element.GetString()!;");
        }
        w.WriteLine("char output = char.Parse(temps);")
        .PopulateReturnOutput();
    }
    public static void DeserializePointF(this ICodeBlock w, TypeModel model, bool property)
    {
        if (model.SpecialCategory == EnumSpecialCategory.Ignore)
        {
            return;
        }
        if (model.TypeCategory != EnumTypeCategory.PointF)
        {
            return;
        }
        if (property)
        {
            w.WriteLine("string firsts = element.GetProperty(property).GetString()!;")
            .WriteLine(w =>
            {
                w.Write("var list = firsts.Split(")
                .AppendDoubleQuote(" ")
                .Write(");");
            });
        }
        else
        {
            w.WriteLine(w =>
            {
                w.Write("var list = element.GetString()!.Split(")
                .AppendDoubleQuote(" ")
                .Write(");");
            });
        }
        w.WriteLine("var x = float.Parse(list[0]);")
        .WriteLine("var y = float.Parse(list[1]);")
        .WriteLine("return new(x, y);");
    }
    public static void DeserializeCustomEnum(this ICodeBlock w, TypeModel model, bool property)
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
            w.WriteLine("string value = element.GetProperty(property).GetString()!;");
        }
        else
        {
            w.WriteLine("string value = element.GetString()!;");
        }
        w.WriteLine(w =>
        {
            w.Write("return ")
            .PopulateFullClassName(model)
            .Write(".FromName(value);");
        });
    }
    public static void DeserializeStandardEnum(this ICodeBlock w, TypeModel model, bool property)
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
        if (property)
        {
            w.WriteLine("string value = element.GetProperty(property).GetString()!;");
        }
        else
        {
            w.WriteLine("string value = element.GetString()!;");
        }
        foreach (var item in model.EnumNames)
        {
            w.PopulateStandardValue(model, item);
        }
        w.PopulateReturnStandardEnumValue(model, model.EnumNames.First());
    }
    private static void PopulateStandardValue(this ICodeBlock w, TypeModel model, string customValue)
    {
        w.WriteLine(w =>
        {
            w.Write("if (value == ")
            .AppendDoubleQuote(customValue)
            .Write(")");
        }).WriteCodeBlock(w =>
        {
            w.PopulateReturnStandardEnumValue(model, customValue);
        });
    }
    private static void PopulateReturnStandardEnumValue(this ICodeBlock w, TypeModel model, string customValue)
    {
        w.WriteLine(w =>
        {
            w.Write("return ")
            .PopulateFullClassName(model)
            .Write(".")
            .Write(customValue)
            .Write(";");
        });
    }
    private static void PrivateStringStyle(this ICodeBlock w, bool property)
    {
        if (property)
        {
            w.WriteLine("string output = element.GetProperty(property).GetString()!;");
        }
        else
        {
            w.WriteLine("string output = element.GetString()!;");
        }
        w.PopulateReturnOutput();
    }
    public static void DeserializeBool(this ICodeBlock w, TypeModel model, bool property)
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
            w.WriteLine("bool output = element.GetProperty(property).GetBoolean();");
        }
        else
        {
            w.WriteLine("bool output = element.GetBoolean();");
        }
        w.PopulateReturnOutput();
    }
    public static void DeserializeString(this ICodeBlock w, TypeModel model, bool property)
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
    public static void DeserializeVector(this ICodeBlock w, TypeModel model, bool property)
    {
        if (model.SpecialCategory == EnumSpecialCategory.Ignore)
        {
            return;
        }
        if (model.TypeCategory != EnumTypeCategory.Vector)
        {
            return;
        }
        if (property)
        {
            w.WriteLine("string firsts = element.GetProperty(property).GetString()!;")
            .WriteLine(w =>
            {
                w.Write("var list = firsts.Split(")
                .AppendDoubleQuote(" ")
                .Write(");");
            });
        }
        else
        {
            w.WriteLine(w =>
            {
                w.Write("var list = element.GetString()!.Split(")
                .AppendDoubleQuote(" ")
                .Write(");");
            });
        }
        w.WriteLine("var row = int.Parse(list[0]);")
        .WriteLine("var column = int.Parse(list[1]);")
        .WriteLine("return new(row, column);");
    }
    public static void DeserializeDecimal(this ICodeBlock w, TypeModel model, bool property)
    {
        if (model.SpecialCategory == EnumSpecialCategory.Ignore || model.TypeCategory != EnumTypeCategory.Decimal)
        {
            return;
        }
        if (property)
        {
            w.WriteLine("decimal output = element.GetProperty(property).GetDecimal();");
        }
        else
        {
            w.WriteLine("decimal output = element.GetDecimal();");
        }
        w.PopulateReturnOutput();
    }
    public static void DeserializeNullableInt(this ICodeBlock w, TypeModel model, bool property)
    {
        if (model.SpecialCategory == EnumSpecialCategory.Ignore || model.TypeCategory != EnumTypeCategory.NullableInt)
        {
            return;
        }
        if (property)
{
            w.WriteLine("var temps = element.GetProperty(property);");
        }
        w.PopulateReturnNull(property);
        if (property)
        {
            w.WriteLine("int output = element.GetProperty(property).GetInt32();");
        }
        else
        {
            w.WriteLine("int output = element.GetInt32();");
        }
        w.PopulateReturnOutput();
    }
    public static void DeserializeInt(this ICodeBlock w, TypeModel model, bool property)
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
            w.WriteLine("int output = element.GetProperty(property).GetInt32();");
        }
        else
        {
            w.WriteLine("int output = element.GetInt32();");
        }
        w.PopulateReturnOutput();
    }
    public static void DeserializeCustomList(this ICodeBlock w, TypeModel model, bool property)
    {
        if (model.SpecialCategory == EnumSpecialCategory.Ignore)
        {
            return;
        }
        if (model.LoopCategory != EnumLoopCategory.Custom)
        {
            return;
        }
        w.WriteLine(w =>
        {
            w.BasicListWrite()
            .CustomGenericWrite(w =>
            {
                if (model.SubSymbol is not null && model.CollectionNameSpace != "")
                {
                    w.SymbolFullNameWrite((INamedTypeSymbol)model.SymbolUsed!);
                    w.Write("<")
                    .GlobalWrite()
                    .Write(model.SubSymbol.ContainingNamespace.ToDisplayString())
                    .Write(".")
                    .Write(model.SubSymbol.Name)
                    .Write(">");
                }
                else if (model.SubSymbol is not null)
                {
                    w.SymbolFullNameWrite(model.SubSymbol);
                }
                else
                {
                    w.SymbolFullNameWrite((INamedTypeSymbol)model.SymbolUsed!);
                }
            })
            .Write(" temp = new();");
        });
        if (property)
        {
            w.WriteLine("global::System.Text.Json.JsonElement array;")
            .WriteLine("array = element.GetProperty(property);")
            .WriteLine("foreach (var item in array.EnumerateArray())")
            .WriteCodeBlock(w =>
            {
                w.PopulateCustomList(model);
            });
        }
        else
        {
            w.WriteLine("foreach (var item in element.EnumerateArray())")
            .WriteCodeBlock(w =>
            {
                w.PopulateCustomList(model);
            });
        }
        w.WriteLine(w =>
        {
            w.PopulateFullClassName(model)
            .Write(" output = new(temp);");
        })
        .PopulateReturnOutput();
    }
    private static void PopulateCustomList(this ICodeBlock w, TypeModel model)
    {
        w.WriteLine(w =>
        {
            w.Write("temp.Add(")
            .Write(model.SubName)
            .Write("DeserializeHandler(item)!);");
        });
    }
    public static void DeserializeDoubleList(this ICodeBlock w, TypeModel model, bool property)
    {
        if (model.SpecialCategory == EnumSpecialCategory.Ignore)
        {
            return;
        }
        if (model.LoopCategory != EnumLoopCategory.Standard)
        {
            return;
        }
        if (model.TypeCategory != EnumTypeCategory.DoubleList)
        {
            return;
        }
        w.PopulateStartOutput(model); //well see if you do the same way (?)
        if (property)
        {
            w.WriteLine("global::System.Text.Json.JsonElement array;")
            .WriteLine("array = element.GetProperty(property);")
            .WriteLine("foreach (var item in array.EnumerateArray())")
            .WriteCodeBlock(w =>
            {
                w.PopulateListOutput(model);
            });
        }
        else
        {
            w.WriteLine("foreach (var item in element.EnumerateArray())")
            .WriteCodeBlock(w =>
            {
                w.PopulateListOutput(model);
            });
        }
        w.PopulateReturnOutput();
    }
    public static void DeserializeDictionary(this ICodeBlock w, TypeModel model, bool property)
    {
        if (model.SpecialCategory == EnumSpecialCategory.Ignore)
        {
            return;
        }
        if (model.TypeCategory != EnumTypeCategory.Dictionary)
        {
            return;
        }
        w.PopulateStartOutput(model);
        if (property)
        {
            w.WriteLine("global::System.Text.Json.JsonElement array;")
            .WriteLine("array = element.GetProperty(property);")
            .WriteLine("foreach (var item in array.EnumerateArray())")
            .WriteCodeBlock(w =>
            {
                w.PopulateDictionaryOutput(model);
            });
        }
        else
        {
            w.WriteLine("foreach (var item in element.EnumerateArray())")
            .WriteCodeBlock(w =>
            {
                w.PopulateDictionaryOutput(model);
            });
        }
        w.PopulateReturnOutput();
    }
    private static void PopulateDictionaryOutput(this ICodeBlock w,TypeModel  model)
    {
        var temps = (INamedTypeSymbol)model.SymbolUsed!;
        var pairs = temps.GetDictionarySymbols();
        w.WriteLine(w =>
        {
            w.Write("var firsts = ")
            .Write(pairs.Key.Name)
            .Write("DeserializeHandler(item, ")
            .AppendDoubleQuote("key")
            .Write(");");
        })
        .WriteLine(w =>
        {
            w.Write("var seconds = ")
            .Write(pairs.Value.Name)
            .Write("DeserializeHandler(item, ")
            .AppendDoubleQuote("value")
            .Write(");");
        })
        .WriteLine("output.Add(firsts, seconds);");
    }
    public static void DeserializeSimpleList(this ICodeBlock w, TypeModel model, bool property)
    {
        if (model.SpecialCategory == EnumSpecialCategory.Ignore)
        {
            return;
        }
        if (model.LoopCategory != EnumLoopCategory.Standard)
        {
            return;
        }
        if (model.TypeCategory != EnumTypeCategory.SingleList)
        {
            return;
        }
        //this is for simple lists.
        w.PopulateStartOutput(model);
        if (property)
        {
            w.WriteLine("global::System.Text.Json.JsonElement array;")
            .WriteLine("array = element.GetProperty(property);")
            .WriteLine("foreach (var item in array.EnumerateArray())")
            .WriteCodeBlock(w =>
            {
                w.PopulateListOutput(model);
            });
        }
        else
        {
            w.WriteLine("foreach (var item in element.EnumerateArray())")
            .WriteCodeBlock(w =>
            {
                w.PopulateListOutput(model);
            });
        }
        w.PopulateReturnOutput();
    }
    private static void PopulateListOutput(this ICodeBlock w, TypeModel model)
    {
        w.WriteLine(w =>
        {
            w.Write("output.Add(")
            .Write(model.SubName)
            .Write("DeserializeHandler(item)!);");
        });
    }
    public static void DeserializeComplex(this ICodeBlock w, TypeModel model, BasicList<IPropertySymbol> ignores, bool property)
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
        if (property)
        {
            w.PopulateDeserializeObjectStart(model);
        }
        else
        {
            w.PopulateStartOutput(model);
            if (model.SpecialCategory != EnumSpecialCategory.Save)
            {
                if (model.NullablePossible)
                {
                    w.PopulateReturnNull(false);
                }
                else
                {
                    w.PopulateConditionalOutput(false);
                }
            }
        }
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
        string variableName;
        if (property)
        {
            variableName = "temps";
        }
        else
        {
            variableName = "element";
        }
        foreach (var p in properties)
        {
            if (p.PropertyIgnored(ignores))
            {
                continue;
            }
            string subs = p.GetSubName();
            w.PopulateDeserializeLine(subs, variableName, p);
        }
        w.PopulateReturnOutput();
    }
    private static void PopulateDeserializeLine(this ICodeBlock w, string subName, string variableName, IPropertySymbol p)
    {
        w.WriteLine(w =>
        {
            w.Write("output.")
            .Write(p.Name)
            .Write(" = ")
            .Write(subName)
            .Write("DeserializeHandler(")
            .Write(variableName)
            .Write(", ")
            .AppendDoubleQuote(p.Name)
            .Write(")!;");
        });
    }
}