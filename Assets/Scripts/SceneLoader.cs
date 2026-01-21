using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// 테스트용(공사중)
/// </summary>
public class SceneLoader : MonoBehaviour
{
    public void LoadSceneAsync(string sceneName)
    {
        StartCoroutine(LoadSceneCoroutine(sceneName));
    }

    private IEnumerator LoadSceneCoroutine(string sceneName)
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);

        while (asyncLoad.isDone != false)
        {
            Debug.Log(asyncLoad.progress);
            yield return null;
        }
    }
}
