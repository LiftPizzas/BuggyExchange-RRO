using BuggyExchange.Properties;
using Microsoft.VisualBasic.Devices;
using System.Diagnostics;
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

        public void ClickLabel(Object sender, System.EventArgs e)
        {
            
            Label btn = (Label)sender;
            for (int i = 0; i < 11; i++)
            {
                lblSlot[i].BorderStyle = BorderStyle.FixedSingle;
                //lblSlot[i].BackColor = SystemColors.Control;
            }
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
            checkedListBox1.Items.Clear();
            //checkedListBox1.Items.Add("Nothing yet...");
            map = new Bitmap(pictureBox1.Width, pictureBox1.Height);

            

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

            //System.Diagnostics.Process.Start("explorer.exe", Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + @"\arr\Saved\SaveGames");

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

        byte[] buff;
        int numSourceCars = 0;
        string[] frameTypes;
        float[] framePositionsX, framePositionsY, framePositionsZ;
        float[] mapX, mapY; //drawing positions of cars
        float[] frameRotationsX, frameRotationsY, frameRotationsZ;
        Bitmap map;

       



        float lastMouseX, lastMouseY;
        int nearestBuggy = -1;

        private void checkedListBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            nearestBuggy = checkedListBox1.SelectedIndex;
            showBuggiesOnMap();
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            //!checkedListBox1.GetItemChecked(nearestBuggy)


        }

        int dragging = 0;
        float dragX, dragY;
        float dx1, dx2, dy1, dy2; //drag rectangle positions for drawing

        private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
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
                    dragging = 2; //deselect nearest one
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
                    dragging = 1; //select nearest one
            }

        }

        private void pictureBox1_MouseUp(object sender, MouseEventArgs e)
        {
            //if dragging, make sure everything inside the rectangle is checked before disabling the drag flag

            dragging = 0;
        }

        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
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
                float dx = lastMouseX - mapX[i];
                float dy = lastMouseY - mapY[i];
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
                    if (mapX[i] >= dx1 && mapX[i] <= dx2)
                        if (mapY[i] >= dy1 && mapY[i] <= dy2)
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


        Brush bRed = new SolidBrush(Color.Red);
        Brush bWhite = new SolidBrush(Color.White);
        Brush bYellow = new SolidBrush(Color.Yellow);
        private void showBuggiesOnMap()
        {
            //if (checkedListBox1.Items.Count == 0) return;
            
            //map = new Bitmap(Resources.ResourceManager. rro_map);
            using (Graphics g = Graphics.FromImage(map))
            {
                g.DrawImage(Resources.rro_map_gray, 0, 0, 850, 850);
                //g.Clear(Color.White);

                //draw Target File's cars on the map
                for (int i = 0; i < numTargetCars; i++)
                        g.FillRectangle(bYellow, map2X[i], map2Y[i], 5f, 5f);

                if (dragging == 3 || dragging == 4)
                {
                    g.DrawRectangle(Pens.LightBlue, dx1, dy1, dx2 - dx1, dy2 - dy1);
                }

                //draw New cars on the map
                for (int i = 0; i < numSourceCars; i++)
                {
                    if (checkedListBox1.GetItemChecked(i))
                        g.FillRectangle(bWhite, mapX[i], mapY[i], 3f, 3f);
                    else
                        g.FillRectangle(bRed, mapX[i], mapY[i], 3f, 3f);
                }
                if (nearestBuggy > -1)
                {
                    g.FillRectangle(new SolidBrush(Color.Blue), mapX[nearestBuggy], mapY[nearestBuggy], 5f, 5f);
                }



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



        void parseSaveNow(string file)
        {
            //need to find all of the rolling stock in the file, find "FrameTypeArray" in the file, number of cars in the save is at this position +58 bytes
            buff = File.ReadAllBytes(file);

            int found = findInBytes("FrameTypeArray");
            if (found == -1)
            {
                MessageBox.Show("Unable to find 'FrameTypeArray' in this save file. Make sure you dropped a valid save file here.");
                this.Text = "Invalid File - Try Again";
                return;
            }

            checkedListBox1.Items.Clear();

            //now we need to get the number of cars
            numSourceCars = buff[found + 58] + (buff[found + 59] * 256);

            if (numSourceCars == 0)
            {
                MessageBox.Show("Note: this save does not contain any buggies.");
                return;
            }

            frameTypes = new string[numSourceCars];
            framePositionsX = new float[numSourceCars];
            framePositionsY = new float[numSourceCars];
            framePositionsZ = new float[numSourceCars];
            mapX = new float[numSourceCars];
            mapY = new float[numSourceCars];
            frameRotationsX = new float[numSourceCars];
            frameRotationsY = new float[numSourceCars];
            frameRotationsZ = new float[numSourceCars];
            carShortName = new string[numSourceCars];
            carShortNumber = new string[numSourceCars];
            cargoShortName = new string[numSourceCars];

            // CAR TYPES
            int i = 0;
            int pos = found + 66;
            int l;
            while (i < numSourceCars)
            {
                l = buff[pos - 4]; //this is the length of the string we're getting
                frameTypes[i] = buffToString(pos, l);
                checkedListBox1.Items.Add(" (" + frameTypes[i].Substring(0, frameTypes[i].Length - 1) + ")");
                pos += l + 4;
                i++;
            }

            //-------------------------------------------
            // LOCATIONS
            found = findInBytes("FrameLocationArray");
            if (found == -1)
            {
                MessageBox.Show("Unable to find 'FrameLocationArray' in this save file. Make sure you dropped a valid save file here.");
                this.Text = "Invalid File - Try Again";
                checkedListBox1.Items.Clear();
                return;
            }

            FrameLocationArray = getVectorArray(Encoding.ASCII.GetBytes( "FrameLocationArray"),147);

            i = 0;
            pos = found + 147;
            while (i < numSourceCars)
            {
                framePositionsX[i] = System.BitConverter.ToSingle(buff, pos);
                framePositionsY[i] = System.BitConverter.ToSingle(buff, pos + 4);
                framePositionsZ[i] = System.BitConverter.ToSingle(buff, pos + 8);
                //Debug.WriteLine(framePositionsX[i].ToString() + ", " + framePositionsY[i].ToString() + ", " + framePositionsZ[i].ToString());
                //show these on the map so we can click them or highlight or something.
                pos += 12;
                i++;
            }

            float mapScale = pictureBox1.Width / 400000f;
            float cX = pictureBox1.Width / 2f;
            float cY = pictureBox1.Width / 2f;

            for (int c = 0; c < numSourceCars; c++)
            {
                //calculate the map positions of the cars.
                mapX[c] = cX - (framePositionsX[c] * mapScale);
                mapY[c] = cY - (framePositionsY[c] * mapScale);
                //Debug.WriteLine(mapX[c].ToString() + ", " + mapY[c].ToString());
            }



            //-------------------------------------------
            // Rotations
            found = findInBytes("FrameRotationArray");
            if (found == -1)
            {
                MessageBox.Show("Unable to find 'FrameRotationArray' in this save file. Make sure you dropped a valid save file here.");
                this.Text = "Invalid File - Try Again";
                checkedListBox1.Items.Clear();
                return;
            }

            FrameRotationArray = getVectorArray(Encoding.ASCII.GetBytes("FrameRotationArray"),148);

            i = 0;
            pos = found + 148;
            while (i < numSourceCars)
            {
                frameRotationsX[i] = System.BitConverter.ToSingle(buff, pos);
                frameRotationsY[i] = System.BitConverter.ToSingle(buff, pos + 4);
                frameRotationsZ[i] = System.BitConverter.ToSingle(buff, pos + 8);
                //Debug.WriteLine(frameRotationsX[i].ToString() + ", " + frameRotationsY[i].ToString() + ", " + frameRotationsZ[i].ToString());
                //show these on the map so we can click them or highlight or something.
                pos += 12;
                i++;
            }

            //the rest of the file data is read in using standardized functions for each type

            //FrameNumberArray (string)
            FrameNumberArray = getStringArray(FrameNumberHeader, FrameNumberLenPos);
            if (FrameNumberArray.Length == 0)
            {
                MessageBox.Show("Error reading file: cannot parse buggy Numbers.");
                return;
            }
            else
            {
                for (i = 0; i < numSourceCars; i++)
                {
                    if (FrameNumberArray[i].Length > 9)
                        carShortNumber[i] = Encoding.ASCII.GetString(FrameNumberArray[i], 13, FrameNumberArray[i].Length - 14);
                }

            }

            FrameNameArray = getStringArray(FrameNameHeader, FrameNameLenPos);
            if (FrameNameArray.Length == 0)
            {
                MessageBox.Show("Error reading file: cannot parse buggy Names.");
                return;
            }
            else
            {
                for (i = 0; i < numSourceCars; i++)
                    if (FrameNameArray[i].Length > 9)
                        carShortName[i] = Encoding.ASCII.GetString(FrameNameArray[i], 13, FrameNameArray[i].Length - 14);
            }

            //Debug.WriteLine("Getting Cargo:");
            //FreightTypeArray (string)
            FreightTypeArray = getCargoArray(FreightTypeHeader, FreightTypeLenPos);
            if (FreightTypeArray.Length == 0)
            {
                MessageBox.Show("Error reading file: cannot parse Freight Types.");
                return;
            }
            else
            {
                for (i = 0; i < numSourceCars; i++)
                    if (FreightTypeArray[i].Length > 4)
                        cargoShortName[i] = Encoding.ASCII.GetString(FreightTypeArray[i], 4, FreightTypeArray[i].Length - 5);
            }
            //Debug.WriteLine("Done Getting Cargo:");


            //display these in the list
            for (int j = 0; j < numSourceCars; j++)
            {
                string toadd = "";
                if (carShortName[j] != null && carShortName[j].Length > 0)
                {
                    toadd = carShortName[j] + " ";
                }
                if (carShortNumber[j] != null && carShortNumber[j].Length > 0)
                {
                    toadd += "#" + carShortNumber[j] + " ";
                }
                if (cargoShortName[j] != null && cargoShortName[j].Length > 0)
                    toadd += "[" + cargoShortName[j] + "]";

                checkedListBox1.Items[j] = toadd + checkedListBox1.Items[j];
            }

            //--------------------------------------------------------------------------------------------------------
            //SmokestackTypeArray (Int)
            SmokestackTypeArray = getIntArray(SmokestackTypeHeader, SmokestackTypeLenPos);
            if (SmokestackTypeArray.Length == 0)
            {
                MessageBox.Show("Error reading file: cannot parse Smokestacks.");
                nearestBuggy = -1;
                return;
            }
            //--------------------------------------------------------------------------------------------------------
            //HeadlightTypeArray (Int)
            HeadlightTypeArray = getIntArray(HeadlightTypeHeader, HeadlightTypeLenPos);
            if (HeadlightTypeArray.Length == 0)
            {
                MessageBox.Show("Error reading file: cannot parse Headlights.");
                nearestBuggy = -1;
                return;
            }
            //--------------------------------------------------------------------------------------------------------
            //PaintTypeArray (Int)
            PaintTypeArray = getIntArray(PaintTypeHeader, PaintTypeLenPos, true);
            if (PaintTypeArray == null || PaintTypeArray.Length == 0)
            {
                this.Text = "Parsing File...";
                //old saves don't contain paint data so we'll just add this in manually.
                PaintTypeArray = new byte[numSourceCars][];
                for (int j = 0; j < numSourceCars; j++)
                {
                    PaintTypeArray[j] = new byte[] { 0x00, 0x00, 0x00, 0x00 };
                }

                //MessageBox.Show("Error reading file: cannot parse PaintType.");
                //nearestBuggy = -1;
                //return;
            }
            //--------------------------------------------------------------------------------------------------------
            //BoilerFuelAmountArray (float)
            BoilerFuelAmountArray = getIntArray(BoilerFuelAmountHeader, BoilerFuelAmountLenPos);
            if (BoilerFuelAmountArray.Length == 0)
            {
                MessageBox.Show("Error reading file: cannot parse BoilerFuelAmount.");
                nearestBuggy = -1;
                return;
            }

            //--------------------------------------------------------------------------------------------------------
            //BoilerFireTempArray (float)
            BoilerFireTempArray = getIntArray(BoilerFireTempHeader, BoilerFireTempLenPos);
            if (BoilerFireTempArray.Length == 0)
            {
                MessageBox.Show("Error reading file: cannot parse BoilerFireTemp.");
                nearestBuggy = -1;
                return;
            }

            //--------------------------------------------------------------------------------------------------------
            //BoilerWaterTempArray (float)
            BoilerWaterTempArray = getIntArray(BoilerWaterTempHeader, BoilerWaterTempLenPos);
            if (BoilerWaterTempArray.Length == 0)
            {
                MessageBox.Show("Error reading file: cannot parse BoilerWaterTemp.");
                nearestBuggy = -1;
                return;
            }

            //--------------------------------------------------------------------------------------------------------
            //BoilerWaterLevelArray (float)
            BoilerWaterLevelArray = getIntArray(BoilerWaterLevelHeader, BoilerWaterLevelLenPos);
            if (BoilerWaterLevelArray.Length == 0)
            {
                MessageBox.Show("Error reading file: cannot parse BoilerWaterLevel.");
                nearestBuggy = -1;
                return;
            }

            //--------------------------------------------------------------------------------------------------------
            //BoilerPressureArray (float)
            BoilerPressureArray = getIntArray(BoilerPressureHeader, BoilerPressureLenPos);
            if (BoilerPressureArray.Length == 0)
            {
                MessageBox.Show("Error reading file: cannot parse BoilerPressure.");
                nearestBuggy = -1;
                return;
            }

            //--------------------------------------------------------------------------------------------------------
            //HeadlightFrontStateArray (bool)
            HeadlightFrontStateArray = getBoolArray(HeadlightFrontStateHeader, HeadlightFrontStateLenPos);
            if (HeadlightFrontStateArray.Length == 0)
            {
                MessageBox.Show("Error reading file: cannot parse HeadlightFrontState.");
                nearestBuggy = -1;
                return;
            }

            //--------------------------------------------------------------------------------------------------------
            //HeadlightRearStateArray (bool)
            HeadlightRearStateArray = getBoolArray(HeadlightRearStateHeader, HeadlightRearStateLenPos);
            if (HeadlightRearStateArray.Length == 0)
            {
                MessageBox.Show("Error reading file: cannot parse HeadlightRearState.");
                nearestBuggy = -1;
                return;
            }

            //--------------------------------------------------------------------------------------------------------
            //CouplerFrontStateArray (bool)
            CouplerFrontStateArray = getBoolArray(CouplerFrontStateHeader, CouplerFrontStateLenPos);
            if (CouplerFrontStateArray.Length == 0)
            {
                MessageBox.Show("Error reading file: cannot parse CouplerFrontState.");
                nearestBuggy = -1;
                return;
            }

            //--------------------------------------------------------------------------------------------------------
            //CouplerRearStateArray (bool)
            CouplerRearStateArray = getBoolArray(CouplerRearStateHeader, CouplerRearStateLenPos);
            if (CouplerRearStateArray.Length == 0)
            {
                MessageBox.Show("Error reading file: cannot parse CouplerRearState.");
                nearestBuggy = -1;
                return;
            }

            //--------------------------------------------------------------------------------------------------------
            //TenderFuelAmountArray (float)
            TenderFuelAmountArray = getIntArray(TenderFuelAmountHeader, TenderFuelAmountLenPos);
            if (TenderFuelAmountArray.Length == 0)
            {
                MessageBox.Show("Error reading file: cannot parse TenderFuelAmount.");
                nearestBuggy = -1;
                return;
            }

            //--------------------------------------------------------------------------------------------------------
            //TenderWaterAmountArray (float)
            TenderWaterAmountArray = getIntArray(TenderWaterAmountHeader, TenderWaterAmountLenPos);
            if (TenderWaterAmountArray.Length == 0)
            {
                MessageBox.Show("Error reading file: cannot parse TenderWaterAmount.");
                nearestBuggy = -1;
                return;
            }


            //--------------------------------------------------------------------------------------------------------
            //CompressorAirPressureArray (float)
            CompressorAirPressureArray = getIntArray(CompressorAirPressureHeader, CompressorAirPressureLenPos);
            if (CompressorAirPressureArray.Length == 0)
            {
                MessageBox.Show("Error reading file: cannot parse CompressorAirPressure.");
                nearestBuggy = -1;
                return;
            }


            //--------------------------------------------------------------------------------------------------------
            //MarkerLightsFrontRightStateArray (Int)
            MarkerLightsFrontRightStateArray = getIntArray(MarkerLightsFrontRightStateHeader, MarkerLightsFrontRightStateLenPos);
            if (MarkerLightsFrontRightStateArray.Length == 0)
            {
                MessageBox.Show("Error reading file: cannot parse MarkerLightsFrontRightState.");
                nearestBuggy = -1;
                return;
            }

            //--------------------------------------------------------------------------------------------------------
            //MarkerLightsFrontLeftStateArray (Int)
            MarkerLightsFrontLeftStateArray = getIntArray(MarkerLightsFrontLeftStateHeader, MarkerLightsFrontLeftStateLenPos);
            if (MarkerLightsFrontLeftStateArray.Length == 0)
            {
                MessageBox.Show("Error reading file: cannot parse MarkerLightsFrontLeftState.");
                nearestBuggy = -1;
                return;
            }

            //--------------------------------------------------------------------------------------------------------
            //MarkerLightsRearRightStateArray (Int)
            MarkerLightsRearRightStateArray = getIntArray(MarkerLightsRearRightStateHeader, MarkerLightsRearRightStateLenPos);
            if (MarkerLightsRearRightStateArray.Length == 0)
            {
                MessageBox.Show("Error reading file: cannot parse MarkerLightsRearRightState.");
                nearestBuggy = -1;
                return;
            }

            //--------------------------------------------------------------------------------------------------------
            //MarkerLightsRearLeftStateArray (Int)
            MarkerLightsRearLeftStateArray = getIntArray(MarkerLightsRearLeftStateHeader, MarkerLightsRearLeftStateLenPos);
            if (MarkerLightsRearLeftStateArray.Length == 0)
            {
                MessageBox.Show("Error reading file: cannot parse MarkerLightsRearLeftState.");
                nearestBuggy = -1;
                return;
            }

            //--------------------------------------------------------------------------------------------------------
            //MarkerLightsCenterStateArray (Int)
            MarkerLightsCenterStateArray = getIntArray(MarkerLightsCenterStateHeader, MarkerLightsCenterStateLenPos);
            if (MarkerLightsCenterStateArray.Length == 0)
            {
                MessageBox.Show("Error reading file: cannot parse MarkerLightsCenterState.");
                nearestBuggy = -1;
                return;
            }

            //--------------------------------------------------------------------------------------------------------
            //FreightAmountArray (Int)
            FreightAmountArray = getIntArray(FreightAmountHeader, FreightAmountLenPos);
            if (FreightAmountArray.Length == 0)
            {
                MessageBox.Show("Error reading file: cannot parse FreightAmount.");
                nearestBuggy = -1;
                return;
            }

            //--------------------------------------------------------------------------------------------------------
            //RegulatorValueArray (float)
            RegulatorValueArray = getIntArray(RegulatorValueHeader, RegulatorValueLenPos);
            if (RegulatorValueArray.Length == 0)
            {
                MessageBox.Show("Error reading file: cannot parse RegulatorValue.");
                nearestBuggy = -1;
                return;
            }

            //--------------------------------------------------------------------------------------------------------
            //BrakeValueArray (float)
            BrakeValueArray = getIntArray(BrakeValueHeader, BrakeValueLenPos);
            if (BrakeValueArray.Length == 0)
            {
                MessageBox.Show("Error reading file: cannot parse BrakeValue.");
                nearestBuggy = -1;
                return;
            }

            //--------------------------------------------------------------------------------------------------------
            //GeneratorValveValueArray (float)
            GeneratorValveValueArray = getIntArray(GeneratorValveValueHeader, GeneratorValveValueLenPos);
            if (GeneratorValveValueArray.Length == 0)
            {
                MessageBox.Show("Error reading file: cannot parse GeneratorValveValue.");
                nearestBuggy = -1;
                return;
            }

            //--------------------------------------------------------------------------------------------------------
            //CompressorValveValueArray (float)
            CompressorValveValueArray = getIntArray(CompressorValveValueHeader, CompressorValveValueLenPos);
            if (CompressorValveValueArray.Length == 0)
            {
                MessageBox.Show("Error reading file: cannot parse CompressorValveValue.");
                nearestBuggy = -1;
                return;
            }

            //--------------------------------------------------------------------------------------------------------
            //ReverserValueArray (float)
            ReverserValueArray = getIntArray(ReverserValueHeader, ReverserValueLenPos);
            if (ReverserValueArray.Length == 0)
            {
                MessageBox.Show("Error reading file: cannot parse ReverserValue.");
                nearestBuggy = -1;
                return;
            }


            //--------------------------------------------------------------------------------------------------------
            //SanderAmountArray (float)
            SanderAmountArray = getIntArray(SanderAmountHeader, SanderAmountLenPos, true);
            if (SanderAmountArray.Length == 0)
            {
                MessageBox.Show("Error reading file: cannot parse SanderAmount.");
                nearestBuggy = -1;
                return;
            }

            showBuggiesOnMap();
            this.Text = "File Loaded. " + checkedListBox1.Items.Count.ToString() + " buggies found.";
        }

        byte[] convertHeader(string hex)
        {
            string[] all = hex.Split(' ');
            byte[] tOut = new byte[all.Length];
            for (int i = 0; i < all.Length; i++)
                tOut[i] = byte.Parse(all[i], System.Globalization.NumberStyles.AllowHexSpecifier);

            return tOut;
        }

        byte[] FrameLocationHeader;
        byte[] FrameRotationHeader;
        byte[] FrameTypeHeader;
        byte[] FrameNameHeader;
        byte[] FrameNumberHeader;
        byte[] SmokestackTypeHeader;
        byte[] HeadlightTypeHeader;
        byte[] PaintTypeHeader;
        byte[] BoilerFuelAmountHeader;
        byte[] BoilerFireTempHeader;
        byte[] BoilerWaterTempHeader;
        byte[] BoilerWaterLevelHeader;
        byte[] BoilerPressureHeader;
        byte[] HeadlightFrontStateHeader;
        byte[] HeadlightRearStateHeader;
        byte[] CouplerFrontStateHeader;
        byte[] CouplerRearStateHeader;
        byte[] TenderFuelAmountHeader;
        byte[] TenderWaterAmountHeader;
        byte[] CompressorAirPressureHeader;
        byte[] MarkerLightsFrontRightStateHeader;
        byte[] MarkerLightsFrontLeftStateHeader;
        byte[] MarkerLightsRearRightStateHeader;
        byte[] MarkerLightsRearLeftStateHeader;
        byte[] MarkerLightsCenterStateHeader;
        byte[] FreightTypeHeader;
        byte[] FreightAmountHeader;
        byte[] RegulatorValueHeader;
        byte[] BrakeValueHeader;
        byte[] GeneratorValveValueHeader;
        byte[] CompressorValveValueHeader;
        byte[] ReverserValueHeader;
        byte[] SanderAmountHeader;

        int FrameLocationLenPos = 41;
        int FrameRotationLenPos = 41;
        int FrameTypeLenPos = 37;
        int FrameNameLenPos = 37;
        int FrameNumberLenPos = 39;
        int SmokestackTypeLenPos = 42;
        int HeadlightTypeLenPos = 41;
        int PaintTypeLenPos = 37;
        int BoilerFuelAmountLenPos = 44;
        int BoilerFireTempLenPos = 42;
        int BoilerWaterTempLenPos = 43;
        int BoilerWaterLevelLenPos = 44;
        int BoilerPressureLenPos = 42;
        int HeadlightFrontStateLenPos = 47;
        int HeadlightRearStateLenPos = 46;
        int CouplerFrontStateLenPos = 45;
        int CouplerRearStateLenPos = 44;
        int TenderFuelAmountLenPos = 44;
        int TenderWaterAmountLenPos = 45;
        int CompressorAirPressureLenPos = 49;
        int MarkerLightsFrontRightStateLenPos = 55;
        int MarkerLightsFrontLeftStateLenPos = 54;
        int MarkerLightsRearRightStateLenPos = 54;
        int MarkerLightsRearLeftStateLenPos = 53;
        int MarkerLightsCenterStateLenPos = 51;
        int FreightTypeLenPos = 39;
        int FreightAmountLenPos = 41;
        int RegulatorValueLenPos = 42;
        int BrakeValueLenPos = 38;
        int GeneratorValveValueLenPos = 47;
        int CompressorValveValueLenPos = 48;
        int ReverserValueLenPos = 41;
        int SanderAmountLenPos = 40;

        string[] carShortName, carShortNumber, cargoShortName;


        byte[][] FrameLocationArray;
        byte[][] FrameRotationArray;
        byte[][] FrameNameArray;
        byte[][] FrameNumberArray;
        byte[][] SmokestackTypeArray;
        byte[][] HeadlightTypeArray;
        byte[][] PaintTypeArray;
        byte[][] BoilerFuelAmountArray;
        byte[][] BoilerFireTempArray;
        byte[][] BoilerWaterTempArray;
        byte[][] BoilerWaterLevelArray;
        byte[][] BoilerPressureArray;
        byte[] HeadlightFrontStateArray;
        byte[] HeadlightRearStateArray;
        byte[] CouplerFrontStateArray;
        byte[] CouplerRearStateArray;
        byte[][] TenderFuelAmountArray;
        byte[][] TenderWaterAmountArray;
        byte[][] CompressorAirPressureArray;
        byte[][] MarkerLightsFrontRightStateArray;
        byte[][] MarkerLightsFrontLeftStateArray;
        byte[][] MarkerLightsRearRightStateArray;
        byte[][] MarkerLightsRearLeftStateArray;
        byte[][] MarkerLightsCenterStateArray;
        byte[][] FreightTypeArray;
        byte[][] FreightAmountArray;
        byte[][] RegulatorValueArray;
        byte[][] BrakeValueArray;
        byte[][] GeneratorValveValueArray;
        byte[][] CompressorValveValueArray;
        byte[][] ReverserValueArray;
        byte[][] SanderAmountArray;

        byte[][] FrameLocationArray2;
        byte[][] FrameRotationArray2;
        byte[][] FrameNameArray2;
        byte[][] FrameNumberArray2;
        byte[][] SmokestackTypeArray2;
        byte[][] HeadlightTypeArray2;
        byte[][] PaintTypeArray2;
        byte[][] BoilerFuelAmountArray2;
        byte[][] BoilerFireTempArray2;
        byte[][] BoilerWaterTempArray2;
        byte[][] BoilerWaterLevelArray2;
        byte[][] BoilerPressureArray2;
        byte[] HeadlightFrontStateArray2;
        byte[] HeadlightRearStateArray2;
        byte[] CouplerFrontStateArray2;
        byte[] CouplerRearStateArray2;
        byte[][] TenderFuelAmountArray2;
        byte[][] TenderWaterAmountArray2;
        byte[][] CompressorAirPressureArray2;
        byte[][] MarkerLightsFrontRightStateArray2;
        byte[][] MarkerLightsFrontLeftStateArray2;
        byte[][] MarkerLightsRearRightStateArray2;
        byte[][] MarkerLightsRearLeftStateArray2;
        byte[][] MarkerLightsCenterStateArray2;
        byte[][] FreightTypeArray2;
        byte[][] FreightAmountArray2;
        byte[][] RegulatorValueArray2;
        byte[][] BrakeValueArray2;
        byte[][] GeneratorValveValueArray2;
        byte[][] CompressorValveValueArray2;
        byte[][] ReverserValueArray2;

        private void button4_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("explorer.exe", Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + @"\arr\Saved\SaveGames");
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
                if (debug) Debug.WriteLine("Checking @ " + pos.ToString() + " = " + BitConverter.ToSingle(arrTemp[i]));
                pos += 4;
            }
            return arrTemp;
        }


        byte[][] getVectorArray(byte[] header, int offset) 
        {
            int ign = header.Length +4;//just set ign to greater than the length... we're not searching for the entire header this time
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
                if (debug) Debug.WriteLine("Checking @ " + pos.ToString() + " = " + (arrTemp[i] == 1).ToString());
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