using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

/// <summary>
/// Handles player interactions with objects in the game world using raycasting.
/// Displays interaction prompts and processes input for various interactable objects.
/// </summary>
public class PlayerInteraction : MonoBehaviour
{
    #region Interaction Settings
    [Header("Interaction")]
    [SerializeField] private float interactionDistance = 2f;
    [SerializeField] private LayerMask interactableLayerMask;
    #endregion

    #region UI References
    [Header("UI")]
    [SerializeField] private GameObject interactionPanel;
    [SerializeField] private TextMeshProUGUI interactionText;

    #endregion

    #region Private Variables
    private Camera cam;
    IInteractable current, last;
    #endregion

    #region Unity Lifecycle
    private void Awake()
    {
        cam = Camera.main;
        HideInteractionPanel();
    }

    private void Update()
    {
        current = HandleInteractionDetection(); // <- IInteractable or null

        if (current != last)
        {
            (last as IFocusable)?.OnBlur(this);
            (current as IFocusable)?.OnFocus(this);
            last = current;
        }

        string activePrompt = null;

        if (!string.IsNullOrWhiteSpace(activePrompt)) ShowInteractionPanel(activePrompt);
        else HideInteractionPanel();

        
        if (current != null
            && Keyboard.current != null
            && Keyboard.current.eKey.wasPressedThisFrame
            && current.CanInteract(this)
            && !IsPointerOverUI())
        {
            current.Interact(this);
        }
    }
    #endregion

    #region Interaction Logic
    private IInteractable HandleInteractionDetection()
    {
        Ray ray = new(cam.transform.position, cam.transform.forward);
        if (Physics.Raycast(ray, out RaycastHit hit, interactionDistance, interactableLayerMask))
        {
            return hit.collider.GetComponentInChildren<IInteractable>();
        }
        return null;
    }

    private void ShowInteractionPanel(string text)
    {
        if (interactionPanel == null || interactionText == null) return;
        interactionPanel.SetActive(true);
        interactionText.text = text;
    }

    private void HideInteractionPanel()
    {
        if (interactionPanel == null) return;
        interactionPanel.SetActive(false);
    }
    #endregion

    #region Helper Methods
    public bool IsPointerOverUI() => EventSystem.current.IsPointerOverGameObject();
    #endregion
}