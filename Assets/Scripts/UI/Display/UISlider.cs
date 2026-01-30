using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class UISlider : MonoBehaviour
{
    private Slider slider;

    private void OnEnable()
    {
        Managers.UI.updateBombGauge += SetGauge;
    }

    private void OnDisable()
    {
        Managers.UI.updateBombGauge -= SetGauge;
    }

    private void Start()
    {
        slider = GetComponent<Slider>();
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
