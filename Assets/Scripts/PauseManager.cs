using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseManager : MonoBehaviour
{
    public static PauseManager instance;

    [Header("UI GameObjects")]
    [SerializeField] private GameObject pausePanel;         // Pause UI පැනලය
    [SerializeField] private Image gameStatusIcon;          // තිරයේ ඉහළ ඇති Image Icon එක

    [Header("Icon Sprites")]
    [SerializeField] private Sprite playSprite;             // Play සලකුණේ රූපය
    [SerializeField] private Sprite pauseSprite;            // Pause සලකුණේ රූපය

    [Header("Audio Component")]
    [SerializeField] private Slider volumeSlider;           // Volume Slider එක

    private bool isPaused = false;

    private void Awake()
    {
        // Singleton Pattern එක මඟින් ස්ක්‍රිප්ට් එක තහවුරු කිරීම
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        // Slider එක දැනට ක්‍රීඩාවේ පවතින Audio Volume අගයට සැකසීම
        if (volumeSlider != null)
        {
            volumeSlider.value = AudioListener.volume;
            volumeSlider.onValueChanged.AddListener(SetVolume);
        }

        // ක්‍රීඩාව ආරම්භයේදී පැනලය වසා, ඉහළ Icon එකට Play Sprite එක ලබාදේ
        if (pausePanel != null) pausePanel.SetActive(false);
        if (gameStatusIcon != null && playSprite != null) gameStatusIcon.sprite = playSprite;
    }

    private void Update()
    {
        // Keyboard එකෙන් "P" යතුර එබූ විට ක්‍රීඩාව Pause හෝ Resume වේ
        if (Input.GetKeyDown(KeyCode.P))
        {
            TogglePause();
        }
    }

    public void TogglePause()
    {
        isPaused = !isPaused;

        if (isPaused)
            PauseGame();
        else
            ResumeGame();
    }

    private void PauseGame()
    {
        if (pausePanel != null) pausePanel.SetActive(true);

        // ඉහළ ඇති Icon එකෙහි Sprite එක Pause සලකුණට මාරු කිරීම
        if (gameStatusIcon != null && pauseSprite != null) gameStatusIcon.sprite = pauseSprite;

        Time.timeScale = 0f; // ක්‍රීඩාවේ කාලය නතර කරයි (Freeze කරයි)

        // 'Maruverayo' ක්‍රීඩාවේ සඟවා ඇති මූසිකය (Cursor) බොත්තම් එබීමට නැවත පෙන්වීම
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void ResumeGame()
    {
        isPaused = false;
        if (pausePanel != null) pausePanel.SetActive(false);

        // ඉහළ ඇති Icon එකෙහි Sprite එක නැවත Play සලකුණට මාරු කිරීම
        if (gameStatusIcon != null && playSprite != null) gameStatusIcon.sprite = playSprite;

        Time.timeScale = 1f; // ක්‍රීඩාව නැවත සාමාන්‍ย පරිදි ආරම්භ කරයි

        // කැමරා පාලනය (CameraController) සඳහා Cursor එක නැවත සඟවයි
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void RestartLevel()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void SetVolume(float volume)
    {
        AudioListener.volume = volume;
    }

    public void QuitGame()
    {
        Debug.Log("Game Quitting...");
        Application.Quit();
    }
}