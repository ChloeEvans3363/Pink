using System;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

//  VolFx © NullTale - https://twitter.com/NullTale/
namespace VolFx
{
    [ShaderName("Hidden/VolFx/Grain")]
    public class GrainPass : VolFxProc.Pass
    {
        private static readonly int s_Intensity   = Shader.PropertyToID("_Intensity");
        private static readonly int s_ResponseTex = Shader.PropertyToID("_ResponseTex");
        private static readonly int s_Tiling      = Shader.PropertyToID("_Tiling");
        private static readonly int s_GrainTex    = Shader.PropertyToID("_GrainTex");
        private static readonly int s_Grain       = Shader.PropertyToID("_Grain");
        private static readonly int s_Tint        = Shader.PropertyToID("_Tint");
        private static readonly int s_Adj         = Shader.PropertyToID("_Adj");

        [Tooltip("Default grain tex if override is not set")]
        public                  GrainTex        _grain = GrainTex.Medium_A;
        [HideInInspector]
        public                  Texture2D[]     _grainTex;
        public                  float           _grainClamp = .5f;
        public                  Optional<float> _fps        = new Optional<float>(25f, false);
        public                  bool            _static;
        private                 Texture2D       _impactTex;
        private                 int             _frame;
        private                 Vector4         _tiling;

        // =======================================================================
        public enum GrainTex
        {
            Large_A,
            Large_B,
            Medium_A,
            Medium_B,
            Medium_C,
            Medium_D,
            Medium_E,
            Medium_F,
            Thin_A,
            Thin_B,
        }
        
        // =======================================================================
        public override void Init()
        {
            _tiling = new Vector4(Screen.width / (float)_grainTex[0].width, Screen.height / (float)_grainTex[0].height, 0f, 0f);
        }

        public override bool Validate(Material mat)
        {
            var settings = Stack.GetComponent<GrainVol>();

            if (settings.IsActive() == false)
                return false;

            var isStatic = _static;
            if (_fps.Enabled)
            {
                var curFrame = Mathf.FloorToInt(Time.unscaledTime / (1f / _fps.Value));
                if (_frame != curFrame)
                {
                    _frame = curFrame;
                }
                else
                {
                    isStatic = true;
                }
            }
            
            var grainTex = _grainTex[(int)(settings.m_GainTex.overrideState ? settings.m_GainTex.value : _grain)];

            if (isStatic == false)
                _tiling = new Vector4(Screen.width / (float)grainTex.width, Screen.height / (float)grainTex.height, Random.value, Random.value);

            mat.SetTexture(s_ResponseTex, settings.m_Response.value.GetTexture(ref _impactTex));
            mat.SetTexture(s_GrainTex, grainTex);
            mat.SetVector(s_Grain, new Vector4(_grainClamp, (1f / (1f - _grainClamp)) * settings.m_Grain.value * 3f));
            mat.SetVector(s_Tiling, _tiling);
            mat.SetVector(s_Adj, new Vector4(settings.m_Hue.value, settings.m_Saturation.value + 1f, settings.m_Brightness.value, 1f - settings.m_Alpha.value));
            mat.SetVector(s_Tint, settings.m_Color.value);
            
            return true;
        }

        protected override bool _editorValidate => _grainTex == null || _grainTex.Length == 0 || _grainTex.Any(n => n == null);

        protected override void _editorSetup(string folder, string asset)
        {
#if UNITY_EDITOR
            _grainTex = Enum
                        .GetNames(typeof(GrainTex))
                        .Select(n => UnityEditor.AssetDatabase.LoadAssetAtPath<Texture2D>($"{folder}\\Grain\\Grain_{n}.png"))
                        .ToArray();
            
            UnityEditor.EditorUtility.SetDirty(this);
#endif
        }
    }
}