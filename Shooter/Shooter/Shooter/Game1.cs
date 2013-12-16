using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using System.Threading;
using DPSF;
using DPSF.ParticleSystem;
namespace Shooter
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        /// <summary>
        /// Menu variables
        /// </summary>
        enum GameState
        {
            StartMenu,
            HowToPlay,
            Loading,
            Playing,
            EndGame,
            Paused
        }

        //textures for buttons and positions
        private Texture2D backGround;
        private Texture2D backGroundFail;
        private Texture2D htpBackGround;
        Rectangle frame;
        private Texture2D startButton;
        private Texture2D exitButton;
        private Texture2D pauseButton;
        private Texture2D backButton;
        private Texture2D resumeButton;
        private Texture2D htpButton;
        private Texture2D loadingScreen;
        private Texture2D muteButtonOn;
        private Texture2D muteButtonOff;
        private Vector2 startButtonPosition;
        private Vector2 exitButtonPosition;
        private Vector2 resumeButtonPosition;
        private Vector2 htpButtonPosition;
        private Vector2 pauseButtonPosition;
        private Vector2 muteButtonPosition;
        private Vector2 backButtonPosition;
        private GameState gameState;
        private GameState prev;
        private Thread backgroundThread;
        private bool isLoading = false;

        //mouse state
        MouseState mouseState;
        MouseState previousMouseState;

        //graphics variable and spritebatch
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        #region User Defined Variables
        //------------------------------------------
        // Added for use with fonts
        //------------------------------------------
        SpriteFont fontToUse;

        //--------------------------------------------------
        // Added for use with playing Audio via Media player
        //--------------------------------------------------
        private Song bkgMusic;
        //--------------------------------------------------
        //Set the sound effects to use
        //--------------------------------------------------
        private SoundEffectInstance robotSoundInstance;
        private SoundEffect robotSound;
        private SoundEffectInstance zombieSoundInstance;
        private SoundEffect zombieSound;
        private SoundEffect machSound;
        private SoundEffect rpgSound;

        // Set the 3D model to draw.
        private Model mdlRobot;
        private Matrix[] mdlRobotTransforms;
        private Matrix robotTransform;
                   
        // The aspect ratio determines how to scale 3d to 2d projection.
        private float aspectRatio;

        // Set the position of the model in world space, and set the rotation.
        private Vector3 mdlPosition = Vector3.Zero;
        private Vector3 mdlRotation = Vector3.Zero;
        private Vector3 mdlVelocity = Vector3.Zero;

        //set the enemies to draw
        private Model mdlZombie;
        private Matrix[] mdlZombieTransforms;
        private Zombies[] zombieList = new Zombies[GameConstants.NumZombies];
        double currTime = 0;

        /// <summary>
        /// Machine Gun variables
        /// </summary>
        bool machEnab = false;
        double spTime = 0.1;
        //array of bulllets
        private Model mdlBullet;
        private Matrix[] mdlBulletTransforms;
        private Bullet[] bulletList = new Bullet[GameConstants.NumBullet];

        /// <summary>
        /// RPG Variables
        /// </summary>
        bool RPGEnab = false;
        double RPGspTime = 1;
        //array of bullets
        private Model mdlMissile;
        private Matrix[] mdlMissileTransforms;
        private Missile[] missileList = new Missile[GameConstants.NumBullet];
        private Vector3 msRot;

        //random variable
        private Random random = new Random();

        //keyboardstates
        private KeyboardState lastState;
        private KeyboardState keyboardState;

        //hit counter for hitting enemies
        private int hitCount;

        // Set the position of the camera in world space, for our view matrix.
        private Vector3 cameraPosition;
        private Matrix viewMatrix;
        private Matrix projectionMatrix;

        //cameras variables
        Matrix rotMat;
        bool tCam = true;
        bool tPCam;

        //skybox variables
        Texture2D[] skyboxTextures;
        Model skyboxModel;
        Effect effect;

        Effect ModelEffect;

        GraphicsDevice device;
     
        //terrain variables
        Model terrain;
        Vector3 tPosition = new Vector3(0,-5,0);
        Matrix[] mdlTerrainTransforms;

        //Particles setup
        SmokeParticles mySmokeParticleSystem = null;
        FireParticles myFireParticleSystem = null;
        BloodParticles myBloodParticleSystem = null;
        ParticleSystemManager particleSystemManager;
        Matrix mPworldMatrix = Matrix.Identity;

        //health integer
        int health = 5;

        float timer;
        int timeCount;
        //int to find number of zombies
        int numZomb = GameConstants.NumZombies;

        //vectore to get position of zombie and add blood
        Vector3 bloodPos;

        int points;

        //mute sound boolean
        bool mute = false;

        #endregion

        private void InitializeTransform()
        {

        }

        public void Score(int t, int health) 
        {
            
            int tScore = (1000 - t / 100);
            points = health * tScore; 
        }
        //THIS CODE WAS ADDED BUT NOT IMPLEMENTED
        //protected BoundingBox UpdateBoundingBox(Model model, Matrix worldTransform)
        //{
        //    // Initialize minimum and maximum corners of the bounding box to max and min values
        //    Vector3 min = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);
        //    Vector3 max = new Vector3(float.MinValue, float.MinValue, float.MinValue);

        //    // For each mesh of the model
        //    foreach (ModelMesh mesh in model.Meshes)
        //    {
        //        foreach (ModelMeshPart meshPart in mesh.MeshParts)
        //        {
        //            // Vertex buffer parameters
        //            int vertexStride = meshPart.VertexBuffer.VertexDeclaration.VertexStride;
        //            int vertexBufferSize = meshPart.NumVertices * vertexStride;

        //            // Get vertex data as float
        //            float[] vertexData = new float[vertexBufferSize / sizeof(float)];
        //            meshPart.VertexBuffer.GetData<float>(vertexData);

        //            // Iterate through vertices (possibly) growing bounding box, all calculations are done in world space
        //            for (int i = 0; i < vertexBufferSize / sizeof(float); i += vertexStride / sizeof(float))
        //            {
        //                Vector3 transformedPosition = Vector3.Transform(new Vector3(vertexData[i], vertexData[i + 1], vertexData[i + 2]), worldTransform);

        //                min = Vector3.Min(min, transformedPosition);
        //                max = Vector3.Max(max, transformedPosition);
        //            }
        //        }
        //    }

        //    // Create and return bounding box
        //    return new BoundingBox(min, max);
        //}

        //This method controls the camera position for birds eye view
        void UpdateCameraTop()
        {
            //get desired camera position
            cameraPosition = new Vector3(0, 150, 10);
            aspectRatio = graphics.GraphicsDevice.Viewport.AspectRatio;

            //set the camera to look at the player
            viewMatrix = Matrix.CreateLookAt(cameraPosition, mdlPosition, Vector3.Up);

            Viewport viewport = graphics.GraphicsDevice.Viewport;
            projectionMatrix = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(45), aspectRatio,
                1.0f, 350.0f);
        }

        //This method controls the camera for third person view
        void UpdateCamera()
        {
            //get the desired camera position
            cameraPosition = new Vector3(0, 20, 50);

            //set a rotation using the models rotation
            rotMat = Matrix.CreateRotationY(mdlRotation.Y);

            //let the camera move to the position as the player moves
            Vector3 transformedReference =
            Vector3.Transform(cameraPosition, rotMat);

            //set camera position
            cameraPosition = transformedReference + mdlPosition;

            //camera look at player
            viewMatrix = Matrix.CreateLookAt(cameraPosition, mdlPosition, Vector3.Up);

            Viewport viewport = graphics.GraphicsDevice.Viewport;
            float aspectRatio = (float)viewport.Width / (float)viewport.Height;
            projectionMatrix = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(45), aspectRatio,
                1.0f, 350.0f);

        }

        //This method controls the model (player)
        private void MoveModel()
        {
            keyboardState = Keyboard.GetState();
            // Create some velocity if the right trigger is down.
            Vector3 mdlVelocityAdd = Vector3.Zero;

            // Find out what direction we should be thrusting, using rotation.
            mdlVelocityAdd.X = -(float)Math.Sin(mdlRotation.Y);
            mdlVelocityAdd.Z = -(float)Math.Cos(mdlRotation.Y);

            if (keyboardState.IsKeyDown(Keys.Left))
            {
                // Rotate left.
                mdlRotation.Y += 1.0f * 0.10f;
            }

            if (keyboardState.IsKeyDown(Keys.Right))
            {
                // Rotate right.
                mdlRotation.Y -= 1.0f * 0.10f;
            }

            if (keyboardState.IsKeyDown(Keys.Up))
            {
                //when holding sprint add velocity to the player if not revert normal speed
                if (keyboardState.IsKeyDown(Keys.LeftShift))
                {
                    mdlVelocityAdd *= 0.3f;
                }
                else
                {
                    mdlVelocityAdd *= 0.1f;
                }
                mdlVelocity += mdlVelocityAdd;
            }

            if (keyboardState.IsKeyDown(Keys.Down))
            {
                //when holding sprint add velocity to the player if not revert normal speed
                if (keyboardState.IsKeyDown(Keys.LeftShift))
                {
                    mdlVelocityAdd *= -0.3f;
                }
                else
                {
                    mdlVelocityAdd *= -0.1f;
                }

                mdlVelocity += mdlVelocityAdd;
            }

            //resets values
            if (keyboardState.IsKeyDown(Keys.R))
            {
                mdlVelocity = Vector3.Zero;
                mdlPosition = Vector3.Zero;
                mdlRotation = Vector3.Zero;
                robotSoundInstance.Play();
            }

            //This block controls player boundary
            if (mdlPosition.X > GameConstants.PlayfieldSizeX)
            {
                mdlPosition.X = GameConstants.PlayfieldSizeX;
            }
            if (mdlPosition.X < -GameConstants.PlayfieldSizeX)
            {
                mdlPosition.X = -GameConstants.PlayfieldSizeX;
            }
            if (mdlPosition.Z > GameConstants.PlayfieldSizeZ)
            {
                mdlPosition.Z = GameConstants.PlayfieldSizeZ;
            }
            if (mdlPosition.Z < -GameConstants.PlayfieldSizeZ)
            {
                mdlPosition.Z =  -GameConstants.PlayfieldSizeZ;
            }

            lastState = keyboardState;
        }

        //This method controls the functionality of the machine gun
        private void MachShoot()
        {
            //are we shooting?
            ////add another bullet.  Find an inactive bullet slot and use it
            ////if all bullets slots are used, ignore the user input
            for (int i = 0; i < GameConstants.NumBullet; i++)
            {
                if (!bulletList[i].isActive)
                {
                    msRot = mdlRotation;
                    robotTransform = Matrix.CreateRotationY(msRot.Y);
                    Vector3 blPosition = new Vector3(-9.0f, 0, 0);
                    //set direction of bullet
                    bulletList[i].direction = robotTransform.Forward;
                    //set the speed
                    bulletList[i].speed = GameConstants.BulletSpeedAdjustment;
                    //set the position 
                    bulletList[i].position = (mdlPosition + Vector3.Transform(blPosition, robotTransform)) + bulletList[i].direction;
                    bulletList[i].isActive = true;
                    machSound.Play();
                   
                    break; //exit the loop 
                }
            }
        }
      
    private void RPGShoot()
    {
        //are we shooting?
        ////add another bullet.  Find an inactive bullet slot and use it
        ////if all bullets slots are used, ignore the user input
        for (int i = 0; i < GameConstants.NumMissile; i++)
        {
            if (!missileList[i].isActive)
            {
                Vector3 mlPosition = new Vector3(8.0f, 1.5f, 1.0f);
                msRot = mdlRotation;
                robotTransform = Matrix.CreateRotationY(msRot.Y);
                missileList[i].direction = robotTransform.Forward;
                missileList[i].speed = GameConstants.BulletSpeedAdjustment;
                missileList[i].position = (mdlPosition + Vector3.Transform(mlPosition, robotTransform)) + missileList[i].direction;
                missileList[i].isActive = true;
                rpgSound.Play();
                break; //exit the loop 
            }
        }
    }
        //This method controls which gun is selected
        private void GunSelect()
        {
            //when button hit set the correct booleans
            if (keyboardState.IsKeyDown(Keys.D1))
            {
                machEnab = true;
                RPGEnab = false;
            }
            else if (keyboardState.IsKeyDown(Keys.D2))
            {
                machEnab = false;
                RPGEnab = true;
            }
        }

        //This method selects the different camera views
        private void SelectCam()
        {
            //When the button is pressed set the booleans accordingly
            if (keyboardState.IsKeyDown(Keys.D8))
            {
                tCam = true;
                tPCam = false;
            }
            else if (keyboardState.IsKeyDown(Keys.D9))
            {
                tCam = false;
                tPCam = true;
            }
        }

        //This method setsup particles for zombie blood
        private void AddBloodParticles()
        {
            //Initialize the particle
            myBloodParticleSystem.AutoInitialize(device, Content, null);
            myBloodParticleSystem.isActive = true;
            //set it's position
            myBloodParticleSystem.Emitter.PositionData.Position = bloodPos;
        }

        //This method setups particles for machine gun fire
        private void AddFireParticles()
        {
            //Initialize the particle
            myFireParticleSystem.AutoInitialize(device, Content, null);

            //loop through the current bullet and set the emmiter accordingly with position and rotation
            for (int i = 0; i < bulletList.Length; i++)
            {
                myFireParticleSystem.isActive = true;
                myFireParticleSystem.Emitter.PositionData.Position = mdlPosition + Vector3.Transform(new Vector3(-9.0f, 0.0f, -4.0f), robotTransform);
                myFireParticleSystem.Rot(robotTransform);
                //myFireParticleSystem.RemoveAllParticles();
                break;
            }
        }

        //This method sets up RPG particles
        private void AddSmokeParticles()
        {
            //Initialize particle
            mySmokeParticleSystem.AutoInitialize(device, Content, null);
            for (int i = 0; i < missileList.Length; i++)
            {
                mySmokeParticleSystem.isActive = true;
                //Updates the values to the missiles position
                mySmokeParticleSystem.Emitter.PositionData.Position = missileList[i].position;
                mySmokeParticleSystem.InitialProperties.VelocityMin = new Vector3(2, -0.1f, 1);
                mySmokeParticleSystem.InitialProperties.VelocityMax = new Vector3(-1, 0.5f, -10);
                mySmokeParticleSystem.Rot(robotTransform);
                mySmokeParticleSystem.ResetPos(missileList[i].position);
                break;
            }
        }

        //Sets up zombie variables
        private void ResetZombies()
        {
            float xStart;
            float zStart;
            for (int i = 0; i < GameConstants.NumZombies; i++)
            {
                //sets position within the playerfield
                if (random.Next(2) == 0)
                {
                    xStart = (float)-GameConstants.PlayfieldSizeX;
                }
                else
                {
                    xStart = (float)GameConstants.PlayfieldSizeX;
                }
                zStart = (float)random.NextDouble() * GameConstants.PlayfieldSizeZ;
                zombieList[i].position = new Vector3(xStart, 2.0f, zStart);
                double angle = random.NextDouble() * 2 * Math.PI;
                zombieList[i].direction.X = -(float)Math.Sin(angle);
                zombieList[i].direction.Z = (float)Math.Cos(angle);
                zombieList[i].speed = GameConstants.zombieMinSpeed +
                   (float)random.NextDouble() * GameConstants.zombieMaxSpeed;
                zombieList[i].isActive = true;
            }

        }

        //This method setups default effects for the models
        private Matrix[] SetupEffectTransformDefaults(Model myModel)
        {
            Matrix[] absoluteTransforms = new Matrix[myModel.Bones.Count];
            myModel.CopyAbsoluteBoneTransformsTo(absoluteTransforms);

            foreach (ModelMesh mesh in myModel.Meshes)
            {
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.EnableDefaultLighting();
                    effect.Projection = projectionMatrix;
                    effect.View = viewMatrix;
                }
            }
            return absoluteTransforms;
        }
      
        //This method draws the models
        public void DrawModel(Model model, Matrix modelTransform, Matrix[] absoluteBoneTransforms)
        {
            //Draw the model, a model can have multiple meshes, so loop
            foreach (ModelMesh mesh in model.Meshes)
            {
                //This is where the mesh orientation is set
                foreach (BasicEffect effect in mesh.Effects)
                {
                    //Instead of default lighting user defined lighting is used
                    effect.EnableDefaultLighting();
                    //effect.DirectionalLight0.DiffuseColor = new Vector3(0.5f, 0, 0);
                    //effect.DirectionalLight0.Direction = new Vector3(0, 1, 0);
                    //effect.DirectionalLight0.SpecularColor = new Vector3(0, 1, 0);
                    //effect.DirectionalLight1.DiffuseColor = new Vector3(1f, 0, 0);
                    //effect.DirectionalLight1.Direction = new Vector3(1, 0, 0);
                    //effect.DirectionalLight1.SpecularColor = new Vector3(0, 1, 0);
                    //effect.DirectionalLight2.DiffuseColor = new Vector3(1.5f, 0, 0);
                    //effect.DirectionalLight2.Direction = new Vector3(0, 0, 1);
                    //effect.DirectionalLight2.SpecularColor = new Vector3(0, 1, 0);

                    //sets an ambient colour with a tint
                  //  effect.AmbientLightColor = new Vector3(0.2f, 0.2f, 0.2f);
                   // effect.EmissiveColor = new Vector3(0.1f, 0, 0);
                   
                    //this set up if a certain camera is used then change the fog
                    //the fog changes colour according to the health status
                    //this is a feature in the game to increase difficulty
                    if (tPCam == true)
                    {
                        effect.FogEnabled = true;
                        if (health == 5)
                        {
                            effect.FogColor = Color.Gray.ToVector3();

                            effect.FogStart = 60f;
                            effect.FogEnd = 500f;
                        }
                        else if (health < 5 && health > 3)
                        {
                            effect.FogColor = Color.Chocolate.ToVector3();

                            effect.FogStart = 30f;
                            effect.FogEnd = 200f;
                        }
                        else if (health < 2)
                        {
                            effect.FogColor = Color.Red.ToVector3();

                            effect.FogStart = 15f;
                            effect.FogEnd = 100f;
                        }
                    }
                    else
                    {
                        effect.FogEnabled = true;
                        effect.FogColor = Color.Chocolate.ToVector3();
                        effect.FogStart = 50f;
                        effect.FogEnd = 300f;
                    }

                    effect.World = absoluteBoneTransforms[mesh.ParentBone.Index] * modelTransform;
                    effect.Projection = projectionMatrix;
                    effect.View = viewMatrix;
                   
                }
                //Draw the mesh, will use the effects set above.  
                mesh.Draw();
            }
        }

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            this.IsMouseVisible = true;
           // graphics.IsFullScreen = true;
           // graphics.ApplyChanges();
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            this.IsMouseVisible = true;
            Window.Title = "Robots vs Zombies!";
            device = graphics.GraphicsDevice;

            ////set the position of the buttons
            startButtonPosition = new Vector2((device.Viewport.Width / 2) - 50, 200);
            exitButtonPosition = new Vector2((device.Viewport.Width / 2) - 50, 300);
            htpButtonPosition = new Vector2((device.Viewport.Width / 2) - 60, 250);
            backButtonPosition = new Vector2((device.Viewport.Width / 2) - 380, GraphicsDevice.Viewport.Height / 2 + 200);
      
            //set the gamestate to start menu
            gameState = GameState.StartMenu;

            //get the mouse state
            mouseState = Mouse.GetState();
            previousMouseState = mouseState;
            
            //hit count of zombies = 0
            hitCount = 0;


            InitializeTransform();
            ResetZombies();
            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            Content.RootDirectory = "Content";
          
            //sets up main menu music
            bkgMusic = Content.Load<Song>(".\\Audio\\Slicey");
            MediaPlayer.Play(bkgMusic);
            MediaPlayer.IsRepeating = true;
          

            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            //sets up rectangle to draw background images
            frame = new Rectangle(0,0,GraphicsDevice.Viewport.Width,GraphicsDevice.Viewport.Height);

            //load the buttonimages into the content pipeline
            startButton = Content.Load<Texture2D>("Textures\\start");
            exitButton = Content.Load<Texture2D>("Textures\\exit");
            htpButton = Content.Load<Texture2D>("Textures\\htp");
            backButton = Content.Load<Texture2D>("Textures\\back");
            backGround = Content.Load<Texture2D>("Textures\\background");
            backGroundFail = Content.Load<Texture2D>("Textures\\backgroundfail");
            htpBackGround = Content.Load<Texture2D>("Textures\\howtoplay");

            //load the loading screen
            loadingScreen = Content.Load<Texture2D>("Textures\\loading");

            aspectRatio = graphics.GraphicsDevice.Viewport.AspectRatio;
           
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
           
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();

            //load the game when needed
            if (gameState == GameState.Loading && !isLoading) //isLoading bool is to prevent the LoadGame method from being called 60 times a seconds
            {
                //set backgroundthread which loads everything we have in our game
                backgroundThread = new Thread(LoadGame);
                isLoading = true;

                //start backgroundthread
                backgroundThread.Start();
            }

            if (gameState == GameState.Playing)
            {
                currTime += (float)gameTime.ElapsedGameTime.TotalSeconds;
               // Allows the game to exit
                if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                    this.Exit();
                if (keyboardState.IsKeyDown(Keys.Escape))
                {
                    this.Exit();
                }

                timer += (float)gameTime.ElapsedGameTime.TotalSeconds;
                timeCount += (int)timer;
              //loop through missiles that are currently active and update the smoke trail
                for (int i = 0; i < missileList.Length; i++)
                {
                    if (mySmokeParticleSystem.isActive)
                    {
                        mySmokeParticleSystem.UpdateMovement(missileList[i].direction * 3);
                    }
                }

                //sets the particles lifetime count down
                if (myFireParticleSystem.isActive)
                {
                    myFireParticleSystem.DesP((float)gameTime.ElapsedGameTime.TotalSeconds);
                }
                if (myBloodParticleSystem.isActive)
                {
                    myBloodParticleSystem.DesP((float)gameTime.ElapsedGameTime.TotalSeconds);
                }

                //when the space bar is held whichever gun is selected perform 
                //the shooting mechanic and add particle respectively
                if ((keyboardState.IsKeyDown(Keys.Space) || lastState.IsKeyDown(Keys.Space)) && machEnab == true)
                {
                    if (spTime < currTime)
                    {
                        MachShoot();
                        AddFireParticles();
                        currTime = 0;
                    }
                }
                if ((keyboardState.IsKeyDown(Keys.Space) || lastState.IsKeyDown(Keys.Space)) && RPGEnab == true)
                {
                    if (RPGspTime < currTime)
                    {
                        RPGShoot();
                        AddSmokeParticles();
                        currTime = 0;
                    }
                }

                lastState = keyboardState;

                //setup methods
                MoveModel();
                GunSelect();
                SelectCam();
                HealthState();

                //update camera position when booleans are triggered
                if (tCam == true)
                {
                    UpdateCameraTop();
                }
                if (tPCam == true)
                {
                    UpdateCamera();
                }

                // Add velocity to the current position.
                mdlPosition += mdlVelocity;

                // Bleed off velocity over time.
                mdlVelocity *= 0.6f;


                float timeDelta = (float)gameTime.ElapsedGameTime.TotalSeconds;

                for (int i = 0; i < GameConstants.NumZombies; i++)
                {
                    zombieList[i].Update(timeDelta);
                }
                for (int i = 0; i < GameConstants.NumBullet; i++)
                {
                    if (bulletList[i].isActive)
                    {
                        bulletList[i].Update(timeDelta);
                    }
                }
                for (int i = 0; i < GameConstants.NumMissile; i++)
                {
                    if (missileList[i].isActive)
                    {
                        missileList[i].Update(timeDelta);
                    }
                }

                //setup collision sphere
                BoundingSphere robotSphere =
                 new BoundingSphere(mdlPosition,
                           mdlRobot.Meshes[0].BoundingSphere.Radius *
                                 GameConstants.robotBoundingSphereScale);
                

                //Check for collisions
                for (int i = 0; i < zombieList.Length; i++)
                {
                    if (zombieList[i].isActive)
                    {
                        BoundingSphere zombieSphereA =
                          new BoundingSphere(zombieList[i].position, mdlZombie.Meshes[0].BoundingSphere.Radius *
                                      GameConstants.ZombieBoundingSphereScale);

                        for (int k = 0; k < bulletList.Length; k++)
                        {
                            if (bulletList[k].isActive)
                            {
                                BoundingSphere bulletSphere = new BoundingSphere(
                                  bulletList[k].position, mdlBullet.Meshes[0].BoundingSphere.Radius *
                                         GameConstants.BulletBoundingSphereScale);

                                if (zombieSphereA.Intersects(bulletSphere))
                                {
                                    //get position of zombie that was hit
                                    bloodPos = zombieList[i].position;
                                    //add particles
                                    AddBloodParticles();
                                    zombieSoundInstance = zombieSound.CreateInstance();
                                    zombieSoundInstance.Play();
                                    zombieList[i].isActive = false;
                                    numZomb -= 1;
                                    bulletList[k].isActive = false;
                                    myFireParticleSystem.isActive = false;
                                    myBloodParticleSystem.isActive = true;
                                    hitCount++;
                                    break; //no need to check other bullets
                                }
                            }

                        }
                        for (int k = 0; k < missileList.Length; k++)
                        {
                            if (missileList[k].isActive)
                            {
                                robotTransform = Matrix.CreateRotationY(mdlRotation.Y);
                                BoundingSphere missileSphere = new BoundingSphere(
                                  missileList[k].position, mdlMissile.Meshes[0].BoundingSphere.Radius *
                                         GameConstants.MissileBoundingSphereScale);
                                if (zombieSphereA.Intersects(missileSphere))
                                {
                                    //get position of the zombie we hit
                                    bloodPos = zombieList[i].position;
                                    //add particles
                                    AddBloodParticles();
                                    zombieSoundInstance = zombieSound.CreateInstance();
                                    zombieSoundInstance.Play();
                                    zombieList[i].isActive = false;
                                    missileList[k].isActive = false;
                                    mySmokeParticleSystem.isActive = false;
                                    myBloodParticleSystem.isActive = true;
                                    numZomb -= 1;
                                    hitCount++;
                                    break; //no need to check other bullets
                                }
                            }
                        }
                        if (zombieSphereA.Intersects(robotSphere)) //Check collision between robot and zombie 
                        {                                          //set the zombies position away from the player
                            zombieSoundInstance = zombieSound.CreateInstance();
                            zombieSoundInstance.Play();
                            zombieList[i].direction *= -1.0f;
                            zombieList[i].position = new Vector3(GameConstants.PlayfieldSizeX, 0, GameConstants.PlayfieldSizeZ);
                            //health decreases
                            health -= 1;
                            break; //no need to check other bullets
                        }
                    }
                }

                if (hitCount >= GameConstants.NumZombies)
                {

                    Score(timeCount, health);
                    gameState = GameState.EndGame;
                }

                particleSystemManager.SetWorldViewProjectionMatricesForAllParticleSystems(mPworldMatrix, viewMatrix, projectionMatrix);

                // Let all of the particle systems know of the Camera's current position
                particleSystemManager.SetCameraPositionForAllParticleSystems(cameraPosition);

                // Update all of the Particle Systems
                particleSystemManager.UpdateAllParticleSystems((float)gameTime.ElapsedGameTime.TotalSeconds);

            }

            //wait for mouseclick
            mouseState = Mouse.GetState();
            if (previousMouseState.LeftButton == ButtonState.Pressed &&
                mouseState.LeftButton == ButtonState.Released)
            {
                MouseClicked(mouseState.X, mouseState.Y);
            }

            previousMouseState = mouseState;

            if (gameState == GameState.Playing && isLoading)
            {
                LoadGame();
                isLoading = false;
            }
            base.Update(gameTime);
        }

        //loads the skybox model
        private Model LoadModel(string assetName, out Texture2D[] textures)
        {
            Model newModel = Content.Load<Model>(assetName);
            textures = new Texture2D[newModel.Meshes.Count];
            int i = 0;
            foreach (ModelMesh mesh in newModel.Meshes)
                foreach (BasicEffect currentEffect in mesh.Effects)
                    textures[i++] = currentEffect.Texture;

            foreach (ModelMesh mesh in newModel.Meshes)
                foreach (ModelMeshPart meshPart in mesh.MeshParts)
                    meshPart.Effect = effect.Clone();

            return newModel;
        }

        //draws the skybox
        private void DrawSkybox()
        {
            SamplerState ss = new SamplerState();
            ss.AddressU = TextureAddressMode.Clamp;
            ss.AddressV = TextureAddressMode.Clamp;
            device.SamplerStates[0] = ss;

            DepthStencilState dss = new DepthStencilState();
            dss.DepthBufferEnable = false;
            device.DepthStencilState = dss;

            Matrix[] skyboxTransforms = new Matrix[skyboxModel.Bones.Count];
            skyboxModel.CopyAbsoluteBoneTransformsTo(skyboxTransforms);
            int i = 0;
            foreach (ModelMesh mesh in skyboxModel.Meshes)
            {
                foreach (Effect currentEffect in mesh.Effects)
                {
                    Matrix worldMatrix = skyboxTransforms[mesh.ParentBone.Index] * Matrix.CreateTranslation(cameraPosition);
                    currentEffect.CurrentTechnique = currentEffect.Techniques["Textured"];
                    currentEffect.Parameters["xWorld"].SetValue(worldMatrix);
                    currentEffect.Parameters["xView"].SetValue(viewMatrix);
                    currentEffect.Parameters["xProjection"].SetValue(projectionMatrix);
                    currentEffect.Parameters["xTexture"].SetValue(skyboxTextures[i++]);
                }
                mesh.Draw();
            }

            dss = new DepthStencilState();
            dss.DepthBufferEnable = true;
            device.DepthStencilState = dss;
        }

        //This method controls the health
        //and what happens after we die
        public void HealthState()
        {
            if (health <= 0)
            {
                health = 0;

                bkgMusic = Content.Load<Song>(".\\Audio\\Slicey");
                MediaPlayer.Play(bkgMusic);
                MediaPlayer.IsRepeating = true;
                gameState = GameState.EndGame;
                mySmokeParticleSystem.Destroy();
                myFireParticleSystem.Destroy();
                myBloodParticleSystem.Destroy();
            }
        }
        /// <summary>
        /// This is called when the game should draw itself.
        /// It goes through and draws each of the things needed in 
        /// each screen
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            spriteBatch.Begin();
            
            if (gameState == GameState.StartMenu)
            {
                spriteBatch.Draw(backGround, frame, Color.White); 
                spriteBatch.Draw(startButton, startButtonPosition, Color.White);
                spriteBatch.Draw(htpButton, htpButtonPosition, Color.White);
                spriteBatch.Draw(exitButton, exitButtonPosition, Color.White);
            }

            //show the loading screen when needed
            if (gameState == GameState.Loading)
            {
                spriteBatch.Draw(backGround, frame, Color.White);
                spriteBatch.Draw(loadingScreen, new Vector2((GraphicsDevice.Viewport.Width / 2) - (loadingScreen.Width / 2), (GraphicsDevice.Viewport.Height / 2) - (loadingScreen.Height / 2)), Color.YellowGreen);
            }

            //draw the the game when playing
            if (gameState == GameState.Playing)
            {
                device.Clear(ClearOptions.Target | ClearOptions.DepthBuffer, Color.DarkSlateBlue, 1.0f, 0);
                DrawSkybox();
                for (int i = 0; i < GameConstants.NumZombies; i++)
                {
                    if (zombieList[i].isActive)
                    {
                        Matrix dalekTransform = Matrix.CreateScale(GameConstants.zombieScalar) * Matrix.CreateTranslation(zombieList[i].position);
                        DrawModel(mdlZombie, dalekTransform, mdlZombieTransforms);
                    }
                }

                for (int i = 0; i < GameConstants.NumBullet; i++)
                {
                    if (bulletList[i].isActive)
                    {
                        Matrix BulletTransform = Matrix.CreateScale(GameConstants.bulletScalar) * Matrix.CreateRotationY(msRot.Y) * Matrix.CreateTranslation(bulletList[i].position);
                        DrawModel(mdlBullet, BulletTransform, mdlBulletTransforms);
                    }
                }
                for (int i = 0; i < GameConstants.NumMissile; i++)
                {
                    if (missileList[i].isActive)
                    {
                        Matrix MissileTransform = Matrix.CreateScale(GameConstants.MissisleScalar) * Matrix.CreateRotationY(msRot.Y) * Matrix.CreateTranslation(missileList[i].position);
                        DrawModel(mdlMissile, MissileTransform, mdlMissileTransforms);
                    }
                }

                Matrix modelTransform = Matrix.CreateScale(5.0f) * Matrix.CreateRotationY(mdlRotation.Y) * Matrix.CreateTranslation(mdlPosition);
                DrawModel(mdlRobot, modelTransform, mdlRobotTransforms);

                Matrix terrainTransform = Matrix.CreateScale(GameConstants.TerrainScalar) * Matrix.CreateTranslation(tPosition);
                DrawModel(terrain, terrainTransform, mdlTerrainTransforms);

                if (RPGEnab != true && machEnab != true)
                {
                    writeText("Select a weapon!" + "\n" + "Hint: 1 or 2", new Vector2((device.Viewport.Width / 2) - 60, (device.Viewport.Height / 2) - 50), Color.Red);
                }

                writeText("Health: " + health.ToString(), new Vector2((device.Viewport.Width / 2) + 250, (device.Viewport.Height / 2) - 220), Color.Black);
                writeText("No. of Zombies : " + numZomb.ToString(), new Vector2((device.Viewport.Width / 2) - 100, (device.Viewport.Height / 2) - 220), Color.Black);
                writeText("Score: " + hitCount.ToString(), new Vector2((device.Viewport.Width / 2) - 350, (device.Viewport.Height / 2) - 220), Color.Black);
                spriteBatch.Draw(pauseButton,pauseButtonPosition, Color.White);

                if (mute == false)
                {
                    spriteBatch.Draw(muteButtonOn, muteButtonPosition, Color.White);
                }
                else if (mute == true)
                {
                    spriteBatch.Draw(muteButtonOff, muteButtonPosition, Color.White);
                }

                if (mySmokeParticleSystem.isActive)
                {
                    mySmokeParticleSystem.Draw();
                }

                if (myFireParticleSystem.isActive)
                {
                    myFireParticleSystem.Draw();
                }

                if (myBloodParticleSystem.isActive)
                {
                    myBloodParticleSystem.Draw();
                }
                
            }
            //draw the pause screen
            if (gameState == GameState.Paused)
            {
                spriteBatch.Draw(backGround, frame, Color.White);
                spriteBatch.Draw(resumeButton, resumeButtonPosition, Color.White);
                spriteBatch.Draw(htpButton, htpButtonPosition, Color.White);
                prev = GameState.Paused;
            }

            if (gameState == GameState.HowToPlay)
            {
                spriteBatch.Draw(htpBackGround, frame, Color.White);
                spriteBatch.Draw(backButton, backButtonPosition, Color.White);
            }

            if (gameState == GameState.EndGame)
            {

                if (health > 0)
                {
                    spriteBatch.Draw(backGround, frame, Color.White);
                    writeText("Your Score" + "\n" + "\n       " + points.ToString(), new Vector2((device.Viewport.Width / 2) - 60, (device.Viewport.Height / 2) - 50), Color.Red);
                }
                else
                {
                    spriteBatch.Draw(backGroundFail, frame, Color.White);
                  
                }
                spriteBatch.Draw(exitButton, backButtonPosition, Color.White);
            }

            spriteBatch.End();

            base.Draw(gameTime);
        
        }

        //this is a controller for where the mouse has clicked on screen
        void MouseClicked(int x, int y)
        {
            //creates a rectangle of 10x10 around the place where the mouse was clicked
            Rectangle mouseClickRect = new Rectangle(x, y, 10, 10);
            Rectangle startButtonRect = new Rectangle((int)startButtonPosition.X, (int)startButtonPosition.Y, 100, 20);
            Rectangle exitButtonRect = new Rectangle((int)exitButtonPosition.X, (int)exitButtonPosition.Y, 100, 20);
            Rectangle htpButtonRect = new Rectangle((int)htpButtonPosition.X, (int)htpButtonPosition.Y, 100, 20);
            Rectangle resumeButtonRect = new Rectangle((int)resumeButtonPosition.X, (int)resumeButtonPosition.Y, 100, 20);
            Rectangle backPauseButton = new Rectangle((int)backButtonPosition.X, (int)backButtonPosition.Y, 100, 20);
               
            //check the startmenu
            if (gameState == GameState.StartMenu)
            {
               
                if (mouseClickRect.Intersects(startButtonRect)) //player clicked start button
                {
                    gameState = GameState.Loading;
                    isLoading = false;
                }
                else if (mouseClickRect.Intersects(exitButtonRect)) //player clicked exit button
                {
                    Exit();
                }
                else if (mouseClickRect.Intersects(htpButtonRect))//player clicked how to play button
                {
                    gameState = GameState.HowToPlay;
                }
            }

            //check the pause and mute buttons
            if (gameState == GameState.Playing)
            {
                Rectangle pauseButtonRect = new Rectangle((int)pauseButtonPosition.X, (int)pauseButtonPosition.Y,100,50);

                if (mouseClickRect.Intersects(pauseButtonRect))
                {
                    gameState = GameState.Paused;
                }

                Rectangle muteButtonRect = new Rectangle((int)muteButtonPosition.X, (int)muteButtonPosition.Y, 100, 50);

                if (mouseClickRect.Intersects(muteButtonRect))
                {
                    if (mute == false)
                    {
                        SoundEffect.MasterVolume = 0;
                        MediaPlayer.IsMuted = true;
                        mute = true;
                    }
                    else if (mute == true)
                    {
                        SoundEffect.MasterVolume = 1;
                        MediaPlayer.IsMuted = false;
                        mute = false;
                    }
                }
            }

            //check the resumebutton
            if (gameState == GameState.Paused)
            {
                if (mouseClickRect.Intersects(resumeButtonRect))
                {
                    gameState = GameState.Playing;
                }
                else if (mouseClickRect.Intersects(htpButtonRect))
                {
                    gameState = GameState.HowToPlay;
                }
            }

            //check the how to play button
            if (gameState == GameState.HowToPlay) 
            {
                if (prev == GameState.Paused)
                {
                    if (mouseClickRect.Intersects(backPauseButton))
                    {
                        gameState = GameState.Paused;
                    }
                }
                else
                {
                    if (mouseClickRect.Intersects(backPauseButton))
                    {
                        gameState = GameState.StartMenu;
                    }
                }
            }

            //check the end game button
            if (gameState == GameState.EndGame) 
            {
                if (mouseClickRect.Intersects(backPauseButton)) //player clicked exit button
                {
                    Exit();
                }
            }
        }

        private void writeText(string msg, Vector2 msgPos, Color msgColour)
        {
            string output = msg;
            // Find the center of the string
            Vector2 FontOrigin = fontToUse.MeasureString(output) / 2;
            Vector2 FontPos = msgPos;
            // Draw the string
            spriteBatch.DrawString(fontToUse, output, FontPos, msgColour);
        }

        //Loads all of the contents needed in the main game
        void LoadGame()
        {
            //load the game images into the content pipeline
            pauseButton = Content.Load<Texture2D>("Textures\\pause");
            resumeButton = Content.Load<Texture2D>("Textures\\resume");
            muteButtonOff = Content.Load<Texture2D>("Textures\\SoundOff");
            muteButtonOn = Content.Load<Texture2D>("Textures\\SoundOn");
            resumeButtonPosition = new Vector2((device.Viewport.Width / 2) -50, (device.Viewport.Height / 2) - 50);
            pauseButtonPosition = new Vector2((device.Viewport.Width / 2) - 395, (device.Viewport.Height / 2) + 165);
            muteButtonPosition = new Vector2((device.Viewport.Width / 2) + 300, device.Viewport.Height / 2 + 165);
            //-------------------------------------------------------------
            // added to load font
            //-------------------------------------------------------------
            fontToUse = Content.Load<SpriteFont>(".\\Fonts\\DrWho");
            //-------------------------------------------------------------
            // added to load Song
            //-------------------------------------------------------------
            bkgMusic = Content.Load<Song>(".\\Audio\\gameMusic");
            MediaPlayer.Play(bkgMusic);
            MediaPlayer.IsRepeating = true;
            //-------------------------------------------------------------
            // added to load Model
            //-------------------------------------------------------------
            mdlRobot = Content.Load<Model>("Models\\BallDroid");
            mdlRobotTransforms = SetupEffectTransformDefaults(mdlRobot);
            mdlZombie = Content.Load<Model>("Models\\zombie");
            mdlZombieTransforms = SetupEffectTransformDefaults(mdlZombie);
            mdlBullet = Content.Load<Model>("Models\\bullet");
            mdlBulletTransforms = SetupEffectTransformDefaults(mdlBullet);
            mdlMissile = Content.Load<Model>("Models\\missile");
            mdlMissileTransforms = SetupEffectTransformDefaults(mdlMissile);
            //-------------------------------------------------------------
            // added to load SoundFX's
            //-------------------------------------------------------------
            robotSound = Content.Load<SoundEffect>("Audio\\robotInitial");
            zombieSound = Content.Load<SoundEffect>("Audio\\zombieGroan");
            machSound = Content.Load<SoundEffect>("Audio\\shot007");
            rpgSound = Content.Load<SoundEffect>("Audio\\missile007");
            robotSoundInstance = robotSound.CreateInstance();
            robotSoundInstance.Play();

            //add the effect and skybox
            effect = Content.Load<Effect>("Effects\\effects");
            skyboxModel = LoadModel("Models\\skybox", out skyboxTextures);

            ModelEffect = Content.Load<Effect>("Effects\\ModelShader");
            //add the terrain
            terrain = Content.Load<Model>("Models\\terrain");
            mdlTerrainTransforms = SetupEffectTransformDefaults(terrain);
           
            //set up the particles
            particleSystemManager = new ParticleSystemManager();
            mySmokeParticleSystem = new SmokeParticles(this);     
            myFireParticleSystem = new FireParticles(this);
            myBloodParticleSystem = new BloodParticles(this);
            particleSystemManager.AddParticleSystem(mySmokeParticleSystem);
            particleSystemManager.AddParticleSystem(myFireParticleSystem);
            particleSystemManager.AddParticleSystem(myBloodParticleSystem);

            Thread.Sleep(3000);

            //start playing
            gameState = GameState.Playing;
            isLoading = false;
        }
    }
}
