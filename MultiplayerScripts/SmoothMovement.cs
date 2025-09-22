using UnityEngine;
using Photon.Pun;

/// <summary>
/// This component's single responsibility is to smoothly interpolate the position and rotation
/// of remote player objects. It receives data from the network via OnPhotonSerializeView
/// and then uses the Update() loop to smoothly move the character towards that target.
/// Attach this to your player prefab.
/// </summary>
public class SmoothMovement : MonoBehaviour, IPunObservable
{
    // The latest network position and rotation received.
    private Vector3 networkPosition;
    private Quaternion networkRotation;

    [Tooltip("This value controls how quickly the character moves to its target position. Higher values are snappier, lower values are smoother.")]
    [Range(5, 25)]
    public float smoothing = 15.0f;

    private PhotonView photonView;

    void Awake()
    {
        photonView = GetComponent<PhotonView>();
        
        // This is a critical check. We only want this script to run on characters
        // that do NOT belong to us. Our own character is controlled directly by our input
        // and does not need smoothing. By disabling it, we save performance and prevent conflicts.
        if (photonView.IsMine)
        {
            this.enabled = false;
        }
    }

    /// <summary>
    /// In the Update loop, we continuously move our character's transform towards the
    /// last known network position. Time.deltaTime ensures the movement is smooth and
    /// frame-rate independent.
    /// </summary>
    void Update()
    {
        // This check is a failsafe. This logic should only ever run on remote players.
        if (!photonView.IsMine)
        {
            // Vector3.Lerp smoothly moves from the current position to the target (networkPosition).
            transform.position = Vector3.Lerp(transform.position, networkPosition, Time.deltaTime * smoothing);
            
            // Quaternion.Lerp does the same for rotation, ensuring the character turns smoothly.
            transform.rotation = Quaternion.Lerp(transform.rotation, networkRotation, Time.deltaTime * smoothing);
        }
    }

    /// <summary>
    /// This is the magic method called by Photon. It is responsible for both sending and
    /// receiving data across the network.
    /// </summary>
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            // This is our player. We are the authority. We send our actual position
            // and rotation to the network for others to receive.
            stream.SendNext(transform.position);
            stream.SendNext(transform.rotation);
        }
        else // stream.IsReading
        {
            // This is a remote player. We are receiving their data.
            // We store this data in our 'networkPosition' and 'networkRotation' variables.
            // The Update() method will then use this data to smoothly move the character.
            this.networkPosition = (Vector3)stream.ReceiveNext();
            this.networkRotation = (Quaternion)stream.ReceiveNext();
        }
    }
}

