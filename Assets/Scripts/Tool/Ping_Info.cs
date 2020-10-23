using UnityEngine;

public class Ping_Info : MonoBehaviour
{
    GUIStyle guiStyle;

    [SerializeField] FightClientForUnity3D FCF = null;

    void Start()
    {
        guiStyle = new GUIStyle();
        guiStyle.normal.background = null;
        guiStyle.fontSize = 40;
    }

    void OnGUI()
    {
        if (FCF != null && FCF.client != null)
        {
            SetColor(FCF.client.Ping);
            GUI.Label(new Rect(10, 50, 200, 50), "ping:" + FCF.client.Ping + "ms", guiStyle);
        }
    }

    /// <summary>
    /// 仿王者荣耀延迟过高，颜色变化
    /// </summary>
    /// <param name="pingValue"></param>
    void SetColor(int pingValue)
    {
        if (pingValue < 100)
        {
            guiStyle.normal.textColor = new Color(0, 1, 0);
        }
        else if (pingValue < 200)
        {
            guiStyle.normal.textColor = new Color(1, 1, 0);
        }
        else
        {
            guiStyle.normal.textColor = new Color(1, 0, 0);
        }
    }
}
