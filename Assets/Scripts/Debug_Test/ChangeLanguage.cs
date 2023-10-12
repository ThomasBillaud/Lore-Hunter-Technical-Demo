using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using UnityEngine.Localization.Components;

public class ChangeLanguage : MonoBehaviour
{
	public void ChangeLocalLanguage()
	{
		if (LocalizationSettings.SelectedLocale == LocalizationSettings.AvailableLocales.Locales[0])
		{
			LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[1];
		} else
		{
			LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[0];
		}
	}
}
