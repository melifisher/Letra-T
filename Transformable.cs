using System;
using OpenTK;

namespace Letra_T
{
    abstract class Transformable
    {
        private Vector3 Position { get; set; }
        private Vector3 Rotation { get; set; }
        private Vector3 Scale { get; set; }
        public Punto CenterOfMass { get; set; }

        protected Matrix4 transformMatrix;

        public Transformable()
        {
            Position = Vector3.Zero;
            Rotation = Vector3.Zero;
            Scale = Vector3.One;
            transformMatrix = Matrix4.Identity;
        }

        public virtual void UpdateTransform()
        {
            transformMatrix = Matrix4.CreateTranslation(-CenterOfMass.ToVector3())
                * Matrix4.CreateRotationX(Rotation.X)
                * Matrix4.CreateRotationY(Rotation.Y)
                * Matrix4.CreateRotationZ(Rotation.Z)
                * Matrix4.CreateScale(Scale)
                * Matrix4.CreateTranslation(CenterOfMass.ToVector3())
                * Matrix4.CreateTranslation(Position);
        }

        public abstract void Draw();

        public void Rotar(Vector3 rotation)
        {
            Rotation += rotation;
            UpdateTransform();
        }

        public void Escalar(Vector3 scale)
        {
            Scale += scale;
            if (Scale.X <= 0 || Scale.Y <= 0 || Scale.Z <= 0)
            {
                Scale -= scale;
                return;
            }
            UpdateTransform();
        }

        public void Trasladar(Vector3 translation)
        {
            Position += translation;
            UpdateTransform();
        }

        public virtual bool Intersects(Vector3 rayOrigin, Vector3 rayDirection, out float distance)
        {
            //esfera de límite
            float radius = 0.5f * Math.Max(Math.Max(Scale.X, Scale.Y), Scale.Z);
            Vector3 center = Position;

            Vector3 m = rayOrigin - center;
            float b = Vector3.Dot(m, rayDirection);
            float c = Vector3.Dot(m, m) - radius * radius;

            if (c > 0f && b > 0f)
            {
                distance = float.MaxValue;
                return false;
            }

            float discr = b * b - c;

            if (discr < 0f)
            {
                distance = float.MaxValue;
                return false;
            }

            distance = -b - (float)Math.Sqrt(discr);

            if (distance < 0f)
                distance = -b + (float)Math.Sqrt(discr);

            return distance >= 0f;
        }

        public Vector3 getPosition()
        {
            return Position;
        }
    }
}
