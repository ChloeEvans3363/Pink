using UnityEngine;

//  VolFx © NullTale - https://twitter.com/NullTale/
namespace VolFx
{
    [ShaderName("Hidden/VolFx/Sharpen")]
    public class SharpenPass : VolFxProc.Pass
    {
        private static readonly int s_Center    = Shader.PropertyToID("_Center");
        private static readonly int s_Side      = Shader.PropertyToID("_Side");
        private static readonly int s_Color     = Shader.PropertyToID("_Color");
        private static readonly int s_Thickness = Shader.PropertyToID("_Thickness");

        public                  AnimationCurve _lerp  = AnimationCurve.Linear(0, 0, 1, 1);
        public                  Vector2        _range = new Vector2(0, 3f);
        private                 bool           _isBox;

        // =======================================================================
        public override bool Validate(Material mat)
        {
            var settings = Stack.GetComponent<SharpenVol>();

            if (settings.IsActive() == false)
                return false;
            
            var steps  = mat.IsKeywordEnabled("BOX") ? 8f : 4f;
            var impact = _range.x + _range.y * _lerp.Evaluate(settings.m_Impact.value);
            mat.SetFloat(s_Center, 1f + impact * steps);
            mat.SetFloat(s_Side, -impact);
            
            var apect  = Screen.width / (float)Screen.height;
            var thickness = settings.m_Thikness.overrideState 
                ? new Vector4(settings.m_Thikness.value * apect * 0.003f, settings.m_Thikness.value * 0.003f) 
                : new Vector4(1f / (float)(Screen.width), 1f / (float)(Screen.height));
            mat.SetVector(s_Thickness, thickness);
            
            mat.SetColor(s_Color, settings.m_Tint.value);
            
            return true;
        }
    }
}