#nullable enable

using System;
using Microsoft.Xna.Framework;
using SolidSilnique.Core.Diagnostics;

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
        /// Object of <see cref="Shader"/> class used for rendering. Cannot be null and must be injected.
        /// </summary>
        private Shader _shader;

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


        /// <summary>
        /// Object of <see cref="Shader"/> class used for rendering. Cannot be null and must be injected.
        /// </summary>
        /// <exception cref="ArgumentException"> if <c>null</c> is passed. Shader cannot be null</exception>
        public Shader Shader
        {
            get => _shader;
            set => _shader = value ?? throw new ArgumentException("Shader cannot be null.");
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="shader">Object of <see cref="Shader"/> class that will be used for rendering</param>
        public LightsManagerComponent(Shader shader)
        {
            _directionalLight = new DirectionalLight(new Vector3(1, -1, 0));

            _pointLights = new PointLight[_maximumAmountOfInstances];
            _spotlights = new Spotlight[_maximumAmountOfInstances];

            _shader = shader;
        }

        /// <summary>
        /// Use this method to send uniforms that will not be updated ever.
        /// </summary>
        /// <exception cref="Diagnostics.UniformNotFoundException"> if requested uniform was not found</exception>
        public override void Start()
        {
            //throw new System.NotImplementedException();
            try
            {
                _shader.SetUniform("dirlightEnabled", _directionalLight.Enabled);
                _shader.SetUniform("dirlight_direction", _directionalLight.Direction);
                _shader.SetUniform("dirlight_ambientColor", _directionalLight.AmbientColor);
                _shader.SetUniform("dirlight_diffuseColor", _directionalLight.DiffuseColor);
                _shader.SetUniform("dirlight_specularColor", _directionalLight.SpecularColor);

                int[] pointLightEnabled = new int[PointLight.PointLightInstances];
                Vector3[] pointLightPosition = new Vector3[PointLight.PointLightInstances];
                Vector4[] pointLightAmbientColor = new Vector4[PointLight.PointLightInstances];
                Vector4[] pointLightDiffuseColor = new Vector4[PointLight.PointLightInstances];
                Vector4[] pointLightSpecularColor = new Vector4[PointLight.PointLightInstances];
                float[] pointLightLinearAttenuation = new float[PointLight.PointLightInstances];
                float[] pointLightQuadraticAttenuation = new float[PointLight.PointLightInstances];
                float[] pointLightConstant = new float[PointLight.PointLightInstances];

                if (null != _pointLights[0])
                {
                    for (int i = 0; i < PointLight.PointLightInstances; i++)
                    {
                        if (_pointLights[i] == null) continue;
                        pointLightEnabled[i] = _pointLights[i].Enabled;
                        pointLightAmbientColor[i] = _pointLights[i].AmbientColor;
                        pointLightDiffuseColor[i] = _pointLights[i].DiffuseColor;
                        pointLightSpecularColor[i] = _pointLights[i].SpecularColor;
                        pointLightPosition[i] = _pointLights[i].gameObject.transform.position;
                        pointLightLinearAttenuation[i] = _pointLights[i].Linear;
                        pointLightQuadraticAttenuation[i] = _pointLights[i].Quadratic;
                        pointLightConstant[i] = _pointLights[i].Constant;
                    }
                }

                _shader.SetUniform("pointlightEnabled", pointLightEnabled);
                _shader.SetUniform("pointlight_position", pointLightPosition);
                _shader.SetUniform("pointlight_ambientColor", pointLightAmbientColor);
                _shader.SetUniform("pointlight_diffuseColor", pointLightDiffuseColor);
                _shader.SetUniform("pointlight_specularColor", pointLightSpecularColor);
                _shader.SetUniform("pointlight_linearAttenuation", pointLightLinearAttenuation);
                _shader.SetUniform("pointlight_quadraticAttenuation", pointLightQuadraticAttenuation);
                _shader.SetUniform("pointlight_constant", pointLightConstant);


                int[] spotlightEnabled = new int[Spotlight.SpotlightInstances];
                Vector3[] spotlightPosition = new Vector3[Spotlight.SpotlightInstances];
                Vector4[] spotlightAmbientColor = new Vector4[Spotlight.SpotlightInstances];
                Vector4[] spotlightDiffuseColor = new Vector4[Spotlight.SpotlightInstances];
                Vector4[] spotlightSpecularColor = new Vector4[Spotlight.SpotlightInstances];
                float[] spotlightLinearAttenuation = new float[Spotlight.SpotlightInstances];
                float[] spotlightQuadraticAttenuation = new float[Spotlight.SpotlightInstances];
                float[] spotlightConstant = new float[Spotlight.SpotlightInstances];
                float[] spotlightInnerCut = new float[Spotlight.SpotlightInstances];
                float[] spotlightOuterCut = new float[Spotlight.SpotlightInstances];

                if (null != _spotlights[0])
                {
                    for (int i = 0; i < Spotlight.SpotlightInstances; i++)
                    {
                        spotlightEnabled[i] = _spotlights[i].Enabled;
                        spotlightAmbientColor[i] = _spotlights[i].AmbientColor;
                        spotlightDiffuseColor[i] = _spotlights[i].DiffuseColor;
                        spotlightSpecularColor[i] = _spotlights[i].SpecularColor;
                        spotlightPosition[i] = _spotlights[i].gameObject.transform.position;
                        spotlightLinearAttenuation[i] = _spotlights[i].Linear;
                        spotlightQuadraticAttenuation[i] = _spotlights[i].Quadratic;
                        spotlightConstant[i] = _spotlights[i].Constant;
                        spotlightInnerCut[i] = _spotlights[i].InnerCut;
                        spotlightOuterCut[i] = _spotlights[i].OuterCut;
                    }
                }

                _shader.SetUniform("spotlightEnabled", spotlightEnabled);
                _shader.SetUniform("spotlight_position", spotlightPosition);
                _shader.SetUniform("spotlight_ambientColor", spotlightAmbientColor);
                _shader.SetUniform("spotlight_diffuseColor", spotlightDiffuseColor);
                _shader.SetUniform("spotlight_specularColor", spotlightSpecularColor);
                _shader.SetUniform("spotlight_linearAttenuation", spotlightLinearAttenuation);
                _shader.SetUniform("spotlight_quadraticAttenuation", spotlightQuadraticAttenuation);
                _shader.SetUniform("spotlight_constant", spotlightConstant);
                _shader.SetUniform("spotlight_innerCut", spotlightInnerCut);
                _shader.SetUniform("spotlight_outerCut", spotlightOuterCut);
            }
            catch (UniformNotFoundException e)
            {
                throw new UniformNotFoundException(e.Message, " error source: LightsManagerComponent.Start()");
            }
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
            return
                index < 0 || index >= _pointLights.Length
                    ? throw new ArgumentException($"Index {index} is out of range (0..{_pointLights.Length - 1}).")
                    : _pointLights[index];
        }

        /// <summary>
        /// Get a spotlight by index and edit it if needed
        /// </summary>
        /// <param name="index">Index of the requested spotlight source</param>
        /// <returns><see cref="Spotlight"/> object that was requested or <c>null</c> if nothing was found in the array</returns>
        /// <exception cref="ArgumentException"> if the index is out of range</exception>
        public Spotlight? GetSpotlight(int index)
        {
            return
                index < 0 || index >= _spotlights.Length
                    ? throw new ArgumentException($"Index {index} is out of range (0..{_spotlights.Length - 1}).")
                    : _spotlights[index];
        }

        /// <summary>
        /// Add your own point light source to the array.
        /// </summary>
        /// <param name="pointLight">New <see cref="PointLight"/> object to be added</param>
        /// <exception cref="ArgumentException"> if there are already 10 objects of that type</exception>
        public void AddPointLight(PointLight pointLight)
        {
            if (PointLight.PointLightInstances >= _maximumAmountOfInstances)
            {
                throw new ArgumentException("Maximum amount of point lights reached.");
            }

            _pointLights[pointLight.PointLightIndex] = pointLight;
            //UpdateNonConstantUniforms();
        }

        /// <summary>
        /// Add your own spotlight source to the array.
        /// </summary>
        /// <param name="spotlight">New <see cref="Spotlight"/> object to be added</param>
        /// <exception cref="ArgumentException"> if there are already 10 objects of that type</exception>
        public void AddSpotLight(Spotlight spotlight)
        {
            if (Spotlight.SpotlightInstances >= _maximumAmountOfInstances)
            {
                throw new ArgumentException("Maximum amount of point lights reached.");
            }

            _spotlights[spotlight.SpotlightIndex] = spotlight;
            //UpdateNonConstantUniforms();
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
            if (PointLight.PointLightInstances >= _maximumAmountOfInstances)
            {
                throw new ArgumentException("Maximum amount of point lights reached.");
            }

            PointLight newlight = new PointLight(linear, quadratic, constant);
            GameObject newObject = new GameObject("PointLight" + newlight.PointLightIndex);

            newlight.gameObject = newObject;
            newObject.AddComponent(newlight);

            PointLights[newlight.PointLightIndex] = newlight;
            //UpdateNonConstantUniforms();
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
            if (Spotlight.SpotlightInstances >= _maximumAmountOfInstances)
            {
                throw new ArgumentException("Maximum amount of point lights reached.");
            }

            Spotlight newlight = new Spotlight(linear, quadratic, constant, direction, cutoff, outerCutoff);
            GameObject newObject = new GameObject("SpotLight" + newlight.SpotlightIndex);

            newlight.gameObject = newObject;
            newObject.AddComponent(newlight);

            Spotlights[newlight.SpotlightIndex] = newlight;
            //UpdateNonConstantUniforms();
        }
    }
}