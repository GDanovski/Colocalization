/*
 CellTool - software for bio-image analysis
 Copyright (C) 2018  Georgi Danovski

 This program is free software: you can redistribute it and/or modify
 it under the terms of the GNU General Public License as published by
 the Free Software Foundation, either version 3 of the License, or
 (at your option) any later version.

 This program is distributed in the hope that it will be useful,
 but WITHOUT ANY WARRANTY; without even the implied warranty of
 MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 GNU General Public License for more details.

 You should have received a copy of the GNU General Public License
 along with this program.  If not, see <http://www.gnu.org/licenses/>.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

using OpenTK.Graphics.OpenGL;
using OpenTK;
using CellToolDK;
using Colocalization;

namespace Cell_Tool_3
{
    class ImageDrawer
    {
        GLControl GLControl1;
        public TifFileInfo fi;
        public double zoom = 1;
        private double Xposition = 1;
        private double Yposition = 1;
        public int[] thresholds = new int[] { 1,1};
        public int[] colIndexes = new int[] { 0, 1 };
        public int[] frames = new int[] {0,0 };
        public bool showBinary = false;
        public bool showROI = false;

        //public Panel corePanel = new Panel();
        Panel ImageMainPanel;
       // Panel VertPanel = new Panel();
       // Panel HorPanel = new Panel();
       // Panel trPanel = new Panel();

        public ContentPipe ImageTexture = new ContentPipe();

        #region Position on screen
        public Rectangle[][] coRect;
        bool[] colors;
        bool composite;
        //scale
        double oldScale = 1;
        //translation
        public double valX = 0;
        public double valY = 0;
        bool changeXY = true;
        #endregion

        #region New Image Drawing
        public void Initialize(Panel ImageMainPanel, TifFileInfo fi)
        {
            GLControl1 = new GLControl();
            ImageMainPanel.Controls.Add(GLControl1);
            GLControl1.Location = new Point(0, 0);
           
            this.fi = fi;
            this.ImageMainPanel = ImageMainPanel;
            
            GLControl1.Load += GLControl_Load;
            GLControl1.Paint += GLControl_Paint;
            GLControl1.Resize += GLControl_Resize;
            
            ImageMainPanel.Resize += MainPanel_Resizing;

            ImageMainPanel.AutoScroll = true;
        }
        private void MainPanel_Resizing(object sender, EventArgs e)
        {
            try
            {
                GLDrawing_Start(this.GLControl1);
               
            }
            catch { }
        }
        public void ClearImage()
        {
            //try
            {
                //Activate Control
                this.GLControl1.MakeCurrent();
                //Load background
                GL.ClearColor(Color.DimGray);
                GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
                //Prepare MatrixMode
                GL.MatrixMode(MatrixMode.Projection);
                GL.LoadIdentity();
                this.GLControl1.SwapBuffers();
            }
           // catch { }
        }
        public void DrawToScreen()
        {
            GLDrawing_Start(this.GLControl1);
        }
        #region GLControl_Events
        public void GLControl_Load(object sender, EventArgs e)
        {
            GLControl GLControl1 = sender as GLControl;
            GLControl1.MakeCurrent();

            GL.ClearColor(Color.DimGray);
            //ImageTexture
            ImageTexture.ReserveTextureID();
            ImageTexture.GenerateNumberTextures();
        }
        public void GLControl_Resize(object sender, EventArgs e)
        {
           // try
            {
                //Global variables
                TifFileInfo fi = this.fi;
                
                if (fi == null) { return; }
                GLControl GLControl1 = sender as GLControl;

                //Activate Control
                GLControl1.MakeCurrent();

                GL.Viewport(0, 0, GLControl1.Width, GLControl1.Height);
                GL.MatrixMode(MatrixMode.Projection);
                GL.LoadIdentity();
            }
           // catch { }
        }

        public void GLControl_Paint(object sender, EventArgs e)
        {
            //Global variables
            GLControl GLControl1 = sender as GLControl;
            GLDrawing_Start(GLControl1);
        }
        private void GLDrawing_Start(GLControl GLControl1)
        {
           //try
            {
                TifFileInfo fi = this.fi;
                
                if (fi == null)
                {
                    return;
                }
                
                if (GLControl1.Visible == false) { GLControl1.Visible = true; }
                
                Rectangle fieldRect = coRect_Calculate(GLControl1);

                //Calculate B&C
                //CalculateImages(fi);
                
                //Start Drawing

                //Activate Control
                GLControl1.MakeCurrent();
                GL.Disable(EnableCap.Texture2D);
                //Load background
                GL.ClearColor(Color.DimGray);
                GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
                //Prepare MatrixMode
                GL.MatrixMode(MatrixMode.Projection);
                GL.LoadIdentity();
                //Prepare Projection
                GL.Ortho(0.0, (double)GLControl1.Width, (double)GLControl1.Height, 0.0, -1.0, 1.0);

                GL.MatrixMode(MatrixMode.Modelview);
                //GL.LoadIdentity();
                GL.Translate(-valX, -valY, 0);
                valX = 0;
                valY = 0;

                //Set viewpoint
                GL.Viewport(0, 0, GLControl1.Width, GLControl1.Height);
                //scale the image
                if (oldScale != zoom)
                {
                    double factor = zoom / oldScale;
                    oldScale = zoom;
                    if (factor != 1)
                    {
                        GL.Scale(factor, factor, 1);
                    }
                }
                //Translation
                changeXY = false;
                this.GLControl1.Height = (int)(fieldRect.Height * zoom);
                this.GLControl1.Width = (int)(fieldRect.Width * zoom);
               
                changeXY = true;

                //make colors transparent
                GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.One);
               // SelectedImage_DrawBorder(fi);
                DrawBackgrounds_Global(fi);
                GL.Enable(EnableCap.Blend);
                GL.ShadeModel(ShadingModel.Flat);
                
                DrawRawImages(fi);

                if(showBinary)
                    DrawFilteredImages(fi);

                GL.Disable(EnableCap.Blend);

                //draw chart
                drawChart(fi, coRect[2][0]);

                
                //draw rois
                if (showROI)
                    drawRoi(fi);

                GLControl1.SwapBuffers();                
            }
           // catch { }
        }
        private void drawChart(TifFileInfo fi, Rectangle rect)
        {
            
            rect = new Rectangle(rect.X + 1, rect.Y + 1, rect.Width - 2, rect.Height - 2);
            Point[] points = null;
            
            Cell_Tool_3.FrameCalculator frameCalc = new FrameCalculator();
            
            int[] curFrames = new int[2];
            fi.frame = frames[0];
            curFrames[0] = frameCalc.FrameC(fi, colIndexes[0]);
            fi.frame = frames[1];
            curFrames[1] = frameCalc.FrameC(fi, colIndexes[1]);
            
            switch (fi.bitsPerPixel)
            {
                case 8:
                    points = Operations.PairImages(fi.image8bit[curFrames[0]], fi.image8bit[curFrames[1]]);
                    break;
                case 16:
                    points = Operations.PairImages(fi.image16bit[curFrames[0]], fi.image16bit[curFrames[1]]);
                    break;
            }
            //Filter By thresholds
            //if (showBinary)
               // points = Operations.FilterPointsByThresholds(points, thresholds[0], thresholds[1]);
            
            //MessageBox.Show((points1.Length - points.Length).ToString());
            //Operations
            int max = Operations.MaxValueInPoints(points);
            int min = Operations.MinValueInPoints(points);
            float[] LUT = Operations.GetLUT(max,min);

            float scale = (float)rect.Width/max;

            PointF[] scaledPoints = Operations.ScalePoint(points, scale);
            
            scaledPoints = Operations.TranslatePoint(scaledPoints, rect.Location);
            scaledPoints = Operations.AddYToPoints(scaledPoints, rect.Size.Height + rect.Location.Y + rect.Location.Y);

            float size = (float)2/(float)zoom;
            GL.Enable(EnableCap.LineSmooth);
            PointF p;
                      
            //GL.ShadeModel(ShadingModel.Flat);
            //foreach (PointF p in scaledPoints)
            for (int i =0;i<scaledPoints.Length;i++)
            {
                p = scaledPoints[i];
                //start drawing
                GL.Begin(PrimitiveType.TriangleStrip);

                GL.Color4(LUT[points[i].X], LUT[points[i].Y], 0f, 1f);

                GL.Vertex2(p.X + size, p.Y);
                GL.Vertex2(p.X, p.Y + size);
                GL.Vertex2(p.X, p.Y);
                GL.Vertex2(p.X + size, p.Y + size);
                GL.End();
            }


            if (showBinary)
            {
               
                if (thresholds[0] < max)
                {
                    float thresh1 = (float)rect.X + (float)thresholds[0] * scale;
                    GL.Begin(PrimitiveType.Lines);
                    GL.Color4(0f, 0f, 1f, 1f);

                    GL.Vertex2(thresh1, rect.Y);
                    GL.Vertex2(thresh1, rect.Y + rect.Height);
                    GL.End();
                }
                if (thresholds[1] < max)
                {
                    float thresh2 = (float)rect.Y + (float)rect.Height - (float)thresholds[1] * scale;
                    GL.Begin(PrimitiveType.Lines);
                    GL.Color4(0f, 0f, 1f, 1f);

                    GL.Vertex2(rect.X, thresh2);
                    GL.Vertex2(rect.X + rect.Width, thresh2);
                    GL.End();
                }
            }

            GL.Disable(EnableCap.LineSmooth);
        }
        private void DrawLine()
        {
            GL.EnableClientState(ArrayCap.VertexArray);
            float[] para_vertex =
{
    50,270,
    100,30,
    54,270,
    104,30,
    58,270,
    108,30
};
            float[] para_color = new float[]
            {
    1f,0f,0f,    //red
};
            GL.VertexPointer(2, VertexPointerType.Float, 0, para_vertex);
            GL.ColorPointer(3, ColorPointerType.Float, 0, para_color);
            GL.DrawArrays(PrimitiveType.TriangleStrip, 0, 6);
            
            GL.DisableClientState(ArrayCap.VertexArray);
        }
        #endregion
        
        private void DrawBackgrounds_Global(TifFileInfo fi)
        {
            //singlechanels or composite
            for (int C = 0; C < 2; C++)
            {
                if (colors[C] == true)
                {
                    //RawImage
                    RawImage_DrawBackColor(coRect[0][C]);
                    //FilterImage
                    if (showBinary)
                        RawImage_DrawBackColor(coRect[1][C]);
                    
                }
            }
            //Chart
            Chart_DrawBackColor(coRect[2][0]);

            if (composite == true)
            {
                RawImage_DrawBackColor(coRect[0][2]);
                if (showBinary)
                    RawImage_DrawBackColor(coRect[1][2]);
            }
        }
        
        private void RawImage_DrawBackColor(Rectangle rect)
        {
            int W = rect.X + rect.Width;
            int H = rect.Y + rect.Height;
            //start drawing
            GL.Begin(PrimitiveType.Quads);

            GL.Color4(0f, 0f, 0f, 1f);

            GL.Vertex2(rect.X, rect.Y);
            GL.Vertex2(rect.X, H);
            GL.Vertex2(W, H);
            GL.Vertex2(W, rect.Y);

            GL.End();

        }
        private void Chart_DrawBackColor(Rectangle rect)
        {
            int W = rect.X + rect.Width;
            int H = rect.Y + rect.Height;
            //start drawing
            GL.Begin(PrimitiveType.Quads);

            GL.Color3(1f, 1f, 1f);

            GL.Vertex2(rect.X, rect.Y);
            GL.Vertex2(rect.X, H);
            GL.Vertex2(W, H);
            GL.Vertex2(W, rect.Y);

            GL.End();

            Chart_drawBorder(rect.X, rect.Y, rect.Width, rect.Height);
        }
        private void Chart_drawBorder(float x, float y, float w, float h)
        {
            w += x;
            h += y;

            GL.Begin(PrimitiveType.LineLoop);
            GL.Color3( 0f, 0f, 0f);

            GL.Vertex2(x, y);
            GL.Vertex2(w, y);
            GL.Vertex2(w, h);
            GL.Vertex2(x, h);

            GL.End();
        }
        private void Draw16BitImage(TifFileInfo fi, int C, int rectC, int[] arrayW, int[] arrayH)
        { 
            try
            {
                FrameCalculator FC = new FrameCalculator();
                //image array
                ushort[][] image = fi.image16bit[FC.FrameC(fi, C)];
                float[] LUT = fi.adjustedLUT[C];
                //Prepare RGB
                float R = (float)(fi.LutList[C].R / 255f);
                float G = (float)(fi.LutList[C].G / 255f);
                float B = (float)(fi.LutList[C].B / 255f);
                //start drawing


                float oldI = 0f;
                float oldJ = 0f;

                //Coordinates
                Rectangle rect = coRect[0][rectC];

                float X = (float)rect.X;
                float Y = (float)rect.Y;
                float W = X + 1f;
                float H = X + 1f;
                int index = 0;

                for (float i = 1f; i <= fi.sizeY; i++)
                {
                    if (arrayH == null)
                    {
                        Y = (float)rect.Y + oldI;
                        H = (float)rect.Y + i;
                    }
                    else
                    {
                        Y = (float)arrayH[(int)oldI];
                        H = (float)arrayH[(int)i];
                    }
                    GL.Begin(PrimitiveType.TriangleStrip);

                    X = (float)arrayW[(int)oldJ];
                    GL.Vertex2(X, Y);
                    GL.Vertex2(X, H);

                    for (float j = 1f; j <= fi.sizeX; j++)
                    {
                        W = (float)arrayW[(int)j];

                        index = image[(int)oldI][(int)oldJ];
                        if (LUT.Length > index)
                            GL.Color4(R, G, B, LUT[index]);
                        else
                            GL.Color4(R, G, B, LUT[LUT.Length - 1]);

                        GL.Vertex2(W, Y);
                        GL.Vertex2(W, H);

                        oldJ = j;
                    }
                    oldJ = 0f;
                    oldI = i;

                    GL.End();
                }
                //end drawing

            }
           catch { }
        }
        private void Draw8BitImage(TifFileInfo fi, int C, int rectC, int[] arrayW, int[] arrayH)
        {
           try
            {
                FrameCalculator FC = new FrameCalculator();
                //image array
                byte[][] image = fi.image8bit[FC.FrameC(fi, C)];
                float[] LUT = fi.adjustedLUT[C];
                //Prepare RGB
                float R = (float)(fi.LutList[C].R / 255f);
                float G = (float)(fi.LutList[C].G / 255f);
                float B = (float)(fi.LutList[C].B / 255f);
                //start drawing

                float oldI = 0f;
                float oldJ = 0f;

                //Coordinates
                Rectangle rect = coRect[0][rectC];

                float X = (float)rect.X;
                float Y = (float)rect.Y;
                float W = X + 1f;
                float H = X + 1f;
                int index = 0;

                for (float i = 1f; i <= fi.sizeY; i++)
                {
                    if (arrayH == null)
                    {
                        Y = (float)rect.Y + oldI;
                        H = (float)rect.Y + i;
                    }
                    else
                    {
                        Y = (float)arrayH[(int)oldI];
                        H = (float)arrayH[(int)i];
                    }

                    GL.Begin(PrimitiveType.TriangleStrip);

                    X = (float)arrayW[(int)oldJ];
                    GL.Vertex2(X, Y);
                    GL.Vertex2(X, H);

                    for (float j = 1f; j <= fi.sizeX; j++)
                    {
                        W = (float)arrayW[(int)j];

                        index = image[(int)oldI][(int)oldJ];
                        if (LUT.Length > index)
                            GL.Color4(R, G, B, LUT[index]);
                        else
                            GL.Color4(R, G, B, LUT[LUT.Length - 1]);

                        GL.Vertex2(W, Y);
                        GL.Vertex2(W, H);

                        oldJ = j;
                    }
                    oldJ = 0f;
                    oldI = i;

                    GL.End();
                }
                //end drawing

            }
           catch { }
        }
        private void Draw16BitFilteredImage(TifFileInfo fi, int C, int rectC, int[] arrayW, int[] arrayH)
        {
            try
            {
                if (fi.image16bitFilter == null) { fi.image16bitFilter = fi.image16bit; }
                FrameCalculator FC = new FrameCalculator();
                //image array
                ushort[][] image = fi.image16bitFilter[FC.FrameC(fi, C)];
                float[] LUT = fi.adjustedLUT[C];
                //calculate spot detector diapasone
                //int[] SpotDiapason = IA.Segmentation.SpotDet.CalculateBorders(fi, C);
                //Prepare RGB
                float R = (float)(fi.LutList[C].R / 255f);
                float G = (float)(fi.LutList[C].G / 255f);
                float B = (float)(fi.LutList[C].B / 255f);
                //start drawing

                float oldI = 0f;
                float oldJ = 0f;

                //Coordinates
                Rectangle rect = coRect[1][rectC];

                float X = (float)rect.X;
                float Y = (float)rect.Y;
                float W = X + 1f;
                float H = X + 1f;
                int val = 0;
                
                int Choise = thresholds[C];
               
                for (float i = 1f; i <= fi.sizeY; i++)
                {
                    if (arrayH == null)
                    {
                        Y = (float)rect.Y + oldI;
                        H = (float)rect.Y + i;
                    }
                    else
                    {
                        Y = (float)arrayH[(int)oldI];
                        H = (float)arrayH[(int)i];
                    }
                    GL.Begin(PrimitiveType.TriangleStrip);

                    X = (float)arrayW[(int)oldJ];
                    GL.Vertex2(X, Y);
                    GL.Vertex2(X, H);

                    for (float j = 1f; j <= fi.sizeX; j++)
                    {
                        W = (float)arrayW[(int)j];
                        val = (int)image[(int)oldI][(int)oldJ];
                        #region Colors

                        Color col;
                        if (val > Choise)
                            col = fi.LutList[C];
                        else
                            col = Color.Black;
                        GL.Color4(col);
                        #endregion Colors

                        GL.Vertex2(W, Y);
                        GL.Vertex2(W, H);

                        oldJ = j;
                    }
                    oldJ = 0f;
                    oldI = i;

                    GL.End();
                }
                //end drawing

            }
            catch { }
        }
        private void Draw8BitFilteredImage(TifFileInfo fi, int C, int rectC, int[] arrayW, int[] arrayH)
        {
           try
            {
                if (fi.image8bitFilter == null) { fi.image8bitFilter = fi.image8bit; }

                FrameCalculator FC = new FrameCalculator();
                //image array
                byte[][] image = fi.image8bitFilter[FC.FrameC(fi, C)];
                float[] LUT = fi.adjustedLUT[C];
                //calculate spot detector diapasone
                

                //Prepare RGB
                float R = (float)(fi.LutList[C].R / 255f);
                float G = (float)(fi.LutList[C].G / 255f);
                float B = (float)(fi.LutList[C].B / 255f);
                //start drawing

                float oldI = 0f;
                float oldJ = 0f;

                //Coordinates
                Rectangle rect = coRect[1][rectC];

                float X = (float)rect.X;
                float Y = (float)rect.Y;
                float W = X + 1f;
                float H = X + 1f;
                int val = 0;
                
                int Choise = thresholds[C];
                

                for (float i = 1f; i <= fi.sizeY; i++)
                {
                    if (arrayH == null)
                    {
                        Y = (float)rect.Y + oldI;
                        H = (float)rect.Y + i;
                    }
                    else
                    {
                        Y = (float)arrayH[(int)oldI];
                        H = (float)arrayH[(int)i];
                    }

                    GL.Begin(PrimitiveType.TriangleStrip);

                    X = (float)arrayW[(int)oldJ];
                    GL.Vertex2(X, Y);
                    GL.Vertex2(X, H);

                    for (float j = 1f; j <= fi.sizeX; j++)
                    {
                        W = (float)arrayW[(int)j];
                        val = (int)image[(int)oldI][(int)oldJ];

                        #region Colors
                        Color col;
                        if (val > Choise)
                            col = Color.Yellow;
                        else
                            col = Color.Black;
                        GL.Color4(col);
                        #endregion Colors

                        GL.Vertex2(W, Y);
                        GL.Vertex2(W, H);

                        oldJ = j;
                    }
                    oldJ = 0f;
                    oldI = i;

                    GL.End();
                }
                //end drawing

            }
           catch { }
        }
        private void DrawFilteredImages(TifFileInfo fi)
        {            
            //singlechanels or composite
            int[] arrayW = new int[fi.sizeX + 1];
            int col = coRect[1].Length - 1;
            for (int i = 0; i < 2; i++)
            {
                if (colors[i] == true)
                {
                    col = i;
                    break;
                }
            }

            int X = coRect[1][col].X;
            for (int i = 0; i < arrayW.Length; i++, X++)
            {
                arrayW[i] = X;
            }

            for (int C = 0; C < 2; C++)
            {
                if (colors[C] == true)
                {
                    fi.frame = frames[C];
                    switch (fi.bitsPerPixel)
                    {
                        case 8:
                            Draw8BitFilteredImage(fi,colIndexes[C], C, arrayW, null);
                            break;
                        case 16:
                            Draw16BitFilteredImage(fi, colIndexes[C], C, arrayW, null);
                            break;
                    }
                }
            }

            if (composite == true)
            {
                int[] arrayH = new int[fi.sizeY + 1];
                int Y = coRect[1][2].Y;
                for (int i = 0; i < arrayH.Length; i++, Y++)
                {
                    arrayH[i] = Y;
                }

                for (int C1 = 0; C1 < 2; C1++)
                {
                    fi.frame = frames[C1];
                    switch (fi.bitsPerPixel)
                    {
                        case 8:
                            Draw8BitFilteredImage(fi, colIndexes[C1], 2, arrayW, null);
                            break;
                        case 16:
                            Draw16BitFilteredImage(fi, colIndexes[C1], 2, arrayW, null);
                            break;
                    }
                }
                arrayH = null;
            }
            arrayW = null;
        }
        private void DrawRawImages(TifFileInfo fi)
        {
            //singlechanels or composite
            int[] arrayW = new int[fi.sizeX + 1];
            int col = coRect[0].Length - 1;
            for (int i = 0; i < col; i++)
            {
                if (colors[i] == true)
                {
                    col = i;
                    break;
                }
            }

            int X = coRect[0][col].X;
            for (int i = 0; i < arrayW.Length; i++, X++)
            {
                arrayW[i] = X;
            }

            for (int C = 0; C < 2; C++)
            {
                if (colors[C] == true)
                {
                    fi.frame = frames[C];
                    switch (fi.bitsPerPixel)
                    {
                        case 8:
                            Draw8BitImage(fi, colIndexes[C], C, arrayW, null);
                            break;
                        case 16:
                            Draw16BitImage(fi, colIndexes[C], C, arrayW, null);
                            break;
                    }
                }
            }
            if (composite == true)
            {
                int[] arrayH = new int[fi.sizeY + 1];
                int Y = coRect[0][2].Y;
                for (int i = 0; i < arrayH.Length; i++, Y++)
                {
                    arrayH[i] = Y;
                }

                for (int C1 = 0; C1 < 2; C1++)
                {
                    fi.frame = frames[C1];
                    switch (fi.bitsPerPixel)
                    {
                        case 8:
                            Draw8BitImage(fi, colIndexes[C1], 2, arrayW, arrayH);
                            break;
                        case 16:
                            Draw16BitImage(fi, colIndexes[C1], 2, arrayW, arrayH);
                            break;
                    }
                }
                arrayH = null;
            }
            arrayW = null;
        }

        public Rectangle coRect_Calculate(GLControl GLControl1)
        {
            TifFileInfo fi = this.fi;
            //Prepare coRect
            coRect = new Rectangle[3][];

            //create color array
            colors = new bool[2];

            for (int i = 0; i < 2; i++)
            {
                colors[i] = true;
                /*
                if (ColorBtnList[i].ImageIndex == 0)
                {
                    colors[i] = true;
                }
                else
                {
                    colors[i] = false;
                }*/
            }
            //composite image

            if (fi.sizeC > 1)
            {
                composite = true;
                /*
                if (ColorBtnList[fi.sizeC].ImageIndex == 0)
                {
                    composite = true;
                }
                else
                {
                    composite = false;
                }*/
            }
            else
            {
                composite = false;
            }
            //Take Size

            int sizeH = 0;
            int sizeW = 0;

            #region RawImages - index 0
            int z = 2;
            if (z > 1) { z++; }
            coRect[0] = new Rectangle[z];

            int biggestW = 0;
            int biggestH = 0;
            int interval = (int)(10 / zoom);
            sizeH = interval;
            sizeW = interval;
            //foreach (bool val in colors)
            for (int i = 0; i < colors.Length; i++)
            {
                if (colors[i] == true)
                {
                    //position
                    Rectangle newRect =
                        new Rectangle((int)sizeW, (int)sizeH,
                        (int)(fi.sizeX), (int)(fi.sizeY));
                    coRect[0][i] = newRect;
                    //MessageBox.Show("X - " + sizeW.ToString() + "\tY - " + sizeH.ToString() + "\tW - " + fi.sizeX.ToString() + "\tH - " + fi.sizeY);
                    //size
                    sizeH += (fi.sizeY + interval);
                    if (biggestW < fi.sizeX) { biggestW = fi.sizeX; }
                }
            }
            if (composite == true)
            {
                //position
                Rectangle newRect =
                    new Rectangle((int)sizeW, (int)sizeH,
                    (int)(fi.sizeX), (int)(fi.sizeY));
                coRect[0][fi.sizeC] = newRect;
                //size
                sizeH += (int)(fi.sizeY + interval);
                if (biggestW < fi.sizeX) { biggestW = fi.sizeX; }

            }

            if (biggestW != interval)
            {
                //size
                sizeW += (int)(biggestW + interval);
            }
            if (biggestH < sizeH) { biggestH = sizeH; }
            #endregion RawImages - index 0

            #region Filtered image - index 1
            if (showBinary)
            {
                z = 2;
                if (z > 1) { z++; }
                coRect[1] = new Rectangle[z];

                biggestW = 0;
                sizeH = interval;

                for (int i = 0; i < colors.Length; i++)
                {
                    if (colors[i] == true)
                    {
                        //position
                        Rectangle newRect =
                            new Rectangle((int)sizeW, (int)sizeH,
                            (int)(fi.sizeX), (int)(fi.sizeY));
                        coRect[1][i] = newRect;
                        //size
                        sizeH += (fi.sizeY + interval);
                        if (biggestW < fi.sizeX) { biggestW = fi.sizeX; }
                    }
                }

                if (composite == true)
                {
                    //position
                    Rectangle newRect =
                        new Rectangle((int)sizeW, (int)sizeH,
                        (int)(fi.sizeX), (int)(fi.sizeY));
                    coRect[1][fi.sizeC] = newRect;
                    //size
                    sizeH += (int)(fi.sizeY + interval);
                    if (biggestW < fi.sizeX) { biggestW = fi.sizeX; }

                }

                if (biggestW != interval)
                {
                    //size
                    sizeW += (int)(biggestW + interval);
                }
                if (biggestH < sizeH) { biggestH = sizeH; }
            }
            #endregion Filtered image - index 1

            #region Chart - index 1
            z = 1;
            coRect[2] = new Rectangle[z];

            biggestW = 0;
            sizeH = interval;

            for (int i = 0; i < 1; i++)
            {
                if (colors[i] == true)
                {
                    //position
                    Rectangle newRect =
                        new Rectangle((int)sizeW, (int)sizeH,
                        (int)(fi.sizeY*2), (int)(fi.sizeY*2));
                    coRect[2][i] = newRect;
                    //size
                    sizeH += (int) (fi.sizeY + interval);
                    // if (biggestW < fi.sizeX) { biggestW = fi.sizeX; }
                    if (biggestW < newRect.Width) { biggestW = newRect.Width; }
                }
            }
            if (biggestW != interval)
            {
                //size
                sizeW += (int)(biggestW + interval);
            }
            if (biggestH < sizeH) { biggestH = sizeH; }
            #endregion Chart - index 1
            //Calculate Field
            Rectangle result = new Rectangle(0, 0, (int)sizeW, (int)biggestH);
            
            return result;

        }
        
        #endregion
        #region MouseMoveField event

        private bool fieldMove = false;
        private int oldX = 0;
        private int oldY = 0;
      
        #endregion MouseMoveField event

        #region Draw ROI
       const float DEG2RAD = (float)(3.14159 / 180.0);
       private void drawEllipse(float x, float y, float xradius, float yradius)
        {
            xradius /= 2;
            yradius /= 2;

            x += xradius;
            y += yradius;

            GL.Begin(PrimitiveType.LineLoop);
            GL.Color4(1f, 1f, 0f, 1f);

            for (int i = 0; i < 360; i++)
            {
                //convert degrees into radians
                float degInRad = i * DEG2RAD;
                double newX = x + Math.Cos(degInRad) * xradius;
                double newY = y + Math.Sin(degInRad) * yradius;
                GL.Vertex2(newX,newY);
            }

            GL.End();
        }
        private void drawEllipse(float x, float y, float xradius, float yradius, Rectangle Rect)
        {
            //Check is it outside
            if(!(x < Rect.X+0.5 | x + xradius > Rect.Width + 0.5 |
                y < Rect.Y + 0.5 | y + yradius > Rect.Height + 0.5))
            {
                drawEllipse(x, y, xradius, yradius);
                return;
            }

            xradius /= 2;
            yradius /= 2;

            x += xradius;
            y += yradius;

            List<float> Xarr = new List<float>();
            List<float> Yarr = new List<float>();

            for (int i = 0; i <= 360; i++)
            {
                //convert degrees into radians
                float degInRad = i * DEG2RAD;
                double newX = x + Math.Cos(degInRad) * xradius;
                double newY = y + Math.Sin(degInRad) * yradius;
                Xarr.Add((float)newX);
                Yarr.Add((float)newY);
            }
            PolygonalFieldCut(Xarr.ToArray(), Yarr.ToArray(), Rect);
        }
        private void fillRectangle(float x, float y, float w, float h, Rectangle Rect)
        {

            RectangleF RectF = new RectangleF(
               (float)Rect.X,
               (float)Rect.Y,
               (float)(Rect.Width + Rect.X),
               (float)(Rect.Height + Rect.Y));

            x += RectF.X + 0.5f;
            y += RectF.Y + 0.5f;

            if (w + x <= RectF.X + 0.5f |
                h + y <= RectF.Y + 0.5f |
                x > RectF.Width |
                y > RectF.Height)
            {
                return;
            }

            w += x;
            h += y;

            if (x <= RectF.X+0.5f) x = RectF.X  + 0.5f;

            if (y <= RectF.Y +0.5f) y = RectF.Y + 0.5f;

            if (w > RectF.Width )
                w = RectF.Width;

            if (h > RectF.Height)
                h = RectF.Height;

            //fill rectangle
            GL.Begin(PrimitiveType.TriangleStrip);
            GL.Color4(1f, 1f, 1f, 1f);

            GL.Vertex2(x, y);
            GL.Vertex2(x, h);
            GL.Vertex2(w, y);
            GL.Vertex2(w, h);

            GL.End();
            //draw borders
            GL.Begin(PrimitiveType.LineLoop);
            GL.Color4(0f, 0f, 0f, 1f);

            GL.Vertex2(x, y);
            GL.Vertex2(w, y);
            GL.Vertex2(w, h);
            GL.Vertex2(x, h);

            GL.End();
        }
        private void drawRectangle(float x, float y, float w, float h,Rectangle Rect)
        {
            RectangleF RectF = new RectangleF(
               (float)Rect.X,
               (float)Rect.Y,
               (float)(Rect.Width + Rect.X),
               (float)(Rect.Height + Rect.Y));

            if (!(x < RectF.X + 0.5f |
                y < RectF.Y + 0.5f |
                w + x > RectF.Width |
                h + y > RectF.Height ))
            {
                drawRectangle(x, y, w, h);
                return;
            }
            else if (w + x < RectF.X + 0.5f |
                h + y < RectF.Y + 0.5f |
                x > RectF.Width |
                y > RectF.Height)
            {
                return;
            }
            
            w += x;
            h += y;

            float[] Xarr = new float[] { x, w, w, x };
            float[] Yarr = new float[] { y, y, h, h };
            PolygonalFieldCut(Xarr, Yarr, Rect);
            
        }
        private void drawRectangle(float x, float y, float w, float h)
        {
            w += x;
            h += y;

            GL.Begin(PrimitiveType.LineLoop);
            GL.Color4(1f, 1f, 0f, 1f);

            GL.Vertex2(x, y);
            GL.Vertex2(w, y);
            GL.Vertex2(w, h);
            GL.Vertex2(x, h);

            GL.End();
        }
        private void drawPolygon(float[] x, float[] y)
        {
            GL.Begin(PrimitiveType.LineLoop);
            GL.Color4(1f, 1f, 0f, 1f);
            
            for(int i = 0; i< x.Length; i++)
               GL.Vertex2(x[i], y[i]);

            GL.End();
        }
        private void drawUnfinishedPolygon(float[] x, float[] y)
        {
            GL.Begin(PrimitiveType.LineStrip);
            GL.Color4(1f, 1f, 0f, 1f);
            for (int i = 0; i < x.Length; i++)
                GL.Vertex2(x[i], y[i]);
            
            GL.End();
        }
        
        private List<PointF> DrawLine(PointF p1, PointF p2)
        {
            //Bresenham's line algorithm
            List<PointF> pxlList = new List<PointF>();
            float deltaX = p2.X - p1.X;
            float deltaY = p2.Y - p1.Y;
            double error = -1;

            if (deltaX == 0)
            {
                // || on X axis
                if (p1.Y < p2.Y)
                {
                    for (float Y = p1.Y; Y <= p2.Y; Y++)
                        pxlList.Add(new PointF(p1.X, Y));
                }
                else
                {
                    for (float Y = p2.Y; Y <= p1.Y; Y++)
                        pxlList.Add(new PointF(p1.X, Y));
                }
                
            }
            else if (deltaY == 0)
            {
                // || on Y axis
                if (p1.X < p2.X)
                {
                    for (float X = p1.X; X <= p2.X; X++)
                        pxlList.Add(new PointF(X, p1.Y));
                }
                else
                {
                    for (float X = p2.X; X <= p1.X; X++)
                        pxlList.Add(new PointF(X, p1.Y));
                }

            }
            else
            {
                double deltaErr = deltaY / deltaX;
                //find wich case is our line
                int case1 = 0;
                if (deltaErr > 0 & deltaErr <= 1)
                {
                    if (deltaX > 0)
                        case1 = 0;
                    else
                        case1 = 4;
                }
                else if (deltaErr > 1)
                {
                    if (deltaX > 0)
                        case1 = 1;
                    else
                        case1 = 5;
                }
                else if (deltaErr < 0 & deltaErr >= -1)
                {
                    if (deltaX > 0)
                        case1 = 7;
                    else
                        case1 = 3;
                }
                else if (deltaErr < -1)
                {
                    if (deltaX > 0)
                        case1 = 6;
                    else
                        case1 = 2;
                }


                //select case x,y
                float x0;
                float y0;
                float x1;
                float y1;

                switch (case1)
                {
                    //case 0:
                    default:
                        x0 = p1.X;
                        y0 = p1.Y;
                        x1 = p2.X;
                        y1 = p2.Y;
                        break;
                    case 1:
                        x0 = p1.Y;
                        y0 = p1.X;
                        x1 = p2.Y;
                        y1 = p2.X;
                        break;
                    case 2:
                        x0 = p1.Y;
                        y0 = -p1.X;
                        x1 = p2.Y;
                        y1 = -p2.X;
                        break;
                    case 3:
                        x0 = -p1.X;
                        y0 = p1.Y;
                        x1 = -p2.X;
                        y1 = p2.Y;
                        break;
                    case 4:
                        x0 = -p1.X;
                        y0 = -p1.Y;
                        x1 = -p2.X;
                        y1 = -p2.Y;
                        break;
                    case 5:
                        x0 = -p1.Y;
                        y0 = -p1.X;
                        x1 = -p2.Y;
                        y1 = -p2.X;
                        break;
                    case 6:
                        x0 = -p1.Y;
                        y0 = p1.X;
                        x1 = -p2.Y;
                        y1 = p2.X;
                        break;
                    case 7:
                        x0 = p1.X;
                        y0 = -p1.Y;
                        x1 = p2.X;
                        y1 = -p2.Y;
                        break;
                }
                //calculate new values
                deltaX = x1 - x0;
                deltaY  = y1 - y0;
                deltaErr = Math.Abs(deltaY / deltaX);

                //Assume deltax != 0 (line is not vertical),
                //note that this division needs to be done in a way that preserves the fractional part
                float y = y0;
                double error1 = -1;
                for (float x = x0; x< x1; x++)
                {
                    switch (case1)
                    {
                        //case 0:
                        default:
                            pxlList.Add(new PointF(x, y));
                            break;
                        case 1:
                            pxlList.Add(new PointF(y,x));
                            break;
                        case 2:
                            pxlList.Add(new PointF(-y, x));
                            break;
                        case 3:
                            pxlList.Add(new PointF(-x, y));
                            break;
                        case 4:
                            pxlList.Add(new PointF(-x, -y));
                            break;
                        case 5:
                            pxlList.Add(new PointF(-y, -x));
                            break;
                        case 6:
                            pxlList.Add(new PointF(y, -x));
                            break;
                        case 7:
                            pxlList.Add(new PointF(x, -y));
                            break;
                    }

                    error1 += deltaErr;
                    if(error1 >= 0.0)
                    {
                        y++;
                        error1 -= 1.0;
                    }
                }
            }

            return pxlList;
        }
        private bool RectFContains(PointF p, RectangleF rectF)
        {
            if (p.X < rectF.X | p.X > rectF.Width | 
                p.Y < rectF.Y | p.Y > rectF.Height)
                return false;
            else
                return true;
        }
        private void drawUnfinishedPolygon(List<PointF> points)
        {
            GL.Begin(PrimitiveType.LineStrip);
            GL.Color4(1f, 1f, 0f, 1f);
            for (int i = 0; i < points.Count; i++)
                GL.Vertex2(points[i].X, points[i].Y);

            GL.End();
        }
        private void PolygonalFieldCut(float[] X, float[] Y, Rectangle Rect)
        {
            #region Variables
            //Create actual points rectangleF
            RectangleF RectF = new RectangleF(
                (float)Rect.X,
                (float)Rect.Y,
                (float)(Rect.Width + Rect.X),
                (float)(Rect.Height + Rect.Y));
           
            List<PointF> resP = new List<PointF>();
            List<PointF> potP;

            PointF p0 = new PointF(X[X.Length-1],Y[Y.Length-1]);
            PointF p1;
            bool drawn = false;
            
            bool visible;
            bool contain;//bool that shows is the point in rectF
            PointF prevP;//the one before the last selected
            PointF lastVisibleP;
            #endregion Variables

            for (int i = 0; i <= X.Length; i++)
            {
                lastVisibleP = PointF.Empty;
                visible = false;
                //set cur point
                if (i < X.Length)
                    p1 = new PointF(X[i], Y[i]);
                else
                    p1 = new PointF(X[0], Y[0]);
                //check is border visible
                if(RectFContains(p0, RectF))
                {
                    resP.Add(p0);
                    visible = true;
                } 
                else
                {
                    drawn = true;
                }
                //calculate potPoint
                potP = DrawLine(p0, p1);//Calculates all potential points
                prevP = p0;//the one before the last selected
                foreach (PointF p in potP)
                {
                    //check is point visible
                    contain = RectFContains(p, RectF);

                    if (contain == true)
                        lastVisibleP = p;

                    if (contain != visible)
                    {
                        visible = contain;
                        if (contain == true)
                        {
                            resP.Add(p);
                        }
                        else
                        {
                            resP.Add(prevP);
                            drawUnfinishedPolygon(resP);
                            resP.Clear();
                            drawn = true;
                        }
                    }
                    //set prev point
                    prevP = p;
                }
                //finish the list
                if (RectFContains(p1, RectF))
                    resP.Add(p1);
                else if (lastVisibleP.IsEmpty == false)
                    resP.Add(lastVisibleP);

                drawUnfinishedPolygon(resP);
                //set old point
                p0 = new PointF(p1.X, p1.Y);
                resP.Clear();
            }

            if (drawn == false)
            {
                drawPolygon(X, Y);
            }
            
        }
       
       
        private void drawRoi(TifFileInfo fi)
        {
            for (int col = 0; col < 2; col++)
            {
                List<ROI> roiList = fi.roiList[0];
                if (roiList == null) continue;

                foreach (ROI roi in roiList)
                {
                    if (roi == null) continue;

                    Rectangle rect = coRect[0][col];
                    int addX = rect.X;
                    int addY = rect.Y;

                    fi.frame = frames[0];
                    FrameCalculator FC = new FrameCalculator();
                    int frame = FC.FrameC(fi,0);

                    if (fi.frame + 1 < roi.FromT | fi.frame + 1 > roi.ToT) continue;
                    if (fi.zValue + 1 < roi.FromZ | fi.zValue + 1 > roi.ToZ) continue;
                    if (roi.Checked == false) continue;
                    Point[] points = roi.GetLocation(frame);
                    if (points == null) continue;

                    if (roi.Shape == 0)
                    {
                        Point p = points[0];
                        
                        drawRectangle(p.X + addX + 0.5f, p.Y + addY + 0.5f, roi.Width, roi.Height, rect);
                    }
                    else if (roi.Shape == 1)
                    {
                        Point p = points[0];
                        drawEllipse(p.X + addX + 0.5f, p.Y + addY + 0.5f, roi.Width, roi.Height, rect);
                    }
                    else if (roi.Shape == 2)
                    {
                        float[] x = new float[points.Length];
                        float[] y = new float[points.Length];
                        for (int i = 0; i < points.Length; i++)
                        {
                            Point p = points[i];
                            x[i] = p.X + addX + 0.5f;
                            y[i] = p.Y + addY + 0.5f;
                        }
                        //drawPolygon(x, y);
                        PolygonalFieldCut(x, y, rect);
                    }
                    else if (roi.Shape == 3)
                    {
                        float[] x = new float[points.Length];
                        float[] y = new float[points.Length];
                        for (int i = 0; i < points.Length; i++)
                        {
                            Point p = points[i];
                            x[i] = p.X + addX + 0.5f;
                            y[i] = p.Y + addY + 0.5f;
                        }

                        PolygonalFieldCut(x, y, rect);
                    }
                    //draw stack roi
                    
                   DrawStringToGL(fi, (roiList.IndexOf(roi) + 1).ToString(),roi, frame, rect);
                }
            }
        }
        
        private int id;
        public void BindTexture(TifFileInfo fi)
        {
            id = ImageTexture.GenerateActiveImageTexture(fi);
        }
        private void DrawTexture(TifFileInfo fi)
        {
            GLControl GLControl1 = this.GLControl1;
            
            GL.Enable(EnableCap.Texture2D);
            GL.BindTexture(TextureTarget.Texture2D, id);

            Rectangle rectOld = coRect[0][fi.cValue];
            Rectangle rect = new Rectangle(rectOld.X, rectOld.Y,
                rectOld.X + rectOld.Width, rectOld.Y + rectOld.Height);
           
            GL.Begin(BeginMode.Quads);

            GL.Color3(fi.LutList[fi.cValue]);

            GL.TexCoord2(0, 0);
            GL.Vertex2(rect.X, rect.Y);

            GL.TexCoord2(0, 1);
            GL.Vertex2(rect.X, rect.Height);

            GL.TexCoord2(1, 1);
            GL.Vertex2(rect.Width, rect.Height);

            GL.TexCoord2(1, 0);
            GL.Vertex2(rect.Width, rect.Y);

            GL.End();
            
            GL.Disable(EnableCap.Texture2D);

            
        }
        public void DrawStringToGL(TifFileInfo fi,string str, ROI roi, int imageN, Rectangle Borders)
        {
            if(ImageTexture.NumberID == null)
            {
                //ImageTexture
                ImageTexture.ReserveTextureID();
                ImageTexture.GenerateNumberTextures();
            }

            int symb = str.Length;
            float W = 13f / (float)zoom;
            float H = 15f / (float)zoom;
            float lineSpace = 7 / (float)zoom;
            
            PointF midP = roi.GetMidPoint(imageN);
            float X = Borders.X + midP.X - (lineSpace * (symb/2)) - 3/(float)zoom;
            float Y = Borders.Y + midP.Y - (H / 2);

            GLControl GLControl1 = this.GLControl1;

            GL.Enable(EnableCap.Blend);
            GL.Enable(EnableCap.Texture2D);

            RectangleF rect = new RectangleF(X, Y,
               X+W, Y+H);

            RectangleF BordersF = 
                new RectangleF(Borders.X, Borders.Y, Borders.Width, Borders.Height);

            foreach (char val in str)
            {
                if (BordersF.Contains(new PointF(rect.X, rect.Y)) &
                    BordersF.Contains(new PointF(rect.Width, rect.Height)))
                {
                    int code = ImageTexture.NumberID[int.Parse(val.ToString())];

                    GL.BindTexture(TextureTarget.Texture2D, code);
                    
                    GL.Begin(PrimitiveType.Quads);

                    GL.Color3(Color.Transparent);

                    GL.TexCoord2(0, 0);
                    GL.Vertex2(rect.X, rect.Y);

                    GL.TexCoord2(0, 1);
                    GL.Vertex2(rect.X, rect.Height);

                    GL.TexCoord2(1, 1);
                    GL.Vertex2(rect.Width, rect.Height);

                    GL.TexCoord2(1, 0);
                    GL.Vertex2(rect.Width, rect.Y);

                    GL.End();
                }

                rect.X += lineSpace;
                rect.Width = rect.X + W; 
            }
            GL.Disable(EnableCap.Texture2D);
            GL.Disable(EnableCap.Blend);
        }
        
        #endregion Draw ROI
    }
    class ContentPipe
    {
        #region Number Textures
        public int[] NumberID = null;
        public void GenerateNumberTextures()
        {
            NumberID = new int[10];
            for (int i = 0; i < NumberID.Length; i++)
            {
                int code = GL.GenTexture();
                NumberID[i] = code;
                //generate number bitmap
                Bitmap bmp = BitmapFromString(i.ToString());
                //create texture
                LoadNumberTexture(bmp, code);
            }
        }
        private Bitmap BitmapFromString(string str)
        {
            Font font = new Font("Times New Roman", 9, FontStyle.Bold);

            Bitmap bmp = new Bitmap(TextRenderer.MeasureText(str,font).Width,
                TextRenderer.MeasureText(str, font).Height,
                System.Drawing.Imaging.PixelFormat.Format32bppArgb);

            Rectangle rect = new Rectangle(0, 0, 
                TextRenderer.MeasureText(str, font).Width,
                TextRenderer.MeasureText(str, font).Height);

            //MessageBox.Show(rect.Width.ToString() + "\n" +                rect.Height.ToString());
            using (Graphics g = Graphics.FromImage(bmp))
            {
                g.FillRectangle(Brushes.Transparent, rect);
                g.DrawString(str, font, Brushes.Yellow, rect);
                g.Flush();
            }

            return bmp;
        }
        private void LoadNumberTexture(Bitmap bmp, int i)
        {
            //Load texture from file
            Bitmap texture_source = bmp;

            //Link empty texture to texture2d
            GL.BindTexture(TextureTarget.Texture2D, i);

            //Lock pixel data to memory and prepare for pass through
            BitmapData bitmap_data = texture_source.LockBits(
                new Rectangle(0, 0, texture_source.Width,
                texture_source.Height), ImageLockMode.ReadOnly,
                System.Drawing.Imaging.PixelFormat.Format32bppRgb);

            //Tell gl to write the data from are bitmap image/data to the bound texture

            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, texture_source.Width, texture_source.Height, 0, OpenTK.Graphics.OpenGL.PixelFormat.Bgra, PixelType.UnsignedByte, bitmap_data.Scan0);

            //Release from memory
            texture_source.UnlockBits(bitmap_data);
            //SetUp parametars
           /*
            //No anti-aliasing!
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.ClampToBorder);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.ClampToBorder);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)All.Nearest);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)All.Nearest);
            */
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.ClampToEdge);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.ClampToEdge);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)All.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)All.Linear);
        }
        #endregion Number Textures
        //Generate empty texture
        private int id;
        private int ChartID;
        
        public void ReserveTextureID()
        {
            id = GL.GenTexture();
            ChartID = GL.GenTexture();
        }
       public int LoadTexture(Bitmap bmp, bool NoAntiAliasing = false)
        {
            //Load texture from file
            Bitmap texture_source = bmp;

            //Link empty texture to texture2d
            if (NoAntiAliasing)
            {
                GL.BindTexture(TextureTarget.Texture2D, id);
            }
            else
            {
                GL.BindTexture(TextureTarget.Texture2D, ChartID);
            }

            //Lock pixel data to memory and prepare for pass through
            BitmapData bitmap_data = texture_source.LockBits(
                new Rectangle(0, 0, texture_source.Width, 
                texture_source.Height), ImageLockMode.ReadOnly, 
                System.Drawing.Imaging.PixelFormat.Format32bppRgb);

            //Tell gl to write the data from are bitmap image/data to the bound texture
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, texture_source.Width, texture_source.Height, 0, OpenTK.Graphics.OpenGL.PixelFormat.Bgra, PixelType.UnsignedByte, bitmap_data.Scan0);
            //Release from memory
            texture_source.UnlockBits(bitmap_data);
            //SetUp parametars
            if (NoAntiAliasing)
            {
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.ClampToEdge);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.ClampToEdge);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)All.Nearest);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)All.Nearest);
                return id;
            }
            else
            {
                //glTexParameteri(GL_TEXTURE_2D, GL_GENERATE_MIPMAP_SGIS, GL_TRUE);
                //GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.GenerateMipmapSgis, (float)All.True);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.ClampToEdge);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.ClampToEdge);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)All.Linear);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)All.Linear);
                return ChartID;
            }
           
        }
        public int GenerateActiveImageTexture(TifFileInfo fi)
        {
            Bitmap bmp = null;
            switch (fi.bitsPerPixel)
            {
                case 8:
                    bmp = Raw8ToBmp(fi);
                    break;
                case 16:
                    bmp = Raw16ToBmp(fi);
                    break;
            }
            id = LoadTexture(bmp,true);
            return id;
        }
        public void TextureFromBackBuffer(int Width, int Height)
        {
            GL.ReadBuffer(ReadBufferMode.Front);
            GL.BindTexture(TextureTarget.Texture2D, id);
            GL.CopyTexSubImage2D(TextureTarget.Texture2D, 0, 0, 0, 0, 0, Width, Height);
            //SetUp parametars
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Repeat);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Repeat);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)All.Nearest);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)All.Nearest);

        }
        private Bitmap Raw8ToBmp(TifFileInfo fi)
        {
            FrameCalculator FC = new FrameCalculator();
            //image array
            byte[][] image = fi.image8bit[FC.Frame(fi)];
            //new bitmap
            Bitmap bmp = new Bitmap(image[0].Length, image.Length, 
                System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            // Lock the bitmap's bits.
            Rectangle rect = new Rectangle(0, 0, bmp.Width, bmp.Height);
            BitmapData bmpData = bmp.LockBits(rect, ImageLockMode.ReadWrite, bmp.PixelFormat);
            // Get the address of the first line.
            IntPtr ptr = bmpData.Scan0;
            //store rgb values
            int bytes = Math.Abs(bmpData.Stride) * bmp.Height;
            byte[] rgbValues = new byte[bytes];
            // Copy the RGB values into the array
            System.Runtime.InteropServices.Marshal.Copy(ptr, rgbValues, 0, bytes);
            //take LUT info

            int position = 0;
            foreach (byte[] row in image)
            {
                foreach (byte val in row)
                {
                    byte val1 = (byte)(fi.adjustedLUT[fi.cValue][val] * 255);
                    rgbValues[position] = val1;
                    position++;
                    rgbValues[position] = val1;
                    position++;
                    rgbValues[position] = val1;
                    position++;
                    rgbValues[position] = 255;
                    position++;
                }
            }

            // Copy the RGB values back to the bitmap
            System.Runtime.InteropServices.Marshal.Copy(rgbValues, 0, ptr, bytes);
            // Unlock the bits.
            bmp.UnlockBits(bmpData);
            //return results
            return bmp;
        }
        private Bitmap Raw16ToBmp(TifFileInfo fi)
        {
            FrameCalculator FC = new FrameCalculator();
            //image array
            ushort[][] image = fi.image16bit[FC.Frame(fi)];
            //new bitmap
            Bitmap bmp = new Bitmap(image[0].Length, image.Length,
                System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            // Lock the bitmap's bits.
            Rectangle rect = new Rectangle(0, 0, bmp.Width, bmp.Height);
            BitmapData bmpData = bmp.LockBits(rect, ImageLockMode.ReadWrite, bmp.PixelFormat);
            // Get the address of the first line.
            IntPtr ptr = bmpData.Scan0;
            //store rgb values
            int bytes = Math.Abs(bmpData.Stride) * bmp.Height;
            byte[] rgbValues = new byte[bytes];
            // Copy the RGB values into the array
            System.Runtime.InteropServices.Marshal.Copy(ptr, rgbValues, 0, bytes);
            //take LUT info

            int position = 0;
            foreach (ushort[] row in image)
            {
                foreach (ushort val in row)
                {
                    byte val1 = (byte)(fi.adjustedLUT[fi.cValue][val] * 255);
                    rgbValues[position] = val1;
                    position++;
                    rgbValues[position] = val1;
                    position++;
                    rgbValues[position] = val1;
                    position++;
                    rgbValues[position] = 255;
                    position++;
                }
            }
            // Copy the RGB values back to the bitmap
            System.Runtime.InteropServices.Marshal.Copy(rgbValues, 0, ptr, bytes);
            // Unlock the bits.
            bmp.UnlockBits(bmpData);
            //return results
            return bmp;
        }
    }
}
