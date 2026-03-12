using UnityEngine;

public class ObjectSpawner : MonoBehaviour
{
    public Transform[] spawnPoints;

    public void SpawnObjects(GameObject correct, GameObject[] wrong)
    {
        GameObject[] objects = new GameObject[3];

        objects[0] = correct;
        objects[1] = wrong[0];
        objects[2] = wrong[1];

        Shuffle(objects);

        for (int i = 0; i < spawnPoints.Length; i++)
        {
            Instantiate(objects[i], spawnPoints[i].position, Quaternion.identity);
        }
    }

    void Shuffle(GameObject[] array)
    {
        for (int i = 0; i < array.Length; i++)
        {
            GameObject temp = array[i];
            int randomIndex = Random.Range(i, array.Length);
            array[i] = array[randomIndex];
            array[randomIndex] = temp;
        }
    }
}