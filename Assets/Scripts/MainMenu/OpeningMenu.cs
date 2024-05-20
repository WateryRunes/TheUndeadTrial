using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class OpeningMenu : MonoBehaviour
{
    private bool hardmode;
    public GameObject titleCollection;
    public GameObject optionsCollection;
    public Slider volumeSlider;

    // Start is called before the first frame update
    void Start()
    {
        hardmode = false;
        titleCollection.SetActive(true);
        optionsCollection.SetActive(false);
        //volumeSlider = GameObject.FindGameObjectWithTag("TESTING").GetComponent<Slider>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void StartGame()
    {
        PlayerPrefs.SetInt("hardmode", hardmode ? 1 : 0);
        PlayerPrefs.SetFloat("volume", volumeSlider.value);
        StartCoroutine(ChangeLevel());
    }

    public void toggleOptionsMenu()
    {
        optionsCollection.SetActive(!optionsCollection.activeInHierarchy);
        titleCollection.SetActive(!titleCollection.activeInHierarchy);
    }

    public void ExitGame()
    {
        Application.Quit();
    }

    public void toggleDifficulty()
    {
        hardmode = !hardmode;
    }

    IEnumerator ChangeLevel()
    {
        float result = GetComponent<FaderScript>().BeginFade(1);
        yield return new WaitForSeconds(result);
        SceneManager.LoadScene(1);
    }
}
