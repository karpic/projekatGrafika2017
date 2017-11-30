﻿using System;
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

namespace GrafikaProj
{


    class World
    {
        #region Private polja

        private float[] groundVertices = new float[]
        {
            -0.5f, 0.5f, 0.0f, //top left
            0.5f, 0.5f, 0.0f, // top right
            0.5f, -0.5f, 0.0f //bottom right
            -0.5f, -0.5f, 0.0f //bottom left
        };

        /// <summary>
        /// Ugao rotacije sveta oko X-ose.
        /// </summary>
        private float m_xRotation = 0.0f;

        /// <summary>
        /// Ugao rotacije sveta oko Y-ose.
        /// </summary>
        private float m_yRotation = 0.0f;

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

        private float m_sceneDistance = 7000.0f;

        #endregion

        #region Public polja

        /// <summary>
        /// Scena
        /// </summary>
        public AssimpScene Scene
        {
            get { return m_scene; }
            set { m_scene = value; }
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
        public World(String scenePath, String sceneFileName, int height, int width, OpenGL gl)
        {
            this.m_scene = new AssimpScene(scenePath, sceneFileName, gl);
            this.m_height = height;
            this.m_width = width;


        }

        ~World()
        {
            this.Dispose(false);
        }

        #region Metode
        public void Initialize(OpenGL gl)
        {
            gl.ClearColor(0.0f, 0.0f, 0.0f, 1.0f);
            gl.Color(0.5f, 0.5f, 0.5f);
            // Model sencenja na flat (konstantno)
            gl.ShadeModel(OpenGL.GL_FLAT);
            if (m_depthTesting)
            {
                gl.Enable(OpenGL.GL_DEPTH_TEST);
            }
            if (m_culling)
            {
                gl.Enable(OpenGL.GLU_CULLING);
            }
            m_scene.LoadScene();
            m_scene.Initialize();
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
            gl.MatrixMode(OpenGL.GL_MODELVIEW);
            gl.LoadIdentity();
            gl.Translate(0.0f, 0.0f, -m_sceneDistance);
            gl.Rotate(m_xRotation, 1.0f, 0.0f, 0.0f);
            gl.Rotate(m_yRotation, 0.0f, 1.0f, 0.0f);

            DrawGround(gl);

            DrawPistol(gl);

            DrawCube(gl);
            DrawCilinder(gl);
            gl.PopMatrix();
            

            gl.Flush();
        }

        /// <summary>
        /// Draws pistol model
        /// </summary>
        /// <param name="gl"></param>
        public void DrawPistol(OpenGL gl)
        {
            

            gl.PushMatrix();

            m_scene.Draw();
            
            gl.PopMatrix();
        }

        /// <summary>
        /// Draws ground using GL_QUADS
        /// </summary>
        /// <param name="gl"></param>
        public void DrawGround(OpenGL gl)
        {
            

            gl.PushMatrix();
            gl.Begin(OpenGL.GL_QUADS);


           
            gl.Vertex(-1000.0f, 0.0f, 1000.0f);
            gl.Vertex(1000.0f, 0.0f, 1000.0f);
            gl.Vertex(1000.0f, 0.0f, -1000.0f);
            gl.Vertex(-1000.0f, 0.0f, -1000.0f);


            gl.End();


            gl.PopMatrix();
        }

        public void DrawCube(OpenGL gl)
        {

            gl.PushMatrix();
            gl.Translate(0.0f, 0.0f, -700.0f);
            gl.Scale(100, 150, 100);
            gl.Color(1.0f, 1.0f, 0.0f);
            Cube cube = new Cube();
            cube.Render(gl, SharpGL.SceneGraph.Core.RenderMode.Render);
            gl.PopMatrix();

        }


        public void DrawCilinder(OpenGL gl)
        {
            gl.PushMatrix();
            gl.Color(0.0f, 1.0f, 1.0f);
            gl.Scale(100, 100, 100);
            Cylinder cil = new Cylinder();
            cil.CreateInContext(gl);
            cil.Render(gl, SharpGL.SceneGraph.Core.RenderMode.Render);
            gl.Disable(OpenGL.GL_LIGHT0);
            gl.Disable(OpenGL.GL_LIGHTING);
            gl.PopMatrix();
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