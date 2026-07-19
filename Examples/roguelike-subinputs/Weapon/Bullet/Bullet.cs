using System;
using Enemies;
using Godot;
using Player;

namespace Weapon {
    public partial class Bullet : StaticBody2D {
        public enum BulletSourceType {
            Player,
            Enemy,
        }

        [Export] public BulletSourceType BulletSource = BulletSourceType.Enemy;
        [Export] private Sprite2D _bulletSprite;
        private WeaponResource _weapon;
        private Vector2 _velocity;


        public void Initialize(WeaponResource weapon, Vector2 velocity, BulletSourceType? bulletSource = null) {
            _weapon = weapon;
            _bulletSprite.Texture = weapon.BulletIcon;
            _velocity = velocity;
            if(bulletSource != null) BulletSource = (BulletSourceType)bulletSource;
        }


        public override void _PhysicsProcess(double delta) {
            KinematicCollision2D collisionResult = MoveAndCollide(_velocity * (float)delta * _weapon.BulletSpeed);

            if(collisionResult != null) {
                Node2D body = collisionResult.GetCollider() as Node2D;
                switch(BulletSource) {
                case BulletSourceType.Enemy:
                    if(body is PlayerMovement playerMovement) {
                        playerMovement.HitboxComponent.ApplyDamage(_weapon.Damage);
                        QueueFree();
                    }
                    else if(body is StaticBody2D) {
                        QueueFree();
                    }
                    else {
                        throw new NotImplementedException();
                    }
                    break;

                case BulletSourceType.Player:
                    if(body is EnemyMovement enemyMovement) {
                        enemyMovement.HitboxComponent.ApplyDamage(_weapon.Damage);
                        QueueFree();
                    }
                    else if(body is StaticBody2D) {
                        QueueFree();
                    }
                    else {
                        throw new NotImplementedException();
                    }
                    break;
                }
            }
        }
    }
}