using UnityEngine;

public class HowToPlayManager : MonoBehaviour
{
    [SerializeField] private GameObject popup1;
    [SerializeField] private GameObject popup2;
    [SerializeField] private GameObject popup3;

    private bool openFromGameplay = false;
    private int currentPage = 1;

    private void Awake()
    {
        popup1.SetActive(false);
        popup2.SetActive(false);
        popup3.SetActive(false);
    }

    public void Open()
    {
        openFromGameplay = false;
        currentPage = 1;

        popup1.SetActive(true);
        popup2.SetActive(false);
        popup3.SetActive(false);
    }

    public void OpenFromGameplay()
    {
        openFromGameplay = true;
        currentPage = 1;

        popup1.SetActive(true);
        popup2.SetActive(false);
        popup3.SetActive(false);
    }

    public void NextPage()
    {
        currentPage++;

        popup1.SetActive(currentPage == 1);
        popup2.SetActive(currentPage == 2);
        popup3.SetActive(currentPage == 3);
    }

    public void Close()
    {
        popup1.SetActive(false);
        popup2.SetActive(false);
        popup3.SetActive(false);
    }

    public bool IsOpenedFromGameplay()
    {
        return openFromGameplay;
    }
}