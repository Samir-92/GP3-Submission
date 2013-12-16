using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

//using DPSF;
namespace DPSF.ParticleSystem
{
    class BloodParticles : DefaultTexturedQuadParticleSystem
    {
        public bool isActive;
        double currTime;
        double PTime = 5.0f;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="cGame">Handle to the Game object being used. Pass in null for this 
        /// parameter if not using a Game object.</param>
        public BloodParticles(Game cGame) : base(cGame) { }

        /// <summary>
        /// Function to Initialize the Particle System with default values.
        /// Particle system properties should not be set until after this is called, as 
        /// they are likely to be reset to their default values.
        /// </summary>
        /// <param name="cGraphicsDevice">The Graphics Device the Particle System should use</param>
        /// <param name="cContentManager">The Content Manager the Particle System should use to load resources</param>
        /// <param name="cSpriteBatch">The Sprite Batch that the Sprite Particle System should use to draw its particles.
        /// If this is not initializing a Sprite particle system, or you want the particle system to use its own Sprite Batch,
        /// pass in null.</param>
        public override void AutoInitialize(GraphicsDevice cGraphicsDevice, ContentManager cContentManager, SpriteBatch cSpriteBatch)
        {
            // Initialize the Particle System before doing anything else
            InitializeTexturedQuadParticleSystem(cGraphicsDevice, cContentManager, 1000, 50000,
                                                UpdateVertexProperties, "Textures\\blood");

            // Finish loading the Particle System in a separate function call, so if
            // we want to reset the Particle System later we don't need to completely 
            // re-initialize it, we can just call this function to reset it.
            LoadParticleSystem();
        }

        /// <summary>
        /// Load the Particle System Events and any other settings
        /// </summary>
        public void LoadParticleSystem()
        {
            // Set the Function to use to Initialize new Particles.
            // The Default Templates include a Particle Initialization Function called
            // InitializeParticleUsingInitialProperties, which initializes new Particles
            // according to the settings in the InitialProperties object (see further below).
            // You can also create your own Particle Initialization Functions as well, as shown with
            // the InitializeParticleProperties function below.
            ParticleInitializationFunction = InitializeParticleUsingInitialProperties;
            //ParticleInitializationFunction = InitializeParticleProperties;

            // Setup the Initial properties of the Particles.
            // These are only applied if using InitializeParticleUsingInitialProperties 
            // as the Particle Initialization Function.
            InitialProperties.LifetimeMin = 2.0f;
            InitialProperties.LifetimeMax = 4.0f;
            InitialProperties.PositionMin = Vector3.Zero;
            InitialProperties.PositionMax = Vector3.Zero;
            InitialProperties.VelocityMin = new Vector3(4, -0.1f, 3);
            InitialProperties.VelocityMax = new Vector3(-10, 4.0f, -10);
            InitialProperties.RotationMin.Z = 0.0f;
            InitialProperties.RotationMax.Z = MathHelper.Pi;
            InitialProperties.RotationalVelocityMin.Z = -MathHelper.Pi;
            InitialProperties.RotationalVelocityMax.Z = MathHelper.Pi;
            InitialProperties.StartSizeMin = 6;
            InitialProperties.StartSizeMax = 12;
            InitialProperties.EndSizeMin = 12;
            InitialProperties.EndSizeMax = 16;
            InitialProperties.StartColorMin = Color.White;
            InitialProperties.StartColorMax = Color.White;
            InitialProperties.EndColorMin = Color.White;
            InitialProperties.EndColorMax = Color.White;

            // Remove all Events first so that none are added twice if this function is called again
            ParticleEvents.RemoveAllEvents();
            ParticleSystemEvents.RemoveAllEvents();

            // Allow the Particle's Position, Rotation, Size, Color, and Transparency to be updated each frame
            ParticleEvents.AddEveryTimeEvent(UpdateParticlePositionUsingVelocity);
            ParticleEvents.AddEveryTimeEvent(UpdateParticleRotationUsingRotationalVelocity);
            ParticleEvents.AddEveryTimeEvent(UpdateParticleWidthAndHeightUsingLerp);
            ParticleEvents.AddEveryTimeEvent(UpdateParticleColorUsingLerp);

            // This function must be executed after the Color Lerp function as the Color Lerp will overwrite the Color's
            // Transparency value, so we give this function an Execution Order of 100 to make sure it is executed last.
            ParticleEvents.AddEveryTimeEvent(UpdateParticleTransparencyToFadeOutUsingLerp, 100);

            // Update the particle to face the camera. Do this after updating it's rotation/orientation.
            ParticleEvents.AddEveryTimeEvent(UpdateParticleToFaceTheCamera, 200);

            // Set the Particle System's Emitter to toggle on and off every 0.5 seconds
            ParticleSystemEvents.LifetimeData.EndOfLifeOption = CParticleSystemEvents.EParticleSystemEndOfLifeOptions.Repeat;
            ParticleSystemEvents.LifetimeData.Lifetime = 1.0f;
            ParticleSystemEvents.AddTimedEvent(0.0f, UpdateParticleSystemEmitParticlesAutomaticallyOn);
            ParticleSystemEvents.AddTimedEvent(0.5f, UpdateParticleSystemEmitParticlesAutomaticallyOff);

            // Setup the Emitter
            Emitter.ParticlesPerSecond = 50;

            // Emitter.PositionData.Position = new Vector3(0, 0, 0);
        }

        /// <summary>
        /// Example of how to create a Particle Initialization Function
        /// </summary>
        /// <param name="cParticle">The Particle to be Initialized</param>
        public void InitializeParticleProperties(DefaultTexturedQuadParticle cParticle)
        {
            // Set the Particle's Lifetime (how long it should exist for)
            cParticle.Lifetime = 2.0f;

            // Set the Particle's initial Position to be wherever the Emitter is
            cParticle.Position = Emitter.PositionData.Position;

            // Set the Particle's Velocity
            Vector3 sVelocityMin = new Vector3(-1, 0.1f, -50);
            Vector3 sVelocityMax = new Vector3(5, 1, 50);
            cParticle.Velocity = DPSFHelper.RandomVectorBetweenTwoVectors(sVelocityMin, sVelocityMax);

            // Adjust the Particle's Velocity direction according to the Emitter's Orientation
            cParticle.Velocity = Vector3.Transform(cParticle.Velocity, Emitter.OrientationData.Orientation);

            // Give the Particle a random Size
            // Since we have Size Lerp enabled we must also set the Start and End Size
            cParticle.Size = cParticle.StartSize = cParticle.EndSize = RandomNumber.Next(1, 10);

            // Give the Particle a random Color
            // Since we have Color Lerp enabled we must also set the Start and End Color
            // cParticle.Color = cParticle.StartColor = cParticle.EndColor = DPSFHelper.RandomColor();

        }

        //===========================================================
        // Particle Update Functions
        //===========================================================

        /// <summary>
        /// Example of how to create a Particle Event Function
        /// </summary>
        /// <param name="cParticle">The Particle to update</param>
        /// <param name="fElapsedTimeInSeconds">How long it has been since the last update</param>
        public void UpdateParticleFunctionExample(DefaultTexturedQuadParticle cParticle, float fElapsedTimeInSeconds)
        {
            // cParticle.Position += cParticle.Velocity * fElapsedTimeInSeconds;
        }

        //===========================================================
        // Particle System Update Functions
        //===========================================================

        /// <summary>
        /// Example of how to create a Particle System Event Function
        /// </summary>
        /// <param name="fElapsedTimeInSeconds">How long it has been since the last update</param>
        public void UpdateParticleSystemFunctionExample(float fElapsedTimeInSeconds)
        {
            // Place code to update the Particle System here
            // Example: Emitter.EmitParticles = true;
            // Example: SetTexture("TextureAssetName");
        }

        //===========================================================
        // Other Particle System Functions
        //===========================================================

        //this method controls the rotaiton of the particle
        public void Rot(Matrix r)
        {
            Emitter.OrientationData.Rotate(r);
        }

        //this method gives the particle a life time
        public void DesP(float a)
        {

            currTime += a;

            if (PTime < currTime)
            {
                isActive = false;
                currTime = 0;
            }
        }

    }
}
