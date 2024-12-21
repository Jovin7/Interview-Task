using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class GamePlayManager : MonoBehaviour
{
    public static GamePlayManager instance;
    public GameObject cardPrefab;
    public GridLayoutGroup cardGrid;
    public RectTransform cardGridRect;

    private float totalTime = 120f;
    public TextMeshProUGUI timerText;
    public TextMeshProUGUI scoreText;
    private int standardScore = 10;
    private int comboMultiplier = 1;
    private int currentScore = 0;

    public List<Sprite> CardGraphics = new List<Sprite>();
    public Card previouslySelectedCard;
    public Card currentlySelectedCard;
    public int rowCount = 2;
    public int columnCount = 2;
    public int totalCount;
    public int totalMatchCount;

    private Card[,] cardMatrixArray;
    public List<CardData> cardDatas;

    private void Awake()
    {
        instance = this;
    }
    // Start is called before the first frame update
    void Start()
    {
        totalCount = rowCount * columnCount;
        totalMatchCount = totalCount;

        SetupGame();
    }
    private void Update()
    {
        if (totalTime > 0)
        {

            totalTime -= Time.deltaTime;
            int minutes = Mathf.FloorToInt(totalTime / 60);
            int seconds = Mathf.FloorToInt(totalTime % 60);

            // Format the time as "00:00"
            string timeFormatted = string.Format("{0:D2}:{1:D2}", minutes, seconds);
            timerText.text = timeFormatted.ToString();
        }
        else
        {
            MainMenuManager.Instance.GameOver();
        }
    }

    private void SetupGame()
    {
        cardMatrixArray = new Card[rowCount, columnCount];
        InitializeCardIndexes();
        ShuffleCardIndexes();
        AssignCardSprites();
        ConfigureGrid();
    }
    private void InitializeCardIndexes()
    {
        cardDatas = new List<CardData>(totalCount);
        GenerateCards(rowCount, columnCount, card =>
        {
            cardDatas.Add(new CardData(card.cardDatas.item1, card.cardDatas.item2));
            card.isVisible = true;

        });
    }
    private void GenerateCards(int rowCount, int columnCount, Action<Card> fetchingCardData)
    {
        for (int i = 0; i < rowCount; i++)
        {
            for (int j = 0; j < columnCount; j++)
            {
                GameObject newCardObject = Instantiate(cardPrefab, Vector3.zero, Quaternion.identity, cardGrid.transform);

                Card newCard = newCardObject.GetComponent<Card>();
                cardMatrixArray[i, j] = newCard;
                newCard.cardDatas = new CardData(i, j);
                fetchingCardData?.Invoke(newCard);
            }
        }
    }
    private void ShuffleCardIndexes()
    {
        UnityEngine.Random.InitState((int)System.DateTime.Now.Ticks);
        for (int i = 0; i < cardDatas.Count / 2; i++)
        {
            int randomIndex01 = UnityEngine.Random.Range(0, cardDatas.Count);
            int randomIndex02 = (randomIndex01 + UnityEngine.Random.Range(1, 100) * 53) % cardDatas.Count;

            Swap(randomIndex01, randomIndex02);
        }
    }
    private void Swap(int index1, int index2)
    {
        var temp = cardDatas[index1];
        cardDatas[index1] = cardDatas[index2];
        cardDatas[index2] = temp;
    }
    private void AssignCardSprites()
    {
        int spriteIndex = 0;
        for (int i = 0; i < cardDatas.Count; i += 2)
        {
            cardMatrixArray[cardDatas[i].item1, cardDatas[i].item2].spriteIndex = spriteIndex;
            cardMatrixArray[cardDatas[i].item1, cardDatas[i].item2].cardFrontGraphics = CardGraphics[spriteIndex];
            cardMatrixArray[cardDatas[i + 1].item1, cardDatas[i + 1].item2].spriteIndex = spriteIndex;
            cardMatrixArray[cardDatas[i + 1].item1, cardDatas[i + 1].item2].cardFrontGraphics = CardGraphics[spriteIndex];
            spriteIndex++;
        }
    }
    private void ConfigureGrid()
    {
        // Calculate available space after padding and spacing
        float availableWidth = cardGridRect.rect.width - cardGrid.padding.horizontal - (columnCount - 1) * cardGrid.spacing.x;
        float availableHeight = cardGridRect.rect.height - cardGrid.padding.vertical - (rowCount - 1) * cardGrid.spacing.y;

        // Determine the size for each card
        float cardWidth = availableWidth / columnCount;
        float cardHeight = availableHeight / rowCount;

        // Set cell size and constraints based on which dimension is more limiting
        float cellSize = Mathf.Min(cardWidth, cardHeight);  // Use the smaller dimension
        cardGrid.cellSize = new Vector2(cellSize, cellSize);

        if (cardWidth < cardHeight)
        {
            cardGrid.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
            cardGrid.constraintCount = columnCount;
        }
        else
        {
            cardGrid.constraint = GridLayoutGroup.Constraint.FixedRowCount;
            cardGrid.constraintCount = rowCount;
        }
    }



    public void OnCardSelect(Card cardDetail)
    {
        if (previouslySelectedCard != null)
        {
            currentlySelectedCard = cardDetail;
            if (currentlySelectedCard.spriteIndex == previouslySelectedCard.spriteIndex)
            {
                previouslySelectedCard.GetComponent<Image>().enabled = false;
                currentlySelectedCard.GetComponent<Image>().enabled = false;
                totalMatchCount -= 2;
                currentScore += (standardScore * comboMultiplier);
                scoreText.text = currentScore.ToString();
                comboMultiplier += 1;
                if (totalMatchCount > 0)
                {
                    Debug.Log("continue GamePlay");
                }
                else
                {
                    Debug.Log("GameOver Screen ");
                    MainMenuManager.Instance.GameOver();
                }
            }
            else
            {
                comboMultiplier = 1;
                currentlySelectedCard.CardFlipBack();
                previouslySelectedCard.CardFlipBack();
            }
            currentlySelectedCard = null;
            previouslySelectedCard = null;
        }
        else
        {
            previouslySelectedCard = cardDetail;
        }
    }


}

[Serializable]
public class CardData
{
    public int item1;
    public int item2;

    public CardData(int _item1, int _item2)
    {
        item1 = _item1;
        item2 = _item2;
    }
}
