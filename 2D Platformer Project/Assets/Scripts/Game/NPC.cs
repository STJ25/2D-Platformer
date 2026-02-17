using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class NPC : MonoBehaviour
{
    [Header("NPC Scriptable Object")]
    public NPCData npcData;

    [Header("UI Elements")]
    public GameObject dialoguePanel;
    public TextMeshProUGUI dialogueText;
    public TextMeshProUGUI npcNameText;
    public Image npcPortrait;

    [Header("Typing Settings")]
    public float wordSpeed = 0.02f;

    private bool playerIsClose;
    private int index = 0;
    private bool isTyping = false;
    private Coroutine typingCoroutine;

    void Start()
    {
        dialogueText.text = "";
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q) && playerIsClose)
        {
            if (!dialoguePanel.activeInHierarchy)
            {
                OpenDialogue();
            }
            else if (!isTyping && dialogueText.text == npcData.dialogueLines[index])
            {
                NextLine();
            }
        }

        if (Input.GetKeyDown(KeyCode.C) && dialoguePanel.activeInHierarchy)
        {
            CloseDialogue();
        }
    }

    void OpenDialogue()
    {
        // Always update NPC-specific UI before showing the panel
        npcNameText.text = npcData.npcName;
        npcPortrait.sprite = npcData.portrait;

        dialoguePanel.SetActive(true);
        index = 0;
        StartTyping();
    }

    void StartTyping()
    {
        dialogueText.text = "";
        typingCoroutine = StartCoroutine(TypeText(npcData.dialogueLines[index]));
    }

    IEnumerator TypeText(string line)
    {
        isTyping = true;
        dialogueText.text = "";

        foreach (char letter in line.ToCharArray())
        {
            // Safety check in case object is destroyed during typing
            if (dialogueText == null || !gameObject.activeInHierarchy)
            {
                yield break;
            }

            dialogueText.text += letter;
            yield return new WaitForSeconds(wordSpeed);
        }

        isTyping = false;
    }

    void NextLine()
    {
        if (index < npcData.dialogueLines.Length - 1)
        {
            index++;
            StartTyping();
        }
        else
        {
            CloseDialogue();
        }
    }

    void CloseDialogue()
    {
        if (typingCoroutine != null)
            StopCoroutine(typingCoroutine);

        dialogueText.text = "";
        isTyping = false;
        index = 0;
        dialoguePanel.SetActive(false);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerIsClose = true;
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerIsClose = false;
            CloseDialogue();
        }
    }

    void OnDestroy()
    {
        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
        }
    }

}
