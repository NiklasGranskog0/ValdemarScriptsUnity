namespace Assets.Scripts.Framework.BlackboardSystem
{
    public interface IExpert
    {
        int GetPriority(Blackboard blackboard);
        void Execute(Blackboard blackboard);
    }
}
