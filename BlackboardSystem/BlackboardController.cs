using Assets.Scripts.Framework.ServiceManagement;
using UnityEngine;

namespace Assets.Scripts.Framework.BlackboardSystem
{
    public class BlackboardController : MonoBehaviour
    {
        [SerializeField] private BlackboardData blackboardData;
        private readonly Blackboard m_Blackboard = new();
        private readonly Arbiter m_Arbiter = new();
        
        private void Awake()
        {
            ServiceLocator.Global.Register(this, ServiceLevel.Global, gameObject.scene.name);
            blackboardData.SetValuesOnBlackboard(m_Blackboard);
            // m_Blackboard.Debug();
        }

        public Blackboard GetBlackboard() => m_Blackboard;
        public void RegisterExpert(IExpert expert) => m_Arbiter.RegisterExpert(expert);
        public void DeregisterExpert(IExpert expert) => m_Arbiter.DeregisterExpert(expert);

        private void Update()
        {
            // Execute all agreed actions from the current iteration
            foreach (var action in m_Arbiter.BlackboardIteration(m_Blackboard))
            {
                action();
            }
        }
    }
}
