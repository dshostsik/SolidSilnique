using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SolidSilnique.Core.Diagnostics;

namespace SolidSilnique.Core
{
    /// <summary>
    /// Shader handler class
    /// </summary>
    public class Shader
    {
        /// <summary>
        /// Shader effect object.
        /// </summary>
        private readonly Effect effect;

        /// <summary>
        /// Effect getter.
        /// </summary>
        public Effect Effect {
            get => effect;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="path">Path to .fx shader file relatively to "Content" folder</param>
        /// <param name="GraphicsDevice">Object of GraphicsDevice class</param>
        /// <param name="game">Object containing the main game loop that's responsible for loading content</param>
        /// <param name="defaultTechnique">Name of default technique to be used</param>
        /// <throws>ArgumentException if the shader file extension is not .fx OR if the shader technique was not found in the shader</throws>
        public Shader(string path, GraphicsDevice GraphicsDevice, Game game, string defaultTechnique)
        {
            effect = new BasicEffect(GraphicsDevice);
            effect = game.Content.Load<Effect>(path);
            try
            {
                effect.CurrentTechnique = effect.Techniques[defaultTechnique];
            }
            catch (Exception e)
            {
                throw new ArgumentException("There is no such technique in this shader. Check the spelling!");
            }
        }

        /// <summary>
        /// Looks for uniform in shader and sets it to the value
        /// </summary>
        /// <param name="nameOfUniform">Name of requested uniform</param>
        /// <param name="value">Value of uniform to be set</param>
        /// <exception cref="UniformNotFoundException">if uniform was not found in shader</exception>
        public void SetUniform(string nameOfUniform, Vector2 value)
        {
            try
            {
                effect.Parameters[nameOfUniform].SetValue(value);
            }
            catch (NullReferenceException)
            {
                throw new UniformNotFoundException(nameOfUniform,
                    " uniform not found!\n Try to check if requested uniform is absent in shader or not in use (might be deprecated while shader optimization)");
            }
        }

        /// <summary>
        /// Looks for uniform in shader and sets it to the value
        /// </summary>
        /// <param name="nameOfUniform">Name of requested uniform</param>
        /// <param name="values">Value of uniform to be set</param>
        /// <exception cref="UniformNotFoundException">if uniform was not found in shader</exception>
        public void SetUniform(string nameOfUniform, Vector2[] values)
        {
            try
            {
                effect.Parameters[nameOfUniform].SetValue(values);
            }
            catch (NullReferenceException)
            {
                throw new UniformNotFoundException(nameOfUniform,
                    " uniform not found!\n Try to check if requested uniform is absent in shader or not in use (might be deprecated while shader optimization)");
            }
        }

        /// <summary>
        /// Looks for uniform in shader and sets it to the value
        /// </summary>
        /// <param name="nameOfUniform">Name of requested uniform</param>
        /// <param name="value">Value of uniform to be set</param>
        /// <exception cref="UniformNotFoundException">if uniform was not found in shader</exception>
        public void SetUniform(string nameOfUniform, Vector3 value)
        {
            try
            {
                effect.Parameters[nameOfUniform].SetValue(value);
            }
            catch (NullReferenceException)
            {
                throw new UniformNotFoundException(nameOfUniform,
                    " uniform not found!\n Try to check if requested uniform is absent in shader or not in use (might be deprecated while shader optimization)");
            }
        }

        /// <summary>
        /// Looks for uniform in shader and sets it to the value
        /// </summary>
        /// <param name="nameOfUniform">Name of requested uniform</param>
        /// <param name="values">Value of uniform to be set</param>
        /// <exception cref="UniformNotFoundException">if uniform was not found in shader</exception>
        public void SetUniform(string nameOfUniform, Vector3[] values)
        {
            try
            {
                effect.Parameters[nameOfUniform].SetValue(values);
            }
            catch (NullReferenceException)
            {
                throw new UniformNotFoundException(nameOfUniform,
                    " uniform not found!\n Try to check if requested uniform is absent in shader or not in use (might be deprecated while shader optimization)");
            }
        }

        /// <summary>
        /// Looks for uniform in shader and sets it to the value
        /// </summary>
        /// <param name="nameOfUniform">Name of requested uniform</param>
        /// <param name="value">Value of uniform to be set</param>
        /// <exception cref="UniformNotFoundException">if uniform was not found in shader</exception>
        public void SetUniform(string nameOfUniform, Vector4 value)
        {
            try
            {
                effect.Parameters[nameOfUniform].SetValue(value);
            }
            catch (NullReferenceException)
            {
                throw new UniformNotFoundException(nameOfUniform,
                    " uniform not found!\n Try to check if requested uniform is absent in shader or not in use (might be deprecated while shader optimization)");
            }
        }

        /// <summary>
        /// Looks for uniform in shader and sets it to the value
        /// </summary>
        /// <param name="nameOfUniform">Name of requested uniform</param>
        /// <param name="values">Value of uniform to be set</param>
        /// <exception cref="UniformNotFoundException">if uniform was not found in shader</exception>
        public void SetUniform(string nameOfUniform, Vector4[] values)
        {
            try
            {
                effect.Parameters[nameOfUniform].SetValue(values);
            }
            catch (NullReferenceException)
            {
                throw new UniformNotFoundException(nameOfUniform,
                    " uniform not found!\n Try to check if requested uniform is absent in shader or not in use (might be deprecated while shader optimization)");
            }
        }

        /// <summary>
        /// Looks for uniform in shader and sets it to the value
        /// </summary>
        /// <param name="nameOfUniform">Name of requested uniform</param>
        /// <param name="value">Value of uniform to be set</param>
        /// <exception cref="UniformNotFoundException">if uniform was not found in shader</exception>
        public void SetUniform(string nameOfUniform, Matrix value)
        {
            try
            {
                effect.Parameters[nameOfUniform].SetValue(value);
            }
            catch (NullReferenceException)
            {
                throw new UniformNotFoundException(nameOfUniform,
                    " uniform not found!\n Try to check if requested uniform is absent in shader or not in use (might be deprecated while shader optimization)");
            }
        }

        /// <summary>
        /// Looks for uniform in shader and sets it to the value
        /// </summary>
        /// <param name="nameOfUniform">Name of requested uniform</param>
        /// <param name="values">Value of uniform to be set</param>
        /// <exception cref="UniformNotFoundException">if uniform was not found in shader</exception>
        public void SetUniform(string nameOfUniform, Matrix[] values)
        {
            try
            {
                effect.Parameters[nameOfUniform].SetValue(values);
            }
            catch (NullReferenceException)
            {
                throw new UniformNotFoundException(nameOfUniform,
                    " uniform not found!\n Try to check if requested uniform is absent in shader or not in use (might be deprecated while shader optimization)");
            }
        }

        /// <summary>
        /// Looks for uniform in shader and sets it to the value
        /// </summary>
        /// <param name="nameOfUniform">Name of requested uniform</param>
        /// <param name="value">Value of uniform to be set</param>
        /// <exception cref="UniformNotFoundException">if uniform was not found in shader</exception>
        public void SetUniform(string nameOfUniform, Texture value)
        {
            try
            {
                effect.Parameters[nameOfUniform].SetValue(value);
            }
            catch (NullReferenceException)
            {
                throw new UniformNotFoundException(nameOfUniform,
                    " uniform not found!\n Try to check if requested uniform is absent in shader or not in use (might be deprecated while shader optimization)");
            }
        }

        /// <summary>
        /// Looks for uniform in shader and sets it to the value
        /// </summary>
        /// <param name="nameOfUniform">Name of requested uniform</param>
        /// <param name="value">Value of uniform to be set</param>
        /// <exception cref="UniformNotFoundException">if uniform was not found in shader</exception>
        public void SetUniform(string nameOfUniform, int value)
        {
            try
            {
                effect.Parameters[nameOfUniform].SetValue(value);
            }
            catch (NullReferenceException)
            {
                throw new UniformNotFoundException(nameOfUniform,
                    " uniform not found!\n Try to check if requested uniform is absent in shader or not in use (might be deprecated while shader optimization)");
            }
        }

        /// <summary>
        /// Looks for uniform in shader and sets it to the value
        /// </summary>
        /// <param name="nameOfUniform">Name of requested uniform</param>
        /// <param name="values">Value of uniform to be set</param>
        /// <exception cref="UniformNotFoundException">if uniform was not found in shader</exception>
        public void SetUniform(string nameOfUniform, int[] values)
        {
            try
            {
                effect.Parameters[nameOfUniform].SetValue(values);
            }
            catch (NullReferenceException)
            {
                throw new UniformNotFoundException(nameOfUniform,
                    " uniform not found!\n Try to check if requested uniform is absent in shader or not in use (might be deprecated while shader optimization)");
            }
        }

        /// <summary>
        /// Looks for uniform in shader and sets it to the value
        /// </summary>
        /// <param name="nameOfUniform">Name of requested uniform</param>
        /// <param name="value">Value of uniform to be set</param>
        /// <exception cref="UniformNotFoundException">if uniform was not found in shader</exception>
        public void SetUniform(string nameOfUniform, float value)
        {
            try
            {
                effect.Parameters[nameOfUniform].SetValue(value);
            }
            catch (NullReferenceException)
            {
                throw new UniformNotFoundException(nameOfUniform,
                    " uniform not found!\n Try to check if requested uniform is absent in shader or not in use (might be deprecated while shader optimization)");
            }
        }

        /// <summary>
        /// Looks for uniform in shader and sets it to the value
        /// </summary>
        /// <param name="nameOfUniform">Name of requested uniform</param>
        /// <param name="values">Value of uniform to be set</param>
        /// <exception cref="UniformNotFoundException">if uniform was not found in shader</exception>
        public void SetUniform(string nameOfUniform, float[] values)
        {
            try
            {
                effect.Parameters[nameOfUniform].SetValue(values);
            }
            catch (NullReferenceException)
            {
                throw new UniformNotFoundException(nameOfUniform,
                    " uniform not found!\n Try to check if requested uniform is absent in shader or not in use (might be deprecated while shader optimization)");
            }
        }

        /// <summary>
        /// Looks for uniform in shader and sets it to the value
        /// </summary>
        /// <param name="nameOfUniform">Name of requested uniform</param>
        /// <param name="value">Value of uniform to be set</param>
        /// <exception cref="UniformNotFoundException">if uniform was not found in shader</exception>
        public void SetUniform(string nameOfUniform, bool value)
        {
            try
            {
                effect.Parameters[nameOfUniform].SetValue(value);
            }
            catch (NullReferenceException)
            {
                throw new UniformNotFoundException(nameOfUniform,
                    " uniform not found!\n Try to check if requested uniform is absent in shader or not in use (might be deprecated while shader optimization)");
            }
        }

        /// <summary>
        /// Looks for uniform in shader and sets it to the value
        /// </summary>
        /// <param name="nameOfUniform">Name of requested uniform</param>
        /// <param name="value">Value of uniform to be set</param>
        /// <exception cref="UniformNotFoundException">if uniform was not found in shader</exception>
        public void SetUniform(string nameOfUniform, Quaternion value)
        {
            try
            {
                effect.Parameters[nameOfUniform].SetValue(value);
            }
            catch (NullReferenceException)
            {
                throw new UniformNotFoundException(nameOfUniform,
                    " uniform not found!\n Try to check if requested uniform is absent in shader or not in use (might be deprecated while shader optimization)");
            }
        }

        /// <summary>
        /// Method for hot-swapping shader techniques.
        /// </summary>
        /// <param name="techniqueName">Name of requested technique</param>
        /// <throws>ArgumentException if the technique was not found in the shader</throws>
        public void SwapTechnique(string techniqueName)
        {
            try
            {
                effect.CurrentTechnique = effect.Techniques[techniqueName];
            } catch (ArgumentException e)
            {
                throw new ArgumentException("There is no such technique in this shader. Check the spelling!");
            }
        }

        /// <summary>
        /// Method used for debugging purposes. Prints all uniforms in shader to the console.
        /// </summary>
        [System.Diagnostics.Conditional("DEBUG")]
        public void DiagnoseUniforms()
        {
            foreach (var parameter in effect.Parameters)
            {
                Console.WriteLine($"Parameter: {parameter.Name}, Type: {parameter.ParameterType}");
            }
        }
    }
}