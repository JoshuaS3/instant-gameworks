using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL4;

namespace InstantGameworks.Graphics
{
    public interface ICamera
    {
        Vector3 Position { get; set; }
        Vector2 Orientation { get; set; }
        Matrix4 PerspectiveMatrix { get; set; }
        float AspectRatio { get; set; }

        Matrix4 Update();
    }

    public class Camera : ICamera
    {
        public Vector3 Position { get; set; } = Vector3.Zero;
        public Vector2 Orientation { get; set; } = new Vector2(0, 0);
        public Matrix4 PerspectiveMatrix { get; set; } = new Matrix4();
        public float AspectRatio { get; set; } = 16f / 9f;

        public float FieldOfView { get; set; } = 90;
        public float MinViewDistance { get; set; } = 0.01f;
        public float MaxViewDistance { get; set; } = 4000;


        public Camera()
        {
            Position = Vector3.Zero;
            Orientation = new Vector2((float)Math.PI, 0);
        }
        public Camera(Vector3 position)
        {
            Position = position;
            Orientation = new Vector2();
        }
        public Camera(Vector3 position, Vector2 orientation)
        {
            Position = position;
            Orientation = orientation;
        }


        float Radian = (float)Math.PI / 180f;
        public Matrix4 Update()
        {
            Vector3 LookAt = new Vector3(
                (float)(Math.Sin(Orientation.X) * Math.Cos(Orientation.Y)),
                (float)Math.Sin(Orientation.Y),
                (float)(Math.Cos(Orientation.X) * Math.Cos(Orientation.Y))
                );
            PerspectiveMatrix = Matrix4.LookAt(Position, Position + LookAt, Vector3.UnitY) * Matrix4.CreatePerspectiveFieldOfView(FieldOfView * Radian, AspectRatio, MinViewDistance, MaxViewDistance);
            return PerspectiveMatrix;
        }

    }

    public class StudioCamera : Camera
    {
        public float MoveSensitivity { get; set; } = 0.3f;
        public float LookSensitivity { get; set; } = 0.0025f;

        public void AddRotation(float DeltaX, float DeltaY)
        {
            DeltaX *= -LookSensitivity;
            DeltaY *= -LookSensitivity;

            Orientation = new Vector2(
                (Orientation.X + DeltaX) % ((float)Math.PI * 2f),
                Math.Max(
                    Math.Min(
                        Orientation.Y + DeltaY,
                        (float)Math.PI / 2f - 0.1f
                    ),
                    (float)-Math.PI / 2f + 0.1f
                )
            );
        }
        public void Move(float x, float y, float z)
        {
            Vector3 offset = new Vector3();

            Vector3 forward = new Vector3(-(float)Math.Sin(Orientation.X), -(float)Math.Tan(Orientation.Y), -(float)Math.Cos(Orientation.X));
            forward.NormalizeFast();
            Vector3 right = new Vector3(-forward.Z, 0, forward.X);
            right.NormalizeFast();
            Vector3 up = new Vector3((float)Math.Sin(Orientation.Y), (float)Math.Tan(Orientation.Y), (float)Math.Cos(Orientation.Y));
            up.NormalizeFast();

            offset += x * right;
            offset += y * up;
            offset += z * forward;
            
            offset = Vector3.Multiply(offset, MoveSensitivity);

            Position += offset;
        }
    }
}
