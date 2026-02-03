/*
 * Script Name: SpawnPosition.cs
 * Student Name: Joel Wong Wan Hao
 * Date: 03/02/2026
 * Description: Forces the player to spawn at this position when the scene starts.
 */
using UnityEngine;

public class SpawnPosition : MonoBehaviour
{
    public GameObject playerRig; // Reference to the VR Player Rig

    void Start()
    {
        // This will force the player to this spawn position at the start of the scene
        if (playerRig == null)
        {
            playerRig = GameObject.FindGameObjectWithTag("Player");
        }

        if (playerRig != null)
        {
            // Teleport the player to THIS object's position
            playerRig.transform.position = transform.position;

            // Teleport the player to THIS object's rotation (facing direction)
            playerRig.transform.rotation = transform.rotation;

            Debug.Log("Player forced to spawn position: " + gameObject.name);
        }
        else
        {
            Debug.LogError("ForceSpawnPosition Error: No Player Rig found! Make sure your VR Rig is tagged 'Player'.");
        }
    }
}