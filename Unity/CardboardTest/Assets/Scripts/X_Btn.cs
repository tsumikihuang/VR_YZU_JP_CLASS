using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Btn : MonoBehaviour {
    public Button button1, button2;
    int i = 0;
    // Use this for initialization
    void Start () {
        Button btn1 = button1.GetComponent<Button>();
        Button btn2 = button2.GetComponent<Button>();

        //Calls the TaskOnClick/TaskWithParameters method when you click the Button
        btn1.onClick.AddListener(btClick);
        btn2.onClick.AddListener(btClick);

        var colors = button1.GetComponent<Button>().colors;
        colors.normalColor = Color.red;
        btn1.colors = colors;
    }

    void Update()
    {
        var colors = button1.GetComponent<Button>().colors;

        if (Input.GetButtonDown("C"))
        {
            if (i == 0)
            {
                i++;
                colors.normalColor = Color.red;
                button2.GetComponent<Button>().colors = colors;
                colors.normalColor = Color.white;
                button1.GetComponent<Button>().colors = colors;
            }
            else{
                i--;
                colors.normalColor = Color.red;
                button1.GetComponent<Button>().colors = colors;
                colors.normalColor = Color.white;
                button2.GetComponent<Button>().colors = colors;
            }
        }

        if (Input.GetButtonDown("A") || Input.GetButtonDown("B") || Input.GetButtonDown("D"))
        {
            if (i==0)
                SceneManager.LoadScene(1);
            else if (i == 1)
                SceneManager.LoadScene(2);
        }
    }
    void btClick()
    {
        if (i == 0)
            SceneManager.LoadScene(1);
        else if (i == 1)
            SceneManager.LoadScene(2);
    }
}
