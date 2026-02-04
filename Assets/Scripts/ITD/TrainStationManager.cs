/*
 * Script Name: TrainStationManager.cs
 * Student Name: Joel Wong Wan Hao
 * Date: 30/01/2026
 * Description: Controls the train station sequence.
 */
using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class TrainStationManager : MonoBehaviour
{
    public float countdownTime = 30f; // Time in seconds before train arrives
    public AudioSource announcementAudio; // Audio source for announcement
    public AudioSource trainApproachAudio; // Audio source for train approaching sound
    public float approachSoundDelay = 20f; // Delay before train approach sound plays


    public GameObject correctPlatformCollider; // Collider for correct platform
    public GameObject wrongPlatformCollider; // Collider for wrong platform

    private bool hasSequenceStarted = false; // To prevent multiple starts


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
        StartCoroutine(PlayApproachSound());
    }

    private IEnumerator CountdownRoutine()
    {
        yield return new WaitForSeconds(countdownTime);

        // Turn on the box colliders so player can walk into them to choose
        if (correctPlatformCollider) correctPlatformCollider.SetActive(true);
        if (wrongPlatformCollider) wrongPlatformCollider.SetActive(true);

        Debug.Log("Platforms are now active! Make your choice.");
    }
    private IEnumerator PlayApproachSound()
    {
        // Wait for the specific delay
        yield return new WaitForSeconds(approachSoundDelay);

        // Play the sound
        trainApproachAudio.Play();
        Debug.Log("Train approaching sound playing!");
    }

}
