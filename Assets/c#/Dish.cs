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
        // �������� ������ ��� �������� �����
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
        // ������ ����������
        if (readyEffect != null)
        {
            readyEffect.Play();
        }

        if (readySound != null && _audioSource != null)
        {
            _audioSource.PlayOneShot(readySound);
        }

        Debug.Log("����� ������ � ������: " + dishName);
    }

    public void MoveToServingArea()
    {
        _isMovingToServe = true;

        // ������� serving area
        ServingArea servingArea = FindObjectOfType<ServingArea>();
        if (servingArea != null && servingArea.HasFreeSpots())
        {
            servingArea.PlaceDishInArea(gameObject);
        }
        else
        {
            Debug.Log("��� ��������� ���� � ���� ������!");
            _isMovingToServe = false;
        }
    }

    // ����� ���������� ����� ����� ��������� � serving area
    public void OnServed()
    {
        isReadyToServe = false;
        if (clickIndicator != null)
        {
            clickIndicator.SetActive(false);
        }

        // ��������� ����
        AddScore(scoreValue);
    }

    void AddScore(int points)
    {
        // ������� ������� �����
        ScoreManager scoreManager = FindObjectOfType<ScoreManager>();
        if (scoreManager != null)
        {
            scoreManager.AddScore(points);
        }
        else
        {
            Debug.Log("+ " + points + " ����� ��: " + dishName);
        }
    }

    System.Collections.IEnumerator MoveDishToSpot(GameObject dish, ServingSpot spot)
    {
        // ��������� �������������� �� ����� �����������
        Dish dishComponent = dish.GetComponent<Dish>();
        if (dishComponent != null)
        {
            dishComponent.isReadyToServe = false;
        }

        // ��������� ����������
        Collider2D col = dish.GetComponent<Collider2D>();
        if (col != null) col.enabled = false;

        // ������� �����������
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

        // ��������������� ���������
        if (col != null) col.enabled = true;

        // ���������� ����� ��� ��� ������
        if (dishComponent != null)
        {
            dishComponent.OnServed();
        }

      
    }
}