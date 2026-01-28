using UnityEngine;
using UnityEngine.UI;

public abstract class UIButton : MonoBehaviour
{
    protected Button btn;

    protected virtual void Start()
    {
        btn = GetComponent<Button>();
        btn.onClick.AddListener(ButtonAction);
    }

    protected abstract void ButtonAction();
}
