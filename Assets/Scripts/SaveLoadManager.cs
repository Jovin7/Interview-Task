using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.Android;
using UnityEngine.UI;

public class SaveLoadManager : MonoBehaviour
{
    public static SaveLoadManager instance;
    private IGameState gameState;
    public bool isGameCompleted;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        gameState = FindIGameState();
        if (gameState == null)
        {
            Debug.LogError("IGameState implementation not found in the scene!");
        }


    }
    public void Start()
    {
#if UNITY_ANDROID
        if (Permission.HasUserAuthorizedPermission(Permission.ExternalStorageWrite) == false)
        {
            // If not, request it at runtime
            Permission.RequestUserPermission(Permission.ExternalStorageWrite);
        }
#endif
    }
    private IGameState FindIGameState()
    {
        // Find all MonoBehaviour objects in the scene
        MonoBehaviour[] allMonoBehaviours = FindObjectsOfType<MonoBehaviour>();
       
        // Search through them to find one that implements IGameState
        foreach (var monoBehaviour in allMonoBehaviours)
        {
            if (monoBehaviour is IGameState)
            {
                return monoBehaviour as IGameState;
            }
        }

        return null; // Return null if no object implements IGameState
    }
    public bool CheckForSaveFile()
    {
        string path = Application.persistentDataPath + "/savefile.dat";
        if (File.Exists(path))
        {
            return true;
        }
        return false;
    }

    public void DeleteSaveFile()
    {
        string path = Application.persistentDataPath + "/savefile.dat";
        if (File.Exists(path))
        {
            File.Delete(path);
            Debug.Log("Save file deleted on game over.");
        }
    } // Save the game state to a file
    public void SaveGame()
    {
        if (gameState == null)
        {
            Debug.LogError("No IGameState implementation found!");
            return;
        }
        GamePlayManager gamePlayManager = gameState as GamePlayManager;
        if (gamePlayManager.cardMatrixArray == null) return;

        List<CardState> cardStateList = new List<CardState>();
        for (int i = 0; i < gamePlayManager.rowCount; i++)
        {
            for (int j = 0; j < gamePlayManager.columnCount; j++)
            {
                Card card = gamePlayManager.cardMatrixArray[i, j];
                CardState cardState = new CardState(i, j, card.spriteIndex, card.isHidden);
                cardStateList.Add(cardState);
            }
        }

        SaveData saveData = new SaveData(
            gamePlayManager.rowCount,
            gamePlayManager.columnCount,
            gamePlayManager.currentScore,
            gamePlayManager.totalMatchCount,
            gamePlayManager.totalTime,
            gamePlayManager.comboMultiplier,
            cardStateList,
            (int)gamePlayManager.cardGrid.cellSize.x,
            gamePlayManager.cardGrid.constraint,
            gamePlayManager.cardGrid.constraintCount
        );

      

        string path = Application.persistentDataPath + "/savefile.dat";
        BinaryFormatter bf = new BinaryFormatter();
        using (FileStream file = File.Create(path))
        {
            bf.Serialize(file, saveData);
        }

        //Debug.Log("Game Data Saved: " + path);
    }

    // Load the game state from a file
    public void LoadGame()
    {
        if (gameState == null)
        {
            Debug.LogError("No IGameState implementation found!");
            return;
        }

        GamePlayManager gamePlayManager = gameState as GamePlayManager;
        string path = Application.persistentDataPath + "/savefile.dat";

        if (File.Exists(path))
        {
            BinaryFormatter bf = new BinaryFormatter();
            using (FileStream file = File.Open(path, FileMode.Open))
            {
                SaveData loadedData =(SaveData) bf.Deserialize(file);

                // Reset current game state
                gamePlayManager.ResetData();
                gamePlayManager.ResetGameplayUi();

                // Update the game state with loaded data
                gamePlayManager.rowCount = loadedData.rowCount;
                gamePlayManager.columnCount = loadedData.columnCount;
                gamePlayManager.totalCount = gamePlayManager.rowCount * gamePlayManager.columnCount;
                gamePlayManager.currentScore = loadedData.score;
                gamePlayManager.totalMatchCount = loadedData.totalMatchCount;
                gamePlayManager.totalTime = loadedData.totalTime;
                gamePlayManager.comboMultiplier = loadedData.comboMultiplier;

                // Generate the game grid again
                gamePlayManager.cardMatrixArray = new Card[gamePlayManager.rowCount, gamePlayManager.columnCount];
                gamePlayManager.GenerateCards(gamePlayManager.rowCount, gamePlayManager.columnCount, card => { });

                // Load card states
                foreach (var cardState in loadedData.cardStateList)
                {
                    Card card = gamePlayManager.cardMatrixArray[cardState.x, cardState.y];

                    card.spriteIndex = cardState.spriteIndex;
                    card.cardFrontGraphics = gamePlayManager.CardGraphics[cardState.spriteIndex];

                    if (cardState.isHidden)
                    {
                        card.GetComponent<Image>().enabled = false;
                        card.isHidden = true;
                    }
                    else
                    {
                        card.GetComponent<Image>().enabled = true;
                        card.isHidden = false;
                    }
                }

                // Reconfigure grid and UI
                gamePlayManager.ConfigureGrid();
                gamePlayManager.scoreText.text = gamePlayManager.currentScore.ToString();
                gamePlayManager.timerText.text = string.Format("{0:D2}:{1:D2}", Mathf.FloorToInt(gamePlayManager.totalTime / 60), Mathf.FloorToInt(gamePlayManager.totalTime % 60));

                Debug.Log("Game Data Loaded");
            }
        }
        else
        {
            Debug.Log("No save file found.");
        }
    }

    private void OnApplicationQuit()
    {
        if(!isGameCompleted)
            GamePlayManager.instance.SaveGameState();
    }
}
