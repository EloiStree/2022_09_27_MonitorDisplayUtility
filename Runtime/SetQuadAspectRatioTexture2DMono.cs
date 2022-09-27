using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetQuadAspectRatioTexture2DMono : MonoBehaviour
{
    public Transform m_localToAffect;
    public Texture m_texture;
    public int m_width;
    public int m_height;
    public float m_ratio;

    public void SetWith(Texture texture) {

        if (texture is Texture2D)
        {
            Texture2D t = (Texture2D)texture;
            m_width = t.width;
            m_height = t.height;
        }
        if (texture is RenderTexture)
        {
            RenderTexture t = (RenderTexture)texture;
            m_width = t.width;
            m_height = t.height;
        }
        if (m_height == 0 || m_width == 0)
            m_ratio = 1;
        else m_ratio = m_height / (float) m_width;
        m_localToAffect.localScale = new Vector3(1, m_ratio, m_ratio);


    }
}
