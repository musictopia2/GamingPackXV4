namespace BasicGameFrameworkLibrary.Core.NetworkingClasses.Data;
public class SentMessage
{
    public string Status { get; set; } = "";
    public string Body { get; set; } = "";
    public override string ToString()
    {
        return js.SerializeObject(this);
    }
}