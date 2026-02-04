using UnityEngine;
using TMPro; // remove this line if you don't use TMP

public class PanelNavigator : MonoBehaviour
{
    [Header("Panels in order (0 = first screen)")]
    [SerializeField] private GameObject[] panels;

    [Header("Optional UI")]
    [SerializeField] private TMP_Text stepText; // e.g. "Step 2 of 3" (optional)

    private int currentIndex = 0;

    private void Start()
    {
        if (panels == null || panels.Length == 0)
        {
            Debug.LogWarning("PanelNavigator: No panels assigned!");
            return;
        }

        ShowPanel(0);
    }

    public void Next()
    {
        if (panels == null || panels.Length == 0) return;

        int nextIndex = currentIndex + 1;
        if (nextIndex >= panels.Length)
        {
            Debug.Log("PanelNavigator: Already at last panel.");
            return;
        }

        ShowPanel(nextIndex);
    }

    public void Previous()
    {
        if (panels == null || panels.Length == 0) return;

        int prevIndex = currentIndex - 1;
        if (prevIndex < 0)
        {
            Debug.Log("PanelNavigator: Already at first panel.");
            return;
        }

        ShowPanel(prevIndex);
    }

    public void GoTo(int index)
    {
        ShowPanel(index);
    }

    private void ShowPanel(int index)
    {
        if (index < 0 || index >= panels.Length)
        {
            Debug.LogWarning($"PanelNavigator: Index out of range: {index}");
            return;
        }

        // Turn all off
        for (int i = 0; i < panels.Length; i++)
        {
            if (panels[i] != null)
                panels[i].SetActive(false);
        }

        // Turn one on
        panels[index].SetActive(true);
        currentIndex = index;

        // Optional step text
        if (stepText != null)
        {
            stepText.text = $"Step {currentIndex + 1} of {panels.Length}";
        }

        Debug.Log($"PanelNavigator: Showing panel {currentIndex + 1}/{panels.Length}");
    }
}