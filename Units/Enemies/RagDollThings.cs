using UnityEngine;
using UnityEngine.AI;

namespace Assets.Scripts.Units.Enemies
{
    public class RagDollThings : MonoBehaviour
    {
        [SerializeField] private Animator animator;
        public Transform ragDollRoot;
        [SerializeField] private NavMeshAgent agent;
        [SerializeField] private Rigidbody chestBody; 

        private Rigidbody[] m_Rigidbodies;
        private CharacterJoint[] m_Joints;

        private void Awake()
        {
            m_Rigidbodies = ragDollRoot.GetComponentsInChildren<Rigidbody>();
            m_Joints = ragDollRoot.GetComponentsInChildren<CharacterJoint>();
            EnableAnimator();
        }

        public void OnEnable() => EnableAnimator();

        // TODO: make one function, add force direction
        public void EnableRagDoll(float knockBackForce)
        {
            animator.enabled = false;
            agent.enabled = false;
            
            foreach (var joint in m_Joints)
            {
                joint.enableCollision = true;
            }

            foreach (var body in m_Rigidbodies)
            {
                body.isKinematic = false;
                chestBody.AddForce(-transform.forward * knockBackForce, ForceMode.Impulse); // Temp direction
                body.detectCollisions = true;
                body.useGravity = true;
            }
        }

        private void EnableAnimator()
        {
            animator.enabled = true;
            agent.enabled = true;
            
            foreach (var joint in m_Joints)
            {
                joint.enableCollision = false;
            }
            
            foreach (var body in m_Rigidbodies)
            {
                body.useGravity = false;
                body.isKinematic = true;
            }
        }
    }
}
