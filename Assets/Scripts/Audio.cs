using UnityEngine;
using System.Collections;

public class Audio : MonoBehaviour {
	public AudioSource[] music;
	public float musicVolum ;

	// Use this for initialization
	void Start () {
		musicVolum = 0.5f;
		music = this.GetComponents<AudioSource> ();

	}
	
	// Update is called once per frame
	public void playClickEffect(){
		music[1].Play();
	}
	public void playMatchEffect(){
		music[0].Play();
	}
}
