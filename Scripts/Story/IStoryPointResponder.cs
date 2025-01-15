public interface IStoryPointResponder
{
    /// <summary>
    /// Called when a story point starts.
    /// </summary>
    /// <param name="order">The order of the story point.</param>
    public void OnStoryPointEpisodeStart(int order);

    /// <summary>
    /// Called when the responder has completed its task.
    /// </summary>
    public void Done();

    /// <summary>
    /// Gets the order associated with this responder.
    /// </summary>
    /// <returns>The order number.</returns>
    public int GetOrder();
    public string GetName(); 
}