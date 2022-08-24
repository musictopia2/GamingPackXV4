namespace BasicGameFrameworkLibrary.Blazor.Extensions;
public static class LabelGridHelpers
{
    public static BasicList<LabelGridModel> AddLabel(this BasicList<LabelGridModel> labels, string header, string value)
    {
        labels.Add(new LabelGridModel(header, value));
        return labels;
    }
}