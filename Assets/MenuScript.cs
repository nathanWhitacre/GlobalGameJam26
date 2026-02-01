using UnityEngine;
using UnityEngine.SceneManagement;
public class MenuScript : MonoBehaviour
{
    public GameObject Title;
    public GameObject Image1;
    public GameObject Image2;
    public GameObject Button1;
    public GameObject Button2;
    public GameObject Button3;
    public GameObject Instructions;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void OnPlayButton ()
    {
        SceneManager.LoadScene(1);
    }

    public void OnQuitButton ()
    {
        Application.Quit();
    }

    public void OnInstructionButton()
    {
        Title.SetActive(false);
        Image1.SetActive(false);
        Image2.SetActive(false);
        Button1.SetActive(false);
        Button2.SetActive(false);
        Button3.SetActive(false);
        Instructions.SetActive(true);


    }
    public void OnInstructionBackButton()
    {

        Title.SetActive(true);
        Image1.SetActive(true);
        Image2.SetActive(true);
        Button1.SetActive(true);
        Button2.SetActive(true);
        Button3.SetActive(true);
        Instructions.SetActive(false);

    }
}
