namespace LottoDominos.Core.Data;
[InstanceGame]
[UseStateChanged]
public partial class LottoDominosScreenState
{
    [StateProperty]
    public partial bool MainPlay { get; set; }
}