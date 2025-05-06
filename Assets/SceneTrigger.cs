using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class SceneTrigger : MonoBehaviour
{
    public Image FadeImage;
    public Collider PlayerCollider;
    public string SceneName;

    public string ObjectToMute;

    private bool isColliding = false;

    private void Start()
    {
        Debug.Log("Script started, FadeImageOut called.");
    }

    private void Update()
    {
        if (isColliding)
        {
            Debug.Log("Player is colliding, SceneTriggered called.");
            SceneTriggered();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("OnTriggerEnter called by: " + other.name);
        Debug.Log("Trigger condition met, setting isColliding to true.");
        isColliding = true;
    }

    private void OnTriggerExit(Collider other)
    {
        Debug.Log("OnTriggerExit called by: " + other.name);
        Debug.Log("Trigger condition met, setting isColliding to true.");
        isColliding = true;
    }

    public void SceneTriggered()
    {
        Debug.Log("SceneTriggered method called, starting scene change.");
        StartCoroutine(FadeAndChangeScene(SceneName));
    }

    private void FadeImageIn()
    {
        StartCoroutine(FadeImageCoroutine(1));
    }

    private void FadeImageOut()
    {
        StartCoroutine(FadeImageCoroutine(0));
    }

    private IEnumerator FadeImageCoroutine(float targetAlpha)
    {
        float fadeDuration = 1f;
        float startAlpha = FadeImage.color.a;
        float time = 0;

        while (time < fadeDuration)
        {
            time += Time.deltaTime;
            float alpha = Mathf.Lerp(startAlpha, targetAlpha, time / fadeDuration);
            FadeImage.color = new Color(FadeImage.color.r, FadeImage.color.g, FadeImage.color.b, alpha);
            yield return null;
        }

        FadeImage.color = new Color(FadeImage.color.r, FadeImage.color.g, FadeImage.color.b, targetAlpha);
        Debug.Log("Fade completed to target alpha: " + targetAlpha);
    }

    private IEnumerator FadeAndChangeScene(string sceneName)
    {
        // Find the GameObject to mute
        GameObject objectToMute = GameObject.Find(ObjectToMute);
        AudioSource audioSource = objectToMute?.GetComponent<AudioSource>();

        // Fade the image out
        FadeImageIn();
        yield return new WaitForSeconds(1f);

        // Fade out the audio if an AudioSource is found
        if (audioSource != null)
        {
            StartCoroutine(FadeAudioSource(audioSource, 0f, 1));
        }

        // Wait for the image fade to complete
        yield return new WaitForSeconds(1f);

        // Load the new scene
        Debug.Log("Changing scene to: " + sceneName);
        SceneManager.LoadScene(sceneName);
    }

    private IEnumerator FadeAudioSource(AudioSource audioSource, float targetVolume, float duration)
    {
        float startVolume = audioSource.volume;
        float time = 0;

        while (time < duration)
        {
            time += Time.deltaTime;
            float volume = Mathf.Lerp(startVolume, targetVolume, time / duration);
            audioSource.volume = volume;
            yield return null;
        }

        audioSource.volume = targetVolume;
        audioSource.Stop();  // Optionally stop the audio after fading out
        Debug.Log("Audio fade completed to target volume: " + targetVolume);
    }
}
