using UnityEngine;
using UnityEngine.UI;

// 폭탄 생성 게이지 UI 관리 스크립트
public class BombGaugeUI : MonoBehaviour
{
    // 인스펙터에서 Board 스크립트와 Slider 컴포넌트를 연결
    public Board board;
    public Slider bombGaugeSlider;

    void Start()
    {
        // Board 참조와 Slider 참조가 있는지 확인
        if (board == null)
        {
            Debug.LogError("Board is not assigned in BombGaugeUI.");
            return;
        }
        if (bombGaugeSlider == null)
        {
            Debug.LogError("Bomb Gauge Slider is not assigned in BombGaugeUI.");
            return;
        }

        // Board의 게이지 업데이트 이벤트에 함수 연결
        board.OnBombGaugeUpdate += UpdateGaugeUI;

        // 초기 게이지 값 설정 (Board.cs의 초기 로직에 맞춰 0.55로 설정)
        // 이 값은 Unity Inspector에서 Slider의 Max Value에 0.55를 설정해야 함
        bombGaugeSlider.maxValue = 1f;
        UpdateGaugeUI(0.55f);
    }

    private void OnDestroy()
    {
        // 씬이 파괴되거나 이 오브젝트가 파괴될 때 이벤트 연결 해제
        if (board != null)
        {
            board.OnBombGaugeUpdate -= UpdateGaugeUI;
        }
    }

    // Board.cs에서 호출하는 게이지 업데이트 함수
    private void UpdateGaugeUI(float value)
    {
        bombGaugeSlider.value = value;
    }
}