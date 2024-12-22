using System.Collections.Generic;
using UnityEngine.UI;

[System.Serializable]
public class SaveData
{
    public int rowCount;
    public int columnCount;
    public int score;
    public int totalMatchCount;
    public float totalTime;
    public int comboMultiplier;
    public List<CardState> cardStateList;  
    public int cardCellSize;
    public GridLayoutGroup.Constraint gridConstraint;
    public int gridConstraintCount;

    
    public SaveData(int rowCount,
                    int columnCount,
                    int score,
                    int totalMatchCount,
                    float totalTime,
                    int comboMultiplier,
                    List<CardState> cardStateList,
                    int cardCellSize,
                    GridLayoutGroup.Constraint gridConstraint,
                    int gridConstraintCount)
    {
        this.rowCount = rowCount;
        this.columnCount = columnCount;
        this.score = score;
        this.totalMatchCount = totalMatchCount;
        this.totalTime = totalTime;
        this.comboMultiplier = comboMultiplier;
        this.cardStateList = cardStateList;
        
        this.cardCellSize = cardCellSize;
        this.gridConstraint = gridConstraint;
        this.gridConstraintCount = gridConstraintCount;
    }
}

[System.Serializable]
public class CardState
{
    public int x;  // Row index
    public int y;  // Column index
    public int spriteIndex;  // Index of the sprite assigned to this card
    public bool isHidden;  // Whether the card is hidden or not

    // Constructor to define the state of the card
    public CardState(int x, int y, int spriteIndex, bool isHidden)
    {
        this.x = x;
        this.y = y;
        this.spriteIndex = spriteIndex;
        this.isHidden = isHidden;
    }
}
