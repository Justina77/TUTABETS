using UnityEngine;

public class LetterBox : MonoBehaviour
{
    string correctObject;

    public GameManager gameManager;

    public void SetCorrectObject(string name)
    {
        correctObject = name;
    }

    private void OnTriggerEnter(Collider other)
    {
        ThrowableItem item = other.GetComponent<ThrowableItem>();

        if (item == null) return;

        if (item.objectName == correctObject)
        {
            gameManager.CorrectAnswer();
        }
        else
        {
            gameManager.WrongAnswer();
        }

        Destroy(other.gameObject);
    }
}