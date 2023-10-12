using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Localization.Settings;

public class QuickMessage : MonoBehaviour
{
    public TextMeshProUGUI m_TextMeshProUGUI;
    public float messageDuration = 1.0f;
    private Animator animator;

	private void Start()
	{
		animator = GetComponent<Animator>();
        StartCoroutine(Timer());
	}

	public void Init(string tableName, string messageKey)
    {
        m_TextMeshProUGUI.text = LocalizationSettings.StringDatabase.GetLocalizedString(tableName, messageKey, LocalizationSettings.SelectedLocale);
    }

    private IEnumerator Timer()
    {
        yield return new WaitForSeconds(messageDuration);
        animator.SetBool("isDisappearing", true);
    }

    public void DestroyMessage()
    {
        Destroy(this.gameObject);
    }
}
