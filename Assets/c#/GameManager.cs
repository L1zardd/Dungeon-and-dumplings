using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    // Ссылка на Canvas меню
    public Canvas menuCanvas;

    // Флаг активности меню
    private bool isMenuActive = false;

    void Start()
    {
        // Скрываем меню при старте игры
        if (menuCanvas != null)
        {
            menuCanvas.gameObject.SetActive(false);
            isMenuActive = false;
        }
    }

    void Update()
    {
        // Проверяем нажатие клавиши Esc
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ToggleMenu();
        }
    }

    // Переключение видимости меню
    private void ToggleMenu()
    {
        if (menuCanvas != null)
        {
            isMenuActive = !isMenuActive;
            menuCanvas.gameObject.SetActive(isMenuActive);

            // Можно добавить паузу игры при открытии меню
            Time.timeScale = isMenuActive ? 0f : 1f;
        }
    }
}