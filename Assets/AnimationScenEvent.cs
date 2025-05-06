using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement; // Needed for scene management

public class AnimationScenEvent : MonoBehaviour
{
    // Called when the script is first loaded or a value is changed in the Inspector
    void Start()
    {
        // Initialization if needed
    }

    // Update is called once per frame
    void Update()
    {
        // Any updates if needed
    }

    // Method to be called by the animation event
    public void ChangeScene(string sceneName)
    {
        // Check if the scene name is valid
        if (!string.IsNullOrEmpty(sceneName))
        {
            // Load the new scene
            SceneManager.LoadScene(sceneName);
        }
        else
        {
            Debug.LogWarning("Scene name is null or empty.");
        }
    }
}
