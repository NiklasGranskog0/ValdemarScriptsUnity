using System.Collections.Generic;
using Assets.Scripts.Framework.ServiceManagement;

namespace Assets.Scripts.Units.Enemies
{
    public class EnemyHandler
    {
        public EnemyHandler() => ServiceLocator.Global.Register(this, ServiceLevel.Global);
        private readonly HashSet<EnemyBase> m_EnemyBases = new();
        public void AddEnemy(EnemyBase e) => m_EnemyBases.Add(e);
        public void RemoveEnemy(EnemyBase e) => m_EnemyBases.Remove(e);
        
        public int GetEnemyCount => m_EnemyBases.Count;
    }
}
