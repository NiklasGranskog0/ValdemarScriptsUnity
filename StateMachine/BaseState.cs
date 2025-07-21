namespace Assets.Scripts.Framework.StateMachine
{
    public class BaseState : IState
    {
        public virtual void OnEnter() { }
        public virtual void Update() { }
        public virtual void FixedUpdate() { }
        public virtual void OnExit() { }
    }
}
