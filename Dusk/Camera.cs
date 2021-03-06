﻿using System;
using OpenTK;

#pragma warning disable CS0675 // Bitwise-or operator used on a sign-extended operand

namespace Dusk
{
    public class Camera
    {
        public const float NearPlane = 0.15f;
        public const float FarPlane = 1000f;

        public float TargetFov { get; private set; }

        public float PartialFov { get; private set; }

        private float _pitch;
        public Vector3 Pos = Vector3.UnitX * -4;

        public float PitchOffset;

        public float Pitch
        {
            get => _pitch;

            set => _pitch = MathHelper.Clamp(value, -MathHelper.PiOver2 + MathHelper.DegreesToRadians(0.1f), MathHelper.PiOver2 - MathHelper.DegreesToRadians(0.1f));
        }

        public float Yaw { get; set; } = MathHelper.PiOver2;

        public Camera()
        {
            PartialFov = TargetFov = 70; // TODO - SettingsManager.GetFloat("fov");

            UpdateViewMatrix();
            UpdateProjectionMatrix();
        }

        public void SetFov(float fov)
        {
            PartialFov = fov;

            UpdateProjectionMatrix();
        }

        public void SetTargetFov(float fov)
        {
            TargetFov = PartialFov = fov;

            UpdateProjectionMatrix();
        }

        public void UpdateViewMatrix()
        {
            Matrix4 x = Matrix4.CreateRotationX(Pitch + PitchOffset);
            Matrix4 y = Matrix4.CreateRotationY(Yaw);

            Matrix4 t = Matrix4.CreateTranslation(-Pos);

            View = t * y * x;

            Shader.SetViewMatrix(View);
        }

        public void UpdateProjectionMatrix()
        {
            float aspectRatio = (float)Game.Instance.Width / Game.Instance.Height;

            Projection = Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(PartialFov), aspectRatio, NearPlane, FarPlane);

            Shader.SetProjectionMatrix(Projection);
        }

        public Vector3 GetLookVec()
        {
            return new Quaternion(0, -Yaw + MathHelper.PiOver2, -_pitch) * Vector3.UnitX;//MathUtil.Rotate(Vector3.UnitX, -_pitch, -Yaw + MathHelper.PiOver2, 0);
        }

        public Vector2 Left
        {
            get
            {
                float s = (float)Math.Sin(-(Yaw + MathHelper.PiOver2));
                float c = (float)Math.Cos(Yaw + MathHelper.PiOver2);

                return new Vector2(s, c).Normalized();
            }
        }

        public Vector2 Forward
        {
            get
            {
                float s = -(float)Math.Sin(-Yaw);
                float c = -(float)Math.Cos(Yaw);

                return new Vector2(s, c).Normalized();
            }
        }

        public Matrix4 View { get; private set; }
        public Matrix4 Projection { get; private set; }
    }
}