/*
 * Script Name: SceneChanger.cs
 * Student Name: Joel Wong Wan Hao
 * Date: 22/01/2026
 * Description: this script manages scene transitions when the player enters a trigger zone.
 */
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChanger : MonoBehaviour
{
    // Name of the scene to load
    public string Scenechange = "Room 2 Escape";
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            SceneManager.LoadScene(Scenechange);
        }
    }
}
