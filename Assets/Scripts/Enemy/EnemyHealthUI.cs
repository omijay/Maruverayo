using UnityEngine;
using UnityEngine.UI;

public class EnemyHealthUI : MonoBehaviour
{
    [SerializeField] GameObject enemyCanvas;
    [SerializeField] Slider enemySlider;
    [SerializeField] Angam enemyFighter;
    [SerializeField] float visibilityDistance = 5f; // සෞඛ්‍ය තීරුව පෙනෙන දුර ප්‍රමාණය

    private Transform playerTransform;
    private Transform camTransform;

    private void Start()
    {
        if (enemyFighter == null)
            enemyFighter = GetComponentInParent<Angam>();

        enemySlider.maxValue = enemyFighter.Health;
        enemySlider.value = enemyFighter.Health;

        if (Camera.main != null)
            camTransform = Camera.main.transform;

        if (map2PlayerController.i != null)
            playerTransform = map2PlayerController.i.transform;
    }

    private void Update()
    {
        if (enemyFighter == null || enemyFighter.Health <= 0)
        {
            enemyCanvas.SetActive(false);
            return;
        }

        // වත්මන් සෞඛ්‍යය යාවත්කාලීන කිරීම
        enemySlider.value = enemyFighter.Health;

        // ක්‍රීඩකයා සහ සතුරා අතර දුර ගණනය කිරීම
        if (playerTransform != null)
        {
            float distance = Vector3.Distance(transform.position, playerTransform.position);

            // ක්‍රීඩකයා සතුරා අසලට පැමිණි විට පමණක් පෙන්වීම
            if (distance <= visibilityDistance)
            {
                enemyCanvas.SetActive(true);
            }
            else
            {
                enemyCanvas.SetActive(false);
            }
        }

        // UI එක හැමවිටම කැමරාව දෙසට හරවා තැබීම (Look at Camera)
        if (camTransform != null && enemyCanvas.activeSelf)
        {
            enemyCanvas.transform.LookAt(enemyCanvas.transform.position + camTransform.forward);
        }
    }
}