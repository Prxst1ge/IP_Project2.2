using UnityEngine;
using UnityEngine.UI;
using Firebase.Database;
using Firebase.Auth;
using Firebase.Storage;
using System.Collections.Generic;
using System.Threading.Tasks;

public class AchievementMenuUI : MonoBehaviour
{
    [System.Serializable]
    public class AchievementUI
    {
        public string achievementId;
        public Image placeholderImage;
        public string placeholderImagePath;
    }

    [SerializeField]
    private List<AchievementUI> achievementUIList = new List<AchievementUI>();

    [SerializeField]
    private string achievementStorageBasePath = "achievements/";

    private DatabaseReference databaseReference;
    private FirebaseAuth auth;
    private FirebaseStorage storage;
    private Dictionary<string, Sprite> imageCache = new Dictionary<string, Sprite>();

    private async void Start()
    {
        InitializeFirebase();
        await LoadAndDisplayAchievements();
    }

    private void InitializeFirebase()
    {
        databaseReference = FirebaseDatabase.DefaultInstance.RootReference;
        auth = FirebaseAuth.DefaultInstance;
        storage = FirebaseStorage.DefaultInstance;
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
                // Construct the badge path from achievement ID
                string badgePath = achievementStorageBasePath + achievementUI.achievementId + ".png";
                Sprite badgeSprite = await LoadImageFromFirebaseStorage(badgePath);
                if (badgeSprite != null)
                {
                    achievementUI.placeholderImage.sprite = badgeSprite;
                }
            }
            else if (!string.IsNullOrEmpty(achievementUI.placeholderImagePath))
            {
                // Load placeholder if not unlocked
                Sprite placeholderSprite = await LoadImageFromFirebaseStorage(achievementUI.placeholderImagePath);
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
    public void AddAchievementUI(string achievementId, Image targetImage, string placeholderPath)
    {
        AchievementUI newAchievement = new AchievementUI
        {
            achievementId = achievementId,
            placeholderImage = targetImage,
            placeholderImagePath = placeholderPath
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
