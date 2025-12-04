namespace BasicGameFrameworkLibrary.Core.SpecializedGameTypes.YahtzeeStyleHelpers.Logic;
public static class UIExtensions
{
    extension (RowInfo row)
    {
        public string RenderPointsObtained()
        {
            if (row.PointsObtained == null)
            {
                return "";
            }
            return row.PointsObtained.Value.ToString();
        }
        public string RenderPointsPossible()
        {
            if (row.Possible == null)
            {
                return "";
            }
            return row.Possible.Value.ToString();
        }
    }
    
}