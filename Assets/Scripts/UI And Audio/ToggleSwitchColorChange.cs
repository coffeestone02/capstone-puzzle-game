using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ToggleSwitchColorChange : ToggleSwitch
{
    [Header("Elements to Recolor")]
    [SerializeField] private Image backgroundImage;
    [SerializeField] private Image handleImage;
        
    [Space]
    [SerializeField] private bool recolorBackground;
    [SerializeField] private bool recolorHandle;
        
    [Header("Colors")]
    [SerializeField] private Color backgroundColorOff = Color.white;
    [SerializeField] private Color backgroundColorOn = new Color(166f, 166f, 166f);
    [Space]
    [SerializeField] private Color handleColorOff = new Color(0f, 255f, 243f);
    [SerializeField] private Color handleColorOn = new Color(123f, 123f, 123f);
        
    protected override void OnValidate()
    {
        base.OnValidate();

        ChangeColors();
    }

    private void OnEnable()
    {
        transitionEffect += ChangeColors;
    }
        
    private void OnDisable()
    {
        transitionEffect -= ChangeColors;
    }

    protected override void Awake() 
    {
        base.Awake();
            
        ChangeColors();
    }



    private void ChangeColors()
    {
        if (recolorBackground)
            backgroundImage.color = Color.Lerp(backgroundColorOff, backgroundColorOn, sliderValue); 
            
        if (recolorHandle)
            handleImage.color = Color.Lerp(handleColorOff, handleColorOn, sliderValue); 
    }
}
