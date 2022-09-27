using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FetchTargetFocusScreenVRDesktopMono : MonoBehaviour { 

    public Eloi.ClassicUnityEvent_Texture2D m_onTextureChanged;
    public Material m_toAffectMaterial;

    [Header("Debug")]
    public byte m_index;
    public Texture2D m_debugTextureView;

    void Start()
    {
        FetchScreenWithVRDesktopMirror.I.m_onCursorScreenChanged += Refresh;
    }

    private void Refresh(byte screenSelected)
    {
        m_index = screenSelected;
        m_debugTextureView = FetchScreenWithVRDesktopMirror.I.GetTextureWithId(screenSelected);
        m_onTextureChanged.Invoke(m_debugTextureView);
        if(m_toAffectMaterial)
        m_toAffectMaterial.mainTexture = (m_debugTextureView);
    }


}
