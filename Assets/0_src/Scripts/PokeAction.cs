using System.Collections;
using System.Collections.Generic;
using Oculus.Interaction;
using UnityEngine;
using UnityEngine.Events;

public class PokeAction : MonoBehaviour
{
    enum PokingState{
        Nothing,
        Begun,
        Finished
    };

    private PokingState pokingState= PokingState.Nothing;
    public UnityEvent OnPoke;

    [SerializeField, Interface(typeof(IInteractableView))]
    private Object _interactableView;

    public IInteractableView InteractableView;

    protected virtual void Awake()
    {
        InteractableView = _interactableView as IInteractableView;
    }
    // Start is called before the first frame update
    void Start()
    {

        InteractableView.WhenStateChanged += UpdatePokingState;
        ChangePokingState();
    }


    private void UpdatePokingState(InteractableStateChangeArgs args) => ChangePokingState();

    private void ChangePokingState()
    {
        switch (InteractableView.State)
        {
            case InteractableState.Normal:
                if(pokingState == PokingState.Begun){
                    pokingState = PokingState.Finished;
                }
                break;
            case InteractableState.Hover:
                if(pokingState == PokingState.Begun){
                    pokingState = PokingState.Finished;
                }
                break;
            case InteractableState.Select:
                if(pokingState == PokingState.Nothing)
                    pokingState = PokingState.Begun;
                break;
            case InteractableState.Disabled:
                Debug.Log("L42:Disabled");
                break;
        }

        if(pokingState == PokingState.Finished){
            OnPoke?.Invoke();
            pokingState = PokingState.Nothing;
        }
    }
    
}
