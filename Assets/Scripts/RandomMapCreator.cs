using IDG;
using UnityEngine;

public class RandomMapCreator : MapCreator, IGameManager
{
    FSClient client;
    public int InitLayer
    {
        get
        {
            return 99;
        }
    }

    public float mapScale;

    public void Init(FSClient client)
    {
        this.client = client;
        map = new GridMap();
        map.CreatTileCallBack = CreateShap;
        map.Init(30, 30, mapScale, client.random.Range);
        map.RandomRoom();

        map.PrimConnect();
        mapView.ShowMap(map);
    }

    public Fixed2 GetRandomPos()
    {
        var tile = map.GetRandomTile(TileType.room);
        return new Fixed2(tile.x * mapScale, tile.y * mapScale);
    }

    void CreateShap(Transform trans, Vector2[] points)
    {
        var shap = new IDG.ShapData();
        shap.Init(client);

        shap.transform.Position = new IDG.Fixed2(trans.position.x, trans.position.z);
        shap.transform.Rotation = trans.rotation.ToFixedRotation();
        shap.transform.Scale = Fixed2.one * mapScale.ToFixed();
        shap.SetShap(points);

        client.objectManager.Instantiate(shap);
    }
}
