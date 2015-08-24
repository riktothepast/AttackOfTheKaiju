using UnityEngine;
using System.Collections;

public class SoundLoader : MonoBehaviour {
    public AudioClip sound;

	public void PlaySound () {
        AudioManager.instance.PlaySound(sound);
	}

}