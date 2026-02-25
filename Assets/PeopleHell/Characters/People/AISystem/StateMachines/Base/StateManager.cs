using System;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public abstract class StateManager<TEState> : MonoBehaviour where TEState : Enum
{
     protected Dictionary<TEState, BaseState<TEState>> States = new Dictionary<TEState, BaseState<TEState>>();
     protected BaseState<TEState> CurrentState;
     protected bool IsTransitioningState = false;
     
     //Debug Variables
     [Header("Debug")]
     [SerializeField] protected TextMeshProUGUI _textMesh;
     [SerializeField] protected String _currentStateName;

     private void Start()
     {
          CurrentState.EnterState();
     }

     private void Update()
     {
          TEState nextStateKey = CurrentState.NextState;
          _textMesh.text = nextStateKey.ToString();
          _currentStateName = nextStateKey.ToString();

          if (!IsTransitioningState && nextStateKey.Equals(CurrentState.StateKey))
          {
               CurrentState.UpdateState();
          }
          else if (!IsTransitioningState)
          {
               TransitionToState(nextStateKey);
          }
     }

     private void TransitionToState(TEState stateKey)
     {
          IsTransitioningState = true;
          CurrentState.ExitState();
          CurrentState = States[stateKey];
          CurrentState.EnterState();
          IsTransitioningState = false;
     }

     private void OnTriggerEnter(Collider other)
     {
          CurrentState.OnTriggerEnter(other);
     }

     private void OnTriggerStay(Collider other)
     {
          CurrentState.OnTriggerStay(other);
     }

     private void OnTriggerExit(Collider other)
     {
          CurrentState.OnTriggerExit(other);
     }

     private void OnCollisionEnter(Collision other)
     {
          CurrentState.OnCollisionEnter(other);
     }

     private void OnCollisionStay(Collision other)
     {
          CurrentState.OnCollisionStay(other);
     }

     private void OnCollisionExit(Collision other)
     {
          CurrentState.OnCollisionExit(other);
     }

}
