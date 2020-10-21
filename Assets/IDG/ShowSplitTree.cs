﻿using System.Collections.Generic;
using UnityEngine;

namespace IDG
{
    public class ShowSplitTree : MonoBehaviour
    {
        public FightClientForUnity3D unityClient;
        public bool showSplitTree = true;

        private void OnDrawGizmos()
        {
            if (!enabled)
                return;

            if (unityClient.client != null && unityClient.client.physics.tree != null)
            {
                Tree4 node;// = ShapPhysics.tree;
                Stack<Tree4> nodes = new Stack<Tree4>();
                float size = 0;
                nodes.Push(unityClient.client.physics.tree);
                Stack<int> indexs = new Stack<int>();
                indexs.Push(-1);
                int i = -1;
                Color[] colors = new Color[4];
                colors[0] = Color.cyan;
                colors[1] = Color.red;
                colors[2] = Color.blue;
                colors[3] = Color.yellow;
                Color c = Color.black;
                while (nodes.Count > 0)
                {
                    node = nodes.Pop();
                    i = indexs.Pop();
                    //Debug.Log(1);

                    size = (node.border.size).ToFloat() * 2 - node.depth * 1f / Tree4.MaxDepth;

                    if (i >= 0)
                    {
                        c = colors[i];
                    }
                    if (showSplitTree)
                    {
                        Gizmos.color = c;
                        Gizmos.DrawWireCube(node.border.center.ToVector3(), new Vector3(size, (Tree4.MaxDepth - node.depth) * 10, size));
                        c.a = 0.2f * (1f * node.objs.Count / Tree4.SplitSize);
                        Gizmos.color = c;
                        Gizmos.DrawCube(node.border.center.ToVector3(), new Vector3(size, (Tree4.MaxDepth - node.depth) * 10, size));
                        Gizmos.color = Color.black;
                    }

                    c.a = 0.3f;
                    Gizmos.color = c;
                    foreach (var item in node.objs)
                    {
                        if (showSplitTree)
                            Gizmos.DrawSphere(item.transform.Position.ToVector3() + Vector3.up * (i + 2), 1);

                        for (int index = 0; index < item.Shap.PointsCount - 1; index++)
                        {
                            var ponit = item.Shap.GetPoint(index);
                            Gizmos.DrawLine((item.transform.Position + ponit).ToVector3(), (item.transform.Position + item.Shap.GetPoint(index + 1)).ToVector3());
                            Gizmos.DrawSphere((item.transform.Position + ponit).ToVector3(), 0.05f);
                        }
                    }

                    if (node.child != null)
                    {
                        nodes.Push(node.child.LeftDown);
                        indexs.Push(0);
                        nodes.Push(node.child.RightDown);
                        indexs.Push(1);
                        nodes.Push(node.child.LeftUp);
                        indexs.Push(2);
                        nodes.Push(node.child.RightUp);
                        indexs.Push(3);
                    }
                }
            }
        }

    }

}