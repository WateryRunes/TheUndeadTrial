using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EndDoor : MonoBehaviour
{
    public bool raising = true;
    public bool playing = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update() // raise until walk throughable
    {
        if (raising)
        {
            if(!playing)
            {
                playing = true;
                GetComponent<AudioSource>().Play();
            }

            transform.position += new Vector3(0, 2f * Time.deltaTime, 0);

            if (SceneManager.GetActiveScene().buildIndex == 1 && transform.position.y > 5.0f)
            {
                raising = false;
            }
            if (SceneManager.GetActiveScene().buildIndex == 2 && transform.position.y > 3.85f)
            {
                raising = false;
            }
            if (SceneManager.GetActiveScene().buildIndex == 3 && transform.position.y > 7f)
            {
                raising = false;
            }

            if (!raising)
            {
                GetComponent<AudioSource>().Stop();
            }
        }


        
    }
}
