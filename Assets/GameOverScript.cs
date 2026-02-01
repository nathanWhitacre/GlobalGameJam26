using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverScript : MonoBehaviour
{
    public AudioSource UIsoundagain;
    public void OnPlayButton()
    {
        SceneManager.LoadScene(1);
    }
    public void OnQuitButton()
    {
        SceneManager.LoadScene(0);
    }
    
    public void PlayTheSound()
    {
        UIsoundagain.Play();
    }
    
}
