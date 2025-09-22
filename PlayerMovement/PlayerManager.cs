using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PlayerManager : MonoBehaviour
{
    InputManager inputManager;
    PlayerMovement playerMovement;
    CameraManager cameraManager;
    Animator animator;
    PhotonView view;

    public bool isInteracting;

    void Awake() {
        inputManager = GetComponent<InputManager>();
        playerMovement = GetComponent<PlayerMovement>();
        animator = GetComponent<Animator>();
        view = GetComponent<PhotonView>();
        cameraManager = FindObjectOfType<CameraManager>();
    }

    void Start() {
        if(!view.IsMine) {
            Destroy(GetComponentInChildren<CameraManager>().gameObject);
        }
    }

    void Update() {
        if(view.IsMine)
            inputManager.HandleAllInputs();
    }

    void FixedUpdate() {
        if(view.IsMine)
            playerMovement.HandleAllMovement();
    }

    void LateUpdate() {
        if(view.IsMine) {
            cameraManager.HandleAllCameraMovement();
            isInteracting = animator.GetBool("isInteracting");
            playerMovement.isJumping = animator.GetBool("isJumping");
            animator.SetBool("isGrounded", playerMovement.isGrounded);
        }
    }
}
