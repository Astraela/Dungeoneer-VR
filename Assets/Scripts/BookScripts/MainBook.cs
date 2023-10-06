using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

public class MainBook : MonoBehaviour
{
    public GameObject gameMenu;
    public GameObject settings;
    public GameObject controls;
    public bool interacted = false;
    public bool goingon = false;
    private GameObject currentOpen;

    private void Start()
    {
        gameMenu.SetActive(false);
        settings.SetActive(false);
        controls.SetActive(true);
        currentOpen = controls;
    }

    public void NewGameEvent() {
        interacted = true;
        string filePath = Application.dataPath + "\\data.txt";
        File.Delete(filePath);
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void LoadGameEvent()
    {
        interacted = true;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void SaveGameEvent()
    {
        interacted = true;
        GameObject.Find("Camera").GetComponent<DataHandler>().saveData();
    }

    public void QuitEvent()
    {
        interacted = true;
        print("Pressed Quit");
    }

    public void PlusEvent()
    {
        interacted = true;
        var audio = GameObject.Find("Camera").GetComponent<AudioScript>();
        audio.volume = audio.volume + 0.1f;
    }

    public void MinusEvent()
    {
        interacted = true;
        var audio = GameObject.Find("Camera").GetComponent<AudioScript>();
        audio.volume = audio.volume - 0.1f;
    }

    public void ChangePage(GameObject newPage) {
        if (newPage != currentOpen)
        {
            interacted = true;
            currentOpen.SetActive(false);
            currentOpen = newPage;
            currentOpen.SetActive(true);
        }
    }

    public IEnumerator BookDelete()
    {
        if (!goingon)
        {
            goingon = true;
            while (true)
            {
                yield return new WaitForSeconds(5);

                if (interacted) 
                    interacted = false;
                else
                    break;
            }
            Destroy(gameObject);
        }
    }
}
