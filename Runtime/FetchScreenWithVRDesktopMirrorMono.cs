
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.UI;
using static VdmDesktopManager;

public class FetchScreenWithVRDesktopMirrorMono : MonoBehaviour
{

    public Eloi.ClassicUnityEvent_Texture2D m_displayInCanvasMouseFocus;
    public Material m_displayInMaterialMouseFocus;

    public Eloi.ClassicUnityEvent_Texture2D[] m_displayAsTexture;
    public Material[] m_displayInSharedMaterial;

   // public FetchScreenWithVRDesktopMirror m_data;
    public IEnumerator Start()
    {
        yield return FetchScreenWithVRDesktopMirror.I.Start();
        StartCoroutine(OnRender());
        for (int i = 0; i < FetchScreenWithVRDesktopMirror.I.m_displayCount; i++)
        {
            if (i < m_displayAsTexture.Length)
                m_displayAsTexture[i].Invoke(FetchScreenWithVRDesktopMirror.I.GetTextureWithId(i)) ;
            if (i < m_displayInSharedMaterial.Length)
                m_displayInSharedMaterial[i].mainTexture = FetchScreenWithVRDesktopMirror.I.GetTextureWithId(i);
        }
        //m_data = FetchScreenWithVRDesktopMirror.I;
    }
    public IEnumerator OnRender()
    {
        yield return FetchScreenWithVRDesktopMirror.I.OnRender();
    }

    public void Update()
    {
        FetchScreenWithVRDesktopMirror.I.Update();
        for (int i = 0; i < FetchScreenWithVRDesktopMirror.I.m_displayCount; i++)
        {
            if (i < FetchScreenWithVRDesktopMirror.I.m_isMouseInScreen.Length)
            {
                 if (FetchScreenWithVRDesktopMirror.I.m_isMouseInScreen[i])
                {
                   m_displayInCanvasMouseFocus.Invoke(FetchScreenWithVRDesktopMirror.I.m_screenTexture[i]);
                   m_displayInMaterialMouseFocus.mainTexture = FetchScreenWithVRDesktopMirror.I.m_screenTexture[i];
                    
                }
            }
        }
    }

    public void Refresh() {

        for (int i = 0; i < FetchScreenWithVRDesktopMirror.I.m_displayCount; i++)
        {
            if (i < m_displayAsTexture.Length && i < FetchScreenWithVRDesktopMirror.I.m_screenTexture.Length)
            {
                m_displayAsTexture[i].Invoke(FetchScreenWithVRDesktopMirror.I.m_screenTexture[i]);
            }
            if (i < m_displayInSharedMaterial.Length && i < FetchScreenWithVRDesktopMirror.I.m_screenTexture.Length)
            {
                m_displayInSharedMaterial[i].mainTexture = FetchScreenWithVRDesktopMirror.I.m_screenTexture[i];
            }
        }
    }

}

[System.Serializable]
public class FetchScreenWithVRDesktopMirror
{
    public Action m_onChangedValue;
    public Action m_onRenderRefresh;
    public Action<byte> m_onCursorScreenChanged;
    public static FetchScreenWithVRDesktopMirror I = new FetchScreenWithVRDesktopMirror();
    public bool m_linearColorSpace = false;
    public int m_displayCount;
    public Texture2D[] m_screenTexture= new  Texture2D[0];
    public bool[] m_isMouseInScreen = new bool[0];


    

    public IEnumerator Start()
    {
        DesktopCapturePlugin_Initialize();
        yield return new WaitForSeconds(1);
        {
            m_displayCount = DesktopCapturePlugin_GetNDesks();
            m_isMouseInScreen = new bool[m_displayCount];
            DesktopCapturePlugin_Initialize();  
            m_screenTexture = new Texture2D[m_displayCount];
            for (int i = 0; i < m_displayCount; i++)
            {
                int width = DesktopCapturePlugin_GetWidth(i);
                int height = DesktopCapturePlugin_GetHeight(i);
                var tex = new Texture2D(width, height, TextureFormat.BGRA32, false, m_linearColorSpace);
                m_screenTexture[i] = tex;
                DesktopCapturePlugin_SetTexturePtr(i, m_screenTexture[i].GetNativeTexturePtr());
            }
            yield return new WaitForSeconds(1);
            if(m_onChangedValue!=null)
                m_onChangedValue.Invoke();
        }
        //        StartCoroutine(OnRender());
    }
    
    public IEnumerator OnRender()
    {
        for (; ; )
        {
            yield return new WaitForEndOfFrame();
            GL.IssuePluginEvent(DesktopCapturePlugin_GetRenderEventFunc(), 0);
            if (m_onRenderRefresh != null)
                m_onRenderRefresh.Invoke();
        }
    }


    [DllImport("user32.dll")]
    public static extern System.IntPtr GetActiveWindow();
    [DllImport("DesktopCapture")]
    public static extern void DesktopCapturePlugin_Initialize();
    [DllImport("DesktopCapture")]
    public static extern int DesktopCapturePlugin_GetNDesks();
    [DllImport("DesktopCapture")]
    public static extern int DesktopCapturePlugin_GetWidth(int iDesk);
    [DllImport("DesktopCapture")]
    public static extern int DesktopCapturePlugin_GetHeight(int iDesk);
    [DllImport("DesktopCapture")]
    public static extern int DesktopCapturePlugin_GetNeedReInit();
    [DllImport("DesktopCapture")]
    public static extern bool DesktopCapturePlugin_IsPointerVisible(int iDesk);
    [DllImport("DesktopCapture")]
    public static extern int DesktopCapturePlugin_GetPointerX(int iDesk);
    [DllImport("DesktopCapture")]
    public static extern int DesktopCapturePlugin_GetPointerY(int iDesk);
    [DllImport("DesktopCapture")]
    public static extern int DesktopCapturePlugin_SetTexturePtr(int iDesk, IntPtr ptr);
    [DllImport("DesktopCapture")]
    public static extern IntPtr DesktopCapturePlugin_GetRenderEventFunc();

    public Texture2D GetTextureWithId(int index)
    {
        if (index < m_screenTexture.Length)
            return m_screenTexture[index];
        return null;
    }

    public void Update()
    {
        for (int i = 0; i < m_displayCount; i++)
        {
            if (i < m_isMouseInScreen.Length)
            {
                bool previous = m_isMouseInScreen[i];
                m_isMouseInScreen[i] = DesktopCapturePlugin_IsPointerVisible(i);
                if (m_isMouseInScreen[i]!= previous && m_isMouseInScreen[i])
                {
                    if (m_onCursorScreenChanged != null)
                        m_onCursorScreenChanged.Invoke((byte)i);

                }
            }
        }
    }
}
