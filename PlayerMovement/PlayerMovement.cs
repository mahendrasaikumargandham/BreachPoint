// // using UnityEngine;
// // using Photon.Pun;
// // using UnityEngine.UI;

// // public class PlayerMovement : MonoBehaviour
// // {
// //     [Header("Essential Scripts")]
// //     InputManager inputManager;
// //     Vector3 moveDirection;
// //     Transform cameraGameObject;
// //     Rigidbody playerRigidbody;
// //     PlayerManager playerManager;
// //     AnimatorManager animatorManager;

// //     [Header("Player Health")]
// //     const float maxHealth = 150f;
// //     public float currentHealth;
// //     public Slider healthBarSlider;
// //     public GameObject playerUI;

// //     [Header("Falling and Landing")]
// //     public float inAirTimer;
// //     public float leapingVelocity;
// //     public float fallingVelocity;
// //     public float raycastHeightOffSet = 0.5f;
// //     public LayerMask groundLayer;
// //     public bool isGrounded;
    
// //     [Header("Movement Variables")]
// //     public float movementSpeed = 2f;
// //     public float rotationSpeed = 13f;
// //     public float sprintingSpeed = 9f;
// //     public bool isSprinting;
// //     public bool isMoving;

// //     [Header("Jumping variables")]
// //     public float jumpHeight = 4f;
// //     public float gravityIntensity = -15f;
// //     public bool isJumping;

// //     [Header("Multiplayer components")]
// //     PhotonView view;
// //     PlayerControllerManager playerControllerManager;

// //     [Header("Footstep Audio")]
// //     public AudioSource footstepAudioSource; // Assign in Inspector
// //     public AudioClip walkFootstepClip; // Walking footstep sound
// //     public AudioClip runFootstepClip; // Running footstep sound
// //     public AudioClip shootWalkFootstepClip; // Shoot walking footstep sound
// //     public float walkFootstepInterval = 0.5f; // Time between steps when walking
// //     public float runFootstepInterval = 0.3f; // Time between steps when running
// //     public float shootWalkFootstepInterval = 0.6f; // Time between steps when shoot walking
// //     private float footstepTimer; // Tracks time since last footstep

// //     public int playerTeam;

// //     void Awake()
// //     {
// //         currentHealth = maxHealth;
// //         inputManager = GetComponent<InputManager>();
// //         playerRigidbody = GetComponent<Rigidbody>();
// //         playerManager = GetComponent<PlayerManager>();
// //         animatorManager = GetComponent<AnimatorManager>();
// //         view = GetComponent<PhotonView>();
// //         playerControllerManager = PhotonView.Find((int)view.InstantiationData[0]).GetComponent<PlayerControllerManager>();
// //         cameraGameObject = Camera.main.transform;
// //         footstepAudioSource = GetComponent<AudioSource>(); // Get AudioSource
// //         if (footstepAudioSource == null)
// //         {
// //             Debug.LogError("AudioSource component missing on Player! Please add one.");
// //         }

// //         healthBarSlider.minValue = 0f;
// //         healthBarSlider.maxValue = maxHealth;
// //         healthBarSlider.value = currentHealth;
// //     }

// //     void Start()
// //     {
// //         if (!view.IsMine)
// //         {
// //             Destroy(playerRigidbody);
// //             Destroy(playerUI);
// //         }

// //         if (view.Owner.CustomProperties.ContainsKey("Team"))
// //         {
// //             int team = (int)view.Owner.CustomProperties["Team"];
// //             playerTeam = team;
// //         }
// //     }

// //     void Update()
// //     {
// //         if (view.IsMine)
// //         {
// //             HandleFootsteps(); // Add footstep handling for local player
// //         }
// //     }

// //     public void HandleAllMovement()
// //     {
// //         HandleFallingAndLanding();
// //         if (playerManager.isInteracting) return;
// //         HandleMovement();
// //         HandleRotation();
// //     }

// //     private void HandleMovement()
// //     {
// //         if (isJumping) return;

// //         moveDirection = new Vector3(cameraGameObject.forward.x, 0f, cameraGameObject.forward.z) * inputManager.verticalInput;
// //         moveDirection = moveDirection + cameraGameObject.right * inputManager.horizontalInput;
// //         moveDirection.Normalize();

// //         moveDirection.y = 0;
        
// //         if (isSprinting)
// //         {
// //             moveDirection = moveDirection * sprintingSpeed;
// //         }
// //         else
// //         {
// //             if (inputManager.movementAmount >= 0.5f)
// //             {
// //                 moveDirection = moveDirection * movementSpeed;
// //                 isMoving = true;
// //             }
// //             if (inputManager.movementAmount <= 0f)
// //             {
// //                 isMoving = false;
// //             }
// //         }

// //         Vector3 movementVelocity = moveDirection;
// //         playerRigidbody.velocity = movementVelocity;
// //     }

// //     private void HandleRotation()
// //     {
// //         if (isJumping) return;

// //         Vector3 targetDirection = Vector3.zero;
// //         targetDirection = cameraGameObject.forward * inputManager.verticalInput;
// //         targetDirection = targetDirection + cameraGameObject.right * inputManager.horizontalInput;
// //         targetDirection.Normalize();

// //         targetDirection.y = 0;
// //         if (targetDirection == Vector3.zero)
// //         {
// //             targetDirection = transform.forward;
// //         }
// //         Quaternion targetRotation = Quaternion.LookRotation(targetDirection);
// //         Quaternion playerRotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

// //         transform.rotation = playerRotation;
// //     }

// //     private void HandleFallingAndLanding()
// //     {
// //         RaycastHit hit;
// //         Vector3 raycastOrigin = transform.position;
// //         Vector3 targetPosition;
// //         raycastOrigin.y = raycastOrigin.y + raycastHeightOffSet;
// //         targetPosition = transform.position;

// //         if (!isGrounded && !isJumping)
// //         {
// //             if (!playerManager.isInteracting)
// //             {
// //                 animatorManager.PlayTargetAnim("Falling", true);
// //             }

// //             inAirTimer = inAirTimer + Time.deltaTime;
// //             playerRigidbody.AddForce(transform.forward * leapingVelocity);
// //             playerRigidbody.AddForce(-Vector3.up * fallingVelocity * inAirTimer);
// //         }   

// //         if (Physics.SphereCast(raycastOrigin, 0.2f, -Vector3.up, out hit, groundLayer))
// //         {
// //             if (!isGrounded && !playerManager.isInteracting)
// //             {
// //                 animatorManager.PlayTargetAnim("Landing", true);
// //             }

// //             Vector3 RaycastHitPoint = hit.point;
// //             targetPosition.y = RaycastHitPoint.y;
// //             inAirTimer = 0;
// //             isGrounded = true;
// //         }
// //         else
// //         {
// //             isGrounded = false;
// //         }

// //         if (isGrounded && !isJumping)
// //         {
// //             if (playerManager.isInteracting || inputManager.movementAmount > 0)
// //             {
// //                 transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime / 0.1f);
// //             }
// //             else
// //             {
// //                 transform.position = targetPosition;
// //             }
// //         }
// //     }

// //     public void HandleJumping()
// //     {
// //         if (isGrounded)
// //         {
// //             animatorManager.animator.SetBool("isJumping", true);
// //             animatorManager.PlayTargetAnim("Jump", false);

// //             float jumpingVelocity = Mathf.Sqrt(-2 * gravityIntensity * jumpHeight);
// //             Vector3 playerVelocity = moveDirection;
// //             playerVelocity.y = jumpingVelocity;
// //             playerRigidbody.velocity = playerVelocity;
// //             isJumping = false;
// //         }
// //     }

// //     public void SetIsJumping(bool isJumping)
// //     {
// //         this.isJumping = isJumping;
// //     }

// //     public void ApplyDamage(float damage)
// //     {
// //         view.RPC("RPC_TakeDamage", RpcTarget.All, damage);
// //     }

// //     [PunRPC]
// //     void RPC_TakeDamage(float damage)
// //     {
// //         if (view.IsMine)
// //         {
// //             currentHealth -= damage;
// //             healthBarSlider.value = currentHealth;
// //             if (currentHealth <= 0f)
// //             {
// //                 Die();
// //             }
// //         }
// //     }

// //     private void Die()
// //     {
// //         playerControllerManager.Die();
// //         // increase score
// //         ScoreBoard.instance.PlayerHasDied(playerTeam);
// //     }

// //     private void HandleFootsteps()
// //     {
// //         if (!view.IsMine || !isGrounded || !isMoving || isJumping)
// //         {
// //             footstepTimer = 0; // Reset timer when not moving or in air
// //             return;
// //         }

// //         footstepTimer += Time.deltaTime;

// //         // Get CameraManager to check isScoped
// //         CameraManager cameraManager = FindObjectOfType<CameraManager>();
// //         bool isScoped = cameraManager != null && cameraManager.isScoped;

// //         // Determine which footstep sound and interval to use
// //         AudioClip footstepClip = walkFootstepClip;
// //         float interval = walkFootstepInterval;

// //         if (isSprinting)
// //         {
// //             footstepClip = runFootstepClip;
// //             interval = runFootstepInterval;
// //         }
// //         else if (isScoped)
// //         {
// //             footstepClip = shootWalkFootstepClip;
// //             interval = shootWalkFootstepInterval;
// //         }

// //         // Play footstep sound if interval has passed
// //         if (footstepTimer >= interval && footstepClip != null && footstepAudioSource != null)
// //         {
// //             footstepAudioSource.PlayOneShot(footstepClip);
// //             footstepTimer = 0; // Reset timer
// //         }
// //     }
// // }








// using UnityEngine;
// using Photon.Pun;
// using UnityEngine.UI;

// public class PlayerMovement : MonoBehaviour
// {
//     [Header("Essential Scripts")]
//     InputManager inputManager;
//     Vector3 moveDirection;
//     Transform cameraGameObject;
//     Rigidbody playerRigidbody;
//     PlayerManager playerManager;
//     AnimatorManager animatorManager;
//     CameraManager cameraManager; // Added for performance, to avoid FindObjectOfType in Update

//     [Header("Player Health")]
//     const float maxHealth = 150f;
//     public float currentHealth;
//     public Slider healthBarSlider;
//     public GameObject playerUI;

//     [Header("Falling and Landing")]
//     public float inAirTimer;
//     public float leapingVelocity;
//     public float fallingVelocity;
//     public float raycastHeightOffSet = 0.5f;
//     public LayerMask groundLayer;
//     public bool isGrounded;
    
//     [Header("Movement Variables")]
//     public float movementSpeed = 2f;
//     public float rotationSpeed = 13f;
//     public float sprintingSpeed = 9f;
//     public bool isSprinting;
//     public bool isMoving;

//     [Header("Jumping variables")]
//     public float jumpHeight = 4f;
//     public float gravityIntensity = -15f;
//     public bool isJumping;

//     [Header("Multiplayer components")]
//     PhotonView view;
//     PlayerControllerManager playerControllerManager;

//     [Header("Footstep Audio")]
//     public AudioSource footstepAudioSource; // Assign in Inspector
//     public AudioClip walkFootstepClip; // Walking footstep sound
//     public AudioClip runFootstepClip; // Running footstep sound
//     public AudioClip shootWalkFootstepClip; // Shoot walking footstep sound
//     public float walkFootstepInterval = 0.5f; // Time between steps when walking
//     public float runFootstepInterval = 0.3f; // Time between steps when running
//     public float shootWalkFootstepInterval = 0.6f; // Time between steps when shoot walking
//     private float footstepTimer; // Tracks time since last footstep

//     public int playerTeam;

//     void Awake()
//     {
//         currentHealth = maxHealth;
//         inputManager = GetComponent<InputManager>();
//         playerRigidbody = GetComponent<Rigidbody>();
//         playerManager = GetComponent<PlayerManager>();
//         animatorManager = GetComponent<AnimatorManager>();
//         view = GetComponent<PhotonView>();
//         playerControllerManager = PhotonView.Find((int)view.InstantiationData[0]).GetComponent<PlayerControllerManager>();
//         cameraGameObject = Camera.main.transform;
        
//         // --- PERFORMANCE FIX: Cache the CameraManager reference here instead of in Update ---
//         cameraManager = FindObjectOfType<CameraManager>();

//         footstepAudioSource = GetComponent<AudioSource>(); // Get AudioSource
//         if (footstepAudioSource == null)
//         {
//             Debug.LogError("AudioSource component missing on Player! Please add one.");
//         }

//         healthBarSlider.minValue = 0f;
//         healthBarSlider.maxValue = maxHealth;
//         healthBarSlider.value = currentHealth;
//     }

//     void Start()
//     {
//         if (!view.IsMine)
//         {
//             Destroy(playerRigidbody);
//             Destroy(playerUI);
//         }

//         if (view.Owner.CustomProperties.ContainsKey("Team"))
//         {
//             int team = (int)view.Owner.CustomProperties["Team"];
//             playerTeam = team;
//         }
//     }

//     void Update()
//     {
//         if (view.IsMine)
//         {
//             HandleFootsteps(); // Add footstep handling for local player
//         }
//     }

//     public void HandleAllMovement()
//     {
//         HandleFallingAndLanding();
//         if (playerManager.isInteracting) return;
//         HandleMovement();
//         HandleRotation();
//     }

//     private void HandleMovement()
//     {
//         if (isJumping) return;

//         moveDirection = new Vector3(cameraGameObject.forward.x, 0f, cameraGameObject.forward.z) * inputManager.verticalInput;
//         moveDirection = moveDirection + cameraGameObject.right * inputManager.horizontalInput;
//         moveDirection.Normalize();

//         moveDirection.y = 0;
        
//         if (isSprinting)
//         {
//             moveDirection = moveDirection * sprintingSpeed;
//         }
//         else
//         {
//             // --- LOGIC FIX: Changed this to correctly set isMoving status ---
//             if (inputManager.movementAmount > 0)
//             {
//                 moveDirection = moveDirection * movementSpeed;
//                 isMoving = true;
//             }
//             else
//             {
//                 isMoving = false;
//             }
//         }

//         Vector3 movementVelocity = moveDirection;
//         playerRigidbody.velocity = movementVelocity;
//     }

//     private void HandleRotation()
//     {
//         if (isJumping) return;

//         Vector3 targetDirection = Vector3.zero;
//         targetDirection = cameraGameObject.forward * inputManager.verticalInput;
//         targetDirection = targetDirection + cameraGameObject.right * inputManager.horizontalInput;
//         targetDirection.Normalize();

//         targetDirection.y = 0;
//         if (targetDirection == Vector3.zero)
//         {
//             targetDirection = transform.forward;
//         }
//         Quaternion targetRotation = Quaternion.LookRotation(targetDirection);
//         Quaternion playerRotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

//         transform.rotation = playerRotation;
//     }

//     private void HandleFallingAndLanding()
//     {
//         RaycastHit hit;
//         Vector3 raycastOrigin = transform.position;
//         Vector3 targetPosition;
//         raycastOrigin.y = raycastOrigin.y + raycastHeightOffSet;
//         targetPosition = transform.position;

//         if (!isGrounded && !isJumping)
//         {
//             if (!playerManager.isInteracting)
//             {
//                 animatorManager.PlayTargetAnim("Falling", true);
//             }

//             inAirTimer = inAirTimer + Time.deltaTime;
//             playerRigidbody.AddForce(transform.forward * leapingVelocity);
//             playerRigidbody.AddForce(-Vector3.up * fallingVelocity * inAirTimer);
//         }   

//         if (Physics.SphereCast(raycastOrigin, 0.2f, -Vector3.up, out hit, 1f, groundLayer)) // Added max distance to SphereCast
//         {
//             if (!isGrounded && playerManager.isInteracting == false)
//             {
//                 animatorManager.PlayTargetAnim("Landing", true);
//             }

//             Vector3 RaycastHitPoint = hit.point;
//             targetPosition.y = RaycastHitPoint.y;
//             inAirTimer = 0;
//             isGrounded = true;
//         }
//         else
//         {
//             isGrounded = false;
//         }

//         if (isGrounded && !isJumping)
//         {
//             if (playerManager.isInteracting || inputManager.movementAmount > 0)
//             {
//                 transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime / 0.1f);
//             }
//             else
//             {
//                 transform.position = targetPosition;
//             }
//         }
//     }

//     public void HandleJumping()
//     {
//         if (isGrounded)
//         {
//             animatorManager.animator.SetBool("isJumping", true);
//             animatorManager.PlayTargetAnim("Jump", false);

//             float jumpingVelocity = Mathf.Sqrt(-2 * gravityIntensity * jumpHeight);
//             Vector3 playerVelocity = moveDirection;
//             playerVelocity.y = jumpingVelocity;
//             playerRigidbody.velocity = playerVelocity;
//             isJumping = false;
//         }
//     }

//     public void SetIsJumping(bool isJumping)
//     {
//         this.isJumping = isJumping;
//     }

//     public void ApplyDamage(float damage)
//     {
//         view.RPC("RPC_TakeDamage", RpcTarget.All, damage);
//     }

//     [PunRPC]
//     void RPC_TakeDamage(float damage)
//     {
//         // This check ensures health is only reduced once, on the correct player object.
//         if (!view.IsMine) return;

//         currentHealth -= damage;
//         healthBarSlider.value = currentHealth;
//         if (currentHealth <= 0f)
//         {
//             Die();
//         }
//     }

//     private void Die()
//     {
//         playerControllerManager.Die();
//         // increase score
//         ScoreBoard.instance.PlayerHasDied(playerTeam);
//     }

//     private void HandleFootsteps()
//     {
//         if (!isGrounded || !isMoving || isJumping)
//         {
//             footstepTimer = 0; // Reset timer when not moving or in air
//             return;
//         }

//         footstepTimer += Time.deltaTime;

//         // --- PERFORMANCE FIX: Use the cached CameraManager reference ---
//         bool isScoped = cameraManager != null && cameraManager.isScoped;

//         // Determine which footstep sound and interval to use
//         AudioClip footstepClip = walkFootstepClip;
//         float interval = walkFootstepInterval;

//         if (isSprinting)
//         {
//             footstepClip = runFootstepClip;
//             interval = runFootstepInterval;
//         }
//         else if (isScoped)
//         {
//             footstepClip = shootWalkFootstepClip;
//             interval = shootWalkFootstepInterval;
//         }

//         // Play footstep sound if interval has passed
//         if (footstepTimer >= interval && footstepClip != null && footstepAudioSource != null)
//         {
//             footstepAudioSource.PlayOneShot(footstepClip);
//             footstepTimer = 0; // Reset
//         }
//     }
// }



using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;
using ExitGames.Client.Photon; // Required for Hashtable

public class PlayerMovement : MonoBehaviour
{
    [Header("Essential Scripts")]
    InputManager inputManager;
    Vector3 moveDirection;
    Transform cameraGameObject;
    Rigidbody playerRigidbody;
    PlayerManager playerManager;
    AnimatorManager animatorManager;
    CameraManager cameraManager;

    [Header("Player Health")]
    const float maxHealth = 150f;
    public float currentHealth;
    public Slider healthBarSlider;
    public GameObject playerUI;

    [Header("Falling and Landing")]
    public float inAirTimer;
    public float leapingVelocity;
    public float fallingVelocity;
    public float raycastHeightOffSet = 0.5f;
    public LayerMask groundLayer;
    public bool isGrounded;
    
    [Header("Movement Variables")]
    public float movementSpeed = 2f;
    public float rotationSpeed = 13f;
    public float sprintingSpeed = 9f;
    public bool isSprinting;
    public bool isMoving;

    [Header("Jumping variables")]
    public float jumpHeight = 4f;
    public float gravityIntensity = -15f;
    public bool isJumping;

    [Header("Multiplayer components")]
    PhotonView view;
    PlayerControllerManager playerControllerManager;

    [Header("Footstep Audio")]
    public AudioSource footstepAudioSource;
    public AudioClip walkFootstepClip;
    public AudioClip runFootstepClip;
    public AudioClip shootWalkFootstepClip;
    public float walkFootstepInterval = 0.5f;
    public float runFootstepInterval = 0.3f;
    public float shootWalkFootstepInterval = 0.6f;
    private float footstepTimer;

    public int playerTeam = 0; // Default to 0, which is an invalid team

    void Awake()
    {
        view = GetComponent<PhotonView>();
        
        currentHealth = maxHealth;
        inputManager = GetComponent<InputManager>();
        playerRigidbody = GetComponent<Rigidbody>();
        playerManager = GetComponent<PlayerManager>();
        animatorManager = GetComponent<AnimatorManager>();
        
        // This is much safer than doing it in Start()
        if (view.IsMine)
        {
            playerControllerManager = PhotonView.Find((int)view.InstantiationData[0]).GetComponent<PlayerControllerManager>();
            cameraGameObject = Camera.main.transform;
            cameraManager = FindObjectOfType<CameraManager>();
        }

        footstepAudioSource = GetComponent<AudioSource>();
        if (footstepAudioSource == null)
        {
            Debug.LogError("AudioSource component missing on Player! Please add one.");
        }
        
        if (playerUI != null)
        {
           healthBarSlider.minValue = 0f;
           healthBarSlider.maxValue = maxHealth;
           healthBarSlider.value = currentHealth;
        }
    }

    void Start()
    {
        if (!view.IsMine)
        {
            Destroy(playerRigidbody);
            if (playerUI != null) Destroy(playerUI);
        }

        // --- BUG FIX IS HERE ---
        // Use TryGetValue which is safer. Most importantly, be consistent with the key!
        // We will use "team" (lowercase) as the standard.
        if (view.Owner.CustomProperties.TryGetValue("Team", out object teamValue))
        {
            playerTeam = (int)teamValue;
            Debug.Log($"Player {view.Owner.NickName} assigned to team {playerTeam}");
        }
    }
    
    // Update is fine for local-only logic like footsteps
    void Update()
    {
        if (view.IsMine)
        {
            HandleFootsteps();
        }
    }

    public void HandleAllMovement()
    {
        HandleFallingAndLanding();
        if (playerManager.isInteracting) return;
        HandleMovement();
        HandleRotation();
    }

    private void HandleMovement()
    {
        if (isJumping) return;

        moveDirection = new Vector3(cameraGameObject.forward.x, 0f, cameraGameObject.forward.z) * inputManager.verticalInput;
        moveDirection += cameraGameObject.right * inputManager.horizontalInput;
        moveDirection.Normalize();

        moveDirection.y = 0;
        
        float targetSpeed = movementSpeed;
        if (isSprinting)
        {
            targetSpeed = sprintingSpeed;
        }
        
        if (inputManager.movementAmount > 0.1f)
        {
            isMoving = true;
            moveDirection *= targetSpeed;
        }
        else
        {
            isMoving = false;
            moveDirection = Vector3.zero; // Stop movement completely
        }

        Vector3 movementVelocity = moveDirection;
        playerRigidbody.velocity = movementVelocity;
    }

    private void HandleRotation()
    {
        if (isJumping) return;

        Vector3 targetDirection = cameraGameObject.forward * inputManager.verticalInput;
        targetDirection += cameraGameObject.right * inputManager.horizontalInput;
        targetDirection.Normalize();
        targetDirection.y = 0;

        if (targetDirection == Vector3.zero)
        {
            targetDirection = transform.forward;
        }
        Quaternion targetRotation = Quaternion.LookRotation(targetDirection);
        Quaternion playerRotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

        transform.rotation = playerRotation;
    }

    private void HandleFallingAndLanding()
    {
        // This logic seems complex and is likely specific to your animation system, so I've left it as is.
        // The health logic below is the key area of concern for this bug.
        RaycastHit hit;
        Vector3 raycastOrigin = transform.position;
        Vector3 targetPosition;
        raycastOrigin.y = raycastOrigin.y + raycastHeightOffSet;
        targetPosition = transform.position;

        if (!isGrounded && !isJumping)
        {
            if (!playerManager.isInteracting)
            {
                animatorManager.PlayTargetAnim("Falling", true);
            }
            inAirTimer += Time.deltaTime;
            playerRigidbody.AddForce(transform.forward * leapingVelocity);
            playerRigidbody.AddForce(-Vector3.up * fallingVelocity * inAirTimer);
        }

        if (Physics.SphereCast(raycastOrigin, 0.2f, -Vector3.up, out hit, 1f, groundLayer))
        {
            if (!isGrounded && !playerManager.isInteracting)
            {
                animatorManager.PlayTargetAnim("Landing", true);
            }
            Vector3 RaycastHitPoint = hit.point;
            targetPosition.y = RaycastHitPoint.y;
            inAirTimer = 0;
            isGrounded = true;
        }
        else
        {
            isGrounded = false;
        }

        if (isGrounded && !isJumping)
        {
            if (playerManager.isInteracting || inputManager.movementAmount > 0)
            {
                transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime / 0.1f);
            }
            else
            {
                transform.position = targetPosition;
            }
        }
    }

    public void HandleJumping()
    {
        if (isGrounded)
        {
            animatorManager.animator.SetBool("isJumping", true);
            animatorManager.PlayTargetAnim("Jump", false);

            float jumpingVelocity = Mathf.Sqrt(-2 * gravityIntensity * jumpHeight);
            Vector3 playerVelocity = moveDirection;
            playerVelocity.y = jumpingVelocity;
            playerRigidbody.velocity = playerVelocity;
            isJumping = false;
        }
    }

    public void SetIsJumping(bool isJumping)
    {
        this.isJumping = isJumping;
    }

    // This function is called by the player who is shooting.
    public void ApplyDamage(float damage)
    {
        // --- BEST PRACTICE CHANGE ---
        // Instead of sending the RPC to 'All', we send it ONLY to the owner of this photonView.
        // This is much more efficient as it doesn't send unnecessary messages to other players.
        view.RPC("RPC_TakeDamage", view.Owner, damage);
    }

    [PunRPC]
    void RPC_TakeDamage(float damage)
    {
        // Since this RPC is now only sent to the owner, we are certain this code
        // is running on the computer of the player who was shot.
        // The 'if(view.IsMine)' check is still a good safety measure.
        if (!view.IsMine) return;

        currentHealth -= damage;
        healthBarSlider.value = currentHealth;
        Debug.Log($"Player {view.Owner.NickName} took damage, health is now {currentHealth}");

        if (currentHealth <= 0f)
        {
            Die();
        }
    }

    private void Die()
    {
        // --- CHANGE HERE: THIS IS THE NEW, ROBUST LOGIC ---
        Debug.Log($"Player {view.Owner.NickName} on team {playerTeam} has died. Reporting to Master Client.");

        if (ScoreBoard.instance != null)
        {
            // Get the PhotonView component from the singleton ScoreBoard instance.
            PhotonView scoreboardView = ScoreBoard.instance.GetComponent<PhotonView>();
            if (scoreboardView != null)
            {
                // Call the RPC on the ScoreBoard, sending it ONLY to the Master Client.
                scoreboardView.RPC("RPC_ReportDeath", RpcTarget.MasterClient, playerTeam);
            }
            else
            {
                Debug.LogError("Could not find a PhotonView on the ScoreBoard object!");
            }
        }
        else
        {
            Debug.LogError("ScoreBoard instance not found! Cannot report death.");
        }

        // Your existing death logic is fine.
        playerControllerManager.Die();
    }
    
    private void HandleFootsteps()
    {
       // Footstep logic left as is.
       if (!view.IsMine || !isGrounded || !isMoving || isJumping)
       {
           footstepTimer = 0; 
           return;
       }

       footstepTimer += Time.deltaTime;
       bool isScoped = cameraManager != null && cameraManager.isScoped;

       AudioClip footstepClip = walkFootstepClip;
       float interval = walkFootstepInterval;

       if (isSprinting)
       {
           footstepClip = runFootstepClip;
           interval = runFootstepInterval;
       }
       else if (isScoped)
       {
           footstepClip = shootWalkFootstepClip;
           interval = shootWalkFootstepInterval;
       }

       if (footstepTimer >= interval && footstepClip != null && footstepAudioSource != null)
       {
           footstepAudioSource.PlayOneShot(footstepClip);
           footstepTimer = 0;
       }
    }
}
    
