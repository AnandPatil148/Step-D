using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;


public class LevelManager : MonoBehaviour
{
    public void LoadMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }

    public void LoadMultiplayer()
    {
        SceneManager.LoadScene("MultiplayerMenu");
    }
    
    public void LoadMP7()
    {
        PhotonNetwork.LoadLevel(7);
    }

    public void LoadNextLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void LoadLevel1()
    {
        SceneManager.LoadScene("Level01");
    }

    public void LoadLevel2()
    {
        SceneManager.LoadScene("Level02");
    }

    public void LoadLevel3()
    {
        SceneManager.LoadScene("Level03");
    }

    public void Quit() 
    {
        Application.Quit();
    }
}
