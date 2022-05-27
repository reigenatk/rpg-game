using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Typewriter : MonoBehaviour
{
    [SerializeField] private float typeSpeed = 30f;

    private readonly Dictionary<HashSet<char>, float> punctuations = new Dictionary<HashSet<char>, float>()
    {
        {new HashSet<char>(){'.', '!', '?'}, 0.6f},
        {new HashSet<char>(){',', ';', ':'}, 0.3f}
    };

    public bool IsRunning { get; private set; }
    
    public void Run(string textToType, TMP_Text textLabel)
    {
        typingCoroutine = StartCoroutine(TypeText(textToType, textLabel));
    }

    private Coroutine typingCoroutine;

    public void Stop()
    {
        StopCoroutine(typingCoroutine);
        IsRunning = false;
    }

    private IEnumerator TypeText(string textToType, TMP_Text textLabel)
    {
        IsRunning = true;
        float elapsed_time = 0;
        int charIndex = 0;

        while (charIndex < textToType.Length)
        {
            int lastCharIndex = charIndex;

            elapsed_time += Time.deltaTime * typeSpeed;

            // floor of elapsed time
            charIndex = Mathf.FloorToInt(elapsed_time);

            // can never be greater than length of string
            charIndex = Mathf.Clamp(charIndex, 0, textToType.Length);

            // look for chars that have been typed since last tick
            for (int i = lastCharIndex; i < charIndex; i++)
            {
                bool isLast = (i >= textToType.Length - 1);

                textLabel.text = textToType.Substring(0, i+1);
                if (IsPunctuation(textToType[i], out float waitTime) && !isLast && !IsPunctuation(textToType[i+1], out _))
                {
                    yield return new WaitForSeconds(waitTime);
                }
            }


            

            yield return null;
        }
        IsRunning = false;
        
    }

    // check if we found the character c
    private bool IsPunctuation(char c, out float waitTime)
    {
        foreach(KeyValuePair<HashSet<char>, float> punctuationCategory in punctuations)
        {
            if (punctuationCategory.Key.Contains(c))
            {
                waitTime = punctuationCategory.Value;
                return true;
            }
        }

        waitTime = default;
        return false;
    }
}
