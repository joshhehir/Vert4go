using UnityEngine;

public class SpawnOnStart : MonoBehaviour
{
    public GameObject prefabToSpawn; // Drag the default prefab you want to spawn onto this field in the Unity Editor
    public string currentPrefab; // The string used to determine which prefab to spawn

    // You can also expose these fields in the Unity Editor and drag the respective prefabs onto them if needed
    public GameObject prefab1;
    public GameObject prefab2;
    public GameObject prefab3;
    public GameObject prefab4;

    void Start()
    {
        // Determine which prefab to spawn based on the value of currentPrefab
        switch (currentPrefab)
        {
            case "prefab1":
                prefabToSpawn = prefab1;
                break;
            case "prefab2":
                prefabToSpawn = prefab2;
                break;
            case "prefab3":
                prefabToSpawn = prefab3;
                break;
            case "prefab4":
                prefabToSpawn = prefab4;
                break;
            default:
                Debug.LogWarning("Invalid prefab specified: " + currentPrefab);
                break;
        }

        // Spawn the selected prefab at the position and rotation of this GameObject
        Instantiate(prefabToSpawn, transform.position, transform.rotation);
    }
}