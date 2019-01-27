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

	private int fadeInTracks;
	private int fadeOutTracks;
	private AudioSource[] tracks;

    // Start is called before the first frame update
    void Start()
    {
        tracks = new AudioSource[music.Length];
        for(int i = 0; i < music.Length; ++i){
        	tracks[i] = gameObject.AddComponent<AudioSource>();
        	tracks[i].clip = music[i];
        	tracks[i].Play();
        	tracks[i].loop = true;
        	tracks[i].volume = 0;
        }

        MakeFadeIn(AudioTracks.HOME_BASE);
    }

    // Update is called once per frame
    void Update()
    {
        for(int i = 0; i < tracks.Length; ++i){
        	int flag = 1 << i;
        	bool fadeIn = (flag & fadeInTracks) != 0;
        	bool fadeOut = (flag & fadeOutTracks) != 0;
        	Debug.Assert(!(fadeIn && fadeOut));

        	if(fadeIn){
        		Fade(tracks[i], fadeSpeed, 0, 1);
        		if(tracks[i].volume == 1){
        			fadeInTracks = fadeInTracks & (~flag);
        		}
        	}
        	if(fadeOut){
        		Fade(tracks[i], fadeSpeed, 1, 0);
        		if(tracks[i].volume == 0){
        			fadeOutTracks = fadeOutTracks & (~flag);
        		}
        	}
        }
    }

    void Fade(AudioSource track, float speed, float start, float end){
    	track.volume = ((end - start) * speed * Time.deltaTime) + track.volume;
    }

    public void MakeFadeIn(AudioTracks track){
    	int flag = 1 << (int)track;
    	fadeInTracks |= flag;
    	fadeOutTracks &= (~flag);
    }

    public void MakeFadeOut(AudioTracks track){
    	int flag = 1 << (int)track;
    	fadeOutTracks |= flag;
    	fadeInTracks &= (~flag);
    }

    public void MakeFadeIn(int track){
    	int flag = 1 << (int)track;
    	fadeInTracks |= flag;
    	fadeOutTracks &= (~flag);
    }
    public void MakeFadeOut(int track){
    	int flag = 1 << (int)track;
    	fadeOutTracks |= flag;
    	fadeInTracks &= (~flag);
    }
}
