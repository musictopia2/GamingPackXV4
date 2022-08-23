namespace ScoreBoardGenerator;
internal static class WriterExtensions
{
    private const string _scoreboardNS = "global::BasicGameFrameworkLibrary.ScoreBoardClassesCP";
    public static IWriter PopulateScoreBoardColumn(this IWriter w)
    {
        w.Write(_scoreboardNS)
            .Write(".")
            .Write("ScoreColumnModel")
            .Write(" column"); //i am always using that variable anyways.
        return w;
    }
    public static void PopulateScoreCategory(this ICodeBlock w, string category, Action<ICodeBlock> action)
    {
        w.WriteLine(w =>
        {
            w.Write("if (column.SpecialCategory == ")
            .Write(_scoreboardNS)
            .Write(".EnumScoreSpecialCategory.")
            .Write(category)
            .Write(")");
        })
        .WriteCodeBlock(w =>
        {
            action.Invoke(w);
        });
    }
    public static void PopulateProperty(this ICodeBlock w, IPropertySymbol p, Action<ICodeBlock> action)
    {
        w.WriteLine(w =>
        {
            w.Write("if (column.MainPath == ")
            .AppendDoubleQuote(p.Name)
            .Write(")");
        })
        .WriteCodeBlock(w =>
        {
            action.Invoke(w);
        });
    }
}