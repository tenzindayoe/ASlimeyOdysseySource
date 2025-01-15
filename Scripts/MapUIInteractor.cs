using UnityEngine;
using UnityEngine.InputSystem;

public class MapUIInteractor : MonoBehaviour
{
    [Header("Input Actions")]
    [SerializeField] private InputActionReference pointActionReference;
    [SerializeField] private InputActionReference leftClickActionReference;

    [Header("References")]
    [SerializeField] private Camera mainCamera;          // Assign your main camera in the Inspector
    [SerializeField] private Canvas canvas;              // Assign your Canvas in the Inspector
    [SerializeField] private RectTransform cursorUI;     // Reference to the UI Image RectTransform

    [Header("Cursor Properties")]
    public Vector3 cursorWorldPosition;                 // Stores the 3D world position of the cursor
    public Vector3 rayDirection;                        // Stores the ray direction from the camera to the cursor

    private Vector2 virtualMousePosition;               // 2D screen position of the virtual mouse
    private RectTransform canvasRectTransform;
    public LakeInfoPortal lastFocusedLake = null; // Tracks the last focused LakeMarkerInteraction object
   

    private void Awake()
    {
        

        // Cache the RectTransform of the Canvas for performance
        if (canvas != null)
        {
            canvasRectTransform = canvas.GetComponent<RectTransform>();
        }
        else
        {
            Debug.LogError("Canvas is not assigned in the inspector.");
        }
    }

    void Update()
    {
        // Read the position of the virtual mouse from the input system
        virtualMousePosition = pointActionReference.action?.ReadValue<Vector2>() ?? Vector2.zero;

        // Update the cursor position and calculate world position
        UpdateCursorUIPosition();

        // Perform raycasting
        if(lastFocusedLake == null || !lastFocusedLake.IsSelected){
            PerformRaycast();
        }
        

        // Handle left-click interaction if triggered
        if (leftClickActionReference.action?.triggered == true && lastFocusedLake!=null)
        {
            selectLake();
        }
    }

    private void UpdateCursorUIPosition()
    {
        if (mainCamera == null || canvas == null || canvasRectTransform == null || cursorUI == null)
        {
            Debug.LogError("Required references are not assigned.");
            return;
        }

        // Set the UI cursor position relative to the Canvas
        Vector2 localCursorPosition;
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
                canvasRectTransform,
                virtualMousePosition,
                mainCamera,
                out localCursorPosition))
        {
            // Update the UI cursor's anchored position
            cursorUI.anchoredPosition = localCursorPosition;

            // Convert the screen point to world space
            RectTransformUtility.ScreenPointToWorldPointInRectangle(
                canvasRectTransform,
                virtualMousePosition,
                mainCamera,
                out cursorWorldPosition
            );

            // Calculate the direction from the camera to the cursor's world position
            rayDirection = (cursorWorldPosition - mainCamera.transform.position).normalized;
        }
        else
        {
            Debug.LogWarning("Failed to convert virtual mouse position to UI local position.");
        }
    }

    private void PerformRaycast()
    {
        if (mainCamera == null)
        {
            Debug.LogError("Main Camera is not assigned.");
            return;
        }

        // Define the ray starting at the camera position and pointing toward the cursor's world position
        Vector3 rayOrigin = mainCamera.transform.position;

        // Perform the raycast
        Ray ray = new Ray(rayOrigin, rayDirection);
        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity))
        {
            var mapMarker = hit.collider.GetComponent<LakeInfoPortal>();

            if (mapMarker != null)
            {
                
                // Check if this is a new focus object
                if (lastFocusedLake != mapMarker)
                {
                    // Remove focus from the previous object
                    if (lastFocusedLake != null)
                    {
                        lastFocusedLake.OnFocusExit();
                    }

                    // Set focus to the current object
                    mapMarker.OnFocusEnter();
                    lastFocusedLake = mapMarker;
                }
            }
            else if (lastFocusedLake != null)
            {
                // Remove focus if no object is under the cursor
                lastFocusedLake.OnFocusExit();
                lastFocusedLake = null;
            }
        }
        else if (lastFocusedLake != null)
        {
            // Remove focus if the raycast does not hit any object
            lastFocusedLake.OnFocusExit();
            lastFocusedLake = null;
        }
    }

    //no other script should attempt to select a lake 
    private void selectLake()
    {
        // Perform the click interaction on the focused object
        lastFocusedLake.OnLeftClick();
        //MapUIGraphicsManager.Instance.displayLakeDetailedView();
        
    }
    //this may be called by ui and other scripts to unselect the currently focused lake. 
    public void unselectLake(){
        if (lastFocusedLake != null)
        {
            lastFocusedLake.unselect();
            lastFocusedLake.OnFocusExit();
            lastFocusedLake = null;
        }
    }
    
}