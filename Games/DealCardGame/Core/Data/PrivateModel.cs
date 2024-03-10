namespace DealCardGame.Core.Data;
public class PrivateModel : IMappable
{
    public DeckRegularDict<DealCardGameCardInformation> Payments { get; set; } = [];
    public bool NeedsPayment { get; set; }
    public StartPaymentPlayerState State { get; set; } = new();
    public RentModel RentInfo { get; set; } = new();
}


/*

the processes i envision is as follows:
has to show otherturn.
to show they have to pay rent.

if there is only one player, then easy.
if there is more than one player, then each take their turn paying.

if the player does not have enough but something, will take what they have (show the cards used).

however, whoever receives the payment needs to review and confirm to continue.


everybody gets to see the cards the host will receive though.


for birthday (easiest one).

steps:
1.  plays card
2.  the next player has something but not enough.
3.  will automatically show the cards used (the host will receive)

or
they don't have any:
just skips it period.


or:
steps:
1.  plays card
2.  for the other player, automatically opens another screen.

they cannot close that screen until its all paid for.

the information on that screen will be:

1.  their money pile
2.  all the cards in their properties pile
all the ones they currently have.

the button would say make payment.

hint:
if they need to break up a set, they have to first use the houses and hotels for payments.

only one at a time is allowed for the properties but can do more than one for bank.

the player receiving (the houses and hotels will go into the bank).

can have option to start over again.

this means that private autoresume will need a list of properties and banked items.

so if starting over, then will reset everything.






 */