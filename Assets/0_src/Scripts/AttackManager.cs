using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

[System.Serializable]
public struct AttackProps{
    public GameObject spawnPoint;
    public float animationDelay;
    public string animationTriggerFwd;
    public string animationTriggerRev;
}

public class AttackManager : MonoBehaviour
{
    public float spawnRadius = 100.0f;
    public List<AttackProps> attackProps;
    public SpaceJunkManager sjm;
    public int maxObjects = 5;
    public int minObjects = 1;

    public int exampleAttack = 0;

    public GameObject targetArea;
    public Animator animator;

    List<int> attacksNotPerformed = new List<int>();

    private void Start(){
        //add all attacks to the list of attacks not performed
        for(int i = 0; i < attackProps.Count; i++){
            attacksNotPerformed.Add(i);
        }
    }

    public void PerformPsudoRandomAttack(){
        //check if there are any attacks left to perform
        if(attacksNotPerformed.Count == 0){
            //if not, reset the list of attacks
            for(int j = 0; j < attackProps.Count; j++){
                attacksNotPerformed.Add(j);
            }
        }
        //get a random attack from the list of attacks not performed
        int i = Random.Range(0, attacksNotPerformed.Count);
        //perform the attack
        PerformAttack(attacksNotPerformed[i]);
        //remove the attack from the list of attacks not performed
        attacksNotPerformed.RemoveAt(i);
    }

    public void PerformExampleAttack(){
        PerformAttack(exampleAttack);
    }

    public void PerformAttack(int i){
        //check if the attack is valid
        if(i < 0 || i >= attackProps.Count){
            Debug.LogError("Invalid attack index: " + i);
            return;
        }
        //get the attack properties
        AttackProps props = attackProps[i];

        // animate the attack
        animator.SetTrigger(props.animationTriggerFwd);

        // start the coroutine to wait for the animation to finish
        StartCoroutine(WaitAndPerformAttack(props));

    }

    // coroutine to wait for the animation to finish
    private IEnumerator WaitAndPerformAttack(AttackProps props){
        yield return new WaitForSeconds(props.animationDelay);
        Vector3 spawnPoint = props.spawnPoint.transform.position;
        Vector3 targetPoint = targetArea.transform.position;
        int numObjects = Random.Range(minObjects, maxObjects);
        sjm.GenerateSpaceJunk(spawnPoint, targetPoint, numObjects, spawnRadius);
        //get first child of props.spawnPoint
        Animator blindController = props.spawnPoint.transform.GetChild(0).GetComponent<Animator>();
        blindController.SetTrigger("TrBlind");
        yield return new WaitForSeconds(4);
        animator.SetTrigger(props.animationTriggerRev);

    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(AttackManager))]
public class AttackManagerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        // Draw the default inspector
        DrawDefaultInspector();

        // Reference to the AttackManager script
        AttackManager attackManager = (AttackManager)target;

        // Add a button to the inspector
        if (GUILayout.Button("Perform Example Attack"))
        {
            // Call PerformExampleAttack on button click
            attackManager.PerformExampleAttack();
        }
    }
}
#endif
