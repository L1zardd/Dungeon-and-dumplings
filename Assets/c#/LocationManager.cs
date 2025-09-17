using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class LocationManager : MonoBehaviour
{
    // Массив для хранения всех точек-локаций
    public Transform[] locations;

    // Кнопки управления для подсветки
    public Button buttonRight;
    public Button buttonLeft;
    public Button buttonUp;
    public Button buttonDown;
    public Button buttonBasement;

    // Цвета для кнопок
    public Color activeColor = Color.green;
    public Color inactiveColor = Color.gray;

    // Индекс текущей локации
    private int currentLocationIndex = 0;

    // Ссылка на основную камеру
    public Camera mainCamera;

    // Скорость перемещения камеры
    public float cameraMoveSpeed = 5f;

    // Флаг, нужно ли плавное перемещение
    private bool isMoving = false;
    private Vector3 targetCameraPosition;

    // Словарь для хранения доступных направлений для каждой локации
    private Dictionary<int, List<string>> availableDirections = new Dictionary<int, List<string>>()
    {
        { 0, new List<string> { "right" } },          // Зал: вправо
        { 1, new List<string> { "left", "right", "down" } }, // Касса: влево, вправо, вниз
        { 2, new List<string> { "left", "down" } },   // Кухня: влево, вниз
        { 3, new List<string> { "left", "up" } },     // Склад: влево, вверх
        { 4, new List<string> { "up", "right" } }      // Спуск: вверх, вниз
    };

    void Start()
    {
        if (mainCamera == null)
            mainCamera = Camera.main;

        if (locations.Length > 0)
        {
            mainCamera.transform.position = locations[currentLocationIndex].position;
            UpdateButtonColors(); // Обновляем цвета кнопок при старте
        }
    }

    void Update()
    {
        if (isMoving)
        {
            mainCamera.transform.position = Vector3.Lerp(
                mainCamera.transform.position,
                targetCameraPosition,
                cameraMoveSpeed * Time.deltaTime
            );

            if (Vector3.Distance(mainCamera.transform.position, targetCameraPosition) < 0.05f)
            {
                mainCamera.transform.position = targetCameraPosition;
                isMoving = false;
                UpdateButtonColors(); // Обновляем кнопки после завершения движения
            }
        }
    }

    // Обновление цветов кнопок в зависимости от текущей локации
    private void UpdateButtonColors()
    {
        // Получаем доступные направления для текущей локации
        List<string> directions = availableDirections[currentLocationIndex];

        // Устанавливаем цвета для кнопок
        buttonRight.GetComponent<Image>().color = directions.Contains("right") ? activeColor : inactiveColor;
        buttonLeft.GetComponent<Image>().color = directions.Contains("left") ? activeColor : inactiveColor;
        buttonUp.GetComponent<Image>().color = directions.Contains("up") ? activeColor : inactiveColor;
        buttonDown.GetComponent<Image>().color = directions.Contains("down") ? activeColor : inactiveColor;

        // Особый случай для кнопки подземелья (доступна только с кухни)
        buttonBasement.GetComponent<Image>().color = (currentLocationIndex == 4) ? activeColor : inactiveColor;
    }

    public void MoveToLocation(int locationIndex)
    {
        if (locationIndex >= 0 && locationIndex < locations.Length && locations[locationIndex] != null)
        {
            targetCameraPosition = locations[locationIndex].position;
            isMoving = true;
            currentLocationIndex = locationIndex;
        }
    }

    // Переработанные методы перемещения с правильной логикой
    public void MoveRight()
    {
        switch (currentLocationIndex)
        {
            case 0: MoveToLocation(1); break; // Зал → Касса
            case 1: MoveToLocation(2); break; // Касса → Кухня
            case 4: MoveToLocation(3); break; 
            default: break; // В других локациях право не работает
        }
    }

    public void MoveLeft()
    {
        switch (currentLocationIndex)
        {
            case 1: MoveToLocation(0); break; // Касса → Зал
            case 2: MoveToLocation(1); break; // Кухня → Касса
            case 3: MoveToLocation(4); break; // Склад → Спуск
            default: break;
        }
    }

    public void MoveDown()
    {
        switch (currentLocationIndex)
        {
            case 1: MoveToLocation(4); break; // Касса → Склад
            case 2: MoveToLocation(3); break; // Кухня → Склад
            default: break;
        }
    }

    public void MoveUp()
    {
        switch (currentLocationIndex)
        {
            case 3: MoveToLocation(2); break; // Склад → Кухня
            case 4: MoveToLocation(1); break; // Спуск → Кухня
            default: break;
        }
    }

    public void PlayGame()
    {
        SceneManager.LoadScene(1);
        print(1);
    }
}