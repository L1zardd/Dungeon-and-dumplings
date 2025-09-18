using UnityEngine;
using UnityEngine.SceneManagement;
using static ServingArea;

public class Dish : MonoBehaviour
{
    [Header("Dish Settings")]
    public string dishName;
    public int scoreValue = 100;
    public bool isReadyToServe = false;
    public int moveSpeed = 0;

    [Header("Effects")]
    public ParticleSystem readyEffect;
    public AudioClip readySound;
    public GameObject clickIndicator;

    [HideInInspector]
    public CookingPot sourcePot;

    private AudioSource _audioSource;
    private bool _isMovingToServe = false;

    void Start()
    {
        _audioSource = GetComponent<AudioSource>();

        if (isReadyToServe)
        {
            ShowReadyIndicator();
        }
    }

    void Update()
    {
        // Мигающий эффект для готового блюда
        if (isReadyToServe && clickIndicator != null)
        {
            clickIndicator.SetActive(Mathf.PingPong(Time.time, 1f) > 0.5f);
        }
    }

    void OnMouseDown()
    {
        if (isReadyToServe && !_isMovingToServe)
        {
            MoveToServingArea();
        }
    }

    void ShowReadyIndicator()
    {
        // Эффект готовности
        if (readyEffect != null)
        {
            readyEffect.Play();
        }

        if (readySound != null && _audioSource != null)
        {
            _audioSource.PlayOneShot(readySound);
        }

        Debug.Log("Блюдо готово к подаче: " + dishName);
    }

    public void MoveToServingArea()
    {
        _isMovingToServe = true;

        // Находим serving area
        ServingArea servingArea = FindObjectOfType<ServingArea>();
        if (servingArea != null && servingArea.HasFreeSpots())
        {
            servingArea.PlaceDishInArea(gameObject);
        }
        else
        {
            Debug.Log("Нет свободных мест в зоне подачи!");
            _isMovingToServe = false;
        }
    }

    // Метод вызывается когда блюдо размещено в serving area
    public void OnServed()
    {
        isReadyToServe = false;
        if (clickIndicator != null)
        {
            clickIndicator.SetActive(false);
        }

        // Начисляем очки
        AddScore(scoreValue);
    }

    void AddScore(int points)
    {
        // Простая система очков
        ScoreManager scoreManager = FindObjectOfType<ScoreManager>();
        if (scoreManager != null)
        {
            scoreManager.AddScore(points);
        }
        else
        {
            Debug.Log("+ " + points + " очков за: " + dishName);
        }
    }

    System.Collections.IEnumerator MoveDishToSpot(GameObject dish, ServingSpot spot)
    {
        // Отключаем кликабельность на время перемещения
        Dish dishComponent = dish.GetComponent<Dish>();
        if (dishComponent != null)
        {
            dishComponent.isReadyToServe = false;
        }

        // Отключаем коллайдеры
        Collider2D col = dish.GetComponent<Collider2D>();
        if (col != null) col.enabled = false;

        // Плавное перемещение
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

        // Восстанавливаем коллайдер
        if (col != null) col.enabled = true;

        // Уведомляем блюдо что оно подано
        if (dishComponent != null)
        {
            dishComponent.OnServed();
        }

      
    }
}