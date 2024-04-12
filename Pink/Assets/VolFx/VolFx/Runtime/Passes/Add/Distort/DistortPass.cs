using UnityEngine;

//  VolFx © NullTale - https://twitter.com/NullTale/
namespace VolFx
{
    [ShaderName("Hidden/VolFx/Distort")]
    public class DistortPass : VolFxProc.Pass
    {
        private static readonly int   s_Settings = Shader.PropertyToID("_Settings");
        private static readonly int   s_Weight = Shader.PropertyToID("_Weight");
        
        private                 float _offset;
        public float            _motionMul = 1f;

        // =======================================================================
        public override bool Validate(Material mat)
        {
            var settings = Stack.GetComponent<DistortVol>();

            if (settings.IsActive() == false)
                return false;

            var sharpness = settings.m_Value.value * .3f;
            var tiling    = settings.m_Tiling.value * 200f;
            var angle     = (settings.m_Angle.value + 90f) * Mathf.Deg2Rad;
            _offset += settings.m_Motion.value * settings.m_Tiling.value * Time.deltaTime * 70f * _motionMul;
            
            mat.SetVector(s_Settings, new Vector4(sharpness, tiling, _offset, angle));
            mat.SetFloat(s_Weight, settings.m_Weight.value);
            
            return true;
        }
    }
}