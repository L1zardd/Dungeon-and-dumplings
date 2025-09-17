using UnityEngine;

public class SettingsController : MonoBehaviour
{
    public GameObject settingsPanel;

    // ���������� �� ������ Settings
    public void OpenSettings()
    {
        if (settingsPanel != null && !settingsPanel.activeSelf)
        {
            settingsPanel.SetActive(true);
        }
    }

    // ���������� �� ������ Close � ������
    public void CloseSettings()
    {
        if (settingsPanel != null && settingsPanel.activeSelf)
        {
            settingsPanel.SetActive(false);
        }
    }

    // ������������� ����� ��� ������������
    public void ToggleSettings()
    {
        if (settingsPanel != null)
        {
            settingsPanel.SetActive(!settingsPanel.activeSelf);
        }
    }
}