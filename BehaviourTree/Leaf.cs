namespace Assets.Scripts.Framework.BehaviourTree
{
    public class Leaf : Node
    {
        public Leaf(string name, IStrategy strategy, int priority = 0) : base(name, priority)
        {
            // Preconditions.CheckNotNull(strategy);
            m_Strategy = strategy;
        }
        
        private readonly IStrategy m_Strategy;
        public override Status Process() => m_Strategy.Process();
        public override void Reset() => m_Strategy.Reset();
    }
}
