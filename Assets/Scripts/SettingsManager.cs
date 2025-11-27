using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SettingsManager : MonoBehaviour
{
    [Header("Fullscreen Settings")]
    public Toggle fullscreenToggle;
    
    [Header("Windowed Mode Resolution")]
    public int windowedWidth = 1280;
    public int windowedHeight = 720;

    [Header("Keyboard Settings")]
    public Toggle keyboardToggle;
    public GameObject keyboardPanel;

    private bool savedFullscreenValue;
    private bool savedKeyboardValue;

    private void Start()
    {
        LoadSettings();
        
        // Set toggle sesuai dengan nilai yang tersimpan
        if (fullscreenToggle != null)
        {
            fullscreenToggle.isOn = savedFullscreenValue;
            fullscreenToggle.onValueChanged.AddListener(OnFullscreenToggleChanged);
        }

        // Set keyboard toggle dan panel
        if (keyboardToggle != null)
        {
            keyboardToggle.isOn = savedKeyboardValue;
            keyboardToggle.onValueChanged.AddListener(OnKeyboardToggleChanged);
        }

        // Set keyboard panel visibility sesuai dengan toggle
        if (keyboardPanel != null)
        {
            keyboardPanel.SetActive(savedKeyboardValue);
        }
    }

    // Load pengaturan yang tersimpan
    private void LoadSettings()
    {
        // Default fullscreen adalah ON (1), jika belum ada yang tersimpan
        savedFullscreenValue = PlayerPrefs.GetInt("Fullscreen", 1) == 1;
        
        // Default keyboard panel adalah OFF (0)
        savedKeyboardValue = PlayerPrefs.GetInt("KeyboardPanel", 0) == 1;
        
        // Apply pengaturan yang tersimpan
        ApplyFullscreenSetting(savedFullscreenValue);
    }

    // Dipanggil saat toggle fullscreen berubah (preview saja, belum tersimpan)
    // <param name="isFullscreen">True untuk fullscreen, False untuk windowed</param>
    private void OnFullscreenToggleChanged(bool isFullscreen)
    {
        ApplyFullscreenSetting(isFullscreen);
    }

    // Dipanggil saat toggle keyboard berubah
    // <param name="showKeyboard">True untuk menampilkan panel, False untuk menyembunyikan</param>
    private void OnKeyboardToggleChanged(bool showKeyboard)
    {
        if (keyboardPanel != null)
        {
            keyboardPanel.SetActive(showKeyboard);
        }
    }

    // Menutup panel keyboard dan set toggle OFF
    public void CloseKeyboardPanel()
    {
        // Set toggle menjadi OFF
        if (keyboardToggle != null)
        {
            keyboardToggle.isOn = false;
        }
        
        // Sembunyikan panel
        if (keyboardPanel != null)
        {
            keyboardPanel.SetActive(false);
        }
    }

    // Mengaplikasikan pengaturan fullscreen
    // <param name="isFullscreen">True untuk fullscreen, False untuk windowed</param>
    private void ApplyFullscreenSetting(bool isFullscreen)
    {
        if (isFullscreen)
        {
            // Set ke fullscreen dengan resolusi native monitor
            Screen.SetResolution(Screen.currentResolution.width, Screen.currentResolution.height, FullScreenMode.FullScreenWindow);
        }
        else
        {
            // Set ke windowed mode dengan ukuran yang ditentukan
            Screen.SetResolution(windowedWidth, windowedHeight, FullScreenMode.Windowed);
        }
        
        Debug.Log($"Fullscreen changed to: {isFullscreen}, Resolution: {Screen.width}x{Screen.height}");
    }

    // Simpan pengaturan secara permanen dan kembali ke Menu
    public void SaveSettings()
    {
        // Simpan fullscreen setting
        if (fullscreenToggle != null)
        {
            PlayerPrefs.SetInt("Fullscreen", fullscreenToggle.isOn ? 1 : 0);
            savedFullscreenValue = fullscreenToggle.isOn;
        }

        // Simpan keyboard panel setting
        if (keyboardToggle != null)
        {
            PlayerPrefs.SetInt("KeyboardPanel", keyboardToggle.isOn ? 1 : 0);
            savedKeyboardValue = keyboardToggle.isOn;
        }

        PlayerPrefs.Save();
        
        Debug.Log("Settings saved! Fullscreen: " + savedFullscreenValue + ", Keyboard Panel: " + savedKeyboardValue);
        
        // Kembali ke Menu setelah save
        SceneManager.LoadScene("Menu");
    }

    /// Kembali ke scene Menu tanpa menyimpan perubahan
    public void BackToMenu()
    {
        // Kembalikan ke pengaturan yang tersimpan sebelumnya
        ApplyFullscreenSetting(savedFullscreenValue);
        
        if (keyboardPanel != null)
        {
            keyboardPanel.SetActive(savedKeyboardValue);
        }
        
        SceneManager.LoadScene("Menu");
    }
}
