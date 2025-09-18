using UnityEngine;

public class DraggableVegetable : MonoBehaviour
{
    private Vector3 _screenPoint;
    private Vector3 _offset;
    private bool _isDragging = false;
    private Vector3 _initialPosition;
    private Camera _mainCamera;
    private GameObject _currentCopy;
    private SpriteRenderer _spriteRenderer;
    private Collider2D _collider;

    [Header("Dragging Settings")]
    public bool canBeDragged = true;
    public float returnSpeed = 8f;
    public float boundsPadding = 0.5f;

    [Header("Copy Settings")]
    public GameObject vegetablePrefab;
    public bool createCopyOnDrag = true;
    public bool isOriginal = true;

    public bool IsDragging => _isDragging;

    void Start()
    {
        _mainCamera = Camera.main;
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _collider = GetComponent<Collider2D>();

        if (isOriginal)
        {
            _initialPosition = transform.position;
        }
    }

    void Update()
    {
        // ������ ��������� ������������ �� �����
        if (isOriginal && !_isDragging && transform.position != _initialPosition && _currentCopy == null)
        {
            transform.position = Vector3.Lerp(transform.position, _initialPosition, returnSpeed * Time.deltaTime);

            if (Vector3.Distance(transform.position, _initialPosition) < 0.1f)
            {
                transform.position = _initialPosition;
            }
        }

        // ���������� ����� ��� ��������������
        if (_isDragging && _currentCopy != null)
        {
            Vector3 curScreenPoint = new Vector3(Input.mousePosition.x, Input.mousePosition.y, _screenPoint.z);
            Vector3 curPosition = _mainCamera.ScreenToWorldPoint(curScreenPoint) + _offset;

            curPosition = ApplyCameraBounds(curPosition);
            _currentCopy.transform.position = curPosition;
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
        if (!canBeDragged) return;

        _isDragging = true;
        _screenPoint = _mainCamera.WorldToScreenPoint(transform.position);
        _offset = transform.position - _mainCamera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, _screenPoint.z));

        // ��������� ��������� �� ����� ��������������
        if (_collider != null)
        {
            _collider.enabled = false;
        }

        // ������ ��������� ������� �����
        if (isOriginal && createCopyOnDrag && vegetablePrefab != null && _currentCopy == null)
        {
            CreateVegetableCopy();
        }

        BringToFront();
    }

    void OnMouseUp()
    {
        if (!_isDragging) return;

        _isDragging = false;

        // �������� ��������� �������
        if (_collider != null)
        {
            _collider.enabled = true;
        }

        // ���� ���� �����, ����������� �� ��� ����������� ������
        if (_currentCopy != null)
        {
            SetupCopyAsIndependent();
            _currentCopy = null;
        }
    }

    void CreateVegetableCopy()
    {
        _currentCopy = Instantiate(vegetablePrefab, transform.position, Quaternion.identity);
        _currentCopy.name = vegetablePrefab.name;

        // ������� ��� ���������� ��������������
        DraggableVegetable[] oldDraggables = _currentCopy.GetComponents<DraggableVegetable>();
        foreach (var old in oldDraggables)
        {
            Destroy(old);
        }

        // ��������� DraggableCopy ���������
        DraggableCopy draggableCopy = _currentCopy.AddComponent<DraggableCopy>();
        draggableCopy.boundsPadding = this.boundsPadding;

        // ����������� ����������
        SpriteRenderer copyRenderer = _currentCopy.GetComponent<SpriteRenderer>();
        if (copyRenderer != null && _spriteRenderer != null)
        {
            copyRenderer.sortingOrder = _spriteRenderer.sortingOrder + 1;
        }

        // �������� ��� ����������� ����������
        Collider2D copyCollider = _currentCopy.GetComponent<Collider2D>();
        if (copyCollider != null)
        {
            copyCollider.enabled = true;
        }

        Vegetable vegetableComponent = _currentCopy.GetComponent<Vegetable>();
        if (vegetableComponent != null)
        {
            vegetableComponent.enabled = true;
        }
    }

    void SetupCopyAsIndependent()
    {
        if (_currentCopy == null) return;

        // ���������� ��� ��� ���������� ��������
        Collider2D collider = _currentCopy.GetComponent<Collider2D>();
        if (collider != null)
        {
            collider.enabled = true;
        }

        Vegetable vegetableComponent = _currentCopy.GetComponent<Vegetable>();
        if (vegetableComponent != null)
        {
            vegetableComponent.enabled = true;
        }

        // ��������������� ���������� sorting order
        SpriteRenderer copyRenderer = _currentCopy.GetComponent<SpriteRenderer>();
        if (copyRenderer != null)
        {
            copyRenderer.sortingOrder = 5;
        }

        // ��������� � ��������
        VegetableManager manager = FindObjectOfType<VegetableManager>();
        if (manager != null)
        {
            manager.AddVegetable(_currentCopy);
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