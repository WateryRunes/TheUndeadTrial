using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialPotion : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnDestroy() // cause portcullis door to raise
    {
        GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>().trigger("Trigger3");
    }
}
