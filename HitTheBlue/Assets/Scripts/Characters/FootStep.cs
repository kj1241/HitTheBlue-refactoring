using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class FootStep : MonoBehaviour
{
    private static object lockObject = new object();
    public AudioSource audioSource;
    public List<AudioClip> footsteps;

    public void Footstep() {
        if (footsteps.Count <= 0) {
            return;
        }
        lock (lockObject)
        {
            audioSource.clip = footsteps[Random.Range(0, footsteps.Count)];
            if (audioSource.clip != null)
                audioSource.Play();
        }
    }

    public void Awake() {
        audioSource = GetComponent<AudioSource>();
    }
}
