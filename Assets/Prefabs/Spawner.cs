using UnityEngine;

public class SpawnOnStart : MonoBehaviour
{
    public GameObject prefabToSpawn; // Drag the prefab you want to spawn onto this field in the Unity Editor

    void Start()
    {
        // Spawn the prefab at the position and rotation of this GameObject
        Instantiate(prefabToSpawn, transform.position, transform.rotation);
    }
}