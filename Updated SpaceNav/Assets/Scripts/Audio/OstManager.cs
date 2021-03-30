using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// For choosing an ambient OST.
public class OstManager : MonoBehaviour
{
    // All Audioclips.
    public AudioClip firstOST;
    public AudioClip secondOST;
    public AudioClip thirdOST;

    // Audioclip list.
    private List<AudioClip> audioClips;

    // Audio Player
    private AudioSource audioSource;

    // Initialize.
    private void Awake() 
    {
        audioClips = new List<AudioClip>();
        audioClips.Add(firstOST);
        audioClips.Add(secondOST);
        audioClips.Add(thirdOST);
        audioSource = GetComponent<AudioSource>();
        audioSource.loop = true;
        audioSource.clip = null;
    }

    // On first frame.
    private void Start()
    {
        // Generate random int and select audio clip at that index.
        int songIndex = (int)Random.Range(0,3);
        print(songIndex);
        audioSource.clip = audioClips[songIndex];
        audioSource.PlayDelayed(1f);
    }
}
