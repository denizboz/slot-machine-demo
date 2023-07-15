using UnityEngine;

namespace Utility
{
    [CreateAssetMenu(fileName = "GameParameters", menuName = "Data/New Parameters")]
    public class ParametersSO : ScriptableObject
    {
        [Range(1f, 5f)]
        public float BaseSpinDuration;
        
        [Range(0f, 5f)]
        public float RewardedSlowDownTime;
        
        [Range(0f, 5f)]
        public float JackpotSlowDownTime;
        
        [Range(1000, 5000), Tooltip("Per second, in terms of pixels.")]
        public int WheelSpeed;

        [Range(1f, 5f)]
        public float CoinAnimDuration;
    }
}