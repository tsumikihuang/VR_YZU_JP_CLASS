using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class YourCustomFunction : MonoBehaviour
{
    public static YourCustomFunction _instance;
    public YoutubePlayer ytPlayer;

    void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(this);
            name = "最初的遊戲管理物件";
        }
        else if (this != _instance)
        {
            string sceneName = SceneManager.GetActiveScene().name;
            Debug.Log("刪除場景" + sceneName + "的" + name);
            Destroy(gameObject);
        }
    }

    /*void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))        //從頭撥放
        {
            ytPlayer.Play("https://www.youtube.com/watch?v=" + "n3LUSPAN9cI");
        }
        if (Input.GetKeyDown(KeyCode.P))            //暫停
        {
            ytPlayer.Pause();
        }
        if (Input.GetKeyDown(KeyCode.S))            //開始
        {
            ytPlayer.Play();
        }
        if (Input.GetKeyDown(KeyCode.M))            //靜音
        {
            ytPlayer.GetComponent<AudioSource>().mute = true;
        }
        if (Input.GetKeyDown(KeyCode.N))            //取消靜音
        {
            ytPlayer.GetComponent<AudioSource>().mute = false;
        }
    }*/
}
