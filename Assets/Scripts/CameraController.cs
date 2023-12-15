using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class CameraController : MonoBehaviour {

    public Vector2Int resolution = new Vector2Int(1920, 1080);
    private Vector2Int prevResolution;
    private Camera cam;
    private float desiredRatio;

    [SerializeField]
    private float minimumSize;

    void Awake() {
        cam = GetComponent<Camera>();

        prevResolution = new Vector2Int();

        desiredRatio = (float) resolution.x / (float) resolution.y;

        RecalculateSize();
    }

    void Update() {
        if ((prevResolution.x != Screen.width) || (prevResolution.y != Screen.height)) {
            RecalculateSize();
        }
    }

    private void RecalculateSize() {
        if (cam == null) return;

        prevResolution.x = Screen.width;
        prevResolution.y = Screen.height;

        float currentRatio = (float) prevResolution.x / (float) prevResolution.y;

        cam.orthographicSize = Mathf.Max(minimumSize * (desiredRatio / currentRatio), minimumSize);

    }
}
