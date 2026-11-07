using js1 = CommonBasicLibraries.AdvancedGeneralFunctionsAndProcesses.JsonSerializers.SystemTextJsonStrings;
namespace GamePackageContractsLibrary;
public class SentMessage
{
    public string Status { get; set; } = "";
    public string Body { get; set; } = "";

    public int SequenceNumber { get; set; }
    public string NetworkSessionId { get; set; } = "";
    public override string ToString()
    {
        return js1.SerializeObject(this);
    }
}