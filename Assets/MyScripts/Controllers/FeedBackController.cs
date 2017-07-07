using UnityEngine;
using System.Collections;

public class FeedBackController : MonoBehaviour {
	public Sprite spriteGoodJob, SpriteBadJob;
	public AudioClip GoodJobSound, BadJobSound;
	AudioSource audioSource;
	SpriteRenderer spriteRenderer;
	public float timeDiplaying = 2.5f;
	// Use this for initialization
	void Start () {
		spriteRenderer = transform.GetComponent<SpriteRenderer>();
		audioSource = transform.GetComponent<AudioSource>();
	
	
	}
	


	public void DisplayGoodJob(){
		DisplayFeedBack (spriteGoodJob, GoodJobSound);

	}
	public void DisplayBadJob(){
		DisplayFeedBack (SpriteBadJob, BadJobSound);

	}

	private void DisplayFeedBack(Sprite sprite, AudioClip clip){
		spriteRenderer.enabled = true;
		spriteRenderer.sprite = sprite;
		audioSource.clip = clip;
		audioSource.Play ();
		Invoke("StopDisplay", timeDiplaying);
	}
	private void StopDisplay(){

		spriteRenderer.enabled = false;
	}
}
