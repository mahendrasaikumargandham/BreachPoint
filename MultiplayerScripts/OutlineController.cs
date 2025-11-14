using UnityEngine;
using Photon.Pun;

public class OutlineController : MonoBehaviour
{
    // A static reference to the local player's ShootingController. 
    // This makes it very easy for any script to find the local player.
    public static ShootingController localPlayerController;

    // We need a reference to the renderer on the child object.
    // It's a SkinnedMeshRenderer because it's an animated character model.
    private SkinnedMeshRenderer bodyRenderer;

    // This is the most efficient way to change material properties at runtime.
    private MaterialPropertyBlock propBlock;
    
    // This player's team number.
    private int playerTeam = 0; // Initialize to 0 (no team)

    private PhotonView view;

    void Awake()
    {
        view = GetComponent<PhotonView>();
        propBlock = new MaterialPropertyBlock();

        // Find the "BodyI_mesh" child object specifically and get its renderer.
        Transform bodyTransform = transform.Find("Bodyl_mesh");
        if (bodyTransform != null)
        {
            bodyRenderer = bodyTransform.GetComponent<SkinnedMeshRenderer>();
        }
        else
        {
            Debug.LogError("Could not find 'BodyI_mesh' child object on " + gameObject.name, this);
        }
    }

    void Start()
    {
        // If this component is on the player that I control...
        if (view.IsMine)
        {
            // ...set the static reference so other players can find me.
            localPlayerController = GetComponent<ShootingController>();
        }

        // Get the team number from Photon Custom Properties, just like in ShootingController.
        if (view.Owner.CustomProperties.ContainsKey("Team"))
        {
            playerTeam = (int)view.Owner.CustomProperties["Team"];
        }
    }

    void Update()
    {
        // --- GUARD CLAUSES ---
        // Don't run the logic if things aren't ready yet.
        if (localPlayerController == null || bodyRenderer == null || playerTeam == 0)
        {
            return;
        }

        // Don't show an outline on my own character.
        if (view.IsMine)
        {
            // Ensure the outline is off for the local player.
            SetOutline(false);
            return;
        }

        // --- CORE LOGIC ---
        // An enemy is someone whose team is different from the local player's team.
        bool isEnemy = playerTeam != localPlayerController.playerTeam;

        // Enable or disable the outline based on whether they are an enemy.
        SetOutline(isEnemy);
    }

    /// <summary>
    /// Applies the outline state to the material using a MaterialPropertyBlock.
    /// </summary>
    /// <param name="enabled">True to show the outline, false to hide it.</param>
    void SetOutline(bool enabled)
    {
        // Get the current properties from the renderer to avoid overwriting others.
        bodyRenderer.GetPropertyBlock(propBlock);
        
        // Set our custom shader property. The value is 1.0f if enabled, 0.0f if disabled.
        propBlock.SetFloat("_OutlineEnabled", enabled ? 1.0f : 0.0f);
        
        // Apply the modified properties back to the renderer.
        bodyRenderer.SetPropertyBlock(propBlock);
    }
}


