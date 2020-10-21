using IDG;
using IDG.MobileInput;
using UnityEngine;

public class MoveJoyStick : JoyStick
{
    public KeyCode up = KeyCode.W;
    public KeyCode down = KeyCode.S;
    public KeyCode left = KeyCode.A;
    public KeyCode right = KeyCode.D;

    void PcControl()
    {
        if (!useKey || onDrag)
            return;

        Vector3 pos = new Vector3();
        isDown = false;
        if (Input.GetKey(left))
        {
            pos.x -= 1;
            isDown = true;
        }
        if (Input.GetKey(right))
        {
            pos.x += 1;
            isDown = true;
        }
        if (Input.GetKey(down))
        {
            pos.y -= 1;
            isDown = true;
        }
        if (Input.GetKey(up))
        {
            pos.y += 1;
            isDown = true;
        }
        moveObj.transform.position = transform.position + pos.normalized * maxScale;
        Vector3 tmp = GetVector3();
        dir = new Fixed2(tmp.x, tmp.y);
    }

    void Update()
    {
        PcControl();
    }
}
