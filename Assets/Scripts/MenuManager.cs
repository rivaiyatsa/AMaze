using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    // Memuat scene Settings
    public void LoadSettingsScene()
    {
        SceneManager.LoadScene("Settings");
    }

    // Keluar dari aplikasi
    public void ExitGame()
    {
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }
}
