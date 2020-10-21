using IDG;
using IDG.MobileInput;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 【战斗客户端Unity接口】管理【战斗客户端】并在每帧进行解析消息
/// </summary>
public class FightClientForUnity3D : MonoBehaviour
{
    public FSClient client;
    public List<JoyStick> joySticks;
    public List<NetButton> netButtons;
    public Camera mainCamera;

    void Awake()
    {
        joySticks = new List<JoyStick>();
        joySticks.AddRange(GetComponentsInChildren<JoyStick>());
        netButtons = new List<NetButton>();
        netButtons.AddRange(GetComponentsInChildren<NetButton>());
        client = new FSClient();

        client.unityClient = this;
        //   client.Connect("127.0.0.1", 12345,10);
        client.Connect(GameUser.user.fightRoom.ip, int.Parse(GameUser.user.fightRoom.port), 10, GetComponentsInChildren<IGameManager>());
    }

    private void FixedUpdate()
    {
        if (client.MessageList.Count > 0)
        {
            client.ParseMessage(client.MessageList.Dequeue());
        }

        CommitKey();
    }

    public void CommitKey()
    {
        if (client == null || client.inputCenter == null)
            return;
        foreach (var btn in netButtons)
        {
            client.inputCenter.SetKey(btn.isDown, btn.key);
        }

        foreach (var joy in joySticks)
        {
            client.inputCenter.SetJoyStick(joy.key, joy.GetInfo());
        }
    }

    public void OnDestroy()
    {
        client.Stop();
    }
}
