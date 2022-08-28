namespace Risk.Core.Data;
public interface IMultiplayerModel
{
    void SendAttackData();
    void AttackArmies();
    void SelectTerritory();
    void ToNextStep();
    void MoveArmies();
    void ReturnRiskCards();
    void PlaceArmies();
}