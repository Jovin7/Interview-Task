using System.Linq;
using TMPro;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
public class MainMenuManager : MonoBehaviour
{

    public static MainMenuManager Instance;
    public GamePlayManager gameplayManager;

    public Transform StartScreen;
 //   public Transform LevelSelectionScreen;
    public Transform LevelSelectionScreen;
    public Transform GamePanel;
    public Transform GameOverScreen;
    public TextMeshProUGUI finalScore;

    
   

    public TMP_Dropdown rowDropDown;
    public TMP_Dropdown columnDropDown;

    private void Awake()
    {
        Instance = this;
    }
    // Start is called before the first frame update
    void Start()
    {
       StartScreen.transform.GetChild(0).DOScale(1.2f, 0.5f).SetEase(Ease.InOutBack).SetLoops(-1, LoopType.Yoyo);
       StartScreen.transform.GetChild(1).GetComponent<RectTransform>().DOAnchorPos(new Vector2(0, 200), 1f).SetEase(Ease.InBack);
    }
   

   

    public void OnStartScreenStartButtonClick()
    {
        
        LevelSelectionScreen.gameObject.SetActive(true);
        StartScreen.gameObject.SetActive(false);
        InitializeRowOptions();

    }
    private void InitializeRowOptions()
    {
        rowDropDown.options.Clear();
        for (int i = 1; i <= gameplayManager.CardGraphics.Count; i++)
        {
            rowDropDown.options.Add(new TMP_Dropdown.OptionData(i.ToString()));
        }
        
        columnDropDown.options.Clear();
        columnDropDown.options.Add(new TMP_Dropdown.OptionData("1"));
        columnDropDown.interactable = false;
    }

    public void OnRowDropDown()
    {
        Debug.Log("OnRowDropDown");
        gameplayManager.rowCount = rowDropDown.value + 1;
        columnDropDown.options.Clear();
        columnDropDown.interactable = true;

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
        string selectedColumnValue = columnDropDown.options[columnDropDown.value].text;

        // Assuming you want to use the selected value (convert it to integer if needed)
        int columnCount = int.Parse(selectedColumnValue);
        gameplayManager.columnCount = columnCount;


        LevelSelectionScreen.gameObject.SetActive(false);
        GamePanel.gameObject.SetActive(true);
        gameplayManager.enabled = true;



        
    }

    public void GameOver()
    {
        GamePanel.gameObject.SetActive(false);
        GameOverScreen.gameObject.SetActive(true);
        finalScore.text = gameplayManager.scoreText.text;
    }

    public void GameOverMenuButtonClick()
    {
        StartScreen.gameObject.SetActive(true);
        GameOverScreen.gameObject.SetActive(false);
        gameplayManager.enabled = false;
        gameplayManager.ResetGameplayUi();
        gameplayManager.ResetData();

    }
}
