using Com.IsartDigital.Rush.Tiles;
using UnityEngine;

// Author : Lefevre Florian
namespace Com.IsartDigital.Rush.Juiciness
{
    public class TeleporterParticles : MonoBehaviour
    {
        private const float HUNDRED_PERCENT = 100f;
        private const float MAX_COLOR_VALUE = 255f;

        [SerializeField] private ColoredTiles _ColorCompenents = null;
        [SerializeField][Range(0f,100f)] private float _PercentColor = 100;

        private void Start()
        {
            Color lColor = ColorLibrary.Library[_ColorCompenents.Color];

            float lInternalRatio = MAX_COLOR_VALUE * (HUNDRED_PERCENT / _PercentColor);

            ParticleSystem.ColorOverLifetimeModule lParticleModule = default;
            Gradient lGradient = null;

            int lLength = transform.childCount;
            for (int i = 0; i < lLength; i++)
            {
                lParticleModule = transform.GetChild(i).GetComponent<ParticleSystem>().colorOverLifetime;
                
                lGradient = new Gradient();
                lGradient.colorKeys = new GradientColorKey[]
                {
                    new GradientColorKey(lColor, 0f),
                    new GradientColorKey(new Color(lColor.r + lInternalRatio * MAX_COLOR_VALUE,lColor.g + lInternalRatio * MAX_COLOR_VALUE, lColor.b + lInternalRatio * MAX_COLOR_VALUE), 1f)
                };
                
                lParticleModule.color = new ParticleSystem.MinMaxGradient(lGradient);
            }
        }
    }
}