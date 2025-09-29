using UnityEngine;
using UnityEngine.UI;

// ��ź ���� ������ UI ���� ��ũ��Ʈ
public class BombGaugeUI : MonoBehaviour
{
    // �ν����Ϳ��� Board ��ũ��Ʈ�� Slider ������Ʈ�� ����
    public Board board;
    public Slider bombGaugeSlider;

    void Start()
    {
        // Board ������ Slider ������ �ִ��� Ȯ��
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

        // Board�� ������ ������Ʈ �̺�Ʈ�� �Լ� ����
        board.OnBombGaugeUpdate += UpdateGaugeUI;

        // �ʱ� ������ �� ���� (Board.cs�� �ʱ� ������ ���� 0.55�� ����)
        // �� ���� Unity Inspector���� Slider�� Max Value�� 0.55�� �����ؾ� ��
        bombGaugeSlider.maxValue = 1f;
        UpdateGaugeUI(0.55f);
    }

    private void OnDestroy()
    {
        // ���� �ı��ǰų� �� ������Ʈ�� �ı��� �� �̺�Ʈ ���� ����
        if (board != null)
        {
            board.OnBombGaugeUpdate -= UpdateGaugeUI;
        }
    }

    // Board.cs���� ȣ���ϴ� ������ ������Ʈ �Լ�
    private void UpdateGaugeUI(float value)
    {
        bombGaugeSlider.value = value;
    }
}