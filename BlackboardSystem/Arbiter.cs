using System;
using System.Collections.Generic;
using Assets.Scripts.Framework.Extensions;

namespace Assets.Scripts.Framework.BlackboardSystem
{
    public class Arbiter
    {
        private readonly List<IExpert> m_Experts = new();

        public void RegisterExpert(IExpert expert)
        {
            Preconditions.CheckNotNull(expert);
            m_Experts.Add(expert);
        }

        public void DeregisterExpert(IExpert expert)
        {
            Preconditions.CheckNotNull(expert);
            m_Experts.Remove(expert);
        }

        public List<Action> BlackboardIteration(Blackboard blackboard)
        {
            IExpert bestExpert = null;
            var highestPriority = 0;

            foreach (var expert in m_Experts)
            {
                var priority = expert.GetPriority(blackboard);
                
                if (priority > highestPriority)
                {
                    highestPriority = priority;
                    bestExpert = expert;
                }
            }
            
            bestExpert?.Execute(blackboard);
            var actions = blackboard.PassedActions;
            blackboard.ClearActions();
            
            // Return or execute the actions here
            return actions;
        }
    }
}
