using UnityEngine;
using UnityEngine.UI;

public class OptionButtonHandler : MonoBehaviour
{
    private ChoicesEngine choice;
    private StoryController storyController;

    public void Setup(ChoicesEngine choice, StoryController storyController)
    {
        this.choice = choice;
        this.storyController = storyController;

        Button button = GetComponentInChildren<Button>();
        if (button != null)
        {
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(() =>
            {
                // Call the callback with the choice
                storyController.OnOptionClicked(choice);

                // Destroy all option buttons
                storyController.DestroyAllOptions();
            });
        }
    }
}
