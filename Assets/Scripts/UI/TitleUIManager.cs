using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class TitleUIManager : MonoBehaviour
{
    [Header("Buttons")]
    [SerializeField] private Button newGameButton;
    [SerializeField] private Button rankingButton;
    [SerializeField] private Button optionButton;

    [Header("Panels with Animators")]
    [SerializeField] private GameObject rankingPanel;
    [SerializeField] private GameObject optionPanel;
    [SerializeField] private Animator rankingAnimator;
    [SerializeField] private Animator optionAnimator;

    private void Start()
    {
        newGameButton.onClick.AddListener(OnNewGameClicked);
        rankingButton.onClick.AddListener(Ranking);
        optionButton.onClick.AddListener(OpenOptionPanel);
    }

    private void OnNewGameClicked()
    {
        SceneManager.LoadScene("GamePlayScene"); // 게임 씬 이름으로 교체
    }

    public void Ranking()
    {
        SceneManager.LoadScene("Ranking");
    }

    private void OpenOptionPanel()
    {
        if (optionPanel != null)
        {
            optionPanel.SetActive(true);
            optionAnimator.SetTrigger("open");
        }
    }

    // 👇 패널 닫기 함수 (애니메이션 재생 후 비활성화)
    public void ClosePanel(GameObject panel, Animator animator)
    {
        StartCoroutine(ClosePanelCoroutine(panel, animator));
    }

    private IEnumerator ClosePanelCoroutine(GameObject panel, Animator animator)
    {
        if (panel.activeSelf && animator != null)
        {
            animator.SetTrigger("close");
            yield return new WaitForSeconds(0.5f); // 애니메이션 길이에 맞게 수정
            panel.SetActive(false);
            animator.ResetTrigger("close");
        }
    }
    // TitleUIManager.cs 안에 추가
    public void CloseOptionPanel()
    {
        ClosePanel(optionPanel, optionAnimator);
    }

    

}
