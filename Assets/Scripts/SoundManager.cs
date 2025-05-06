using System.Collections;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    [Header("--- Main Chorus ---")]
    public AudioSource Prelude;
    public AudioSource BondBroken;
    public AudioSource LastGoodNote;

    [Header("--- Ambiances ---")]
    public AudioSource Bar;
    public AudioSource Kids;

    [Header("--- Fade Settings ---")]
    public float fadeDuration = 1.0f;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
        Application.targetFrameRate = 120;
    }

    public void StartPre()
    {
        StartCoroutine(FadeIn(Prelude));
        StopBond();
        StopGood();
    }

    public void StopPre()
    {
        StartCoroutine(FadeOut(Prelude));
    }

    public void StartBond()
    {
        StartCoroutine(FadeIn(BondBroken));
        StopPre();
        StopGood();
    }

    public void StopBond()
    {
        StartCoroutine(FadeOut(BondBroken));
    }

    public void StartGood()
    {
        StartCoroutine(FadeIn(LastGoodNote));
        StopBond();
        StopPre();
    }

    public void StopGood()
    {
        StartCoroutine(FadeOut(LastGoodNote));
    }

    public void TransitionBar()
    {
        StartCoroutine(FadeOut(Bar));
        StartCoroutine(FadeIn(Kids));
    }

    public void StopAmb()
    {
        StartCoroutine(FadeOut(Bar));
        StartCoroutine(FadeOut(Kids));
    }

    private IEnumerator FadeIn(AudioSource audioSource)
    {
        audioSource.enabled = true;
        audioSource.volume = 0f;
        audioSource.Play();

        float startVolume = audioSource.volume;
        float targetVolume = 1.0f;

        float elapsed = 0f;
        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            audioSource.volume = Mathf.Lerp(startVolume, targetVolume, elapsed / fadeDuration);
            yield return null;
        }

        audioSource.volume = targetVolume;
    }

    private IEnumerator FadeOut(AudioSource audioSource)
    {
        float startVolume = audioSource.volume;
        float targetVolume = 0f;

        float elapsed = 0f;
        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            audioSource.volume = Mathf.Lerp(startVolume, targetVolume, elapsed / fadeDuration);
            yield return null;
        }

        audioSource.volume = targetVolume;
        audioSource.Stop();
        audioSource.enabled = false;
    }
}
