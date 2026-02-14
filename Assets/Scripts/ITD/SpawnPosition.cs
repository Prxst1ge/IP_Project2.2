/*
 * Script Name: SpawnPosition.cs
 * Student Name: Joel Wong Wan Hao
 * Date: 03/02/2026
 * Description: Forces the player to spawn at this position when the scene starts.
 */
using UnityEngine;
using System.Collections;

public class SpawnPosition : MonoBehaviour
{
    [Header("Drag Objects Here")]
    public Transform player;      // Player object
    public Transform spawnPoint;  // The Empty GameObject you created

    /// <summary>
    /// Called when the scene starts to move the player to the spawn position.
    /// </summary>
    void Start()
    {
        MovePlayerToSpawn();
    }

    /// <summary>
    /// Moves the player to the spawn point.
    /// </summary>
    void MovePlayerToSpawn()
    {
        if (player == null || spawnPoint == null)
        {
            Debug.LogError("SpawnManager: You forgot to assign the Player or Spawn Point in the Inspector!");
            return;
        }

        // If the player uses a CharacterController will disable it briefly
        CharacterController cc = player.GetComponent<CharacterController>();
        if (cc != null) cc.enabled = false;

        // Move position and rotation to match the Empty
        player.position = spawnPoint.position;
        player.rotation = spawnPoint.rotation;

        // Re-enable CharacterController
        if (cc != null) cc.enabled = true;
        {
            Debug.Log("Player CharacterController re-enabled.");
        }

        Debug.Log("Player moved to Spawn Point");
    }

}