using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Framework.Extensions;

namespace Assets.Scripts.Framework.BehaviourTree
{
    // UntilSuccess etc . . .
    
    public class UntilFailure : Node
    {
        public UntilFailure(string name) : base(name) {}

        public override Status Process()
        {
            if (children[0].Process() == Status.Failure)
            {
                Reset();
                return Status.Failure;
            }

            return Status.InProgress;
        }
    }
    
    public class Inverter : Node
    {
        public Inverter(string name) : base(name) {}

        public override Status Process()
        {
            switch (children[0].Process())
            {
                case Status.Failure: return Status.Success;
                case Status.InProgress: return Status.InProgress;
                default: return Status.Failure;
            }
        }
    }
    
    public class RandomSelector : PrioritySelector
    {
        public RandomSelector(string name) : base(name) {}
        
       protected override List<Node> SortChildren() => children.Shuffle().ToList();
    }
    
    public class PrioritySelector : Selector
    {
        public PrioritySelector(string name) : base(name) {}
    
        private List<Node> m_SortedChildren;
        private List<Node> SortedChildren => m_SortedChildren ??= SortChildren();

        protected virtual List<Node> SortChildren() => children.OrderByDescending(child => child.priority).ToList();

        public override void Reset()
        {
            base.Reset();
            m_SortedChildren = null;
        }

        public override Status Process()
        {
            foreach (var child in SortedChildren)
            {
                switch (child.Process())
                {
                    case Status.InProgress: return Status.InProgress;
                    case Status.Success: return Status.Success;
                    default: continue;
                }
            }

            return Status.Failure;
        }
    }
    
    public class Selector : Node
    {
        public Selector(string name, int priority = 0) : base(name, priority) {}

        public override Status Process()
        {
            if (currentChild < children.Count)
            {
                switch (children[currentChild].Process())
                {
                    case Status.InProgress: return Status.InProgress;
                    case Status.Success:
                        Reset();
                        return Status.Success;
                    default:
                        currentChild++;
                        return Status.InProgress;
                }
            }
            
            Reset();
            return Status.Failure;
        }
    }
    
    public class Sequence : Node
    {
        public Sequence(string name, int priority = 0) : base(name, priority) { }

        public override Status Process()
        {
            if (currentChild < children.Count)
            {
                switch (children[currentChild].Process())
                {
                    case Status.Success: return Status.Success;
                    case Status.Failure:
                        Reset();
                        return Status.Failure;
                    default:
                        currentChild++;
                        return currentChild == children.Count ? Status.Success : Status.InProgress;
                }
            }

            Reset();
            return Status.Success;
        }
    }
    
    public class BehaviourTree : Node
    {
        public BehaviourTree(string name) : base(name) { }

        public override Status Process()
        {
            while (currentChild < children.Count)
            {
                var status = children[currentChild].Process();
                if (status != Status.Success) return status;
                currentChild++;
            }

            return Status.Success;
        }
    }
}
