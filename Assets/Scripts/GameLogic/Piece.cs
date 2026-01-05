using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Tilemaps;

public class Piece : MonoBehaviour
{
    private TriominoData data;
    private Vector2Int[] cells;
    private Vector2Int position;
    private int rotationIdx;
}
