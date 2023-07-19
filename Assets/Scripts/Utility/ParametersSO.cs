using UnityEngine;

namespace Utility
{
    [CreateAssetMenu(fileName = "GameParameters", menuName = "Data/New Parameters")]
    public class ParametersSO : ScriptableObject
    {
        [Range(1f, 2f)]
        public float BaseSpinDuration;

        [Range(1f, 5f)]
        public float LastSpinStopDelay;
        
        [Range(1f, 5f)]
        public float RewardedSpinStopDelay;
        
        [Range(0f, 0.1f)]
        public float SpinDelayMin;

        [Range(0.1f, 0.5f)]
        public float SpinDelayMax;
        
        [Range(10f, 50f), Tooltip("Per second.")]
        public float WheelSpeed;

        [Range(1f, 5f)]
        public float CoinAnimDuration;

        [Range(1, 50)]
        public int BaseCoinParticleRate;
    }
}