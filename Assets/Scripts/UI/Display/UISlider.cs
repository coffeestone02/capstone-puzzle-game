using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class UISlider : MonoBehaviour
{
    private Slider slider;

    private void Start()
    {
        slider = GetComponent<Slider>();

        Managers.UI.updateBombGauge -= SetGauge;
        Managers.UI.updateBombGauge += SetGauge;
    }

    private void SetGauge()
    {
        int gauge = Managers.Rule.BlockCounter;
        int limit = Managers.Rule.bombSpawnLimit;

        if (gauge < limit)
        {
            slider.value = (float)gauge / limit;
        }
        else
        {
            slider.value = 0f;
        }
    }

}
