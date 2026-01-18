using UnityEngine;
using UnityEngine.SceneManagement;

public class TestScript : MonoBehaviour
{
    public void IntoGamePlay()
    {
        SceneManager.LoadScene("GamePlayScene");
    }
}
