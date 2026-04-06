using System.Collections;
using UnityEngine;

public class ItemButtonGroup : MonoBehaviour
{
    private RectTransform rect;
    private Vector2 basePos;

    private float moveUpY = 120f;         // 버튼 위로 올리는 거리
    private float aspectThreshold = 0.5f; // 9:20(0.45)은 그대로, 9:16(0.56), 3:4(0.75)는 이동

    private int lastScreenWidth;
    private int lastScreenHeight;

    private void Awake()
    {
        rect = GetComponent<RectTransform>();
        basePos = rect.anchoredPosition;

        lastScreenWidth = Screen.width;
        lastScreenHeight = Screen.height;
    }

    private void Start()
    {
        StartCoroutine(ApplyNextFrame());
    }

    private IEnumerator ApplyNextFrame()
    {
        yield return null;
        ApplyPosition();
    }

    private void Update()
    {
        // 화면 비율이 바뀌었을 때 다시 적용
        if (Screen.width != lastScreenWidth || Screen.height != lastScreenHeight)
        {
            lastScreenWidth = Screen.width;
            lastScreenHeight = Screen.height;
            ApplyPosition();
        }

    }

    private void ApplyPosition()
    {
        float aspect = (float)Screen.width / Screen.height;

        Vector2 pos = basePos;

        if (aspect >= aspectThreshold)
        {
            pos.y += moveUpY;
        }

        rect.anchoredPosition = pos;
    }
}