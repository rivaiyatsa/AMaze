using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    // memuat scene mapping saat tombol play ditekan
    public void PlayGame()
    {
        SceneManager.LoadScene("Mapping");
    }

    // memuat scene settings
    public void LoadSettingsScene()
    {
        SceneManager.LoadScene("Settings");
    }

    // keluar dari aplikasi
    public void ExitGame()
    {
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }
}
