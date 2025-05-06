using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class StartFadeIn : MonoBehaviour
{
    public Image FadeImage;  // The Image component to fade out
    public float initialDelay = 5f;  // Initial delay before starting the fade
    public float fadeDuration = 5f;  // Duration of the fade-out

    public void Start()
    {
        StartCoroutine(FadeOutImage());
    }

    private IEnumerator FadeOutImage()
    {
        // Wait for the initial delay before starting the fade-out
        yield return new WaitForSecondsRealtime(initialDelay);

        Color imageColor = FadeImage.color;
        float startAlpha = imageColor.a;

        for (float t = 0; t < fadeDuration; t += Time.deltaTime)
        {
            float normalizedTime = t / fadeDuration;
            imageColor.a = Mathf.Lerp(startAlpha, 0, normalizedTime);
            FadeImage.color = imageColor;
            yield return null;
        }

        // Ensure the image is fully transparent at the end of the fade
        imageColor.a = 0;
        FadeImage.color = imageColor;
    }
}
