using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public bool ifSkipTutorial = true;
    
    public void OnStartClicked()
    {
#if UNITY_EDITOR
        if (ifSkipTutorial)
        {
            SceneManager.LoadScene("Cutscenes");
            return;
        }
#endif
        SceneManager.LoadScene("Tutorial Scene");
    }
}
