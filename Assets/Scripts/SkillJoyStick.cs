﻿using IDG;
using IDG.MobileInput;
using UnityEngine;
using UnityEngine.UI;

public class SkillJoyStick : JoyStick
{
    public Image fillImage;
    public Image backImage;
    public SkillEngine skillList;
    public KeyCode pcKey = KeyCode.Mouse0;

    public FightClientForUnity3D unityClient;

    void Start()
    {
        unityClient = GetComponentInParent<FightClientForUnity3D>();
    }

    void Update()
    {
        PcControl();
        SkillMask();
    }

    void PcControl()
    {
        if (!useKey || onDrag || unityClient.client.localPlayer == null)
            return;
        isDown = Input.GetKey(pcKey);

        Vector3 pos = Input.mousePosition - unityClient.mainCamera.WorldToScreenPoint(unityClient.client.localPlayer.view.transform.position);
        moveObj.transform.position = transform.position + pos.normalized * maxScale;
        Vector3 tmp = GetVector3();
        dir = new Fixed2(tmp.x, tmp.y);
    }

    void SkillMask()
    {
        if (skillList == null)
        {
            if (unityClient != null && unityClient.client.localPlayer != null)
            {
                skillList = (unityClient.client.localPlayer as PlayerData).skillList;
            }
        }

        SkillRuntime skill = GetSkill(key);
        if (skill != null)
        {
            fillImage.fillAmount = (skill.timer / skill.skillData.coolDownTime).ToFloat();
            backImage.enabled = fillImage.fillAmount > 0;
            if (!useKey)
                group.blocksRaycasts = fillImage.fillAmount <= 0;
        }
    }

    SkillRuntime GetSkill(KeyNum key)
    {
        if (skillList != null)
        {
            return skillList.GetSkill(key);
        }
        return null;
    }
}
