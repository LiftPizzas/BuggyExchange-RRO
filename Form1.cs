using BuggyExchange.Properties;
using Microsoft.VisualBasic.Devices;
using System.Diagnostics;
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
            dragging = 0;
        }

        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            if (treeDragging)
            {
                if (checkBoxAddTrees.Checked || checkBoxRemoveTrees.Checked)
                {
                    addTreesPaint(e.X, e.Y);
                    return;
                }
            }

            //if (checkedListBox1.Items.Count == 0) return;
            //if (numSourceCars == 0) return;
            lastMouseX = e.X;
            lastMouseY = e.Y;
            //this.Text = lastMouseX + " " + lastMouseY;
            //checkedListBox1.Items.Clear();
            float dist;
            float nearest = 999999f;
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


        //call this whenever scale needs to be recalculated.
        void scaleChanged()
        {
            float mapMax = 200000f, mapMax2 = 400000f;
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
        Pen handlePen3 = new Pen(Color.White, 3f);
        Brush bRed = new SolidBrush(Color.Red);
        Brush bDarkRed = new SolidBrush(Color.DarkRed);
        Brush bWhite = new SolidBrush(Color.White);
        Brush bYellow = new SolidBrush(Color.Yellow);
        Brush bBlack = new SolidBrush(Color.Black);
        Brush bTrees = new SolidBrush(Color.Green);
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

                //draw track segments
                if (checkBox3.Checked)
                {
                    for (int i = 0; i < numTracks; i++)
                    {
                        g.FillRectangle(bBlack, trackDrawStart[i].X, trackDrawStart[i].Y, 5f, 5f);
                        g.DrawBezier(handlePen3, trackDrawStart[i].X, trackDrawStart[i].Y, tangentDrawStart[i].X, tangentDrawStart[i].Y, tangentDrawEnd[i].X, tangentDrawEnd[i].Y, trackDrawEnd[i].X, trackDrawEnd[i].Y);
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
            if (checkBoxAddTrees.Checked) checkBoxRemoveTrees.Checked = false;
        }

        private void checkBoxRemoveTrees_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBoxRemoveTrees.Checked) checkBoxAddTrees.Checked = false;
        }

        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            label8.Text = "Radius" + trackBar1.Value.ToString() + "m";
        }

        private void button8_Click(object sender, EventArgs e)
        {
            MessageBox.Show("This might take a minute...","Possible Long Operation");
            clearTreesFromTracks();
            showBuggiesOnMap();
        }

        private void button7_Click(object sender, EventArgs e)
        {
            //add all the trees back in...
            for (int i = 0;i<treeExists.Length;i++) treeExists[i] = true;
            showBuggiesOnMap();
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



        byte[][] SanderAmountArray2;

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
                Debug.WriteLine("Checking @ " + pos.ToString());
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
                        MessageBox.Show("Note: Possible Name corruption (buggy number " + i.ToString() + " of " + numSourceCars.ToString() + ") detected in save. If this operation fails you should load a copy of this save in ROSE to rename the buggies.\nLook for one that might have a <br> or other odd characters in it, or count in the list up from zero to the number supplied earlier.");
                        //find the next instance of "FF" and then back up 4 spaces:
                        for (int k = pos; k < buff.Length; k++)
                        {
                            if (buff[k] == 255)
                            {
                                pos = k - 4;
                                break;
                            }
                        }
                        //create a blank name for this one
                        arrTemp[i] = new byte[] { 0x00, 0x00, 0x00, 0x00, 0xFF, 0x00, 0x00, 0x00, 0x00 };
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