using TMPro;
using UnityEngine;

public class LetterDisplay : MonoBehaviour
{
    public TextMeshProUGUI text;

    public void ShowLetter(string letter)
    {
        text.text = letter;
    }
}