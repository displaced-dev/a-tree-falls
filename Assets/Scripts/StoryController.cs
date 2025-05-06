using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement; // Import for scene management

public class StoryController : MonoBehaviour
{
    [Header("--- Starting Prompt ---")]
    public PromptEngine StartingPrompt;

    [Header("--- Blending Info ---")]
    public Image blendImage;
    public AudioSource Audio;
    public AudioSource Ambiance;
    public Image backgroundImage;  // Assuming this is the current background image

    [Header("--- Text References ---")]
    public TMP_Text Location;
    public TMP_Text Prompt;

    public GameObject OptionPrefab;
    public GameObject OptionSpawnLocal;

    public string GameObjectToFind;
    public string CurrentAmbianceObject;

    private PromptEngine currentPrompt;
    public PromptEngine futurePrompt;

    private bool isMusic;

    void Start()
    {
        if (StartingPrompt.AmbianceObj != null)
        {
            CurrentAmbianceObject = StartingPrompt.AmbianceObj;
        }

        // Find the GameObject using the specified string and grab the AudioSource component
        GameObject targetObject = GameObject.Find(GameObjectToFind);
        if (targetObject != null)
        {
            Audio = targetObject.GetComponent<AudioSource>();
            if (Audio == null)
            {
                Debug.LogWarning("No AudioSource component found on the specified GameObject.");
            }
        }

        GameObject targetObject2 = GameObject.Find(CurrentAmbianceObject);
        if (targetObject2 != null)
        {
            Ambiance = targetObject2.GetComponent<AudioSource>();
            if (Audio == null)
            {
                Debug.LogWarning("No AudioSource component found on the specified GameObject.");
            }
        }

        else
        {
            Debug.LogWarning("GameObject not found in the scene.");
        }

        if (StartingPrompt != null)
        {
            currentPrompt = StartingPrompt;
            StartCoroutine(StartSequence());

            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }

    IEnumerator StartSequence()
    {
        // Fade in the background image first
        backgroundImage.color = StartingPrompt.promptColor;

        yield return StartCoroutine(FadeInImage(backgroundImage));

        // Now set the prompt and fade in the text and options
        SetPrompt(currentPrompt);

        yield return StartCoroutine(FadeInText(Prompt));
        yield return StartCoroutine(FadeInOptions());
    }

    void SetPrompt(PromptEngine promptEngine)
    {
        // Update location and prompt text
        if (Location.text != promptEngine.location)
        {
            StartCoroutine(FadeOutText(Location, () =>
            {
                Location.text = promptEngine.location; // Update location text after fade out
                StartCoroutine(FadeInText(Location)); // Fade in after location text is updated
            }));
        }
        else
        {
            Location.text = promptEngine.location;
        }

        Prompt.text = promptEngine.prompt;

        // Clear existing options
        DestroyAllOptions();

        // Instantiate new Option Prefabs for each choice
        foreach (ChoicesEngine choice in promptEngine.choices)
        {
            GameObject option = Instantiate(OptionPrefab, OptionSpawnLocal.transform);
            TMP_Text optionText = option.GetComponentInChildren<TMP_Text>();
            OptionButtonHandler optionHandler = option.GetComponent<OptionButtonHandler>();

            if (optionText != null)
            {
                optionText.text = choice.prompt;
            }

            if (optionHandler != null)
            {
                optionHandler.Setup(choice, this);  // Pass the controller itself
            }
        }
    }

    public void OnOptionClicked(ChoicesEngine choice)
    {
        StartCoroutine(TransitionToNextPrompt(choice));
    }

    public void DestroyAllOptions()
    {
        foreach (Transform child in OptionSpawnLocal.transform)
        {
            Destroy(child.gameObject);
        }
    }

    IEnumerator TransitionToNextPrompt(ChoicesEngine choice)
    {
        PromptEngine nextPrompt = choice.goTo;
        futurePrompt = nextPrompt;

        if (!isMusic)
        {
            Audio.enabled = true;
            isMusic = true;
        }

        if (currentPrompt.AmbianceObj != futurePrompt.AmbianceObj && futurePrompt != null)
        {
            // Fade out current ambiance audio
            if (Ambiance != null)
            {
                yield return StartCoroutine(FadeOutAudio(Ambiance));
                Ambiance.enabled = false;
                Ambiance = null;
            }

            // Set up new ambiance
            CurrentAmbianceObject = futurePrompt.AmbianceObj;
            GameObject targetObject2 = GameObject.Find(CurrentAmbianceObject);
            if (targetObject2 != null)
            {
                Ambiance = targetObject2.GetComponent<AudioSource>();
                if (Ambiance == null)
                {
                    Debug.LogWarning("No AudioSource component found on the specified GameObject.");
                }
            }
        }

        if (choice.isSceneChange)
        {
            // Fade out Location, Prompt, and Background
            yield return StartCoroutine(FadeOutText(Location));
            yield return StartCoroutine(FadeOutText(Prompt));
            yield return StartCoroutine(FadeOutImage(backgroundImage));

            if (Ambiance != null)
            {
                Ambiance.enabled = false;
            }

            SceneManager.LoadScene(choice.SceneToChange);
        }
        else
        {
            // Determine if location text should fade out
            bool shouldFadeOutLocation = Location.text != nextPrompt.location;

            // Fade out current text and options simultaneously
            if (shouldFadeOutLocation)
            {
                yield return StartCoroutine(FadeOutText(Location));
                Location.text = nextPrompt.location;
            }
            yield return StartCoroutine(FadeOutText(Prompt));
            yield return StartCoroutine(FadeOutOptions());

            // If the background image is different, perform the blend
            if (nextPrompt.dataImage != null && backgroundImage.sprite.texture != nextPrompt.dataImage)
            {
                // Fade in blend image
                yield return StartCoroutine(FadeInImage(blendImage));

                // Change the background image
                backgroundImage.sprite = Sprite.Create(nextPrompt.dataImage, new Rect(0, 0, nextPrompt.dataImage.width, nextPrompt.dataImage.height), new Vector2(0.5f, 0.5f));

                backgroundImage.color = futurePrompt.promptColor;

                // Fade out blend image
                yield return StartCoroutine(FadeOutImage(blendImage));
            }

            // Set new prompt text and options
            currentPrompt = nextPrompt;
            SetPrompt(currentPrompt);

            // Fade in new text and options simultaneously
            if (shouldFadeOutLocation)
            {
                yield return StartCoroutine(FadeInText(Location));
            }
            yield return StartCoroutine(FadeInText(Prompt));
            yield return StartCoroutine(FadeInOptions());

            // Fade in new ambiance audio after blend image has faded out
            if (Ambiance != null)
            {
                Ambiance.enabled = true;
                yield return StartCoroutine(FadeInAudio(Ambiance));
            }
        }
    }

    IEnumerator FadeOutAudio(AudioSource audioSource)
    {
        for (float t = audioSource.volume; t > 0; t -= Time.deltaTime)
        {
            audioSource.volume = t;
            yield return null;
        }
        audioSource.volume = 0;
    }

    IEnumerator FadeInAudio(AudioSource audioSource)
    {
        audioSource.volume = 0;
        for (float t = 0; t < 1; t += Time.deltaTime)
        {
            audioSource.volume = t;
            yield return null;
        }
        audioSource.volume = 1;
    }

    IEnumerator FadeOutText(TMP_Text text, System.Action onComplete = null)
    {
        Color originalColor = text.color;
        for (float t = 1.0f; t >= 0.0f; t -= Time.deltaTime)
        {
            text.color = new Color(originalColor.r, originalColor.g, originalColor.b, t);
            yield return null;
        }
        text.color = new Color(originalColor.r, originalColor.g, originalColor.b, 0);
        onComplete?.Invoke();
    }

    IEnumerator FadeInText(TMP_Text text)
    {
        Color originalColor = text.color;
        for (float t = 0.0f; t <= 1.0f; t += Time.deltaTime)
        {
            text.color = new Color(originalColor.r, originalColor.g, originalColor.b, t);
            yield return null;
        }
        text.color = new Color(originalColor.r, originalColor.g, originalColor.b, 1);
    }

    IEnumerator FadeOutOptions()
    {
        foreach (Transform child in OptionSpawnLocal.transform)
        {
            TMP_Text optionText = child.GetComponentInChildren<TMP_Text>();
            if (optionText != null)
            {
                yield return StartCoroutine(FadeOutText(optionText));
            }
        }
    }

    IEnumerator FadeInOptions()
    {
        foreach (Transform child in OptionSpawnLocal.transform)
        {
            TMP_Text optionText = child.GetComponentInChildren<TMP_Text>();
            if (optionText != null)
            {
                yield return StartCoroutine(FadeInText(optionText));
            }
        }
    }

    IEnumerator FadeInImage(Image image)
    {
        Color originalColor = image.color;
        for (float t = 0.0f; t <= 1.0f; t += Time.deltaTime)
        {
            image.color = new Color(originalColor.r, originalColor.g, originalColor.b, t);
            yield return null;
        }
        image.color = new Color(originalColor.r, originalColor.g, originalColor.b, 1);
    }

    IEnumerator FadeOutImage(Image image)
    {
        Color originalColor = image.color;
        for (float t = 1.0f; t >= 0.0f; t -= Time.deltaTime)
        {
            image.color = new Color(originalColor.r, originalColor.g, originalColor.b, t);
            yield return null;
        }
        image.color = new Color(originalColor.r, originalColor.g, originalColor.b, 0);
    }
}
