using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SpaceRumble.Networking;

public class ClientSceneManager : MonoBehaviour {

    public AudioSource Music;
    public AudioSource sfx;
    public AudioSource buttonClickSound;

    public GameObject Options;
    public GameObject respawn;

    public Text respawnPlaceText;

    private NetworkIdentity respawnableNetworkIdentity;
    private bool respawnable = false;


	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void OpenSettings()
    {
        buttonClickSound.Play();
        Options.SetActive(true);
    }

    public void CloseSettings()
    {
        buttonClickSound.Play();
        Options.SetActive(false);
    }

    public void setMusicVolume(float vol)
    {
        Music.volume = vol;
    }

    public void setSFXVolume(float vol)
    {
        sfx.volume = vol;
    }

    public void Respawn() {
        buttonClickSound.Play();
        if (respawnableNetworkIdentity != null && respawnable == true) {
            respawnableNetworkIdentity.GetSocket().Emit("respawnPlayer");
            respawnable = false;
        }
        respawn.SetActive(false);
    }

    public void OpenRespawn () {
        respawn.SetActive(true);
    }

    public void GotoMainMenu () {
        respawnableNetworkIdentity.GetSocket().Emit("leaveGame");
        Application.LoadLevel("Offline");
    }

    public void setRespawnPlaceText (int place) {
        respawnPlaceText.text = "your place: " + place;
    }

    public void setRespawnableNetworkIdentity(NetworkIdentity ni) {

        this.respawnableNetworkIdentity = ni;
        this.respawnable = true;

    }




}
