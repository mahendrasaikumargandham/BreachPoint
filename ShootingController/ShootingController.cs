using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class ShootingController : MonoBehaviour
{
    Animator animator;
    InputManager inputManager;
    PlayerMovement playerMovement;

    [Header("Shooting Variables")]
    public Transform firepoint;
    public float fireRate = 0.1f;
    public float fireRange = 100f;
    public float fireDamage = 15f;
    private float nextFireTime = 0f;

    [Header("Shooting Flags")]
    public bool isShooting;
    public bool isWalking;
    public bool isShootingInput;

    [Header("Reloading Variables")]
    public int maxAmmo = 30;
    private int currentAmmo;
    public float reloadTime = 1.5f;
    public bool isReloading = false;

    [Header("Sound effects")]
    public AudioSource soundAudioSource;
    public AudioClip shootingSound;
    public AudioClip reloadingSound;

    [Header("Particle Effects")]
    public ParticleSystem muzzleFlash;
    public ParticleSystem bloodEffect;


    PhotonView view;

    public int playerTeam;
    
    void Start() {
        animator = GetComponent<Animator>();
        inputManager = GetComponent<InputManager>();
        playerMovement = GetComponent<PlayerMovement>();
        view = GetComponent<PhotonView>();
        currentAmmo = maxAmmo;

        if(view.Owner.CustomProperties.ContainsKey("Team")) {
            int team = (int)view.Owner.CustomProperties["Team"];
            playerTeam = team;
        }
    }

    void Update() {
        if(!view.IsMine) return;

        if(isReloading || playerMovement.isSprinting) {
            animator.SetBool("Shoot", false);
            animator.SetBool("ShootingMovement", false);
            animator.SetBool("ShootWalk", false);
            return;
        }

        isWalking = playerMovement.isMoving;
        isShootingInput = inputManager.fireInput;

        if(isShootingInput) {
            if(isWalking) {
                 if(Time.time >= nextFireTime) {
                    nextFireTime = Time.time + 1f / fireRate;
                    Shoot();
                 }
                animator.SetBool("ShootWalk", true);
                animator.SetBool("Shoot", false);
                animator.SetBool("ShootingMovement", true);
                isShooting = true;
            } else { 
                 if(Time.time >= nextFireTime) {
                    nextFireTime = Time.time + 1f / fireRate;
                    Shoot();
                }
                animator.SetBool("Shoot", true);
                animator.SetBool("ShootingMovement", false);
                animator.SetBool("ShootWalk", false);
                isShooting = true;
            }
        } else {
            animator.SetBool("Shoot", false);
            animator.SetBool("ShootingMovement", false);
            animator.SetBool("ShootWalk", false);
            isShooting = false;
        }

        if(inputManager.reloadInput && currentAmmo < maxAmmo) {
            StartReload();
        }
    }

    private void Shoot() {
        if(currentAmmo > 0) {
            RaycastHit hit;
            if(Physics.Raycast(firepoint.position, firepoint.forward, out hit, fireRange)) {
                Debug.Log("Shooting ----" +hit.transform.name);
                Vector3 hitPoint = hit.point;
                Vector3 hitNormal = hit.normal;

                PlayerMovement playerMovementDamage = hit.collider.GetComponent<PlayerMovement>();

                if(playerMovementDamage != null && playerMovementDamage.playerTeam != playerTeam) {
                    // apply damage to player
                    view.RPC("RPC_Shoot", RpcTarget.All, hitPoint, hitNormal);
                    playerMovementDamage.ApplyDamage(fireDamage);
                }
            }
            muzzleFlash.Play();
            soundAudioSource.PlayOneShot(shootingSound);
            currentAmmo--;
        }
        else {
            StartReload();
        }
    }

    [PunRPC]
    void RPC_Shoot(Vector3 hitPoint, Vector3 hitNormal) {
        ParticleSystem blood = Instantiate(bloodEffect, hitPoint, Quaternion.LookRotation(hitNormal));
        Destroy(blood.gameObject, blood.main.duration);
    }

    private void StartReload() {
        if(!isReloading && currentAmmo < maxAmmo) {
            soundAudioSource.PlayOneShot(reloadingSound);
            isShooting = false;
            animator.SetBool("Shoot", false);
            animator.SetBool("ShootingMovement", false);
            animator.SetBool("ShootWalk", false);

            if(isShootingInput) {
                animator.SetTrigger("Reload");
            }
            isReloading = true;
            Invoke("FinishReloading", reloadTime);
        }
    }

    private void FinishReloading() {
        currentAmmo = maxAmmo;
        isReloading = false;
    }
}