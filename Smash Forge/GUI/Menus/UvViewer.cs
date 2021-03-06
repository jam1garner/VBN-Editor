﻿using System;
using System.Windows.Forms;
using OpenTK.Graphics.OpenGL;
using SFGraphics.GLObjects.GLObjectManagement;
using SFGraphics.GLObjects.Shaders;
using OpenTK;
using SmashForge.Rendering;
using SmashForge.Rendering.Meshes;

namespace SmashForge.Gui.Menus
{
    public partial class UvViewer : Form
    {
        private Nud sourceNud;
        private Nud.Polygon polygonToRender;
        private NudRenderMesh forgeMesh;

        public UvViewer(Nud sourceNud, Nud.Polygon polygonToRender)
        {
            // We need the nud to generate buffers due to the way nud rendering works.
            InitializeComponent();
            this.sourceNud = sourceNud;
            this.polygonToRender = polygonToRender;
        }

        private void glControl1_Load(object sender, EventArgs e)
        {
            OpenTkSharedResources.InitializeSharedResources();
            if (OpenTkSharedResources.SetupStatus == OpenTkSharedResources.SharedResourceStatus.Initialized)
            {
                if (sourceNud != null)
                {
                    glControl1.MakeCurrent();
                    forgeMesh = sourceNud.CreateRenderMesh(polygonToRender);
                    // Ignore the material values.
                    forgeMesh.ResetRenderSettings();
                }
            }
        }

        private void glControl1_Paint(object sender, PaintEventArgs e)
        {
            if (OpenTkSharedResources.SetupStatus != OpenTkSharedResources.SharedResourceStatus.Initialized)
                return;

            RenderUvs();

            GLObjectManager.DeleteUnusedGLObjects();
        }

        private void RenderUvs()
        {
            glControl1.MakeCurrent();

            Mesh3D screenTriangle = ScreenDrawing.CreateScreenTriangle();


            GL.Viewport(glControl1.ClientRectangle);
            // Draw darker to make the UVs visible.
            ScreenDrawing.DrawTexturedQuad(RenderTools.uvTestPattern, 0.5f, screenTriangle);

            DrawPolygonUvs();

            glControl1.SwapBuffers();
        }

        private void DrawPolygonUvs()
        {
            Shader shader = OpenTkSharedResources.shaders["UV"];
            shader.UseProgram();
            Matrix4 matrix = Matrix4.CreateOrthographicOffCenter(0, 1, 1, 0, -1, 1);
            shader.SetMatrix4x4("mvpMatrix", ref matrix);

            forgeMesh.Draw(shader);
        }

        private void glControl1_Resize(object sender, EventArgs e)
        {
            glControl1.Invalidate();
        }
    }
}
