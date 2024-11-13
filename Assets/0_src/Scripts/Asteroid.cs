using UnityEngine;

public class AsteroidVisibility : MonoBehaviour
{
    public float visibilityThreshold = 5f; // Minimum speed to show the trail
    public float fadeThreshold = 10f;      // Speed above which the trail is fully opaque
    private ParticleSystem asteroidTrail;
    private Rigidbody rb;
    private ParticleSystem.MainModule mainModule;
    public bool debug;

    void Start()
    {
        // Get Rigidbody component
        rb = GetComponent<Rigidbody>();

        // Get ParticleSystem component from the child object
        asteroidTrail = GetComponentInChildren<ParticleSystem>();
        if (asteroidTrail != null)
        {
            mainModule = asteroidTrail.main;
        }
        else
        {
            Debug.LogError("No Particle System found in child object.");
        }
    }

    void Update()
    {
        // Calculate current speed
        float speed = rb.velocity.magnitude;
        if(debug) Debug.Log("L42: Speed: " + speed);

        // Adjust particle trail transparency or deactivate based on speed
        if (speed < visibilityThreshold)
        {
            asteroidTrail.gameObject.SetActive(false); // Deactivate if below threshold
        }
        else
        {
            if (!asteroidTrail.gameObject.activeSelf)
            {
                asteroidTrail.gameObject.SetActive(true); // Reactivate if speed goes back up
            }

            // Linearly interpolate transparency based on speed between visibilityThreshold and fadeThreshold
            float alpha = Mathf.InverseLerp(visibilityThreshold, fadeThreshold, speed);
            SetTrailTransparency(alpha);

            // Rotate the particle system to face the velocity direction
            RotateTrailToFaceVelocity();
        }
    }

    private void SetTrailTransparency(float alpha)
    {
        // Check if asteroidTrail is valid before changing the transparency
        if (asteroidTrail != null)
        {
            Color startColor = mainModule.startColor.color;
            startColor.a = alpha;
            mainModule.startColor = new ParticleSystem.MinMaxGradient(startColor);
        }
    }

    private void RotateTrailToFaceVelocity()
    {
        if (rb.velocity != Vector3.zero)
        {
            //align the z axis of the trail with the velocity vector * -1
            asteroidTrail.transform.rotation = Quaternion.LookRotation(-rb.velocity);
        }
    }
}
