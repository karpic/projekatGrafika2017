using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.IO;
using System.Reflection;
using SharpGL.SceneGraph;
using Microsoft.Win32;
using System.Globalization;

namespace GrafikaProj
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        #region Atributi

        /// <summary>
        ///	 Instanca OpenGL "sveta" - klase koja je zaduzena za iscrtavanje koriscenjem OpenGL-a.
        /// </summary>
        World m_world = null;

        

        #endregion Atributi

        #region Konstruktori

        public MainWindow()
        {
            // Inicijalizacija komponenti
            InitializeComponent();

            // Kreiranje OpenGL sveta
            try
            {
                m_world = new World(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "model\\pistolj"), "pistol.3DS", Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "model\\metak"),"Bullet.3ds", (int)openGLControl.Width, (int)openGLControl.Height, openGLControl.OpenGL);
            }
            catch (Exception e)
            {
                MessageBox.Show("Neuspesno kreirana instanca OpenGL sveta. Poruka greške: " + e.Message, "Poruka", MessageBoxButton.OK);
                this.Close();
            }
        }

        #endregion Konstruktori

        /// <summary>
        /// Handles the OpenGLDraw event of the openGLControl1 control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="args">The <see cref="SharpGL.SceneGraph.OpenGLEventArgs"/> instance containing the event data.</param>
        private void openGLControl_OpenGLDraw(object sender, OpenGLEventArgs args)
        {
            m_world.Draw(args.OpenGL);
        }

        /// <summary>
        /// Handles the OpenGLInitialized event of the openGLControl1 control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="args">The <see cref="SharpGL.SceneGraph.OpenGLEventArgs"/> instance containing the event data.</param>
        private void openGLControl_OpenGLInitialized(object sender, OpenGLEventArgs args)
        {
            m_world.Initialize(args.OpenGL);
        }

        /// <summary>
        /// Handles the Resized event of the openGLControl1 control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="args">The <see cref="SharpGL.SceneGraph.OpenGLEventArgs"/> instance containing the event data.</param>
        private void openGLControl_Resized(object sender, OpenGLEventArgs args)
        {
            m_world.Resize(args.OpenGL, (int)openGLControl.ActualWidth, (int)openGLControl.ActualHeight);
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.F10: this.Close(); break;
                case Key.W: m_world.RotationX -= 5.0f; break;
                case Key.S: m_world.RotationX += 5.0f; break;
                case Key.A: m_world.RotationY -= 5.0f; break;
                case Key.D: m_world.RotationY += 5.0f; break;
                case Key.Add: m_world.SceneDistance -= 700.0f; break;
                case Key.Subtract: m_world.SceneDistance += 700.0f; break;
                case Key.X: m_world.startPistolAnimation(); break;
               /* case Key.F2:
                    OpenFileDialog opfModel = new OpenFileDialog();
                    bool result = (bool)opfModel.ShowDialog();
                    if (result)
                    {

                        try
                        {
                            World newWorld = new World(Directory.GetParent(opfModel.FileName).ToString(), Path.GetFileName(opfModel.FileName), (int)openGLControl.Width, (int)openGLControl.Height, openGLControl.OpenGL);
                            m_world.Dispose();
                            m_world = newWorld;
                            m_world.Initialize(openGLControl.OpenGL);
                        }
                        catch (Exception exp)
                        {
                            MessageBox.Show("Neuspesno kreirana instanca OpenGL sveta:\n" + exp.Message, "GRESKA", MessageBoxButton.OK);
                        }
                    }
                    break;*/
            }
        }



       

        private void targetHup(object sender, RoutedEventArgs e)
        {
            if (m_world != null)
            {
                m_world.TargetValueHeight = m_world.TargetValueHeight + 100.0f;
            }
        }

        private void targetHdown(object sender, RoutedEventArgs e)
        {
            if (m_world != null)
            {
                m_world.TargetValueHeight = m_world.TargetValueHeight - 100.0f;
            }
        }

        private void calibarVUp(object sender, RoutedEventArgs e)
        {
            if (m_world != null)
            {
                m_world.BulletCaliber = m_world.BulletCaliber + 10.0f;
            }
        }

        private void calibarDown_Click(object sender, RoutedEventArgs e)
        {
            if (m_world != null)
            {
                m_world.BulletCaliber = m_world.BulletCaliber - 10.0f;
            }
        }

        private void ambientRUp_Click(object sender, RoutedEventArgs e)
        {
            if(m_world != null)
            {
                m_world.AmbientR += 0.2f;
            }
        }

        private void ambientRDown_Click(object sender, RoutedEventArgs e)
        {
            if (m_world != null)
            {
                m_world.AmbientR -= 0.2f;    
            }
        }

        private void ambientGUp_Click(object sender, RoutedEventArgs e)
        {
            if (m_world != null)
            {
                m_world.AmbientG += 0.2f;
            }
        }

        private void ambientGDown_Click(object sender, RoutedEventArgs e)
        {
            if (m_world != null)
            {
                m_world.AmbientG -= 0.2f;
            }
        }

        private void ambientBUp_Click(object sender, RoutedEventArgs e)
        {
            if (m_world != null)
            {
                m_world.AmbientB += 0.2f;
            }
        }

        private void ambientBDown_Click(object sender, RoutedEventArgs e)
        {
            if (m_world != null)
            {
                m_world.AmbientB -= 0.2f;
            }
        }
    }
}
