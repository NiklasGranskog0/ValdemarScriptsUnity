using System;

namespace Assets.Scripts.Framework.StateMachine
{
    public class FuncPredicate : IPredicate
    {
        private readonly Func<bool> m_Func;

        public FuncPredicate(Func<bool> func)
        {
            m_Func = func;
        }

        public bool Evaluate() => m_Func.Invoke();
    }
}
