using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GamplayButton : MonoBehaviour
{
    Button gameplayBtn;

    private void Start()
    {
        gameplayBtn = GetComponent<Button>();
        gameplayBtn.onClick.AddListener(IntoGamePlay);
    }

    public void IntoGamePlay()
    {
        SceneManager.LoadScene("GamePlayScene");
    }
}
