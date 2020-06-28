﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Control : MonoBehaviour
{
    public float _progressSlider;  //************************************************************
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
    int now_nosound;
    int now_startsound;


    void Start()
    {
        pic_btn = pic_button.GetComponent<Button>();
        now_nosound = static_class.Courses[static_class.course_id].Clips[static_class.clip_id].Start_nosound;
        now_startsound = static_class.Courses[static_class.course_id].Clips[static_class.clip_id].End_nosound;
        status.text = "影片讀取中，請稍等。";
        p_btn = play_btn.GetComponent<Button>();
        p_btn.onClick.AddListener(Play_video);
        static_class.finished_last_record = false;
        _progressSlider = static_class.Courses[static_class.course_id].Clips[static_class.clip_id].Time;
        
        Play_video();

    }

    IEnumerator Example()
    {
        int time_gap;//到下一次回答問題過多久(影片播放長度)
        //要改成 >> 這次問題點-上次問題點
        if (static_class.clip_id == 0)
        {
            time_gap = static_class.Courses[static_class.course_id].Clips[static_class.clip_id].Time;
        }
        else
        {
            time_gap = static_class.Courses[static_class.course_id].Clips[static_class.clip_id].Time - static_class.Courses[static_class.course_id].Clips[static_class.clip_id - 1].Time;
        }
        Debug.Log("time_gap: " + time_gap);
        yield return new WaitForSeconds(time_gap);

        YourCustomFunction._instance.ytPlayer.Pause();
    }

    public void pushBTN()
    {
        is_there = true;
        if (!static_class.google_speeching && YourCustomFunction._instance.ytPlayer.pauseCalled)
            StartCoroutine(CountThree("btn"));
    }
    public void exitpoint()
    {
        if (static_class.google_speeching)
        {
            Play_video();//停止錄音
        }
        is_there = false;
    }

    public void pushHOME_BTN()
    {
        is_there = true;
        StartCoroutine(CountThree("home"));
    }

    bool is_there = false;

    IEnumerator CountThree(string mode)
    {
        count3Text.text = "3";
        yield return new WaitForSeconds(1);
        if (is_there)
        {
            count3Text.text = "2";
            yield return new WaitForSeconds(1);
            if (is_there)
            {
                count3Text.text = "1";
                yield return new WaitForSeconds(1);
                if (is_there)
                {
                    if (mode == "home")
                    {
                        static_class.btn_mode = 0;
                        SceneManager.LoadScene(0);
                    }
                    else
                    {
                        count3Text.text = "0";
                        Play_video();
                    }
                }
            }
        }
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
                        //把剩下的影片播完，不用再StartCoroutine(Example());
                    }
                    else
                    {                           //撥放完要回答問題
                        status.text = "影片停止後，按下按鈕開始錄製回答。";
                        p_btn.GetComponentInChildren<Text>().text = "Start Record";
                        pic_btn.image.sprite = microphone_icon;
                        static_class.btn_mode = 1;
                        YourCustomFunction._instance.ytPlayer.Play();
                        StartCoroutine(Example());
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
                //else
                //{                                   //接下來繼續Play
                //    static_class.btn_mode = 0;
                //    p_btn.GetComponentInChildren<Text>().text = "Play";
                //    //如果這是最後一個回答
                //    static_class.ans_is_OK = false;
                //    if (static_class.clip_id == static_class.Courses[static_class.course_id].Clips_len - 1)
                //        finished_last_record = true;
                //}
                GoogleVoiceSpeech._instance.OnClick();
                break;
            case 3:     //回首頁
                static_class.btn_mode = 0;
                SceneManager.LoadScene(0);
                break;

        }
    }

    void Update()
    {
        //Debug.Log("now_nosound : " + now_nosound);
        //Debug.Log("now_startsound : " + now_startsound);
        if (YourCustomFunction._instance.ytPlayer.currentVideoDuration >= now_nosound && YourCustomFunction._instance.ytPlayer.currentVideoDuration <= now_startsound)
        {
            //Debug.Log("0000000000000 : "+ YourCustomFunction._instance.ytPlayer.currentVideoDuration);
            YourCustomFunction._instance.ytPlayer.GetComponent<AudioSource>().volume = 0;
        }
        else
        {
            now_nosound = static_class.Courses[static_class.course_id].Clips[static_class.clip_id].Start_nosound;
            now_startsound = static_class.Courses[static_class.course_id].Clips[static_class.clip_id].End_nosound;
            YourCustomFunction._instance.ytPlayer.GetComponent<AudioSource>().volume = 1;
            //Debug.Log("11111111 : " + YourCustomFunction._instance.ytPlayer.currentVideoDuration);

        }
    }
}
