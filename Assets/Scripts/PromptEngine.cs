using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Prompt", menuName = "ScriptableObjects/Prompts", order = 1)]
public class PromptEngine : ScriptableObject
{
    public string location;
    public string prompt;
    public Texture2D dataImage;

    public Color promptColor = new Color(1f, 1f, 1f, 1f);  // White with full alpha

    public string AmbianceObj;
    public string MusicObj;
    public List<ChoicesEngine> choices;
}
