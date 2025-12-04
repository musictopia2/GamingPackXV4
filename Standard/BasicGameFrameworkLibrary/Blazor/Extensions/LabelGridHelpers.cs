namespace BasicGameFrameworkLibrary.Blazor.Extensions;
public static class LabelGridHelpers
{
    extension (BasicList<LabelGridModel> labels)
    {
        public BasicList<LabelGridModel> AddLabel(string header, string value)
        {
            labels.Add(new LabelGridModel(header, value));
            return labels;
        }
    }
}