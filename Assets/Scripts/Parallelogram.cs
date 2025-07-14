using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
public class SkewQuad : MonoBehaviour
{
    public float skewAmount = 0.5f;

    void Start()
    {
        var mf = GetComponent<MeshFilter>();

        // ✅ 创建一个新的 mesh 实例
        Mesh originalMesh = mf.mesh;
        Mesh meshCopy = Instantiate(originalMesh);
        mf.mesh = meshCopy;

        // ✅ 修改顶点
        Vector3[] vertices = meshCopy.vertices;
        vertices[1] += new Vector3(skewAmount, 0, 0); // 左上
        vertices[3] += new Vector3(skewAmount, 0, 0); // 右上
        meshCopy.vertices = vertices;

        // ✅ 更新法线和边界盒
        meshCopy.RecalculateNormals();
        meshCopy.RecalculateBounds();

        Debug.Log("平行四边形变形完成！");
    }
}

