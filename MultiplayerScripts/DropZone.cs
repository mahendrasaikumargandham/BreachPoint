using UnityEngine;
using UnityEngine.EventSystems;
using Photon.Pun;
using Photon.Realtime;

// Add this script to your Red Team and Blue Team panel objects
public class DropZone : MonoBehaviour, IDropHandler
{
    // Assign this in the Inspector: 1 for Red Team, 2 for Blue Team
    public int teamID; 

    public void OnDrop(PointerEventData eventData)
    {
        DraggablePlayer draggable = eventData.pointerDrag.GetComponent<DraggablePlayer>();
        
        if (draggable != null)
        {
            if (!PhotonNetwork.IsMasterClient) return;

            Debug.Log($"Dropped player {draggable.playerInfo.NickName} on Team {teamID}");

            // Set the parent of the dropped object to this drop zone's transform
            draggable.transform.SetParent(this.transform);

            // Tell the Launcher to handle the logic of changing the team
            Launcher.instance.RequestTeamChange(draggable.playerInfo, teamID);
        }
    }
}
