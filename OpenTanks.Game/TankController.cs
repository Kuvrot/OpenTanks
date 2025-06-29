using System;
using Stride.Core.Mathematics;
using Stride.Input;
using Stride.Engine;
using Stride.Physics;
using Stride.Particles;
using Stride.Particles.Components;
using SharpDX.DirectInput;
using OpenTanks.Core;
using OpenTanks.Gameplay;
using Stride.Core.Shaders.Ast;
using Stride.Audio;
using Silk.NET.SDL;

namespace OpenTanks.Tank
{
    public class TankController : SyncScript
    {
        public byte player = 1;

        public CameraComponent camera;
        public TransformComponent tankHead;
        public Prefab bullet;
        public TransformComponent cannonPosition;
        public ParticleSystemComponent MuzzleFlash;
        public float reloadTime = 2.5f;

        public float health = 100;
        public Sound engineSound;
        public Sound shootSound;
        private SoundInstance engineSoundInstance;
        private float lastReloadTime = 0;
        private float xAxis, yAxis;
        private CharacterComponent character;

        private HitResult hitResult;

        public override void Start()
        {
            character = Entity.Get<CharacterComponent>();
            MuzzleFlash.ParticleSystem.Stop();
            lastReloadTime = TimeUtils.Instance.GetTime();
            engineSoundInstance = engineSound.CreateInstance();
        }

        public override void Update()
        {
            Move();
            DebugText.Print(health.ToString(), new Int2(300 , 300));
            ShootSystem();
            PlayEngineSound();
        }

        private HitResult ScreenPositionToWorldPositionRaycast(Vector2 screenPos, CameraComponent camera, Simulation simulation)
        {
            Matrix invViewProj = Matrix.Invert(camera.ViewProjectionMatrix);

            // Reconstruct the projection-space position in the (-1, +1) range.
            //    Don't forget that Y is down in screen coordinates, but up in projection space
            Vector3 sPos;
            sPos.X = screenPos.X * 2f - 1f;
            sPos.Y = 1f - screenPos.Y * 2f;

            // Compute the near (start) point for the raycast
            // It's assumed to have the same projection space (x,y) coordinates and z = 0 (lying on the near plane)
            // We need to unproject it to world space
            sPos.Z = 0f;
            Vector4 vectorNear = Vector3.Transform(sPos, invViewProj);
            vectorNear /= vectorNear.W;

            // Compute the far (end) point for the raycast
            // It's assumed to have the same projection space (x,y) coordinates and z = 1 (lying on the far plane)
            // We need to unproject it to world space
            sPos.Z = 1f;
            Vector4 vectorFar = Vector3.Transform(sPos, invViewProj);
            vectorFar /= vectorFar.W;

            // Raycast from the point on the near plane to the point on the far plane and get the collision result
            return simulation.Raycast(vectorNear.XYZ(), vectorFar.XYZ());
        }


        private void Move()
        {
            ControllerSetup();
            var v = new Vector3(-xAxis, 0f, yAxis);
            v.Normalize();
            character.SetVelocity(v * 100 * (float)Game.UpdateTime.Elapsed.TotalSeconds);
            // Character orientation
            hitResult = ScreenPositionToWorldPositionRaycast(Input.MousePosition, camera, this.GetSimulation());

            Vector3 targetPos = hitResult.Point;
            Vector3 headPos = tankHead.Entity.Transform.WorldMatrix.TranslationVector;

            Vector3 direction = Vector3.Normalize(targetPos - headPos);
            float yaw = MathF.Atan2(direction.X, direction.Z);
            tankHead.Rotation = Quaternion.RotationYawPitchRoll(yaw, 0f, 0f);
        }

        private void ControllerSetup()
        {

            xAxis = 0f;
            yAxis = 0f;

            if (Input.IsKeyDown(Keys.W)) yAxis += 1f;
            if (Input.IsKeyDown(Keys.S)) yAxis -= 1f;
            if (Input.IsKeyDown(Keys.D)) xAxis += 1f;
            if (Input.IsKeyDown(Keys.A)) xAxis -= 1f;

            Vector2 direction = new Vector2(xAxis, yAxis);
            if (direction.LengthSquared() > 0.001f)
            {
                direction.Normalize();

                // Crear una dirección 3D en el plano XZ (Y es arriba)
                Vector3 lookDirection = new Vector3(-direction.X, 0, direction.Y);

                // Usamos LookRotation para apuntar hacia esa dirección
                Quaternion rotation = Quaternion.LookRotation(lookDirection, Vector3.UnitY);

                Entity.GetChild(0).Transform.Rotation = rotation;
            }
        }

        private void ShootSystem()
        {
            if (Input.IsMouseButtonPressed(MouseButton.Left) && TimeUtils.Instance.HasSecondsPassed(lastReloadTime, reloadTime))
            {
                MuzzleFlash.ParticleSystem.Stop();
                MuzzleFlash.ParticleSystem.Play();
                lastReloadTime = TimeUtils.Instance.GetTime();
                var newBullet = bullet.Instantiate();
                var cannonPos = cannonPosition.WorldMatrix.TranslationVector;
                newBullet[0].Transform.Position = cannonPos;
                var target = hitResult.Point;
                target.Y += 0.15f;
                newBullet[0].Get<BulletComponent>().SetTarget(target);
                Entity.Scene.Entities.AddRange(newBullet);
                Entity.Get<SoundManager>().PlaySound(shootSound);
            }
        }

        private async void PlayEngineSound()
        {
            if (engineSound != null)
            {
                if (yAxis != 0 || xAxis != 0)
                {
                    engineSoundInstance.Pitch = 0.9f;
                }
                else
                {
                    engineSoundInstance.Pitch = 0.7f;
                }

                await engineSoundInstance.ReadyToPlay();
                engineSoundInstance.Play();
            }
        }
    }
}
