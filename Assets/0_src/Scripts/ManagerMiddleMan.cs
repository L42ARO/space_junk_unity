using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManagerMiddleMan : MonoBehaviour
{
    // Start is called before the first frame update
    public void ReducePlayerHealth(){
        GameManager.Instance.ReducePlayerHealth();
    }
}
