using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assimp;
using SharpGL;
using AssimpSample;
using SharpGL.Enumerations;
using SharpGL.SceneGraph.Primitives;
using SharpGL.SceneGraph.Quadrics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Threading;
using System.Threading;

namespace GrafikaProj
{


    class World
    {
        #region Private polja

        /// <summary>
        /// targert timeri i polja
        /// </summary>
        private DispatcherTimer timer1;
        private DispatcherTimer timer2;
        private int targetJumpCounter = 0;
        private float targetAnimationHeight = 0.0f;
        private bool targetsGoingUp;

        /// <summary>
        /// Pistolj tajmeri i polja za postavljanje pistolja u poziciju za pucanje
        /// </summary>
        private DispatcherTimer pistolTimer;
        private float pistolAnimationPosition = 0.0f;

        /// <summary>
        /// tajmeri i polja za animaciju pucanja pistolja
        /// </summary>
        private DispatcherTimer pucanjeTimer;
        private float pistolRotation = 0.0f;
        private int pistolFireCounter = 0;
        private bool pistolGoingUp;

        private float targetValueHeight = 200.0f;

        private float bulletCaliber = 10.0f;

        private float ambientR = 0.0f;
        private float ambientG = 0.0f;
        private float ambientB = 1.0f;
        private float ambientU = 1.0f;

        /// <summary>
        /// Ugao rotacije sveta oko X-ose.
        /// </summary>
        private float m_xRotation = 0.0f;

        /// <summary>
        /// Ugao rotacije sveta oko Y-ose.
        /// </summary>
        private float m_yRotation = 0.0f;

        /// <summary>
        /// Identifikatori tekstura
        /// </summary>
        private enum TextureObjects { Concrete = 0, Woood, Rust};
        private readonly int m_textureCount = Enum.GetNames(typeof(TextureObjects)).Length;

        /// <summary>
        /// Indentifikatori OpenGL tekstura
        /// </summary>
        private uint[] m_textures = null;

        /// <summary>
        /// Putanje do fajlova za teksture
        /// </summary>
        private string[] m_textureFiles = { "..//..//images//concrete.jpg", "..//..//images/wood.jpg", "..//..//images/rust.jpg" };

        /// <summary>
        /// Indikator stanja mehanizma za iscrtavanje nevidljivih povrsina.
        /// </summary>
        bool m_culling = true;

        /// <summary>
        /// Indikator stanja mehanizma za testiranje dubine.
        /// </summary>
        bool m_depthTesting = true;

        /// <summary>
        /// Sirina openGL kontrole. 
        /// Pikseli
        /// </summary>
        private int m_width;

        /// <summary>
        /// Visina openGL kontrole
        /// Pikseli
        /// </summary>
        private int m_height;

        private AssimpScene m_scene;
        private AssimpScene m_bullet;

        private float m_sceneDistance = 7000.0f;

        #endregion

        #region Public polja

        public float AmbientR
        {
            get
            {
                return ambientR;
            }
            set
            {
                ambientR = value;
            }
        }

        public float AmbientG
        {
            get
            {
                return ambientG;
            }
            set
            {
                ambientG = value;
            }
        }

        public float AmbientB
        {
            get
            {
                return ambientB;
            }
            set
            {
                ambientB = value;
            }
        }

        public float AmbientU
        {
            get
            {
                return ambientU;
            }
            set
            {
                ambientU = value;
            }
        }

        /// <summary>
        /// Odredjuje visinu cilindara koji su mete
        /// </summary>
        public float TargetValueHeight
        {
            get
            {
                return targetValueHeight;
            }
            set
            {
                targetValueHeight = value;
            }
        }

        public float BulletCaliber
        {
            get
            {
                return bulletCaliber;
            }
            set
            {
                bulletCaliber = value;
            }
        }

        /// <summary>
        /// Scena
        /// </summary>
        public AssimpScene Scene
        {
            get { return m_scene; }
            set { m_scene = value; }
        }

        public AssimpScene Bullet
        {
            get { return m_bullet; }
            set { m_bullet = value; }
        }

        public float SceneDistance
        {
            get { return m_sceneDistance; }
            set { m_sceneDistance = value; }
        }
        public int Width
        {
            get { return m_width; }
            set { m_width = value; }
        }

        public int Height
        {
            get { return m_height; }
            set { m_height = value; }
        }

        /// <summary>
        /// Rotacija oko X ose
        /// </summary>
        public float RotationX
        {
            get { return m_xRotation; }
            set { m_xRotation = value; }
        }

        /// <summary>
        /// Rotacija oko Y ose
        /// </summary>
        public float RotationY
        {
            get { return m_yRotation; }
            set { m_yRotation = value; }
        }

        /// <summary>
        /// Iscrtavanje mehanizma nevidljivih povrsina
        /// </summary>
        public bool Culling
        {
            get { return m_culling; }
            set { m_culling = value; }
        }

        public bool DepthTesting
        {
            get { return m_depthTesting; }
            set { m_depthTesting = value; }
        }
        #endregion
        public World(String scenePath, String sceneFileName, String bulletPath, String BulletFileName, int height, int width, OpenGL gl)
        {
            this.m_scene = new AssimpScene(scenePath, sceneFileName, gl);
            this.m_bullet = new AssimpScene(bulletPath, BulletFileName, gl);
            this.m_height = height;
            this.m_width = width;
            m_textures = new uint[m_textureCount];
            

        }

        ~World()
        {
            this.Dispose(false);
        }

        #region Metode
        public void startTargetTimers()
        {
            targetAnimationHeight = 0.0f;
            targetsGoingUp = true;

            this.timer1 = new DispatcherTimer();
            timer1.Interval = TimeSpan.FromMilliseconds(20);
            timer1.Tick += new EventHandler(UpdateAnimation1);
            timer1.Start();

            this.timer2 = new DispatcherTimer();
            timer2.Interval = TimeSpan.FromMilliseconds(3f);
            timer2.Tick += new EventHandler(UpdateAnimation2);
            timer2.Start();
        }

        public void stopTargetTimers()
        {
            this.timer1.Stop();
            this.timer2.Stop();
        }

        private void UpdateAnimation1(object sender, EventArgs e)
        {
            if (targetsGoingUp)
            {
                targetAnimationHeight += 50.0f;
            }
            else
            {
                targetAnimationHeight -= 50.0f;
            }

        }

       

        private void UpdateAnimation2(object sender, EventArgs e)
        {
            if (!targetsGoingUp)
            {
                targetAnimationHeight = 0.0f;
                
            }
            
            targetsGoingUp = !targetsGoingUp;
        }

        public void startPistolAnimation()
        {
            this.pistolTimer = new DispatcherTimer();
            pistolTimer.Interval = TimeSpan.FromMilliseconds(20);
            pistolTimer.Tick += new EventHandler(PostaviPistolj);
            pistolTimer.Start();
        }

        private void PostaviPistolj(object sender, EventArgs e)
        {
            pistolAnimationPosition += 30.0f;
            if (pistolAnimationPosition%300 == 0)
            {
                pistolTimer.Stop();
                
                StartFireAnimation();
            }
        }

        public void StartFireAnimation()
        {
            this.pucanjeTimer = new DispatcherTimer();
            pucanjeTimer.Interval = TimeSpan.FromMilliseconds(20);
            pucanjeTimer.Tick += new EventHandler(RotirajPistolj);
            pucanjeTimer.Start();
        }

        private void RotirajPistolj(object sender, EventArgs e)
        {
            pistolRotation += 5.0f;
            if(pistolRotation == 45.0f)
            {
                pistolRotation = 0.0f;
                pistolFireCounter += 1;
            }
            if(pistolFireCounter == 3)
            {
                pucanjeTimer.Stop();
                startTargetTimers(); 
            }
        }

        public void Initialize(OpenGL gl)
        {

            //gl.ClearColor(0.0f, 0.0f, 0.0f, 1.0f);
            //gl.Color(0.5f, 0.5f, 0.5f);
            // Model sencenja na flat (konstantno)
            gl.ShadeModel(OpenGL.GL_FLAT);
            gl.Enable(OpenGL.GL_DEPTH_TEST);
            gl.Enable(OpenGL.GLU_CULLING);
            gl.FrontFace(OpenGL.GL_CCW);
            gl.Enable(OpenGL.GL_COLOR_MATERIAL);
            gl.ColorMaterial(OpenGL.GL_FRONT, OpenGL.GL_AMBIENT_AND_DIFFUSE);

            setupLighting(gl);
            

            

            gl.TexParameter(OpenGL.GL_TEXTURE_2D, OpenGL.GL_TEXTURE_MIN_FILTER, OpenGL.GL_NEAREST);     // Nearest Neighbour Filtering
            gl.TexParameter(OpenGL.GL_TEXTURE_2D, OpenGL.GL_TEXTURE_MAG_FILTER, OpenGL.GL_NEAREST);     // Nearest Neighbour Filtering
            gl.TexParameter(OpenGL.GL_TEXTURE_2D, OpenGL.GL_TEXTURE_WRAP_S, OpenGL.GL_REPEAT);
            gl.TexParameter(OpenGL.GL_TEXTURE_2D, OpenGL.GL_TEXTURE_WRAP_T, OpenGL.GL_REPEAT);
            
            gl.TexEnv(OpenGL.GL_TEXTURE_ENV, OpenGL.GL_TEXTURE_ENV_MODE, OpenGL.GL_ADD);

            gl.Enable(OpenGL.GL_TEXTURE_2D);

            gl.GenTextures(m_textureCount, m_textures);
            for(int i = 0; i < m_textureCount; ++i)
            {
                //Pridruzuje se tekstura odredjenom identifikatoru
                gl.BindTexture(OpenGL.GL_TEXTURE_2D, m_textures[i]);

                // Ucitaj sliku i podesi parametre teksture
                Bitmap image = new Bitmap(m_textureFiles[i]);
                // rotiramo sliku zbog koordinantog sistema opengl-a
                image.RotateFlip(RotateFlipType.RotateNoneFlipY);
                Rectangle rect = new Rectangle(0, 0, image.Width, image.Height);
                // RGBA format (dozvoljena providnost slike tj. alfa kanal)
                BitmapData imageData = image.LockBits(rect, System.Drawing.Imaging.ImageLockMode.ReadOnly,
                                                      System.Drawing.Imaging.PixelFormat.Format32bppArgb);

                gl.Build2DMipmaps(OpenGL.GL_TEXTURE_2D, (int)OpenGL.GL_RGBA8, image.Width, image.Height, OpenGL.GL_BGRA, OpenGL.GL_UNSIGNED_BYTE, imageData.Scan0);
                //gl.TexParameter(OpenGL.GL_TEXTURE_2D, OpenGL.GL_TEXTURE_MIN_FILTER, OpenGL.GL_NEAREST);		// Nearest Neighbour Filtering
                //gl.TexParameter(OpenGL.GL_TEXTURE_2D, OpenGL.GL_TEXTURE_MAG_FILTER, OpenGL.GL_NEAREST);		// Nearest Neighbour Filtering
                //gl.TexParameter(OpenGL.GL_TEXTURE_2D, OpenGL.GL_TEXTURE_WRAP_S, OpenGL.GL_REPEAT);
                //gl.TexParameter(OpenGL.GL_TEXTURE_2D, OpenGL.GL_TEXTURE_WRAP_T, OpenGL.GL_REPEAT);

                //gl.Enable(OpenGL.GL_TEXTURE_GEN_S);
                //gl.Enable(OpenGL.GL_TEXTURE_GEN_T);

                image.UnlockBits(imageData);
                image.Dispose();
            }

            m_scene.LoadScene();
            m_scene.Initialize();

            m_bullet.LoadScene();
            m_bullet.Initialize();

            
        }

        /// <summary>
        ///  Funkcija ograničava vrednost na opseg min - max
        /// </summary>
        public static float Clamp(float value, float min, float max)
        {
            return (value < min) ? min : (value > max) ? max : value;
        }

        /// <summary>
        /// Podesavanja viewport-a i perspektive
        /// </summary>
        public void Resize(OpenGL gl, int width, int height)
        {
            m_width = width;
            m_height = height;
            gl.Viewport(0, 0, width, height);
            gl.MatrixMode(OpenGL.GL_PROJECTION);      // selektuj Projection Matrix
            gl.LoadIdentity();
            gl.Perspective(50f, (double) width/height, 1, 20000);
            gl.MatrixMode(OpenGL.GL_MODELVIEW);
            gl.LoadIdentity();


        }

        public void Draw(OpenGL gl)
        {
            gl.Clear(OpenGL.GL_COLOR_BUFFER_BIT | OpenGL.GL_DEPTH_BUFFER_BIT);

            gl.PushMatrix();
            
            gl.Translate(0.0f, 0.0f, -m_sceneDistance);
            gl.Rotate(m_xRotation, 1.0f, 0.0f, 0.0f);
            gl.Rotate(m_yRotation, 0.0f, 1.0f, 0.0f);
            gl.LookAt(100.0f, 250.0f, 650.0f,
                       0.0f, 250.0f, 650.0f,
                       0.0f, 1.0f, 0.0f);

            TestKockica(gl, 0.0f, 800.0f, -700.0f);
            DrawGround(gl);

            DrawPistol(gl);

            DrawCube(gl);

            DrawCilinder(gl);

            DrawBullet(gl);

            
            Draw3DText(gl);
            

            gl.PopMatrix();

            

            gl.Flush();
        }

        /// <summary>
        /// Podesava osvetljenje u scecni
        /// </summary>
        /// <param name="gl"></param>
        public void setupLighting(OpenGL gl)
        {
            

            float[] ambijentalnaKomponenta = { 1.0f, 1.0f, 1.0f, 1.0f };
            float[] difuznaKomponenta = { 1.0f, 1.0f, 1.0f, 1.0f };
            //float[] specularnaKomponenta = { 1.0f, 1.0f, 1.0f, 1.0f };
            //float[] lightPos0 = { 600.0f, -300.0f, 650.0f };
            float[] lightPos0 = { 500.0f, 500.0f, 850.0f };
            //pridruzivanje ambijentalne i difuzne komponente svetlosnom izvoru LIGHT0
            gl.Light(OpenGL.GL_LIGHT0, OpenGL.GL_SPOT_CUTOFF, 180.0f);
            gl.Light(OpenGL.GL_LIGHT0, OpenGL.GL_POSITION, lightPos0);
            gl.Light(OpenGL.GL_LIGHT0, OpenGL.GL_AMBIENT, ambijentalnaKomponenta);
            gl.Light(OpenGL.GL_LIGHT0, OpenGL.GL_DIFFUSE, difuznaKomponenta);
            //gl.Light(OpenGL.GL_LIGHT0, OpenGL.GL_SPECULAR, specularnaKomponenta);
            //podesavanje cuttoff-a na 180 da bi svetlost bila tackasta
            

            gl.Enable(OpenGL.GL_LIGHTING);
            gl.Enable(OpenGL.GL_LIGHT0);


            

            float[] ambijentalnaKomponentaBlue = { ambientR, ambientG, ambientB, 1.0f };
            float[] difuznaKomponentaBlue = { 0.0f, 0.0f, 1.0f, 1.0f };
           // float[] specularnaKomponentaBlue = { 0.0f, 0.0f, 1.0f, 1.0f };
            float[] lightPos1 = { 0.0f, 800.0f, -700.0f, 1.0f };
            //float[] lightPos1 = { 500f, 300.0f, -700.0f, 1.0f };
            

            float[] smer = { 0.0f, 0.0f, -1.0f };
            gl.Light(OpenGL.GL_LIGHT1, OpenGL.GL_SPOT_DIRECTION, smer);
            gl.Light(OpenGL.GL_LIGHT1, OpenGL.GL_SPOT_CUTOFF, 25.0f);
            gl.Light(OpenGL.GL_LIGHT1, OpenGL.GL_POSITION, lightPos1);
            gl.Light(OpenGL.GL_LIGHT1, OpenGL.GL_AMBIENT, ambijentalnaKomponentaBlue);
            gl.Light(OpenGL.GL_LIGHT1, OpenGL.GL_DIFFUSE, difuznaKomponentaBlue);
            //gl.Light(OpenGL.GL_LIGHT1, OpenGL.GL_SPECULAR, specularnaKomponentaBlue);
            // Podesi parametre reflektorkskog izvora



            gl.Enable(OpenGL.GL_LIGHTING);
            gl.Enable(OpenGL.GL_LIGHT1);

            gl.Enable(OpenGL.GL_NORMALIZE);

            
        }

        public void setupTargetLight(OpenGL gl)
        {
            

            float[] ambijentalnaKomponenta = { ambientR, ambientG, ambientB, 1.0f };
            float[] difuznaKomponenta = { 0.0f, 0.0f, 1.0f, 1.0f };
            float[] specularnaKomponenta = { 0.0f, 0.0f, 1.0f, 1.0f };
            float[] lightPos1 = { 0.0f, 500.0f, -700.0f, 1.0f };
            //float[] lightPos1 = { 500f, 300.0f, -700.0f, 1.0f };
            

            float[] smer = { 0.0f, -1.0f, 0.0f };
            gl.Light(OpenGL.GL_LIGHT1, OpenGL.GL_SPOT_DIRECTION, smer);
            gl.Light(OpenGL.GL_LIGHT1, OpenGL.GL_SPOT_CUTOFF, 25.0f);
            gl.Light(OpenGL.GL_LIGHT1, OpenGL.GL_POSITION, lightPos1);
            gl.Light(OpenGL.GL_LIGHT1, OpenGL.GL_AMBIENT, ambijentalnaKomponenta);
            gl.Light(OpenGL.GL_LIGHT1, OpenGL.GL_DIFFUSE, difuznaKomponenta);
            gl.Light(OpenGL.GL_LIGHT1, OpenGL.GL_SPECULAR, specularnaKomponenta);
            // Podesi parametre reflektorkskog izvora
            
            

            gl.Enable(OpenGL.GL_LIGHTING);
            gl.Enable(OpenGL.GL_LIGHT1);
            
            gl.Enable(OpenGL.GL_NORMALIZE);
            
        }

        public void TestKockica(OpenGL gl, float a, float b, float c)
        {
            gl.PushMatrix();

            gl.Translate(a, b, c);

            gl.Color(1.0f, 0.0f, 0.0f);

            

            gl.Rotate(-90.0f, 0.0f, 0.0f);
            gl.Scale(10, 10, 10);
            Cylinder cil = new Cylinder();
           
            cil.TopRadius = cil.BaseRadius;
            cil.CreateInContext(gl);

            cil.Render(gl, SharpGL.SceneGraph.Core.RenderMode.Render);

            gl.PopMatrix();
        }

        /// <summary>
        /// Draws pistol model
        /// </summary>
        /// <param name="gl"></param>
        public void DrawPistol(OpenGL gl)
        {
            

            gl.PushMatrix();
            
            gl.Translate(0.0f, 250.0f, 650.0f + pistolAnimationPosition);
            gl.Rotate(pistolRotation, 0.0f, 0.0f);
            gl.Scale(1.2f, 1.2f, 1.2f);
            gl.BindTexture(OpenGL.GL_TEXTURE_2D, m_textures[(int)TextureObjects.Woood]);

            //gl.Enable(OpenGL.GL_TEXTURE_GEN_S);
            //gl.Enable(OpenGL.GL_TEXTURE_GEN_T);
            
            m_scene.Draw();
            
            //gl.Disable(OpenGL.GL_TEXTURE_GEN_S);
            //gl.Disable(OpenGL.GL_TEXTURE_GEN_T);
            
            gl.PopMatrix();
            
        }

        public void DrawBullet(OpenGL gl)
        {

            gl.PushMatrix();
            gl.Translate(0.0f, 400.0f, 0.0f);
            gl.Scale(bulletCaliber, bulletCaliber, 10);
            gl.Color(1.0f, 0.0f, 1.0f);
            
            
            m_bullet.Draw();

            gl.PopMatrix();
        }

        /// <summary>
        /// Draws ground using GL_QUADS
        /// </summary>
        /// <param name="gl"></param>
        public void DrawGround(OpenGL gl)
        {
            

            gl.PushMatrix();

            gl.BindTexture(OpenGL.GL_TEXTURE_2D, m_textures[(int)TextureObjects.Concrete]);
            gl.Begin(OpenGL.GL_QUADS);

            gl.Color(0.5f, 0.5f, 0.5f);
            gl.Normal(0.0f, -1.0f, 0.0f);
            gl.TexCoord(0.0f, 0.0f);
            gl.Vertex(-1000.0f, 0.0f, 1000.0f);
            gl.TexCoord(0.0f, 1.0f);
            gl.Vertex(1000.0f, 0.0f, 1000.0f);
            gl.TexCoord(1.0f, 1.0f);
            gl.Vertex(1000.0f, 0.0f, -1000.0f);
            gl.TexCoord(1.0f, 0.0f);
            gl.Vertex(-1000.0f, 0.0f, -1000.0f);

            

            gl.End();


            gl.PopMatrix();
        }

        public void DrawCube(OpenGL gl)
        {

            gl.PushMatrix();
            gl.Translate(0.0f, 160f, -700.0f);
            gl.Scale(300, 150, 100);
            gl.Color(1.0f, 1.0f, 0.0f);
            Cube cube = new Cube();
            cube.Render(gl, SharpGL.SceneGraph.Core.RenderMode.Render);
            gl.PopMatrix();

        }


        public void DrawCilinder(OpenGL gl)
        {
            float specificCubeHeight;
            specificCubeHeight = targetAnimationHeight;
            //crtanje prve konzerve
            gl.PushMatrix();

            

            
           // specificCubeHeight = Clamp(specificCubeHeight, 0.0f, 3.0f);

            //gl.Translate(0.0f, 300.0f + targetValueHeight, -700.0f);
            gl.Translate(0.0f, 300.0f + specificCubeHeight, -700.0f);
            
            gl.Color(0.0f, 0.0f, 0.0f);
           
            //gl.Translate(0.0f, 200.0f, -1000.0f);
            gl.BindTexture(OpenGL.GL_TEXTURE_2D, m_textures[(int)TextureObjects.Rust]);
            
           //// 
            
            gl.Rotate(-90.0f, 0.0f, 0.0f);
            gl.Scale(50, 50, targetValueHeight);
            Cylinder cil = new Cylinder();
            cil.TextureCoords = true;
            cil.TopRadius = cil.BaseRadius;
            cil.CreateInContext(gl);
            
            cil.Render(gl, SharpGL.SceneGraph.Core.RenderMode.Render);
            
            gl.PopMatrix();

            //crtanje druge konzerve

            gl.PushMatrix();
            gl.Translate(150.0f, 300.0f + specificCubeHeight, -700.0f);
            gl.Rotate(-90.0f, 0.0f, 0.0f);
            //gl.Color(0.5f, 0.5f, 1.0f);
            gl.Scale(50, 50, targetValueHeight);

            //gl.Translate(0.0f, 200.0f, -1000.0f);
            gl.BindTexture(OpenGL.GL_TEXTURE_2D, m_textures[(int)TextureObjects.Rust]);

            Cylinder cil1 = new Cylinder();
            cil1.TopRadius = cil.BaseRadius;
            cil1.TextureCoords = true;
            cil1.CreateInContext(gl);

            cil1.Render(gl, SharpGL.SceneGraph.Core.RenderMode.Render);
            

            gl.PopMatrix();

            //crtanje trece konzerve

            gl.PushMatrix();
            gl.Translate(-150.0f, 300.0f + specificCubeHeight, -700.0f);
            gl.Rotate(-90.0f, 0.0f, 0.0f);
            //gl.Color(0.5f, 0.5f, 1.0f);
            gl.Scale(50, 50, targetValueHeight);
            gl.BindTexture(OpenGL.GL_TEXTURE_2D, m_textures[(int)TextureObjects.Rust]);
            gl.TexCoord(1.0f, 0.0f);
            //gl.Translate(0.0f, 200.0f, -1000.0f);
            Cylinder cil2 = new Cylinder();
            cil2.TextureCoords = true;
            cil2.TopRadius = cil.BaseRadius;
            cil2.CreateInContext(gl);

            cil2.Render(gl, SharpGL.SceneGraph.Core.RenderMode.Render);

            gl.PopMatrix();

        }

        public void Draw3DText(OpenGL gl)
        {
            
            gl.Viewport(m_width / 2, 0, m_width / 2, m_height / 2);
            
            gl.PushMatrix();
            
            gl.Translate(0.0f, 0.0f, 0.0f);
            gl.Color(1.0f, 1.0f, 1.0f);
            gl.Scale(300, 300, 300);
            gl.DrawText3D("Verdana italic", 12f, 1f, 0.1f, "Predmet: Racunarska grafika");
            
            gl.PopMatrix();

            gl.PushMatrix();
            
            gl.Translate(0.0f, -200.0f, 0.0f);
            gl.Color(1.0f, 1.0f, 1.0f);
            gl.Scale(300, 300, 300);
            gl.DrawText3D("Verdana italic", 12f, 1f, 0.1f, "Sk.god: 2017/18");

            gl.PopMatrix();

            gl.PushMatrix();
            
            gl.Translate(0.0f, -400.0f, 0.0f);
            gl.Color(1.0f, 1.0f, 1.0f);
            gl.Scale(300, 300, 300);
            gl.DrawText3D("Verdana italic", 12f, 1f, 0.1f, "Ime: Arsenije");

            gl.PopMatrix();

            gl.PushMatrix();
            
            gl.Translate(0.0f, -600.0f, 0.0f);
            gl.Color(1.0f, 1.0f, 1.0f);
            gl.Scale(300, 300, 300);
            gl.DrawText3D("Verdana italic", 12f, 1f, 0.1f, "Prezime: Karpic");

            gl.PopMatrix();

            gl.PushMatrix();
            
            gl.Translate(0.0f, -800.0f, 0.0f);
            gl.Color(1.0f, 1.0f, 1.0f);
            gl.Scale(300, 300, 300);
            gl.DrawText3D("Verdana italic", 12f, 1f, 0.1f, "Sifra zad: 19.2");

            gl.PopMatrix();

            gl.Viewport(0, 0, m_width, m_height); //resetuj na stari view port
            
        }
        #endregion

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                m_scene.Dispose();
            }
        }

        

        #region IDisposable metode

        /// <summary>
        ///  Dispose metoda.
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion IDisposable metode
    }
}