using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum AudioTracks {
	HOME_BASE,
	HOME1,
	HOME2,
	HOME3,
	HOME4,
	FOOD_JENGA,
	ALIEN_GO_HOME,
	MUSEUM,
	PLANTS
}

public class SoundMixer : MonoBehaviour
{

	[SerializeField] float fadeSpeed = .1f;
	[SerializeField] AudioClip[] music;

	private int activeTracks;
	private int fadeInTracks;
	private int fadeOutTracks;
	private AudioSource[] tracks;

    // Start is called before the first frame update
    void Start()
    {
        tracks = new AudioSource[music.Length];
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void Fade(AudioSource track, float speed, float start, float end){
    	track.volume = ((end - start) * speed * Time.deltaTime) + track.volume;
    }
}
