using UnityEngine;

public abstract class ExampleBaseState : BaseState<ExampleStateMachine.EExampleState>
{
    protected ExampleContext Context;

    public ExampleBaseState(ExampleStateMachine.EExampleState key, ExampleContext context) : base(key)
    {
        Context = context;
    }
    
    public override void EnterState()
    {
        NextState = StateKey;
    }

    public override void ExitState()
    {
        NextState = StateKey;
    }

    public abstract void DoTheCoolThing(GameObject coolGameObject);
}
