using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PieceMover : MonoBehaviour
{
    public float stepDelay = 1.25f; // 중심으로 이동하는 속도. 자동으로 stepDelay의 시간만큼 중심으로 이동함
    public float moveDelay = 0.1f; // 플레이어의 입력 이동 속도를 정함. 값이 클수록 입력을 많이 못함
    public float lockDelay = 0.5f; // 이 시간만큼 못 움직이면 피스를 Lock함

    private float stepTime; // 중심으로 이동하는 시기
    private float moveTime; // 다음 입력을 받을 수 있는 시기
    public float lockTime { get; private set; } // 고정되는 시기(lockTime이 lockDelay를 넘기는 순간 고정됨)
}
