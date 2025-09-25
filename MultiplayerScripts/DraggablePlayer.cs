using UnityEngine;
using UnityEngine.EventSystems;
using Photon.Pun;
using Photon.Realtime;

// Add this script to your playerListItemPrefab
[RequireComponent(typeof(CanvasGroup))]
public class DraggablePlayer : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private CanvasGroup canvasGroup;
    private Transform originalParent;
    private Vector3 startPosition;

    // This will be set by the Launcher script
    public Player playerInfo;

    private bool isDraggable = false;

    void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
    }

    public void SetDraggable(bool canDrag)
    {
        isDraggable = canDrag;
    }



    public void OnBeginDrag(PointerEventData eventData)
    {
        if (!isDraggable) return;

        Debug.Log("Begin Drag");
        originalParent = transform.parent;
        startPosition = transform.position;
        
        // This allows the raycast to pass through the dragged object to detect the drop zone.
        canvasGroup.blocksRaycasts = false; 
        
        // Bring the item to the front so it renders over other UI elements.
        transform.SetParent(transform.root); 
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!isDraggable) return;
        
        // Make the item follow the mouse cursor.
        transform.position = eventData.position;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (!isDraggable) return;

        Debug.Log("End Drag");
        canvasGroup.blocksRaycasts = true;

        // If the item wasn't dropped on a valid DropZone, it will have no parent or the root canvas.
        // In that case, snap it back to its original position.
        if (transform.parent == originalParent || transform.parent == transform.root)
        {
            transform.SetParent(originalParent);
            transform.position = startPosition;
        }
    }
}
