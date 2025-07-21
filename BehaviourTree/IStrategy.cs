using System;

namespace Assets.Scripts.Framework.BehaviourTree
{
    public interface IStrategy
    {
        Node.Status Process();

        void Reset()
        {
            // No op
        }
    }

    public class ActionStrategy : IStrategy
    {
        public ActionStrategy(Action doSomething)
        {
            m_DoSomething = doSomething;
        }
        
        private readonly Action m_DoSomething;

        public Node.Status Process()
        {
            m_DoSomething();
            return Node.Status.Success;
        }
    }

    public class Condition : IStrategy
    {
        public Condition(Func<bool> predicate)
        {
            m_Predicate = predicate;
        }
        
        private readonly Func<bool> m_Predicate;

        public Node.Status Process() => m_Predicate() ? Node.Status.Success : Node.Status.Failure;
    }
}
