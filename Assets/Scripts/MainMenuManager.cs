using DG.Tweening;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuManager : MonoBehaviour
{

    public static MainMenuManager Instance;
    public GamePlayManager gameplayManager;
    public SaveLoadManager saveLoadManager;

    public Transform StartScreen;

    public Transform LevelSelectionScreen;

    public RectTransform rowRect;
    public RectTransform colRect;
    public RectTransform nextRect;
    public Transform GamePanel;
    public Transform GameOverScreen;
    public TextMeshProUGUI finalScore;

    public Transform LoadButton;


    public TMP_Dropdown rowDropDown;
    public TMP_Dropdown columnDropDown;
    Tween currentTween;
    private void Awake()
    {
        Instance = this;
    }
    // Start is called before the first frame update
    void Start()
    {


        StartScreen.transform.GetChild(0).DOScale(1.2f, 0.5f).SetEase(Ease.InOutBack).SetLoops(-1, LoopType.Yoyo);
        StartScreen.transform.GetChild(1).GetComponent<RectTransform>().DOAnchorPos(new Vector2(0, 200), 1f).SetEase(Ease.InBack);

        if (saveLoadManager.CheckForJson())
        {
            Debug.Log("JSON Found");
            LoadButton.gameObject.SetActive(true);
        }
        else
        {
            Debug.Log("JSON not Found");
            LoadButton.gameObject.SetActive(false);
        }
    }

    public void OnStartScreenStartButtonClick()
    {
        LevelSelectionScreen.gameObject.SetActive(true);
        StartScreen.gameObject.SetActive(false);
        InitializeRowOptions();
        LevelSelectionScreen.GetComponent<Image>().DOFade(0.8f, 0.5f).OnComplete(() =>
        {
            LevelSelectionScreen.GetChild(0).GetComponent<RectTransform>().DOAnchorPosY(500, 0.5f).OnComplete(() =>
            {
                rowRect.DOAnchorPosX(0, 0.5f).OnComplete(() =>
                {
                    colRect.DOAnchorPosX(0, 0.5f).OnComplete(() =>
                    {
                        nextRect.DOAnchorPosY(-400, 0.5f).OnComplete(() =>
                        {
                          
                        });
                    });
                });

            });
        });





    }
    private void InitializeRowOptions()
    {
        Debug.LogError("InitializeRowOptions");
        rowDropDown.options.Clear();
        for (int i = 1; i <= gameplayManager.CardGraphics.Count; i++)
        {
            rowDropDown.options.Add(new TMP_Dropdown.OptionData(i.ToString()));
        }
        rowDropDown.value = 0;
        columnDropDown.options.Clear();
        
        columnDropDown.options.Add(new TMP_Dropdown.OptionData("2"));
        columnDropDown.value = 0;
        columnDropDown.interactable = false;
    }

    public void OnRowDropDown()
    {
        Debug.Log("OnRowDropDown");
        gameplayManager.rowCount = rowDropDown.value + 1;
        columnDropDown.options.Clear();
        columnDropDown.interactable = true;
        nextRect.transform.GetComponent<Button>().interactable = true;

        columnDropDown.options.AddRange(Enumerable
                                .Range(1, gameplayManager.CardGraphics.Count)
                                .Where(i => (rowDropDown.value + 1) * i % 2 == 0 && (rowDropDown.value + 1) * i <= gameplayManager.CardGraphics.Count * 2)
                                .Select(i => new TMP_Dropdown.OptionData(i.ToString()))
                                        );
        columnDropDown.value = 0;
        columnDropDown.captionText.text = columnDropDown.options[0].text;
    }
    public void OnColumnDropDown()
    {
        string selectedColumnValue = columnDropDown.options[columnDropDown.value].text;
        Debug.Log(selectedColumnValue);
    }

    public void GameStart()
    {
        nextRect.transform.GetComponent<Button>().interactable = false;

        LevelSelectionScreen.GetChild(0).GetComponent<RectTransform>().DOAnchorPosY(1500, 0.5f).OnComplete(() =>
        {
            rowRect.DOAnchorPosX(-1000, 0.5f).OnComplete(() =>
            {
                colRect.DOAnchorPosX(1000, 0.5f).OnComplete(() =>
                {
                    nextRect.DOAnchorPosY(-1000, 0.5f).OnComplete(() =>
                    {

                        LevelSelectionScreen.GetComponent<Image>().DOFade(0f, 0.5f).OnComplete(() =>
                        {
                            
                            string selectedColumnValue = columnDropDown.options[columnDropDown.value].text;

                            // Assuming you want to use the selected value (convert it to integer if needed)
                            int columnCount = int.Parse(selectedColumnValue);
                            gameplayManager.columnCount = columnCount;


                            LevelSelectionScreen.gameObject.SetActive(false);
                            GamePanel.gameObject.SetActive(true);
                            gameplayManager.enabled = true;
                            SaveLoadManager.instance.isGameCompleted = false;
                        });
                    });
                });
            });
        });

    }

    public void LoadGameButtonClick()
    {
        gameplayManager.isLoaded = true;
        gameplayManager.LoadGameState();
        GamePanel.gameObject.SetActive(true);
        gameplayManager.enabled = true;
        StartScreen.gameObject.SetActive(false);
    }

    public void GameOver()
    {

        GamePanel.gameObject.SetActive(false);

        GameOverScreen.gameObject.SetActive(true);


        GameOverScreen.GetComponent<Image>().DOFade(0.8f, 0.5f);
        currentTween = GameOverScreen.GetChild(0).DOScale(1.2f, 0.5f).SetEase(Ease.Linear).SetLoops(-1, LoopType.Yoyo);
        finalScore.text = gameplayManager.scoreText.text;
        SaveLoadManager.instance.DeleteJson();
        SaveLoadManager.instance.isGameCompleted = true;

    }

    public void GameOverMenuButtonClick()
    {
        currentTween.Kill();
        GameOverScreen.GetComponent<Image>().DOFade(0f, 0.5f);
        StartScreen.gameObject.SetActive(true);
        GameOverScreen.gameObject.SetActive(false);
        gameplayManager.enabled = false;
        gameplayManager.ResetGameplayUi();
        gameplayManager.ResetData();
        if (saveLoadManager.CheckForJson())
            LoadButton.gameObject.SetActive(true);
        else
            LoadButton.gameObject.SetActive(false);


    }
}
