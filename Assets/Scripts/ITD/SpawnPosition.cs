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
    public Transform player;      // Your Player object
    public Transform spawnPoint;  // The Empty GameObject you created

    void Start()
    {
        MovePlayerToSpawn();
    }

    void MovePlayerToSpawn()
    {
        if (player == null || spawnPoint == null)
        {
            Debug.LogError("SpawnManager: You forgot to assign the Player or Spawn Point in the Inspector!");
            return;
        }

        // 1. If the player uses a CharacterController, we must disable it briefly
        // or the physics engine might override our teleportation.
        CharacterController cc = player.GetComponent<CharacterController>();
        if (cc != null) cc.enabled = false;

        // 2. Move position and rotation to match the Empty
        player.position = spawnPoint.position;
        player.rotation = spawnPoint.rotation;

        // 3. Re-enable CharacterController
        if (cc != null) cc.enabled = true;

        Debug.Log("Player moved to Spawn Point");
    }
}