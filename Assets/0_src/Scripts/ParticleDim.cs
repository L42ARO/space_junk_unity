using UnityEngine;

public class HeightBasedTransparency : MonoBehaviour
{
    [SerializeField] private ParticleSystem particleSystem;
    [SerializeField] private float targetHeight = 5.0f; // The global y-axis value to approach.
    [SerializeField] private float maxDistance = 10.0f; // The distance at which the transparency is full.

    private ParticleSystem.MainModule particleMain;

    private void Start()
    {
        if (particleSystem == null)
        {
            Debug.LogError("Particle system is not assigned.");
            return;
        }

        particleMain = particleSystem.main;
    }

    private void Update()
    {
        float currentY = transform.position.y; // Ensure this uses the global position.
        float distance = Mathf.Abs(currentY - targetHeight);

        // Calculate the alpha value such that it decreases as the object gets closer to the target height.
        float alpha = Mathf.Clamp01(distance / maxDistance); // Lower alpha when closer, higher when further.

        // Update the start color of the particle system with the calculated alpha.
        Color currentColor = particleMain.startColor.color;
        currentColor.a = alpha;
        particleMain.startColor = new ParticleSystem.MinMaxGradient(currentColor);
    }
}
