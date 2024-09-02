using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BgmFight1 : MonoBehaviour
{
    public AudioClip bgm;
    private AudioSource player;
    // Start is called before the first frame update
    void Start()
    {
        player = GetComponent<AudioSource>();
        player.clip = bgm;
        player.loop = true;
        player.volume = 1.0f;
        player.Play();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.M))
        {
            if (player.isPlaying)
            {
                player.Pause();
            }
            else
            {
                player.UnPause();
            }
        }
    }
}

