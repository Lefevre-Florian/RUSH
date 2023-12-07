using Com.IsartDigital.Rush.Camera;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// Author : Lefevre Florian
namespace Com.IsartDigital.Rush.UI
{
    public class DynamicRenderer : MonoBehaviour
    {
        [SerializeField] private RawImage _Renderer = null;
        [SerializeField] private GameObject _FailCam = null;

        private RenderTexture _MemoryTexture = null;

        private void OnEnable()
        {
            _FailCam.SetActive(true);

            _MemoryTexture = new RenderTexture(128,128,16);
            _MemoryTexture.Create();

            _FailCam.transform.position = OrbitalCamera.GetInstance().transform.position;
            _FailCam.transform.rotation = OrbitalCamera.GetInstance().transform.rotation;

            _FailCam.GetComponent<UnityEngine.Camera>().targetTexture = _MemoryTexture;
            _Renderer.texture = _MemoryTexture;
        }

        private void OnDisable()
        {
            _FailCam.SetActive(false);
            _FailCam.GetComponent<UnityEngine.Camera>().targetTexture = null;


            _MemoryTexture.Release();
            _MemoryTexture.DiscardContents();

            Destroy(_MemoryTexture);
            _MemoryTexture = null;
        }
    }
}