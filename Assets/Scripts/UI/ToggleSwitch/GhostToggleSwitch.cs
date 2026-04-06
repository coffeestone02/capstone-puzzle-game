using UnityEngine;

public class GhostToggleSwitch : ToggleSwitch
{
    private const string GHOST_KEY = "GHOST_OFF";

    [SerializeField] private GameObject ghostGrid;

    protected override void Start()
    {
        base.Start();

        onToggleOn -= GhostOff;
        onToggleOff -= GhostOn;

        onToggleOn += GhostOff;
        onToggleOff += GhostOn;

        ApplySavedSetting();
    }

    private void GhostOff()
    {
        if (ghostGrid != null)
            ghostGrid.SetActive(false);

        PlayerPrefs.SetInt(GHOST_KEY, 1);
        PlayerPrefs.Save();
    }

    private void GhostOn()
    {
        if (ghostGrid != null)
            ghostGrid.SetActive(true);

        PlayerPrefs.SetInt(GHOST_KEY, 0);
        PlayerPrefs.Save();
    }

    public void ApplySavedSetting()
    {
        bool isGhostOff = PlayerPrefs.GetInt(GHOST_KEY, 0) == 1;

        SetVisual(isGhostOff);

        if (ghostGrid != null)
            ghostGrid.SetActive(!isGhostOff);
    }
}