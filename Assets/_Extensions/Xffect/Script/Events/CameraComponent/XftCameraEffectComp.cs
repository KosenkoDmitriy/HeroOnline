using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Xft
{
    [ExecuteInEditMode]
    public class XftCameraEffectComp : MonoBehaviour
    {
        protected List<CameraEffectEvent> m_eventList = new List<CameraEffectEvent>();

        void Awake()
        {
            this.enabled = false;
        }


        public void ClearEvent()
        {
            m_eventList.Clear();
        }


        public void ResetEvent(CameraEffectEvent e)
        {
            m_eventList.Remove(e);
            if (m_eventList.Count == 0)
                enabled = false;
        }

        public void AddEvent(CameraEffectEvent e)
        {

            if (m_eventList.Contains(e))
            {
                Debug.LogWarning("camera effect event already exists: " + e.EffectType);
                return;
            }
            m_eventList.Add(e);
            m_eventList.Sort();

            enabled = true;
        }


        void OnPreRender()
        {
            for (int i = 0; i < m_eventList.Count; i++)
            {
                m_eventList[i].OnPreRender();
            }
        }

        void OnRenderImage(RenderTexture source, RenderTexture destination)
        {
            if (m_eventList.Count == 0)
                return;

            RenderTexture buffer1 = RenderTexture.GetTemporary(source.width, source.height, source.depth);
            RenderTexture buffer2 = RenderTexture.GetTemporary(source.width, source.height, source.depth);

            m_eventList[0].OnRenderImage(source, buffer1);


            bool oddEven = true;
            for (int i = 1; i < m_eventList.Count; i++)
            {
                if (oddEven)
                    m_eventList[i].OnRenderImage(buffer1, buffer2);
                else
                    m_eventList[i].OnRenderImage(buffer2, buffer1);
                oddEven = !oddEven;
            }

            if (oddEven)
                Graphics.Blit(buffer1, destination);
            else
                Graphics.Blit(buffer2, destination);


            RenderTexture.ReleaseTemporary(buffer1);
            RenderTexture.ReleaseTemporary(buffer2);
        }

    }
}

