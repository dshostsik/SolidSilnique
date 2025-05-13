using Microsoft.Xna.Framework;

namespace SolidSilnique.Core.Components
{
    public class LightsManagerComponent : Component
    {

        /// <summary>
        /// Maximum length of arrays <see cref="PointLights"/> and <see cref="Spotlights"/>
        /// </summary>
        private static int _maximumAmountOfInstances = 10;
        
        /// <summary>
        /// Object of class <see cref="DirectionalLight"/> representing global illumination.
        /// </summary>
        private DirectionalLight _directionalLight;

        /// <summary>
        /// Position of the sun. Used for computing shadows.
        /// </summary>
        private Vector3 _directionalLightPosition;

        /// <summary>
        /// Array of objects of class <see cref="PointLight"/> representing point light sources.
        /// </summary>
        private readonly PointLight[] _pointLights;
        
        /// <summary>
        /// Array of objects of class <see cref="Spotlight"/> representing spotlight sources.
        /// </summary>
        private readonly Spotlight[] _spotlights;

        /// <summary>
        /// Default ambient color. Used for instancing the light source. <p><b>Feel free to change it with another value by changing light objects fields.</b></p>
        /// </summary>
        private Vector4 _defaultAmbientColor;

        /// <summary>
        /// Default diffuse color. Used for instancing the light source. <p><b>Feel free to change it with another value by changing light objects fields.</b></p>
        /// </summary>
        private Vector4 _defaultDiffuseColor;

        /// <summary>
        /// Default specular color. Used for instancing the light source. <p><b>Feel free to change it with another value by changing light objects fields.</b></p>
        /// </summary>
        private Vector4 _defaultSpecularColor;

        /// <summary>
        /// Object of class <see cref="DirectionalLight"/> representing global illumination.
        /// </summary>
        public DirectionalLight DirectionalLight
        {
            get => _directionalLight;
            set => _directionalLight = value;
        }

        /// <summary>
        /// Position of the directional light. Used for computing shadows.
        /// </summary>
        public Vector3 DirectionalLightPosition
        {
            get => _directionalLightPosition;
            set => _directionalLightPosition = value;
        }
        
        /// <summary>
        /// Array of objects of class <see cref="PointLight"/> representing point light sources.
        /// </summary>
        public PointLight[] PointLights => _pointLights;

        /// <summary>
        /// Array of objects of class <see cref="Spotlight"/> representing spotlight sources.
        /// </summary>
        public Spotlight[] Spotlights => _spotlights;

        public LightsManagerComponent()
        {
            _defaultAmbientColor = new Vector4(0.1f, 0.1f, 0.1f, 1.0f);
            _defaultDiffuseColor = new Vector4(0.8f, 0.8f, 0.8f, 1.0f);
            _defaultSpecularColor = new Vector4(1.0f, 1.0f, 1.0f, 1.0f);

            _directionalLight = new DirectionalLight(new Vector3(1, -1, 0));
            
            _pointLights = new PointLight[_maximumAmountOfInstances];
            _spotlights = new Spotlight[_maximumAmountOfInstances];
        }

        public override void Start()
        {
            throw new System.NotImplementedException();
        }

        public override void Update()
        {
            throw new System.NotImplementedException();
        }
    }
}