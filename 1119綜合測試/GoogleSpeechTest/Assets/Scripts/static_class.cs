using System.Collections.Generic;

public static class static_class
{
    public struct clip_info
    {
        public int Time;
        //public int Length;
        public string Ques;
        public int Start_nosound;
        public int End_nosound;
        public int Ans_len;
        public List<string> Ans;
    }
    public struct course_info
    {
        public string Name;
        public string Youtube_id;
        public int Clips_len;
        public List<clip_info> Clips;
    }

    public static int course_id;
    public static int clip_id;  //從0開始
    public static List<course_info> Courses = new List<course_info>();      //JSON內容
    public static bool do_record = false;
    public static bool ans_is_OK = false;                                   //回答有沒有過
    public static int btn_mode = 0;                                         //按鈕模式
    public static bool finished_last_record = false;                        //是否是最後一個問題，如果是就回首頁
    public static bool movie_OK = false;                                    //影片準備好了
    public static bool google_speeching=false;                              //正在錄音
    //public static bool video_OK = false;
}