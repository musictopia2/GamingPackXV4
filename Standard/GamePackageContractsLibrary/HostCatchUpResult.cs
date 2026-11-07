namespace GamePackageContractsLibrary;
public class HostCatchUpResult
{
    public bool SessionMatches { get; set; }
    public string NetworkSessionId { get; set; } = "";
    public int LatestSequenceNumber { get; set; }
    public List<SentMessage> Messages { get; set; } = [];
}