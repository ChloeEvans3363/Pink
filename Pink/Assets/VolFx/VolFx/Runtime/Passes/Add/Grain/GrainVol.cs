using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

//  VolFx © NullTale - https://twitter.com/NullTale/
namespace VolFx
{
    [Serializable, VolumeComponentMenu("VolFx/Grain")]
    public sealed class GrainVol : VolumeComponent, IPostProcessComponent
    {
        public ClampedFloatParameter m_Grain    = new ClampedFloatParameter(0f, 0f, 3f);
        public CurveParameter        m_Response = new CurveParameter(new CurveValue(new AnimationCurve(new Keyframe(0, 0, 0, 0), new Keyframe(1, 0, 0, 0))), false);
        
        public ClampedFloatParameter m_Brightness = new ClampedFloatParameter(0, -1, 1);
        public ClampedFloatParameter m_Hue        = new ClampedFloatParameter(0, -1, 1);
        public ClampedFloatParameter m_Saturation = new ClampedFloatParameter(0, -1, 1);
        
        public GrainTexParameter     m_GainTex    = new GrainTexParameter(GrainPass.GrainTex.Thin_A, false);
        public ClampedFloatParameter m_Alpha      = new ClampedFloatParameter(1, 0, 1);
        
        //public ClampedFloatParameter m_Contrast   = new ClampedFloatParameter(0, -1, 1);
        public ColorParameter        m_Color  = new ColorParameter(new Color(1, 1, 1, 0));
        
        // =======================================================================
        [Serializable]
        public class GrainTexParameter : VolumeParameter<GrainPass.GrainTex>
        {
            public GrainTexParameter(GrainPass.GrainTex value, bool overrideState) : base(value, overrideState) { }
        }
        
        // =======================================================================
        public bool IsActive() => m_Grain.value > 0f;

        public bool IsTileCompatible() => true;
    }
}