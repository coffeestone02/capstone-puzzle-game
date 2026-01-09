using UnityEngine;

/// <summary>
/// 게임룰을 포함한 다른 매니저들을 관리하는 클래스
/// </summary>
public class Managers : MonoBehaviour
{
    public static Managers Instance { get; private set; }

    private GameRule rule = new GameRule();
    private InputManager input = new InputManager();

    public static GameRule Rule { get { return Instance.rule; } }
    public static InputManager Input { get { return Instance.input; } }

    private void Start()
    {
        Init();
    }

    private void Update()
    {
        input.OnUpdate();
    }

    private void Init()
    {
        if (Instance == null)
            Instance = this;
        else if (Instance != this)
            Destroy(gameObject);

        DontDestroyOnLoad(gameObject);
    }
}
