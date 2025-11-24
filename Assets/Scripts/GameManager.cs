using UnityEngine;
using UnityEngine.SceneManagement; // untuk scene management
using UnityEngine.UI; // untuk komponen UI seperti Slider, Toggle
using UnityEngine.Audio; // untuk audio mixer
using TMPro; // untuk TextMeshPro

public class GameManager : MonoBehaviour
{
    // referensi mixer audio
    [Header("Audio")]
    [SerializeField] private AudioMixer mainAudioMixer;
    private const string SoundEffectVolumeKey = "SFXVolume"; // kunci playerprefs untuk volume sfx
    private const string MusicVolumeKey = "MusicVolume"; // kunci playerprefs untuk volume musik
    
    // referensi UI Settings di scene Settings
    [Header("Settings UI References")]
    [SerializeField] private Slider sfxSlider;
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Toggle fullscreenToggle;
    [SerializeField] private Toggle keyboardToggle;
    [SerializeField] private GameObject keyboardGuidePanel;
    
    // referensi panel settings utama
    [Header("Settings Panel")]
    [SerializeField] private GameObject menuSettingsPanel; // panel menu settings (aktif default)

    // kunci playerprefs untuk fullscreen
    private const string FullscreenKey = "Fullscreen";
    private bool isSettingSaved = true; // flag untuk melacak apakah pengaturan sudah disimpan

    // singleton pattern untuk memastikan hanya ada satu instance dan persistensi
    public static GameManager Instance;

    private void Awake()
    {
        // membuat singleton instance
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        // memuat pengaturan saat game dimulai
        LoadSettings();
    }

    private void Start()
    {
        // menginisialisasi UI settings hanya jika di scene settings
        if (SceneManager.GetActiveScene().name == "Settings")
        {
            InitializeSettingsUI();
        }
    }

    private void OnEnable()
    {
        // berlangganan event scene loaded
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        // berhenti berlangganan event scene loaded
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    // dipanggil setiap kali scene dimuat
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // jika scene yang dimuat adalah settings, inisialisasi ui
        if (scene.name == "Settings")
        {
            InitializeSettingsUI();
        }
        // untuk scene lain (misal: Menu), pastikan flag reset
        else
        {
            isSettingSaved = true;
        }
    }

    // =========================================================================================================
    // scene management
    // =========================================================================================================

    // memuat scene baru berdasarkan nama
    public void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    // keluar dari aplikasi (hanya bekerja di build)
    public void ExitGame()
    {
        #if UNITY_EDITOR
            // di editor, menghentikan play mode
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            // di build, keluar dari aplikasi
            Application.Quit();
        #endif
    }

    // =========================================================================================================
    // pengaturan audio & visual
    // =========================================================================================================

    // memuat pengaturan yang tersimpan
    private void LoadSettings()
    {
        // memuat volume sfx
        float sfxVol = PlayerPrefs.GetFloat(SoundEffectVolumeKey, 0.75f); // default 0.75
        SetVolume(sfxVol, "SFX");
        
        // memuat volume musik
        float musicVol = PlayerPrefs.GetFloat(MusicVolumeKey, 0.75f); // default 0.75
        SetVolume(musicVol, "Music");
        
        // memuat status fullscreen
        int fullscreenState = PlayerPrefs.GetInt(FullscreenKey, 1); // default 1 (true)
        Screen.fullScreen = (fullscreenState == 1);
    }
    
    // inisialisasi komponen UI di scene Settings
    private void InitializeSettingsUI()
    {
        // mengambil referensi dari hierarki saat scene settings dimuat
        if (SceneManager.GetActiveScene().name != "Settings") return;

        // mencoba menemukan slider/toggle/panel berdasarkan struktur hierarki
        sfxSlider = GameObject.Find("Slider Sound Effect")?.GetComponent<Slider>();
        musicSlider = GameObject.Find("Slider Musik")?.GetComponent<Slider>();
        fullscreenToggle = GameObject.Find("Toggle Full Screen")?.GetComponent<Toggle>();
        keyboardToggle = GameObject.Find("Toggle Keyboard")?.GetComponent<Toggle>();
        keyboardGuidePanel = GameObject.Find("Panel Keyboard Guide");
        menuSettingsPanel = GameObject.Find("Panel Menu Settings");
        
        // pastikan referensi ditemukan sebelum digunakan
        if (sfxSlider != null)
        {
            // mengupdate nilai slider dari volume saat ini
            mainAudioMixer.GetFloat("SFX", out float volDB);
            sfxSlider.value = Mathf.Pow(10, volDB / 20); // konversi db ke linear
            // menambahkan listener untuk perubahan slider
            sfxSlider.onValueChanged.AddListener(SetSfxVolume);
        }

        if (musicSlider != null)
        {
            // mengupdate nilai slider dari volume saat ini
            mainAudioMixer.GetFloat("Music", out float volDB);
            musicSlider.value = Mathf.Pow(10, volDB / 20); // konversi db ke linear
            // menambahkan listener untuk perubahan slider
            musicSlider.onValueChanged.AddListener(SetMusicVolume);
        }

        if (fullscreenToggle != null)
        {
            // mengupdate toggle berdasarkan status fullscreen saat ini
            fullscreenToggle.isOn = Screen.fullScreen;
            // menambahkan listener untuk perubahan toggle
            fullscreenToggle.onValueChanged.AddListener(SetFullscreen);
        }
        
        // keyboard guide harus non-aktif secara default saat scene dimuat
        if (keyboardGuidePanel != null)
        {
            keyboardGuidePanel.SetActive(false);
        }
        
        // inisialisasi toggle keyboard: default off
        if (keyboardToggle != null)
        {
            keyboardToggle.isOn = false; // default off
            // menambahkan listener untuk perubahan toggle
            keyboardToggle.onValueChanged.AddListener(ToggleKeyboardPanel);
        }

        // mengatur flag bahwa pengaturan belum disimpan jika ini pertama kali di settings
        isSettingSaved = true; // reset
    }

    // mengkonversi linear volume ke desibel dan mengatur mixer
    private void SetVolume(float linearVolume, string mixerGroup)
    {
        // logaritma untuk konversi linear ke desibel
        float db = linearVolume > 0 ? 20f * Mathf.Log10(linearVolume) : -80f; 
        mainAudioMixer.SetFloat(mixerGroup, db);
    }

    // listener untuk slider sound effect
    public void SetSfxVolume(float linearVolume)
    {
        SetVolume(linearVolume, "SFX");
        isSettingSaved = false; // pengaturan diubah
    }

    // listener untuk slider musik
    public void SetMusicVolume(float linearVolume)
    {
        SetVolume(linearVolume, "Music");
        isSettingSaved = false; // pengaturan diubah
    }

    // listener untuk toggle fullscreen
    public void SetFullscreen(bool isFullscreen)
    {
        Screen.fullScreen = isFullscreen;
        isSettingSaved = false; // pengaturan diubah
    }

    // listener untuk toggle keyboard
    public void ToggleKeyboardPanel(bool isActive)
    {
        // mengaktifkan/menonaktifkan panel panduan keyboard
        if (keyboardGuidePanel != null)
        {
            keyboardGuidePanel.SetActive(isActive);
            // menonaktifkan panel settings utama saat panel keyboard aktif
            if (menuSettingsPanel != null)
            {
                menuSettingsPanel.SetActive(!isActive);
            }
        }
        
        // pastikan toggle kembali off setelah panel ditutup, ini hanya sebagai trigger
        if (!isActive && keyboardToggle != null)
        {
            keyboardToggle.isOn = false;
        }
    }
    
    // =========================================================================================================
    // penyimpanan pengaturan
    // =========================================================================================================

    // memuat scene settings
    public void LoadSettingsScene() 
    {
        // fungsi wrapper statis, yang memanggil fungsi dinamis dengan parameter yang sudah ditentukan
        LoadScene("Settings");
    }

    public void LoadPlayScene() 
    {
        // fungsi wrapper untuk tombol Play (ganti nama scene sesuai kebutuhan)
        LoadScene("NamaSceneGameBerikutnya"); 
    }

    // menyimpan pengaturan ke playerprefs
    public void SaveSettings()
    {
        if (sfxSlider != null)
        {
            // menyimpan volume sfx
            PlayerPrefs.SetFloat(SoundEffectVolumeKey, sfxSlider.value);
        }

        if (musicSlider != null)
        {
            // menyimpan volume musik
            PlayerPrefs.SetFloat(MusicVolumeKey, musicSlider.value);
        }

        if (fullscreenToggle != null)
        {
            // menyimpan status fullscreen (1 untuk true, 0 untuk false)
            PlayerPrefs.SetInt(FullscreenKey, fullscreenToggle.isOn ? 1 : 0);
        }

        // memastikan data tersimpan ke disk
        PlayerPrefs.Save();
        isSettingSaved = true; // pengaturan sudah disimpan
        
        // kembali ke scene menu setelah save
        LoadScene("Menu");
    }

    // kembali ke scene menu tanpa menyimpan
    public void BackWithoutSave()
    {
        // jika pengaturan belum disimpan, muat ulang yang tersimpan
        if (!isSettingSaved)
        {
            LoadSettings(); // memuat kembali pengaturan yang tersimpan
        }
        
        // kembali ke scene menu
        LoadScene("Menu");
    }
}