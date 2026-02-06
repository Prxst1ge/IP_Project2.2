using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Firebase.Storage;
using System.Threading.Tasks;
using System;
using System.Collections;

public class AchievementNotificationUI : MonoBehaviour
{
    public static AchievementNotificationUI Instance;

    [Header("UI Elements")]
    [SerializeField] private GameObject notificationPanel;
    [SerializeField] private TextMeshProUGUI achievementNameText;
    [SerializeField] private TextMeshProUGUI achievementDescriptionText;
    [SerializeField] private Image achievementBadgeImage;
    [SerializeField] private Image placeholderImage; // Optional: shown while loading

    [Header("Animation Settings")]
    [SerializeField] private float displayDuration = 5f;
    [SerializeField] private float fadeInDuration = 0.5f;
    [SerializeField] private float fadeOutDuration = 0.5f;

    [Header("Audio")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip achievementUnlockedSound;

    private CanvasGroup canvasGroup;
    private bool isShowing = false;
    private Coroutine hideCoroutine;

    void Awake()
    {
        // Singleton pattern
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        // Get or add CanvasGroup for fade effects
        canvasGroup = notificationPanel.GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            canvasGroup = notificationPanel.AddComponent<CanvasGroup>();
        }

        // Hide notification panel initially
        HideImmediate();
    }

    /// <summary>
    /// Show achievement notification with badge image from Firebase Storage
    /// </summary>
    public async void ShowAchievement(string achievementName, string description, string firebaseImagePath)
    {
        // If already showing, cancel previous hide coroutine
        if (hideCoroutine != null)
        {
            StopCoroutine(hideCoroutine);
        }

        isShowing = true;

        // Set text
        if (achievementNameText != null)
        {
            achievementNameText.text = achievementName;
        }
        if (achievementDescriptionText != null)
        {
            achievementDescriptionText.text = description;
        }

        // Show placeholder while loading image
        if (placeholderImage != null)
        {
            placeholderImage.gameObject.SetActive(true);
        }
        if (achievementBadgeImage != null)
        {
            achievementBadgeImage.gameObject.SetActive(false);
        }

        // Show panel with fade in
        notificationPanel.SetActive(true);
        StartCoroutine(FadeIn());

        // Play sound effect
        PlayAchievementSound();

        // Load image from Firebase Storage
        await LoadBadgeImage(firebaseImagePath);

        // Auto-hide after duration
        hideCoroutine = StartCoroutine(AutoHideAfterDelay());
    }

    /// <summary>
    /// Load achievement badge image from Firebase Storage
    /// </summary>
    private async Task LoadBadgeImage(string firebaseImagePath)
    {
        try
        {
            if (Database.Instance == null)
            {
                Debug.LogError("Database instance not found. Ensure the Database GameObject exists in the scene.");
                return;
            }

            if (!Database.Instance.IsStorageConfigured())
            {
                Debug.LogError("Firebase Storage not configured. Set the Storage Bucket URL in the Database component.");
                return;
            }

            Debug.Log($"Loading achievement badge from: {firebaseImagePath}");

            // Get Firebase Storage reference
            FirebaseStorage storage = FirebaseStorage.DefaultInstance;
            string fullUrl = Database.Instance.GetFullStorageUrl(firebaseImagePath);
            StorageReference storageRef = storage.GetReferenceFromUrl(fullUrl);

            // Download image as byte array
            byte[] imageData = await storageRef.GetBytesAsync(1024 * 1024 * 5); // Max 5MB

            // Create texture from bytes
            Texture2D texture = new Texture2D(2, 2);
            if (texture.LoadImage(imageData))
            {
                // Create sprite from texture
                Sprite sprite = Sprite.Create(
                    texture,
                    new Rect(0, 0, texture.width, texture.height),
                    new Vector2(0.5f, 0.5f)
                );

                // Apply to image
                if (achievementBadgeImage != null)
                {
                    achievementBadgeImage.sprite = sprite;
                    achievementBadgeImage.gameObject.SetActive(true);
                }

                // Hide placeholder
                if (placeholderImage != null)
                {
                    placeholderImage.gameObject.SetActive(false);
                }

                Debug.Log("Achievement badge loaded successfully");
            }
            else
            {
                Debug.LogError("Failed to load texture from image data");
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"Error loading achievement badge: {ex.Message}");
            
            // Keep placeholder visible if image load fails
            if (placeholderImage != null)
            {
                placeholderImage.gameObject.SetActive(true);
            }
        }
    }

    /// <summary>
    /// Play achievement unlock sound effect
    /// </summary>
    private void PlayAchievementSound()
    {
        if (audioSource != null && achievementUnlockedSound != null)
        {
            audioSource.PlayOneShot(achievementUnlockedSound);
            Debug.Log("Playing achievement sound effect");
        }
        else
        {
            Debug.LogWarning("AudioSource or AudioClip not assigned for achievement notification");
        }
    }

    /// <summary>
    /// Fade in the notification panel
    /// </summary>
    private IEnumerator FadeIn()
    {
        float elapsed = 0f;
        canvasGroup.alpha = 0f;

        while (elapsed < fadeInDuration)
        {
            elapsed += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(0f, 1f, elapsed / fadeInDuration);
            yield return null;
        }

        canvasGroup.alpha = 1f;
    }

    /// <summary>
    /// Fade out the notification panel
    /// </summary>
    private IEnumerator FadeOut()
    {
        float elapsed = 0f;
        float startAlpha = canvasGroup.alpha;

        while (elapsed < fadeOutDuration)
        {
            elapsed += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(startAlpha, 0f, elapsed / fadeOutDuration);
            yield return null;
        }

        canvasGroup.alpha = 0f;
        HideImmediate();
    }

    /// <summary>
    /// Auto-hide notification after display duration
    /// </summary>
    private IEnumerator AutoHideAfterDelay()
    {
        yield return new WaitForSeconds(displayDuration);
        yield return StartCoroutine(FadeOut());
        isShowing = false;
    }

    /// <summary>
    /// Immediately hide the notification without animation
    /// </summary>
    private void HideImmediate()
    {
        notificationPanel.SetActive(false);
        canvasGroup.alpha = 0f;
        isShowing = false;
    }

    /// <summary>
    /// Manually hide the notification (can be called by button)
    /// </summary>
    public void HideNotification()
    {
        if (hideCoroutine != null)
        {
            StopCoroutine(hideCoroutine);
        }
        StartCoroutine(FadeOut());
    }
}
