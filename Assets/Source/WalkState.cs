public class WalkState : IState
{
    Batman batman;
    public WalkState(Batman batman)
    {
        this.batman = batman;
    }
    public void OnEnter()
    {
        batman.MoveOnEnter();
    }
    public void OnUpdate()
    {
        
    }
    public void OnFixedUpdate()
    {
        batman.Move(batman.WalkSpeed);
    }
    public void OnExit()
    {

    }
    public IState NextState()
    {
        if (batman.IsOnGround())
        {
            return this;
        }
        else return new FlyState(batman);
    }
}