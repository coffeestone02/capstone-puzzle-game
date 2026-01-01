using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

// 게임관리 기능
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    private ScoreManager scoreManager = new ScoreManager();
    private UIManager uiManager = new UIManager();

    public static UIManager UI { get { return Instance.uiManager; } }
    private SaveController saveController;
    public Board board { get; private set; }

    // 상태와 점수 변수들
    public bool isPause = false;
    public bool isOver = false;
    public float playtime { get; private set; }
    public int score { get; set; }
    public int level { get; set; } = 1;

    private void Start()
    {
        Init();
    }

    private void Init()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this);
        }

        uiManager.Init();
        saveController = GameObject.Find("Save Controller").GetComponent<SaveController>();
        board = GameObject.Find("Board").GetComponent<Board>();
    }

    private void Update()
    {
        Timer();
        uiManager.OnUpdate();
    }

    private void Timer()
    {
        if (isOver || isPause) return;

        playtime += Time.deltaTime;
        scoreManager.playTime = playtime;
    }

    public void MainMenu()
    {
        SceneManager.LoadScene("Title");
    }

    public void GamePlay()
    {
        SceneManager.LoadScene("GamePlayScene");
    }

    public void Restart()
    {
        SaveSystem.Clear();
        AudioManager.Instance.PlayBgm(1);
        SceneManager.LoadScene("GamePlayScene");
    }

    public void Exit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false; // 에디터 상에서 종료
#else
        Application.Quit(); // 앱 종료
#endif
    }

    private void OnApplicationPause(bool pause) //일시정지 시 저장
    {
        if (pause && saveController != null)
            saveController.SaveNow();
    }

    private void OnApplicationQuit() // 나갈 때 저장
    {
        if (saveController != null)
            saveController.SaveNow();
    }
}