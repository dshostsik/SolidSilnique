#nullable enable

using System;
using Microsoft.Xna.Framework;

namespace SolidSilnique.Core.Components
{
    public class LightsManagerComponent : Component
    {
        /// <summary>
        /// Maximum length of arrays <see cref="PointLights"/> and <see cref="Spotlights"/>
        /// </summary>
        private static readonly int _maximumAmountOfInstances = 10;

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
            _directionalLight = new DirectionalLight(new Vector3(1, -1, 0));

            _pointLights = new PointLight[_maximumAmountOfInstances];
            _spotlights = new Spotlight[_maximumAmountOfInstances];
        }

        /// <summary>
        /// Use this method to send uniforms that will not be updated ever.
        /// </summary>
        /// <exception cref="Diagnostics.UniformNotFoundException"> if requested uniform was not found</exception>
        public override void Start()
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// Use this method to send uniforms that will be updated in every frame.
        /// </summary>
        /// <exception cref="Diagnostics.UniformNotFoundException"> if requested uniform was not found</exception>
        public override void Update()
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// Use this method to send uniforms that will not be updated in every frame but only in one frame.
        /// </summary>
        /// <exception cref="Diagnostics.UniformNotFoundException"> if requested uniform was not found</exception>
        public void UpdateNonConstantUniforms()
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// Get a point light by index and edit it if needed
        /// </summary>
        /// <param name="index">Index of the requested point light source</param>
        /// <returns><see cref="PointLight"/> object that was requested or <c>null</c> if nothing was found in the array</returns>
        /// <exception cref="ArgumentException"> if the index is out of range</exception>
        public PointLight? GetPointLight(int index)
        {
            try
            {
                return _pointLights[index];
            }
            catch (ArgumentOutOfRangeException e)
            {
                Console.WriteLine(e);
                throw new ArgumentException("Index out of range.");
            }
        }

        /// <summary>
        /// Get a spotlight by index and edit it if needed
        /// </summary>
        /// <param name="index">Index of the requested spotlight source</param>
        /// <returns><see cref="Spotlight"/> object that was requested or <c>null</c> if nothing was found in the array</returns>
        /// <exception cref="ArgumentException"> if the index is out of range</exception>
        public Spotlight? GetSpotlight(int index)
        {
            try
            {
                return _spotlights[index];
            }
            catch (ArgumentOutOfRangeException e)
            {
                Console.WriteLine(e);
                throw new ArgumentException("Index out of range.");
            }
        }

        /// <summary>
        /// Add your own point light source to the array.
        /// </summary>
        /// <param name="pointLight">New <see cref="PointLight"/> object to be added</param>
        /// <exception cref="ArgumentException"> if there are already 10 objects of that type</exception>
        public void AddPointLight(PointLight pointLight)
        {
            if (PointLight.Instances >= 10)
            {
                throw new ArgumentException("Maximum amount of point lights reached.");
            }
            
            _pointLights[pointLight.Index] = pointLight;
            UpdateNonConstantUniforms();
        }
        
        /// <summary>
        /// Add your own spotlight source to the array.
        /// </summary>
        /// <param name="spotlight">New <see cref="Spotlight"/> object to be added</param>
        /// <exception cref="ArgumentException"> if there are already 10 objects of that type</exception>
        public void AddSpotLight(Spotlight spotlight)
        {
            if (Spotlight.SpotlightInstances >= 10)
            {
                throw new ArgumentException("Maximum amount of point lights reached.");
            }
            
            _spotlights[spotlight.Index] = spotlight;
            UpdateNonConstantUniforms();
        }
        
        /// <summary>
        /// Create a new point light source with default values.
        /// </summary>
        /// <param name="linear">Linear coefficient</param>
        /// <param name="quadratic">Quadratic coefficient</param>
        /// <param name="constant">Constant coefficient (always equals to 1)</param>
        /// <exception cref="ArgumentException"> if there are already 10 objects of that type</exception>
        public void CreateNewPointLight(float linear = 0.022f, float quadratic = 0.0019f, float constant = 1)
        {
            if (PointLight.Instances >= 10)
            {
                throw new ArgumentException("Maximum amount of point lights reached.");
            }
            
            PointLight newlight = new PointLight(linear, quadratic, constant);
            PointLights[newlight.Index] = newlight;
            UpdateNonConstantUniforms();
        }
        
        /// <summary>
        /// Create a new spotlight source with default values.
        /// </summary>
        /// <param name="linear">Linear coefficient</param>
        /// <param name="quadratic">Quadratic coefficient</param>
        /// <param name="constant">Constant coefficient (always equals to 1)</param>
        /// <param name="direction">Direction of the light</param>
        /// <param name="cutoff">Inner cutoff angle</param>
        /// <param name="outerCutoff">Outer cutoff angle</param>
        /// <exception cref="ArgumentException"> if there are already 10 objects of that type</exception>
        public void CreateNewSpotLight(float linear = 0.022f, float quadratic = 0.0019f, float constant = 1,
            Vector3 direction = default, float cutoff = 5.5f, float outerCutoff = 7.5f)
        {
            if (Spotlight.SpotlightInstances >= 10)
            {
                throw new ArgumentException("Maximum amount of point lights reached.");
            }
            
            Spotlight newlight = new Spotlight(linear, quadratic, constant, direction, cutoff, outerCutoff);
            Spotlights[newlight.Index] = newlight;
            UpdateNonConstantUniforms();
        }
    }
}