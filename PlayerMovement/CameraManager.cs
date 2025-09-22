using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    public InputManager inputManager;
    public Transform playerTransform;
    public Transform cameraPivot;
    public Transform cameraTransform;

    [Header("Camera Movement")]
    private Vector3 cameraFollowVelocity = Vector3.zero;
    public float cameraFollowSpeed = 0.3f;

    public float cameraLookSpeed = 0.3f;
    public float cameraPivotSpeed = 0.3f;

    public float lookAngle;
    public float pivotAngle;

    public float minPivotAngle = -20f;
    public float maxPivotAngle = 20f;

    [Header("Camera Collision")]
    public LayerMask collisionLayer;
    private float defaultPosition;
    public float cameraCollisionOffset = 0.2f;
    public float minCollisionOffset = 0.2f;
    public float cameraCollisionRadius = 2f;

    private Vector3 cameraVectorPosition;

    [Header("Reference for player rotation standard")]
    private PlayerMovement playerMovement;

    [Header("Scoped Settings")]
    public float scoppedFOV = 20f;
    public float defaultFOV = 60f;
    public bool isScoped = false;
    public Camera camera;   

    void Awake()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        playerTransform = FindObjectOfType<PlayerManager>().transform;
        inputManager = FindObjectOfType<InputManager>();
        playerMovement = FindObjectOfType<PlayerMovement>();
        cameraTransform = Camera.main.transform;
        defaultPosition = cameraTransform.localPosition.z;
    }

    public void HandleAllCameraMovement()
    {
        FollowTarget();
        RotateCamera();
        CameraCollision();
        HandleScoppedFOV();
    }

    private void FollowTarget()
    {
        Vector3 targetPosition = Vector3.SmoothDamp(transform.position, playerTransform.position, ref cameraFollowVelocity, cameraFollowSpeed);
        transform.position = targetPosition;
    }

    private void RotateCamera()
    {
        lookAngle += (inputManager.cameraInputX * cameraLookSpeed);
        pivotAngle -= (inputManager.cameraInputY * cameraPivotSpeed);

        pivotAngle = Mathf.Clamp(pivotAngle, minPivotAngle, maxPivotAngle);

        Quaternion targetRotation = Quaternion.Euler(0, lookAngle, 0);
        transform.rotation = targetRotation;

        Quaternion targetPivotRotation = Quaternion.Euler(pivotAngle, 0, 0);
        cameraPivot.localRotation = targetPivotRotation;

        if(!playerMovement.isMoving && !playerMovement.isSprinting) {
            playerTransform.rotation = Quaternion.Euler(0, lookAngle, 0);
        }

        if(isScoped) {
            cameraLookSpeed = 0.1f;
            cameraPivotSpeed = 0.1f;
            minPivotAngle = -0.5f;
            maxPivotAngle = 6f;
        }
        else {
            cameraLookSpeed = 0.1f;
            cameraPivotSpeed = 0.1f;
            minPivotAngle = -30f;
            maxPivotAngle = 30f;
        }
    }

    private void CameraCollision() {
        float targetPosition = defaultPosition;
        RaycastHit hit;
        Vector3 direction = cameraTransform.position - cameraPivot.position;
        direction.Normalize();

        if(Physics.SphereCast(cameraPivot.transform.position, cameraCollisionRadius, direction, out hit, Mathf.Abs(targetPosition), collisionLayer)) {
            float distance = Vector3.Distance(cameraPivot.position, hit.point);
            targetPosition =- (distance - cameraCollisionOffset);
        }

        if(Mathf.Abs(targetPosition) < minCollisionOffset) {
            targetPosition = targetPosition - minCollisionOffset;
        }

        cameraVectorPosition.z = Mathf.Lerp(cameraTransform.localPosition.z, targetPosition, 0.2f);
        cameraTransform.localPosition = cameraVectorPosition;
    }

    private void HandleScoppedFOV() {
        if(inputManager.scopeInput) {
            camera.fieldOfView = scoppedFOV;
            isScoped = true;
        }
        else {
            camera.fieldOfView = defaultFOV;
            isScoped = false;
        }
    }
}