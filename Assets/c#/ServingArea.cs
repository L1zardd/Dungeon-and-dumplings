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
    public Vector3[] servingPositions; // Позиции для блюд

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

        // Если позиции не заданы в инспекторе, создаем автоматически
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
        // Проверяем что это готовое блюдо (можно добавить тег "Dish")
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
            Debug.Log("Нет свободных мест для блюд!");
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
        // Отключаем физику и коллайдеры на время перемещения
        Rigidbody2D rb = dish.GetComponent<Rigidbody2D>();
        Collider2D col = dish.GetComponent<Collider2D>();

        if (rb != null) rb.simulated = false;
        if (col != null) col.enabled = false;

        // Отключаем перетаскивание
        DraggableVegetable draggable = dish.GetComponent<DraggableVegetable>();
        DraggableVegetable draggableVeg = dish.GetComponent<DraggableVegetable>();
        if (draggable != null) draggable.canBeDragged = false;
        if (draggableVeg != null) draggableVeg.canBeDragged = false;

        // Плавное перемещение к spot
        Vector3 startPosition = dish.transform.position;
        float journey = 0f;

        while (journey < 1f)
        {
            journey += Time.deltaTime * moveSpeed;
            dish.transform.position = Vector3.Lerp(startPosition, spot.position, journey);
            yield return null;
        }

        // Фиксируем позицию
        dish.transform.position = spot.position;

        // Помечаем spot как занятый
        spot.isOccupied = true;
        spot.currentDish = dish;

        // Восстанавливаем коллайдер (но не физику)
        if (col != null) col.enabled = true;

        // Проигрываем звук
        if (placeDishSound != null && _audioSource != null)
        {
            _audioSource.PlayOneShot(placeDishSound);
        }

        // Визуальный эффект
        if (highlightEffect != null)
        {
            Instantiate(highlightEffect, spot.position, Quaternion.identity);
        }

        Debug.Log("Блюдо размещено на позиции: " + spot.position);
    }

    // Освободить spot (например, когда блюдо забирают)
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

    // Методы для UI и управления
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

    // Визуализация в редакторе
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