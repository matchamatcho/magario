using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class clear : MonoBehaviour
{
    private AudioSource eataudio;
    // Start is called before the first frame update
    public AudioClip clearmusic;

    void Start()
    {
        eataudio = GetComponent<AudioSource>();
        if(clearmusic!=null){
            eataudio.PlayOneShot(clearmusic);
        }
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.R)){
            SceneManager.LoadScene("stage");
        }
        
    }
}
