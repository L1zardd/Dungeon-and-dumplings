using UnityEngine;

public class Vegetable : MonoBehaviour
{
    public Sprite wholeSprite;
    public Sprite slicedSprite;

    [Header("Size Settings")]
    public Vector2 wholeSize = Vector2.one;
    public Vector2 slicedSize = Vector2.one;

    [Header("Scale Settings")]
    public Vector3 wholeScale = Vector3.one;
    public Vector3 slicedScale = Vector3.one;

    [Header("Dragging Settings")]
    public bool canBeDraggedWhenWhole = true;
    public bool canBeDraggedWhenSliced = false;
    public bool returnToInitialPosition = true;

    [Header("Sorting Settings")]
    public string vegetableSortingLayer = "Vegetables";
    public string slicedSortingLayer = "Vegetables";
    public int vegetableSortingOrder = 1;
    public int slicedSortingOrder = 0;

    public float timeToSlice = 1.5f;
    private float _sliceProgress = 0f;
    private bool _isSliced = false;

    private SpriteRenderer _spriteRenderer;
    private BoxCollider2D _boxCollider;
    private DraggableObject _draggable;
    private Vector3 _originalScale;

    void Start()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _boxCollider = GetComponent<BoxCollider2D>();
        _draggable = GetComponent<DraggableObject>();
        _originalScale = transform.localScale;

        // Устанавливаем слой овоща
        gameObject.layer = LayerMask.NameToLayer("Vegetable");

        InitializeVegetable();
    }

    void InitializeVegetable()
    {
        if (wholeSprite != null)
        {
            _spriteRenderer.sprite = wholeSprite;
            if (_boxCollider != null)
            {
                _boxCollider.size = wholeSize;
            }
            transform.localScale = Vector3.Scale(_originalScale, wholeScale);
        }

        _isSliced = false;
        _sliceProgress = 0f;

        // Настраиваем сортировку
        if (_spriteRenderer != null)
        {
            _spriteRenderer.sortingLayerName = vegetableSortingLayer;
            _spriteRenderer.sortingOrder = vegetableSortingOrder;
        }

        // Настраиваем возможность перетаскивания
        if (_draggable != null)
        {
            _draggable.canBeDragged = canBeDraggedWhenWhole;
            _draggable.returnToInitialPosition = returnToInitialPosition;
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Knife") && !_isSliced && !IsBeingDragged())
        {
            Debug.Log("Начинаем резать " + gameObject.name);
        }
    }

    void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("Knife") && !_isSliced && !IsBeingDragged())
        {
            _sliceProgress += Time.deltaTime;

            if (_sliceProgress >= timeToSlice)
            {
                SliceVegetable();
            }
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Knife") && !_isSliced)
        {
            Debug.Log("Прервали резку " + gameObject.name);
        }
    }

    void SliceVegetable()
    {
        _isSliced = true;

        if (slicedSprite != null)
        {
            _spriteRenderer.sprite = slicedSprite;
            if (_boxCollider != null)
            {
                _boxCollider.size = slicedSize;
            }
            transform.localScale = Vector3.Scale(_originalScale, slicedScale);
        }

        // Меняем слой на разрезанный овощ
        gameObject.layer = LayerMask.NameToLayer("SlicedVegetable");

        // Меняем сортировку
        if (_spriteRenderer != null)
        {
            _spriteRenderer.sortingLayerName = slicedSortingLayer;
            _spriteRenderer.sortingOrder = slicedSortingOrder;
        }

        // Меняем настройки перетаскивания
        if (_draggable != null)
        {
            _draggable.canBeDragged = canBeDraggedWhenSliced;
            _draggable.SetInitialPosition(transform.position);
        }

        Debug.Log(gameObject.name + " разрезан!");
    }

    private bool IsBeingDragged()
    {
        return _draggable != null && _draggable.IsDragging;
    }

    public void ResetVegetable()
    {
        InitializeVegetable();
    }
}