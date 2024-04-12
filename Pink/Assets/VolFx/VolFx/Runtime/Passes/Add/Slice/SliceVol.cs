using System;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

//  VolFx © NullTale - https://twitter.com/NullTale/
namespace VolFx
{
    [Serializable, VolumeComponentMenu("VolFx/Slice")]
    public sealed class SliceVol : VolumeComponent, IPostProcessComponent
    {
        public ClampedFloatParameter         m_Value  = new ClampedFloatParameter(0, 0, 3);
        public NoInterpClampedFloatParameter m_Tiling = new NoInterpClampedFloatParameter(500, 0, 700);
        public NoInterpClampedFloatParameter m_Angle  = new NoInterpClampedFloatParameter(0, -180, 180);
        
        // =======================================================================
        public bool IsActive() => active && m_Value.value > 0;

        public bool IsTileCompatible() => false;
    }
}