using UnityEngine;

public class SettingsController : MonoBehaviour
{
    public GameObject settingsPanel;

    // Вызывается по кнопке Settings
    public void OpenSettings()
    {
        if (settingsPanel != null && !settingsPanel.activeSelf)
        {
            settingsPanel.SetActive(true);
        }
    }

    // Вызывается по кнопке Close в панели
    public void CloseSettings()
    {
        if (settingsPanel != null && settingsPanel.activeSelf)
        {
            settingsPanel.SetActive(false);
        }
    }

    // Универсальный метод для переключения
    public void ToggleSettings()
    {
        if (settingsPanel != null)
        {
            settingsPanel.SetActive(!settingsPanel.activeSelf);
        }
    }
}