using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FetchTargetScreenVRDesktopMono : MonoBehaviour
{
    public byte m_index;
    public Eloi.ClassicUnityEvent_Texture2D m_onTextureChanged;
    public Material m_toAffectMaterial;
    public Texture2D m_debugTextureView;

    void Start()
    {
        FetchScreenWithVRDesktopMirror.I.m_onChangedValue += Refresh;
    }

    private void Refresh()
    {
        m_debugTextureView= FetchScreenWithVRDesktopMirror.I.GetTextureWithId(m_index);
        m_onTextureChanged.Invoke(m_debugTextureView);
        if(m_toAffectMaterial)
        m_toAffectMaterial.mainTexture= (m_debugTextureView);
    }

    
}
