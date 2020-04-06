using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using CellToolDK;

namespace Colocalization
{
    public partial class MainForm : Form
    {
        TifFileInfo fi;
        Cell_Tool_3.ImageDrawer imgDrawer;
        public MainForm(TifFileInfo fi)
        {
            this.fi = fi;
            InitializeComponent();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            //add data            
            string[] channels = new string[fi.sizeC];
            for (int i = 0; i < channels.Length; i++)
                channels[i] = (i+1).ToString();

            this.comboBox_Ch1.Items.Clear();
            this.comboBox_Ch1.Items.AddRange(channels);
            this.comboBox_Ch1.SelectedIndex = 0;

            this.comboBox_Ch2.Items.Clear();
            this.comboBox_Ch2.Items.AddRange(channels);
            this.comboBox_Ch2.SelectedIndex = 1;

            if (fi.roiList == null || fi.roiList[0] == null || fi.roiList[0].Count == 0)
                checkBox_UseROIs.Enabled = false;
            else
                checkBox_UseROIs.Enabled = true;
            
            this.trackBar_TimeCh1.Minimum = 1;
            this.trackBar_TimeCh1.Maximum = fi.sizeT;
            this.trackBar_TimeCh1.Value = 1;
            this.textBox_TCh1.Text = "1";
            this.textBox_TCh1.Tag = this.trackBar_TimeCh1;
            this.trackBar_TimeCh1.Tag = this.textBox_TCh1;

            this.trackBar_TimeCh2.Minimum = 1;
            this.trackBar_TimeCh2.Maximum = fi.sizeT;
            this.trackBar_TimeCh2.Value = 1;
            this.textBox_TCh2.Text = "1";
            this.textBox_TCh2.Tag = this.trackBar_TimeCh2;
            this.trackBar_TimeCh2.Tag = this.textBox_TCh2;

            this.trackBar_ThreshCh1.Minimum = 1;
            if(fi.bitsPerPixel == 8)
                this.trackBar_ThreshCh1.Maximum = (int)Math.Pow(2,8)-1;
            else if (fi.bitsPerPixel == 16)
                this.trackBar_ThreshCh1.Maximum = (int)Math.Pow(2, 14) - 1;
            this.trackBar_ThreshCh1.Value = 1;
            this.textBox_ThreshCh1.Text = "1";
            this.textBox_ThreshCh1.Tag = this.trackBar_ThreshCh1;
            this.trackBar_ThreshCh1.Tag = this.textBox_ThreshCh1;

            this.trackBar_ThreshCh2.Minimum = 1;
            if (fi.bitsPerPixel == 8)
                this.trackBar_ThreshCh2.Maximum = (int)Math.Pow(2, 8) - 1;
            else if (fi.bitsPerPixel == 16)
                this.trackBar_ThreshCh2.Maximum = (int)Math.Pow(2, 14) - 1;
            this.trackBar_ThreshCh2.Value = 1;
            this.textBox_ThreshCh2.Text = "1";
            this.textBox_ThreshCh2.Tag = this.trackBar_ThreshCh2;
            this.trackBar_ThreshCh2.Tag = this.textBox_ThreshCh2;
            //GLControl
            this.imgDrawer = new Cell_Tool_3.ImageDrawer();
            this.imgDrawer.Initialize(panel_GLControl, this.fi);
            //add events
            this.FormClosing += Form_OnClosing;

            this.textBox_TCh1.LostFocus += TextBox_LostFocus;
            this.textBox_TCh1.KeyDown += TextBox_KeyPress;
            this.trackBar_TimeCh1.ValueChanged += TrackBar_ChangeValue;

            this.textBox_TCh2.LostFocus += TextBox_LostFocus;
            this.textBox_TCh2.KeyDown += TextBox_KeyPress;
            this.trackBar_TimeCh2.ValueChanged += TrackBar_ChangeValue;

            this.textBox_ThreshCh1.LostFocus += TextBox_LostFocus;
            this.textBox_ThreshCh1.KeyDown += TextBox_KeyPress;
            this.trackBar_ThreshCh1.ValueChanged += TrackBar_ChangeValue;

            this.textBox_ThreshCh2.LostFocus += TextBox_LostFocus;
            this.textBox_ThreshCh2.KeyDown += TextBox_KeyPress;
            this.trackBar_ThreshCh2.ValueChanged += TrackBar_ChangeValue;
            this.Resize += Form_Resizing;
            //Load image for the first time
            ReloadImage();
        }
        private void Form_Resizing(object sender,EventArgs e)
        {
            panel_GLControl.Width = this.Width - panel_Settings.Width-20;
            panel_GLControl.Height = this.Height-50;
        }
        private void Form_OnClosing(object sender, CancelEventArgs e)
        {
            imgDrawer.fi = null;
            fi.Delete();
            fi = null;
        }
        private void TrackBar_ChangeValue(object sender, EventArgs e)
        {
            TrackBar trackB = (TrackBar)sender;

            if (!trackB.Focused) return;

            TextBox textB = (TextBox)trackB.Tag;

            textB.Text = trackB.Value.ToString();

            if(checkBox_LockFrames.Checked && (trackB == trackBar_TimeCh1 || trackB == trackBar_TimeCh2))
            {
                TChangeApplyToAll(trackB.Value);
            }

            ReloadImage();
        }
        private void TextBox_LostFocus(object sender, EventArgs e)
        {
            TextBox textB = (TextBox)sender;

            TrackBar trackB = (TrackBar)textB.Tag;

            int a = 0;

            if(int.TryParse(textB.Text, out a))
            {
                if (a < trackB.Minimum)
                    a = trackB.Minimum;
                else if (a > trackB.Maximum)
                    a = trackB.Maximum;

                textB.Text = a.ToString();
                trackB.Value = a;

                if (checkBox_LockFrames.Checked && (trackB == trackBar_TimeCh1 || trackB == trackBar_TimeCh2))
                {
                    TChangeApplyToAll(trackB.Value);
                }

                ReloadImage();
            }
            else
            {
                textB.Text = trackB.Value.ToString();
            }
        }
        private void TextBox_KeyPress(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                TextBox_LostFocus(sender, e);
                e.Handled = true;
                e.SuppressKeyPress = true;
            }
        }
        private void checkBox_UseBinary_CheckedChanged(object sender, EventArgs e)
        {
            bool status = ((CheckBox)sender).Checked;

            this.textBox_ThreshCh1.Enabled = status;
            this.trackBar_ThreshCh1.Enabled = status;
            this.label7.Enabled = status;

            this.textBox_ThreshCh2.Enabled = status;
            this.trackBar_ThreshCh2.Enabled = status;
            this.label6.Enabled = status;

            ReloadImage();
        }

        private void checkBox_LockFrames_CheckedChanged(object sender, EventArgs e)
        {
            if (((CheckBox)sender).Checked)
            {
                TChangeApplyToAll(trackBar_TimeCh1.Value);
                ReloadImage();
            }
        }
        private void TChangeApplyToAll(int value)
        {
            this.textBox_TCh1.Text = value.ToString();
            this.textBox_TCh2.Text = value.ToString();
            this.trackBar_TimeCh1.Value = value;
            this.trackBar_TimeCh2.Value = value;
        }

        private void ReloadImage()
        {

            double[] vals = new double[] { 6.25, 12.5, 25, 50, 100, 200, 300, 400, 500, 600, 700, 800, 900, 1000 };
            imgDrawer.zoom = vals[comboBox_Zoom.SelectedIndex] / 100;

            imgDrawer.thresholds[0] = trackBar_ThreshCh1.Value;
            imgDrawer.thresholds[1] = trackBar_ThreshCh2.Value;

            imgDrawer.colIndexes[0] = comboBox_Ch1.SelectedIndex;
            imgDrawer.colIndexes[1] = comboBox_Ch2.SelectedIndex;

            imgDrawer.frames[0] = trackBar_TimeCh1.Value-1;
            imgDrawer.frames[1] = trackBar_TimeCh2.Value-1;

            imgDrawer.showBinary = checkBox_UseBinary.Checked;
            imgDrawer.showROI = checkBox_UseROIs.Checked;

            imgDrawer.DrawToScreen();
            GetStatistics();
        }

        private void comboBox_Zoom_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                ReloadImage();
            }
            catch { }
        }

        private void comboBox_Ch1_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                ReloadImage();
            }
            catch { }
        }

        private void comboBox_Ch2_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                ReloadImage();
            }
            catch { }
        }

        private void checkBox_UseROIs_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                ReloadImage();
            }
            catch { }
        }
        private void GetStatistics()
        {
            if (checkBox_UseROIs.Checked && checkBox_UseROIs.Enabled)
            {
                GetStatisticsROIs();
                return;
            }

            if (imgDrawer == null || imgDrawer.frames == null || fi == null) return;

            Point[] points = null;
            Cell_Tool_3.FrameCalculator frameCalc = new Cell_Tool_3.FrameCalculator();

            int[] curFrames = new int[2];
            fi.frame = imgDrawer.frames[0];
            curFrames[0] = frameCalc.FrameC(fi, imgDrawer.colIndexes[0]);
            fi.frame = imgDrawer.frames[1];
            curFrames[1] = frameCalc.FrameC(fi, imgDrawer.colIndexes[1]);

            switch (fi.bitsPerPixel)
            {
                case 8:
                    points = Operations.PairImages(fi.image8bit[curFrames[0]], fi.image8bit[curFrames[1]]);
                    break;
                case 16:
                    points = Operations.PairImages(fi.image16bit[curFrames[0]], fi.image16bit[curFrames[1]]);
                    break;
            }
            if (points == null) return;
            double[] MCC = null;
            //Filter By thresholds
            if (imgDrawer.showBinary)
            {
                points = Operations.FilterPointsByThresholds(points, imgDrawer.thresholds[0], imgDrawer.thresholds[1]);
                MCC = Operations.MCC(points, imgDrawer.thresholds[0], imgDrawer.thresholds[1]);
            }
            else
            {
                MCC = Operations.MCC(points, 1, 1);
            }

            //processing
            double PCC = Operations.PCC(points);
            double Rsquare = Math.Pow(PCC, 2.0);
            double MOC = Operations.MOC(points);

            // MessageBox.Show("PCC: " + PCC + "\nRsquared: " + Rsquare+"\nMOC: " + MOC
            // + "\nMCC 1: " + MCC[0] + "\nMCC 2: " + MCC[1]);

            //Generate data table
            DataTable tbl = new DataTable();

            tbl.Columns.Add(new DataColumn("Coefficient", typeof(string)));
            tbl.Columns.Add(new DataColumn("Value", typeof(double)));

            DataRow row = tbl.NewRow();
            row["Coefficient"] = "PCC";
            row["Value"] = PCC;
            tbl.Rows.Add(row);
            row = tbl.NewRow();
            row["Coefficient"] = "Rsquared";
            row["Value"] = Rsquare;
            tbl.Rows.Add(row);
            row = tbl.NewRow();
            row["Coefficient"] = "MOC";
            row["Value"] = MOC;
            tbl.Rows.Add(row);
            row = tbl.NewRow();
            row["Coefficient"] = "MCC 1";
            row["Value"] = MCC[0];
            tbl.Rows.Add(row);
            row = tbl.NewRow();
            row["Coefficient"] = "MCC 2";
            row["Value"] = MCC[1];
            tbl.Rows.Add(row);


            dataGridView1.DataSource = tbl;
        }
        private void GetStatisticsROIs()
        {

            if (imgDrawer == null || imgDrawer.frames == null || fi == null) return;
            //Generate data table
            DataTable tbl = new DataTable();

            tbl.Columns.Add(new DataColumn("Coefficient", typeof(string)));

            DataRow row = tbl.NewRow();
            row["Coefficient"] = "PCC";

            tbl.Rows.Add(row);
            row = tbl.NewRow();
            row["Coefficient"] = "Rsquared";
            tbl.Rows.Add(row);
            row = tbl.NewRow();
            row["Coefficient"] = "MOC";
            tbl.Rows.Add(row);
            row = tbl.NewRow();
            row["Coefficient"] = "MCC 1";
            tbl.Rows.Add(row);
            row = tbl.NewRow();
            row["Coefficient"] = "MCC 2";
            tbl.Rows.Add(row);

            //Points list
            Point[] points = null;
            ROI roi = null;
            Cell_Tool_3.FrameCalculator frameCalc = new Cell_Tool_3.FrameCalculator();
            int[] curFrames = new int[2];

            fi.frame = imgDrawer.frames[0];
            int roiFrame = frameCalc.FrameC(fi, 0);
            curFrames[0] = frameCalc.FrameC(fi, imgDrawer.colIndexes[0]);
            fi.frame = imgDrawer.frames[1];
            curFrames[1] = frameCalc.FrameC(fi, imgDrawer.colIndexes[1]);

            if (fi.roiList != null && fi.roiList.Length > 0 && fi.roiList[0] != null)
                for (int i = 0; i < fi.roiList[0].Count; i++)
                {
                    roi = fi.roiList[0][i];
                    points = ROIManager.GetROIPoints(roiFrame, roi, fi).ToArray();
                    if (points == null || points.Length < 2) return;

                    points = CheckArePointsInTheImage(points, fi.sizeX, fi.sizeY);
                    if (points == null || points.Length < 2) return;

                    switch (fi.bitsPerPixel)
                    {
                        case 8:
                            points = Operations.PairImages(fi.image8bit[curFrames[0]], fi.image8bit[curFrames[1]], points);
                            break;
                        case 16:
                            points = Operations.PairImages(fi.image16bit[curFrames[0]], fi.image16bit[curFrames[1]], points);
                            break;
                    }

                    if (points == null || points.Length < 2) return;

                    double[] MCC = null;
                    //Filter By thresholds
                    if (imgDrawer.showBinary)
                    {
                        points = Operations.FilterPointsByThresholds(points, imgDrawer.thresholds[0], imgDrawer.thresholds[1]);
                       // MessageBox.Show(points.Length.ToString());
                        MCC = Operations.MCC(points, imgDrawer.thresholds[0], imgDrawer.thresholds[1]);
                    }
                    else
                    {
                        MCC = Operations.MCC(points, 1, 1);
                    }

                    //processing
                    double PCC = Operations.PCC(points);
                    double Rsquare = Math.Pow(PCC, 2.0);
                    double MOC = Operations.MOC(points);

                    tbl.Columns.Add(new DataColumn("ROI " + (i+1), typeof(double)));

                    row = tbl.Rows[0];
                    row["ROI " + (i + 1)] = PCC;
                    row = tbl.Rows[1];
                    row["ROI " + (i + 1)] = Rsquare;
                    row = tbl.Rows[2];
                    row["ROI " + (i + 1)] = MOC;
                    row = tbl.Rows[3];
                    row["ROI " + (i + 1)] = MCC[0];
                    row = tbl.Rows[4];
                    row["ROI " + (i + 1)] = MCC[1];
                }

            dataGridView1.DataSource = tbl;
        }
        private Point[] CheckArePointsInTheImage(Point[] input, int w, int h)
        {
            List<Point> Output = new List<Point>();

            foreach (Point p in input)
                if (p.X >= 0 && p.Y >= 0 && p.X < w && p.Y < h)
                    Output.Add(p);

            return Output.ToArray();
        }

        private void button_SaveResults_Click(object sender, EventArgs e)
        {
            try
            {
                //get names
                string str = "";
                string dir = fi.Dir;
                string name = "(1)";
                dir = dir.Substring(0, dir.LastIndexOf("."));
                name = dir.Substring(dir.LastIndexOf("\\") + 1, dir.Length - dir.LastIndexOf("\\") - 1);

                if (!Directory.Exists(dir))
                    Directory.CreateDirectory(dir);
                //export settings
                str = "File:\t" + dir;
                str += "\nCh1:\t" + imgDrawer.colIndexes[0];
                str += "\nCh2:\t" + imgDrawer.colIndexes[1];
                str += "\nLock frames:\t" + checkBox_LockFrames.Checked;
                str += "\nTime Frame Ch1:\t" + trackBar_TimeCh1.Value;
                str += "\nTime Frame Ch2:\t" + trackBar_TimeCh2.Value;
                str += "\nUse thresholds:\t" + checkBox_UseBinary.Checked;
                str += "\nThreshold Ch1:\t" + trackBar_ThreshCh1.Value;
                str += "\nThreshold Ch2:\t" + trackBar_ThreshCh2.Value;
                str += "\nUse ROIs:\t" + checkBox_UseROIs.Checked;
                str += "\n\nResults:\n";
                
                //export dataTable
                using (StreamWriter write = new StreamWriter(File.Create(dir + @"\" + name + "_DataTable.txt")))
                {

                    int row = dataGridView1.Rows.Count;
                    int cell = dataGridView1.Rows[1].Cells.Count;

                    for (int i = 0; i < dataGridView1.Columns.Count; i++)
                        str += dataGridView1.Columns[i].Name + "\t";

                    write.WriteLine(str);

                    for (int i = 0; i < row; i++)
                    {
                        str = "";
                        for (int j = 0; j < cell; j++)
                        {
                            if (dataGridView1.Rows[i].Cells[j].Value == null)
                            {
                                dataGridView1.Rows[i].Cells[j].Value = "null";
                            }
                            str += dataGridView1.Rows[i].Cells[j].Value.ToString() + "\t";
                        }
                        write.WriteLine(str);
                    }

                    write.Close();
                    write.Dispose();
                }
                //Export plots
                Point[] values = null;
                List<Point> points = new List<Point>();
                List<Point> pointsFilt = new List<Point>();
                List<Point> valuesFilt = new List<Point>();

                Cell_Tool_3.FrameCalculator frameCalc = new Cell_Tool_3.FrameCalculator();

                int[] curFrames = new int[2];
                fi.frame = imgDrawer.frames[0];
                int roiFrame = frameCalc.FrameC(fi, 0);
                curFrames[0] = frameCalc.FrameC(fi, imgDrawer.colIndexes[0]);
                fi.frame = imgDrawer.frames[1];
                curFrames[1] = frameCalc.FrameC(fi, imgDrawer.colIndexes[1]);

                {
                    //Whole image
                    switch (fi.bitsPerPixel)
                    {
                        case 8:
                            values = Operations.PairImages(fi.image8bit[curFrames[0]], fi.image8bit[curFrames[1]]);
                            break;
                        case 16:
                            values = Operations.PairImages(fi.image16bit[curFrames[0]], fi.image16bit[curFrames[1]]);
                            break;
                    }

                    for (int y = 0, i = 0; y < fi.sizeY; y++)
                        for (int x = 0; x < fi.sizeX; x++, i++)
                        {
                            points.Add(new Point(x, y));
                            if (values[i].X >= imgDrawer.thresholds[0] || values[i].Y >= imgDrawer.thresholds[1])
                            {
                                pointsFilt.Add(points[i]);
                                valuesFilt.Add(values[i]);
                            }
                        }

                    ExportScaterPlot(dir, name + "_All", values, points.ToArray());
                    if (checkBox_UseBinary.Checked)
                        ExportScaterPlot(dir, name + "_All_noBG", valuesFilt.ToArray(), pointsFilt.ToArray());

                    points.Clear();
                    pointsFilt.Clear();
                    valuesFilt.Clear();
                    
                }
                if (checkBox_UseROIs.Checked && fi.roiList != null && fi.roiList.Length > 0 && fi.roiList[0] != null)
                    for (int n = 0; n < fi.roiList[0].Count; n++)
                    {
                        ROI roi = fi.roiList[0][n];
                        points = ROIManager.GetROIPoints(roiFrame, roi, fi);
                        points = CheckArePointsInTheImage(points.ToArray(), fi.sizeX, fi.sizeY).ToList();
                        //Whole image
                        switch (fi.bitsPerPixel)
                        {
                            case 8:
                                values = Operations.PairImages(fi.image8bit[curFrames[0]], fi.image8bit[curFrames[1]],points.ToArray());
                                break;
                            case 16:
                                values = Operations.PairImages(fi.image16bit[curFrames[0]], fi.image16bit[curFrames[1]], points.ToArray());
                                break;
                        }

                        for (int i = 0; i < points.Count; i++)
                        {
                            if (values[i].X >= imgDrawer.thresholds[0] || values[i].Y >= imgDrawer.thresholds[1])
                            {
                                pointsFilt.Add(points[i]);
                                valuesFilt.Add(values[i]);
                            }
                        }

                        ExportScaterPlot(dir, name + "_ROI" + (n+1), values, points.ToArray());
                        if (checkBox_UseBinary.Checked)
                            ExportScaterPlot(dir, name + "_ROI"+(n + 1) +"_noBG", valuesFilt.ToArray(), pointsFilt.ToArray());

                        points.Clear();
                        pointsFilt.Clear();
                        valuesFilt.Clear();
                    }
                //exit
                MessageBox.Show("Files saved!");
            }
            catch
            {
                MessageBox.Show("Error: Files NOT saved!");
            }
        }
        private void ExportScaterPlot(string dir,string name,Point[] values,Point[]coords)
        {
            //Export ScaterPlots
            using (StreamWriter write = new StreamWriter(File.Create(dir + @"\" + name + "_Scater.txt")))
            {
                write.WriteLine(string.Join("\t", new string[] { "X", "Y", "Image 1", "Image 2"}));

                for (int i = 0; i < values.Length; i++)
                    write.WriteLine(string.Join("\t", new int[] { coords[i].X, coords[i].Y, values[i].X, values[i].Y }));

                write.Close();
                write.Dispose();
            }
        }
    }
}
