using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;

public class IntroPanelController : MonoBehaviour
{
    [Header("UI References")]
    public CanvasGroup panelCanvasGroup;
    public TMP_Text introText; // Use TextMeshPro
    public Button skipButton;

    [TextArea]
    public string fullText = "Welcome, brave Knight, to your quest...";
    public float typingSpeed = 0.05f;
    public float fadeDuration = 1f;

    private static bool hasSkipped = false;

    void Start()
    {
        if (hasSkipped)
        {
            panelCanvasGroup.gameObject.SetActive(false);
        }
        else
        {
            panelCanvasGroup.gameObject.SetActive(true);
            skipButton.onClick.AddListener(SkipIntro);
            StartCoroutine(PlayIntroSequence());
        }
    }

    IEnumerator PlayIntroSequence()
    {
        // Fade In
        panelCanvasGroup.alpha = 0;
        yield return StartCoroutine(FadeCanvasGroup(0f, 1f, fadeDuration));

        // Typing effect
        introText.text = "";
        foreach (char c in fullText)
        {
            introText.text += c;
            yield return new WaitForSeconds(typingSpeed);
        }
    }

    void SkipIntro()
    {
        hasSkipped = true;
        StartCoroutine(SkipAndFadeOut());
    }

    IEnumerator SkipAndFadeOut()
    {
        skipButton.interactable = false; // Prevent double-click
        yield return StartCoroutine(FadeCanvasGroup(1f, 0f, fadeDuration));
        panelCanvasGroup.gameObject.SetActive(false);
    }

    IEnumerator FadeCanvasGroup(float from, float to, float duration)
    {
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            panelCanvasGroup.alpha = Mathf.Lerp(from, to, elapsed / duration);
            yield return null;
        }
        panelCanvasGroup.alpha = to;
    }
}
