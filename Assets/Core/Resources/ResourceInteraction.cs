using UnityEngine;

public class ResourceInteraction : MonoBehaviour, IInteractable
{
    [SerializeField] private string interactionPrompt;
    private ResourceData interactionData;
    private PlayerData pData;

    private void Start()
    {
        if (interactionData == null) interactionData = GetComponent<ResourceData>();
        if (pData == null) pData = FindFirstObjectByType<PlayerData>();
    }

    public bool CanInteract(PlayerInteraction ctx)
    {
        // TODO: Tool Check
        return InventoryManager.Instance.HasSpace();
    }

    public string GetInteractionPrompt(PlayerInteraction ctx)
    {
        return interactionPrompt;
    }

    public void Interact(PlayerInteraction ctx)
    {
        if (interactionData.resourceHealth > 0)
        {
            InventoryManager.Instance.AddItemToInventory(interactionData.resItem, 1);
            interactionData.resourceHealth--;
        }
        else
        {
            Destroy(this.gameObject);
        }
    }
}
