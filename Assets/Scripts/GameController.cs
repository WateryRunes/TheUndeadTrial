using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    public TMP_Text healthText;
    public TMP_Text healthChangeText;
    public TMP_Text instructionText;
    public GameObject player;
    public Image optionsBackground;
    public Slider volumeSlider;
    public GameObject pauseCollection;
    public GameObject optionsCollection;
    public GameObject deathCollection;
    public GameObject winCollection;
    public List<GameObject> meleeEnemiesCollection;
    public List<GameObject> rangedEnemiesCollection;
    public GameObject endDoor;
    public bool levelOver;
    public bool hardmode;
    public float dmgMulti;

    // Start is called before the first frame update
    void Start()
    {
        dmgMulti = 1.0f;
        hardmode = PlayerPrefs.GetInt("hardmode") == 1 ? true : false;
        AudioListener.volume = PlayerPrefs.GetFloat("volume");
        volumeSlider.value = AudioListener.volume;

        healthChangeText.enabled = false;
        optionsBackground.enabled = false;

        pauseCollection.SetActive(false);
        optionsCollection.SetActive(false);
        deathCollection.SetActive(false);
        winCollection.SetActive(false);
    }

    void OnLevelWasLoaded(int level)
    {
        StartCoroutine(waitAndLoad(1, level));       
    }

    IEnumerator waitAndLoad(int delay, int level)
    {
        yield return new WaitForSeconds(delay);
        levelOver = false;
        player = GameObject.FindGameObjectWithTag("Player");
        endDoor = GameObject.FindGameObjectWithTag("EndDoor");
        meleeEnemiesCollection.AddRange(GameObject.FindGameObjectsWithTag("MeleeEnemy").ToList<GameObject>());
        rangedEnemiesCollection.AddRange(GameObject.FindGameObjectsWithTag("RangedEnemy").ToList<GameObject>());
        if (level == 2)
        {
            player.GetComponent<PlayerController>().updateHealth(100, false); // set health to max after tutorial
            instructionText.enabled = false;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(!levelOver)
        {
            if (meleeEnemiesCollection.All(x => x.GetComponent<MeleeEnemyController>().dead == true) && rangedEnemiesCollection.All(x => x.GetComponent<RangedEnemyController>().dead == true))
            {
                endDoor.GetComponent<EndDoor>().raising = true;
                levelOver = true;
                meleeEnemiesCollection.Clear();
                rangedEnemiesCollection.Clear();

                if(SceneManager.GetActiveScene().buildIndex == 1)
                {
                    trigger("Trigger4");
                }
            }
        }
    }

    public void togglePauseMenu()
    {

        if (optionsCollection.activeInHierarchy)
        {
            pauseCollection.SetActive(!pauseCollection.activeInHierarchy);
            optionsCollection.SetActive(false);
        }
        else
        {
            optionsBackground.enabled = !optionsBackground.enabled;
            pauseCollection.SetActive(!pauseCollection.activeInHierarchy);
        }
    }

    private void toggleOptionsMenu()
    {
        Cursor.visible = true;
        pauseCollection.SetActive(!pauseCollection.activeInHierarchy);
        optionsCollection.SetActive(!optionsCollection.activeInHierarchy);
    }

    public void deathScreen()
    {
        deathCollection.SetActive(true);
    }

    public void winScreen()
    {
        winCollection.SetActive(true);
    }

    public void endGame()
    {
        SceneManager.LoadScene(0);
        Destroy(gameObject);
        Destroy(GameObject.FindGameObjectWithTag("HUD").gameObject);
    }

    public void setVolume()
    {
        AudioListener.volume = volumeSlider.value;
    }

    public void ExitGame()
    {
        Application.Quit();
    }

    public void updateHealthText(int health, int change, bool showText = true)
    {
        if (showText)
        {
            // make plus or minus pop up by resetting healthchangetext and setting new values
            if (change < 0)
            {
                healthChangeText.color = new Color(255, 0, 0, 255);
                healthChangeText.text = "" + change.ToString();
            }
            else
            {
                healthChangeText.color = new Color(0, 255, 0, 255);
                healthChangeText.text = "+" + change.ToString();
            }
            healthChangeText.transform.position = new Vector3(90, -20, 0);
            healthChangeText.enabled = true;
        }

        // update health total text
        healthText.text = "Health: " + health + "/100";
    }

    public void trigger(string name)
    {
        switch (name)
        {
            case "Trigger1":
                StartCoroutine(Trigger1()); // triggered wall from tutorial beginning
                break;
            case "Trigger2":
                instructionText.text = "DRINK THE POTION WITH 'E'"; // triggered wall to tutorial area
                break;
            case "Trigger3":
                StartCoroutine(Trigger3()); // completed drink potion tutorial in TutorialPotion.cs
                break;
            case "Trigger4":
                StartCoroutine(Trigger4()); // killed enemies
                break;
            case "Trigger5":
                StartCoroutine(Trigger5()); // completed tutorial, load scene '2'
                break;
            case "Trigger6":
                StartCoroutine(Trigger6()); // completed level, load scene '3'
                break;
        }
    }

    IEnumerator Trigger1()
    {
        instructionText.color = new Color(0, 255, 0, 255);
        yield return new WaitForSeconds(2);
        instructionText.text = "";
        instructionText.color = new Color(255, 255, 255, 255);
    }

    IEnumerator Trigger3()
    {
        instructionText.color = new Color(0, 255, 0, 255);
        yield return new WaitForSeconds(2);
        instructionText.text = "";
        instructionText.color = new Color(255, 255, 255, 255);
        yield return new WaitForSeconds(1);
        instructionText.text = "KILL THE ENEMIES WITH 'LCLICK' (SWORD) AND 'RCLICK' (SPELL)";
        foreach(GameObject o in meleeEnemiesCollection)
        {
            o.SetActive(true);
        }
        foreach (GameObject o in rangedEnemiesCollection)
        {
            o.SetActive(true);
        }
    }

    IEnumerator Trigger4()
    {
        instructionText.color = new Color(0, 255, 0, 255);
        yield return new WaitForSeconds(2);
        instructionText.text = "";
        instructionText.color = new Color(255, 255, 255, 255);
        yield return new WaitForSeconds(1);
        instructionText.text = "TUTORIAL COMPLETED! EXIT THROUGH THE DOOR";
    }

    IEnumerator Trigger5()
    {
        instructionText.text = "";
        float result = player.GetComponent<FaderScript>().BeginFade(1);
        yield return new WaitForSeconds(result);
        SceneManager.LoadScene(2);
    }

    IEnumerator Trigger6()
    {
        float result = player.GetComponent<FaderScript>().BeginFade(1);
        yield return new WaitForSeconds(result);
        SceneManager.LoadScene(3);
    }
}
