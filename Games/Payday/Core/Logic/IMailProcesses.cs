namespace Payday.Core.Logic;
public interface IMailProcesses
{
    Task ProcessMailAsync();
    void PopulateMails();
    void SetUpMail();
    Task ReshuffleMailAsync(DeckRegularDict<MailCard> list);
}