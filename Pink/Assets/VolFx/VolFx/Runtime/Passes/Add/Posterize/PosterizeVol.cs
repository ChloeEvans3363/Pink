using System;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

//  VolFx © NullTale - https://twitter.com/NullTale/
namespace VolFx
{
    [Serializable, VolumeComponentMenu("VolFx/Posterize")]
    public sealed class PosterizeVol : VolumeComponent, IPostProcessComponent
    {
        public ClampedIntParameter m_Count = new ClampedIntParameter(64, 1, 64);

        // =======================================================================
        public bool IsActive() => active && m_Count.overrideState;

        public bool IsTileCompatible() => false;
    }
}