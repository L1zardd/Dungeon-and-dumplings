using UnityEngine;

public class KnifeController : MonoBehaviour
{
    private Vector3 _screenPoint;
    private Vector3 _offset;
    private bool _isDragging = false;
    private SpriteRenderer _spriteRenderer;
    private int _originalSortingOrder;
    private string _originalSortingLayer;
    private Collider2D _collider;

    [Header("Knife Settings")]
    public string knifeSortingLayer = "Knife";
    public int knifeSortingOrder = 10;

    void Start()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _collider = GetComponent<Collider2D>();

        if (_spriteRenderer != null)
        {
            _originalSortingOrder = _spriteRenderer.sortingOrder;
            _originalSortingLayer = _spriteRenderer.sortingLayerName;
        }

        // Устанавливаем слой ножа
        gameObject.layer = LayerMask.NameToLayer("Knife");
    }

    void Update()
    {
        if (_isDragging)
        {
            Vector3 curScreenPoint = new Vector3(Input.mousePosition.x, Input.mousePosition.y, _screenPoint.z);
            Vector3 curPosition = Camera.main.ScreenToWorldPoint(curScreenPoint) + _offset;

            // Сохраняем Z-координату для правильных коллизий
            curPosition.z = 10;
            transform.position = curPosition;
        }
    }

    void OnMouseDown()
    {
        _isDragging = true;
        _screenPoint = Camera.main.WorldToScreenPoint(transform.position);
        _offset = transform.position - Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, _screenPoint.z));

        BringKnifeToFront();
    }

    void OnMouseUp()
    {
        _isDragging = false;
        ReturnKnifeToNormal();
    }

    void BringKnifeToFront()
    {
        if (_spriteRenderer != null)
        {
            _spriteRenderer.sortingLayerName = knifeSortingLayer;
            _spriteRenderer.sortingOrder = knifeSortingOrder;
        }

        // Временно отключаем коллайдер ножа чтобы не мешал сам себе
        if (_collider != null)
        {
            _collider.enabled = false;
        }
    }

    void ReturnKnifeToNormal()
    {
        if (_spriteRenderer != null)
        {
            _spriteRenderer.sortingLayerName = _originalSortingLayer;
            _spriteRenderer.sortingOrder = _originalSortingOrder;
        }

        // Включаем коллайдер обратно
        if (_collider != null)
        {
            _collider.enabled = true;
        }
    }
}