using UnityEngine;

public class DraggablePot : MonoBehaviour
{
    private Vector3 _screenPoint;
    private Vector3 _offset;
    private bool _isDragging = false;
    private Camera _mainCamera;
    private CookingPot _cookingPot;

    [Header("Dragging Settings")]
    public float boundsPadding = 1f;

    void Start()
    {
        _mainCamera = Camera.main;
        _cookingPot = GetComponent<CookingPot>();
    }

    void Update()
    {
        if (_isDragging)
        {
            Vector3 curScreenPoint = new Vector3(Input.mousePosition.x, Input.mousePosition.y, _screenPoint.z);
            Vector3 curPosition = _mainCamera.ScreenToWorldPoint(curScreenPoint) + _offset;

            curPosition = ApplyCameraBounds(curPosition);
            transform.position = curPosition;
        }
    }

    Vector3 ApplyCameraBounds(Vector3 position)
    {
        if (_mainCamera == null) return position;

        float cameraHeight = _mainCamera.orthographicSize;
        float cameraWidth = cameraHeight * _mainCamera.aspect;
        Vector3 cameraPos = _mainCamera.transform.position;

        position.x = Mathf.Clamp(position.x,
            cameraPos.x - cameraWidth + boundsPadding,
            cameraPos.x + cameraWidth - boundsPadding);

        position.y = Mathf.Clamp(position.y,
            cameraPos.y - cameraHeight + boundsPadding,
            cameraPos.y + cameraHeight - boundsPadding);

        return position;
    }

    void OnMouseDown()
    {
        _isDragging = true;
        _screenPoint = _mainCamera.WorldToScreenPoint(transform.position);
        _offset = transform.position - _mainCamera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, _screenPoint.z));
    }

    void OnMouseUp()
    {
        _isDragging = false;
    }
}