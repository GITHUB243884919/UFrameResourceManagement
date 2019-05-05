using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UFrame.Common;
using UFrame.Data;

namespace UFrame.ResourceManagement
{
    public class TerrainManager : Singleton<TerrainManager>, ISingleton
    {
        GameObject terrainRoot;
        Transform terrainRootTrans;
        TerrainSlicingData slicingData;
        Vector2 mapTilesize;
        Vector2 trunkSize;
        /// <summary>
        /// trunk是9宫格，边长为3
        /// </summary>
        int trunkEdgeNum = 3;
        Dictionary<int, GameObject> trunkDic = new Dictionary<int, GameObject>();
        List<int> unloadTrunkLst = new List<int>();

        public void Init()
        {

        }

        public void LoadSlicingTerrain(string terrainName)
        {
            LoadTerrainRoot();
            LoadSlicingData(terrainName);
            for (int i = 0; i < slicingData.slicingSize; i++)
            {
                for (int j = 0; j < slicingData.slicingSize; j++)
                {
                    string path = string.Format("terrainslicing/{0}/{1}_{2}_{3}", slicingData.terrainName, slicingData.terrainName, i, j);
                    var getterGo = ResHelper.LoadGameObject(path);
                    GameObject go = getterGo.Get();
                    go.transform.position = new Vector3(slicingData.terrainSize.x / slicingData.slicingSize * i, 0, slicingData.terrainSize.z / slicingData.slicingSize * j);
                    //go.transform.SetParent(terrainRootTrans);
                }
            }
        }

        //public void LoadSlicingMapTile(string terrainName, Vector3 pos)
        //{
        //    LoadTerrainRoot();
        //    LoadSlicingData(terrainName);
        //    LoadNineTrunk(pos);
        //}

        public void LoadSlicingMapTileAsync(string terrainName, Vector3 pos)
        {
            LoadTerrainRoot();
            LoadSlicingData(terrainName);
            LoadNineTrunkAsync(pos);
        }
        

        void LoadSlicingData(string terrainName)
        {
            if (slicingData != null)
            {
                return;
            }
            string slicingDataPath = string.Format("terrainslicing/{0}/{1}_slicingdata", terrainName, terrainName);
            var getter = ResHelper.LoadAsset(slicingDataPath);
            slicingData = getter.Get(terrainRoot) as TerrainSlicingData;
            trunkSize = new Vector2(slicingData.terrainSize.x / slicingData.slicingSize, slicingData.terrainSize.z / slicingData.slicingSize);
            mapTilesize = new Vector2(slicingData.terrainSize.x, slicingData.terrainSize.z);
            Logger.LogWarp.LogFormat("{0}, {1}", mapTilesize, trunkSize);
        }

        void LoadNineTrunkAsync(Vector3 pos)
        {
            Vector2_Bit idx = LocateTrunk(pos);
            //加载当前的九宫格
            LoadCurrNineTrunk(idx);
            //释放之前9宫格不在当前9宫格内的trunk
            UnLoadPreNineTrunk(idx);
        }

        Vector2_Bit LocateTrunk(Vector3 pos)
        {
            //ceil是上取整， 从0开始计数，所以-2
            int x = Mathf.CeilToInt(pos.x / trunkSize.x);
            int y = Mathf.CeilToInt(pos.z / trunkSize.y);
            return new Vector2_Bit(x - 2, y - 2);
        }

        void LoadCurrNineTrunk(Vector2_Bit idx)
        {
            for (int i = 0; i < trunkEdgeNum; i++)
            {
                for (int j = 0; j < trunkEdgeNum; j++)
                {
                    Vector2_Bit idxTrunk = new Vector2_Bit(i + idx.x, j + idx.y);
                    string path = string.Format("terrainslicing/{0}/{1}_{2}_{3}",
                        slicingData.terrainName, slicingData.terrainName,
                        idxTrunk.x, idxTrunk.y);
                    GameObject trunkGo = null;
                    if (!trunkDic.TryGetValue(idxTrunk.BitData, out trunkGo))
                    {
                        trunkDic.Add(idxTrunk.BitData, null);
                        ResHelper.LoadGameObjectAsync(path, (getter) =>
                        {
                            trunkGo = getter.Get();
                            float x = trunkSize.x * idxTrunk.x;
                            float y = trunkSize.y * idxTrunk.y;

                            trunkGo.transform.position = new Vector3(x, 0, y);

                            trunkGo.transform.SetParent(terrainRootTrans);
                            trunkDic[idxTrunk.BitData] = trunkGo;
                        });
                    }
                }
            }
        }

        void UnLoadPreNineTrunk(Vector2_Bit idx)
        {
            int x = idx.x + 1;
            int y = idx.y + 1;
            if (trunkDic.Count <= (trunkEdgeNum * trunkEdgeNum))
            {
                return;
            }
            unloadTrunkLst.Clear();
            foreach (var kv in trunkDic)
            {
                Vector2_Bit preIdx = new Vector2_Bit(kv.Key);
                int preX = preIdx.x;
                int preY = preIdx.y;

                if (Mathf.Abs(preX - x) > 1 || Mathf.Abs(preY - y) > 1)
                {
                    //Debug.Log(preIdx);
                    if (kv.Value != null)
                    {
                        ResHelper.DestroyGameObject(kv.Value);
                    }
                    unloadTrunkLst.Add(kv.Key);
                }
            }
            ResHelper.RealseAllUnUse();

            for (int i = 0; i < unloadTrunkLst.Count; i++)
            {
                trunkDic.Remove(unloadTrunkLst[i]);
            }
        }


        void LoadTerrainRoot()
        {
            if (terrainRoot != null)
            {
                return;
            }

            var getter = ResHelper.LoadGameObject("terrainslicing/terrrain_root");
            terrainRoot = getter.Get();
            terrainRootTrans = terrainRoot.transform;
            terrainRoot.transform.position = Vector3.zero;
            terrainRoot.transform.rotation = Quaternion.Euler(Vector3.zero);
        }

    }
}

