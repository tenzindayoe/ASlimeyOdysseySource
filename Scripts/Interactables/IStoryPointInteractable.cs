public interface IStoryPointInteractable
{
    void Interact();
    bool IsInteractable(); 
    void SetSignalID(int id);

    int GetSignalID();  

    void SendSignal();
}