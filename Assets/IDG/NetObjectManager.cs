using UnityEngine;

namespace IDG
{
    public class NetObjectManager
    {
        FSClient client;

        public NetObjectManager(FSClient client)
        {
            this.client = client;
        }

        public GameObject Instantiate(NetData data)
        {
            GameObject obj = GameObject.Instantiate(GetPrefab(data), data.transform.Position.ToVector3(), data.transform.Rotation.ToUnityRotation());
            obj.transform.parent = (client.unityClient as MonoBehaviour).gameObject.transform;
            obj.transform.localScale = data.transform.Scale.ToVector3(1);
            var view = obj.GetComponent<View>();
            view.netData = data;
            data.view = view;
            return obj;
        }

        public void Destory(View view)
        {
            if (view == null)
            {
                Debug.Log("show is Null");
            }
            view.netData.Destory();
            GameObject.Destroy(view.gameObject);
        }

        public GameObject GetPrefab(NetData data)
        {
            var prefab = Resources.Load(data.PrefabPath()) as GameObject;
            if (prefab == null)
            {
                Debug.LogError("{" + data.PrefabPath() + "}is Null");
            }
            return prefab;
        }
    }
}