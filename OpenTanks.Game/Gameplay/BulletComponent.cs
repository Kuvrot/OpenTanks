using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Stride.Core.Mathematics;
using Stride.Input;
using Stride.Engine;
using Stride.Physics;
using OpenTanks.Tank;
using Stride.Core.Extensions;
using BulletSharp.SoftBody;
using OpenTanks.Core;
using Stride.Particles.Components;

namespace OpenTanks.Gameplay
{
    public class BulletComponent : SyncScript
    {
        public float lifeTime = 1f; //(in seconds)
        public float speed = 5;
        public ParticleSystemComponent explosionVFX;
        private RigidbodyComponent rigidbodyComponent;
        private bool impacted = false;
        private float lastTimeImpact = 0;
        private Vector3 target;
        public override void Start()
        {
            rigidbodyComponent = Entity.Get<RigidbodyComponent>();
            explosionVFX.ParticleSystem.Stop();
        }

        public override void Update()
        {
            if (!impacted)
            {
                if (target != Vector3.Zero)
                {
                    Vector3 direction = target - Entity.Transform.Position;
                    direction.Normalize();
                    Entity.Transform.Position += direction * speed * (float)Game.UpdateTime.Elapsed.TotalSeconds;

                    foreach (var collision in rigidbodyComponent.Collisions)
                    {
                        if (collision.ColliderB.Entity.Get<TankController>() != null)
                        {
                            collision.ColliderB.Entity.Get<TankController>().health -= 10;
                        }

                        impacted = true;
                        explosionVFX.ParticleSystem.Play();
                        lastTimeImpact = TimeUtils.Instance.GetTime();
                        break;
                    }
                }
            }
            else
            {
                if (TimeUtils.Instance.HasSecondsPassed(lastTimeImpact , lifeTime))
                {
                    Entity.Scene.Entities.Remove(Entity);
                }
            }
        }

        public void SetTarget(Vector3 target)
        {
            this.target = target;
        }
    }
}
