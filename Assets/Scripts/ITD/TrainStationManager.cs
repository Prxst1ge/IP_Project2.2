/*
 * Script Name: TrainStationManager.cs
 * Student Name: Joel Wong Wan Hao
 * Date: 22/01/2026
 * Description: Controls the train station sequence.
 */
using UnityEngine;
using System.Collections;

public class TrainStationManager : MonoBehaviour
{
    [Header("Settings")]
    public float countdownTime = 30f;
    public AudioSource announcementAudio;

    [Header("Platform Triggers")]
    public GameObject correctPlatformCollider;
    public GameObject wrongPlatformCollider;

    private bool hasSequenceStarted = false;

    void Start()
    {
        // Ensure the choice colliders are hidden/disabled at start
        if (correctPlatformCollider) correctPlatformCollider.SetActive(false);
        if (wrongPlatformCollider) wrongPlatformCollider.SetActive(false);
    }

    // Called by the VRElevator via the Inspector UnityEvent
    public void StartStationSequence()
    {
        if (hasSequenceStarted) return; // Prevents restarting if player rides lift again

        hasSequenceStarted = true;
        Debug.Log("Sequence Started: 30 seconds until train arrives.");

        if (announcementAudio != null) announcementAudio.Play();

        StartCoroutine(CountdownRoutine());
    }

    private IEnumerator CountdownRoutine()
    {
        yield return new WaitForSeconds(countdownTime);

        // Turn on the box colliders so player can walk into them to choose
        if (correctPlatformCollider) correctPlatformCollider.SetActive(true);
        if (wrongPlatformCollider) wrongPlatformCollider.SetActive(true);

        Debug.Log("Platforms are now active! Make your choice.");
    }
}
