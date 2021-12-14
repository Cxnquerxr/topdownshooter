using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class MenuController: MonoBehaviour
{
    [Header("Volume Settings")] 
    [SerializeField] private TMP_Text volumeValue = null;
    [SerializeField] private Slider volumeSlider = null;
    [SerializeField] private float defaultVolume = 1.0f;

    [Header("Confirmation")] 
    [SerializeField] private GameObject confirmationPrompt = null;

    [Header("Gameplay Settings")] 
    [SerializeField] private TMP_Text SensValue = null;
    [SerializeField] private Slider SensSlider = null;
    [SerializeField] private int DefaultSens = 1;
    
    public int MainControllerSens = 1;

    [Header("Graphics Settings")] 
    [SerializeField] private Slider brightSlider = null;
    [SerializeField] private TMP_Text brightnessValue = null;
    [SerializeField] private float defaultBrightness = 1;

    [Space(10)]
    [SerializeField] private TMP_Dropdown qualityDropdown;
    [SerializeField] private Toggle fullscreenToggle;

    private int _qualityLevel;
    private bool _isFullscreen;
    private float _brightnessLevel;


    [Header("Levels to load")]
    public string _newGameLevel;
    private string levelToLoad;
    [SerializeField] private GameObject NoSavePopup = null;

    [Header("Resolution Dropdowns")] 
    public TMP_Dropdown resolutionDropdown;
    private Resolution[] resolutions;

    private void Start()
    {
        resolutions = Screen.resolutions;
        resolutionDropdown.ClearOptions();

        List<string> options = new List<string>();

        int currentResolutionIndex = 0;

        for (int i = 0; i < resolutions.Length; i++)
        {
            string option = resolutions[i].width + " x " + resolutions[i].height;
            options.Add(option);

            if (resolutions[i].width == Screen.width && resolutions[i].height == Screen.height)
            {
                currentResolutionIndex = i;
            }
        }

        resolutionDropdown.AddOptions(options);
        resolutionDropdown.value = currentResolutionIndex;
        resolutionDropdown.RefreshShownValue();
    }

    public void SetResolution(int resolutionIndex)
    {
        Resolution resolution = resolutions[resolutionIndex];
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
    }

    public void NewGameYes()
    {
        SceneManager.LoadScene(_newGameLevel);
    }

    public void LoadGameYes()
    {
        if (PlayerPrefs.HasKey("SavedLevel"))
        {
            levelToLoad = PlayerPrefs.GetString("SavedLevel");
            SceneManager.LoadScene(levelToLoad);
        }
        else
        {
            NoSavePopup.SetActive(true);
        }
    }

    public void ExitButton()
    {
        Application.Quit();
    }

    public void SetVolume(float volume)
    {
        AudioListener.volume = volume;
        volumeValue.text = volume.ToString("0.0");
    }

    public void VolumeApply()
    {
        PlayerPrefs.SetFloat("masterVolume", AudioListener.volume);
        StartCoroutine(ConfirmationBox());
    }

    public void SetSens(float sens)
    {
        MainControllerSens = Mathf.RoundToInt(sens);
        SensValue.text = sens.ToString("0");
    }

    public void GameplayApply()
    {
        PlayerPrefs.SetInt("masterSens", MainControllerSens);
        StartCoroutine(ConfirmationBox());
    }

    public void SetBrighness(float brightness)
    {
        _brightnessLevel = brightness;
        brightnessValue.text = brightness.ToString("0.0");
    }

    public void SetFullScreen(bool isFulscreen)
    {
        _isFullscreen = isFulscreen;
    }

    public void SetQuality(int qualityIndex)
    {
        _qualityLevel = qualityIndex;
    }

    public void GraphicsApply()
    {
        PlayerPrefs.SetFloat("masterBrightness", _brightnessLevel);

        PlayerPrefs.SetInt("masterQuality", _qualityLevel);
        QualitySettings.SetQualityLevel(_qualityLevel);

        PlayerPrefs.SetInt("masterFullscreen", (_isFullscreen ? 1 : 0));
        Screen.fullScreen = _isFullscreen;

        StartCoroutine(ConfirmationBox());
    }

    public void ResetButton(string MenuType)
    {
        if (MenuType == "Audio")
        {
            AudioListener.volume = defaultVolume;
            volumeSlider.value = defaultVolume;
            volumeValue.text = defaultVolume.ToString("0.0");
            VolumeApply();
        }

        if (MenuType == "Gameplay")
        {
            SensValue.text = DefaultSens.ToString("0");
            SensSlider.value = DefaultSens;
            MainControllerSens = DefaultSens;
            GameplayApply();
        }

        if (MenuType == "Video")
        {
            brightSlider.value = defaultBrightness;
            brightnessValue.text = defaultBrightness.ToString("0.0");

            qualityDropdown.value = 2;
            QualitySettings.SetQualityLevel(1);

            fullscreenToggle.isOn = false;
            Screen.fullScreen = false;

            Resolution currentResolution = Screen.currentResolution;
            Screen.SetResolution(currentResolution.width, currentResolution.height, Screen.fullScreen);
            resolutionDropdown.value = resolutions.Length;
            GraphicsApply();
        }

    }
    public IEnumerator ConfirmationBox()
    {
        confirmationPrompt.SetActive(true);
        yield return new WaitForSeconds(2);
        confirmationPrompt.SetActive(false);
    }
}
