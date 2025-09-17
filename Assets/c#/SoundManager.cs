using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;

public class AdvancedSoundManager : MonoBehaviour
{
    [System.Serializable]
    public class SoundGroup
    {
        public string groupName;
        public AudioSource[] audioSources;
        public Scrollbar scrollbar;
        [Range(0f, 1f)] public float volume = 0.7f;
    }

    [Header("Sound Groups")]
    public SoundGroup musicGroup;
    public List<SoundGroup> soundGroups = new List<SoundGroup>();

    [Header("UI Elements")]
    public Button saveButton;
    public Button resetButton;

    [Header("Test Sounds")]
    public AudioClip[] testSounds;

    void Start()
    {
        // Загружаем настройки
        LoadSettings();

        // Настраиваем обработчики событий
        SetupUIListeners();

        // Применяем начальные настройки
        ApplyAllSettings();
    }

    private void SetupUIListeners()
    {
        // Обработчик для музыки
        if (musicGroup.scrollbar != null)
        {
            musicGroup.scrollbar.onValueChanged.AddListener(value => {
                musicGroup.volume = value;
                SetGroupVolume(musicGroup);
            });
        }

        // Обработчики для звуковых групп
        foreach (var group in soundGroups)
        {
            if (group.scrollbar != null)
            {
                group.scrollbar.onValueChanged.AddListener(value => {
                    group.volume = value;
                    SetGroupVolume(group);

                    // Проигрываем тестовый звук при изменении
                    PlayTestSound(group);
                });
            }
        }

        // Кнопка сохранения
        if (saveButton != null)
            saveButton.onClick.AddListener(SaveSettings);

        // Кнопка сброса
        if (resetButton != null)
            resetButton.onClick.AddListener(ResetSettings);
    }

    // Установка громкости для группы
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

    // Проигрывание тестового звука
    private void PlayTestSound(SoundGroup group)
    {
        if (testSounds != null && testSounds.Length > 0 && group.volume > 0)
        {
            // Находим первый доступный AudioSource в группе
            var audioSource = group.audioSources.FirstOrDefault(asource => asource != null);
            if (audioSource != null)
            {
                // Проигрываем случайный тестовый звук
                AudioClip testSound = testSounds[Random.Range(0, testSounds.Length)];
                audioSource.PlayOneShot(testSound);
            }
        }
    }

    // Сохранение всех настроек
    public void SaveSettings()
    {
        // Сохраняем музыку
        PlayerPrefs.SetFloat("MusicVolume", musicGroup.volume);

        // Сохраняем звуковые группы
        for (int i = 0; i < soundGroups.Count; i++)
        {
            PlayerPrefs.SetFloat($"SoundGroup_{i}_Volume", soundGroups[i].volume);
        }

        PlayerPrefs.Save();
    }

    // Загрузка настроек
    private void LoadSettings()
    {
        // Загружаем музыку
        musicGroup.volume = PlayerPrefs.GetFloat("MusicVolume", 0.7f);
        if (musicGroup.scrollbar != null)
            musicGroup.scrollbar.value = musicGroup.volume;

        // Загружаем звуковые группы
        for (int i = 0; i < soundGroups.Count; i++)
        {
            soundGroups[i].volume = PlayerPrefs.GetFloat($"SoundGroup_{i}_Volume", 0.7f);
            if (soundGroups[i].scrollbar != null)
                soundGroups[i].scrollbar.value = soundGroups[i].volume;
        }
    }

    // Применение всех настроек
    private void ApplyAllSettings()
    {
        SetGroupVolume(musicGroup);
        foreach (var group in soundGroups)
        {
            SetGroupVolume(group);
        }
    }

    // Сброс настроек
    public void ResetSettings()
    {
        // Сбрасываем музыку
        musicGroup.volume = 0.7f;
        if (musicGroup.scrollbar != null)
            musicGroup.scrollbar.value = musicGroup.volume;

        // Сбрасываем звуковые группы
        foreach (var group in soundGroups)
        {
            group.volume = 0.7f;
            if (group.scrollbar != null)
                group.scrollbar.value = group.volume;
        }

        ApplyAllSettings();
        SaveSettings();
    }

    // Метод для проигрывания конкретного звука в группе
    public void PlaySoundInGroup(string groupName, AudioClip clip)
    {
        var group = soundGroups.Find(g => g.groupName == groupName);
        if (group != null && group.audioSources.Length > 0)
        {
            var audioSource = group.audioSources[0]; // Используем первый источник
            audioSource.PlayOneShot(clip);
        }
    }

    // Метод для остановки всех звуков в группе
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
}