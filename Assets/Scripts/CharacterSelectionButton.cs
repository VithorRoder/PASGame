using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterSelectionButton : MonoBehaviour
{
    public bool isNextButton;
    private CharacterSelection attIndexScript;

    private void Start()
    {
        GameObject objectWithIndex = GameObject.Find("AttIndex");
        if (objectWithIndex != null)
        {
            attIndexScript = objectWithIndex.GetComponent<CharacterSelection>();
        }
        else
        {
            Debug.LogError("GameObject 'AttIndex' not found.");
        }
    }

    public void OnButtonClick()
    {
        if (attIndexScript != null)
        {
            if (isNextButton)
            {
                attIndexScript.currentIndex = (attIndexScript.currentIndex + 1) % attIndexScript.characterSprites.Length;
            }
            else
            {
                attIndexScript.currentIndex = (attIndexScript.currentIndex - 1 + attIndexScript.characterSprites.Length) % attIndexScript.characterSprites.Length;
            }

            attIndexScript.UpdateCharacter(attIndexScript.currentIndex);
        }
    }

    public void OnSelectConfirmClick()
    {
        if (attIndexScript != null)
        {
            attIndexScript.UpdateCharacter(attIndexScript.currentIndex);
            Debug.Log("Personagem confirmado: " + attIndexScript.currentIndex);
        }
    }
}