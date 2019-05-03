using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UFrame.Common;

namespace UFrame.ResourceManagement
{
    public class TerrainManager : Singleton<TerrainManager>, ISingleton
    {
        public void Init()
        {

        }

        public void LoadSlicingTerrain(string terrainName)
        {
            string slicingDataPath = string.Format("terrainslicing/{0}/{1}_slicingdata", terrainName, terrainName);
            var getter = ResHelper.LoadAsset(slicingDataPath);
            TerrainSlicingData slicingData = getter.Get(ResHelper.GetPubAssetGetterGo()) as TerrainSlicingData;
            for (int i = 0; i < slicingData.slicingSize; i++)
            {
                for (int j = 0; j < slicingData.slicingSize; j++)
                {
                    string path = string.Format("terrainslicing/{0}/{1}_{2}_{3}", slicingData.terrainName, slicingData.terrainName, i, j);
                    var getterGo = ResHelper.LoadGameObject(path);
                    GameObject go = getterGo.Get();
                    go.transform.position = new Vector3(slicingData.terrainSize.x / slicingData.slicingSize * i, 0, slicingData.terrainSize.z / slicingData.slicingSize * j);
                }
            }
        }

    }
}

