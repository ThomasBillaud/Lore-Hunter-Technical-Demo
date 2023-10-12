using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueTrigger : MonoBehaviour
{
    private Animator animator;

    public Dialogue dialogue;

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    public void TriggerDialogue()
    {
        FindObjectOfType<DialogueManager>().StartDialogue(dialogue, this);
    }

    public void StartDialogue()
    {
        transform.LookAt(GameObject.FindWithTag("Player").transform);
        if (animator != null)
        {
            animator.SetBool("isTalking", true);
        }
    }

    public void EndDialogue()
    {
        if (animator != null)
        {
            animator.SetBool("isTalking", false);
        }
    }
}
