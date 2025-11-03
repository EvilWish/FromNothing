public interface IInteractable
{
    string GetInteractionPrompt(PlayerInteraction ctx);
    bool CanInteract(PlayerInteraction ctx);
    void Interact(PlayerInteraction ctx);
}

public interface IFocusable
{
    void OnFocus(PlayerInteraction ctx);
    void OnBlur(PlayerInteraction ctx);
}
