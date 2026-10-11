namespace Froggies.Core.Data;
[UseStateChanged]
public partial class LilyPadModel(int x, int y, bool pHasFrog)
{
    public int Row { get; set; } = y;
    public int Column { get; set; } = x;
    [StateProperty]
    public partial bool HasFrog { get; set; } = pHasFrog;
    [StateProperty]
    public partial bool IsSelected { get; set; }
    [StateProperty]
    public partial bool IsTarget { get; set; }
    [StateProperty]
    public partial bool StartedWithFrog { get; set; }
}