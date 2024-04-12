using System;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

//  VolFx © NullTale - https://twitter.com/NullTale/
namespace VolFx
{
    [Serializable, VolumeComponentMenu("VolFx/Scanlines")]
    public sealed class ScanlinesVol : VolumeComponent, IPostProcessComponent
    {
        public ClampedFloatParameter m_Intensity = new ClampedFloatParameter(0, 0, 1.2f);
        public ClampedFloatParameter m_Count     = new ClampedFloatParameter(570, 100, 1000);
        
        public ClampedFloatParameter m_Speed = new ClampedFloatParameter(0, -1, 1);
        public ClampedFloatParameter m_Flicker = new ClampedFloatParameter(0, 0, 1);

        // =======================================================================

        // Can be used to skip rendering if false
        public bool IsActive() => active && (m_Intensity.value > 0);

        public bool IsTileCompatible() => false;
    }
}