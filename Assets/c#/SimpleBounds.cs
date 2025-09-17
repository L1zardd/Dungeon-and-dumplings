using UnityEngine;

public class SimpleBounds : MonoBehaviour
{
    [Header("Bounds Settings")]
    public float padding = 1f;

    private Camera _mainCamera;
    private float _minX, _maxX, _minY, _maxY;

    void Start()
    {
        _mainCamera = Camera.main;
        CalculateBounds();
    }

    void Update()
    {
        KeepInBounds();
    }

    void CalculateBounds()
    {
        if (_mainCamera == null) return;

        float cameraHeight = _mainCamera.orthographicSize;
        float cameraWidth = cameraHeight * _mainCamera.aspect;

        Vector3 cameraPos = _mainCamera.transform.position;

        _minX = cameraPos.x - cameraWidth + padding;
        _maxX = cameraPos.x + cameraWidth - padding;
        _minY = cameraPos.y - cameraHeight + padding;
        _maxY = cameraPos.y + cameraHeight - padding;
    }

    void KeepInBounds()
    {
        Vector3 pos = transform.position;

        pos.x = Mathf.Clamp(pos.x, _minX, _maxX);
        pos.y = Mathf.Clamp(pos.y, _minY, _maxY);

        transform.position = pos;
    }

    // Для обновления границ при изменении размера окна игры
    void OnRectTransformDimensionsChange()
    {
        CalculateBounds();
    }
}