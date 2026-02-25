using UnityEngine;

public class ExamplePatrollingState : ExampleBaseState
{
    public ExamplePatrollingState(ExampleStateMachine.EExampleState key, ExampleContext context) : base(key, context)
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
        
    }
    
    
    //Member State Methods
    
    
    //BaseStateMethods
    public override void DoTheCoolThing(GameObject coolGameObject) { }

    //Basic Generic Methods
    public override void OnTriggerEnter(Collider other){}
    
    public override void OnTriggerStay(Collider other){}
    
    public override void OnTriggerExit(Collider other){}
    
    public override void OnCollisionEnter(Collision other){}
    
    public override void OnCollisionStay(Collision other){}
    
    public override void OnCollisionExit(Collision other){}
}
