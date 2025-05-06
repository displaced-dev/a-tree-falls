using UnityEngine;

[CreateAssetMenu(fileName = "Choices", menuName = "ScriptableObjects/Choices", order = 1)]
public class ChoicesEngine : ScriptableObject
{
    public string prompt;
    public PromptEngine goTo;

    public bool isSceneChange;
    public string SceneToChange;
}
