using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TriggerBattle : MonoBehaviour
{
    public string battleSceneName;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            PlayerPrefs.SetFloat("ReturnX", collision.transform.position.x);
            PlayerPrefs.SetFloat("ReturnY", collision.transform.position.y);
            SceneManager.LoadScene(battleSceneName);
        }
    }
}
