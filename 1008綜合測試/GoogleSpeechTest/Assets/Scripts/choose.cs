﻿using SimpleJSON;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class choose : MonoBehaviour {
    public Button btn_next, btn_before, btn_middle;
    private int course_id = 0;
    private int Course_len = 0;
    private string url = "https://vrteachingmaterial.github.io/JPcourse_JSON/JPcourse.json";
	//"https://yzu-vmlab-team.github.io/JPcourse_JSON/JPcourse.json";	
	
    private WWW www = null;
    string jsonString;

    //---------------------------------------------------------------------------------------------------------------
    void Start() {
        //json用
        www = new WWW(url);
        while (!www.isDone)
        {
        }
        jsonString = www.text;
        Load();

        Button btn_n = btn_next.GetComponent<Button>();
        Button btn_b = btn_before.GetComponent<Button>();
        Button btn_m = btn_middle.GetComponent<Button>();

        //Calls the TaskOnClick/TaskWithParameters method when you click the Button
        btn_n.onClick.AddListener(onclick3);
        btn_b.onClick.AddListener(onclick2);
        btn_m.onClick.AddListener(onclick);
        
        //改中間的字
        btn_middle.GetComponentInChildren<Text>().text = static_class.Courses[0].Name;

    }
    public void onclick(){
        static_class.course_id = course_id;
        static_class.clip_id = 0;
        SceneManager.LoadScene(1);
    }
    public void onclick2()
    {
        if (course_id != Course_len - 1) course_id++;
        else course_id = 0;
        btn_middle.GetComponentInChildren<Text>().text = static_class.Courses[course_id].Name;
    }
    public void onclick3()
    {
        if (course_id != 0) course_id--;
        else course_id = Course_len - 1;
        btn_middle.GetComponentInChildren<Text>().text = static_class.Courses[course_id].Name;
    }

    // Update is called once per frame
    void Update () {
        if (Input.GetButtonDown("A"))   //next
        {
            if (course_id != Course_len - 1) course_id++;
            else course_id = 0;

            btn_middle.GetComponentInChildren<Text>().text = static_class.Courses[course_id].Name;
        }
        if (Input.GetButtonDown("B"))   //before
        {
            if (course_id != 0) course_id--;
            else course_id = Course_len-1;

            btn_middle.GetComponentInChildren<Text>().text = static_class.Courses[course_id].Name;
        }
        if (Input.GetButtonDown("C"))   //enter
        {
            static_class.course_id = course_id;
            static_class.clip_id = 0;
            static_class.google_speeching = false;
            static_class.finished_last_record = false;
            static_class.movie_OK = false;
            static_class.btn_mode = 0;
            SceneManager.LoadScene(1);
        }
        if (Input.GetKeyDown(KeyCode.L))
        {
            Load();
            //Debug.Log(Name);
        }
    }
    
    void Load()
    {
        JSONObject playerJson = (JSONObject)JSON.Parse(jsonString);

        //取得資料數
        Course_len = playerJson["course_length"];
        Debug.Log("course_length : " + Course_len);

        //建立courses陣列
        for (int i = 0; i < Course_len; i++)
        {
            //*********
            static_class.course_info co_info;
            co_info.Name = playerJson["courses"].AsArray[i]["name"];
            co_info.Youtube_id = playerJson["courses"].AsArray[i]["youtube_id"];
            co_info.Clips_len = playerJson["courses"].AsArray[i]["clips_len"];

            co_info.Clips = new List<static_class.clip_info>();

            //建立clips陣列
            for (int j = 0; j < co_info.Clips_len; j++)
            {
                //***
                static_class.clip_info cl_info;
                cl_info.Time = playerJson["courses"].AsArray[i]["clips"].AsArray[j]["time"];
                //cl_info.Length = playerJson["courses"].AsArray[i]["clips"].AsArray[j]["length"];
                cl_info.Ques = playerJson["courses"].AsArray[i]["clips"].AsArray[j]["ques"];
                cl_info.Ans = playerJson["courses"].AsArray[i]["clips"].AsArray[j]["ans"];
                //***
                co_info.Clips.Add(cl_info);
            }

            //*********
            static_class.Courses.Add(co_info);
        }
    }
    
}
