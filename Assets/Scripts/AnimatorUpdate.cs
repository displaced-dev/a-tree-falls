using System.Collections.Generic;
using UnityEngine;

public class AnimatorUpdate : MonoBehaviour
{
    // List of Animator Controllers
    public List<Animator> animatorControllers;

    // Local index variable
    private int index = 0;

    // IncreaseIndex function to increment the index and update the Animator Controllers
    public void IncreaseIndex()
    {
        // Increment the local index variable
        index++;

        // Loop through all Animator Controllers in the list and update their "index" parameter
        foreach (Animator animator in animatorControllers)
        {
            // Check if the Animator Controller has the "index" parameter
            if (animator.HasParameter("index"))
            {
                // Set the "index" parameter to the current value of the local index variable
                animator.SetInteger("index", index);
            }
        }
    }
}

// Extension method to check if the parameter exists in the Animator
public static class AnimatorExtensions
{
    public static bool HasParameter(this Animator animator, string paramName)
    {
        foreach (AnimatorControllerParameter param in animator.parameters)
        {
            if (param.name == paramName)
            {
                return true;
            }
        }
        return false;
    }
}
