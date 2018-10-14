//	Copyright (c) 2016 steele of lowkeysoft.com
//        http://lowkeysoft.com
//
//	This software is provided 'as-is', without any express or implied warranty. In
//	no event will the authors be held liable for any damages arising from the use
//	of this software.
//
//	Permission is granted to anyone to use this software for any purpose,
//	including commercial applications, and to alter it and redistribute it freely,
//	subject to the following restrictions:
//
//	1. The origin of this software must not be misrepresented; you must not claim
//	that you wrote the original software. If you use this software in a product,
//	an acknowledgment in the product documentation would be appreciated but is not
//	required.
//
//	2. Altered source versions must be plainly marked as such, and must not be
//	misrepresented as being the original software.
//
//	3. This notice may not be removed or altered from any source distribution.
//
//  =============================================================================
//
// Acquired from https://github.com/steelejay/LowkeySpeech
//
using UnityEngine;
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine.UI;

[RequireComponent (typeof (AudioSource))]

public class GoogleVoiceSpeech : MonoBehaviour {

    public Text ansText,scoreText,trueAns;
    public Button play_btn;
    private Button p_btn;
    private bool is_record=false;
    private GUIStyle guiStyle = new GUIStyle(); //create a new variable
    private int ans_wrong=0;

    const int HEADER_SIZE = 44;

    string result_global = "";//****************************************************************

    private int minFreq;
	private int maxFreq;

	private bool micConnected = false;

	//A handle to the attached AudioSource
	private AudioSource goAudioSource;

	public string apiKey;

	// Use this for initialization
	void Start () {
        //###
        p_btn = play_btn.GetComponent<Button>();
        ans_wrong = 0;

        //Check if there is at least one microphone connected
        if (Microphone.devices.Length <= 0)
		{
				//Throw a warning message at the console if there isn't
				Debug.LogWarning("Microphone not connected!");
		}
		else //At least one microphone is present
		{
				//Set 'micConnected' to true
				micConnected = true;

				//Get the default microphone recording capabilities
				Microphone.GetDeviceCaps(null, out minFreq, out maxFreq);

				//According to the documentation, if minFreq and maxFreq are zero, the microphone supports any frequency...
				if(minFreq == 0 && maxFreq == 0)
				{
						//...meaning 44100 Hz can be used as the recording sampling rate
						maxFreq = 44100;
				}

				//Get the attached AudioSource component
				goAudioSource = this.GetComponent<AudioSource>();
		}
	}

    void Update()
    {
        if(static_class.do_record == true)
        {
            static_class.do_record = false;
            OnClick();
        }
    }

    //#####################################################################################################################
    private void OnClick()
    {
        if (micConnected)
        {
            
            //If the audio from any microphone isn't being recorded
            if (!Microphone.IsRecording(null))          //還沒錄音
            {
                //Case the 'Record' button gets pressed
                if (!is_record)
                {
                    is_record = !is_record;
                    //Start recording and store the audio captured from the microphone at the AudioClip in the AudioSource
                    goAudioSource.clip = Microphone.Start(null, true, 7, maxFreq); //Currently set for a 7 second clip
                }
            }
            else //Recording is in progress             //正在錄音
            {
                //Case the 'Stop and Play' button gets pressed
                if (is_record)
                {
                    is_record = !is_record;
                    
                    float filenameRand = UnityEngine.Random.Range(0.0f, 10.0f);

                    string filename = "testing" + filenameRand;

                    Microphone.End(null); //Stop the audio recording

                    Debug.Log("Recording Stopped");

                    if (!filename.ToLower().EndsWith(".wav"))
                    {
                        filename += ".wav";
                    }

                    var filePath = Path.Combine("testing/", filename);
                    filePath = Path.Combine(Application.persistentDataPath, filePath);
                    Debug.Log("Created filepath string: " + filePath);

                    // Make sure directory exists if user is saving to sub dir.
                    Directory.CreateDirectory(Path.GetDirectoryName(filePath));
                    SavWav.Save(filePath, goAudioSource.clip); //Save a temporary Wav File
                    Debug.Log("Saving @ " + filePath);
                    string apiURL = "https://speech.googleapis.com/v1/speech:recognize?&key=AIzaSyAti4kyTwgVbXXmEVJ6_Z7ezoxzJmJGcw4";
                    string Response;

                    Debug.Log("Uploading " + filePath);
                    Response = HttpUploadFile(apiURL, filePath, "file", "audio/wav; rate=44100");
                    Debug.Log("Response String: " + Response);

                    var jsonresponse = SimpleJSON.JSON.Parse(Response);

                    if (jsonresponse != null)
                    {
                        string resultString = jsonresponse["result"][0].ToString();
                        var jsonResults = SimpleJSON.JSON.Parse(resultString);

                        string transcripts = jsonResults["alternative"][0]["transcript"].ToString();

                        Debug.Log("transcript string: " + transcripts);


                    }
                    //goAudioSource.Play(); //Playback the recorded audio

                    File.Delete(filePath); //Delete the Temporary Wav file

                }
                ansText.text = result_global;
                //GUI.Label(new Rect(Screen.width / 2 - 100, Screen.height / 2 + 25, 200, 50), "Recording in progress...");

                string true_ans = static_class.Courses[static_class.course_id].Clips[static_class.clip_id].Ans;
                float num_of_error = Compute(true_ans, result_global);
                float compute = 100 * (1 - num_of_error / true_ans.Length);
                if (compute < 0)
                    compute = 0;
                if (compute < 60)
                {
                    ans_wrong++;
                    if (ans_wrong == 3)        //如果回答錯三次，就不管他，讓他過
                        static_class.ans_is_OK = true;
                }
                else
                    static_class.ans_is_OK = true;
                
                if (static_class.ans_is_OK)         //接下來繼續Play
                {
                    ans_wrong = 0;
                    static_class.btn_mode = 0;
                    p_btn.GetComponentInChildren<Text>().text = "Play";
                    static_class.ans_is_OK = false;
                    //如果這是最後一個回答
                    if (static_class.clip_id == static_class.Courses[static_class.course_id].Clips_len - 1)
                        static_class.finished_last_record = true;
                    else
                        static_class.clip_id++;
                }
                Debug.Log(num_of_error);
                scoreText.text = compute.ToString();

                trueAns.text = true_ans;

                Debug.Log(compute + "------------------------------------------------------------------");
                Debug.Log(ans_wrong + "------------------------------------------------------------------");
                Debug.Log(static_class.ans_is_OK + "------------------------------------------------------------------");
            }
        }
        else // No microphone
        {
            //Print a red "Microphone not connected!" message at the center of the screen
            //GUI.contentColor = Color.red;
            //GUI.Label(new Rect(Screen.width / 2 - 100, Screen.height / 2 - 25, 200, 50), "Microphone not connected!");

            //!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
            Debug.Log("Microphone not connected!");
            ansText.text = "Microphone not connected!";
        }

    }

    //####################################################################################################################
    public string HttpUploadFile(string url, string file, string paramName, string contentType)
    {

        System.Net.ServicePointManager.ServerCertificateValidationCallback += (o, certificate, chain, errors) => true;
        Debug.Log(string.Format("Uploading {0} to {1}", file, url));

        Byte[] bytes = File.ReadAllBytes(file);
        String file64 = Convert.ToBase64String(bytes,
                                         Base64FormattingOptions.None);

        Debug.Log(file64);

        try
        {

            var httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
            httpWebRequest.ContentType = "application/json";
            httpWebRequest.Method = "POST";

            using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
            {
                //string json = "{ \"config\": { \"languageCode\" : \"en-US\" }, \"audio\" : { \"content\" : \"" + file64 + "\"}}";
                string json = "{ \"config\": { \"languageCode\" : \"ja-JP\" }, \"audio\" : { \"content\" : \"" + file64 + "\"}}";

                Debug.Log(json);
                streamWriter.Write(json);
                streamWriter.Flush();
                streamWriter.Close();
            }

            var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
            Debug.Log(httpResponse);

            using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
            {
                var result = streamReader.ReadToEnd();
                static_class.google_speeching = false;//************************************************
                Debug.Log("Response:" + result);
                //******************************************************************
                if (result != "{}\n") { 
                    string[] str_array = result.Split('\n');    //result.Split()回傳string[]
                    string[] str2_array = str_array[5].Split('"');
                    result_global = str2_array[3];
                }
                else//如果是空字串
                {
                    result_global = "";
                }
            }

        } catch (WebException ex) {
         var resp = new StreamReader(ex.Response.GetResponseStream()).ReadToEnd();
                    Debug.Log(resp);
 
        }

        return "empty";
		
	}
    //**********************************************************************************字串比對(不一樣的數量)
    public static int Compute(string s, string t)
    {
        int n = s.Length;
        int m = t.Length;
        int[,] d = new int[n + 1, m + 1];

        // Step 1
        if (n == 0)
        {
            return m;
        }

        if (m == 0)
        {
            return n;
        }

        // Step 2
        for (int i = 0; i <= n; d[i, 0] = i++)
        {
        }

        for (int j = 0; j <= m; d[0, j] = j++)
        {
        }

        // Step 3
        for (int i = 1; i <= n; i++)
        {
            //Step 4
            for (int j = 1; j <= m; j++)
            {
                // Step 5
                int cost = (t[j - 1] == s[i - 1]) ? 0 : 1;

                // Step 6
                d[i, j] = Math.Min(
                    Math.Min(d[i - 1, j] + 1, d[i, j - 1] + 1),
                    d[i - 1, j - 1] + cost);
            }
        }
        // Step 7
        return d[n, m];
    }
}
		
