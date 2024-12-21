using System.Linq;
using TMPro;
using UnityEngine;
public class MainMenuManager : MonoBehaviour
{

    public static MainMenuManager Instance;
    public GamePlayManager gameplayManager;

    public Transform StartScreen;
 //   public Transform LevelSelectionScreen;
    public Transform LevelSelectionScreen;
    public Transform GamePanel;
    public Transform GameOverScreen;

    public TMP_Dropdown rowDropDown;
    public TMP_Dropdown columnDropDown;

    private void Awake()
    {
        Instance = this;
    }
    // Start is called before the first frame update
    void Start()
    {
        rowDropDown.options.Clear();
        for (int i = 1; i <= gameplayManager.CardGraphics.Count; i++)
        {
            rowDropDown.options.Add(new TMP_Dropdown.OptionData(i.ToString()));
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void OnStartScreenStartButtonClick()
    {
       LevelSelectionScreen.gameObject.SetActive(true);
        StartScreen.gameObject.SetActive(false);
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


    public void GameStart()
    {
        gameplayManager.columnCount = columnDropDown.value + 1;


        LevelSelectionScreen.gameObject.SetActive(false);
        GamePanel.gameObject.SetActive(true);
        gameplayManager.enabled = true;
        // need to enable Score , Timer
    }

    public void GameOver()
    {
        GamePanel.gameObject.SetActive(false);
        GameOverScreen.gameObject.SetActive(true);
    }
}
