using UnityEngine;

public class ResourceInteraction : MonoBehaviour, IInteractable
{
    [SerializeField] private string interactionPrompt;
    [SerializeField] private ResourceData interactionData;

    public bool CanInteract(PlayerInteraction ctx)
    {
        return true;
    }

    public string GetInteractionPrompt(PlayerInteraction ctx)
    {
        return interactionPrompt;
    }

    public void Interact(PlayerInteraction ctx)
    {
        Debug.Log($"Resource collected: {interactionData.resItem.itemName}");
        Destroy(gameObject);
    }
}
