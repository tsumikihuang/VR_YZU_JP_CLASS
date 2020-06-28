using SimpleJSON;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class choose : MonoBehaviour
{
    public Button btn_middle;
    private int course_id = 0;
    private int Course_len = 0;
    private string url = "https://vrteachingmaterial.github.io/JPcourse_JSON/JPcourse.json";
    //h ttps://yzu-vmlab-team.github.io/JPcourse_JSON/JPcourse.json

    public Text count3Text;

    bool is_net = true;
    private WWW www = null;
    string jsonString;

    //---------------------------------------------------------------------------------------------------------------
    void Start()
    {
        //json用
        www = new WWW(url);
        while (!www.isDone)
        {
        }
        jsonString = www.text;
        Load();


        //改中間的字
        btn_middle.GetComponentInChildren<Text>().text = static_class.Courses[0].Name;

    }
    IEnumerator CountThree()
    {
        count3Text.text = "3";
        yield return new WaitForSeconds(0.5f);
        count3Text.text = "2";
        yield return new WaitForSeconds(0.5f);
        count3Text.text = "1";
        yield return new WaitForSeconds(0.5f);
        count3Text.text = "0";
        if (is_net)
        {
            static_class.course_id = course_id;
            static_class.clip_id = 0;
            //SceneManager.LoadScene(1);
            YourCustomFunction._instance.ytPlayer.Play("https://www.youtube.com/watch?v=" + static_class.Courses[course_id].Youtube_id);
            OK = true;
        }
        count3Text.text = "";
    }

    bool OK = false;
    private void Update()
    {
        if (OK && YourCustomFunction._instance.ytPlayer.youtubeUrlReady && YourCustomFunction._instance.ytPlayer.videoPlayer.isPrepared)
        {
            SceneManager.LoadScene(1);
        }
    }

    private IEnumerator Count3;

    public void onclick()///////////////////////////////////////////////////////////////////////////////////// 進入課程
    {
        Count3 = CountThree();
        StartCoroutine(Count3);
    }

    public void exitpoint()///////////////////////////////////////////////////////////////////////////////// 跑掉
    {
        if (Count3 != null)
        {
            StopCoroutine(Count3);
            Count3 = null;
            count3Text.text = "";
        }
    }

    public void onclick3()////////////////////////////////////////////////////////////////////////////////// 後一個課程
    {
        if (course_id != Course_len - 1) course_id++;
        else course_id = 0;
        btn_middle.GetComponentInChildren<Text>().text = static_class.Courses[course_id].Name;
    }

    public void onclick2()///////////////////////////////////////////////////////////////////////////////// 前一個課程
    {
        if (course_id != 0) course_id--;
        else course_id = Course_len - 1;
        btn_middle.GetComponentInChildren<Text>().text = static_class.Courses[course_id].Name;
    }

    void Load()
    {
        JSONObject playerJson = (JSONObject)JSON.Parse(jsonString);

        if (playerJson == null)//沒有網路
        {
            is_net = false;
            btn_middle.GetComponentInChildren<Text>().text = "沒感測到網路，請重啟APP";
            Debug.Log("沒有獲取json檔，可能是因為沒有網路");
            return;
        }
        //取得資料數
        Course_len = playerJson["course_length"];
        //Debug.Log("course_length : " + Course_len);

        //建立courses陣列
        for (int i = 0; i < Course_len; i++)
        {
            //*********
            static_class.course_info co_info;
            co_info.Name = playerJson["courses"].AsArray[i]["name"];
            co_info.Youtube_id = playerJson["courses"].AsArray[i]["youtube_id"];
            co_info.Total_len = playerJson["courses"].AsArray[i]["total_len"];
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
                cl_info.Start_nosound = playerJson["courses"].AsArray[i]["clips"].AsArray[j]["start_nosound"];
                cl_info.End_nosound = playerJson["courses"].AsArray[i]["clips"].AsArray[j]["end_nosound"];
                cl_info.Ans_len = playerJson["courses"].AsArray[i]["clips"].AsArray[j]["ans_len"];

                //建立ans陣列
                cl_info.Ans = new List<string>();
                for (int k = 0; k < cl_info.Ans_len; k++)
                {
                    cl_info.Ans.Add(playerJson["courses"].AsArray[i]["clips"].AsArray[j]["ans"].AsArray[k]);
                }

                //***
                co_info.Clips.Add(cl_info);
            }

            //*********
            static_class.Courses.Add(co_info);
        }
    }

}
