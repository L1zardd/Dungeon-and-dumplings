using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    // ������ �� Canvas ����
    public Canvas menuCanvas;

    // ���� ���������� ����
    private bool isMenuActive = false;

    void Start()
    {
        // �������� ���� ��� ������ ����
        if (menuCanvas != null)
        {
            menuCanvas.gameObject.SetActive(false);
            isMenuActive = false;
        }
    }

    void Update()
    {
        // ��������� ������� ������� Esc
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ToggleMenu();
        }
    }

    // ������������ ��������� ����
    private void ToggleMenu()
    {
        if (menuCanvas != null)
        {
            isMenuActive = !isMenuActive;
            menuCanvas.gameObject.SetActive(isMenuActive);

            // ����� �������� ����� ���� ��� �������� ����
            Time.timeScale = isMenuActive ? 0f : 1f;
        }
    }
}