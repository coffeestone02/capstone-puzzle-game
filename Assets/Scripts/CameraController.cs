using UnityEngine;
using UnityEngine.Tilemaps;

public class CameraController : MonoBehaviour
{
    private Camera cam;

    void Start()
    {
        cam = GetComponent<Camera>();
        SetCameraDistance();
    }

    private void SetCameraDistance()
    {
        Tilemap tilemap = GameObject.Find("VisibleGrid").GetComponentInChildren<Tilemap>();
        if (tilemap == null)
        {
            Debug.LogError("VisibleTilemap이 없음");
            return;
        }

        int width = tilemap.cellBounds.size.x;
        float aspect = (float)Screen.width / Screen.height;

        cam.orthographicSize = width / (2f * aspect);
    }
}
