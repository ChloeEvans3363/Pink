using System;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

//  VolFx © NullTale - https://twitter.com/NullTale/
namespace VolFx
{
    [Serializable, VolumeComponentMenu("VolFx/Chromatic")]
    public sealed class ChromaticVol : VolumeComponent, IPostProcessComponent
    {
        public ClampedFloatParameter _Weight    = new ClampedFloatParameter(1f, 0f, 1f);
        public ClampedFloatParameter _Intensity = new ClampedFloatParameter(0f, -1f, 1f);
        public ClampedFloatParameter _Radial    = new ClampedFloatParameter(0f, -1f, 1f);
        public ClampedFloatParameter _Alpha     = new ClampedFloatParameter(0f, -1f, 1f);
        
        // =======================================================================
        public bool IsActive() => active && (_Intensity.value != 0f || _Radial.value != 0f && _Weight.value > 0f);

        public bool IsTileCompatible() => true;
    }
}