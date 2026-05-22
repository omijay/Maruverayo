using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealthUI : MonoBehaviour
{
    [SerializeField] GameObject playerCanvas;
    [SerializeField] Slider playerSlider;
    [SerializeField] Angam playerFighter;
    [SerializeField] float delayTime = 3f; // ආරම්භ වීමට ගතවන කාලය (තත්පර)

    private void Start()
    {
        if (playerFighter == null)
            playerFighter = map2PlayerController.i.GetComponent<Angam>();

        // ක්‍රීඩකයාගේ උපරිම සෞඛ්‍ය අගය Slider එකට ලබා දීම
        playerSlider.maxValue = playerFighter.Health;
        playerSlider.value = playerFighter.Health;

        // සුළු වේලාවකින් පසු දර්ශනය වීමේ Coroutine එක ආරම්භ කිරීම
        StartCoroutine(ShowHealthBarAfterDelay());
    }

    private void Update()
    {
        if (playerFighter != null)
        {
            // නිරන්තරයෙන් ක්‍රීඩකයාගේ සෞඛ්‍යය යාවත්කාලීන කිරීම
            playerSlider.value = playerFighter.Health;
        }
    }

    IEnumerator ShowHealthBarAfterDelay()
    {
        yield return new WaitForSeconds(delayTime);
        if (playerCanvas != null)
        {
            playerCanvas.SetActive(true); // Canvas එක සක්‍රිය කිරීම
        }
    }
}