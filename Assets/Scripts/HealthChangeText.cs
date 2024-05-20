using System.Collections;
using System.Collections.Generic;
using System.Threading;
using TMPro;
using UnityEngine;

public class HealthChangeText : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update() // move up and disappear slowly
    {
        if (GetComponent<TextMeshProUGUI>().enabled) {
            transform.position = new Vector3(transform.position.x, transform.position.y + (30f * Time.deltaTime), transform.position.z);
            GetComponent<TextMeshProUGUI>().alpha -= (100f * Time.deltaTime); // changing alpha of popup is not working

            if (transform.position.y > 30)
            {
                GetComponent<TextMeshProUGUI>().enabled = false;
            }
        }
    }
}
