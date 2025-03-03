using UnityEngine;
using UnityEngine.UI;

public class CharacterSelectButton : MonoBehaviour
{
    public Button button; // Assign this in the Inspector

    void Start()
    {
        if (button != null)
        {
            button.onClick.AddListener(SetCharacterToDefault);
        }
    }

    public void SetCharacterToDefault()
    {
        PlayerPrefs.SetInt("selectedCharacter", 0);
        PlayerPrefs.Save();
        Debug.Log("Character set to default (index 0)");
    }
}
