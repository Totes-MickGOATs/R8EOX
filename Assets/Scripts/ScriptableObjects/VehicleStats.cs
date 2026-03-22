using System;
using UnityEngine;

namespace R8EOX
{
    [Serializable]
    public struct VehicleStats
    {
        [SerializeField] [Range(0f, 1f)] private float topSpeed;
        [SerializeField] [Range(0f, 1f)] private float acceleration;
        [SerializeField] [Range(0f, 1f)] private float handling;
        [SerializeField] [Range(0f, 1f)] private float weight;

        public float TopSpeed => topSpeed;
        public float Acceleration => acceleration;
        public float Handling => handling;
        public float Weight => weight;

        public VehicleStats(float topSpeed, float acceleration, float handling, float weight)
        {
            this.topSpeed = Mathf.Clamp01(topSpeed);
            this.acceleration = Mathf.Clamp01(acceleration);
            this.handling = Mathf.Clamp01(handling);
            this.weight = Mathf.Clamp01(weight);
        }
    }
}
