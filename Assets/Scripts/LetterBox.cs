using System.Collections.Generic;
using UnityEngine;

public class LetterBox : MonoBehaviour
{
    public GameManager gameManager;

    private string correctItemId;
    private bool roundLocked = false;

    private readonly HashSet<int> processedInstanceIds = new HashSet<int>();

    public void SetCorrectItem(string id)
    {
        correctItemId = id;
        roundLocked = false;
        processedInstanceIds.Clear();
    }

    private void OnTriggerStay(Collider other)
    {
        if (gameManager == null) return;
        if (!gameManager.IsGameRunning) return;
        if (roundLocked) return;

        ThrowableItem item = other.GetComponentInParent<ThrowableItem>();
        if (item == null) return;

        int instanceId = item.gameObject.GetInstanceID();
        if (processedInstanceIds.Contains(instanceId)) return;

        processedInstanceIds.Add(instanceId);

        Debug.Log("В коробке предмет: " + item.itemId + " | нужно: " + correctItemId);

        if (item.itemId == correctItemId)
        {
            roundLocked = true;
            gameManager.CorrectAnswer();
            Destroy(item.gameObject);
        }
        else
        {
            gameManager.WrongAnswer();
            Destroy(item.gameObject);
        }
    }
}