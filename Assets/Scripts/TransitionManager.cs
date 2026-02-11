using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class TransitionManager : MonoBehaviour
{
    public static TransitionManager Instance;


    public RectTransform leftCurtain;  // The left black panel
    public RectTransform rightCurtain; // The right black panel
    public RectTransform logoImage;    // The logo image (optional)


    public float transitionDuration = 1.0f; // Time for curtains to close/open
    public float logoHiddenY = 400f;        // Y Position above the screen
    public float logoVisibleY = 0f;         // Y Position in center of screen

    private void Awake()
    {
        // Ensures this script is accessible from anywhere
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Keep the object alive when changing scenes
        }
        else
        {
            Destroy(gameObject); // Destroy duplicate managers if scene reloads
        }
    }

    private void Start()
    {
        // Curtains open (Scale 0), Logo hidden (Top)
        SetCurtainScale(0f);
        if (logoImage != null)
            logoImage.anchoredPosition = new Vector2(0, logoHiddenY);
    }

    // Call this function from your door script
    public void LoadSceneWithTransition(string sceneName)
    {
        StartCoroutine(TransitionRoutine(sceneName));
    }

    private IEnumerator TransitionRoutine(string sceneName)
    {
        // Close Curtains & Drop Logo
        yield return StartCoroutine(AnimateTransition(true));

        // Load the Scene Asynchronously
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName);

        // Wait until scene is fully loaded
        while (!operation.isDone)
        {
            yield return null;
        }

        // Short pause to let player stabilize in new scene
        yield return new WaitForSeconds(0.5f);

        // Open Curtains & Raise Logo
        yield return StartCoroutine(AnimateTransition(false));
    }

    private IEnumerator AnimateTransition(bool isClosing)
    {
        float elapsedTime = 0f;

        // Define Start and End values based on if we are opening or closing
        float startScale = isClosing ? 0f : 1f;
        float endScale = isClosing ? 1f : 0f;

        float startLogoY = isClosing ? logoHiddenY : logoVisibleY;
        float endLogoY = isClosing ? logoVisibleY : logoHiddenY;

        while (elapsedTime < transitionDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / transitionDuration;

            // SmoothStep makes movement feel natural (slow start/end)
            float smoothT = Mathf.SmoothStep(0, 1, t);

            // Animate Curtain Width (Scale X)
            float currentScale = Mathf.Lerp(startScale, endScale, smoothT);
            SetCurtainScale(currentScale);

            // Animate Logo Position (Y)
            if (logoImage != null)
            {
                float currentY = Mathf.Lerp(startLogoY, endLogoY, smoothT);
                logoImage.anchoredPosition = new Vector2(0, currentY);
            }

            yield return null;
        }

        // Ensure exact final values
        SetCurtainScale(endScale);
        if (logoImage != null) logoImage.anchoredPosition = new Vector2(0, endLogoY);
    }

    private void SetCurtainScale(float scaleX)
    {
        // Apply scale to X axis only
        if (leftCurtain) leftCurtain.localScale = new Vector3(scaleX, 1, 1);
        if (rightCurtain) rightCurtain.localScale = new Vector3(scaleX, 1, 1);
    }
}
