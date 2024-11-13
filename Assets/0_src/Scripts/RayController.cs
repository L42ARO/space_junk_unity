using UnityEngine;

public class RayController : MonoBehaviour
{
    public GameObject player; // Assign the player object in the Inspector
    public GameObject hitIndicatorBig;
    public GameObject hitIndicatorSmall;
    public bool specialRay = false;
    public float speedMultiplier = 0.1f; // Controls how much additional speed to add based on distance
    private Rigidbody rb;
    private float maxDistance = 4000f;


    void Start()
    {
        rb = GetComponent<Rigidbody>(); // Get the Rigidbody component
        if (rb == null)
        {
            Debug.LogError("Rigidbody component missing from this GameObject!");
        }
    }

    void Update()
    {
        if (player == null || rb == null) return;

        float distance = Vector3.Distance(transform.position, player.transform.position);
        
        // Calculate additional velocity based on distance and add it to the current velocity
        Vector3 direction = (transform.position - player.transform.position).normalized;
        Vector3 additionalVelocity = direction * distance * speedMultiplier;

        // Add additional velocity to the existing velocity
        rb.velocity += additionalVelocity * Time.deltaTime;

        // Check if the distance to the player is greater than the maximum allowed distance
        if (distance > maxDistance)
        {
            Destroy(gameObject);
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        // Destroy this object when it collides with something
        if (collision.gameObject.CompareTag("SpaceJunk"))
        {
            GameManager.Instance.SpaceJunkPoints();
            Destroy(collision.gameObject);
        }
        else if (collision.gameObject.CompareTag("Titan"))
        {
            if (!specialRay)
            {
                GameManager.Instance.ReduceEnemyHealthRegular();
                //instantiate a hit indicator at the position
                //get the HitIndicatorController component
                GameObject hitIndicator = Instantiate(hitIndicatorBig, transform.position, Quaternion.identity);
                hitIndicator.SetActive(true);
                HitIndicatorController hitIndicatorController = hitIndicator.GetComponent<HitIndicatorController>();
                hitIndicatorController.IndicateBigHit();
            }
            else
            {
                GameManager.Instance.ReduceEnemyHealthSpecial();
                //instantiate a hit indicator at the position
                //get the HitIndicatorController component
                GameObject hitIndicator = Instantiate(hitIndicatorSmall, transform.position, Quaternion.identity);
                hitIndicator.SetActive(true);
                HitIndicatorController hitIndicatorController = hitIndicator.GetComponent<HitIndicatorController>();
                hitIndicatorController.IndicateHit();
            }
        }
        Destroy(gameObject);
    }
}
