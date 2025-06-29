﻿using System;
using Microsoft.Xna.Framework;
using SolidSilnique.Core.Diagnostics;

namespace SolidSilnique.Core
{
    public class DirectionalLight : Light
    {
        private Vector3 _direction;

        public DirectionalLight(Vector3 direction)
        {
            _direction = direction;
            AmbientColor = new Vector4(.1f, .1f, .1f, .0f);
            DiffuseColor = new Vector4(.8f, .8f, .8f, .0f);
            SpecularColor = new Vector4(1.0f, 1.0f, 1.0f, .0f);
        }

        public Vector3 Direction
        {
            get => _direction;
            set => _direction = value;
        }

        public override void SendToShader(Shader shader)
        {
            try
            {
                shader.SetUniform("dirlightEnabled", Enabled);
                shader.SetUniform("dirlight_direction", Direction);
                shader.SetUniform("dirlight_ambientColor", AmbientColor);
                shader.SetUniform("dirlight_diffuseColor", DiffuseColor);
                shader.SetUniform("dirlight_specularColor", SpecularColor);
            }
            catch (UniformNotFoundException e)
            {
                throw new UniformNotFoundException(e.Message, " error source: DirectionalLight.cs");
            }
        }

        public override void Start()
        {
            throw new NotImplementedException();
        }

        public override void Update()
        {
            throw new NotImplementedException();
        }
    }
}