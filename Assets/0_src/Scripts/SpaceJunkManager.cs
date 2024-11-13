using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[System.Serializable]
public struct SpaceJunkData
{
    public GameObject spaceJunkObject;
    public Vector3 finalDestination;
    public float velocity;
    public bool finalForceApplied;
    public bool secondForceApplied;
}

public class SpaceJunkManager : MonoBehaviour
{
    public List<GameObject> spaceJunkTemplates;
    public float speedTowardsDestination = 50.0f;
    public float closeDistance = 50.0f;
    public Vector2 forceMagnitudeRange = new Vector2(1000.0f, 2000.0f);
    public float DisappearanceLine = -200;

    // Example values for custom editor
    public GameObject exampleStart;
    public GameObject exampleEnd;
    public int exampleQuantity = 1;
    public float exampleRadius = 10.0f;

    private List<SpaceJunkData> spaceJunkList = new List<SpaceJunkData>();
    private List<GameObject> availableTemplates = new List<GameObject>();

    void FixedUpdate()
    {
        for (int i = spaceJunkList.Count - 1; i >= 0; i--)
        {
            SpaceJunkData junkData = spaceJunkList[i];
            GameObject spaceJunk = junkData.spaceJunkObject;

            if (spaceJunk == null)
            {
                spaceJunkList.RemoveAt(i);
                continue;
            }

            if (spaceJunk.transform.position.z < DisappearanceLine)
            {
                Destroy(spaceJunk);
                spaceJunkList.RemoveAt(i);
                continue;
            }

            Vector3 directionToDestination = junkData.finalDestination - spaceJunk.transform.position;
            float distanceToDestination = directionToDestination.magnitude;
            Rigidbody rb = spaceJunk.GetComponent<Rigidbody>();

            if (distanceToDestination > closeDistance)
            {
                rb.velocity = directionToDestination.normalized * junkData.velocity;
            }
            else if (!junkData.finalForceApplied)
            {
                rb.useGravity = true;
                Vector3 finalApproachDirection = directionToDestination.normalized + new Vector3(
                    Random.Range(-0.01f, 0.01f),
                    Random.Range(-0.01f, 0.01f),
                    Random.Range(-0.01f, 0.01f)
                );

                finalApproachDirection.y *= 0.7f;

                float finalForce = Random.Range(forceMagnitudeRange.x, forceMagnitudeRange.y);
                rb.AddForce(finalApproachDirection * finalForce, ForceMode.Impulse);
                rb.drag = Random.Range(0.1f, 0.3f);

                SpaceJunkData newJunkData = new SpaceJunkData
                {
                    spaceJunkObject = junkData.spaceJunkObject,
                    finalDestination = junkData.finalDestination,
                    finalForceApplied = true
                };
                spaceJunkList[i] = newJunkData;
            }
            else if (rb.velocity.magnitude < 40 && !junkData.secondForceApplied)
            {
                Vector3 finalApproachDirection = directionToDestination.normalized;
                float finalForce = 5000;
                rb.AddForce(finalApproachDirection * finalForce, ForceMode.Impulse);

                SpaceJunkData newJunkData = new SpaceJunkData
                {
                    spaceJunkObject = junkData.spaceJunkObject,
                    finalDestination = junkData.finalDestination,
                    finalForceApplied = true,
                    secondForceApplied = true
                };
                spaceJunkList[i] = newJunkData;
            }
        }
    }

    public void GenerateSpaceJunk()
    {
        GenerateSpaceJunk(exampleStart.transform.position, exampleEnd.transform.position, exampleQuantity, exampleRadius);
    }

    public void GenerateSpaceJunk(Vector3 startPosition, Vector3 endPosition, int numberOfObjects, float radius)
    {
        for (int i = 0; i < numberOfObjects; i++)
        {
            // Ensure all templates are used before repeating
            if (availableTemplates.Count == 0)
            {
                availableTemplates.AddRange(spaceJunkTemplates);
            }

            // Pick a template and remove it from the available list
            int templateIndex = Random.Range(0, availableTemplates.Count);
            GameObject template = availableTemplates[templateIndex];
            availableTemplates.RemoveAt(templateIndex);

            // Generate a random position within the radius from the start position
            Vector3 randomOffset = Random.insideUnitSphere * radius;
            Vector3 spawnPosition = startPosition + randomOffset;

            // Instantiate the space junk with a random rotation
            Quaternion randomRotation = Random.rotation;
            GameObject newSpaceJunk = Instantiate(template, spawnPosition, randomRotation);
            newSpaceJunk.SetActive(true);

            Rigidbody rb = newSpaceJunk.GetComponent<Rigidbody>();
            rb.useGravity = false;

            // Apply initial rolling force
            Vector3 rollingForce = new Vector3(
                Random.Range(-5f, 5f),
                Random.Range(-5f, 5f),
                Random.Range(-5f, 5f)
            );
            rb.AddTorque(rollingForce, ForceMode.Impulse);

            Vector3 directionToDestination = endPosition - spawnPosition;
            rb.velocity = directionToDestination.normalized * speedTowardsDestination;

            SpaceJunkData junkData = new SpaceJunkData
            {
                spaceJunkObject = newSpaceJunk,
                finalDestination = endPosition,
                velocity = speedTowardsDestination * Random.Range(0.5f, 1.5f)
            };

            spaceJunkList.Add(junkData);
        }
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(SpaceJunkManager))]
public class SpaceJunkManagerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        SpaceJunkManager manager = (SpaceJunkManager)target;

        if (GUILayout.Button("Generate Space Junk"))
        {
            if (manager.exampleStart != null && manager.exampleEnd != null)
            {
                manager.GenerateSpaceJunk();
            }
            else
            {
                Debug.LogWarning("Please assign both exampleStart and exampleEnd GameObjects in the inspector.");
            }
        }
    }
}
#endif
