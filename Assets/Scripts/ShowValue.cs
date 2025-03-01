using UnityEngine;
using UnityEngine.UI;

public class ShowValue : MonoBehaviour
{
    public Text sourceText;
    public Text targetText;

    public void ShowValueText()
    {
        targetText.text = sourceText.text;
    }
}