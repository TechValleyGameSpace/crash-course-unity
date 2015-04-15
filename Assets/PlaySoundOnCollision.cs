using UnityEngine;
public class PlaySoundOnCollision : MonoBehaviour {
	AudioSource audio;
	void Start () {
		audio = GetComponent<AudioSource>();
	}
	void OnCollisionEnter(Collision info) {
		audio.Stop();
		audio.Play();
	}
}
