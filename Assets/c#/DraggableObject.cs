using UnityEngine;

public class DraggableObject : MonoBehaviour
{
    private Vector3 _screenPoint;
    private Vector3 _offset;
    private bool _isDragging = false;
    private Rigidbody2D _rigidbody;
    private Collider2D _collider;

    [Header("Dragging Settings")]
    public bool canBeDragged = true;
    public bool returnToInitialPosition = false;
    public float returnSpeed = 5f;

    private Vector3 _initialPosition;
    private bool _isReturning = false;

    public bool IsDragging => _isDragging;

    void Start()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
        _collider = GetComponent<Collider2D>();
        _initialPosition = transform.position;
    }

    void Update()
    {
        if (_isReturning && returnToInitialPosition)
        {
            transform.position = Vector3.Lerp(transform.position, _initialPosition, returnSpeed * Time.deltaTime);

            if (Vector3.Distance(transform.position, _initialPosition) < 0.1f)
            {
                transform.position = _initialPosition;
                _isReturning = false;

                if (_rigidbody != null)
                {
                    _rigidbody.velocity = Vector2.zero;
                    _rigidbody.angularVelocity = 0f;
                }
            }
            return;
        }

        if (_isDragging)
        {
            Vector3 curScreenPoint = new Vector3(Input.mousePosition.x, Input.mousePosition.y, _screenPoint.z);
            Vector3 curPosition = Camera.main.ScreenToWorldPoint(curScreenPoint) + _offset;

            if (_rigidbody != null && !_rigidbody.isKinematic)
            {
                _rigidbody.MovePosition(curPosition);
            }
            else
            {
                transform.position = curPosition;
            }
        }
    }

    void OnMouseDown()
    {
        if (!canBeDragged) return;

        _isDragging = true;
        _isReturning = false;
        _screenPoint = Camera.main.WorldToScreenPoint(transform.position);
        _offset = transform.position - Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, _screenPoint.z));

        if (_collider != null)
        {
            _collider.enabled = false;
        }

        BringToFront();
    }

    void OnMouseUp()
    {
        if (!_isDragging) return;

        _isDragging = false;

        if (_collider != null)
        {
            _collider.enabled = true;
        }

        if (returnToInitialPosition)
        {
            _isReturning = true;
        }
    }

    void BringToFront()
    {
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            spriteRenderer.sortingOrder = 10;
        }
    }

    public void StartDragging()
    {
        OnMouseDown();
    }

    public void StopDragging()
    {
        OnMouseUp();
    }

    public void SetInitialPosition(Vector3 newPosition)
    {
        _initialPosition = newPosition;
    }
}