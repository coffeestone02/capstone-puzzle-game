using UnityEngine;

public class Managers : MonoBehaviour
{
    public static Managers Instance { get; private set; }

    private GameRule rule = new GameRule();

    public static GameRule Rule { get { return Instance.rule; } }

    private void Start()
    {
        Init();
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
