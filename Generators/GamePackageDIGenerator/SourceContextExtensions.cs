namespace GamePackageDIGenerator;
internal static class SourceContextExtensions
{
    public static void RaiseDuplicateException(this SourceProductionContext context, string className)
    {
        string information = $"Cannot specify as singleton and instance game.  The class name was {className}";
        context.ReportDiagnostic(Diagnostic.Create(RaiseException(information, "NoVariable"), Location.None));
    }
    public static void RaiseDuplicateInstances(this SourceProductionContext context)
    {
        string information = "You can only have specialized registration helpers for multiplayers, colors, cards, etc with one class";
        context.ReportDiagnostic(Diagnostic.Create(RaiseException(information, "Duplicate instances"), Location.None));
    }
    private static DiagnosticDescriptor RaiseException(string information, string id) => new(id,
        "Could not create helpers",
        information,
        "CustomID",
        DiagnosticSeverity.Error,
        true);
}