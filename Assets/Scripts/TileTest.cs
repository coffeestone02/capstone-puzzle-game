using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TileTest : MonoBehaviour
{
    [SerializeField] private float fadeSpeed = 1.5f;

    public float totalDeltaTime;
    public Tile testTile;
    public Vector3Int[] arr = {
        new Vector3Int(0, 0),
        new Vector3Int(0, 1),
        new Vector3Int(0, -1),
        new Vector3Int(1, 0),
        new Vector3Int(-1, 0),
    };
    public Tilemap tilemap;

    void Start()
    {
        foreach (var pos in arr)
        {
            tilemap.SetTile(pos, testTile);
        }

        StartCoroutine(Fade());
    }

    // private void Update()
    // {
    //     totalDeltaTime += Time.deltaTime;

    //     float step = totalDeltaTime / fadeSpeed;
    //     Color currentColor = Color.Lerp(tilemap.GetTile<Tile>(pos).color, Color.clear, step);

    //     if (step >= 2.0f)
    //     {
    //         currentColor = Color.clear;
    //     }

    //     tilemap.SetColor(pos, currentColor);
    // }

    private IEnumerator Fade()
    {
        foreach (var pos in arr)
        {
            tilemap.RemoveTileFlags(pos, TileFlags.LockColor);
        }

        while (totalDeltaTime < fadeSpeed)
        {
            totalDeltaTime += Time.deltaTime;
            float step = totalDeltaTime / fadeSpeed;

            foreach (var pos in arr)
            {
                Color c = Color.Lerp(tilemap.GetTile<Tile>(pos).color, Color.clear, step);
                tilemap.SetColor(pos, c);
            }

            yield return null;
        }

        yield return new WaitForSeconds(2f);
    }
}
