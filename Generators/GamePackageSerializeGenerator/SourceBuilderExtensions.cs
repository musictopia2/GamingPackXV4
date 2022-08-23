namespace GamePackageSerializeGenerator;
internal static class SourceBuilderExtensions
{
    public static void WriteContext(this SourceCodeStringBuilder builder, Compilation compilation, ResultsModel result, Action<ICodeBlock> action)
    {
        builder.WriteLine("#nullable enable")
        .WriteLine(w =>
        {
            w.Write("namespace ")
            .Write(compilation.Assembly.Name)
            .Write(".AutoResumeContexts;");
        })
        .WriteLine(w =>
        {
            w.Write("internal partial class ")
            .Write(result.ContextName)
            .Write(" : ")
            .PopulateInterface(result);
        })
        .WriteCodeBlock(w =>
        {
            action.Invoke(w);
        });
    }
}