using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UFrame.Common;

namespace UFrame.ResourceManagement
{
    public class TerrainManager : Singleton<TerrainManager>, ISingleton
    {
        GameObject terrainRoot;
        Transform terrainRootTrans;
        TerrainSlicingData slicingData;
        Vector2 mapTilesize;
        Vector2 trunkSize;
        Dictionary<Vector2, GameObject> trunckDic = new Dictionary<Vector2, GameObject>();

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

        public void LoadSlicingMapTile(string terrainName, Vector3 pos)
        {
            LoadTerrainRoot();
            LoadSlicingData(terrainName);
            LoadNineTrunk(pos);
        }

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

        Vector2 LocateTrunk(Vector3 pos)
        {
            Vector2 index = Vector2.zero;
            float x = Mathf.Ceil(pos.x / trunkSize.x);
            x -= 1;
            float y = Mathf.Ceil(pos.z / trunkSize.y);
            y -= 1;
            return index;
        }

        void LoadNineTrunk(Vector3 pos)
        {
            Vector2 idx = LocateTrunk(pos);
            Logger.LogWarp.Log("idx " + idx);
            for(int i = 0; i < 3; i++)
            {
                for(int j = 0; j < 3; j++)
                {
                    string path = string.Format("terrainslicing/{0}/{1}_{2}_{3}", 
                        slicingData.terrainName, slicingData.terrainName,
                        i + (int)(idx.x), j + (int)(idx.y));
                    Logger.LogWarp.Log(path);
                    var getterGo = ResHelper.LoadGameObject(path);
                    GameObject go = getterGo.Get();
                    float x = trunkSize.x * (i + (int)(idx.x));
                    float y = trunkSize.y * (j + (int)(idx.y));
                    go.transform.position = new Vector3(x, 0, y);
                    go.transform.SetParent(terrainRootTrans);
                }
            }
        }

        void LoadNineTrunkAsync(Vector3 pos)
        {
            Vector2 idx = LocateTrunk(pos);
            Vector2 idxTrunk = Vector2.zero;
            Logger.LogWarp.Log("idx " + idx);
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    idxTrunk.x = i + (int)(idx.x);
                    idxTrunk.y = j + (int)(idx.y);
                    string path = string.Format("terrainslicing/{0}/{1}_{2}_{3}",
                        slicingData.terrainName, slicingData.terrainName,
                        idxTrunk.x, idxTrunk.y);
                    Logger.LogWarp.Log(path);
                    ResHelper.LoadGameObjectAsync(path, (getter) =>
                    {
                        GameObject go = getter.Get();
                        float x = trunkSize.x * (i + (int)(idx.x));
                        float y = trunkSize.y * (j + (int)(idx.y));
                        go.transform.position = new Vector3(x, 0, y);
                        go.transform.SetParent(terrainRootTrans);
                    });

                }
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
        }

    }
}

