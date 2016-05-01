using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;

using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;

using Cobalt;
using Cobalt.Mesh;

namespace CSGeom
{
    public partial class MainForm : Form
    {
        static readonly string projectionMatrixName = "projection_matrix";
        static readonly string modelviewMatrixName = "modelview_matrix";

        static readonly string materialTextureName = "material_texture";
        static readonly string materialAmbientName = "material_ambient";
        static readonly string materialDiffuseName = "material_diffuse";
        static readonly string materialSpecularName = "material_specular";

        static readonly string sunPositionName = "sun_position";
        static readonly string sunLightColorName = "sun_lightColor";
        static readonly string ambientIntensityName = "ambient_intensity";

        static readonly string discardOpaqueName = "discard_opaque";
        static readonly string enableLightName = "enable_light";

        Vector3 eye, target;
        float scale;
        Matrix4 modelviewMatrix;

        Camera camera;
        Shader shader;
        Cobalt.Font font;

        OpenTK.Input.KeyboardState lastKbd;
        bool wireframe, culling, lighting;

        bool takeScreenshot;
        string screenshotPath;

        float sunAngle;
        string lastGeomFile;
        GeomFile geom;
        List<Mesh> meshes;

        public MainForm()
        {
            InitializeComponent();
        }

        private void renderControl_Load(object sender, EventArgs e)
        {
            eye = new Vector3(0.0f, 0.0f, 15.0f);
            target = new Vector3(0.0f, 0.0f, 0.0f);
            scale = 1.0f;
            modelviewMatrix = Matrix4.CreateScale(scale) * Matrix4.LookAt(eye, target, Vector3.UnitY);

            camera = new Camera();
            shader = new Shader(
                File.ReadAllText(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Assets\\VertexShader.glsl")),
                File.ReadAllText(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Assets\\FragmentShader.glsl")));
            font = new Cobalt.Font("DejaVu Sans");

            lastKbd = OpenTK.Input.Keyboard.GetState();
            wireframe = false;
            culling = true;
            lighting = true;

            takeScreenshot = false;

            if (shader != null)
            {
                shader.SetUniformName(ShaderCommonUniform.ProjectionMatrix, projectionMatrixName);
                shader.SetUniformName(ShaderCommonUniform.ModelViewMatrix, modelviewMatrixName);

                shader.SetUniformName(ShaderCommonUniform.MaterialTexture, materialTextureName);
                shader.SetUniformName(ShaderCommonUniform.MaterialAmbientColor, materialAmbientName);
                shader.SetUniformName(ShaderCommonUniform.MaterialDiffuseColor, materialDiffuseName);
                shader.SetUniformName(ShaderCommonUniform.MaterialSpecularColor, materialSpecularName);

                shader.SetUniform(ShaderCommonUniform.MaterialTexture, (int)0);
                shader.SetUniform(sunLightColorName, new Vector3(1.0f, 1.0f, 1.0f));
                shader.SetUniform(ambientIntensityName, 0.5f);
            }

            GL.Enable(EnableCap.DepthTest);
            GL.Enable(EnableCap.CullFace);

            sunAngle = 45.0f;

            StartupDebugShortcuts();
        }

        [System.Diagnostics.Conditional("DEBUG")]
        private void StartupDebugShortcuts()
        {
            // debugging shortcuts
            if (Environment.MachineName == "NANAMI-X")
            {
                openGeomToolStripMenuItem.Enabled = true;
                openTexturesToolStripMenuItem.Enabled = false;

                string file = Path.Combine(Program.GeomDir, "chr000.geom");
                file = Path.Combine(Program.GeomDir, "chr317.geom");
                file = Path.Combine(Program.GeomDir, "chr365.geom");
                file = Path.Combine(Program.GeomDir, "chr391.geom");
                file = Path.Combine(Program.GeomDir, "t0100acces00.geom");
                file = Path.Combine(Program.GeomDir, "t0101f.geom");
                //file = Path.Combine(Program.GeomDir, "d0101f.geom");
                file = Path.Combine(Program.GeomDir, "chr027.geom");
                //file = Path.Combine(Program.GeomDir, "chr762.geom");
                //file = Path.Combine(Program.GeomDir, "ui_ch_create_02.geom");
                //file = @"E:\[SSD User Data]\Downloads\geoms\d6401f.geom";
                file = Path.Combine(Program.GeomDir, "chr350.geom");

                OpenGeomFile(file);
            }
        }

        private void renderControl_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Escape: Application.Exit(); break;
                case Keys.F1: renderControl.VSync = !renderControl.VSync; break;
                case Keys.F2: wireframe = !wireframe; break;
                case Keys.F3: culling = !culling; break;
                case Keys.F4: lighting = !lighting; break;
            }
        }

        private void renderControl_Render(object sender, EventArgs e)
        {
            this.Text = string.Format(System.Globalization.CultureInfo.InvariantCulture, "{0} - {1} FPS", Application.ProductName, Core.CurrentFramesPerSecond);

            RenderControl renderControl = (sender as RenderControl);

            if (takeScreenshot)
                GL.ClearColor(1.0f, 1.0f, 1.0f, 0.0f);

            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit | ClearBufferMask.StencilBufferBit);

            // OpenTK GLControl focus checking is GARBAGE, might as well leave the check here off -.-
            if (renderControl.Focused)
            {
                OpenTK.Input.KeyboardState kbdState = OpenTK.Input.Keyboard.GetState();

                if (kbdState[OpenTK.Input.Key.Q]) sunAngle += Core.DeltaTime / 15.0f;
                if (kbdState[OpenTK.Input.Key.E]) sunAngle -= Core.DeltaTime / 15.0f;

                lastKbd = kbdState;

                camera.Update(Core.DeltaTime);
            }

            if (wireframe)
                GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Line);
            else
                GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Fill);

            if (culling)
                GL.Enable(EnableCap.CullFace);
            else
                GL.Disable(EnableCap.CullFace);

            if (shader != null)
            {
                Matrix4 tempMatrix = modelviewMatrix * camera.GetViewMatrix();
                shader.SetUniformMatrix(modelviewMatrixName, false, tempMatrix);

                shader.SetUniform(sunPositionName, new Vector3((float)(Math.Cos(sunAngle * Math.PI / 180.0) * 70.0f), (float)(Math.Sin(sunAngle * Math.PI / 180.0) * 70.0f), 0.0f));
                shader.SetUniform(enableLightName, Convert.ToInt32(lighting));
            }

            if (meshes != null)
            {
                GL.Enable(EnableCap.Blend);
                GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);

                shader.SetUniform(discardOpaqueName, 0);
                foreach (Mesh mesh in meshes) mesh.Render();
                shader.SetUniform(discardOpaqueName, 1);
                foreach (Mesh mesh in meshes) mesh.Render();
            }

            if (!takeScreenshot && font != null)
            {
                GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Fill);

                StringBuilder builder = new StringBuilder();
                builder.AppendFormat(System.Globalization.CultureInfo.InvariantCulture, "{0:0} FPS\n", Core.CurrentFramesPerSecond);

                if (meshes != null)
                {
                    builder.AppendLine();
                    builder.AppendFormat(System.Globalization.CultureInfo.InvariantCulture, "Camera: WASD & Mouse+Left Button\n");
                    builder.AppendLine();
                    builder.AppendFormat(System.Globalization.CultureInfo.InvariantCulture, "Vsync (F1): {0}\n", renderControl.VSync);
                    builder.AppendFormat(System.Globalization.CultureInfo.InvariantCulture, "Wireframe (F2): {0}\n", wireframe);
                    builder.AppendFormat(System.Globalization.CultureInfo.InvariantCulture, "Culling (F3): {0}\n", culling);
                    builder.AppendFormat(System.Globalization.CultureInfo.InvariantCulture, "Lighting (F4): {0}\n", lighting);
                    builder.AppendLine();
                    builder.AppendFormat(System.Globalization.CultureInfo.InvariantCulture, "Rotate Light (Q/E)\n");
                }

                font.DrawString(8.0f, 8.0f, builder.ToString());
            }

            if (takeScreenshot)
            {
                renderControl.GrabScreenshot(true).Save(screenshotPath);
                takeScreenshot = false;

                GL.ClearColor(renderControl.BackColor);
            }
        }

        private void renderControl_Resize(object sender, EventArgs e)
        {
            RenderControl renderControl = (sender as RenderControl);
            GL.Viewport(0, 0, renderControl.Width, renderControl.Height);

            if (shader != null)
            {
                float aspectRatio = (renderControl.ClientRectangle.Width / (float)(renderControl.ClientRectangle.Height));
                shader.SetUniformMatrix(projectionMatrixName, false, Matrix4.CreatePerspectiveFieldOfView(MathHelper.PiOver4, aspectRatio, 0.1f, 15000.0f));
            }

            if (font != null)
                font.SetScreenSize(renderControl.ClientRectangle.Width, renderControl.ClientRectangle.Height);
        }

        private void OpenGeomFile(string filename)
        {
            using (FileStream stream = new FileStream(filename, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                if (meshes != null)
                    foreach (Mesh mesh in meshes)
                        mesh.Dispose();

                geom = new GeomFile(stream);

                meshes = geom.GetMeshes();
                foreach (Mesh mesh in meshes) mesh.SetShader(shader);

                lastGeomFile = filename;
                saveColladaToolStripMenuItem.Enabled = true;
                saveScreenshotToolStripMenuItem.Enabled = true;
            }
        }

        private void openTexturesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (fbdTextures.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                Program.ImageDir = fbdTextures.SelectedPath;
                openGeomToolStripMenuItem.Enabled = true;
                openTexturesToolStripMenuItem.Enabled = false;
            }
        }

        private void openGeomToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (ofdGeomFiles.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                OpenGeomFile(ofdGeomFiles.FileName);
        }

        private void saveColladaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (fbdColladaPath.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                ExportCollada.Export(Path.Combine(fbdColladaPath.SelectedPath, Path.GetFileNameWithoutExtension(lastGeomFile), "model.dae"), geom);
            }
        }

        private void saveScreenshotToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (sfdScreenshot.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                takeScreenshot = true;
                screenshotPath = sfdScreenshot.FileName;
            }
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Version version = new Version(Application.ProductVersion);
            string description = (System.Reflection.Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(System.Reflection.AssemblyDescriptionAttribute), false).FirstOrDefault() as System.Reflection.AssemblyDescriptionAttribute).Description;
            string copyright = (System.Reflection.Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(System.Reflection.AssemblyCopyrightAttribute), false).FirstOrDefault() as System.Reflection.AssemblyCopyrightAttribute).Copyright;

            StringBuilder builder = new StringBuilder();
            builder.AppendFormat("{0} v{1}.{2}", Application.ProductName, version.Major, version.Minor);
            if (version.Build != 0) builder.AppendFormat(".{0}", version.Build);
            builder.AppendFormat(" - {0}\n", description);
            builder.AppendLine();
            builder.AppendLine(copyright);

            MessageBox.Show(builder.ToString(), "About", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
}
