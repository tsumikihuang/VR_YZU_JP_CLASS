using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Control : MonoBehaviour
{
    //public float _progressSlider;  //************************************************************
    public Text quesText;
    public Button play_btn;
    Button p_btn, pic_btn;
    public Text count3Text;

    public Button pic_button;
    public Sprite microphone_icon;
    public Sprite record_icon;
    public Sprite play_icon;
    public Sprite home_icon;

    public Text status;

    bool firsttime = true;
    bool n_t_s = true;
    float now_nosound;
    float now_startsound;


    void Start()
    {
        pic_btn = pic_button.GetComponent<Button>();
        now_nosound = static_class.Courses[static_class.course_id].Clips[static_class.clip_id].Start_nosound;
        now_startsound = static_class.Courses[static_class.course_id].Clips[static_class.clip_id].End_nosound;
        status.text = "影片讀取中，請稍等。";
        p_btn = play_btn.GetComponent<Button>();
        p_btn.onClick.AddListener(Play_video);
        static_class.finished_last_record = false;
        //_progressSlider = static_class.Courses[static_class.course_id].Clips[static_class.clip_id].Time;

        Play_video();

    }
    float next_time_stop = 0;     //下一次影片停止的時間點
    bool callOnce = false;

    private IEnumerator Count3;

    public void pushBTN()
    {
        if ((!static_class.google_speeching && YourCustomFunction._instance.ytPlayer.pauseCalled) || static_class.finished_last_record)
        {
            Count3 = CountThree("btn");
            StartCoroutine(Count3);
        }
    }
    public void exitpoint()
    {
        if (static_class.google_speeching)
        {
            Play_video();//停止錄音
        }
        if (Count3 != null)
        {
            StopCoroutine(Count3);
            count3Text.text = "";
            Count3 = null;
        }
    }

    public void pushHOME_BTN()
    {
        Count3 = CountThree("home");
        StartCoroutine(Count3);

    }


    IEnumerator CountThree(string mode)
    {
        count3Text.text = "3";
        yield return new WaitForSeconds(0.5f);
        count3Text.text = "2";
        yield return new WaitForSeconds(0.5f);
        count3Text.text = "1";
        yield return new WaitForSeconds(0.5f);

        if (mode == "home")
            static_class.btn_mode = 3;
        else
            count3Text.text = "0";

        Play_video();
        count3Text.text = "";
    }


    public void Play_video()
    {
        switch (static_class.btn_mode)
        {
            case 0:     //播放
                quesText.text = static_class.Courses[static_class.course_id].Clips[static_class.clip_id].Ques;
                //如果是最後問題回答完，撥放完就回首頁
                if (static_class.finished_last_record)
                {                           //撥放完就沒了，接下來回首頁
                    status.text = "已經是最後一題，回首頁。";
                    p_btn.GetComponentInChildren<Text>().text = "Home";
                    pic_btn.image.sprite = home_icon;
                    static_class.btn_mode = 3;
                    YourCustomFunction._instance.ytPlayer.Play();
                }
                else
                {                           //撥放完要回答問題
                    status.text = "影片停止後，按下按鈕開始錄製回答。";
                    p_btn.GetComponentInChildren<Text>().text = "Start Record";
                    pic_btn.image.sprite = microphone_icon;
                    static_class.btn_mode = 1;
                    YourCustomFunction._instance.ytPlayer.Play();
                    next_time_stop = static_class.Courses[static_class.course_id].Clips[static_class.clip_id].Time;
                    n_t_s = true;

                }
                break;
            case 1:     //開始錄音，接下來結束錄音
                if (!static_class.google_speeching && YourCustomFunction._instance.ytPlayer.pauseCalled)  //如果語音還再辨識中或影片還在撥放中，就不執行以下
                {
                    static_class.btn_mode = 2;
                    status.text = "答案錄製中，按下按鈕停止錄音。";
                    p_btn.GetComponentInChildren<Text>().text = "Stop Record";
                    pic_btn.image.sprite = record_icon;
                    GoogleVoiceSpeech._instance.OnClick();
                    static_class.google_speeching = true;
                }
                break;
            case 2:     //結束錄音
                if (!static_class.ans_is_OK)    //接下來再回答一次      //如果OK的情況寫在GoogleVoiceSpeech.cs
                {
                    status.text = "回答未達標準，再次錄製回答。";
                    static_class.btn_mode = 1;
                    p_btn.GetComponentInChildren<Text>().text = "Start Record";
                    pic_btn.image.sprite = microphone_icon;
                }
                GoogleVoiceSpeech._instance.OnClick();
                break;
            case 3:     //回首頁
                static_class.btn_mode = 0;
                YourCustomFunction._instance.ytPlayer.Stop();
                SceneManager.LoadScene(0);
                break;

        }
    }

    void Update()
    {
        if (YourCustomFunction._instance.ytPlayer.currentVideoDuration >= now_nosound && YourCustomFunction._instance.ytPlayer.currentVideoDuration <= now_startsound)
        {
            YourCustomFunction._instance.ytPlayer.GetComponent<AudioSource>().volume = 0;
        }
        else
        {
            now_nosound = static_class.Courses[static_class.course_id].Clips[static_class.clip_id].Start_nosound;
            now_startsound = static_class.Courses[static_class.course_id].Clips[static_class.clip_id].End_nosound;
            YourCustomFunction._instance.ytPlayer.GetComponent<AudioSource>().volume = 1;
        }

        if (!callOnce && YourCustomFunction._instance.ytPlayer.currentVideoDuration >= next_time_stop && n_t_s)
        {
            callOnce = true;
            Debug.Log("Stop:" + YourCustomFunction._instance.ytPlayer.currentVideoDuration);
            Debug.Log("next_time_stop:" + next_time_stop);
            YourCustomFunction._instance.ytPlayer.Pause();
            n_t_s = false;
        }
        else if (callOnce)
            callOnce = false;

    }
}
