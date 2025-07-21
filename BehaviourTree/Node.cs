using System.Collections.Generic;

namespace Assets.Scripts.Framework.BehaviourTree
{
    public class Node
    {
        public Node(string name = "Node", int priority = 0)
        {
            this.m_Name = name;
            this.priority = priority;
        }
        
        public enum Status { Success, Failure, InProgress }
        private readonly string m_Name;
        public readonly int priority;
        
        public readonly List<Node> children = new();
        protected int currentChild;

        public void AddChild(Node child) => children.Add(child);
        public virtual Status Process() => children[currentChild].Process();

        public virtual void Reset()
        {
            currentChild = 0;

            foreach (var child in children)
            {
                child.Reset();
            }
        }
    }
}
