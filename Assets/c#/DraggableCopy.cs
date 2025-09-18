using UnityEngine;

public class DraggableCopy : MonoBehaviour
{
    private Vector3 _screenPoint;
    private Vector3 _offset;
    private bool _isDragging = false;
    private Camera _mainCamera;
    private SpriteRenderer _spriteRenderer;
    private Collider2D _collider;

    [Header("Dragging Settings")]
    public float boundsPadding = 0.5f;

    public bool IsDragging => _isDragging;

    void Start()
    {
        _mainCamera = Camera.main;
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _collider = GetComponent<Collider2D>();
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

        if (_collider != null)
        {
            _collider.enabled = false;
        }

        BringToFront();
    }

    void OnMouseUp()
    {
        _isDragging = false;

        if (_collider != null)
        {
            _collider.enabled = true;
        }
    }

    void BringToFront()
    {
        if (_spriteRenderer != null)
        {
            _spriteRenderer.sortingOrder = 100;
        }
    }
}