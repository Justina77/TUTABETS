using UnityEngine;

[CreateAssetMenu(fileName = "LetterData", menuName = "Alphabet/Letter")]
public class LetterData : ScriptableObject
{
    public string letter;
    public GameObject correctObject;
    public GameObject[] wrongObjects;
}
