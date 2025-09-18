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
        // Только оригиналы возвращаются на место
        if (isOriginal && !_isDragging && transform.position != _initialPosition && _currentCopy == null)
        {
            transform.position = Vector3.Lerp(transform.position, _initialPosition, returnSpeed * Time.deltaTime);

            if (Vector3.Distance(transform.position, _initialPosition) < 0.1f)
            {
                transform.position = _initialPosition;
            }
        }

        // Перемещаем копию при перетаскивании
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

        // Отключаем коллайдер на время перетаскивания
        if (_collider != null)
        {
            _collider.enabled = false;
        }

        // Только оригиналы создают копии
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

        // Включаем коллайдер обратно
        if (_collider != null)
        {
            _collider.enabled = true;
        }

        // Если есть копия, настраиваем ее как независимый объект
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

        // УДАЛЯЕМ все компоненты перетаскивания
        DraggableVegetable[] oldDraggables = _currentCopy.GetComponents<DraggableVegetable>();
        foreach (var old in oldDraggables)
        {
            Destroy(old);
        }

        // ДОБАВЛЯЕМ DraggableCopy компонент
        DraggableCopy draggableCopy = _currentCopy.AddComponent<DraggableCopy>();
        draggableCopy.boundsPadding = this.boundsPadding;

        // Настраиваем сортировку
        SpriteRenderer copyRenderer = _currentCopy.GetComponent<SpriteRenderer>();
        if (copyRenderer != null && _spriteRenderer != null)
        {
            copyRenderer.sortingOrder = _spriteRenderer.sortingOrder + 1;
        }

        // ВКЛЮЧАЕМ все необходимые компоненты
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

        // Убеждаемся что все компоненты включены
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

        // Восстанавливаем нормальный sorting order
        SpriteRenderer copyRenderer = _currentCopy.GetComponent<SpriteRenderer>();
        if (copyRenderer != null)
        {
            copyRenderer.sortingOrder = 5;
        }

        // Добавляем в менеджер
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