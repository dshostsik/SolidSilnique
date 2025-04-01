using System;
using Microsoft.Xna.Framework;
using Vector3 = Microsoft.Xna.Framework.Vector3;

namespace SolidSilniqueProto
{
    public class Camera
    {
        private const float YAW = -90.0f;
        private const float PITCH = .0f;
        private const float ZOOM = 35.0f;
        private const float SENSE = .005f;
        private const float SPEED = .025f;

        public enum directions
        {
            FORWARD,
            BACKWARD,
            LEFT,
            RIGHT
        }

        Vector3 Position;
        Vector3 Front;
        Vector3 Up;
        Vector3 Right;
        Vector3 WorldUp;

        float Yaw;
        float Pitch;

        float MovementSpeed;
        float MouseSensitivity;
        float Zoom;

        public Camera(Vector3 position, float yaw = YAW, float pitch = PITCH)
        {
            Position = position;
            WorldUp = -Vector3.Up;
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
            return Matrix.CreateLookAt(Position, Position + Front, Up);
        }

        public void move(directions direction, float deltaTime)
        {
            float speed = MovementSpeed * deltaTime;

            if (direction == directions.FORWARD) Position += Front * speed;
            if (direction == directions.BACKWARD) Position -= Front * speed;

            if (direction == directions.RIGHT) Position += Right * speed;
            if (direction == directions.LEFT) Position -= Right * speed;
        }

        public void mouseMovement(float xOffset, float yOffset, float deltaTime, bool constrainPitch = true,
            bool constrainYaw = true)
        {
            xOffset *= MouseSensitivity;
            yOffset *= MouseSensitivity;

            Yaw += xOffset * deltaTime;
            Pitch += yOffset * deltaTime;

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
            front.X = MathF.Cos(MathHelper.ToRadians(Yaw) * MathF.Cos(MathHelper.ToRadians(Pitch)));
            front.Y = MathF.Sin(MathHelper.ToRadians(Pitch));
            front.Z = MathF.Sin(MathHelper.ToRadians(Yaw) * MathF.Cos(MathHelper.ToRadians(Pitch)));
            Front = Vector3.Normalize(front);

            Right = Vector3.Normalize(Vector3.Cross(Front, WorldUp));
            Up = Vector3.Normalize(Vector3.Cross(Right, Front));
        }
    }
}