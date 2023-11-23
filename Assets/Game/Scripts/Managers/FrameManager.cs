using UnityEngine;
using static TMPro.SpriteAssetUtilities.TexturePacker_JsonArray;

// Author : Lefevre Florian
namespace Com.IsartDigital.Rush.Managers
{
    public class FrameManager : MonoBehaviour
    {
        #region Singleton
        private static FrameManager _Instance = null;

        public static FrameManager GetInstance()
        {
            if (_Instance == null)
                _Instance = new FrameManager();
            return _Instance;
        }

        private FrameManager() : base() { }
        #endregion

        private const int MINIMUM_FPS_HARD_LIMIT = 15;

        [Header("Framerates")]
        [SerializeField] private bool _IsFPSLocked = true;

        [Space(5)]
        [SerializeField] private  int _DefaultMobileFramesRate = 30;
        [SerializeField] private  int _DefaultComputerFrameRate = 60;

        [SerializeField] private  int _MaxMobileFrameRate = 30;
        [SerializeField] private  int _MaxComputerFrameRate = 4000;

        private int _CurrentFrameRate = 0;

        public int CurrentFPS { get { return _CurrentFrameRate; } private set { _CurrentFrameRate = value; } }

        private void Awake()
        {
            if(_Instance != null)
            {
                Destroy(gameObject);
                return;
            }

            _Instance = this;
        }

        private void Start()
        {
            #if UNITY_STANDALONE
            _CurrentFrameRate = _DefaultComputerFrameRate;
            #endif
            #if UNITY_ANDROID
            _CurrentFrameRate = _DefaultMobileFramesRate;
            #endif


            Application.targetFrameRate = _CurrentFrameRate;
        }

        public void ChangeFrameRate(int pFrames)
        {
            int lCurrentMax = 0;

            #if UNITY_STANDALONE
            lCurrentMax = _MaxComputerFrameRate;
            #endif
            #if UNITY_ANDROID
            lCurrentMax = _MaxMobileFrameRate;
            #endif

            if (_IsFPSLocked || pFrames < MINIMUM_FPS_HARD_LIMIT || pFrames > lCurrentMax)
                return;

            _CurrentFrameRate = pFrames;
            Application.targetFrameRate = _CurrentFrameRate;
        }

        private void OnDestroy()
        {
            if (_Instance != null)
                _Instance = null;
        }
    }
}