using UnityEngine;

public class TriggerWall : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag.Equals("Player"))
        {
            GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>().trigger(name);
            Destroy(gameObject);
        }
    }
}
