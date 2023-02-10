public class FlyState : IState
{
    Batman batman;
    public FlyState(Batman batman)
    {
        this.batman = batman;
    }
    public void OnEnter()
    {
        batman.FlyOnEnter();
    }
    public void OnUpdate()
    {

    }
    public void OnFixedUpdate()
    {
        batman.Fly();
    }
    public void OnExit()
    {

    }
    public IState NextState()
    {
        if (batman.IsOnGround())
        {
            return new WalkState(batman);
        }
        else return this;
    }
}