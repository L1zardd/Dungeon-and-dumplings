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
        // ��������� ���������
        LoadSettings();

        // ����������� ����������� �������
        SetupUIListeners();

        // ��������� ��������� ���������
        ApplyAllSettings();
    }

    private void SetupUIListeners()
    {
        // ���������� ��� ������
        if (musicGroup.scrollbar != null)
        {
            musicGroup.scrollbar.onValueChanged.AddListener(value => {
                musicGroup.volume = value;
                SetGroupVolume(musicGroup);
            });
        }

        // ����������� ��� �������� �����
        foreach (var group in soundGroups)
        {
            if (group.scrollbar != null)
            {
                group.scrollbar.onValueChanged.AddListener(value => {
                    group.volume = value;
                    SetGroupVolume(group);

                    // ����������� �������� ���� ��� ���������
                    PlayTestSound(group);
                });
            }
        }

        // ������ ����������
        if (saveButton != null)
            saveButton.onClick.AddListener(SaveSettings);

        // ������ ������
        if (resetButton != null)
            resetButton.onClick.AddListener(ResetSettings);
    }

    // ��������� ��������� ��� ������
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

    // ������������ ��������� �����
    private void PlayTestSound(SoundGroup group)
    {
        if (testSounds != null && testSounds.Length > 0 && group.volume > 0)
        {
            // ������� ������ ��������� AudioSource � ������
            var audioSource = group.audioSources.FirstOrDefault(asource => asource != null);
            if (audioSource != null)
            {
                // ����������� ��������� �������� ����
                AudioClip testSound = testSounds[Random.Range(0, testSounds.Length)];
                audioSource.PlayOneShot(testSound);
            }
        }
    }

    // ���������� ���� ��������
    public void SaveSettings()
    {
        // ��������� ������
        PlayerPrefs.SetFloat("MusicVolume", musicGroup.volume);

        // ��������� �������� ������
        for (int i = 0; i < soundGroups.Count; i++)
        {
            PlayerPrefs.SetFloat($"SoundGroup_{i}_Volume", soundGroups[i].volume);
        }

        PlayerPrefs.Save();
    }

    // �������� ��������
    private void LoadSettings()
    {
        // ��������� ������
        musicGroup.volume = PlayerPrefs.GetFloat("MusicVolume", 0.7f);
        if (musicGroup.scrollbar != null)
            musicGroup.scrollbar.value = musicGroup.volume;

        // ��������� �������� ������
        for (int i = 0; i < soundGroups.Count; i++)
        {
            soundGroups[i].volume = PlayerPrefs.GetFloat($"SoundGroup_{i}_Volume", 0.7f);
            if (soundGroups[i].scrollbar != null)
                soundGroups[i].scrollbar.value = soundGroups[i].volume;
        }
    }

    // ���������� ���� ��������
    private void ApplyAllSettings()
    {
        SetGroupVolume(musicGroup);
        foreach (var group in soundGroups)
        {
            SetGroupVolume(group);
        }
    }

    // ����� ��������
    public void ResetSettings()
    {
        // ���������� ������
        musicGroup.volume = 0.7f;
        if (musicGroup.scrollbar != null)
            musicGroup.scrollbar.value = musicGroup.volume;

        // ���������� �������� ������
        foreach (var group in soundGroups)
        {
            group.volume = 0.7f;
            if (group.scrollbar != null)
                group.scrollbar.value = group.volume;
        }

        ApplyAllSettings();
        SaveSettings();
    }

    // ����� ��� ������������ ����������� ����� � ������
    public void PlaySoundInGroup(string groupName, AudioClip clip)
    {
        var group = soundGroups.Find(g => g.groupName == groupName);
        if (group != null && group.audioSources.Length > 0)
        {
            var audioSource = group.audioSources[0]; // ���������� ������ ��������
            audioSource.PlayOneShot(clip);
        }
    }

    // ����� ��� ��������� ���� ������ � ������
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