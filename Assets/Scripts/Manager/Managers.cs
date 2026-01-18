using UnityEngine;

/// <summary>
/// 게임룰을 포함한 다른 매니저들을 관리하는 클래스
/// </summary>
public class Managers : MonoBehaviour
{
    public static Managers Instance { get; private set; }

    private GameManager _rule = new GameManager();
    private InputManager _input = new InputManager();
    private AudioManager _audio = new AudioManager();
    private ScoreManager _score = new ScoreManager();

    public static GameManager Rule { get { return Instance._rule; } }
    public static InputManager Input { get { return Instance._input; } }
    public static AudioManager Audio { get { return Instance._audio; } }
    public static ScoreManager Score { get { return Instance._score; } }

    private void Start()
    {
        Init();
    }

    private void Update()
    {
        _input.OnUpdate();
    }

    private void Init()
    {
        if (Instance == null)
            Instance = this;
        else if (Instance != this)
            Destroy(gameObject);

        DontDestroyOnLoad(gameObject);
        Instance._audio.Init();
    }
}
