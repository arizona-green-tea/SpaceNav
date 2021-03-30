using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
// Main Menu Controller.
public class MainMenu : MonoBehaviour
{
    // Audio Player.
    private AudioSource audioSource;

    // Loading Screen
    public GameObject loadingScreen;
    public Slider slider;
    public TextMeshProUGUI progressText;
    public AudioClip click;

    // On initialization.
    private void Awake() 
    {
        if(GetComponent<AudioSource>() != null)
        {
            audioSource = GetComponent<AudioSource>();
            audioSource.loop = true;
        }
    }

    // On first frame.
    private void Start() 
    {
        // Play the clip after 3 seconds if the player exists.
        if(GetComponent<AudioSource>() != null)
        {
            audioSource.PlayDelayed(3f);
        }
    }

    // Start the game by generating the next scene.
    public void PlayGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        audioSource.PlayOneShot(click);
    }

    // Close the game.
    public void ExitGame()
    {
        print("exit");
        Application.Quit();
    }

    public void LoadLevel(int index)
    {
       StartCoroutine(LoadAsynchronously(index));
    }

    // Load scene asynchronously and store status of load, execute loop until process is complete.
    IEnumerator LoadAsynchronously(int index)
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(index);

        loadingScreen.SetActive(true);
        while(!operation.isDone)
        {
            float progress = Mathf.Clamp01(operation.progress / 0.9f);
            slider.value = progress;
            progressText.text = "Progress: " + progress * 100f + "%";
            yield return null;
        }
    }
}