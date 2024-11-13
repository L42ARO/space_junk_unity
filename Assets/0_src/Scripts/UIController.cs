using System.Collections;
using UnityEngine;
using UnityEditor;
using TMPro;

public class UIController : MonoBehaviour
{
    public GameObject StartButton;
    public GameObject TitleCard;
    public GameObject StatsGroup;
    public GameObject hudObject;
    public GameObject resetButton;
    public Renderer hudRenderer;
    public Color emissionColor = Color.red; // Color of the emission
    public float maxEmissionIntensity = 2.0f; // Max intensity for the emission
    public float transitionDuration = 1.0f; // Duration for each half of the transition

    public TextMeshProUGUI healthText;
    public TextMeshProUGUI titanHealth;
    public TextMeshProUGUI resultText;
    public GameObject resultObject;
    public TextMeshProUGUI pointsText;

    private Coroutine showPainCoroutine;

    // Start is called before the first frame update
    void Start()
    {
        if (hudObject != null)
        {
            hudObject.SetActive(false); // Ensure HUD starts off
        }
        //set all text to full health
        ChangeHealth(1.0f);
        ChangeTitanhealth(1.0f);
        ChangePoints(0);
    }

    // ShowPain function to activate HUD and perform pulsing effect
    public void ShowPain()
    {
        if (showPainCoroutine != null)
        {
            StopCoroutine(showPainCoroutine);
        }
        showPainCoroutine = StartCoroutine(ShowPainEffect());
    }

    private IEnumerator ShowPainEffect()
    {
        hudObject.SetActive(true); // Activate HUD

        // Fade in
        yield return ChangeEmissionIntensity(0, maxEmissionIntensity, transitionDuration);

        // Fade out
        yield return ChangeEmissionIntensity(maxEmissionIntensity, -10, transitionDuration);

        hudObject.SetActive(false); // Deactivate HUD
    }

    public void ShowPainPermanent()
    {
        StartCoroutine(ShowPainPermanentCoroutine());
    }
    private IEnumerator ShowPainPermanentCoroutine(){
        yield return new WaitForSeconds(4);
        hudObject.SetActive(true);
        yield return ChangeEmissionIntensity(0, maxEmissionIntensity, transitionDuration);
    }

    private IEnumerator ChangeEmissionIntensity(float startIntensity, float endIntensity, float duration)
    {
        float elapsedTime = 0;

        while (elapsedTime < duration)
        {
            float currentIntensity = Mathf.Lerp(startIntensity, endIntensity, elapsedTime / duration);
            SetEmissionIntensity(currentIntensity);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        SetEmissionIntensity(endIntensity);
    }

    private void SetEmissionIntensity(float intensity)
    {
        if (hudRenderer != null)
        {
            Color finalColor = emissionColor * Mathf.LinearToGammaSpace(intensity);
            hudRenderer.material.SetColor("_EmissionColor", finalColor);
            hudRenderer.material.EnableKeyword("_EMISSION");
        }
    }

    public void ChangePoints(int points)
    {
        pointsText.text = points.ToString();
    }
    public void ChangeHealth(float health)
    {
        // max health is : ||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||

        // health is between 0 and 1, render the bars

        int totalBars = 62;

        int healthBars = (int)(health * totalBars);
        string healthBarString = new string('|', healthBars);
        if (healthBars <= 0)
        {
            healthBarString = "---";
        }

        healthText.text = healthBarString;
        
    }
    public void ChangeTitanhealth(float health)
    {
        int totalBars = 62;
        int healthBars = (int)(health * totalBars);
        string healthBarString = new string('|', healthBars);
        if (healthBars <= 0)
        {
            healthBarString = "---";
        }
        titanHealth.text = "<color=\"yellow\">"+healthBarString + "</color>";
    }
    public void DisplayWin(){
        resultObject.SetActive(true);
        resultText.text = "YOU WIN";
        resetButton.SetActive(true);
    }
    public void DisplayLose(){
        resultObject.SetActive(true);
        resultText.text = "YOU LOST";
        resetButton.SetActive(true);
    }
    public void ChangeToGameUI(){
        resetButton.SetActive(false);
        StatsGroup.SetActive(true);
        StartButton.SetActive(false);
        TitleCard.SetActive(false);

    }
    public void ChangeToHomeUI(){
        StatsGroup.SetActive(false);
        StartButton.SetActive(true);
        TitleCard.SetActive(true);
        resetButton.SetActive(false);
    }
    public void ResetGame(){
        GameManager.Instance.ResetGame();
    }
}


#if UNITY_EDITOR
[CustomEditor(typeof(UIController))]
public class UIControllerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        UIController uiController = (UIController)target;
        if (GUILayout.Button("Test ShowPain"))
        {
            uiController.ShowPain();
        }
    }
}
#endif
