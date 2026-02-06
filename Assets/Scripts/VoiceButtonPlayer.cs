using UnityEngine;

public class VoiceButtonPlayer : MonoBehaviour
{
    [Header("Assign in Inspector")]
    public AudioSource audioSource;
    public AudioClip voiceClip;

    public void PlayVoice()
    {
        if (audioSource == null)
        {
            Debug.LogWarning("VoiceButtonPlayer: AudioSource is NOT assigned.");
            return;
        }

        if (voiceClip == null)
        {
            Debug.LogWarning("VoiceButtonPlayer: VoiceClip is NOT assigned.");
            return;
        }

        Debug.Log("VoiceButtonPlayer: PlayVoice() called.");

        // Restart the voice if button is clicked again
        audioSource.Stop();
        audioSource.clip = voiceClip;
        audioSource.Play();

        Debug.Log("VoiceButtonPlayer: Voice is now PLAYING.");
    }

    public void StopVoice()
    {
        if (audioSource == null)
        {
            Debug.LogWarning("VoiceButtonPlayer: AudioSource is NOT assigned.");
            return;
        }

        if (audioSource.isPlaying)
        {
            audioSource.Stop();
            Debug.Log("VoiceButtonPlayer: Voice STOPPED.");
        }
        else
        {
            Debug.Log("VoiceButtonPlayer: StopVoice() called, but nothing was playing.");
        }
    }

    public void ToggleVoice()
    {
        if (audioSource == null)
        {
            Debug.LogWarning("VoiceButtonPlayer: AudioSource is NOT assigned.");
            return;
        }

        if (voiceClip == null)
        {
            Debug.LogWarning("VoiceButtonPlayer: VoiceClip is NOT assigned.");
            return;
        }

        if (audioSource.isPlaying)
        {
            audioSource.Stop();
            Debug.Log("VoiceButtonPlayer: ToggleVoice() → STOPPED.");
        }
        else
        {
            audioSource.clip = voiceClip;
            audioSource.Play();
            Debug.Log("VoiceButtonPlayer: ToggleVoice() → PLAYING.");
        }
    }
}