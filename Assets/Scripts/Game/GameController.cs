using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using LemonadeStore;

public class GameController : MonoBehaviour
{
    private GameObject boardManager;
    private BoardController boardController;
    private GameTileManager gameTileManager;

    [SerializeField]
    private ShopController shopController;
    [SerializeField]
    private TutorialController tutorialController;
    private BGColors currentColor
    {
        get
        {
            return (BGColors)PlayerPrefsHelper.GetInt(GlobalConst.CURRENT_COLOR_KEY);
        }
        set
        {
            PlayerPrefsHelper.SetInt(GlobalConst.CURRENT_COLOR_KEY, (int)value);
        }
    }
    
    [SerializeField]
    private GameObject startScreen;
    [SerializeField]
    private GameObject gameScreen;
    [SerializeField]
    private GameObject pauseScreen;
    [SerializeField]
    private GameObject endScreen;
    [SerializeField]
    private GameObject settingsScreen;
    [SerializeField]
    private GameObject shopScreen;
    [SerializeField]
    private GameObject backgroundScreen;
    [SerializeField]
    private GameObject exitDialogue;
    [SerializeField]
    private UpgradeWorker lemoncityController;


    [SerializeField]
    private Text pointsBoardText;
    [SerializeField]
    private Text endGameScore;

    private int points;
    private bool isPaused;

    private void Awake()
    {
        StaticEventManager.OnGameLose += EndGame;
        StaticEventManager.OnTileDead += AddPoints;

        Time.timeScale = 1;
    }

    private void OnDestroy()
    {
        StaticEventManager.OnGameLose -= EndGame;
        StaticEventManager.OnTileDead -= AddPoints;
    }

    private void Start()
    {
        if (boardManager == null)
            boardManager = GameObject.FindGameObjectWithTag("BoardManager");
        if (shopController == null)
            shopController = FindObjectOfType<ShopController>();

        boardController = boardManager.GetComponent<BoardController>();
        gameTileManager = boardManager.GetComponent<GameTileManager>();

        boardController.enabled = false;
        gameTileManager.enabled = false;
        isPaused = false;

        startScreen.SetActive(true);
        settingsScreen.SetActive(false);
        gameScreen.SetActive(false);
        pauseScreen.SetActive(false);
        endScreen.SetActive(false);
        shopScreen.SetActive(false);
        backgroundScreen.SetActive(false);

        shopController.items[0].IsUnlocked = true;

        StaticEventManager.CallOnMenuEnter();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (exitDialogue.activeSelf)
                exitDialogue.SetActive(false);
            else
                exitDialogue.SetActive(true);
        }

        if (isPaused)
            Time.timeScale = 0f;
        else
            Time.timeScale = 1f;
    }

    public void CloseExitDialogue()
    {
        exitDialogue.SetActive(false);
    }

    public void StartGame()
    {
        boardController.enabled = true;
        gameTileManager.enabled = true;
        boardController.Initialize();
        startScreen.SetActive(false);
        gameScreen.SetActive(true);

        int firstLockedItemIndex = shopController.GetFirstLockedIndex();
        StaticEventManager.CallOnBottleImageRefresh(firstLockedItemIndex);

        if (shopController.isRandom)
            shopController.GetRandomBackground();

        StaticEventManager.CallOnBackgroundColorChanged((int)currentColor);
        StaticEventManager.CallOnMenuExit();

        // Here goes tutorial activation code
        // Enables on first launch of the game

        if(AdsController.gameStartsCount < 1)
        {
            tutorialController.gameObject.SetActive(true);

            Button pauseButton = gameScreen.transform.GetChild(0).GetComponent<Button>();
            pauseButton.interactable = false;
            Button bottleButton = gameScreen.transform.GetChild(2).GetChild(0).GetComponent<Button>();
            bottleButton.interactable = false;

            isPaused = true;
        }
    }

    public void EndTutorial()
    {
        Button pauseButton = gameScreen.transform.GetChild(0).GetComponent<Button>();
        pauseButton.interactable = true;
        Button bottleButton = gameScreen.transform.GetChild(2).GetChild(0).GetComponent<Button>();
        bottleButton.interactable = true;

        tutorialController.gameObject.SetActive(false);
        isPaused = false;
    }

    private void EndGame()
    {
        boardController.IsPlaying = false;
        gameScreen.SetActive(false);
        ParticleController pc = FindObjectOfType<ParticleController>();
        SoundPlayer.Play("lose", 1f);
        pc.LaunchBubbleWall();
        StartCoroutine(DelayedEndGame());
    }

    private IEnumerator DelayedEndGame()
    {
        yield return new WaitForSeconds(0.625f);
        boardController.ResetBoard(false);
        yield return new WaitForSeconds(4.5f);
        endScreen.SetActive(true);
        //currentOpenScreen = endScreen;
        endGameScore.text = points.ToString();
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;
        isPaused = false;
        endScreen.SetActive(false);
        pauseScreen.SetActive(false);

        if (!gameScreen.activeSelf)
            gameScreen.SetActive(true);

        points = 0;
        if (pointsBoardText.text != null)
            pointsBoardText.text = points.ToString();

        boardController.ResetBoard(true);
        boardController.PlayAgain();

        if (shopController.isRandom)
            shopController.GetRandomBackground();
    }

    public void ExitGame()
    {
        Application.Quit();
    }

    public void ExitToMenu()
    {
        boardController.ResetBoard(true);
        boardController.enabled = false;
        gameTileManager.enabled = false;

        Time.timeScale = 1f;
        isPaused = false;

        startScreen.SetActive(true);
        settingsScreen.SetActive(false);
        gameScreen.SetActive(false);
        pauseScreen.SetActive(false);
        endScreen.SetActive(false);
        shopScreen.SetActive(false);
        backgroundScreen.SetActive(false);

        StaticEventManager.CallOnMenuEnter();
    }
    public void GoToLemoncity()
    {
        boardController.ResetBoard(true);
        boardController.enabled = false;
        gameTileManager.enabled = false;

        Time.timeScale = 1f;
        isPaused = false;

        startScreen.SetActive(true);
        settingsScreen.SetActive(false);
        gameScreen.SetActive(false);
        pauseScreen.SetActive(false);
        endScreen.SetActive(false);
        shopScreen.SetActive(false);
        backgroundScreen.SetActive(false);

        StaticEventManager.CallOnMenuEnter();

        lemoncityController.OpenLemoncityWindow();
    }
    public void OpenLemoncity()
    {
        lemoncityController.OpenLemoncityWindow();
    }
    public void OpenPauseScreen()
    {
        isPaused = true;
        pauseScreen.SetActive(true);
        gameScreen.SetActive(false);
    }

    public void ClosePauseScreen()
    {
        isPaused = false;
        pauseScreen.SetActive(false);
        gameScreen.SetActive(true);
    }

    public void OpenSettings()
    {
        settingsScreen.SetActive(true);

        if (isPaused)
            pauseScreen.SetActive(false);
        else
            startScreen.SetActive(false);
    }

    public void CloseSettings()
    {
        settingsScreen.SetActive(false);

        if (isPaused)
            pauseScreen.SetActive(true);
        else
            startScreen.SetActive(true);
    }

    public void OpenBackgroundScreen()
    {
        isPaused = true;
        backgroundScreen.SetActive(true);
        gameScreen.SetActive(false);
    }

    public void CloseBackgroundScreen()
    {
        isPaused = false;
        backgroundScreen.SetActive(false);
        gameScreen.SetActive(true);
    }

    public void OpenShop()
    {
        startScreen.SetActive(false);
        shopScreen.SetActive(true);
    }

    public void CloseShop()
    {
        startScreen.SetActive(true);
        shopScreen.SetActive(false);
    }

    private void AddPoints()
    {
        points += 100;
        if (lemoncityController != null)
            lemoncityController.Points += 1;
        
        if (pointsBoardText.text != null)
            pointsBoardText.text = points.ToString();
        if (points % 5000 == 0)
            boardController.SetGameTilesSpeed();
        if(points % 5000 == 0)
            boardController.SetDifficulty();

        if (points == 5000)        
            shopController.SetNewBackground(1);
        else if (points == 10000)
            shopController.SetNewBackground(2);
        else if (points == 15000)
            shopController.SetNewBackground(3);
        else if (points == 20000)
            shopController.SetNewBackground(4);
    }
}
