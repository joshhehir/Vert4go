using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class LoadCharacter : MonoBehaviour
{
	public GameObject[] characterPrefabs;
	public Transform spawnPoint;
	public TMP_Text label;

    void Start()
    {
        int selectedCharacter = PlayerPrefs.GetInt("selectedCharacter", 0); // Default to 0
        Debug.Log("Selected Character Index: " + selectedCharacter);

        if (selectedCharacter < 0 || selectedCharacter >= characterPrefabs.Length)
        {
            Debug.LogError("Selected character index is out of range! Check PlayerPrefs.");
            return;
        }

        GameObject prefab = characterPrefabs[selectedCharacter];

        if (prefab == null)
        {
            Debug.LogError("Character prefab is missing or not assigned at index: " + selectedCharacter);
            return;
        }

        GameObject clone = Instantiate(prefab, spawnPoint.position, Quaternion.identity);
        //label.text = prefab.name;

        if (spawnPoint == null)
        {
            Debug.LogError("Spawn point is missing! Assign it in the Inspector.");
            return;
        }


    }

}
