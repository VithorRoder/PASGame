using UnityEngine;
using UnityEngine.UI;

public class CharacterSelection : MonoBehaviour
{
    public Image characterImage;
    public Sprite[] characterSprites;
    public string[] characterNames;
    public int currentIndex = 0;
    public Text textFieldCharacterName;

    public void UpdateCharacter(int index)
    {
        characterImage.sprite = characterSprites[index];
        textFieldCharacterName.text = characterNames[index];
    }
}