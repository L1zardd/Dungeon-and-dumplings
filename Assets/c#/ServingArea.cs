using UnityEngine;
using System.Collections.Generic;

public class ServingArea : MonoBehaviour
{
    [System.Serializable]
    public class ServingSpot
    {
        public Vector3 position;
        public bool isOccupied;
        public GameObject currentDish;
    }

    [Header("Serving Settings")]
    public int maxDishes = 5;
    public float moveSpeed = 3f;
    public Vector3[] servingPositions; // ������� ��� ����

    [Header("Visual Settings")]
    public GameObject highlightEffect;
    public AudioClip placeDishSound;

    private List<ServingSpot> _servingSpots = new List<ServingSpot>();
    private AudioSource _audioSource;

    void Start()
    {
        _audioSource = GetComponent<AudioSource>();
        InitializeServingSpots();
    }

    void InitializeServingSpots()
    {
        _servingSpots = new List<ServingSpot>();

        // ���� ������� �� ������ � ����������, ������� �������������
        if (servingPositions == null || servingPositions.Length == 0)
        {
            CreateDefaultPositions();
        }

        foreach (Vector3 pos in servingPositions)
        {
            ServingSpot spot = new ServingSpot
            {
                position = transform.position + pos,
                isOccupied = false,
                currentDish = null
            };
            _servingSpots.Add(spot);
        }
    }

    void CreateDefaultPositions()
    {
        servingPositions = new Vector3[]
        {
            new Vector3(-2f, 0f, 0f),
            new Vector3(-1f, 0f, 0f),
            new Vector3(0f, 0f, 0f),
            new Vector3(1f, 0f, 0f),
            new Vector3(2f, 0f, 0f)
        };
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        // ��������� ��� ��� ������� ����� (����� �������� ��� "Dish")
        Dish dish = other.GetComponent<Dish>();
        if (dish != null)
        {
            PlaceDishInArea(other.gameObject);
        }
    }

    public void PlaceDishInArea(GameObject dish)
    {
        ServingSpot freeSpot = GetFreeServingSpot();
        if (freeSpot != null)
        {
            StartCoroutine(MoveDishToSpot(dish, freeSpot));
        }
        else
        {
            Debug.Log("��� ��������� ���� ��� ����!");
        }
    }

    ServingSpot GetFreeServingSpot()
    {
        foreach (ServingSpot spot in _servingSpots)
        {
            if (!spot.isOccupied)
            {
                return spot;
            }
        }
        return null;
    }

    System.Collections.IEnumerator MoveDishToSpot(GameObject dish, ServingSpot spot)
    {
        // ��������� ������ � ���������� �� ����� �����������
        Rigidbody2D rb = dish.GetComponent<Rigidbody2D>();
        Collider2D col = dish.GetComponent<Collider2D>();

        if (rb != null) rb.simulated = false;
        if (col != null) col.enabled = false;

        // ��������� ��������������
        DraggableVegetable draggable = dish.GetComponent<DraggableVegetable>();
        DraggableVegetable draggableVeg = dish.GetComponent<DraggableVegetable>();
        if (draggable != null) draggable.canBeDragged = false;
        if (draggableVeg != null) draggableVeg.canBeDragged = false;

        // ������� ����������� � spot
        Vector3 startPosition = dish.transform.position;
        float journey = 0f;

        while (journey < 1f)
        {
            journey += Time.deltaTime * moveSpeed;
            dish.transform.position = Vector3.Lerp(startPosition, spot.position, journey);
            yield return null;
        }

        // ��������� �������
        dish.transform.position = spot.position;

        // �������� spot ��� �������
        spot.isOccupied = true;
        spot.currentDish = dish;

        // ��������������� ��������� (�� �� ������)
        if (col != null) col.enabled = true;

        // ����������� ����
        if (placeDishSound != null && _audioSource != null)
        {
            _audioSource.PlayOneShot(placeDishSound);
        }

        // ���������� ������
        if (highlightEffect != null)
        {
            Instantiate(highlightEffect, spot.position, Quaternion.identity);
        }

        Debug.Log("����� ��������� �� �������: " + spot.position);
    }

    // ���������� spot (��������, ����� ����� ��������)
    public void FreeServingSpot(GameObject dish)
    {
        foreach (ServingSpot spot in _servingSpots)
        {
            if (spot.currentDish == dish)
            {
                spot.isOccupied = false;
                spot.currentDish = null;
                break;
            }
        }
    }

    // ������ ��� UI � ����������
    public int GetOccupiedSpotsCount()
    {
        int count = 0;
        foreach (ServingSpot spot in _servingSpots)
        {
            if (spot.isOccupied) count++;
        }
        return count;
    }

    public int GetFreeSpotsCount()
    {
        return _servingSpots.Count - GetOccupiedSpotsCount();
    }

    public bool HasFreeSpots()
    {
        return GetFreeSpotsCount() > 0;
    }

    // ������������ � ���������
    void OnDrawGizmosSelected()
    {
        if (servingPositions != null)
        {
            Gizmos.color = Color.green;
            foreach (Vector3 pos in servingPositions)
            {
                Gizmos.DrawWireCube(transform.position + pos, new Vector3(0.8f, 0.8f, 0f));
            }
        }
    }
}