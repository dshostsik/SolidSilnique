using System;
using Microsoft.Xna.Framework;
using SolidSilnique.Core.Components;
using Vector3 = Microsoft.Xna.Framework.Vector3;

namespace SolidSilnique.Core
{
    /// <summary>
    /// Class representing camera
    /// </summary>
    public class Camera
    {
        private const float YAW = -90.0f;
        private const float PITCH = .0f;
        private const float ZOOM = 35.0f;
        private const float SENSE = 10.0f;
        private const float SPEED = 10f;

        public enum directions
        {
            FORWARD,
            BACKWARD,
            LEFT,
            RIGHT,
            UP,
            DOWN,
        }

        
        Vector3 Front;
        Vector3 Up;
        Vector3 Right;
        Vector3 WorldUp;

        public Vector3 CameraPosition { get { return cameraComponent.gameObject.transform.globalPosition; } }
        
        float Yaw;
        float Pitch;

        float MovementSpeed;
        float MouseSensitivity;
        public float Zoom;
        CameraComponent cameraComponent;

        public Camera(CameraComponent camComponent, float yaw = YAW, float pitch = PITCH)
        {
            cameraComponent = camComponent;
            //Position = position;
            WorldUp = Vector3.Up;
            Yaw = yaw;
            Pitch = pitch;

            // Pitch =
            //     -MathF.Acos(
            //         -position.Z /
            //         MathF.Sqrt(
            //             MathF.Pow(position.Z, 2)
            //             +
            //             MathF.Pow(position.Y, 2)
            //         )
            //     ) * 180 / MathHelper.Pi;

            Front = Vector3.Forward;
            MovementSpeed = SPEED;
            MouseSensitivity = SENSE;
            Zoom = ZOOM;
            UpdateCameraVectors();
        }

        public Matrix getViewMatrix()
        {
            return Matrix.CreateLookAt(CameraPosition, CameraPosition + Front, Up);
        }

        public void move(directions direction, float deltaTime)
        {
            float speed = MovementSpeed * deltaTime;
            Vector3 pos = CameraPosition;

			if (direction == directions.FORWARD) pos += Front * speed;
            if (direction == directions.BACKWARD) pos -= Front * speed;

            if (direction == directions.RIGHT) pos += Right * speed;
            if (direction == directions.LEFT) pos -= Right * speed;

			if (direction == directions.UP) pos += Vector3.Up * speed;
			if (direction == directions.DOWN) pos -= Vector3.Up * speed;

			cameraComponent.gameObject.transform.position = pos;
		}

        public void mouseMovement(float xOffset, float yOffset, float deltaTime, bool constrainPitch = true)
        {
            xOffset *= MouseSensitivity;
            yOffset *= MouseSensitivity;

            Yaw -= xOffset * (deltaTime / 1000.0f);
            Pitch -= yOffset * (deltaTime / 1000.0f);

            //Yaw += xOffset;
            //Pitch += yOffset;
            
            if (constrainPitch)
            {
                if (Pitch > 89.0f)
                {
                    Pitch = 89.0f;
                }

                if (Pitch < -89.0f)
                {
                    Pitch = -89.0f;
                }
            }
            
            Console.WriteLine("Yaw: " + Yaw + " pitch: " + Pitch);
            UpdateCameraVectors();
        }

        public void processScroll(double yOffset)
        {
            Zoom -= (float)yOffset;
            if (Zoom < 5.0f) Zoom = 5.0f;
            if (Zoom > 60.0f) Zoom = 60.0f;
        }

        private void UpdateCameraVectors()
        {
            Vector3 front;
            front.X = MathF.Cos(MathHelper.ToRadians(Yaw)) * MathF.Cos(MathHelper.ToRadians(Pitch));
            front.Y = MathF.Sin(MathHelper.ToRadians(Pitch));
            front.Z = MathF.Sin(MathHelper.ToRadians(Yaw)) * MathF.Cos(MathHelper.ToRadians(Pitch));
            Front = Vector3.Normalize(front);

            Right = Vector3.Normalize(Vector3.Cross(Front, WorldUp));
            Up = Vector3.Normalize(Vector3.Cross(Right, Front));
        }
    }
}