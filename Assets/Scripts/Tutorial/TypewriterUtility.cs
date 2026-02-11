using System.Collections;
using TMPro;
using UnityEngine;

public class TypewriterUtility : MonoBehaviour
{
    private Coroutine typingCoroutine;
    private bool skip;

    public void StartTyping(TextMeshProUGUI textUI, string message, float duration)
    {
        if (typingCoroutine != null)
            StopCoroutine(typingCoroutine);

        typingCoroutine = StartCoroutine(TypeRoutine(textUI, message, duration));
    }

    private IEnumerator TypeRoutine(TextMeshProUGUI textUI, string message, float duration)
    {
        skip = false;

        textUI.text = message;
        textUI.ForceMeshUpdate();

        int totalChars = textUI.textInfo.characterCount;
        textUI.maxVisibleCharacters = 0;

        if (totalChars == 0)
            yield break;

        float timePerChar = duration / totalChars;
        float timer = 0f;
        int visibleCount = 0;

        while (visibleCount < totalChars)
        {
            if (skip)
            {
                textUI.maxVisibleCharacters = totalChars;
                break;
            }

            timer += Time.unscaledDeltaTime;

            while (timer >= timePerChar && visibleCount < totalChars)
            {
                visibleCount++;
                timer -= timePerChar;
                textUI.maxVisibleCharacters = visibleCount;
            }

            yield return null;
        }

        typingCoroutine = null;
    }

    public void Skip()
    {
        skip = true;
    }
}