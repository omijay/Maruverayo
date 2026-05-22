using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [Header("UI Panels")]
    [SerializeField] private GameObject winPanel;
    [SerializeField] private GameObject losePanel;

    [Header("Delay Settings")]
    [Tooltip("ජයග්‍රහණය කළ පසු පැනලය දර්ශනය වීමට ගතවන කාලය (තත්පර)")]
    [SerializeField] private float winDelay = 2.5f;
    [Tooltip("පරාජය වූ පසු පැනලය දර්ශනය වීමට ගතවන කාලය (තත්පර)")]
    [SerializeField] private float loseDelay = 3.0f;

    [Header("Fighter References")]
    private Angam playerFighter;
    private bool isGameOver = false;

    private void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        // map2PlayerController හරහා Player ගේ Angam ස්ක්‍රිප්ට් එක සොයා ගැනීම
        if (map2PlayerController.i != null)
        {
            playerFighter = map2PlayerController.i.GetComponent<Angam>();
        }

        // මට්ටමේ සිටින මුළු සතුරන් සංඛ්‍යාව ගණනය කිරීම
        EnemyController[] allEnemies = FindObjectsOfType<EnemyController>();

        // සතුරන් මියයන විට එය හඳුනා ගැනීමට සතුරන්ගේ ස්ක්‍රිප්ට් වෙත සවන්දීම (Subscribe)
        foreach (var enemy in allEnemies)
        {
            Angam enemyAngam = enemy.GetComponent<Angam>();
            if (enemyAngam != null)
            {
                enemyAngam.OnGotHit += (attacker) => {
                    CheckEnemyStatus(enemyAngam);
                };
            }
        }
    }

    private void Update()
    {
        // ක්‍රීඩාව දැනටමත් අවසන් වී ඇත්නම් නැවත පරීක්ෂා නොකරයි
        if (isGameOver) return;

        // Player ගේ සෞඛ්‍යය (Health) 0 වුවහොත් ක්‍රීඩාව පරාජය කිරීම
        if (playerFighter != null && playerFighter.Health <= 0)
        {
            isGameOver = true;
            StartCoroutine(WaitAndShowLosePanel());
        }
    }

    // සතුරන් මියගොස් ඇත්දැයි පරීක්ෂා කිරීම
    private void CheckEnemyStatus(Angam enemyFighter)
    {
        if (enemyFighter.Health <= 0)
        {
            // සතුරා මියගිය පසු ඉතිරි සතුරන් ගණන බැලීමට සුළු වෙලාවකින් පසුව පරීක්ෂා කරයි
            Invoke("VerifyRemainingEnemies", 0.2f);
        }
    }

    private void VerifyRemainingEnemies()
    {
        if (isGameOver) return;

        EnemyController[] remainingEnemies = FindObjectsOfType<EnemyController>();
        int activeEnemies = 0;

        foreach (var enemy in remainingEnemies)
        {
            // සජීවීව සටනේ සිටින සතුරන් පමණක් ගණනය කිරීම
            if (enemy.IsInState(EnemyStates.CombatMovement) || enemy.IsInState(EnemyStates.Idle) || enemy.IsInState(EnemyStates.Attack))
            {
                activeEnemies++;
            }
        }

        // සියලුම සතුරන් විනාශ වී ඇත්නම් ජයග්‍රහණය ලබා දීම
        if (activeEnemies == 0)
        {
            isGameOver = true;
            StartCoroutine(WaitAndShowWinPanel());
        }
    }

    // සුළු වේලාවකට පසු Win Panel එක පෙන්වන Coroutine එක
    private IEnumerator WaitAndShowWinPanel()
    {
        // Time.timeScale = 0 කිරීමට පෙර නියමිත තත්පර ගණන සාමාන්‍ය පරිදි ධාවනය වීමට ඉඩ දෙයි
        yield return new WaitForSeconds(winDelay);

        winPanel.SetActive(true);
        Time.timeScale = 0f; // ක්‍රීඩාව නැවතීමට (Pause)
        Cursor.lockState = CursorLockMode.None; // මූසිකය (Cursor) දර්ශනය කිරීමට
        Cursor.visible = true;
    }

    // සුළු වේලාවකට පසු Lose Panel එක පෙන්වන Coroutine එක
    private IEnumerator WaitAndShowLosePanel()
    {
        yield return new WaitForSeconds(loseDelay);

        losePanel.SetActive(true);
        Time.timeScale = 0f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    // BUTTON FUNCTIONS (බොත්තම් සඳහා ක්‍රියාකාරකම්)

    // ඊළඟ මට්ටමට යාමට (Next Level)
    public void LoadNextLevel(string nextLevelName)
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(nextLevelName);
    }

    // වත්මන් මට්ටම නැවත ආරම්භ කිරීමට (Reload Level)
    public void ReloadLevel()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    // ප්‍රධාන සිතියමට/මෙනුවට යාමට (Load Main Menu / Main Map)
    public void LoadMainMap(string mainMapName)
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(mainMapName);
    }
}