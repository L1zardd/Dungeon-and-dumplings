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

    public float timeToSlice = 1.5f;
    private float _sliceProgress = 0f;
    private bool _isSliced = false;

    private SpriteRenderer _spriteRenderer;
    private BoxCollider2D _boxCollider;
    private Vector3 _originalScale;

    // Публичное свойство для проверки разрезан ли овощ
    public bool IsSliced => _isSliced;

    void Start()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _boxCollider = GetComponent<BoxCollider2D>();
        _originalScale = transform.localScale;

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

        Debug.Log(gameObject.name + " разрезан!");
    }

    private bool IsBeingDragged()
    {
        DraggableVegetable draggable = GetComponent<DraggableVegetable>();
        return draggable != null && draggable.IsDragging;
    }

    public void ResetVegetable()
    {
        InitializeVegetable();
    }
}