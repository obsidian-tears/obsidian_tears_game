// Copyright © Pixel Crushers. All rights reserved.

using UnityEngine;
using UnityEngine.Audio;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace PixelCrushers.DialogueSystem.MenuSystem
{

    /// <summary>
    /// This script manages the OptionsPanel.
    /// </summary>
    public class Options : MonoBehaviour
    {

        [Header("Video")]
        public UnityEngine.UI.Toggle fullScreenToggle;
        public string fullScreenPrefsKey = "options.fullScreen";
        public UnityEngine.UI.Dropdown resolutionDropdown;
        public string resolutionPrefsKey = "options.resolution";
        public UnityEngine.UI.Dropdown graphicsQualityDropdown;
        public string graphicsQualityPrefsKey = "options.quality";

        [Header("Audio")]
        public AudioMixer mainMixer;
        public string musicVolumeMixerParameter = "musicVol";
        public string musicVolumePrefsKey = "options.musicVol";
        public UnityEngine.UI.Slider musicVolumeSlider;
        public string sfxVolumeMixerParameter = "sfxVol";
        public string sfxVolumePrefsKey = "options.sfxVol";
        public UnityEngine.UI.Slider sfxVolumeSlider;

        [Header("Subtitles")]
        public UnityEngine.UI.Toggle subtitles;
        public bool setNPCSubtitlesDuringLine = true;
        public bool setNPCSubtitlesWithResponseMenu = true;
        public bool setPCSubtitlesDuringLine = false;
        public string subtitlesPrefsKey = "options.subtitles";

        [Header("Languages")]
        public UnityEngine.UI.Dropdown languages;

        private bool m_started = false;
        private List<string> resolutionDropdownItems = new List<string>();

        private void Start()
        {
            m_started = true;
            RefreshMenuElements();
        }

        private void OnEnable()
        {
            if (m_started) RefreshMenuElements();
        }

        private void OnDisable()
        {
            m_started = false;
        }

        public void RefreshMenuElements()
        {
            RefreshResolutionDropdown();
            RefreshFullscreenToggle();
            RefreshGraphicsQualityDropdown();
            RefreshMusicVolumeSlider();
            RefreshSfxVolumeSlider();
            RefreshSubtitlesToggle();
            RefreshLanguagesDropdown();
        }

        private void RefreshFullscreenToggle()
        {
            fullScreenToggle.isOn = GetFullScreen();
        }

        private bool GetFullScreen()
        {
            return PlayerPrefs.HasKey(fullScreenPrefsKey) ? (PlayerPrefs.GetInt(fullScreenPrefsKey) == 1) : Screen.fullScreen;
        }

        public void SetFullScreen(bool on)
        {
            Screen.fullScreen = on;
            PlayerPrefs.SetInt(fullScreenPrefsKey, on ? 1 : 0);
            SetResolutionIndex(GetResolutionIndex());
            SelectNextFrame(fullScreenToggle);
        }

        // 2018-07-31: Saved resolution index is now index of cleaned-up dropdown list, 
        // not Screen.resolutions array which contains duplicates.

        private string GetResolutionString(Resolution resolution)
        {
            return (resolution.refreshRate > 0) ? (resolution.width + "x" + resolution.height + " " + resolution.refreshRate + "Hz")
                : (resolution.width + "x" + resolution.height);
        }

        private void RefreshResolutionDropdownItems()
        {
            resolutionDropdownItems.Clear();
            var uniqueResolutions = Screen.resolutions.Distinct();
            foreach (var resolution in uniqueResolutions)
            {
                resolutionDropdownItems.Add(GetResolutionString(resolution));
            }
        }

        private int GetCurrentResolutionDropdownIndex()
        {
            var currentString = GetResolutionString(Screen.currentResolution);
            for (int i = 0; i < resolutionDropdownItems.Count; i++)
            {
                if (string.Equals(resolutionDropdownItems[i], currentString)) return i;
            }
            return 0;
        }

        private int ResolutionDropdownIndexToScreenResolutionsIndex(int dropdownIndex)
        {
            if (0 <= dropdownIndex && dropdownIndex < resolutionDropdownItems.Count)
            {
                var dropdownString = resolutionDropdownItems[dropdownIndex];
                for (int i = 0; i < Screen.resolutions.Length; i++)
                {
                    if (string.Equals(GetResolutionString(Screen.resolutions[i]), dropdownString)) return i;
                }
            }
            // If we don't find a match, return current resolution's index:
            for (int i = 0; i < Screen.resolutions.Length; i++)
            {
                if (Equals(Screen.resolutions[i], Screen.currentResolution)) return i;
            }
            return 0;
        }

        private void RefreshResolutionDropdown()
        {
            if (PlayerPrefs.HasKey(resolutionPrefsKey)) SetResolutionIndex(PlayerPrefs.GetInt(resolutionPrefsKey));
            RefreshResolutionDropdownItems();
            resolutionDropdown.ClearOptions();
            resolutionDropdown.AddOptions(resolutionDropdownItems);
            var dropdownIndex = Mathf.Clamp(GetResolutionIndex(), 0, resolutionDropdownItems.Count - 1);
            resolutionDropdown.value = dropdownIndex;
            resolutionDropdown.captionText.text = resolutionDropdownItems[dropdownIndex];
        }

        private int GetResolutionIndex() // Returns dropdown list index.
        {
            return PlayerPrefs.HasKey(resolutionPrefsKey) ? PlayerPrefs.GetInt(resolutionPrefsKey) : GetCurrentResolutionDropdownIndex();
        }

        public void SetResolutionIndex(int dropdownIndex) // Dropdown list index.
        {
            if (0 <= dropdownIndex && dropdownIndex < resolutionDropdownItems.Count)
            {
                var resolutionsIndex = ResolutionDropdownIndexToScreenResolutionsIndex(dropdownIndex);
                if (0 <= resolutionsIndex && resolutionsIndex < Screen.resolutions.Length)
                {
                    var resolution = Screen.resolutions[resolutionsIndex];
                    if (InputDeviceManager.instance != null) InputDeviceManager.instance.BrieflyIgnoreMouseMovement(); // Mouse "moves" (resets position) when resolution changes.
                    Screen.SetResolution(resolution.width, resolution.height, GetFullScreen());
                    PlayerPrefs.SetInt(resolutionPrefsKey, dropdownIndex);
                }
            }
            SelectNextFrame(resolutionDropdown);
        }

        private void RefreshGraphicsQualityDropdown()
        {
            if (PlayerPrefs.HasKey(graphicsQualityPrefsKey)) SetGraphicsQualityIndex(PlayerPrefs.GetInt(graphicsQualityPrefsKey));
            var list = new List<string>(QualitySettings.names);
            graphicsQualityDropdown.ClearOptions();
            graphicsQualityDropdown.AddOptions(list);
            var index = GetGraphicsQualityIndex();
            graphicsQualityDropdown.value = index;
            graphicsQualityDropdown.captionText.text = list[index];
            SelectNextFrame(graphicsQualityDropdown);
        }

        private int GetGraphicsQualityIndex()
        {
            return PlayerPrefs.HasKey(graphicsQualityPrefsKey) ? PlayerPrefs.GetInt(graphicsQualityPrefsKey) : QualitySettings.GetQualityLevel();
        }

        public void SetGraphicsQualityIndex(int index)
        {
            QualitySettings.SetQualityLevel(index);
            PlayerPrefs.SetInt(graphicsQualityPrefsKey, index);
            SelectNextFrame(graphicsQualityDropdown);
        }

        private void SelectNextFrame(UnityEngine.UI.Selectable selectable)
        {
            if (InputDeviceManager.autoFocus && selectable != null && selectable.gameObject.activeInHierarchy)
            {
                StopAllCoroutines();
                StartCoroutine(SelectNextFrameCoroutine(selectable));
            }
        }

        private IEnumerator SelectNextFrameCoroutine(UnityEngine.UI.Selectable selectable)
        {
            yield return null;
            UITools.Select(selectable);
        }

        private void RefreshMusicVolumeSlider()
        {
            if (musicVolumeSlider != null) musicVolumeSlider.value = PlayerPrefs.GetFloat(musicVolumePrefsKey, 0);
        }

        public void SetMusicLevel(float musicLevel)
        {
            if (!m_started) return;
            if (mainMixer != null) mainMixer.SetFloat(musicVolumeMixerParameter, musicLevel);
            PlayerPrefs.SetFloat(musicVolumePrefsKey, musicLevel);
        }

        private void RefreshSfxVolumeSlider()
        {
            if (sfxVolumeSlider != null) sfxVolumeSlider.value = PlayerPrefs.GetFloat(sfxVolumeMixerParameter, 0);
        }

        public void SetSfxLevel(float sfxLevel)
        {
            if (!m_started) return;
            if (mainMixer != null) mainMixer.SetFloat(sfxVolumeMixerParameter, sfxLevel);
            PlayerPrefs.SetFloat(sfxVolumePrefsKey, sfxLevel);
        }

        private void RefreshSubtitlesToggle()
        {
            subtitles.isOn = PlayerPrefs.GetInt(subtitlesPrefsKey, GetDefaultSubtitlesSetting() ? 1 : 0) == 1;
        }

        public void OnSubtitlesToggleChanged()
        {
            if (!m_started) return;
            SetSubtitles(subtitles.isOn);
        }

        public void SetSubtitles(bool on)
        {
            var subtitleSettings = DialogueManager.DisplaySettings.subtitleSettings;
            subtitleSettings.showNPCSubtitlesDuringLine = subtitles.isOn && setNPCSubtitlesDuringLine;
            subtitleSettings.showNPCSubtitlesWithResponses = subtitles.isOn && setNPCSubtitlesWithResponseMenu;
            subtitleSettings.showPCSubtitlesDuringLine = subtitles.isOn && setPCSubtitlesDuringLine;
            PlayerPrefs.SetInt(subtitlesPrefsKey, on ? 1 : 0);
        }

        private bool GetDefaultSubtitlesSetting()
        {
            var subtitleSettings = DialogueManager.displaySettings.subtitleSettings;
            return subtitleSettings.showNPCSubtitlesDuringLine || subtitleSettings.showNPCSubtitlesWithResponses || subtitleSettings.showPCSubtitlesDuringLine;
        }

        private void RefreshLanguagesDropdown()
        {
            if (languages == null || DialogueManager.DisplaySettings.localizationSettings.textTable == null) return;
            var language = PlayerPrefs.GetString("Language");
            var languageList = new List<string>(DialogueManager.DisplaySettings.localizationSettings.textTable.languages.Keys);
            for (int i = 0; i < languageList.Count; i++)
            {
                if (languageList[i] == language)
                {
                    languages.value = i;
                    return;
                }
            }
        }

        public void SetLanguageByIndex(int index)
        {
            if (DialogueManager.DisplaySettings.localizationSettings.textTable == null) return;
            var language = string.Empty;
            var languageList = new List<string>(DialogueManager.DisplaySettings.localizationSettings.textTable.languages.Keys);
            if (0 <= index && index < languageList.Count)
            {
                language = languageList[index];
            }
            var uiLocalizationManager = FindObjectOfType<UILocalizationManager>();
            if (uiLocalizationManager == null) uiLocalizationManager = gameObject.AddComponent<UILocalizationManager>();
            uiLocalizationManager.currentLanguage = language;
            Localization.language = language;
            DialogueManager.SetLanguage(language);
        }

    }
}