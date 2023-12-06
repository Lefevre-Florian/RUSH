using Com.IsartDigital.Rush.Managers;
using UnityEngine;

// Author : Lefevre Florian
namespace Com.IsartDigital.Rush.UI
{
    public abstract class Screen : MonoBehaviour
    {
        [SerializeField] private GameObject _ChildScreen = null;
        [SerializeField] private GameObject _ParentScreen = null;

        private void Start() => Init();

        protected virtual void Init() { }

        protected void Back()
        {
            LevelManager.GetInstance().TriggerFadeOutScreen();
            if (_ParentScreen == null)
                return;

            Open(_ParentScreen);
            Close();
        }

        protected void Next()
        {
            if (_ChildScreen == null)
                return;

            Open(_ChildScreen);
            Close();
        }

        public void Close() => Destroy(gameObject);

        protected void Open(GameObject pScreen) => LevelManager.GetInstance().SwitchScene(pScreen);

    }
}