using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleJSON;

public class player : MonoBehaviour
{
    private string url = "https://yzu-vmlab-team.github.io/JPcourse_JSON/JPcourse.json"; 
    private WWW www = null;
    public string jsonString;
    void Start()
    {
        www = new WWW(url);
        StartCoroutine(ReceiveResponse());
    }

    private IEnumerator ReceiveResponse()
    {
        yield return www;

        jsonString=www.text;
    }
    //---------------------------------------------------------------------------------------------------------------
    public struct clip_info
    {
        public string Time;
        public string Length;
        public string Ques;
        public string Ans;
    }
    public struct course_info
    {
        public string Name;
        public string Youtube_id;
        public int Clips_len;
        public List<clip_info> Clips;
    }

    public List<course_info> Courses = new List<course_info>();

    void Load()
    {
        /*string path = Application.persistentDataPath + "/test.json";
        string jsonString = File.ReadAllText(path);*/
        
        JSONObject playerJson = (JSONObject)JSON.Parse(jsonString);

        //取得資料數
        int Course_len = playerJson["course_length"];
        Debug.Log("course_length : " + Course_len);

        //建立courses陣列
        for (int i = 0; i < Course_len; i++)
        {
            //*********
            course_info co_info;
            co_info.Name = playerJson["courses"].AsArray[i]["name"];
            co_info.Youtube_id = playerJson["courses"].AsArray[i]["youtube_id"];
            co_info.Clips_len = playerJson["courses"].AsArray[i]["clips_len"];

            co_info.Clips = new List<clip_info>();

            //建立clips陣列
            for (int j = 0; j < co_info.Clips_len; j++)
            {
                //***
                clip_info cl_info;
                cl_info.Time = playerJson["courses"].AsArray[i]["clips"].AsArray[j]["time"];
                cl_info.Length = playerJson["courses"].AsArray[i]["clips"].AsArray[j]["length"];
                cl_info.Ques = playerJson["courses"].AsArray[i]["clips"].AsArray[j]["ques"];
                cl_info.Ans = playerJson["courses"].AsArray[i]["clips"].AsArray[j]["ans"];
                //***
                co_info.Clips.Add(cl_info);
            }

            //*********
            Courses.Add(co_info);
        }

            ///////////////////////////////////////////////////////////////
            Debug.Log(Courses[0].Name);
            Debug.Log(Courses[0].Youtube_id);
            Debug.Log(Courses[0].Clips_len);
            Debug.Log(Courses[0].Clips[0].Time);
            Debug.Log(Courses[0].Clips[0].Length);
            Debug.Log(Courses[0].Clips[0].Ques);
            Debug.Log(Courses[0].Clips[0].Ans);
            Debug.Log(Courses[0].Clips[1].Time);
            Debug.Log(Courses[0].Clips[1].Length);
            Debug.Log(Courses[0].Clips[1].Ques);
            Debug.Log(Courses[0].Clips[1].Ans);
            Debug.Log(Courses[0].Clips[2].Time);
            Debug.Log(Courses[0].Clips[2].Length);
            Debug.Log(Courses[0].Clips[2].Ques);
            Debug.Log(Courses[0].Clips[2].Ans);

            Debug.Log(Courses[1].Name);
            Debug.Log(Courses[1].Youtube_id);
            Debug.Log(Courses[1].Clips_len);
            Debug.Log(Courses[1].Clips[0].Time);
            Debug.Log(Courses[1].Clips[0].Length);
            Debug.Log(Courses[1].Clips[0].Ques);
            Debug.Log(Courses[1].Clips[0].Ans);
            Debug.Log(Courses[1].Clips[1].Time);
            Debug.Log(Courses[1].Clips[1].Length);
            Debug.Log(Courses[1].Clips[1].Ques);
            Debug.Log(Courses[1].Clips[1].Ans);
        
        
    }
    

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.L))
        {
            Load();
            //Debug.Log(Name);
        }
    }

    
}
