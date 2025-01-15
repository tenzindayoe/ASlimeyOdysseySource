// // StoryPointInvoker.cs
// using UnityEngine;
// using UnityEngine.Events;
// using System.Collections.Generic;
// using System.Linq;
// using Obi;


// [System.Serializable]
// public struct Episode{
//         public int order; 
//         public List<string> storyResponders; 
// }

// [System.Serializable]

// public enum TriggerType{
//     OnStart, 
//     OnTriggerEnter,
//     OnTriggerExit,
//     ViaScript
// }

// [RequireComponent(typeof(Collider))]
// public class StoryPointInvoker : MonoBehaviour
// {
//     [Header("Story Point Episodes - Visualize the order of the episodes")]
//     public List<Episode> episodes = new List<Episode>();

//     [Header("Story Point Settings")]
//     [Tooltip("Description of the story point.")]
//     public string StoryPointDescription;

//     [Header("Is One Time Play ? ")]
//     public bool OneTime = true;
//     [Header("Is the story point playing ? ")]
//     public bool isPlaying = false;
//     private bool isFinished = false;


//     public TriggerType SPInvocationType = TriggerType.OnTriggerEnter;

//     [HideInInspector]
    
//     public UnityEvent<int> onStoryPointStart = new UnityEvent<int>();

//     // Internal tracking
//     private Collider triggerCollider;
    

//     //make it assignable in the editor

    
//     public List<GameObject> storyPointResponders = new List<GameObject>();
//     public Dictionary<int, List<IStoryPointResponder>> storyPointRespondersDict = new Dictionary<int, List<IStoryPointResponder>>();    
//     public List<int> storyPointOrderList = new List<int>();
//     public int currentEpisodeIndex = 0 ; 
    
    

//     //maps from episode order to the number of responders in that episode
//     public Dictionary<int, int> doneResponders = new Dictionary<int, int>();
//     //maps from episode order to the number of responders in that episode
//     public Dictionary<int, int> totalResponders = new Dictionary<int, int>();
    

//     void Start(){
//         triggerCollider = GetComponent<Collider>();
//         triggerCollider.isTrigger = true;

//         if(triggerCollider == null){
//             Debug.LogError("The object " + this.gameObject.name + " does not have a collider attached to it. Please attach a collider to the object to use the StoryPointInvoker script");
//         }

//         SetupStoryPointResponders(); 


//         if(SPInvocationType == TriggerType.OnStart){
//             isPlaying = true;
//             isFinished = false;
//             FirstCall();
//         }
//     }


//     public void InvokeViaScript(){
//         if(SPInvocationType == TriggerType.ViaScript){
//             isPlaying = true;
//             isFinished = false;
//             FirstCall();
//         }
//     }


//     private void SetupStoryPointResponders()
//     {
//         // Clear existing episodes
//         episodes.Clear();

//         // Dictionary to group responders by order
//         Dictionary<int, List<string>> respondersGrouped = new Dictionary<int, List<string>>();

//         foreach (GameObject responderGO in storyPointResponders)
//         {
//             if (responderGO == null)
//             {
//                 Debug.LogWarning($"A responder GameObject is not assigned in '{gameObject.name}'.");
//                 continue;
//             }

//             IStoryPointResponder responder = responderGO.GetComponent<IStoryPointResponder>();
//             if (responder == null)
//             {
//                 Debug.LogError($"GameObject '{responderGO.name}' does not have a component implementing IStoryPointResponder.");
//                 continue;
//             }

//             onStoryPointStart.AddListener(responder.OnStoryPointEpisodeStart);

//             int order = responder.GetOrder();
//             string responderName = responder.GetName();

//             if (respondersGrouped.ContainsKey(order))
//             {
//                 respondersGrouped[order].Add(responderName);
//             }
//             else
//             {
//                 respondersGrouped.Add(order, new List<string> { responderName });
//             }
//         }

//         // Sort the orders in ascending order
//         List<int> sortedOrders = respondersGrouped.Keys.ToList();
//         sortedOrders.Sort();
//         storyPointOrderList = sortedOrders;

//         // Populate the episodes list
//         foreach (int order in sortedOrders)
//         {
//             episodes.Add(new Episode
//             {
//                 order = order,
//                 storyResponders = respondersGrouped[order]
//             });
//         }

//         //setup the done responders and total responders
//         foreach (int order in sortedOrders)
//         {
//             doneResponders.Add(order, 0);
//             totalResponders.Add(order, respondersGrouped[order].Count);
//         }
//         PrettyPrintDictionary(doneResponders, "doneResponders");
//         PrettyPrintDictionary(totalResponders, "totalResponders");
//     }
//     private void PrettyPrintDictionary(Dictionary<int, int> dict, string name)
//     {
//         Debug.Log($"Dictionary: {name}");
//         foreach (KeyValuePair<int, int> kvp in dict)
//         {
//             Debug.Log($"Key: {kvp.Key}, Value: {kvp.Value}");
//         }
//     }

//     public void ResponderDone(int episodeNumber, string name){
//         Debug.Log("Heard back from a responder " + episodeNumber.ToString() + " - " + name);
//         //Debug.Log("Heard back from a responder " + episodeNumber.ToString());
//         doneResponders[episodeNumber] += 1;
//         PrettyPrintDictionary(doneResponders, "doneResponders");
//         PrettyPrintDictionary(totalResponders, "totalResponders");
//         if(isPlaying){
//             OnResponderDone();
//         }
//         if (doneResponders[episodeNumber] > totalResponders[episodeNumber])
//         {
//             Debug.LogWarning($"ResponderDone called more times than expected for episode {episodeNumber}.");
//             return;
//         }
//     }

//     private void OnResponderDone(){
//         //check if all responders are done
//         int currentEpisode = storyPointOrderList[currentEpisodeIndex];
//         if(doneResponders[currentEpisode] == totalResponders[currentEpisode]){
            
//             //all responders are done
//             currentEpisodeIndex += 1;
//             if(currentEpisodeIndex == storyPointOrderList.Count){
//                 isPlaying = false;
//                 isFinished = true;
//                 return;
//             }else{
//                 //start the next episode
//                 int nextEpisode = storyPointOrderList[currentEpisodeIndex];
//                 onStoryPointStart.Invoke(nextEpisode);
//             }

//         }
//     }

//     private void FirstCall(){
//         currentEpisodeIndex = 0;
//         int currentEpisode = storyPointOrderList[currentEpisodeIndex];
//         onStoryPointStart.Invoke(currentEpisode);
//     }

    
//     /// <summary>
//     /// Automatically updates the episodes list in the Editor when changes are made.
//     /// </summary>
//     private void OnValidate()
// {
//     if (Application.isPlaying) return; // Only allow during non-play mode
//     SetupStoryPointResponders();
// }

//     void OnTriggerEnter(Collider other){

//         if(SPInvocationType == TriggerType.OnTriggerEnter){
//             if (other.gameObject.tag == "Player")
//             {
//                 if (OneTime && isFinished)
//                 {
//                     return;
//                 }

//                 if(!isPlaying){
//                     isPlaying = true;
//                     isFinished = false;
//                     FirstCall();

//                 }
                
//             }
//         }
//     }

//     void OnTriggerExit(Collider other){
//         if(SPInvocationType == TriggerType.OnTriggerExit){
//             if (other.gameObject.tag == "Player")
//             {
//                 if (OneTime && isFinished)
//                 {
//                     return;
//                 }

//                 if(!isPlaying){
//                     isPlaying = true;
//                     isFinished = false;
//                     FirstCall();

//                 }
                
//             }
//         }
//     }

//     private void StartStoryPointSequence()
//     {
//         if (episodes.Count == 0)
//         {
//             Debug.LogWarning($"No episodes to handle StoryPointInvoker '{gameObject.name}'.");
//             return;
//         }

//         foreach (Episode episode in episodes.OrderBy(e => e.order))
//         {
//             Debug.Log($"Starting Episode {episode.order}:");
//             foreach (string responderName in episode.storyResponders)
//             {
//                 Debug.Log($" - Responder: {responderName}");
//                 // Here you can implement the logic to invoke each responder
//                 // For example, find the responder by name and call its method
//             }
//         }

//         // Implement the actual functionality as needed
//     }


// }


using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;
using System.Linq;

[System.Serializable]
public struct Episode
{
    public int order;
    public List<string> storyResponders;
}

public enum TriggerType
{
    OnStart,
    OnTriggerEnter,
    OnTriggerExit,
    ViaScript
}

[RequireComponent(typeof(Collider))]
public class StoryPointInvoker : MonoBehaviour
{
    [Header("Story Point Episodes - Visualize the order of the episodes")]
    public List<Episode> episodes = new List<Episode>();

    [Header("Story Point Settings")]
    [Tooltip("Description of the story point.")]
    public string StoryPointDescription;

    [Header("Is One Time Play?")]
    public bool OneTime = true;
    [Header("Is the story point playing?")]
    public bool isPlaying = false;
    private bool isFinished = false;

    public TriggerType SPInvocationType = TriggerType.OnTriggerEnter;

    [HideInInspector]
    public UnityEvent<int> onStoryPointStart = new UnityEvent<int>();

    // Internal tracking
    private Collider triggerCollider;

    // Assignable in the editor
    public List<GameObject> storyPointResponders = new List<GameObject>();

    // Responders grouped by episode order
    private Dictionary<int, List<IStoryPointResponder>> storyPointRespondersDict = new Dictionary<int, List<IStoryPointResponder>>();
    private List<int> storyPointOrderList = new List<int>();
    private int currentEpisodeIndex = 0;

    // Maps from episode order to the number of responders in that episode
    private Dictionary<int, int> doneResponders = new Dictionary<int, int>();
    private Dictionary<int, int> totalResponders = new Dictionary<int, int>();

    void Start()
    {
        triggerCollider = GetComponent<Collider>();
        triggerCollider.isTrigger = true;

        if (triggerCollider == null)
        {
            Debug.LogError($"The object {this.gameObject.name} does not have a collider attached. Please attach a collider to use the StoryPointInvoker script.");
        }

        SetupStoryPointResponders();

        if (SPInvocationType == TriggerType.OnStart)
        {
            isPlaying = true;
            isFinished = false;
            FirstCall();
        }
    }

    public void InvokeViaScript()
    {
        if (SPInvocationType == TriggerType.ViaScript)
        {
            isPlaying = true;
            isFinished = false;
            FirstCall();
        }
    }

    private void SetupStoryPointResponders()
    {
        // Clear existing data
        episodes.Clear();
        storyPointRespondersDict.Clear();
        doneResponders.Clear();
        totalResponders.Clear();

        // Dictionary to group responders by order
        Dictionary<int, List<IStoryPointResponder>> respondersGrouped = new Dictionary<int, List<IStoryPointResponder>>();

        foreach (GameObject responderGO in storyPointResponders)
        {
            if (responderGO == null)
            {
                Debug.LogWarning($"A responder GameObject is not assigned in '{gameObject.name}'.");
                continue;
            }

            IStoryPointResponder responder = responderGO.GetComponent<IStoryPointResponder>();
            if (responder == null)
            {
                Debug.LogError($"GameObject '{responderGO.name}' does not have a component implementing IStoryPointResponder.");
                continue;
            }

            int order = responder.GetOrder();
            string responderName = responder.GetName();

            if (respondersGrouped.ContainsKey(order))
            {
                respondersGrouped[order].Add(responder);
            }
            else
            {
                respondersGrouped.Add(order, new List<IStoryPointResponder> { responder });
            }
        }

        // Sort the orders in ascending order
        List<int> sortedOrders = respondersGrouped.Keys.ToList();
        sortedOrders.Sort();
        storyPointOrderList = sortedOrders;

        // Populate the episodes list and dictionaries
        foreach (int order in sortedOrders)
        {
            if (doneResponders.ContainsKey(order))
            {
                Debug.LogError($"Duplicate episode order detected: {order}. Each episode must have a unique order.");
                continue;
            }

            List<IStoryPointResponder> responders = respondersGrouped[order];
            episodes.Add(new Episode
            {
                order = order,
                storyResponders = responders.Select(r => r.GetName()).ToList()
            });

            storyPointRespondersDict.Add(order, responders);
            doneResponders.Add(order, 0);
            totalResponders.Add(order, responders.Count);
        }

        //PrettyPrintDictionary(doneResponders, "doneResponders");
        //PrettyPrintDictionary(totalResponders, "totalResponders");
    }

    private void PrettyPrintDictionary(Dictionary<int, int> dict, string name)
    {
        Debug.Log($"Dictionary: {name}");
        foreach (KeyValuePair<int, int> kvp in dict)
        {
            Debug.Log($"Key: {kvp.Key}, Value: {kvp.Value}");
        }
    }

    public void ResponderDone(int episodeNumber, string name)
    {
        int currentEpisode = storyPointOrderList[currentEpisodeIndex];

        // Only process responder done if it matches the current episode
        if (episodeNumber != currentEpisode)
        {
            Debug.LogWarning($"Responder '{name}' for episode {episodeNumber} invoked during episode {currentEpisode}. Ignoring.");
            return;
        }

        Debug.Log($"Heard back from responder '{name}' for episode {episodeNumber}.");
        doneResponders[episodeNumber] += 1;
        //PrettyPrintDictionary(doneResponders, "doneResponders");
        //PrettyPrintDictionary(totalResponders, "totalResponders");

        if (isPlaying)
        {
            OnResponderDone();
        }

        if (doneResponders[episodeNumber] > totalResponders[episodeNumber])
        {
            Debug.LogWarning($"ResponderDone called more times than expected for episode {episodeNumber}.");
            return;
        }
    }

    private void OnResponderDone()
    {
        int currentEpisode = storyPointOrderList[currentEpisodeIndex];
        Debug.Log($"Checking completion for episode {currentEpisode}.");

        if (doneResponders[currentEpisode] == totalResponders[currentEpisode])
        {
            Debug.Log($"All responders for episode {currentEpisode} are done.");

            // Deactivate responders of the completed episode
            DeactivateRespondersOfEpisode(currentEpisode);

            currentEpisodeIndex += 1;

            if (currentEpisodeIndex == storyPointOrderList.Count)
            {
                isPlaying = false;
                isFinished = true;
                Debug.Log("All episodes completed.");
                return;
            }
            else
            {
                int nextEpisode = storyPointOrderList[currentEpisodeIndex];
                Debug.Log($"Starting next episode: {nextEpisode}");
                ActivateRespondersOfEpisode(nextEpisode);
            }
        }
        else
        {
            Debug.Log($"Episode {currentEpisode} is still in progress: {doneResponders[currentEpisode]}/{totalResponders[currentEpisode]} responders done.");
        }
    }

    private void FirstCall()
    {
        currentEpisodeIndex = 0;
        int currentEpisode = storyPointOrderList[currentEpisodeIndex];
        Debug.Log($"FirstCall: Starting episode {currentEpisode}");
        ActivateRespondersOfEpisode(currentEpisode);
    }

    private void ActivateRespondersOfEpisode(int episodeNumber)
    {
        if (!storyPointRespondersDict.ContainsKey(episodeNumber))
        {
            Debug.LogWarning($"No responders found for episode {episodeNumber} to activate.");
            return;
        }

        List<IStoryPointResponder> responders = storyPointRespondersDict[episodeNumber];

        foreach (var responder in responders)
        {
            // Enable the responder's GameObject if it's not already active
            MonoBehaviour responderMono = responder as MonoBehaviour;
            if (responderMono != null && !responderMono.gameObject.activeInHierarchy)
            {
                responderMono.gameObject.SetActive(true);
            }

            // Add listener for the current episode
            onStoryPointStart.AddListener(responder.OnStoryPointEpisodeStart);
        }

        // Invoke the event for the current episode
        onStoryPointStart.Invoke(episodeNumber);
    }

    private void DeactivateRespondersOfEpisode(int episodeNumber)
    {
        if (!storyPointRespondersDict.ContainsKey(episodeNumber))
        {
            Debug.LogWarning($"No responders found for episode {episodeNumber} to deactivate.");
            return;
        }

        List<IStoryPointResponder> responders = storyPointRespondersDict[episodeNumber];

        foreach (var responder in responders)
        {
            // Remove listener
            onStoryPointStart.RemoveListener(responder.OnStoryPointEpisodeStart);

            // Optionally, disable the responder's GameObject
            MonoBehaviour responderMono = responder as MonoBehaviour;
            if (responderMono != null)
            {
                Debug.Log($"Deactivating responder '{responder.GetName()}' of episode {episodeNumber}.");
                responderMono.gameObject.SetActive(false);
            }
        }

        // Optionally, clean up dictionaries if responders won't be reused
        // storyPointRespondersDict.Remove(episodeNumber);
        // doneResponders.Remove(episodeNumber);
        // totalResponders.Remove(episodeNumber);
    }

    /// <summary>
    /// Automatically updates the episodes list in the Editor when changes are made.
    /// </summary>
    private void OnValidate()
    {
        if (Application.isPlaying) return; // Only allow during non-play mode
        SetupStoryPointResponders();
    }

    void OnTriggerEnter(Collider other)
    {
        if (SPInvocationType == TriggerType.OnTriggerEnter)
        {
            if (other.gameObject.CompareTag("Player"))
            {
                if (OneTime && isFinished)
                {
                    return;
                }

                if (!isPlaying)
                {
                    isPlaying = true;
                    isFinished = false;
                    FirstCall();
                }
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (SPInvocationType == TriggerType.OnTriggerExit)
        {
            if (other.gameObject.CompareTag("Player"))
            {
                if (OneTime && isFinished)
                {
                    return;
                }

                if (!isPlaying)
                {
                    isPlaying = true;
                    isFinished = false;
                    FirstCall();
                }
            }
        }
    }

    private void StartStoryPointSequence()
    {
        if (episodes.Count == 0)
        {
            Debug.LogWarning($"No episodes to handle StoryPointInvoker '{gameObject.name}'.");
            return;
        }

        foreach (Episode episode in episodes.OrderBy(e => e.order))
        {
            Debug.Log($"Starting Episode {episode.order}:");
            foreach (string responderName in episode.storyResponders)
            {
                Debug.Log($" - Responder: {responderName}");
                // Implement the logic to invoke each responder if needed
            }
        }

        // Implement the actual functionality as needed
    }
}
