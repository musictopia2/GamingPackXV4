namespace GamePackageSerializeGenerator;
internal class EmitClass
{
    private readonly SourceProductionContext _context;
    private readonly Dictionary<string, ResultsModel> _results;
    private readonly Compilation _compilation;
    private readonly BasicList<IPropertySymbol> _ignoreProperties;
    public EmitClass(SourceProductionContext context, CompleteInformation info, Compilation compilation)
    {
        _context = context;
        _results = info.Results;
        _ignoreProperties = info.PropertiesToIgnore;
        _compilation = compilation;
    }
    public void Emit()
    {
        foreach (var result in _results.Values)
        {
            ProcessG(result);
            ProcessTypes(result);
        }
        ProcessRegistration();
    }
    private void ProcessTypes(ResultsModel result)
    {
        foreach (var item in result.Types)
        {
            ProcessType(result, item);
        }
    }
    private void ProcessType(ResultsModel result, TypeModel model)
    {
        if (model.SpecialCategory == EnumSpecialCategory.Ignore)
        {
            return; //this should take care of the first issue.
        }
        SourceCodeStringBuilder builder = new();
        builder.WriteContext(_compilation, result, w =>
        {
            if (model.SpecialCategory == EnumSpecialCategory.Save)
            {
                ProcessSaveType(w, result, model);
                return;
            }
            if (model.SpecialCategory == EnumSpecialCategory.Main)
            {
                ProcessMainType(w, result, model);
                return;
            }
            ProcessOtherType(w, result, model);
        });
        _context.AddSource($"{result.ClassName}Context.{model.FileName}.g", builder.ToString());
    }
    private void ProcessMainType(ICodeBlock w, ResultsModel result, TypeModel model)
    {
        w.PopulateSerialize(result, model, false, w =>
        {
            Serialize(w, model, false);
        });
        w.PopulateDeserialize(result, model, false, w =>
        {
            Deserialize(w, model, false);
        }); //iffy for now.
    }
    private void ProcessOtherType(ICodeBlock w, ResultsModel result, TypeModel model)
    {
        w.PopulateSerialize(result, model, false, w =>
        {
            Serialize(w, model, false);
        });
        w.PopulateDeserialize(result, model, false, w =>
        {
            Deserialize(w, model, false);
        });
        w.PopulateSerialize(result, model, true, w =>
        {
            Serialize(w, model, true);
        });
        w.PopulateDeserialize(result, model, true, w =>
        {
            Deserialize(w, model, true);
        });
    }
    private void Serialize(ICodeBlock w, TypeModel model, bool property)
    {
        w.SerializeInt(model, property);
        w.SerializeSimpleList(model, property);
        w.SerializeComplex(model, _ignoreProperties, property);
        w.SerializeVector(model, property);
        w.SerializeString(model, property);
        w.SerializeCustomList(model, property);
        w.SerializeBool(model, property);
        w.SerializeStandardEnum(model, property);
        w.SerializeCustomEnum(model, property);
        w.SerializePointF(model, property);
        w.SerializeDoubleList(model, property);
        w.SerializeDictionary(model, property);
        w.SerializeChar(model, property);
        w.SerializeDecimal(model, property);
        w.SerializeNullableInt(model, property);
    }
    private void Deserialize(ICodeBlock w, TypeModel model, bool property)
    {
        w.DeserializeInt(model, property);
        w.DeserializeSimpleList(model, property);
        w.DeserializeComplex(model, _ignoreProperties, property);
        w.DeserializeVector(model, property);
        w.DeserializeString(model, property);
        w.DeserializeCustomList(model, property);
        w.DeserializeBool(model, property);
        w.DeserializeStandardEnum(model, property);
        w.DeserializeCustomEnum(model, property);
        w.DeserializePointF(model, property);
        w.DeserializeDoubleList(model, property);
        w.DeserializeDictionary(model, property);
        w.DeserializeChar(model, property);
        w.DeserializeDecimal(model, property);
        w.DeserializeNullableInt(model, property);
    }
    private void ProcessSaveType(ICodeBlock w, ResultsModel result, TypeModel model)
    {
        w.PopulateSerialize(result, model, false, w =>
        {
            if (result.HasChildren == false)
            {
                w.PopulateStartObject(false);
                w.PopulateEndObject();
                return;
            }
            w.SerializeComplex(model, _ignoreProperties, false);
        });
        w.PopulateDeserialize(result, model, false, w =>
        {
            if (result.HasChildren == false)
            {
                w.PopulateStartOutput(model)
                .PopulateReturnOutput();
                return;
            }
            w.DeserializeComplex(model, _ignoreProperties, false);
        });
    }
    private void ProcessRegistration()
    {
        SourceCodeStringBuilder builder = new();
        builder.StartGlobalProcesses(_compilation, "AutoResumeContexts", "GlobalRegistrations", w =>
        {
            foreach (var result in _results.Values)
            {
                string camel = result.ContextName.ChangeCasingForVariable(EnumVariableCategory.PrivateFieldParameter);
                w.WriteLine(w =>
                {
                    w.Write("private static ")
                    .Write(result.ContextName)
                    .Write(" ")
                    .Write(camel)
                    .Write(" = new();");
                });
            }
            w.WriteLine("public static void Register()")
            .WriteCodeBlock(w =>
            {
                foreach (var result in _results.Values)
                {
                    string camel = result.ContextName.ChangeCasingForVariable(EnumVariableCategory.PrivateFieldParameter);
                    w.WriteLine(w =>
                    {
                        w.Write("CommonBasicLibraries.AdvancedGeneralFunctionsAndProcesses.JsonSerializers.CustomSerializeHelpers<")
                        .Write(result.GlobalName)
                        .Write(">.MasterContext = ")
                        .Write(camel)
                        .Write(";");
                    });
                }
            });
        });
        _context.AddSource("GlobalRegistrations.g", builder.ToString());
    }
    private void ProcessG(ResultsModel result)
    {
        SourceCodeStringBuilder builder = new();
        builder.WriteContext(_compilation, result, w =>
        {
            SerializeProcess(w, result);
            DeserializeProcess(w, result);
        });
        _context.AddSource($"{result.ContextName}.g", builder.ToString());
    }
    private void SerializeProcess(ICodeBlock w, ResultsModel result)
    {
        w.WriteLine(w =>
        {
            w.Write("string ")
            .PopulateInterface(result)
            .Write(".Serialize(")
            .Write(result.GlobalName)
            .Write(" obj)");
        })
        .WriteCodeBlock(w =>
        {
            w.WriteLine("using var ms = new global::System.IO.MemoryStream();")
            .WriteLine("using var writer = new global::System.Text.Json.Utf8JsonWriter(ms, new global::System.Text.Json.JsonWriterOptions()")
            .WriteLambaBlock(w =>
            {
                w.WriteLine("Indented = true");
            })
            .WriteLine(w =>
             {
                 w.Write(result.ClassName)
                 .Write("SerializeHandler(writer");
                 if (result.HasChildren == false)
                 {
                     w.Write(");");
                 }
                 else
                 {
                     w.Write(", obj);");
                 }
             })
            .WriteLine("writer.Flush();")
            .WriteLine("string jsonString = global::System.Text.Encoding.UTF8.GetString(ms.ToArray());")
            .WriteLine("ms.Close();")
            .WriteLine("return jsonString;");
        });
    }
    private void DeserializeProcess(ICodeBlock w, ResultsModel result)
    {
        w.WriteLine(w =>
        {
            w.Write(result.GlobalName)
            .Write(" ")
            .PopulateInterface(result)
            .Write(".Deserialize(string json)");
        })
        .WriteCodeBlock(w =>
        {
            w.WriteLine("using var doc = global::System.Text.Json.JsonDocument.Parse(json);")
            .WriteLine("var element = doc.RootElement;")
            .WriteLine(w =>
            {
                w.Write(result.GlobalName)
                .Write(" output = ")
                .Write(result.ClassName)
                .Write("DeserializeHandler(");
                if (result.HasChildren == false)
                {
                    w.Write(");");
                }
                else
                {
                    w.Write("element);");
                }
            })
            .WriteLine("return output;");
        });
    }
}