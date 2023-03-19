using BuggyExchange.Properties;
using Microsoft.VisualBasic.Devices;
using System.Diagnostics;
using System.Drawing.Drawing2D;
using System.Numerics;
using System.Reflection;
using System.Text;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Window;

/// program notes
/// Some names have escape chars or other weird shit. They will not parse correctly from here.
/// use left/right mouse to select/deselect, hold shift to make a selection rectangle
/// 
/// Please note: this code was hastily written, not intended for reusability or teaching, and I don't care that it's a complete clusterfuck.



namespace BuggyExchange
{
    public partial class Form1 : Form
    {
        int saveIndex = 0;

        byte[] buff;
        int numSourceCars = 0;
        string[] frameTypes;
        float[] framePositionsX, framePositionsY, framePositionsZ;
        float[] buggyDrawX, buggyDrawY; //drawing positions of cars
        float[] frameRotationsX, frameRotationsY, frameRotationsZ;
        Bitmap map, minimap;
        float lastMouseX, lastMouseY;
        int nearestBuggy = -1;






        //float zoomAmount = 1f;
        float totalZoom;
        //bool scaleInvalid = false; //set this to true if scale needs to be recalculated for all the drawing stuff.



        public void ClickLabel(Object sender, System.EventArgs e)
        {
            Label btn = (Label)sender;
            for (int i = 0; i < 11; i++)
            {
                lblSlot[i].BorderStyle = BorderStyle.FixedSingle;
                //lblSlot[i].BackColor = SystemColors.Control;
            }
            saveIndex = int.Parse(btn.Tag.ToString());
            colorSlots();
            btn.BorderStyle = BorderStyle.None;
            btn.BackColor = Color.LightBlue;
            if (btn.Tag == "0")
            {
                saveFileDialog1.Title = "Choose a filename for the output file.";
                saveFileDialog1.InitialDirectory = openFileDialog1.InitialDirectory;
                saveFileDialog1.Filter = openFileDialog1.Filter;
                saveFileDialog1.FileName = "New Buggies.sav";
                DialogResult res = saveFileDialog1.ShowDialog();
                if (res != DialogResult.OK)
                {
                    saveFileDialog1.FileName = "New Buggies.sav";
                }
            }
            //MessageBox.Show("You clicked character [" + btn.Text + "]");
        }
        System.Windows.Forms.Label[] lblSlot;
        public Form1()
        {
            InitializeComponent();
            setHeaders();
            this.pictureBox1.MouseWheel += new System.Windows.Forms.MouseEventHandler(this.pictureBox2_MouseWheel);
            this.pictureBox2.MouseWheel += new System.Windows.Forms.MouseEventHandler(this.pictureBox2_MouseWheel);

            this.KeyPreview = true;
            checkedListBox1.Items.Clear();
            //checkedListBox1.Items.Add("Nothing yet...");
            map = new Bitmap(pictureBox1.Width, pictureBox1.Height);
            minimap = new Bitmap(pictureBox2.Width, pictureBox2.Height);

            //init gradecolors
            int pSize = 7;
            gradeColor = new Pen[21];
            gradeColor[0] = new Pen( Color.FromArgb(255, 255, 255),pSize); //0% grade WHITE
            gradeColor[1] = new Pen( Color.FromArgb(128, 128, 128),pSize);
            gradeColor[2] = new Pen( Color.FromArgb(0, 255, 0),pSize); //1% GREEN
            gradeColor[3] = new Pen( Color.FromArgb(128, 255, 128),pSize);
            gradeColor[4] = new Pen( Color.FromArgb(255, 255, 0),pSize); //2% YELLOW
            gradeColor[5] = new Pen( Color.FromArgb(255, 255,128),pSize);
            gradeColor[6] = new Pen( Color.FromArgb(255, 0, 0),pSize); //3% RED
            gradeColor[7] = new Pen( Color.FromArgb(255, 128, 128),pSize);
            gradeColor[8] = new Pen( Color.FromArgb(0, 0, 255),pSize); //4% BLUE
            gradeColor[9] = new Pen( Color.FromArgb(128, 128, 255),pSize);
            gradeColor[10] = new Pen( Color.FromArgb(0, 255, 0),pSize); //1% GREEN
            gradeColor[11] = new Pen( Color.FromArgb(128, 255, 128),pSize);
            gradeColor[12] = new Pen( Color.FromArgb(255, 255, 0),pSize); //2% YELLOW
            gradeColor[13] = new Pen( Color.FromArgb(255, 255, 128),pSize);
            gradeColor[14] = new Pen( Color.FromArgb(255, 0, 0),pSize); //3% RED
            gradeColor[15] = new Pen( Color.FromArgb(255, 128, 128),pSize);
            gradeColor[16] = new Pen( Color.FromArgb(0, 0, 255),pSize); //4% BLUE
            gradeColor[17] = new Pen( Color.FromArgb(128, 128, 255),pSize);
            gradeColor[18] = new Pen( Color.FromArgb(255, 0, 255),pSize); //9% PURPLE
            gradeColor[19] = new Pen( Color.FromArgb(255, 128, 255),pSize);
            gradeColor[20] = new Pen( Color.FromArgb(0, 0, 0),pSize); //10% or above BLACK

            //init track type pens
            pSize = 3;
            tracktypePen = new Pen[11];
            tracktypePen[0] = new Pen(Color.FromArgb(255, 255, 255), pSize); // WHITE
            tracktypePen[1] = new Pen(Color.FromArgb(139, 69, 19), pSize); //brown
            tracktypePen[2] = new Pen(Color.FromArgb(192, 192, 192), pSize); // plain rails
            tracktypePen[3] = new Pen(Color.FromArgb(192, 192, 192), pSize); 
            tracktypePen[4] = new Pen(Color.FromArgb(192, 192, 192), pSize); 
            tracktypePen[5] = new Pen(Color.FromArgb(192, 192, 192), pSize); 
            tracktypePen[6] = new Pen(Color.FromArgb(255, 0,0), pSize); //Bumper
            tracktypePen[7] = new Pen(Color.FromArgb(210, 180, 140), pSize); //wood trestle
            tracktypePen[8] = new Pen(Color.FromArgb(128,0,0), pSize); //Fancy wood trestle with walkway
            tracktypePen[9] = new Pen(Color.FromArgb(128, 128, 192), pSize); //steel trestle
            tracktypePen[10] = new Pen(Color.FromArgb(192, 192, 64), pSize); //Stone Wall

            getAllTreesFromFile();

            //parseSaveNow(@"C:\Users\abc\AppData\Local\arr\Saved\SaveGames/slot1.sav");

            //if save dir can't be located, user must navigate to it.
            //After this a txt file will be used to guide us to the proper save location
            if (File.Exists("savedir.txt"))
            {
                string newdir = File.ReadAllText("savedir.txt");
                if (Directory.Exists(newdir))
                {
                    openFileDialog1.InitialDirectory = newdir;
                    saveFileDialog1.InitialDirectory = newdir;
                }
                else
                {
                    File.Delete("savedir.txt");
                    MessageBox.Show("Invalid directory specified in savedir.txt file. Please try again.");
                    this.Close();
                }

            }
            else
            {
                if (!Directory.Exists(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + @"\arr\Saved\SaveGames"))
                {
                    FolderBrowserDialog dialog = new System.Windows.Forms.FolderBrowserDialog();
                    dialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
                    dialog.Description = "Find your SaveGames folder.";
                    System.Windows.Forms.DialogResult result = dialog.ShowDialog();
                    if (result == DialogResult.OK && dialog.SelectedPath != null)
                    {
                        openFileDialog1.InitialDirectory = dialog.SelectedPath;
                        saveFileDialog1.InitialDirectory = dialog.SelectedPath;
                        File.WriteAllText("savedir.txt", dialog.SelectedPath);//this should let us find it automatically in future sessions
                    }
                    else
                    {
                        MessageBox.Show("No valid folder selected.\nPlease locate your saves folder. It should be located in your appdata under arr/saved/savegames or similar\nByeeeee.");
                    }
                }
                else
                {
                    openFileDialog1.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + @"\arr\Saved\SaveGames";
                    saveFileDialog1.InitialDirectory = openFileDialog1.InitialDirectory;
                }
            }

            openFileDialog1.Filter = "Sav files (*.sav)|*.sav";
            saveFileDialog1.FileName = "New Buggies.sav";


            lblSlot = new Label[11];
            for (int i = 0; i < lblSlot.Length; i++)
            {
                lblSlot[i] = new Label();
                lblSlot[i].Tag = i.ToString();
                lblSlot[i].Text = "slot" + i.ToString();
                lblSlot[i].Width = 46;
                lblSlot[i].Height = 17;
                lblSlot[i].Top = 0;
                lblSlot[i].Left = i * (lblSlot[0].Width + 3);
                lblSlot[i].BorderStyle = BorderStyle.FixedSingle;
                lblSlot[i].BackColor = SystemColors.Control;
                panel1.Controls.Add(lblSlot[i]);
                lblSlot[i].Visible = true;
                lblSlot[i].Enabled = true;
                lblSlot[i].Click += new EventHandler(ClickLabel);
            }
            lblSlot[0].Text = "New";
            lblSlot[0].BorderStyle = BorderStyle.None;
            lblSlot[0].BackColor = Color.LightBlue;
            label3.BackColor = Color.LightBlue;

            colorSlots();




            setZoomRectangle();
            scaleChanged();
            //System.Diagnostics.Process.Start("explorer.exe", Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + @"\arr\Saved\SaveGames");

        }



        private void pictureBox1_MouseWheel(object sender, System.Windows.Forms.MouseEventArgs e)
        {

        }

        private void colorSlots()
        {
            for (int i = 1; i < 11; i++)
            {
                if (File.Exists(saveFileDialog1.InitialDirectory + @"\" + "slot" + i.ToString() + ".sav"))
                    lblSlot[i].BackColor = Color.DarkRed;
                else
                    lblSlot[i].BackColor = SystemColors.Control;
            }
            if (File.Exists(saveFileDialog1.InitialDirectory + @"\" + saveFileDialog1.FileName))
                lblSlot[0].BackColor = Color.DarkRed;
            else
                lblSlot[0].BackColor = SystemColors.Control;
        }

        private void label2_DragDrop(object sender, DragEventArgs e)
        {
            if (e.Data.GetData(DataFormats.FileDrop) == null) return;
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
            if (files.Length == 0) return;
            foreach (string file in files) Console.WriteLine(file);
            this.Text = files[0];
            parseTargetSaveNow(files[0]);
        }

        private void label2_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop)) e.Effect = DragDropEffects.Copy;
        }

        private void label1_DragDrop(object sender, DragEventArgs e)
        {
            if (e.Data.GetData(DataFormats.FileDrop) == null) return;
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
            if (files.Length == 0) return;
            foreach (string file in files) Console.WriteLine(file);
            this.Text = files[0];
            parseSaveNow(files[0]);
        }

        private void label1_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop)) e.Effect = DragDropEffects.Copy;
        }



        private void checkedListBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            nearestBuggy = checkedListBox1.SelectedIndex;
            showBuggiesOnMap();
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            //!checkedListBox1.GetItemChecked(nearestBuggy)


        }

        //adds or subtracts trees based on user mouse position and other settings
        void addTreesPaint(float x, float y)
        {
            Math.Clamp(x, 0f, 850f);
            Math.Clamp(y, 0f, 850f);

            //need to calculate zoomed position based on current zoom rectangle
            //left of the picturebox is total map size scaled to 0-1
            float mapLeft = (zoomLeft * 400000f) - 200000f;
            float mapx = mapLeft + (((x / 850f) * (zoomRight - zoomLeft)) * 400000f);
            float mapTop = (zoomTop * 400000f) - 200000f;
            float mapy = mapTop + (((y / 850f) * (zoomBottom - zoomTop)) * 400000f);
            //this.Text = mapx.ToString("0.00") + " " + mapy.ToString("0.00");

            //find any trees within a given radius of this place:
            //get the regions using 8 points in radius to ensure we don't miss any.

            bool rem = checkBoxRemoveTrees.Checked;
            removeRadiusTrees(-mapx, -mapy, trackBar1.Value * 100, rem);
            //removeNearestTree(-mapx, -mapy);
            showBuggiesOnMap();
        }

        bool treeDragging = false;

        int dragging = 0;
        float dragX, dragY;
        float dx1, dx2, dy1, dy2; //drag rectangle positions for drawing

        private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            if (checkBoxAddTrees.Checked || checkBoxRemoveTrees.Checked)
            {
                treeDragging = true;
                addTreesPaint(e.X, e.Y);
                return;
            }

            if (checkBox6.Checked)
            {
                splineDragging = true;
                //FIXME: add code here
                return;
            }

            dragging = 0;
            if (e.Button == MouseButtons.Right)
            {
                if (Control.ModifierKeys == Keys.Shift)
                {
                    dragging = 4; //deselect in a rectangle
                    dragX = e.X;
                    dragY = e.Y;
                }
                else
                {
                    dragging = 2; //deselect nearest one
                    if (nearestBuggy > -1)
                    {
                        checkedListBox1.SelectedIndex = nearestBuggy;
                        checkedListBox1.SetItemChecked(nearestBuggy, false);
                    }
                }

            }

            if (e.Button == MouseButtons.Left)
            {
                if (Control.ModifierKeys == Keys.Shift)
                {
                    dragging = 3; //select in a rectangle
                    dragX = e.X;
                    dragY = e.Y;
                }
                else
                {
                    dragging = 1; //select nearest one
                    if (nearestBuggy > -1)
                    {
                        checkedListBox1.SelectedIndex = nearestBuggy;
                        checkedListBox1.SetItemChecked(nearestBuggy, true);
                    }
                }
            }

        }

        private void pictureBox1_MouseUp(object sender, MouseEventArgs e)
        {
            //if dragging, make sure everything inside the rectangle is checked before disabling the drag flag
            treeDragging = false;
            splineDragging = false;
            dragging = 0;
        }

        bool splineDragging = false;
        int activeSpline = -1;
        int activeSplineType = -1;
        //0 = start point
        //1 = start tangent
        //2 = end tangent
        //3 = end point
        //Vector3[] splineDivisions = new Vector3[1000];


        //put the active spline's info in the labels
        private void showSplineStats()
        {
            if (activeSpline < 0)
            {
                labelSplineIndex.Text = "None Selected";
                labelTrackType.Text = "None Selected";
                labelTrackGrade.Text = "None Selected";
                label9.Text = "None Selected";
                label11.Text = "None Selected";
                label12.Text = "None Selected";
                label13.Text = "None Selected";
                return;
            }

            labelSplineIndex.Text = "Index " + activeSpline.ToString();
            labelTrackType.Text = TrackSegmentNames[activeSpline];
            labelTrackGrade.Text = trackGrade[activeSpline].ToString("0.00");
            label9.Text =trackStart[activeSpline].ToString("0.00");
            label11.Text = tangentStart[activeSpline].ToString("0.00");
            label12.Text = tangentEnd[activeSpline].ToString("0.00");
            label13.Text = trackEnd[activeSpline].ToString("0.00");

        }

        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            lastMouseX = e.X;
            lastMouseY = e.Y;
            float dist;
            float nearest;


            if (treeDragging)
            {
                if (checkBoxAddTrees.Checked || checkBoxRemoveTrees.Checked)
                {
                    addTreesPaint(e.X, e.Y);
                    return;
                }
            }


            if (splineDragging)
            {

            }

            //if we're in spline editing mode but not dragging, 
            if (checkBox6.Checked)
            {   //find nearest spline so we can highlight or do things to it
                nearest = 999999f;
                for (int i = 0; i < trackDrawStart.Length; i++)
                {
                    float dx = lastMouseX - trackDrawStart[i].X;
                    float dy = lastMouseY - trackDrawStart[i].Y;
                    dist = dx * dx + dy * dy;

                    if (dist < nearest)
                    {
                        nearest = dist;
                        activeSpline = i;
                    }
                    dx = lastMouseX - trackDrawEnd[i].X;
                    dy = lastMouseY - trackDrawEnd[i].Y;
                    dist = dx * dx + dy * dy;

                    if (dist < nearest)
                    {
                        nearest = dist;
                        activeSpline = i;
                    }
                }
                showSplineStats();

            }
            else
            {
                //this.Text = lastMouseX + " " + lastMouseY;
                //checkedListBox1.Items.Clear();

                nearest = 999999f;
                for (int i = 0; i < numSourceCars; i++)
                {
                    float dx = lastMouseX - buggyDrawX[i];
                    float dy = lastMouseY - buggyDrawY[i];
                    dist = dx * dx + dy * dy;
                    //checkedListBox1.Items.Add(frameTypes[i] + " = " + dist.ToString() ,CheckState.Unchecked );  

                    if (dist < nearest)
                    {
                        nearest = dist;
                        nearestBuggy = i;
                    }
                }
            }


            if (dragging == 3 || dragging == 4)
            {
                //establish our rectangle

                if (e.X < dragX)
                {
                    dx1 = e.X;
                    dx2 = dragX;
                }
                else
                {
                    dx1 = dragX;
                    dx2 = e.X;
                }
                if (e.Y < dragY)
                {
                    dy1 = e.Y;
                    dy2 = dragY;
                }
                else
                {
                    dy1 = dragY;
                    dy2 = e.Y;
                }

                bool toCheck = true;
                if (dragging == 4) toCheck = false;

                for (int i = 0; i < numSourceCars; i++)
                {
                    if (buggyDrawX[i] >= dx1 && buggyDrawX[i] <= dx2)
                        if (buggyDrawY[i] >= dy1 && buggyDrawY[i] <= dy2)
                            checkedListBox1.SetItemChecked(i, toCheck);
                }
            }

            if (nearestBuggy > -1)
            {
                checkedListBox1.SelectedIndex = nearestBuggy;

                if (dragging == 1) checkedListBox1.SetItemChecked(nearestBuggy, true);
                if (dragging == 2) checkedListBox1.SetItemChecked(nearestBuggy, false);
            }

            showBuggiesOnMap();
        }



        private void pictureBox1_Paint(object sender, PaintEventArgs e) { }

        float mapMax = 200000f, mapMax2 = 400000f;
        //call this whenever scale needs to be recalculated.
        void scaleChanged()
        {

            //using the zoom rectangle dragged out in picture2, determine our scale and offsets
            totalZoom = 850f / (zoomRight - zoomLeft);

            label6.Text = "Zoom = " + (totalZoom / 850f).ToString("0.00");

            //source cars
            for (int c = 0; c < numSourceCars; c++)
            {
                //calculate the map positions of the cars.
                buggyDrawX[c] = ((((-framePositionsX[c] + mapMax) / mapMax2) - zoomLeft) * totalZoom);
                buggyDrawY[c] = ((((-framePositionsY[c] + mapMax) / mapMax2) - zoomTop) * totalZoom);
            }

            //map cars
            for (int c = 0; c < numTargetCars; c++)
            {
                //calculate the map positions of the cars.
                buggyDraw2X[c] = ((((-framePositions2X[c] + mapMax) / mapMax2) - zoomLeft) * totalZoom);
                buggyDraw2Y[c] = ((((-framePositions2Y[c] + mapMax) / mapMax2) - zoomTop) * totalZoom);
            }

            //rescale all the drawing positions:
            for (int i = 0; i < treePos.Length; i++)
            {
                treeDraw[i].X = ((((-treePos[i].X + mapMax) / mapMax2) - zoomLeft) * totalZoom);
                treeDraw[i].Y = ((((-treePos[i].Y + mapMax) / mapMax2) - zoomTop) * totalZoom);
            }
            //tracks:
            for (int c = 0; c < numTracks; c++)
            {
                trackDrawStart[c].X = ((((-trackStart[c].X + mapMax) / mapMax2) - zoomLeft) * totalZoom);
                trackDrawStart[c].Y = ((((-trackStart[c].Y + mapMax) / mapMax2) - zoomTop) * totalZoom);
                tangentDrawStart[c].X = ((((-tangentStart[c].X + mapMax) / mapMax2) - zoomLeft) * totalZoom);
                tangentDrawStart[c].Y = ((((-tangentStart[c].Y + mapMax) / mapMax2) - zoomTop) * totalZoom);
                tangentDrawEnd[c].X = ((((-tangentEnd[c].X + mapMax) / mapMax2) - zoomLeft) * totalZoom);
                tangentDrawEnd[c].Y = ((((-tangentEnd[c].Y + mapMax) / mapMax2) - zoomTop) * totalZoom);
                trackDrawEnd[c].X = ((((-trackEnd[c].X + mapMax) / mapMax2) - zoomLeft) * totalZoom);
                trackDrawEnd[c].Y = ((((-trackEnd[c].Y + mapMax) / mapMax2) - zoomTop) * totalZoom);
            }

            //recalculate the background image drawing points

            //if scale changed, everything needs to be redrawn
            showBuggiesOnMap();

        }

        Pen trackPen = new Pen(Color.Beige, 2f);
        Pen handlePen = new Pen(Color.Red, 2f);
        Pen handlePen2 = new Pen(Color.Blue, 2f);
        Pen handlePenGreen = new Pen(Color.Green, 2f);
        Pen handlePen3 = new Pen(Color.White, 3f);
        Brush bRed = new SolidBrush(Color.Red);
        Brush bDarkRed = new SolidBrush(Color.DarkRed);
        Brush bWhite = new SolidBrush(Color.White);
        Brush bYellow = new SolidBrush(Color.Yellow);
        Brush bBlack = new SolidBrush(Color.Black);
        Brush bTrees = new SolidBrush(Color.Green);
        bool flashOn = false;
        private void showBuggiesOnMap()
        {
            using (Graphics g = Graphics.FromImage(minimap))
            {
                g.Clear(Color.Black);
                g.DrawImage(Resources.rro_map, 0, 0, 255, 255);
                //int x = (int)(-drawOffsetX * 0.3 / zoomAmount);
                //int y = (int)(-drawOffsetY * 0.3 / zoomAmount);
                //g.DrawRectangle(trackPen, x, y, (255 / zoomAmount), (255 / zoomAmount));
                g.DrawRectangle(trackPen, zoomLeft * 255f, zoomTop * 255f, (zoomRight - zoomLeft) * 255f, (zoomBottom - zoomTop) * 255f);
            }
            pictureBox2.Image = minimap;
            pictureBox2.Refresh();

            //map = new Bitmap(Resources.ResourceManager. rro_map);
            using (Graphics g = Graphics.FromImage(map))
            {
                g.Clear(Color.Black);
                g.DrawImage(Resources.reliefmap, new Rectangle(0, 0, 850, 850), new Rectangle((int)(zoomLeft * 4000f), (int)(zoomTop * 4000f), (int)((zoomRight - zoomLeft) * 4000f), (int)((zoomBottom - zoomTop) * 4000f)), GraphicsUnit.Pixel);

                //testing to see if our algorithm lines up with the built-in one
                //for (int t = 0; t < numTracks; t++)
                //{
                //    Vector2 prevPoint = trackDrawStart[t];
                //    Vector2 aLen = tangentDrawStart[t] - trackDrawStart[t];
                //    Vector2 bLen = tangentDrawEnd[t] - tangentDrawStart[t];
                //    Vector2 cLen = trackDrawEnd[t] - tangentDrawEnd[t];
                //    for (float s = 0.01f; s <= 1.0f; s += 0.01f)
                //    {
                //        //get the spline point at this distance along
                //        Vector2 a = trackDrawStart[t] + (aLen * s);
                //        Vector2 b = tangentDrawStart[t] + (bLen * s);
                //        Vector2 c = tangentDrawEnd[t] + (cLen * s);

                //        Vector2 dLen = b - a;
                //        Vector2 d = a + (dLen * s);
                //        Vector2 eLen = c - b;
                //        Vector2 e = b + (eLen * s);

                //        Vector2 fLen = e - d;
                //        Vector2 f = d + (fLen * s); //final spline position

                //        g.DrawLine(new Pen(bRed,5.0f), f.X,f.Y, prevPoint.X,prevPoint.Y);
                //        prevPoint = f;

                //    }
                //}

                if (checkBox7.Checked)
                {
                    for (int i = 0; i < numTracks; i++)
                    {
                        g.DrawBezier(gradeColor[trackColor[i]], trackDrawStart[i].X, trackDrawStart[i].Y, tangentDrawStart[i].X, tangentDrawStart[i].Y, tangentDrawEnd[i].X, tangentDrawEnd[i].Y, trackDrawEnd[i].X, trackDrawEnd[i].Y);
                    }
                }

                //draw track segments
                if (checkBox3.Checked)
                {
                    if (checkBox8.Checked)
                    { //show in type colors
                        for (int i = 0; i < numTracks; i++)
                        {
                            g.FillRectangle(bBlack, trackDrawStart[i].X, trackDrawStart[i].Y, 5f, 5f);
                            g.DrawBezier(tracktypePen[trackTypeColor[i]], trackDrawStart[i].X, trackDrawStart[i].Y, tangentDrawStart[i].X, tangentDrawStart[i].Y, tangentDrawEnd[i].X, tangentDrawEnd[i].Y, trackDrawEnd[i].X, trackDrawEnd[i].Y);
                        }
                    }
                    else
                    { //just draw all tracks in white
                        for (int i = 0; i < numTracks; i++)
                        {
                            g.FillRectangle(bBlack, trackDrawStart[i].X, trackDrawStart[i].Y, 5f, 5f);
                            g.DrawBezier(handlePen3, trackDrawStart[i].X, trackDrawStart[i].Y, tangentDrawStart[i].X, tangentDrawStart[i].Y, tangentDrawEnd[i].X, tangentDrawEnd[i].Y, trackDrawEnd[i].X, trackDrawEnd[i].Y);
                        }
                    }
                    
                    //draw the selected segment (if any) in a highlighted or flashing color
                    if (activeSpline > -1)
                    {
                        g.FillRectangle(bRed, trackDrawStart[activeSpline].X, trackDrawStart[activeSpline].Y, 5f, 5f);
                        g.DrawBezier(handlePenGreen, trackDrawStart[activeSpline].X, trackDrawStart[activeSpline].Y, tangentDrawStart[activeSpline].X, tangentDrawStart[activeSpline].Y, tangentDrawEnd[activeSpline].X, tangentDrawEnd[activeSpline].Y, trackDrawEnd[activeSpline].X, trackDrawEnd[activeSpline].Y);

                        //draw the four points (start end and two tangents)
                        //start to tangent 1
                        g.DrawLine(handlePen, trackDrawStart[activeSpline].X, trackDrawStart[activeSpline].Y, tangentDrawStart[activeSpline].X, tangentDrawStart[activeSpline].Y);
                        //end to tangent 2
                        g.DrawLine(handlePen, tangentDrawEnd[activeSpline].X, tangentDrawEnd[activeSpline].Y, trackDrawEnd[activeSpline].X, trackDrawEnd[activeSpline].Y);
                        if (activeSplineType == 1) g.FillRectangle(bYellow, trackDrawStart[activeSpline].X, trackDrawStart[activeSpline].Y, 6f, 6f);
                        else if (activeSplineType == 2) g.FillRectangle(bYellow, tangentDrawStart[activeSpline].X, tangentDrawStart[activeSpline].Y, 6f, 6f);
                        else if (activeSplineType == 3) g.FillRectangle(bYellow, tangentDrawEnd[activeSpline].X, tangentDrawEnd[activeSpline].Y, 6f, 6f);
                        else if (activeSplineType == 4) g.FillRectangle(bYellow, trackDrawEnd[activeSpline].X, trackDrawEnd[activeSpline].Y, 6f, 6f);
                    }

                }

                //draw Target File's cars on the map
                if (checkBox4.Checked)
                {
                    for (int i = 0; i < numTargetCars; i++)
                        g.FillRectangle(bYellow, buggyDraw2X[i], buggyDraw2Y[i], 5f, 5f);

                    if (dragging == 3 || dragging == 4)
                    {
                        g.DrawRectangle(Pens.LightBlue, dx1, dy1, dx2 - dx1, dy2 - dy1);
                    }
                }


                //draw New cars on the map
                if (checkBox5.Checked)
                {
                    for (int i = 0; i < numSourceCars; i++)
                    {
                        if (checkedListBox1.GetItemChecked(i))
                            g.FillRectangle(bWhite, buggyDrawX[i], buggyDrawY[i], 3f, 3f);
                        else
                            g.FillRectangle(bRed, buggyDrawX[i], buggyDrawY[i], 3f, 3f);
                    }
                    if (nearestBuggy > -1)
                    {
                        g.FillRectangle(new SolidBrush(Color.Blue), buggyDrawX[nearestBuggy], buggyDrawY[nearestBuggy], 5f, 5f);
                    }
                }


                if (checkBox1.Checked) //only check once if we should draw the deleted trees, otherwise we're wasting time rechecking it
                {
                    if (checkBox2.Checked) //only check once if we should draw the deleted trees, otherwise we're wasting time rechecking it
                    {
                        //draw the trees:
                        //add a checkbox to show/hide trees
                        for (int i = 0; i < treeExists.Length; i++)
                            if (treeExists[i])
                                g.FillRectangle(bTrees, treeDraw[i].X, treeDraw[i].Y, 2, 2);
                            else
                                g.FillRectangle(bDarkRed, treeDraw[i].X, treeDraw[i].Y, 2, 2);
                    }
                    else
                    {
                        //draw the trees:
                        //add a checkbox to show/hide trees
                        for (int i = 0; i < treeExists.Length; i++)
                            if (treeExists[i])
                                g.FillRectangle(bTrees, treeDraw[i].X, treeDraw[i].Y, 2, 2);
                    }
                }


                //bool han = false;
                ////draw splines manually
                //for (int i = 0; i < trackDrawStart.Length; i++)
                //{
                //    han = !han;
                //    //longest possible segment is distance between start and end * pi
                //    float dx, dy, dist;
                //    dx = trackDrawStart[i].X - trackDrawEnd[i].X;
                //    dy = trackDrawStart[i].Y - trackDrawEnd[i].Y;
                //    dist = 1f / ((float)Math.Sqrt(dx * dx + dy * dy) * 0.0314159f);
                //    float lastx = trackDrawStart[i].X, lasty = trackDrawStart[i].Y;

                //    for (float w = 0f; w < 1f; w += dist)
                //    {
                //        //get the spline point at this location:
                //        float ax = trackDrawStart[i].X + ((tangentDrawStart[i].X - trackDrawStart[i].X) * w);
                //        float bx = tangentDrawStart[i].X + ((tangentDrawEnd[i].X - tangentDrawStart[i].X) * w);
                //        float cx = tangentDrawEnd[i].X + ((trackDrawEnd[i].X - tangentDrawEnd[i].X )* w);
                //        float ex = ax + ((bx - ax) * w);
                //        float fx = bx + ((cx - bx) * w);
                //        float gx = ex + ((fx - ex) * w);

                //        float ay = trackDrawStart[i].Y + ((tangentDrawStart[i].Y - trackDrawStart[i].Y) * w);
                //        float by = tangentDrawStart[i].Y + ((tangentDrawEnd[i].Y - tangentDrawStart[i].Y) * w);
                //        float cy = tangentDrawEnd[i].Y + ((trackDrawEnd[i].Y - tangentDrawEnd[i].Y) * w);
                //        float ey = ay + ((by - ay) * w);
                //        float fy = by + ((cy - by) * w);
                //        float gy = ey + ((fy - ey) * w);

                //        g.FillRectangle(bBlack, gx, gy, 4, 4);
                //        //each step along the path, draw a short line segment
                //        if (han) g.DrawLine(handlePen, gx, gy, lastx, lasty);
                //        else g.DrawLine(handlePen2, gx, gy, lastx, lasty);
                //        lastx = gx;
                //        lasty = gy;
                //    }
                //}



            }
            pictureBox1.Image = map;
            pictureBox1.Refresh();
        }

        int findInBytes(string pattern)
        {
            byte[] findme = Encoding.ASCII.GetBytes(pattern);
            //go through buff until we get to findme:
            int i = 0;
            int j = 0;
            while (i < buff.Length)
            {
                if (buff[i] == findme[j])
                {
                    j++;
                    if (j == findme.Length)
                    {
                        return (i - findme.Length) + 1;
                    }
                }
                else j = 0;
                i++;
            }
            return -1;
        }



        string buffToString(int pos, int l)
        {
            byte[] t = new byte[l];
            for (int i = 0; i < l; i++)
            {
                t[i] = buff[pos + i];
            }
            return Encoding.ASCII.GetString(t);
        }




        private void button4_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("explorer.exe", Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + @"\arr\Saved\SaveGames");
        }



        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            showBuggiesOnMap();
        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            showBuggiesOnMap();
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.A && (Control.ModifierKeys & Keys.Control) == Keys.Control)
            {
                for (int i = 0; i < checkedListBox1.Items.Count; i++)
                    checkedListBox1.SetItemChecked(i, true);
            }
            if (e.KeyCode == Keys.D && (Control.ModifierKeys & Keys.Control) == Keys.Control)
            {
                for (int i = 0; i < checkedListBox1.Items.Count; i++)
                    checkedListBox1.SetItemChecked(i, false);
            }

            if (e.KeyCode == Keys.D1) { activeSplineType = 1; showBuggiesOnMap(); }
            if (e.KeyCode == Keys.D2) { activeSplineType = 2; showBuggiesOnMap(); }
            if (e.KeyCode == Keys.D3) { activeSplineType = 3; showBuggiesOnMap(); }
            if (e.KeyCode == Keys.D4) { activeSplineType = 4; showBuggiesOnMap(); }

            if (e.KeyCode == Keys.OemCloseBrackets)
            {
                activeSpline += 1;
                if (activeSpline >= tangentDrawStart.Length) activeSpline = 0;
                showSplineStats();
                showBuggiesOnMap();
            }
            if (e.KeyCode == Keys.OemOpenBrackets)
            {
                activeSpline -= 1;
                if (activeSpline < 0) activeSpline = tangentDrawStart.Length - 1;
                showSplineStats();
                showBuggiesOnMap();
            }

            if (e.KeyCode == Keys.Enter) //re-zoom to the active spline segment
            {
                setNewZoom();
                showBuggiesOnMap();
            }

            if (e.KeyCode == Keys.Up && activeSpline > -1)
            {
                float tLen = 100f;
                if ((Control.ModifierKeys & Keys.Shift) == Keys.Shift) tLen = 10f;
                if ((Control.ModifierKeys & Keys.Control) == Keys.Control) tLen = 1f;
                adjustActiveSplinePoint(0f, tLen, 0f);
            }
            if (e.KeyCode == Keys.Down && activeSpline > -1)
            {
                float tLen = -100f;
                if ((Control.ModifierKeys & Keys.Shift) == Keys.Shift) tLen = -10f;
                if ((Control.ModifierKeys & Keys.Control) == Keys.Control) tLen = -1f;
                adjustActiveSplinePoint(0f,tLen, 0f);
            }
            if (e.KeyCode == Keys.Left && activeSpline > -1)
            {
                float tLen = 100f;
                if ((Control.ModifierKeys & Keys.Shift) == Keys.Shift) tLen = 10f;
                if ((Control.ModifierKeys & Keys.Control) == Keys.Control) tLen = 1f;
                adjustActiveSplinePoint(tLen, 0f, 0f);
            }

            if (e.KeyCode == Keys.Right && activeSpline > -1)
            {
                float tLen = -100f;
                if ((Control.ModifierKeys & Keys.Shift) == Keys.Shift) tLen = -10f;
                if ((Control.ModifierKeys & Keys.Control) == Keys.Control) tLen = -1f;
                adjustActiveSplinePoint(tLen, 0f, 0f);
            }

            ////backspace re-zooms, and we might as well recenter so it actually fits
            //if (e.KeyCode == Keys.Back)
            //{
            //    drawOffsetX = pictureBox1.Width / 2;
            //    drawOffsetY = pictureBox1.Height / 2;
            //    zoomAmount = 1f;
            //    scaleChanged();
            //}
            ////enter recenters the map but doesn't re-zoom
            //if (e.KeyCode == Keys.Enter)
            //{
            //    drawOffsetX = pictureBox1.Width / 2;
            //    drawOffsetY = pictureBox1.Height / 2;
            //    scaleChanged();
            //}

            //float scrollAmt = 10f;
            ////if player moves we want to find the new center offset:
            //if (e.KeyCode == Keys.Right)
            //{
            //    drawOffsetX += scrollAmt * zoomAmount;
            //    if (drawOffsetX > 2000f) drawOffsetX = 2000f;
            //    scaleChanged();
            //}
            //if (e.KeyCode == Keys.Left)
            //{
            //    drawOffsetX -= scrollAmt * zoomAmount;
            //    if (drawOffsetX < -2000f) drawOffsetX = -2000f;
            //    scaleChanged();
            //}
            //if (e.KeyCode == Keys.Down)
            //{
            //    drawOffsetY += scrollAmt * zoomAmount;
            //    if (drawOffsetY > 2000f) drawOffsetY = 2000f;
            //    scaleChanged();
            //}
            //if (e.KeyCode == Keys.Up)
            //{
            //    drawOffsetY -= scrollAmt * zoomAmount;
            //    if (drawOffsetY < -2000f) drawOffsetY = -2000f;
            //    scaleChanged();
            //}

        }

        private void adjustActiveSplinePoint(float x, float y, float z)
        {
            if (activeSplineType == 1)
            {
                trackStart[activeSpline].X += x;
                trackStart[activeSpline].Y += y;
                trackStart[activeSpline].Z += z;
            }
            else if (activeSplineType == 2)
            {
                tangentStart[activeSpline].X += x;
                tangentStart[activeSpline].Y += y;
                tangentStart[activeSpline].Z += z;
            }
            else if (activeSplineType == 3)
            {
                tangentEnd[activeSpline].X += x;
                tangentEnd[activeSpline].Y += y;
                tangentEnd[activeSpline].Z += z;
            }
            else if (activeSplineType == 4)
            {
                trackEnd[activeSpline].X += x;
                trackEnd[activeSpline].Y += y;
                trackEnd[activeSpline].Z += z;
            }
            scaleChanged();
        }


        //given x and y coords, find the left/right/top/bottom and apply them to the zoom
        private void setNewZoom()
        {
            //zoom values are from 0.0 to 1.0
            //given the active spline segment, find the outer extremes out of the four positions:
            float left = trackStart[activeSpline].X;
            if (trackEnd[activeSpline].X < left) left = trackEnd[activeSpline].X;
            if (tangentStart[activeSpline].X < left) left = tangentStart[activeSpline].X;
            if (tangentEnd[activeSpline].X < left) left = tangentEnd[activeSpline].X;
            //left /= 850f;
            zDragStart.X = ((-left + mapMax) / mapMax2);

            float right = trackStart[activeSpline].X;
            if (trackEnd[activeSpline].X > right) right = trackEnd[activeSpline].X;
            if (tangentStart[activeSpline].X > right) right = tangentStart[activeSpline].X;
            if (tangentEnd[activeSpline].X > right) right = tangentEnd[activeSpline].X;
            //right /= 850f;
            zDragEnd.X = ((-right + mapMax) / mapMax2);

            float top = trackStart[activeSpline].Y;
            if (trackEnd[activeSpline].Y < top) top = trackEnd[activeSpline].Y;
            if (tangentStart[activeSpline].Y < top) top = tangentStart[activeSpline].Y;
            if (tangentEnd[activeSpline].Y < top) top = tangentEnd[activeSpline].Y;
            //top /= 850f;
            zDragStart.Y = ((-top + mapMax) / mapMax2);

            float bottom = trackStart[activeSpline].Y;
            if (trackEnd[activeSpline].Y > bottom) bottom = trackEnd[activeSpline].Y;
            if (tangentStart[activeSpline].Y > bottom) bottom = tangentStart[activeSpline].Y;
            if (tangentEnd[activeSpline].Y > bottom) bottom = tangentEnd[activeSpline].Y;
            //bottom /= 850f;
            zDragEnd.Y = ((-bottom + mapMax) / mapMax2);

            //((-framePositionsX[c] + mapMax) / mapMax2)

            zDragStart.X = (zDragStart.X + zDragEnd.X) / 2f; //the starting point is at the center, not at the edge.
            zDragStart.Y = (zDragStart.Y + zDragEnd.Y) / 2f;

            setZoomRectangle();
            scaleChanged();

            ////make this fit comfortably within a zoom region, so the outer edges are actually outside this box
            //zoomLeft = left - ((right - left) / 3f);
            //zoomRight = right + ((right - left) / 3f);
            //zoomTop = top - ((bottom - top) / 3f);
            //zoomBottom = bottom + ((bottom - top) / 3f);
            //scaleChanged(); //apply this and draw
        }


        private void panel2_Paint(object sender, PaintEventArgs e)
        {

        }

        private void checkBox3_CheckedChanged(object sender, EventArgs e)
        {
            showBuggiesOnMap();
        }

        private void checkBox4_CheckedChanged(object sender, EventArgs e)
        {
            showBuggiesOnMap();
        }

        private void checkBox5_CheckedChanged(object sender, EventArgs e)
        {
            showBuggiesOnMap();
        }


        bool zoomDragging = false, movingZoom = false;
        Vector2 zDragStart = new Vector2();
        Vector2 zDragEnd = new Vector2() { X = 1, Y = 1 };
        float zoomTop = 0f, zoomLeft = 0f, zoomRight = 1f, zoomBottom = 1f;


        private void pictureBox2_MouseWheel(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            // Update the drawing based upon the mouse wheel scrolling.

            float z = (float)(e.Delta * SystemInformation.MouseWheelScrollLines / 120);
            float scaleBy;
            if (z > 0) scaleBy = (5f / 6f) / 2f;
            else scaleBy = 0.6f;

            //float offX = e.X

            float axis = (zoomRight - zoomLeft) * scaleBy;
            if (axis > 0.5f) axis = 0.5f;
            if (axis < 0.025f) axis = 0.025f;
            float center = (zoomRight + zoomLeft) / 2f;
            if (center - axis < 0f) center = axis;
            if (center + axis > 1f) center = 1f - axis;
            zoomLeft = center - axis;
            zoomRight = center + axis;

            center = (zoomTop + zoomBottom) / 2f;
            if (center - axis < 0f) center = axis;
            if (center + axis > 1f) center = 1f - axis;
            zoomTop = center - axis;
            zoomBottom = center + axis;

            scaleChanged();
        }
        private void pictureBox2_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                movingZoom = true;
                zoomDragging = false;
                return;
            }

            zoomDragging = true;
            zDragStart.X = e.X / 255f;
            zDragStart.Y = e.Y / 255f;
        }



        private void pictureBox2_MouseMove(object sender, MouseEventArgs e)
        {
            if (movingZoom && e.Button == MouseButtons.Left)
            {
                moveZoomRectangle((e.X / 255f), (e.Y / 255f));
                scaleChanged();
                return;
            }

            if (!zoomDragging) return;

            zDragEnd.X = e.X / 255f;
            zDragEnd.Y = e.Y / 255f;
            setZoomRectangle();

            using (Graphics g = Graphics.FromImage(minimap))
            {
                g.Clear(Color.Black);
                g.DrawImage(Resources.rro_map, 0, 0, 255, 255); //fixme: work out all the scaling crap for these. need to find the rectangle that is the corners of the map that fits in the window at the current zoom and offset.
                g.DrawRectangle(trackPen, zoomLeft * 255f, zoomTop * 255f, (zoomRight - zoomLeft) * 255f, (zoomBottom - zoomTop) * 255f);
            }
            pictureBox2.Image = minimap;
            pictureBox2.Refresh();
        }

        private void checkBoxAddTrees_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBoxAddTrees.Checked)
            {
                checkBoxRemoveTrees.Checked = false;
                checkBox6.Checked = false;
            }
        }
        private void checkBoxRemoveTrees_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBoxRemoveTrees.Checked)
            {
                checkBoxAddTrees.Checked = false;
                checkBox6.Checked = false;
            }
        }

        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            label8.Text = "Radius" + trackBar1.Value.ToString() + "m";
        }

        private void button8_Click(object sender, EventArgs e)
        {
            MessageBox.Show("This might take a minute...", "Possible Long Operation");
            clearTreesFromTracks();
            showBuggiesOnMap();
        }

        private void button7_Click(object sender, EventArgs e)
        {
            //add all the trees back in...
            for (int i = 0; i < treeExists.Length; i++) treeExists[i] = true;
            showBuggiesOnMap();
        }

        private void button6_Click(object sender, EventArgs e)
        {

        }

        private void button9_Click(object sender, EventArgs e)
        {
            //add all the trees back in...
            for (int i = 0; i < treeExists.Length; i++) treeExists[i] = false;
            showBuggiesOnMap();
        }

        private void button10_Click(object sender, EventArgs e)
        {
            if( MessageBox.Show("This will thin out the trees by cutting down a percentage of them across the entire map. The higher the radius you select, the more trees will be deleted. Proceed?","Thin Trees",MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                Random r = new Random();
                //get the fraction of trees to keep:
                float per = ((float)trackBar1.Value - 2.5f) / 100f; //gives a number from .025 to .975
                for (int i = 0; i < treeExists.Length; i++)
                    if (treeExists[i] && r.NextSingle() < per)  treeExists[i] = false;
                showBuggiesOnMap();
            }
        }

        private void checkBox6_CheckedChanged(object sender, EventArgs e)
        {
            splineDragging = false;
            checkBoxAddTrack.Checked = false;
            checkBoxRemoveTrack.Checked = false;
            panelTracks.Visible = !panelTracks.Visible; //when this is visible we are in track editing mode
            if (checkBox6.Checked)
            {
                checkBoxAddTrees.Checked = false;
                checkBoxRemoveTrees.Checked = false;
            }
            activeSpline = -1;
            activeSplineType = -1;
        }

        private void button5_Click(object sender, EventArgs e)
        {
            button1_Click(sender, e);
        }

        void setZoomRectangle()
        {
            //we want to make sure it's a square so it fills the whole drawing area. 
            //dragging starts at the center and outward through a radius
            //Find the largest axis and increase the smallest axis to match it:
            float axis = Math.Max(Math.Abs(zDragStart.X - zDragEnd.X), Math.Abs(zDragStart.Y - zDragEnd.Y));
            if (axis < 0.025f) axis = 0.025f; //limit zoom level 
            if (axis > 0.5f) axis = 0.5f;

            //rebuild the center and keep it in bounds
            float center = zDragStart.X;
            if (center - axis < 0f) center = axis;
            if (center + axis > 1f) center = 1f - axis;
            zoomRight = center + axis;
            zoomLeft = center - axis;

            center = zDragStart.Y;
            if (center - axis < 0f) center = axis;
            if (center + axis > 1f) center = 1f - axis;
            zoomBottom = center + axis;
            zoomTop = center - axis;

        }


        void moveZoomRectangle(float x, float y)
        {
            //keep the same scale but move the zoom area:
            float axis = (zoomRight - zoomLeft) / 2f;

            //rebuild the center and keep it in bounds
            float center = x;
            if (center - axis < 0f) center = axis;
            if (center + axis > 1f) center = 1f - axis;
            zoomRight = center + axis;
            zoomLeft = center - axis;

            center = y;
            if (center - axis < 0f) center = axis;
            if (center + axis > 1f) center = 1f - axis;
            zoomBottom = center + axis;
            zoomTop = center - axis;

        }
        private void pictureBox2_MouseUp(object sender, MouseEventArgs e)
        {
            if (movingZoom && e.Button == MouseButtons.Left)
            {
                moveZoomRectangle((e.X / 255f), (e.Y / 255f));
                scaleChanged();
                movingZoom = false;
                zoomDragging = false;
                return;
            }

            if (!zoomDragging) return;

            zoomDragging = false;
            zDragEnd.X = (float)e.X / 255f;
            //if (zDragEnd.X < 0f) zDragEnd.X = 0f;
            zDragEnd.Y = (float)e.Y / 255f;
            //if (zDragEnd.Y < 0f) zDragEnd.Y = 0f;
            setZoomRectangle();
            scaleChanged();

        }



        private void button3_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Selection Shortcuts:\nCtrl-A = Select All\nCtrl-D = Select None\nClick on Map = Select Nearest\nRMB on Map = Deselect Nearest\nShift+Drag on Map = Select all within rectangle\nShift+RMB Drag on Map = Deselect all within rectangle");
        }




        void setHeaders()
        {
            FrameLocationHeader = convertHeader("13 00 00 00 46 72 61 6D 65 4C 6F 63 61 74 69 6F 6E 41 72 72 61 79 00 0E 00 00 00 41 72 72 61 79 50 72 6F 70 65 72 74 79 00 26 02 00 00 00 00 00 00 0F 00 00 00 53 74 72 75 63 74 50 72 6F 70 65 72 74 79 00 00 27 00 00 00 13 00 00 00 46 72 61 6D 65 4C 6F 63 61 74 69 6F 6E 41 72 72 61 79 00 0F 00 00 00 53 74 72 75 63 74 50 72 6F 70 65 72 74 79 00 D4 01 00 00 00 00 00 00 07 00 00 00 56 65 63 74 6F 72 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00");
            FrameRotationHeader = convertHeader("13 00 00 00 46 72 61 6D 65 52 6F 74 61 74 69 6F 6E 41 72 72 61 79 00 0E 00 00 00 41 72 72 61 79 50 72 6F 70 65 72 74 79 00 27 02 00 00 00 00 00 00 0F 00 00 00 53 74 72 75 63 74 50 72 6F 70 65 72 74 79 00 00 27 00 00 00 13 00 00 00 46 72 61 6D 65 52 6F 74 61 74 69 6F 6E 41 72 72 61 79 00 0F 00 00 00 53 74 72 75 63 74 50 72 6F 70 65 72 74 79 00 D4 01 00 00 00 00 00 00 08 00 00 00 52 6F 74 61 74 6F 72 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00");
            FrameTypeHeader = convertHeader("0F 00 00 00 46 72 61 6D 65 54 79 70 65 41 72 72 61 79 00 0E 00 00 00 41 72 72 61 79 50 72 6F 70 65 72 74 79 00 B7 02 00 00 00 00 00 00 0C 00 00 00 53 74 72 50 72 6F 70 65 72 74 79 00 00");
            FrameNumberHeader = convertHeader("11 00 00 00 46 72 61 6D 65 4E 75 6D 62 65 72 41 72 72 61 79 00 0E 00 00 00 41 72 72 61 79 50 72 6F 70 65 72 74 79 00 9A 01 00 00 00 00 00 00 0D 00 00 00 54 65 78 74 50 72 6F 70 65 72 74 79 00 00");
            FrameNameHeader = convertHeader("0F 00 00 00 46 72 61 6D 65 4E 61 6D 65 41 72 72 61 79 00 0E 00 00 00 41 72 72 61 79 50 72 6F 70 65 72 74 79 00 18 02 00 00 00 00 00 00 0D 00 00 00 54 65 78 74 50 72 6F 70 65 72 74 79 00 00");
            SmokestackTypeHeader = convertHeader("14 00 00 00 53 6D 6F 6B 65 73 74 61 63 6B 54 79 70 65 41 72 72 61 79 00 0E 00 00 00 41 72 72 61 79 50 72 6F 70 65 72 74 79 00 A0 00 00 00 00 00 00 00 0C 00 00 00 49 6E 74 50 72 6F 70 65 72 74 79 00 00");
            HeadlightTypeHeader = convertHeader("13 00 00 00 48 65 61 64 6C 69 67 68 74 54 79 70 65 41 72 72 61 79 00 0E 00 00 00 41 72 72 61 79 50 72 6F 70 65 72 74 79 00 A0 00 00 00 00 00 00 00 0C 00 00 00 49 6E 74 50 72 6F 70 65 72 74 79 00 00");
            PaintTypeHeader = convertHeader("0F 00 00 00 50 61 69 6E 74 54 79 70 65 41 72 72 61 79 00 0E 00 00 00 41 72 72 61 79 50 72 6F 70 65 72 74 79 00 A0 00 00 00 00 00 00 00 0C 00 00 00 49 6E 74 50 72 6F 70 65 72 74 79 00 00");
            BoilerFuelAmountHeader = convertHeader("16 00 00 00 42 6F 69 6C 65 72 46 75 65 6C 41 6D 6F 75 6E 74 41 72 72 61 79 00 0E 00 00 00 41 72 72 61 79 50 72 6F 70 65 72 74 79 00 A0 00 00 00 00 00 00 00 0E 00 00 00 46 6C 6F 61 74 50 72 6F 70 65 72 74 79 00 00");
            BoilerFireTempHeader = convertHeader("14 00 00 00 42 6F 69 6C 65 72 46 69 72 65 54 65 6D 70 41 72 72 61 79 00 0E 00 00 00 41 72 72 61 79 50 72 6F 70 65 72 74 79 00 A0 00 00 00 00 00 00 00 0E 00 00 00 46 6C 6F 61 74 50 72 6F 70 65 72 74 79 00 00");
            BoilerWaterTempHeader = convertHeader("15 00 00 00 42 6F 69 6C 65 72 57 61 74 65 72 54 65 6D 70 41 72 72 61 79 00 0E 00 00 00 41 72 72 61 79 50 72 6F 70 65 72 74 79 00 A0 00 00 00 00 00 00 00 0E 00 00 00 46 6C 6F 61 74 50 72 6F 70 65 72 74 79 00 00");
            BoilerWaterLevelHeader = convertHeader("16 00 00 00 42 6F 69 6C 65 72 57 61 74 65 72 4C 65 76 65 6C 41 72 72 61 79 00 0E 00 00 00 41 72 72 61 79 50 72 6F 70 65 72 74 79 00 A0 00 00 00 00 00 00 00 0E 00 00 00 46 6C 6F 61 74 50 72 6F 70 65 72 74 79 00 00");
            BoilerPressureHeader = convertHeader("14 00 00 00 42 6F 69 6C 65 72 50 72 65 73 73 75 72 65 41 72 72 61 79 00 0E 00 00 00 41 72 72 61 79 50 72 6F 70 65 72 74 79 00 A0 00 00 00 00 00 00 00 0E 00 00 00 46 6C 6F 61 74 50 72 6F 70 65 72 74 79 00 00");
            HeadlightFrontStateHeader = convertHeader("19 00 00 00 48 65 61 64 6C 69 67 68 74 46 72 6F 6E 74 53 74 61 74 65 41 72 72 61 79 00 0E 00 00 00 41 72 72 61 79 50 72 6F 70 65 72 74 79 00 2B 00 00 00 00 00 00 00 0D 00 00 00 42 6F 6F 6C 50 72 6F 70 65 72 74 79 00 00");
            HeadlightRearStateHeader = convertHeader("18 00 00 00 48 65 61 64 6C 69 67 68 74 52 65 61 72 53 74 61 74 65 41 72 72 61 79 00 0E 00 00 00 41 72 72 61 79 50 72 6F 70 65 72 74 79 00 2B 00 00 00 00 00 00 00 0D 00 00 00 42 6F 6F 6C 50 72 6F 70 65 72 74 79 00 00");
            CouplerFrontStateHeader = convertHeader("17 00 00 00 43 6F 75 70 6C 65 72 46 72 6F 6E 74 53 74 61 74 65 41 72 72 61 79 00 0E 00 00 00 41 72 72 61 79 50 72 6F 70 65 72 74 79 00 2B 00 00 00 00 00 00 00 0D 00 00 00 42 6F 6F 6C 50 72 6F 70 65 72 74 79 00 00");
            CouplerRearStateHeader = convertHeader("16 00 00 00 43 6F 75 70 6C 65 72 52 65 61 72 53 74 61 74 65 41 72 72 61 79 00 0E 00 00 00 41 72 72 61 79 50 72 6F 70 65 72 74 79 00 2B 00 00 00 00 00 00 00 0D 00 00 00 42 6F 6F 6C 50 72 6F 70 65 72 74 79 00 00");
            TenderFuelAmountHeader = convertHeader("16 00 00 00 54 65 6E 64 65 72 46 75 65 6C 41 6D 6F 75 6E 74 41 72 72 61 79 00 0E 00 00 00 41 72 72 61 79 50 72 6F 70 65 72 74 79 00 A0 00 00 00 00 00 00 00 0E 00 00 00 46 6C 6F 61 74 50 72 6F 70 65 72 74 79 00 00");
            TenderWaterAmountHeader = convertHeader("17 00 00 00 54 65 6E 64 65 72 57 61 74 65 72 41 6D 6F 75 6E 74 41 72 72 61 79 00 0E 00 00 00 41 72 72 61 79 50 72 6F 70 65 72 74 79 00 A0 00 00 00 00 00 00 00 0E 00 00 00 46 6C 6F 61 74 50 72 6F 70 65 72 74 79 00 00");
            CompressorAirPressureHeader = convertHeader("1B 00 00 00 43 6F 6D 70 72 65 73 73 6F 72 41 69 72 50 72 65 73 73 75 72 65 41 72 72 61 79 00 0E 00 00 00 41 72 72 61 79 50 72 6F 70 65 72 74 79 00 A0 00 00 00 00 00 00 00 0E 00 00 00 46 6C 6F 61 74 50 72 6F 70 65 72 74 79 00 00");
            MarkerLightsFrontRightStateHeader = convertHeader("21 00 00 00 4D 61 72 6B 65 72 4C 69 67 68 74 73 46 72 6F 6E 74 52 69 67 68 74 53 74 61 74 65 41 72 72 61 79 00 0E 00 00 00 41 72 72 61 79 50 72 6F 70 65 72 74 79 00 A0 00 00 00 00 00 00 00 0C 00 00 00 49 6E 74 50 72 6F 70 65 72 74 79 00 00");
            MarkerLightsFrontLeftStateHeader = convertHeader("20 00 00 00 4D 61 72 6B 65 72 4C 69 67 68 74 73 46 72 6F 6E 74 4C 65 66 74 53 74 61 74 65 41 72 72 61 79 00 0E 00 00 00 41 72 72 61 79 50 72 6F 70 65 72 74 79 00 A0 00 00 00 00 00 00 00 0C 00 00 00 49 6E 74 50 72 6F 70 65 72 74 79 00 00");
            MarkerLightsRearRightStateHeader = convertHeader("20 00 00 00 4D 61 72 6B 65 72 4C 69 67 68 74 73 52 65 61 72 52 69 67 68 74 53 74 61 74 65 41 72 72 61 79 00 0E 00 00 00 41 72 72 61 79 50 72 6F 70 65 72 74 79 00 A0 00 00 00 00 00 00 00 0C 00 00 00 49 6E 74 50 72 6F 70 65 72 74 79 00 00");
            MarkerLightsRearLeftStateHeader = convertHeader("1F 00 00 00 4D 61 72 6B 65 72 4C 69 67 68 74 73 52 65 61 72 4C 65 66 74 53 74 61 74 65 41 72 72 61 79 00 0E 00 00 00 41 72 72 61 79 50 72 6F 70 65 72 74 79 00 A0 00 00 00 00 00 00 00 0C 00 00 00 49 6E 74 50 72 6F 70 65 72 74 79 00 00");
            MarkerLightsCenterStateHeader = convertHeader("1D 00 00 00 4D 61 72 6B 65 72 4C 69 67 68 74 73 43 65 6E 74 65 72 53 74 61 74 65 41 72 72 61 79 00 0E 00 00 00 41 72 72 61 79 50 72 6F 70 65 72 74 79 00 A0 00 00 00 00 00 00 00 0C 00 00 00 49 6E 74 50 72 6F 70 65 72 74 79 00 00");
            FreightTypeHeader = convertHeader("11 00 00 00 46 72 65 69 67 68 74 54 79 70 65 41 72 72 61 79 00 0E 00 00 00 41 72 72 61 79 50 72 6F 70 65 72 74 79 00 54 01 00 00 00 00 00 00 0C 00 00 00 53 74 72 50 72 6F 70 65 72 74 79 00 00");
            FreightAmountHeader = convertHeader("13 00 00 00 46 72 65 69 67 68 74 41 6D 6F 75 6E 74 41 72 72 61 79 00 0E 00 00 00 41 72 72 61 79 50 72 6F 70 65 72 74 79 00 A0 00 00 00 00 00 00 00 0C 00 00 00 49 6E 74 50 72 6F 70 65 72 74 79 00 00");
            RegulatorValueHeader = convertHeader("14 00 00 00 52 65 67 75 6C 61 74 6F 72 56 61 6C 75 65 41 72 72 61 79 00 0E 00 00 00 41 72 72 61 79 50 72 6F 70 65 72 74 79 00 A0 00 00 00 00 00 00 00 0E 00 00 00 46 6C 6F 61 74 50 72 6F 70 65 72 74 79 00 00");
            BrakeValueHeader = convertHeader("10 00 00 00 42 72 61 6B 65 56 61 6C 75 65 41 72 72 61 79 00 0E 00 00 00 41 72 72 61 79 50 72 6F 70 65 72 74 79 00 A0 00 00 00 00 00 00 00 0E 00 00 00 46 6C 6F 61 74 50 72 6F 70 65 72 74 79 00 00");
            GeneratorValveValueHeader = convertHeader("19 00 00 00 47 65 6E 65 72 61 74 6F 72 56 61 6C 76 65 56 61 6C 75 65 41 72 72 61 79 00 0E 00 00 00 41 72 72 61 79 50 72 6F 70 65 72 74 79 00 A0 00 00 00 00 00 00 00 0E 00 00 00 46 6C 6F 61 74 50 72 6F 70 65 72 74 79 00 00");
            CompressorValveValueHeader = convertHeader("1A 00 00 00 43 6F 6D 70 72 65 73 73 6F 72 56 61 6C 76 65 56 61 6C 75 65 41 72 72 61 79 00 0E 00 00 00 41 72 72 61 79 50 72 6F 70 65 72 74 79 00 A0 00 00 00 00 00 00 00 0E 00 00 00 46 6C 6F 61 74 50 72 6F 70 65 72 74 79 00 00");
            ReverserValueHeader = convertHeader("13 00 00 00 52 65 76 65 72 73 65 72 56 61 6C 75 65 41 72 72 61 79 00 0E 00 00 00 41 72 72 61 79 50 72 6F 70 65 72 74 79 00 A0 00 00 00 00 00 00 00 0E 00 00 00 46 6C 6F 61 74 50 72 6F 70 65 72 74 79 00 00");
            SanderAmountHeader = convertHeader("12 00 00 00 53 61 6E 64 65 72 41 6D 6F 75 6E 74 41 72 72 61 79 00 0E 00 00 00 41 72 72 61 79 50 72 6F 70 65 72 74 79 00 A0 00 00 00 00 00 00 00 0E 00 00 00 46 6C 6F 61 74 50 72 6F 70 65 72 74 79 00 00");

            SplineTypeTrackHeader = convertHeader("15 00 00 00 53 70 6C 69 6E 65 54 72 61 63 6B 54 79 70 65 41 72 72 61 79 00 0E 00 00 00 41 72 72 61 79 50 72 6F 70 65 72 74 79 00 D9 01 00 00 00 00 00 00 0C 00 00 00 53 74 72 50 72 6F 70 65 72 74 79 00 00");
            SplineTrackLocationHeader = convertHeader("19 00 00 00 53 70 6C 69 6E 65 54 72 61 63 6B 4C 6F 63 61 74 69 6F 6E 41 72 72 61 79 00 0E 00 00 00 41 72 72 61 79 50 72 6F 70 65 72 74 79 00 54 01 00 00 00 00 00 00 0F 00 00 00 53 74 72 75 63 74 50 72 6F 70 65 72 74 79 00 00 15 00 00 00 19 00 00 00 53 70 6C 69 6E 65 54 72 61 63 6B 4C 6F 63 61 74 69 6F 6E 41 72 72 61 79 00 0F 00 00 00 53 74 72 75 63 74 50 72 6F 70 65 72 74 79 00 FC 00 00 00 00 00 00 00 07 00 00 00 56 65 63 74 6F 72 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00");
            SplineTrackRotationHeader = convertHeader("19 00 00 00 53 70 6C 69 6E 65 54 72 61 63 6B 52 6F 74 61 74 69 6F 6E 41 72 72 61 79 00 0E 00 00 00 41 72 72 61 79 50 72 6F 70 65 72 74 79 00 55 01 00 00 00 00 00 00 0F 00 00 00 53 74 72 75 63 74 50 72 6F 70 65 72 74 79 00 00 15 00 00 00 19 00 00 00 53 70 6C 69 6E 65 54 72 61 63 6B 52 6F 74 61 74 69 6F 6E 41 72 72 61 79 00 0F 00 00 00 53 74 72 75 63 74 50 72 6F 70 65 72 74 79 00 FC 00 00 00 00 00 00 00 08 00 00 00 52 6F 74 61 74 6F 72 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00");
            SplineTrackStartPointHeader = convertHeader("1B 00 00 00 53 70 6C 69 6E 65 54 72 61 63 6B 53 74 61 72 74 50 6F 69 6E 74 41 72 72 61 79 00 0E 00 00 00 41 72 72 61 79 50 72 6F 70 65 72 74 79 00 56 01 00 00 00 00 00 00 0F 00 00 00 53 74 72 75 63 74 50 72 6F 70 65 72 74 79 00 00 15 00 00 00 1B 00 00 00 53 70 6C 69 6E 65 54 72 61 63 6B 53 74 61 72 74 50 6F 69 6E 74 41 72 72 61 79 00 0F 00 00 00 53 74 72 75 63 74 50 72 6F 70 65 72 74 79 00 FC 00 00 00 00 00 00 00 07 00 00 00 56 65 63 74 6F 72 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00");
            SplineTrackEndPointHeader = convertHeader("19 00 00 00 53 70 6C 69 6E 65 54 72 61 63 6B 45 6E 64 50 6F 69 6E 74 41 72 72 61 79 00 0E 00 00 00 41 72 72 61 79 50 72 6F 70 65 72 74 79 00 54 01 00 00 00 00 00 00 0F 00 00 00 53 74 72 75 63 74 50 72 6F 70 65 72 74 79 00 00 15 00 00 00 19 00 00 00 53 70 6C 69 6E 65 54 72 61 63 6B 45 6E 64 50 6F 69 6E 74 41 72 72 61 79 00 0F 00 00 00 53 74 72 75 63 74 50 72 6F 70 65 72 74 79 00 FC 00 00 00 00 00 00 00 07 00 00 00 56 65 63 74 6F 72 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00");
            SplineTrackStartTangentHeader = convertHeader("1D 00 00 00 53 70 6C 69 6E 65 54 72 61 63 6B 53 74 61 72 74 54 61 6E 67 65 6E 74 41 72 72 61 79 00 0E 00 00 00 41 72 72 61 79 50 72 6F 70 65 72 74 79 00 58 01 00 00 00 00 00 00 0F 00 00 00 53 74 72 75 63 74 50 72 6F 70 65 72 74 79 00 00 15 00 00 00 1D 00 00 00 53 70 6C 69 6E 65 54 72 61 63 6B 53 74 61 72 74 54 61 6E 67 65 6E 74 41 72 72 61 79 00 0F 00 00 00 53 74 72 75 63 74 50 72 6F 70 65 72 74 79 00 FC 00 00 00 00 00 00 00 07 00 00 00 56 65 63 74 6F 72 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00");
            SplineTrackEndTangentHeader = convertHeader("1B 00 00 00 53 70 6C 69 6E 65 54 72 61 63 6B 45 6E 64 54 61 6E 67 65 6E 74 41 72 72 61 79 00 0E 00 00 00 41 72 72 61 79 50 72 6F 70 65 72 74 79 00 56 01 00 00 00 00 00 00 0F 00 00 00 53 74 72 75 63 74 50 72 6F 70 65 72 74 79 00 00 15 00 00 00 1B 00 00 00 53 70 6C 69 6E 65 54 72 61 63 6B 45 6E 64 54 61 6E 67 65 6E 74 41 72 72 61 79 00 0F 00 00 00 53 74 72 75 63 74 50 72 6F 70 65 72 74 79 00 FC 00 00 00 00 00 00 00 07 00 00 00 56 65 63 74 6F 72 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00");
            SplineTrackSwitchStateHeader = convertHeader("1C 00 00 00 53 70 6C 69 6E 65 54 72 61 63 6B 53 77 69 74 63 68 53 74 61 74 65 41 72 72 61 79 00 0E 00 00 00 41 72 72 61 79 50 72 6F 70 65 72 74 79 00 58 00 00 00 00 00 00 00 0C 00 00 00 49 6E 74 50 72 6F 70 65 72 74 79 00 00 15 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 01 00 00 00 00 00 00 00 01 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00");
            SplineTrackPaintStyleHeader = convertHeader("1B 00 00 00 53 70 6C 69 6E 65 54 72 61 63 6B 50 61 69 6E 74 53 74 79 6C 65 41 72 72 61 79 00 0E 00 00 00 41 72 72 61 79 50 72 6F 70 65 72 74 79 00 58 00 00 00 00 00 00 00 0C 00 00 00 49 6E 74 50 72 6F 70 65 72 74 79 00 00");
        }


        byte[][] getIntArray(byte[] header, int ign, bool silent = false, bool debug = false)
        {
            byte[][] arrTemp = new byte[numSourceCars][];
            //find location of trigger text in file, jump ahead by offset, gather each int
            int found = findBytesInBytes(header, ign);
            if (found == -1)
            {
                if (!silent)
                {
                    MessageBox.Show("Unable to parse this save file. Make sure you dropped a valid save file here.");
                    this.Text = "Invalid File - Try Again";
                    checkedListBox1.Items.Clear();
                }
                return null; //new byte[0][];//an empty array is a problem reading the file and should stop the attempt to read
            }

            //need to construct an array of byte arrays. One byte array (Int32) for each car.
            int offset = header.Length + 4; //do we need an offset from the found position?

            int pos = found + offset;
            for (int i = 0; i < numSourceCars; i++)
            { //for each car, check to see if there is a name or not
                arrTemp[i] = new byte[4];
                for (int j = 0; j < 4; j++)
                    arrTemp[i][j] = buff[pos + j];
                //if (debug) Debug.WriteLine("Checking @ " + pos.ToString() + " = " + BitConverter.ToSingle(arrTemp[i]));
                pos += 4;
            }
            return arrTemp;
        }


        byte[][] getVectorArray(byte[] header, int offset)
        {
            int ign = header.Length + 4;//just set ign to greater than the length... we're not searching for the entire header this time
            byte[][] arrTemp = new byte[numSourceCars][];
            //find location of trigger text in file, jump ahead by offset, gather each int
            int found = findBytesInBytes(header, ign);
            if (found == -1)
            {
                MessageBox.Show("Unable to parse this save file. Make sure you dropped a valid save file here.");
                this.Text = "Invalid File - Try Again";
                checkedListBox1.Items.Clear();
                return null; //new byte[0][];//an empty array is a problem reading the file and should stop the attempt to read
            }

            int pos = found + offset; //NOTE: the 147 is a hardcoded offset from the header start to the data start
            for (int i = 0; i < numSourceCars; i++)
            { //for each car, check to see if there is a name or not
                arrTemp[i] = new byte[12];
                for (int j = 0; j < 12; j++)
                    arrTemp[i][j] = buff[pos + j];
                //if (debug) Debug.WriteLine("Checking @ " + pos.ToString() + " = " + BitConverter.ToSingle(arrTemp[i]));
                pos += 12;
            }
            return arrTemp;
        }

        byte[] getBoolArray(byte[] header, int ign, bool debug = false)
        {
            byte[] arrTemp = new byte[numSourceCars];
            //find location of trigger text in file, jump ahead by offset, gather each int
            int found = findBytesInBytes(header, ign);
            if (found == -1)
            {
                //MessageBox.Show("Unable to parse this save file. Make sure you dropped a valid save file here.");
                this.Text = "Invalid File - Try Again";
                checkedListBox1.Items.Clear();
                return null; //new byte[0][];//an empty array is a problem reading the file and should stop the attempt to read
            }

            //need to construct an array. One byte for each car. 
            int offset = header.Length + 4; //do we need an offset from the found position?

            int pos = found + offset;
            for (int i = 0; i < numSourceCars; i++)
            { //for each car, check to see if there is a name or not
                arrTemp[i] = buff[pos];
                //if (debug) Debug.WriteLine("Checking @ " + pos.ToString() + " = " + (arrTemp[i] == 1).ToString());
                pos++;
            }
            return arrTemp;
        }


        //Get a string array, note we don't need to have actual strings, just the bytes to be put back into the file
        //IF STRING PRESENT: 02 00 00 00 FF 01 00 00 00 (len + 1) 00 00 00 (stringchars 00)
        //IF NO STRING:      00 00 00 00 FF 00 00 00 00 
        //IF Special string: 01 00 00 00 03 08 00 00 00 00 00 00 00 00 21 00 00 00 35 36 46 38 44 32 37 31 34 39 43 43 35 45 32 44 31 32 31 30 33 42 42 45 42 46 43 41 39 30 39 37 00 0B 00 00 00 7B 30 7D 3C 62 72 3E 7B 31 7D 00 02 00 00 00 02 00 00 00 30 00 04 02 00 00 00 FF
        byte[][] getStringArray(byte[] header, int ign)
        {
            byte[][] arrTemp = new byte[numSourceCars][];
            //find location of trigger text in file, jump ahead by offset, gather each string
            int found = findBytesInBytes(header, ign);
            if (found == -1)
            {
                //MessageBox.Show("Unable to parse this save file. Make sure you dropped a valid save file here.");
                this.Text = "Invalid File - Try Again";
                checkedListBox1.Items.Clear();
                return null; //new byte[0][];//an empty array is a problem reading the file and should stop the attempt to read
            }

            //need to construct an array of byte arrays. One byte array ("string") for each car.
            int offset = header.Length + 4; //do we need an offset from the found position?

            int pos = found + offset;
            for (int i = 0; i < numSourceCars; i++)
            { //for each car, check to see if there is a name or not
                //Debug.WriteLine("Checking @ " + pos.ToString());
                int strlen = Convert.ToInt32(buff[pos]);
                if (strlen == 0)
                {
                    arrTemp[i] = new byte[] { 0x00, 0x00, 0x00, 0x00, 0xFF, 0x00, 0x00, 0x00, 0x00 };
                    pos += 9;
                }
                else //this one has a string associated with it
                {
                    // check for a specific type of corrupted data that came with new saves in the beta:
                    if (Convert.ToInt32(buff[pos]) == 1)
                    {
                        //MessageBox.Show("Note: Possible Name corruption (buggy number " + i.ToString() + " of " + numSourceCars.ToString() + ") detected in save. If this operation fails you should load a copy of this save in ROSE to rename the buggies.\nLook for one that might have a <br> or other odd characters in it, or count in the list up from zero to the number supplied earlier.");
                        //This is whree we have detected a special string that requires more processing to deal with:
                        //check to see if it matches our special header block:
                        byte[] specialHeader = new byte[] { 0x01, 0x00, 0x00, 0x00, 0x03, 0x08, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x21, 0x00, 0x00, 0x00, 0x35, 0x36, 0x46, 0x38, 0x44, 0x32, 0x37, 0x31, 0x34, 0x39, 0x43, 0x43, 0x35, 0x45, 0x32, 0x44, 0x31, 0x32, 0x31, 0x30, 0x33, 0x42, 0x42, 0x45, 0x42, 0x46, 0x43, 0x41, 0x39, 0x30, 0x39, 0x37, 0x00, 0x0B, 0x00, 0x00, 0x00, 0x7B, 0x30, 0x7D, 0x3C, 0x62, 0x72, 0x3E, 0x7B, 0x31, 0x7D, 0x00, 0x02, 0x00, 0x00, 0x00, 0x02, 0x00, 0x00, 0x00, 0x30, 0x00, 0x04, 0x02, 0x00, 0x00, 0x00, 0xFF };
                        byte[] specialTrailer = new byte[] { 0x02, 0x00, 0x00, 0x00, 0x31, 0x00, 0x04, 0x02, 0x00, 0x00, 0x00, 0xFF, 0x00, 0x00, 0x00, 0x00 };
                        int m = 0;
                        for (int k = pos; k < buff.Length && m<specialHeader.Length; k++)
                        {
                            if (buff[k] != specialHeader[m])
                            {
                                MessageBox.Show("Failed to parse Rolling Stock Name. If this operation fails you can try loading a copy of this save in ROSE to rename the buggies.");
                                break;
                            }
                            m++;
                        }
                        //if the header is correct, we can get the data and stuff it into a byte array as usual
                        //but first we need toi detect a zero-length string
                        int detect = Convert.ToInt32(buff[pos + specialHeader.Length]);
                        if (detect == 1)
                        {
                            int tlen = Convert.ToInt32(buff[pos + specialHeader.Length + 4]); //length of the string and trailer
                            Debug.WriteLine("Tlen= " + tlen);
                            arrTemp[i] = new byte[specialHeader.Length + 8 + tlen + specialTrailer.Length]; //the 4 is the length of the 4 digits used to store the length value
                            for (int j = 0; j < arrTemp[i].Length; j++)
                                arrTemp[i][j] = buff[pos + j];
                            pos += arrTemp[i].Length;
                        }//otherwise it's a zero-length, has to be handled differently
                        else
                        {
                            Debug.WriteLine("BLANK ");
                            arrTemp[i] = new byte[specialHeader.Length + 4 + specialTrailer.Length]; //the 4 is the length of the 4 digits used to store the length value
                            for (int j = 0; j < arrTemp[i].Length; j++)
                                arrTemp[i][j] = buff[pos + j];
                            pos += arrTemp[i].Length;
                        }

                        //find the next instance of "FF" and then back up 4 spaces:
                        //for (int k = pos; k < buff.Length; k++)
                        //{
                        //    if (buff[k] == 255)
                        //    {
                        //        pos = k - 4;
                        //        break;
                        //    }
                        //}
                        //create a blank name for this one
                        //arrTemp[i] = new byte[] { 0x00, 0x00, 0x00, 0x00, 0xFF, 0x00, 0x00, 0x00, 0x00 };
                    }
                    else
                    {
                        //get the length of the string and all:
                        int tlen = Convert.ToInt32(buff[pos + 9]); //length of the string and trailer
                        arrTemp[i] = new byte[13 + tlen];
                        for (int j = 0; j < 13 + tlen; j++)
                            arrTemp[i][j] = buff[pos + j];
                        pos += 13 + tlen;
                    }


                }

                //Debug.WriteLine(Encoding.ASCII.GetString(arrTemp[i]));



            }
            return arrTemp;
        }


        byte[][] getCargoArray(byte[] header, int ign)
        {
            byte[][] arrTemp = new byte[numSourceCars][];
            //find location of trigger text in file, jump ahead by offset, gather each string
            int found = findBytesInBytes(header, ign);
            if (found == -1)
            {
                //MessageBox.Show("Unable to parse this save file. Make sure you dropped a valid save file here.");
                this.Text = "Invalid File - Try Again";
                checkedListBox1.Items.Clear();
                return null; //new byte[0][];//an empty array is a problem reading the file and should stop the attempt to read
            }

            //need to construct an array of byte arrays. One byte array ("string") for each car.
            int offset = header.Length + 4; //do we need an offset from the found position?

            int pos = found + offset;
            for (int i = 0; i < numSourceCars; i++)
            { //for each car, check to see if there is a name or not
              //Debug.WriteLine("Checking @ " + pos.ToString());
                int strlen = Convert.ToInt32(buff[pos]);
                if (strlen == 0)
                {
                    arrTemp[i] = new byte[] { 0x00, 0x00, 0x00, 0x00 };
                    pos += 4;
                }
                else //this one has a string associated with it
                {
                    //get the length of the string and all:
                    int tlen = Convert.ToInt32(buff[pos]); //length of the string and trailer
                    arrTemp[i] = new byte[4 + tlen];
                    for (int j = 0; j < 4 + tlen; j++)
                        arrTemp[i][j] = buff[pos + j];
                    pos += 4 + tlen;
                }

                //Debug.WriteLine(Encoding.ASCII.GetString(arrTemp[i]));



            }
            return arrTemp;
        }


        //finds a string of bytes wtthin the file's bytes
        //we need to ignore the one section (4 bytes) in the header where it stores the length, because this will vary from file to file
        int findBytesInBytes(byte[] findme, int lengthPos)
        {
            int test = 0;
            //go through buff until we get to findme:
            int i = 0;
            int j = 0;
            while (i < buff.Length)
            {
                if (j >= lengthPos && j < lengthPos + 4)
                {
                    j++; //don't test these 4 bytes because they are different in each save
                }
                else
                {
                    if (buff[i] == findme[j])
                    {
                        j++;
                        if (j == findme.Length)
                        {
                            return (i - findme.Length) + 1;
                        }
                    }
                    else
                        j = 0;
                }

                i++;
            }
            return -1;
        }

        private void label1_Click(object sender, EventArgs e)
        {
            DialogResult result = openFileDialog1.ShowDialog(this);
            if (result == DialogResult.OK)
            {
                parseSaveNow(openFileDialog1.FileName);
                label1.Text = Path.GetFileName(openFileDialog1.FileName);
            }
        }

        string mapFilePath = "";
        private void label2_Click(object sender, EventArgs e)
        {
            DialogResult result = openFileDialog1.ShowDialog(this);
            if (result == DialogResult.OK)
            {
                mapFilePath = openFileDialog1.FileName;
                label2.Text = Path.GetFileName(mapFilePath);
                parseTargetSaveNow(mapFilePath);
            }
        }
    }
}