namespace Fluxx.Core.Logic;
public interface IGiveTaxationProcesses
{
    Task GiveCardsForTaxationAsync(IDeckDict<FluxxCardInformation> list);
}