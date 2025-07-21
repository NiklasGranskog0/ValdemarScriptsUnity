using Assets.Scripts.Framework;

namespace Assets.Scripts.Managers
{
    public class SelectionManager : SingletonInstance<SelectionManager>
    {
        // public GameObject GetSelectedUnit { get; private set; }
        //
        // [SerializeField] private Camera selectionCamera;
        //
        // private GameObject m_SelectedUnit;
        //
        // private const float k_UnitYHeight = 1f;
        // private Vector3 m_MousePosition;
        // public LayerMask unitLayerMask;
        // public LayerMask groundLayerMask;
        //
        // private void Update()
        // {
        //     m_MousePosition = Input.mousePosition;
        //     var ray = selectionCamera.ScreenPointToRay(m_MousePosition);
        //     GetSelectedUnit = m_SelectedUnit;
        //     
        //     if (m_SelectedUnit)
        //     {
        //         if (Physics.Raycast(ray, out var hit, Mathf.Infinity, groundLayerMask))
        //         {
        //             m_SelectedUnit.transform.position = new Vector3(hit.point.x, hit.point.y + k_UnitYHeight, hit.point.z);
        //         }
        //     }
        //
        //     if (Input.GetMouseButtonDown(0)) // Left
        //     {
        //         if (!Physics.Raycast(ray, out var hit, Mathf.Infinity, unitLayerMask))
        //             return;
        //     
        //         if (!hit.transform.TryGetComponent(out IUnit _)) 
        //             return;
        //
        //         m_SelectedUnit = hit.transform.gameObject;
        //     }
        //
        //     if (Input.GetMouseButtonUp(0) && m_SelectedUnit)
        //     {
        //         if (Physics.Raycast(ray, out var hit, Mathf.Infinity, groundLayerMask))
        //         {
        //             m_SelectedUnit.transform.position = new Vector3(hit.point.x, hit.point.y + k_UnitYHeight, hit.point.z);
        //                 
        //             m_SelectedUnit = null;
        //         }
        //     }
        // }
    }
}
