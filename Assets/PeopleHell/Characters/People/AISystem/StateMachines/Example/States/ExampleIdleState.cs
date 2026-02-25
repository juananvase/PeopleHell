using UnityEngine;

public class ExampleIdleState : ExampleBaseState
{
    public ExampleIdleState(ExampleStateMachine.EExampleState key, ExampleContext context) : base(key, context)
    {
        ExampleContext Context = context;
    }
    
    //State Machine Methods
    public override void EnterState()
    {
        base.EnterState();
    }

    public override void ExitState()
    {
        base.ExitState();
    }

    public override void UpdateState()
    {
        DoTheCoolThing(Context.CharacterTransform.gameObject);
    }
    
    
    //Member State Methods
    
    
    //BaseStateMethods
    public override void DoTheCoolThing(GameObject coolGameObject)
    {
        Debug.Log($"Im doing the cool thing. The cool GameObject is: {coolGameObject.name}");
        NextState = ExampleStateMachine.EExampleState.Patrolling;
    }


    //Basic Generic Methods
    public override void OnTriggerEnter(Collider other){}
    
    public override void OnTriggerStay(Collider other){}
    
    public override void OnTriggerExit(Collider other){}
    
    public override void OnCollisionEnter(Collision other){}
    
    public override void OnCollisionStay(Collision other){}
    
    public override void OnCollisionExit(Collision other){}
    
}
