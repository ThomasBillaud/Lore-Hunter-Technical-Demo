using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using UnityEngine.Localization.Components;
using TMPro;
using Cinemachine;

public class DialogueManager : MonoBehaviour
{
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI dialogueText;
    public GameObject dialogueBox;
    public GameObject blackFade;
    public GameObject mainCamera;
    public GameObject dialogueCamera;
    public GameObject gameHUD;

    private string tableName;
    private Queue<string> keys;
    private Locale currentSelectedLocale;
    private Animator animator;
    private Animator blackFadeAnimator;
    private DialogueTrigger character;
    private GameObject player;
    private InputReader input;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindWithTag("Player");
        keys = new Queue<string>();
        currentSelectedLocale = LocalizationSettings.SelectedLocale;
        animator = dialogueBox.GetComponent<Animator>();
        blackFadeAnimator = blackFade.GetComponent<Animator>();
        input = FindObjectOfType<InputReader>();
    }

    public void ChangeLocalization()
    {
        currentSelectedLocale = LocalizationSettings.SelectedLocale;
    }

    public void StartDialogue(Dialogue dialogue)
    {
        input.DisableAllInput();
        ChangeLocalization();
        animator.SetBool("isOpen", true);
        nameText.text = dialogue.name;
        tableName = dialogue.tableName;

        keys.Clear();

        foreach(string key in dialogue.keyName)
        {
            keys.Enqueue(key);
        }
        DisplayNextSentence();
    }

    public void StartDialogue(Dialogue dialogue, DialogueTrigger characterTrigger)
    {
        input.DisableAllInput();
        character = characterTrigger;
        StartCoroutine(InitializeDialogue(dialogue));
    }

    public void DisplayNextSentence()
    {
        if (keys.Count == 0)
        {
            EndDialogue();
            return;
        }
        string key = keys.Dequeue();
        string sentence = LocalizationSettings.StringDatabase.GetLocalizedString(tableName, key, currentSelectedLocale);
        StopAllCoroutines();
        StartCoroutine(TypeSentence(sentence));
    }

    IEnumerator InitializeDialogue(Dialogue dialogue)
    {
        blackFade.SetActive(true);
        blackFadeAnimator.SetBool("Fade", true);
        yield return new WaitForSeconds(0.35f);
        gameHUD.SetActive(false);
        mainCamera.SetActive(false);
        dialogueCamera.SetActive(true);
        blackFade.SetActive(false);
        player.transform.LookAt(character.gameObject.transform);
        character.StartDialogue();
        player.GetComponent<CharacterController>().enabled = false;
        player.transform.position = character.gameObject.transform.position + character.gameObject.transform.forward * 1.5f;
        player.GetComponent<CharacterController>().enabled = true;
        ChangeLocalization();
        blackFadeAnimator.SetBool("Fade", false);
        yield return new WaitForSeconds(0.35f);
        animator.SetBool("isOpen", true);
        nameText.text = dialogue.name;
        tableName = dialogue.tableName;

        keys.Clear();

        foreach(string key in dialogue.keyName)
        {
            keys.Enqueue(key);
        }
        input.EnableUIInput();
        DisplayNextSentence();
    }

    IEnumerator TypeSentence(string sentence)
    {
        dialogueText.text = "";
        foreach(char letter in sentence.ToCharArray())
        {
            dialogueText.text += letter;
            yield return null;
        }
    }

    void EndDialogue()
    {
        animator.SetBool("isOpen", false);
        if (character != null)
        {
            character.EndDialogue();
            character = null;
        }
        dialogueCamera.SetActive(false);
        mainCamera.SetActive(true);
        gameHUD.SetActive(true);
        input.DisableUIInput();
    }

}
