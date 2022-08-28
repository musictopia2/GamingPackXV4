﻿namespace DiceDominos.Core.Logic;
[SingletonGame]
[AutoReset]
public class DiceDominosComputerAI
{
    private readonly GameBoardCP _gameBoard;
    private readonly DiceDominosGameContainer _gameContainer;
    public DiceDominosComputerAI(GameBoardCP gameBoard, DiceDominosGameContainer gameContainer)
    {
        _gameBoard = gameBoard;
        _gameContainer = gameContainer;
    }
    private static int HowMany(int whatNumber, DeckRegularDict<SimpleDominoInfo> possibleList)
    {
        int nums = default;
        foreach (var thisDomino in possibleList)
        {
            if (thisDomino.FirstNum == whatNumber || thisDomino.SecondNum == whatNumber || (whatNumber == 6 && thisDomino.FirstNum == 0) || (whatNumber == 6 && thisDomino.SecondNum == 0))
            {
                nums += 1;
            }
        }
        return nums;
    }
    public int Move()
    {
        DeckRegularDict<SimpleDominoInfo> newList = new();
        var possibleList = _gameBoard.GetVisibleList();
        if (possibleList.Count == 0)
        {
            throw new CustomBasicException("There cannot be 0 visible items or the game would have been over.");
        }
        foreach (var thisDomino in possibleList)
        {
            if (_gameBoard.IsValidMove(thisDomino.Deck) == true)
            {
                newList.Add(thisDomino);
            }
        }
        if (newList.Count == 0)
        {
            return 0;
        }
        return newList.GetRandomItem().Deck;
    }
    public int DominoToHold()
    {
        int firstNumber;
        int secondNumber;
        var possibleList = _gameBoard.GetVisibleList();
        firstNumber = HowMany(_gameBoard.DiceValue(1), possibleList);
        secondNumber = HowMany(_gameBoard.DiceValue(2), possibleList);
        if (firstNumber > secondNumber)
        {
            return 1;
        }
        if (secondNumber > firstNumber)
        {
            return 2;
        }
        return _gameContainer.Random.GetRandomNumber(2);
    }
}