using System;
using System.Collections.Generic;
using System.Text;

namespace Shooter
{
    static class GameConstants
    {
        //camera constants
        public const float CameraHeight = 25000.0f;
        public const float PlayfieldSizeX = 200f;
        public const float PlayfieldSizeZ = 200f;
        //Dalek constants
        public const int NumZombies = 15;
        public const float zombieMinSpeed = 3.0f;
        public const float zombieMaxSpeed = 20.0f;
        public const float zombieSpeedAdjustment = 2.5f;
        public const float zombieScalar = 0.2f;
        //collision constants
        public const float ZombieBoundingSphereScale = 0.6f;  //50% size
        public const float robotBoundingSphereScale = 0.5f;  //50% size
        //bullet constants
        public const int NumBullet = 100;
        public const float BulletSpeedAdjustment = 15.0f;
        public const float bulletScalar = 0.3f;
        public const float BulletBoundingSphereScale = 0.03f;  //50% size
        //missile constants
        public const int NumMissile = 10;
        public const float MissileSpeedAdjustment = 15.0f;
        public const float MissisleScalar = 0.03f;
        public const float MissileBoundingSphereScale = 0.01f;  //50% size
        //Terrain constants
        public const float TerrainScalar = 2.3f;


    }
}
