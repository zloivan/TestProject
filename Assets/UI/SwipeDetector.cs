using UnityEngine;
using UnityEngine.UI;

public class SwipeDetector : MonoBehaviour
{

    private SlidingPanel slidingPanel;
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private CanvasGroup deactiveGroup;


    private Color startColor;
    private void Awake()
    {
        slidingPanel = GetComponent<SlidingPanel>();
        slidingPanel.OnActivated.AddListener(OnAcivatedHandler);
        slidingPanel.OnDeactivated.AddListener(OnDeactivatedHandler);
        slidingPanel.OnPanelDrag.AddListener(UpdateVisibility);
        
        
        deactiveGroup.alpha = 0f;
    }

    private void UpdateVisibility(float arg0)
    {
        canvasGroup.alpha = arg0;
        deactiveGroup.alpha = 1 - arg0;
    }

    private void OnDeactivatedHandler()
    {
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
    }

    private void OnAcivatedHandler()
    {
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;
    }
}