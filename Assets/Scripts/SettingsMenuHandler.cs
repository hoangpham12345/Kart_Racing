﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;	// Unity audio library, import this to get the AudioMixer
using TMPro;	// TextMesh pro library, use this to take the TMP_Dropdown
using UnityEngine.Rendering;	// Unity Rendering, use this to get the list of RenderPipelineAsset
using UnityEngine.UI; // Unity UI library, use to get resolutions

public class SettingsMenuHandler : MonoBehaviour
{

  public AudioMixer audioMixer;
  public RenderPipelineAsset[] qualityLevels;
  public TMP_Dropdown qualityDropdown;
  public TMP_Dropdown resolutionDropdown;
  Resolution[] resolutions;

  void Start()
  {
    qualityDropdown.value = QualitySettings.GetQualityLevel();

    resolutions = Screen.resolutions;

    // CLear the resolutionDropdown list before adding the resolutions
    resolutionDropdown.ClearOptions();

    // Create a list to store all option strings
    List<string> options = new List<string>();

    int currentResolutionIndex = 0;
    // Iterate all elements in the resolution array and add it to the list.
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

  // Note: To make this method work, the value of the slider should be: min: 0.0001 and max: 1
  public void SetVolume(float volume)
  {
    // Normally with Unity mixer, human ear can get the sound greater than -40dB. Therefore, we will get the value from 0.0001 to 1 and modify them to get the value from -40dB to 0dB
    audioMixer.SetFloat("MasterVolume", Mathf.Log10(volume) * 20);
  }

  public void SetQuality(int qualityIndex)
  {
    QualitySettings.SetQualityLevel(qualityIndex);
    QualitySettings.renderPipeline = qualityLevels[qualityIndex];
  }

  public void SetResolution(int resolutionIndex)
  {
    Resolution resolution = resolutions[resolutionIndex];
    Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
  }

  public void SetFullScreen(bool isFullScreen)
  {
    Screen.fullScreen = isFullScreen;
  }
}
