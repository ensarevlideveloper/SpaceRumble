using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OfflineSceneManager : MonoBehaviour {

    public GameObject Main;
    public GameObject Options;
    public AudioSource themeMusic;
    public AudioSource buttonClickSound;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void StartClient () {
        buttonClickSound.Play();
        Application.LoadLevel("Client");
    }

    public void OpenSettings () {
        buttonClickSound.Play();
        Main.SetActive(false);
        Options.SetActive(true);
    }

    public void CloseSettings () {
        buttonClickSound.Play();
        Options.SetActive(false);
        Main.SetActive(true);
    }

    public void setMusicVolume(float vol) {
        themeMusic.volume = vol;
    }

}
