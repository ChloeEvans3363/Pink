using UnityEngine;

//  VolFx © NullTale - https://twitter.com/NullTale/
namespace VolFx
{
    [ShaderName("Hidden/VolFx/Scanlines")]
    public class ScanlinesPass : VolFxProc.Pass
    {
        private static readonly int s_Scanlines = Shader.PropertyToID("_Scanlines");

        [CurveRange]
        public AnimationCurve   _move = new AnimationCurve(new Keyframe(0, .5f, 0, 0), new Keyframe(1f, 1f, 0, 0)) { postWrapMode = WrapMode.PingPong };
        public float            _movePeriod = 5f;
        [CurveRange]
        public AnimationCurve   _flicker = new AnimationCurve(new Keyframe(0, 0, 0, 0), new Keyframe(.5f, 1f, 0, 0), new Keyframe(1f, 0, 0, 0)) { postWrapMode = WrapMode.PingPong };
        public  float           _flickerPeriod      = 3f;
        public  float           _flickerToIntencity = 0.3f;
        private float           _offset;
        public  Optional<float> _forceCount = new Optional<float>(570f, false);
        
        // =======================================================================
        public override void Init()
        {
            _offset = 0;
        }

        public override bool Validate(Material mat)
        {
            var settings = Stack.GetComponent<ScanlinesVol>();

            if (settings.IsActive() == false)
                return false;
            
            var flicker   = _flicker.Evaluate(Time.time / _flickerPeriod) * settings.m_Flicker.value;
            var intensity = settings.m_Intensity.value + flicker * _flickerToIntencity;
            _offset += _move.Evaluate(Time.time / _movePeriod) * settings.m_Speed.value;
            
            while (_offset > 1f)
                _offset -= 1f;
            
            mat.SetVector(s_Scanlines, new Vector4(_forceCount.Enabled ? _forceCount.Value : settings.m_Count.value, intensity, flicker, _offset));
            
            return true;
        }
    }
}