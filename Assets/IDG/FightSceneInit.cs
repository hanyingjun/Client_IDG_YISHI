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
        id = idCount++;

        for (int i = 0; i < views.Length; i++)
        {
            Destroy(views[i].gameObject);
        }
        if (id == 0)
        {
            for (int i = 0; i < testCount; i++)
            {
                GameObject.Instantiate(gameObject);
            }
        }
        InitScene();
    }

    public int testCount = 2;
    static int idCount = 0;
    public int id;
    Dictionary<KeyCode, int> KeyToId;
    void Start()
    {
        if (id >= 1)
        {
            ChildActive(false);
        }
        KeyToId = new Dictionary<KeyCode, int>();
        KeyToId.Add(KeyCode.F1, 0);
        KeyToId.Add(KeyCode.F2, 1);
        KeyToId.Add(KeyCode.F3, 2);


    }

    private void Update()
    {
        ChangeClient();
    }

    public void ChangeClient()
    {
        foreach (var t in KeyToId)
        {
            if (Input.GetKeyDown(t.Key))
            {
                if (id == t.Value)
                {
                    ChildActive(true);
                }
                else
                {
                    ChildActive(false);
                }
            }
        }
    }

    public void ChildActive(bool active)
    {
        foreach (var item in GetComponentsInChildren<Transform>(true))
        {
            if (item.gameObject != gameObject)
            {
                item.gameObject.SetActive(active);
            }
        }
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