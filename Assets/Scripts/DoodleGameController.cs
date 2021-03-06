﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DoodleGameController : MonoBehaviour {
	public GameObject player;//the doodle jumper
	public Camera maincam;//main camera
	public GameObject imagetarget;//image target

	public GameObject textreference;//the display to follow this

	public GameObject platform;//platform to duplicate
	public int platformSpawnCount = 10;
	public float distanceBetweenPlatforms = 2.0f;

	public GameObject chaser;//thing to chase

	private List<GameObject> platforms = new List<GameObject>();
	private List<GameObject> texts = new List<GameObject>();


	// Use this for initialization
	void Start () {
		spawnPlatforms ();
	}
	
	// Update is called once per frame
	void Update () {
		moveCamera ();

		Vector3 playerpos = player.transform.position;
		Vector3 campos = maincam.transform.position;
		//dead player
		if (playerpos.y < campos.y - 13) {
			reset ();
		}

		//build more platforms
		if (platforms.Count > 1) {
				if (player.transform.position.y > platforms [platforms.Count - 1].transform.position.y - 10.0f) {
						despawnPlatforms ();
						spawnPlatforms ();
				}
		}
		//chaserManager ();

	}

	void textManager(){
	}

	void createNewStory(){
		Vector3 campos = maincam.transform.position;
		float distanceBetweenText = 18.0f;
		for (int i=0; i<8; i++) {
			Vector3 newpos = new Vector3(0, campos.y + distanceBetweenText*i + 15.0f, 02);
			GameObject t = (GameObject)Instantiate(textreference, newpos, Quaternion.identity);
			t.GetComponent<InfoOverlay>().textNum = i;
		}

	}


	public void trackingFound(){
		Chaser c = chaser.GetComponent<Chaser> ();

		c.gameObject.SetActive (true);
		
		if(c.isChasing == false){
			createNewStory();
			c.respawn();
		}
		c.isChasing = true;
	}

	public void trackingLost(){
		Chaser c = chaser.GetComponent<Chaser> ();
		c.isChasing = false;
	}



	void reset(){
		maincam.transform.position = new Vector3 (0, 0, -15);

		removeAllPlatforms ();
		spawnPlatforms ();

		player.GetComponent<DoodleJumper> ().reset();
		chaser.GetComponent<Chaser> ().reset ();
	}

	void moveCamera(){//handle the camera
		Vector3 playerPos = player.transform.position;
		Vector3 pos = maincam.transform.position;
		float cameraOffset = 3.0f;//how far higher the camera should be relative to the player
		if (playerPos.y+cameraOffset > pos.y) {
			maincam.transform.position = new Vector3 (pos.x, player.transform.position.y+cameraOffset, pos.z);
		}
		imagetarget.transform.position = maincam.transform.position;
	}

	void removeAllPlatforms(){


		foreach(GameObject p in platforms){//destroy all platforms
			Destroy (p);
		}
		platforms = new List<GameObject>();
	}

	void despawnPlatforms(){//remove platforms behind player
		for (int i = platforms.Count-1; i>0; i--) {
			if(platforms[i].transform.position.y < player.transform.position.y - 8.0f){
				Destroy (platforms[i]);
				platforms.RemoveAt (i);
			}

		}
	}

	void spawnPlatforms(){//create new platforms
		Vector3 spawnPos = maincam.transform.position;//default spawn position
		if(platforms.Count > 1){//if we're above
			spawnPos = new Vector3(maincam.transform.position.x, platforms[platforms.Count-1].transform.position.y, 0);
		}

		for (int i = 0; i<platformSpawnCount; i++) {
			float xoffset = Random.Range(-5, 5);//randomize position of platform
			float yoffset = Random.Range(-.5f, .5f);//offset y a bit
			float newWidth = Random.Range(2, 4); //randomize size
			Vector3 position = new Vector3(spawnPos.x + xoffset, spawnPos.y+yoffset+distanceBetweenPlatforms*i,0);
			GameObject newPlatform = (GameObject)Instantiate(platform, position, Quaternion.identity);
			newPlatform.transform.localScale = new Vector3(newWidth, .4f, 1);
			platforms.Add (newPlatform);
		}
	}
}
