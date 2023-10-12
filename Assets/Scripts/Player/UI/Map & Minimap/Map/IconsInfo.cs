using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class IconsInfo : MonoBehaviour
{
	public string iconName = "Dummy";

	public TextMeshProUGUI text;
	public RectTransform icon;

	public void HoverFocus()
	{
		text.text = iconName;
		icon.localScale = new Vector2(2.5f, 2.5f);
		text.gameObject.SetActive(true);
	}

	public void HoverUnfocus()
	{
		icon.localScale = new Vector2(1f, 1f);
		text.gameObject.SetActive(false);
	}
}
