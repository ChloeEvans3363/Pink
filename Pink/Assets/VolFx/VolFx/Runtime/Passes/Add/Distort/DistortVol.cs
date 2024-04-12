using System;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

//  VolFx © NullTale - https://twitter.com/NullTale/
namespace VolFx
{
    [Serializable, VolumeComponentMenu("VolFx/Distort")]
    public sealed class DistortVol : VolumeComponent, IPostProcessComponent
    {
        public ClampedFloatParameter         m_Weight = new ClampedFloatParameter(0, 0, 1);
        public ClampedFloatParameter         m_Value  = new ClampedFloatParameter(0, 0, 1);
        public ClampedFloatParameter         m_Tiling = new ClampedFloatParameter(0, 0, 1);
        public NoInterpClampedFloatParameter m_Angle  = new NoInterpClampedFloatParameter(0, -180, 180);
        public ClampedFloatParameter         m_Motion = new ClampedFloatParameter(0, 0, 1);
        
        // =======================================================================
        public bool IsActive() => active && m_Value.value > 0;

        public bool IsTileCompatible() => false;
    }
}