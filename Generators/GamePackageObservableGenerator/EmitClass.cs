namespace GamePackageObservableGenerator;
internal class EmitClass(ImmutableArray<ResultsModel> results, SourceProductionContext context, EnumObservableType observableType)
{
    private static readonly DiagnosticDescriptor _propertyMustBePartial = new(
        id: "GPO001",
        title: "Observable property must be partial",
        messageFormat: "Observable property '{0}' must be declared partial",
        category: "GamePackageObservableGenerator",
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true);
    public void Emit()
    {
        foreach (var item in results)
        {
            WriteItem(item);
        }
    }
    private void WriteItem(ResultsModel item)
    {
        SourceCodeStringBuilder builder = new();
        string others;




        if (observableType == EnumObservableType.Label)
        {
            others = "LabelObservable";
            builder.StartPartialClass(item, item.GenericString, w =>
            {
                PopulateDetails(w, item);
            });
        }
        else if (observableType == EnumObservableType.Scoreboard)
        {
            others = "ScoreboardObservable";
            builder.StartPartialClass(item, item.GenericString, w =>
            {
                PopulateDetails(w, item);
            });
        }
        else
        {
            others = "StateObservable";
            if (item.InheritedEvent)
            {
                builder.StartPartialClass(item, item.GenericString, w =>
                {
                    PopulateDetails(w, item);
                });
            }
            else
            {
                builder.StartPartialClassImplements(item, item.GenericString, ": global::BasicGameFrameworkLibrary.Core.CommonInterfaces.IStateChanged", w =>
                {
                    PopulateDetails(w, item);
                });
            }
        }
        context.AddSource($"{item.ClassName}.{others}.g.cs", builder.ToString()); //change sample to what you want.
    }
    private string GetCategory(PropertyModel property)
    {
        if (property.VariableCustomCategory == EnumSimpleTypeCategory.CustomEnum ||
            property.VariableCustomCategory == EnumSimpleTypeCategory.StandardEnum)
        {
            return property.UnderlyingSymbolName;
        }
        string firsts;
        if (property.VariableCustomCategory == EnumSimpleTypeCategory.None)
        {
            firsts = property.UnderlyingSymbolName;
        }
        else
        {
            firsts = property.VariableCustomCategory.ToString().ToLower();
        }
        
        if (property.Nullable)
        {
            firsts = $"{firsts}?";
        }

        return firsts;
    }
    private void PopulateDetails(ICodeBlock w, ResultsModel result)
    {
        if (result.InheritedEvent == false)
        {
            if (observableType == EnumObservableType.Label)
            {
                w.WriteLine("public event Action<string>? LabelValueChanged;")
                    .WriteLine("protected void OnLabelValueChanged(string propertyName)")
                    .WriteCodeBlock(w =>
                    {
                        w.WriteLine("LabelValueChanged?.Invoke(propertyName);");
                    });

            }
            else if (observableType == EnumObservableType.Scoreboard)
            {
                w.WriteLine("public event Action<string>? ScoreValueChanged;")
                    .WriteLine("protected void OnScoreValueChanged(string propertyName)")
                    .WriteCodeBlock(w =>
                    {
                        w.WriteLine("ScoreValueChanged?.Invoke(propertyName);");
                    });
            }
            else if (result.AlreadyHadAction)
            {
                w.WriteLine("private void OnStateChanged()")
                   .WriteCodeBlock(w =>
                   {
                       w.WriteLine("StateChanged?.Invoke();");
                   });
            }
            else
            {

                w.WriteLine("public event Action? StateChanged;");
                //for now, will not do the other (this will require rethinking when i get to scoreboards.
                w.WriteLine("protected void OnStateChanged()")
                    .WriteCodeBlock(w =>
                    {
                        w.WriteLine("StateChanged?.Invoke();");
                    });
            }
        }
        
        foreach (var property in result.Properties)
        {
            string category = GetCategory(property);
            if (property.Location is null)
            {
                w.WriteLine($"public partial {category} {property.PropertyName}")
                .WriteCodeBlock(w =>
                {
                    w.WriteLine("get => field;");
                    if (property.PrivateAccesor)
                    {
                        w.WriteLine("private set");
                    }
                    else
                    {
                        w.WriteLine("set");
                    }
                    w.WriteCodeBlock(w =>
                    {
                        w.WriteLine("if (field == value)")
                        .WriteCodeBlock(w =>
                        {
                            w.WriteLine("return;");
                        });
                        w.WriteLine("field = value;");
                        w.WriteLine($"On{property.PropertyName}Changed(value);");
                        if (observableType == EnumObservableType.Label)
                        {
                            w.WriteLine($"OnLabelValueChanged(nameof({property.PropertyName}));");
                        }
                        else if (observableType == EnumObservableType.Scoreboard)
                        {
                            w.WriteLine($"OnScoreValueChanged(nameof({property.PropertyName}));");
                        }
                        else
                        {
                            w.WriteLine("OnStateChanged();");
                        }
                    });
                });
                w.WriteLine($"partial void On{property.PropertyName}Changed({category} value);");
            }
            else
            {
                Diagnostic diagnostic = Diagnostic.Create(
               _propertyMustBePartial,
                property.Location,
                property.PropertyName);
                context.ReportDiagnostic(diagnostic);
            }
        }
    }
}