using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerFootPrintSounds : MonoBehaviour
{
    private AudioSource audioSource;

    [Header("Footstep source")]
    [SerializeField] private AudioClip[] footStepSounds;
    

    // void Awake() {
    //     audioSource = GetComponent<AudioSource>();
    // }

    // private AudioClip GetRandomFootStep() {
    //     return footStepSounds[UnityEngine.Random.Range(0, footStepSounds.Length)];
    // }

    // private void Step() {
    //     AudioClip clip = GetRandomFootStep();
    //     audioSource.PlayOneShot(clip);
    // }
}
