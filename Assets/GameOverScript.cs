using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverScript : MonoBehaviour
{
    public AudioSource UIsound;
    public void OnPlayButton()
    {
        SceneManager.LoadScene(1);
    }
    public void OnQuitButton()
    {
        Application.Quit();
    }
    /*
    public void PlaySound()
    {
        UIsound.Play();
    }
    */
}
