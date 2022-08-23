namespace CommandsGenerator;
internal static class SourceContextExtensions
{
    public static void RaiseNoMultipleImplementsCommand(this SourceProductionContext context, string className)
    {
        string information = $"You cannot inherit from Observable Simple Control and implement IPlainObservable.  The class name was {className}";
        context.ReportDiagnostic(Diagnostic.Create(RaiseException(information, "SingleCommand"), Location.None));
    }
    public static void RaiseNeedsSingleCommand(this SourceProductionContext context, string className)
    {
        string information = $"Needs a single command because it inherits from Observable Simple Control.  The class name was {className}";
        context.ReportDiagnostic(Diagnostic.Create(RaiseException(information, "SingleCommand"), Location.None));
    }
    public static void RaiseNeedsSingleMethod(this SourceProductionContext context, string className)
    {
        string information = $"Needs a single method because it inherits from Observable Simple Control.  The class name was {className}";
        context.ReportDiagnostic(Diagnostic.Create(RaiseException(information, "SingleCommand"), Location.None));
    }
    //public static void RaiseWrongNameType(this SourceProductionContext context, string className, string methodName)
    //{
    //    string information = $"You cannot specify a name for commands because only classes that inherit from SimpleControlObservable can do that.  The class name was {className} and the method name was {methodName}";
    //    context.ReportDiagnostic(Diagnostic.Create(RaiseException(information, "NoVariable"), Location.None));
    //}
    public static void RaiseNeedsPlain(this SourceProductionContext context, string className, string methodName)
    {
        string information = $"Needs to be plain command because you are implementing IPlainObservable.  The class name was {className} and the method name was {methodName}";
        context.ReportDiagnostic(Diagnostic.Create(RaiseException(information, "NoVariable"), Location.None));
    }
    public static void RaiseWrongReturnType(this SourceProductionContext context, string className, string methodName)
    {
        string information = $"Wrong return type for method command.  Must be void or task.  The class name was {className} and the method name was {methodName}";
        context.ReportDiagnostic(Diagnostic.Create(RaiseException(information, "NoVariable"), Location.None));
    }
    public static void RaiseTooManyParameters(this SourceProductionContext context, string className, string methodName)
    {
        string information = $"Has too many parameters.  Only one parameter is supported at the most.  The class name was {className} and the method name was {methodName}";
        context.ReportDiagnostic(Diagnostic.Create(RaiseException(information, "NoVariable"), Location.None));
    }
    public static void RaiseInvalidCast(this SourceProductionContext context, string className, string methodName)
    {
        string information = $"Invalid cast.  The parameter for CanExecute method and main method type must match.  The class was {className} and the method name was {methodName}";
        context.ReportDiagnostic(Diagnostic.Create(RaiseException(information, "NoVariable"), Location.None));
    }
    //public static void RaiseMismatchParameters(this SourceProductionContext context, string className, string methodName)
    //{
    //    string information = $"Has mismatch.  If the method has one parameter, then function has to have one parameter.  If method has 0 parameters, then function needs 0 parameters.  The class name was {className} and the method name was {methodName}";
    //    context.ReportDiagnostic(Diagnostic.Create(RaiseException(information, "NoVariable"), Location.None));
    //}
    public static void RaiseNotImplemented(this SourceProductionContext context, string className, string methodName, string commandString)
    {
        string information = $"The command with type {commandString} is not supported currently.  The class was {className} and the method name was {methodName}";
        context.ReportDiagnostic(Diagnostic.Create(RaiseException(information, "NoVariable"), Location.None));
    }
    public static void RaiseNoPartialClassException(this SourceProductionContext context, string className)
    {
        string information = $"Needs to have partial class for class name was {className}";
        context.ReportDiagnostic(Diagnostic.Create(RaiseException(information, "NoClass"), Location.None));
    }
    public static void RaiseNoCreateCommandsRegularException(this SourceProductionContext context, string className)
    {
        string information = $"Needs to have partial CreateCommands.  Try to use private partial void CreateCommands();  The class name was {className}";
        context.ReportDiagnostic(Diagnostic.Create(RaiseException(information, "SubscribeMethod"), Location.None));
    }
    public static void RaiseNoCreateCommandsContainerException(this SourceProductionContext context, string className)
    {
        string information = $"Needs to have partial CreateCommands.  Try to use private partial void CreateCommands(CommandContainer container); The class name was {className}";
        context.ReportDiagnostic(Diagnostic.Create(RaiseException(information, "UnsubscribeMethod"), Location.None));
    }
    private static DiagnosticDescriptor RaiseException(string information, string id) => new(id,
        "Could not create helpers",
        information,
        "CustomID",
        DiagnosticSeverity.Error,
        true);

}