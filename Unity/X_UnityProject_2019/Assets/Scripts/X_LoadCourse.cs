using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LoadCourse : MonoBehaviour {
    //public Text QuesText;
    public Button home_btn,next_btn;
    Button btn_n;

    // Use this for initialization
    void Start () {
        //QuesText.text = static_class.Courses[static_class.course_id].Clips[static_class.clip_id].Ques;
        Button btn_h = home_btn.GetComponent<Button>();
        btn_h.onClick.AddListener(onclick1);

        btn_n = next_btn.GetComponent<Button>();
        btn_n.onClick.AddListener(onclick2);
    }

    public void onclick1()
    {
        SceneManager.LoadScene(0);
    }
    public void onclick2()
    {
        if (static_class.clip_id != static_class.Courses[static_class.course_id].Clips_len - 1)
        {
            static_class.clip_id++;
            SceneManager.LoadScene(1);
        }
        else
        {
            //QuesText.text = "這是最後一題了，請回首頁";
            btn_n.enabled = false;
        }
    }

}
