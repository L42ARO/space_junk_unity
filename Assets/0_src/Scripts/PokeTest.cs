using System.Collections;
using System.Collections.Generic;
using Oculus.Interaction;
using UnityEngine;

public class PokeTest : MonoBehaviour
{
    [SerializeField, Interface(typeof(IInteractableView))]
    private Object _interactableView;

    public IInteractableView InteractableView;

    protected virtual void Awake()
    {
        InteractableView = _interactableView as IInteractableView;
    }
    protected virtual void Start()
    {
        InteractableView.WhenStateChanged += UpdateVisualState;
        UpdateVisual();
    }

    private void UpdateVisualState(InteractableStateChangeArgs args) => UpdateVisual();
    private void UpdateVisual()
        {
            switch (InteractableView.State)
            {
                case InteractableState.Normal:
                    Debug.Log("L42:Normal");
                    break;
                case InteractableState.Hover:
                    Debug.Log("L42:Hover");
                    break;
                case InteractableState.Select:
                    Debug.Log("L42:Select");
                    break;
                case InteractableState.Disabled:
                    Debug.Log("L42:Disabled");
                    break;
            }
        }
}
