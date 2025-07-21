using System;
using System.Collections.Generic;

namespace Assets.Scripts.Framework.StateMachine
{
    public class StateMachine
    {
        private StateNode m_Current;
        private readonly Dictionary<Type, StateNode> m_Nodes = new();
        private readonly HashSet<ITransition> m_AnyTransition = new();

        public void Update()
        {
            var transition = GetTransition();
            if (transition is not null) ChangeState(transition.To);
            m_Current.State.Update();
        }

        public void FixedUpdate() => m_Current.State.FixedUpdate();

        public void SetState(IState state)
        {
            m_Current = m_Nodes[state.GetType()];
            m_Current.State.OnEnter();
        }

        // If transitions is set up for the state machine this should not be needed called from another script
        private void ChangeState(IState state)
        {
            if (state == m_Current.State) return;

            var previousState = m_Current.State;
            var nextState = m_Nodes[state.GetType()].State;
            
            previousState.OnExit();
            nextState.OnEnter();
            m_Current = m_Nodes[state.GetType()];
        }

        public void AddTransition(IState from, IState to, IPredicate condition)
            => GetOrAddNode(from).AddTransition(GetOrAddNode(to).State, condition);

        public void AddAnyTransition(IState to, IPredicate condition) 
            => m_AnyTransition.Add(new Transition(GetOrAddNode(to).State, condition));

        private StateNode GetOrAddNode(IState state)
        {
            var node = m_Nodes.GetValueOrDefault(state.GetType());

            if (node is null)
            {
                node = new StateNode(state);
                m_Nodes.Add(state.GetType(), node);
            }

            return node;
        }

        private ITransition GetTransition()
        {
            foreach (var transition in m_AnyTransition) if (transition.Condition.Evaluate()) return transition;
            foreach (var transition in m_Current.Transitions) if (transition.Condition.Evaluate()) return transition;

            return null;
        }

        private class StateNode
        {
            public IState State { get; }
            public HashSet<ITransition> Transitions { get; }

            public StateNode(IState state)
            {
                State = state;
                Transitions = new HashSet<ITransition>();
            }

            public void AddTransition(IState to, IPredicate condition)
            {
                Transitions.Add(new Transition(to, condition));
            }
        }
    }
}
