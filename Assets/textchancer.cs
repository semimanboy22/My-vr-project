using UnityEngine;

using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Reflection;

[System.Serializable]
public class Page
{
    public string pageName;
    public List<GameObject> uiElements = new List<GameObject>();
}

public class textchancer : MonoBehaviour
{
    [SerializeField] private List<Page> pages = new List<Page>();
    [SerializeField] private int startPageIndex = 0;
    [SerializeField] private Button nextButton;
    [SerializeField] private Button previousButton;
    [SerializeField] private GameObject nextInteractable;
    [SerializeField] private GameObject previousInteractable;

    private int currentPageIndex = 0;

    void OnValidate()
    {
        // This runs in the editor to ensure non-selected pages are hidden
        if (!Application.isPlaying)
        {
            UpdatePageVisibility();
        }
    }

    void Start()
    {
        // Validate start page index
        if (startPageIndex >= 0 && startPageIndex < pages.Count)
        {
            currentPageIndex = startPageIndex;
        }
        else
        {
            currentPageIndex = 0;
        }

        // Setup UI button listeners
        if (nextButton != null)
        {
            nextButton.onClick.AddListener(NextPage);
        }

        if (previousButton != null)
        {
            previousButton.onClick.AddListener(PreviousPage);
        }

        // Setup XR Interactable listeners
        SetupXRInteractable(nextInteractable, NextPage);
        SetupXRInteractable(previousInteractable, PreviousPage);

        // Display the initial page
        ShowPage(currentPageIndex);
    }

    private void SetupXRInteractable(GameObject interactableObject, System.Action callback)
    {
        if (interactableObject == null)
            return;

        System.Type interactableType = System.Type.GetType("UnityEngine.XR.Interaction.Toolkit.XRSimpleInteractable, Unity.XR.Interaction.Toolkit");

        if (interactableType != null)
        {
            var interactable = interactableObject.GetComponent(interactableType);

            if (interactable != null)
            {
                // Get the selectEntered event
                var selectEnteredProperty = interactableType.GetProperty("selectEntered");
                if (selectEnteredProperty != null)
                {
                    var selectEnteredEvent = selectEnteredProperty.GetValue(interactable);
                    var addListenerMethod = selectEnteredEvent.GetType().GetMethod("AddListener");
                    if (addListenerMethod != null)
                    {
                        addListenerMethod.Invoke(selectEnteredEvent, new object[] { callback });
                        Debug.Log("Page navigation interactable connected successfully!");
                    }
                }
            }
        }
    }

    public void NextPage()
    {
        if (pages.Count > 0)
        {
            // Move to next page, or wrap around to first page if at the end
            currentPageIndex = (currentPageIndex + 1) % pages.Count;
            ShowPage(currentPageIndex);
        }
    }

    public void PreviousPage()
    {
        if (pages.Count > 0)
        {
            // Move to previous page, or wrap around to last page if at the beginning
            currentPageIndex = (currentPageIndex - 1 + pages.Count) % pages.Count;
            ShowPage(currentPageIndex);
        }
    }

    public void GoToPage(int pageIndex)
    {
        if (pageIndex >= 0 && pageIndex < pages.Count)
        {
            currentPageIndex = pageIndex;
            ShowPage(currentPageIndex);
        }
    }

    private void ShowPage(int pageIndex)
    {
        // Hide all pages first
        for (int i = 0; i < pages.Count; i++)
        {
            foreach (GameObject element in pages[i].uiElements)
            {
                if (element != null)
                {
                    element.SetActive(false);
                }
            }
        }

        // Show the current page
        if (pageIndex >= 0 && pageIndex < pages.Count)
        {
            foreach (GameObject element in pages[pageIndex].uiElements)
            {
                if (element != null)
                {
                    element.SetActive(true);
                }
            }
        }
    }

    private void UpdatePageVisibility()
    {
        // Hide all pages first
        for (int i = 0; i < pages.Count; i++)
        {
            foreach (GameObject element in pages[i].uiElements)
            {
                if (element != null)
                {
                    element.SetActive(false);
                }
            }
        }

        // Show only the start page
        if (startPageIndex >= 0 && startPageIndex < pages.Count)
        {
            foreach (GameObject element in pages[startPageIndex].uiElements)
            {
                if (element != null)
                {
                    element.SetActive(true);
                }
            }
        }
    }

    public int GetCurrentPageIndex()
    {
        return currentPageIndex;
    }

    public int GetPageCount()
    {
        return pages.Count;
    }
}
