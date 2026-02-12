using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Firebase.Database;
using Firebase.Auth;
using Firebase.Storage;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

public class AchievementMenuUI : MonoBehaviour
{
    [System.Serializable]
    public class AchievementUI
    {
        public string achievementId;
        public Image placeholderImage;
        [Tooltip("Optional: Override the automatic snake_case conversion with a custom badge filename. Leave empty to use auto-conversion.")]
        public string customBadgeName;
    }

    [SerializeField]
    private List<AchievementUI> achievementUIList = new List<AchievementUI>();

    [SerializeField]
    private TextMeshProUGUI playerDisplayNameText;

    [SerializeField]
    private string achievementStorageBasePath = "achievements/";

    [SerializeField]
    private string placeholderStorageBasePath = "PlaceholderImages/";

    [SerializeField]
    private float achievementCheckInterval = 5f; // Check for achievements every 5 seconds

    private DatabaseReference databaseReference;
    private FirebaseAuth auth;
    private FirebaseStorage storage;
    private Dictionary<string, Sprite> imageCache = new Dictionary<string, Sprite>();
    private Coroutine achievementCheckCoroutine;

    private async void Start()
    {
        InitializeFirebase();
        await LoadAndDisplayAchievements();
        StartPeriodicAchievementCheck();
    }

    private void OnDestroy()
    {
        StopPeriodicAchievementCheck();
    }

    private void InitializeFirebase()
    {
        databaseReference = FirebaseDatabase.DefaultInstance.RootReference;
        auth = FirebaseAuth.DefaultInstance;
        storage = FirebaseStorage.DefaultInstance;
    }

    private string ConvertToSnakeCase(string input)
    {
        string snakeCase = System.Text.RegularExpressions.Regex.Replace(input, "([a-z0-9])([A-Z])", "$1_$2").ToLower();
        return snakeCase;
    }

    private async Task LoadAndDisplayAchievements()
    {
        try
        {
            FirebaseUser currentUser = auth.CurrentUser;
            if (currentUser == null)
            {
                Debug.LogWarning("No user logged in. Cannot load achievements.");
                return;
            }

            string userId = currentUser.UserId;
            
            // Fetch player display name
            await LoadAndDisplayPlayerName(userId);
            
            string path = $"Game/Players/{userId}/Stats/AchievementsCollected";

            // Fetch achievements from Firebase
            DataSnapshot snapshot = await databaseReference.Child(path).GetValueAsync();

            if (snapshot.Exists)
            {
                Dictionary<string, object> achievementsData = snapshot.Value as Dictionary<string, object>;
                await UpdateAchievementUI(achievementsData);
            }
            else
            {
                Debug.LogWarning("No achievements found for this user.");
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Error loading achievements: {e.Message}");
        }
    }

    private async Task LoadAndDisplayPlayerName(string userId)
    {
        try
        {
            if (playerDisplayNameText == null)
            {
                Debug.LogWarning("Player Display Name Text is not assigned.");
                return;
            }

            string path = $"Game/Players/{userId}/Stats/DisplayName";
            DataSnapshot snapshot = await databaseReference.Child(path).GetValueAsync();

            if (snapshot.Exists && snapshot.Value != null)
            {
                string displayName = snapshot.Value.ToString();
                playerDisplayNameText.text = $"Hello {displayName}";
            }
            else
            {
                Debug.LogWarning("DisplayName not found for this user.");
                playerDisplayNameText.text = "Hello Unknown Player";
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Error loading player display name: {e.Message}");
            playerDisplayNameText.text = "Error Loading Name";
        }
    }

    private async Task UpdateAchievementUI(Dictionary<string, object> achievementsData)
    {
        foreach (AchievementUI achievementUI in achievementUIList)
        {
            // Ignore internal placeholder entries
            if (achievementUI.achievementId.StartsWith("_"))
            {
                continue;
            }

            bool isUnlocked = false;

            // Check if the achievement exists and is unlocked
            if (achievementsData.ContainsKey(achievementUI.achievementId))
            {
                object value = achievementsData[achievementUI.achievementId];
                if (value is bool boolValue)
                {
                    isUnlocked = boolValue;
                }
            }

            // Load and update the image based on unlock status
            if (isUnlocked)
            {
                // Use custom badge name if provided, otherwise convert to snake_case
                string badgeFileName = string.IsNullOrEmpty(achievementUI.customBadgeName)
                    ? ConvertToSnakeCase(achievementUI.achievementId)
                    : achievementUI.customBadgeName;
                string badgePath = achievementStorageBasePath + badgeFileName + ".png";
                Sprite badgeSprite = await LoadImageFromFirebaseStorage(badgePath);
                if (badgeSprite != null)
                {
                    achievementUI.placeholderImage.sprite = badgeSprite;
                }
            }
            else
            {
                // Use custom badge name if provided, otherwise convert to snake_case
                string lockedFileName = string.IsNullOrEmpty(achievementUI.customBadgeName)
                    ? ConvertToSnakeCase(achievementUI.achievementId) + "_locked"
                    : achievementUI.customBadgeName + "_locked";
                string lockedImagePath = placeholderStorageBasePath + lockedFileName + ".png";
                Sprite placeholderSprite = await LoadImageFromFirebaseStorage(lockedImagePath);
                if (placeholderSprite != null)
                {
                    achievementUI.placeholderImage.sprite = placeholderSprite;
                }
            }
        }
    }

    private async Task<Sprite> LoadImageFromFirebaseStorage(string imagePath)
    {
        try
        {
            // Check if image is already cached
            if (imageCache.ContainsKey(imagePath))
            {
                return imageCache[imagePath];
            }

            // Get the storage reference
            StorageReference imageRef = storage.GetReference(imagePath);

            // Download the image (max 10MB)
            long maxBytes = 10 * 1024 * 1024;
            byte[] imageData = await imageRef.GetBytesAsync(maxBytes);

            // Create a texture from the downloaded bytes
            Texture2D texture = new Texture2D(2, 2);
            texture.LoadImage(imageData);

            // Create a sprite from the texture
            Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.one * 0.5f);

            // Cache the sprite
            imageCache[imagePath] = sprite;

            return sprite;
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Error loading image from Firebase Storage ({imagePath}): {e.Message}");
            return null;
        }
    }

    /// <summary>
    /// Starts periodic checking for achievement updates from Firebase
    /// </summary>
    public void StartPeriodicAchievementCheck()
    {
        if (achievementCheckCoroutine != null)
        {
            Debug.LogWarning("Achievement check coroutine is already running.");
            return;
        }

        achievementCheckCoroutine = StartCoroutine(PeriodicAchievementCheckCoroutine());
    }

    /// <summary>
    /// Stops periodic checking for achievement updates
    /// </summary>
    public void StopPeriodicAchievementCheck()
    {
        if (achievementCheckCoroutine != null)
        {
            StopCoroutine(achievementCheckCoroutine);
            achievementCheckCoroutine = null;
        }
    }

    /// <summary>
    /// Coroutine that checks for achievement updates at regular intervals
    /// </summary>
    private IEnumerator PeriodicAchievementCheckCoroutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(achievementCheckInterval);
            _ = LoadAndDisplayAchievements(); // Fire and forget
        }
    }

    /// <summary>
    /// Call this method to manually refresh achievements from Firebase
    /// </summary>
    public async void RefreshAchievements()
    {
        await LoadAndDisplayAchievements();
    }

    /// <summary>
    /// Clear the image cache to free memory
    /// </summary>
    public void ClearImageCache()
    {
        foreach (var sprite in imageCache.Values)
        {
            Destroy(sprite.texture);
        }
        imageCache.Clear();
    }

    /// <summary>
    /// Add a new achievement UI dynamically
    /// </summary>
    public void AddAchievementUI(string achievementId, Image targetImage)
    {
        AddAchievementUI(achievementId, targetImage, "");
    }

    /// <summary>
    /// Add a new achievement UI dynamically with custom badge name
    /// </summary>
    public void AddAchievementUI(string achievementId, Image targetImage, string customBadgeName)
    {
        AchievementUI newAchievement = new AchievementUI
        {
            achievementId = achievementId,
            placeholderImage = targetImage,
            customBadgeName = customBadgeName
        };

        achievementUIList.Add(newAchievement);
    }

    /// <summary>
    /// Remove an achievement UI from the list
    /// </summary>
    public void RemoveAchievementUI(string achievementId)
    {
        achievementUIList.RemoveAll(a => a.achievementId == achievementId);
    }
}
