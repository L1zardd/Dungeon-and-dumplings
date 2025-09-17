using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.SceneManagement;
using UnityEngine.Rendering;

public class AdvancedSettingsManager : MonoBehaviour
{
    [System.Serializable]
    public class SoundGroup
    {
        public string groupName;
        public AudioSource[] audioSources;
        public Scrollbar scrollbar;
        [Range(0f, 1f)] public float volume = 0.7f;
    }

    [System.Serializable]
    public class FullscreenToggleSettings
    {
        public RectTransform checkmark;
        public float leftPositionOn = 37f;
        public float rightPositionOn = 0f;
        public float leftPositionOff = 0f;
        public float rightPositionOff = 37f;
        public float animationSpeed = 5f;
    }

    [Header("Sound Groups")]
    public SoundGroup musicGroup;
    public List<SoundGroup> soundGroups = new List<SoundGroup>();

    [Header("Graphics Settings")]
    public Dropdown resolutionDropdown;
    public Dropdown qualityDropdown;
    public Toggle fullscreenToggle;
    public FullscreenToggleSettings toggleSettings;

    [Header("Test Sounds")]
    public AudioClip[] testSounds;

    private Resolution[] resolutions;
    private bool isAnimatingCheckmark = false;
    private Vector2 targetCheckmarkPosition;

    void Start()
    {
        InitializeResolutionDropdown();
        InitializeQualityDropdown();
        LoadAllSettings();
        SetupUIListeners();
        ApplyAllSettings();

        // Устанавливаем начальную позицию checkmark
        UpdateCheckmarkPositionImmediate(fullscreenToggle.isOn);
    }

    void Update()
    {
        // Плавная анимация checkmark
        if (isAnimatingCheckmark && toggleSettings.checkmark != null)
        {
            toggleSettings.checkmark.offsetMin = Vector2.Lerp(
                toggleSettings.checkmark.offsetMin,
                new Vector2(targetCheckmarkPosition.x, toggleSettings.checkmark.offsetMin.y),
                toggleSettings.animationSpeed * Time.unscaledDeltaTime
            );

            toggleSettings.checkmark.offsetMax = Vector2.Lerp(
                toggleSettings.checkmark.offsetMax,
                new Vector2(-targetCheckmarkPosition.y, toggleSettings.checkmark.offsetMax.y),
                toggleSettings.animationSpeed * Time.unscaledDeltaTime
            );

            // Проверяем завершение анимации
            if (Vector2.Distance(toggleSettings.checkmark.offsetMin, new Vector2(targetCheckmarkPosition.x, toggleSettings.checkmark.offsetMin.y)) < 0.1f)
            {
                isAnimatingCheckmark = false;
            }
        }
    }

    private void InitializeResolutionDropdown()
    {
        if (resolutionDropdown != null)
        {
            resolutionDropdown.ClearOptions();
            List<string> options = new List<string>();
            resolutions = Screen.resolutions;
            int currentResolutionIndex = 0;

            for (int i = 0; i < resolutions.Length; i++)
            {
                string option = resolutions[i].width + "x" + resolutions[i].height + " " + resolutions[i].refreshRateRatio + "Hz";
                options.Add(option);
                if (resolutions[i].width == Screen.currentResolution.width &&
                    resolutions[i].height == Screen.currentResolution.height)
                    currentResolutionIndex = i;
            }

            resolutionDropdown.AddOptions(options);
            resolutionDropdown.value = PlayerPrefs.GetInt("ResolutionPreference", currentResolutionIndex);
            resolutionDropdown.RefreshShownValue();
        }
    }

    private void InitializeQualityDropdown()
    {
        if (qualityDropdown != null)
        {
            qualityDropdown.value = PlayerPrefs.GetInt("QualitySettingsPreference", QualitySettings.GetQualityLevel());
            qualityDropdown.RefreshShownValue();
        }
    }

    private void SetupUIListeners()
    {
        // Обработчики для звуковых групп
        if (musicGroup.scrollbar != null)
        {
            musicGroup.scrollbar.onValueChanged.AddListener(value => {
                musicGroup.volume = value;
                SetGroupVolume(musicGroup);
                SaveSettings();
            });
        }

        foreach (var group in soundGroups)
        {
            if (group.scrollbar != null)
            {
                group.scrollbar.onValueChanged.AddListener(value => {
                    group.volume = value;
                    SetGroupVolume(group);
                    PlayTestSound(group);
                    SaveSettings();
                });
            }
        }

        // Обработчики для графических настроек
        if (resolutionDropdown != null)
        {
            resolutionDropdown.onValueChanged.AddListener(value => {
                SetResolution(value);
                SaveSettings();
            });
        }

        if (qualityDropdown != null)
        {
            qualityDropdown.onValueChanged.AddListener(value => {
                SetQuality(value);
                SaveSettings();
            });
        }

        if (fullscreenToggle != null)
        {
            fullscreenToggle.onValueChanged.AddListener(value => {
                SetFullscreen(value);
                AnimateCheckmark(value);
                SaveSettings();
            });
        }
    }

    // Анимация checkmark
    private void AnimateCheckmark(bool isOn)
    {
        if (toggleSettings.checkmark != null)
        {
            isAnimatingCheckmark = true;
            targetCheckmarkPosition = isOn ?
                new Vector2(toggleSettings.leftPositionOn, toggleSettings.rightPositionOn) :
                new Vector2(toggleSettings.leftPositionOff, toggleSettings.rightPositionOff);
        }
    }

    // Мгновенное обновление позиции checkmark (без анимации)
    private void UpdateCheckmarkPositionImmediate(bool isOn)
    {
        if (toggleSettings.checkmark != null)
        {
            Vector2 position = isOn ?
                new Vector2(toggleSettings.leftPositionOn, toggleSettings.rightPositionOn) :
                new Vector2(toggleSettings.leftPositionOff, toggleSettings.rightPositionOff);

            toggleSettings.checkmark.offsetMin = new Vector2(position.x, toggleSettings.checkmark.offsetMin.y);
            toggleSettings.checkmark.offsetMax = new Vector2(-position.y, toggleSettings.checkmark.offsetMax.y);
        }
    }

    // Графические настройки
    public void SetFullscreen(bool isFullscreen)
    {
        Screen.fullScreen = isFullscreen;
    }

    public void SetResolution(int resolutionIndex)
    {
        if (resolutions != null && resolutionIndex >= 0 && resolutionIndex < resolutions.Length)
        {
            Resolution resolution = resolutions[resolutionIndex];
            Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
        }
    }

    public void SetQuality(int qualityIndex)
    {
        QualitySettings.SetQualityLevel(qualityIndex);
    }

    // Звуковые настройки
    private void SetGroupVolume(SoundGroup group)
    {
        foreach (var audioSource in group.audioSources)
        {
            if (audioSource != null)
            {
                audioSource.volume = group.volume;
            }
        }
    }

    private void PlayTestSound(SoundGroup group)
    {
        if (testSounds != null && testSounds.Length > 0 && group.volume > 0)
        {
            var audioSource = group.audioSources.FirstOrDefault(asource => asource != null);
            if (audioSource != null)
            {
                AudioClip testSound = testSounds[Random.Range(0, testSounds.Length)];
                audioSource.PlayOneShot(testSound);
            }
        }
    }

    // Сохранение всех настроек
    private void SaveSettings()
    {
        // Сохраняем звуковые настройки
        PlayerPrefs.SetFloat("MusicVolume", musicGroup.volume);

        for (int i = 0; i < soundGroups.Count; i++)
        {
            PlayerPrefs.SetFloat($"SoundGroup_{i}_Volume", soundGroups[i].volume);
        }

        // Сохраняем графические настройки
        if (resolutionDropdown != null)
            PlayerPrefs.SetInt("ResolutionPreference", resolutionDropdown.value);

        if (qualityDropdown != null)
            PlayerPrefs.SetInt("QualitySettingsPreference", qualityDropdown.value);

        PlayerPrefs.SetInt("FullscreenPreference", System.Convert.ToInt32(Screen.fullScreen));

        PlayerPrefs.Save();
    }

    // Загрузка всех настроек
    private void LoadAllSettings()
    {
        // Загружаем звуковые настройки
        musicGroup.volume = PlayerPrefs.GetFloat("MusicVolume", 0.7f);
        if (musicGroup.scrollbar != null)
            musicGroup.scrollbar.value = musicGroup.volume;

        for (int i = 0; i < soundGroups.Count; i++)
        {
            soundGroups[i].volume = PlayerPrefs.GetFloat($"SoundGroup_{i}_Volume", 0.7f);
            if (soundGroups[i].scrollbar != null)
                soundGroups[i].scrollbar.value = soundGroups[i].volume;
        }

        // Загружаем графические настройки
        if (fullscreenToggle != null)
        {
            bool fullscreenValue = PlayerPrefs.GetInt("FullscreenPreference", 1) == 1;
            fullscreenToggle.isOn = fullscreenValue;
            UpdateCheckmarkPositionImmediate(fullscreenValue);
        }
    }

    // Применение всех настроек
    private void ApplyAllSettings()
    {
        // Применяем звуковые настройки
        SetGroupVolume(musicGroup);
        foreach (var group in soundGroups)
        {
            SetGroupVolume(group);
        }

        // Применяем графические настройки
        if (fullscreenToggle != null)
            SetFullscreen(fullscreenToggle.isOn);

        if (resolutionDropdown != null)
            SetResolution(resolutionDropdown.value);

        if (qualityDropdown != null)
            SetQuality(qualityDropdown.value);
    }

    // Дополнительные методы для управления звуком
    public void PlaySoundInGroup(string groupName, AudioClip clip)
    {
        var group = soundGroups.Find(g => g.groupName == groupName);
        if (group != null && group.audioSources.Length > 0)
        {
            var audioSource = group.audioSources[0];
            audioSource.PlayOneShot(clip);
        }
    }

    public void StopGroup(string groupName)
    {
        var group = soundGroups.Find(g => g.groupName == groupName);
        if (group != null)
        {
            foreach (var audioSource in group.audioSources)
            {
                audioSource.Stop();
            }
        }
    }

    // Метод для сброса настроек
    public void ResetToDefault()
    {
        // Сброс звуковых настроек
        musicGroup.volume = 0.7f;
        if (musicGroup.scrollbar != null)
            musicGroup.scrollbar.value = musicGroup.volume;

        foreach (var group in soundGroups)
        {
            group.volume = 0.7f;
            if (group.scrollbar != null)
                group.scrollbar.value = group.volume;
        }

        // Сброс графических настроек
        if (qualityDropdown != null)
            qualityDropdown.value = 3;

        if (fullscreenToggle != null)
        {
            fullscreenToggle.isOn = true;
            UpdateCheckmarkPositionImmediate(true);
        }

        ApplyAllSettings();
        SaveSettings();
    }
}