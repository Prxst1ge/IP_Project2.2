/*
 * Script Name: TransitionManager.cs
 * Student Name: Joel Wong Wan Hao
 * Date: 12/02/2026
 * Description: Controls the transition between scenes in a VR environment.
 */
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

    private Canvas myCanvas; // Reference to the Canvas component

    /// <summary>
    ///  Awake is called when the script instance is being loaded. We use it to set up our singleton and find the Canvas component.
    /// </summary>
    private void Awake()
    {
        // Singleton Pattern
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            // Find the Canvas component immediately so we can use it later
            myCanvas = GetComponent<Canvas>();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// OnSceneLoaded is called every time a new scene is loaded. We use it to update the Canvas's worldCamera reference to the new scene's main camera.
    /// </summary>
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Camera newCamera = Camera.main;

        // Safety Check: If we somehow lost the canvas reference, find it again
        if (myCanvas == null) myCanvas = GetComponent<Canvas>();

        if (newCamera != null && myCanvas != null)
        {
            myCanvas.worldCamera = newCamera;
            myCanvas.planeDistance = 0.4f;
        }
        else
        {
            Debug.LogWarning("VRTransitionManager: Could not find MainCamera or Canvas!");
        }
    }

    /// <summary>
    /// OnEnable is called when the object becomes enabled and active. We use it to start listening for scene load events.
    /// </summary>
    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    /// <summary>
    /// OnDisable is called when the behaviour becomes disabled or inactive. We use it to stop listening for scene load events to prevent memory leaks.
    /// </summary>
    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    /// <summary>
    /// Start is called before the first frame update. We use it to initialize the curtains and logo to their starting positions (curtains open, logo hidden).
    /// </summary>
    private void Start()
    {
        // Curtains open (Scale 0), Logo hidden (Top)
        SetCurtainScale(0f);
        if (logoImage != null)
            logoImage.anchoredPosition = new Vector2(0, logoHiddenY);
    }

    /// <summary>
    /// LoadSceneWithTransition is the public method that other scripts can call to start a scene transition. It takes the name of the scene to load as a parameter and starts the transition coroutine.
    /// </summary>
    public void LoadSceneWithTransition(string sceneName)
    {
        StartCoroutine(TransitionRoutine(sceneName));
    }

    /// <summary>
    /// TransitionRoutine is the main coroutine that handles the entire transition process. It first animates the curtains closing and the logo dropping, then loads the new scene asynchronously, waits for it to finish loading, and finally animates the curtains opening and the logo rising back up.
    /// </summary>
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

    /// <summary>
    /// AnimateTransition is a coroutine that animates the curtains and logo either closing (if isClosing is true) or opening (if isClosing is false). It uses a smooth step interpolation to make the movement feel natural. The curtains scale on the X axis from 0 to 1 when closing, and from 1 to 0 when opening. The logo moves vertically from hiddenY to visibleY when closing, and back up when opening.
    /// </summary>
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

    /// <summary>
    /// SetCurtainScale is a helper method that sets the scale of both curtains on the X axis. A scale of 0 means the curtains are fully open (invisible), and a scale of 1 means the curtains are fully closed (covering the screen). We only modify the X scale to create a horizontal opening/closing effect, while keeping the Y scale at 1 to maintain full height.
    /// </summary>
    private void SetCurtainScale(float scaleX)
    {
        // Apply scale to X axis only
        if (leftCurtain) leftCurtain.localScale = new Vector3(scaleX, 1, 1);
        if (rightCurtain) rightCurtain.localScale = new Vector3(scaleX, 1, 1);
    }
}
