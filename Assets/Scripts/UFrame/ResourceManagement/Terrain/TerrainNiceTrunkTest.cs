using UnityEngine;
using UFrame.ResourceManagement;

public class TerrainNiceTrunkTest : MonoBehaviour
{
    void LateUpdate()
    {
        if (transform.position.x > 25
            && transform.position.x < 175
            && transform.position.z > 25 &&
            transform.position.z < 175)
        {
            TerrainManager.GetInstance().LoadSlicingMapTileAsync("terrain", transform.position);
        }

    }

    //void OnDrawGizmos()
    //{
    //    //if (target != null)
    //    int Num = 200 / 8;
    //    for (int i = 0;i < Num; i++)
    //    {
    //        Gizmos.color = Color.blue;
    //        Vector3 begin = new Vector3(0, 0, Num * i);
    //        Vector3 end = new Vector3(200, 0, Num * i);
    //        Gizmos.DrawLine(begin, end);
    //    }
    //}
    
}