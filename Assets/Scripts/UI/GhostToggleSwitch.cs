using UnityEngine;

public class GhostToggleSwitch : ToggleSwitch
{
    private GameObject ghostGrid;

    protected override void Start()
    {
        base.Start();

        ghostGrid = GameObject.Find("GhostGrid");

        onToggleOn -= GhostOff;
        onToggleOff -= GhostOn;

        onToggleOn += GhostOff;
        onToggleOff += GhostOn;
    }

    private void GhostOff()
    {
        ghostGrid.SetActive(false);
    }

    private void GhostOn()
    {
        ghostGrid.SetActive(true);
    }
}
