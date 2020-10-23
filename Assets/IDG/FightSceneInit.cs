using IDG;
using System.Collections.Generic;
using UnityEngine;

public class FightSceneInit : MonoBehaviour
{
    public FightClientForUnity3D unityClient;
    public View[] views;
    public List<DataInitInfo> sceneInfo;

    private void Awake()
    {
        for (int i = 0; i < views.Length; i++)
        {
            Destroy(views[i].gameObject);
        }
        InitScene();
    }

    [ContextMenu("SaveScene")]
    public void SaveScene()
    {
        sceneInfo = new List<DataInitInfo>();
        views = GetComponentsInChildren<View>(true);
        foreach (var v in views)
        {
            var info = new DataInitInfo();
            info.className = v.GetDataType().ToString();
            info.pos = new int[] { (int)v.transform.position.x, (int)v.transform.position.z };
            sceneInfo.Add(info);
        }
    }

    public void InitScene()
    {
        foreach (var dataInfo in sceneInfo)
        {
            var data = System.Reflection.Assembly.GetExecutingAssembly().CreateInstance(dataInfo.className, false) as NetData;
            if (data != null)
            {
                data.Init(unityClient.client);
                data.transform.Reset(new IDG.Fixed2(dataInfo.pos[0], dataInfo.pos[1]), Fixed.Zero);
                unityClient.client.objectManager.Instantiate(data);
            }
        }
    }
}

[System.Serializable]
public struct DataInitInfo
{
    public string className;
    public int[] pos;
}