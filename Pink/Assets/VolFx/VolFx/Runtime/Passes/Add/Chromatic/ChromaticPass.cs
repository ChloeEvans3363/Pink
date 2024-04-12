using UnityEngine;

//  VolFx © NullTale - https://twitter.com/NullTale/
namespace VolFx
{
    [ShaderName("Hidden/VolFx/Chromatic")]
    public class ChromaticPass : VolFxProc.Pass
    {
        private static readonly int s_Weight    = Shader.PropertyToID("_Weight");
        private static readonly int s_Radial    = Shader.PropertyToID("_Radial");
        private static readonly int s_Intensity = Shader.PropertyToID("_Intensity");
        private static readonly int s_Alpha     = Shader.PropertyToID("_Alpha");
        private static readonly int s_R         = Shader.PropertyToID("_R");
        private static readonly int s_G         = Shader.PropertyToID("_G");
        private static readonly int s_B         = Shader.PropertyToID("_B");

        private float _angle;
        
        // =======================================================================
        public override void Init()
        {
            _material.SetVector(s_R, Vector2.left);
            _material.SetVector(s_G, Vector2.right);
            _material.SetVector(s_B, Vector2.up);
        }

        public override bool Validate(Material mat)
        {
            var settings = Stack.GetComponent<ChromaticVol>();

            if (settings.IsActive() == false)
                return false;
   
            mat.SetFloat(s_Intensity, settings._Intensity.value * 0.07f);
            
            //_angle += settings._Speed.value * Time.deltaTime;
            _angle %= (Mathf.PI * 2f);
            
            var step = (Mathf.PI * 2f) / 3f;
            mat.SetVector(s_R, (_angle + step * 1f).ToNormal());
            mat.SetVector(s_G, (_angle + step * 2f).ToNormal());
            mat.SetVector(s_B, (_angle + step * 3f).ToNormal());
            
            mat.SetFloat(s_Weight, settings._Weight.value);
            mat.SetFloat(s_Radial, settings._Radial.value);
            
            mat.SetFloat(s_Alpha, settings._Alpha.value >= 0f ? Mathf.Lerp(.3f, 3f, settings._Alpha.value) : Mathf.Lerp(0f, .3f, 1f + settings._Alpha.value));
            
            return true;
        }
    }
}