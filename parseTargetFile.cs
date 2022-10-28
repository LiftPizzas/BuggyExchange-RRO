using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace BuggyExchange
{
    public partial class Form1 : Form
    {
        byte[] buff2;
        int numTargetCars = 0;
        string[] frameTypes2;
        float[] framePositions2X, framePositions2Y, framePositions2Z;
        float[] buggyDraw2X, buggyDraw2Y; //drawing positions of cars
        float[] frameRotations2X, frameRotations2Y, frameRotations2Z;


        string buff2ToString(int pos, int l)
        {
            byte[] t = new byte[l];
            for (int i = 0; i < l; i++)
            {
                t[i] = buff2[pos + i];
            }
            return Encoding.ASCII.GetString(t);
        }

        void parseTargetSaveNow(string file)
        {
            //need to find all of the rolling stock in the file, find "FrameTypeArray" in the file, number of cars in the save is at this position +58 bytes
            buff2 = File.ReadAllBytes(file);

            int found = findInBytes2("FrameTypeArray");
            if (found == -1)
            {
                MessageBox.Show("Unable to find 'FrameTypeArray' in this save file. Make sure you dropped a valid save file here.");
                this.Text = "Invalid File - Try Again";
                return;
            }

            checkedListBox2.Items.Clear();

            //now we need to get the number of cars
            numTargetCars = buff2[found + 58] + (buff2[found + 59] * 256);

            if (numTargetCars == 0)
            {
                MessageBox.Show("Note: this save does not contain any buggies.");
                return;
            }

            frameTypes2 = new string[numTargetCars];
            framePositions2X = new float[numTargetCars];
            framePositions2Y = new float[numTargetCars];
            framePositions2Z = new float[numTargetCars];
            buggyDraw2X = new float[numTargetCars];
            buggyDraw2Y = new float[numTargetCars];
            frameRotations2X = new float[numTargetCars];
            frameRotations2Y = new float[numTargetCars];
            frameRotations2Z = new float[numTargetCars];


            // CAR TYPES
            int i = 0;
            int pos = found + 66;
            int l;
            while (i < numTargetCars)
            {
                l = buff2[pos - 4]; //this is the length of the string we're getting
                frameTypes2[i] = buff2ToString(pos, l);
                checkedListBox2.Items.Add(" (" + frameTypes2[i].Substring(0, frameTypes2[i].Length - 1) + ")");
                pos += l + 4;
                i++;
            }

            //-------------------------------------------
            // LOCATIONS
            found = findInBytes2("FrameLocationArray");
            if (found == -1)
            {
                MessageBox.Show("Unable to find 'FrameLocationArray' in this save file. Make sure you dropped a valid save file here.");
                this.Text = "Invalid File - Try Again";
                checkedListBox2.Items.Clear();
                return;
            }

            FrameLocationArray2 = getVectorArray2(Encoding.ASCII.GetBytes("FrameLocationArray"), 147, numTargetCars);

            i = 0;
            pos = found + 147;
            while (i < numTargetCars)
            {
                framePositions2X[i] = System.BitConverter.ToSingle(buff2, pos);
                framePositions2Y[i] = System.BitConverter.ToSingle(buff2, pos + 4);
                framePositions2Z[i] = System.BitConverter.ToSingle(buff2, pos + 8);
                //Debug.WriteLine(framePositions2X[i].ToString() + ", " + framePositions2Y[i].ToString() + ", " + framePositions2Z[i].ToString());
                //show these on the map so we can click them or highlight or something.
                pos += 12;
                i++;
            }


            //-------------------------------------------
            // Rotations
            found = findInBytes2("FrameRotationArray");
            if (found == -1)
            {
                MessageBox.Show("Unable to find 'FrameRotationArray' in this save file. Make sure you dropped a valid save file here.");
                this.Text = "Invalid File - Try Again";
                checkedListBox2.Items.Clear();
                return;
            }

            FrameRotationArray2 = getVectorArray2(Encoding.ASCII.GetBytes("FrameRotationArray"), 148, numTargetCars);

            i = 0;
            pos = found + 148;
            while (i < numTargetCars)
            {
                frameRotations2X[i] = System.BitConverter.ToSingle(buff2, pos);
                frameRotations2Y[i] = System.BitConverter.ToSingle(buff2, pos + 4);
                frameRotations2Z[i] = System.BitConverter.ToSingle(buff2, pos + 8);
                //Debug.WriteLine(frameRotations2X[i].ToString() + ", " + frameRotations2Y[i].ToString() + ", " + frameRotations2Z[i].ToString());
                //show these on the map so we can click them or highlight or something.
                pos += 12;
                i++;
            }

            //the rest of the file data is read in using standardized functions for each type

            //FrameNumberArray (string)
            FrameNumberArray2 = getStringArray2(FrameNumberHeader, FrameNumberLenPos);
            if (FrameNumberArray2.Length == 0)
            {
                MessageBox.Show("Error reading file: cannot parse buggy Numbers.");
                return;
            }
            //else
            //{
            //    for (i = 0; i < numTargetCars; i++)
            //    {
            //        if (FrameNumberArray2[i].Length > 9)
            //            carShortNumber[i] = Encoding.ASCII.GetString(FrameNumberArray2[i], 13, FrameNumberArray2[i].Length - 14);
            //    }

            //}

            FrameNameArray2 = getStringArray2(FrameNameHeader, FrameNameLenPos);
            if (FrameNameArray2.Length == 0)
            {
                MessageBox.Show("Error reading file: cannot parse buggy Names.");
                return;
            }
            //else
            //{
            //    for (i = 0; i < numTargetCars; i++)
            //        if (FrameNameArray2[i].Length > 9)
            //            carShortName[i] = Encoding.ASCII.GetString(FrameNameArray2[i], 13, FrameNameArray2[i].Length - 14);
            //}

            //Debug.WriteLine("Getting Cargo:");
            //FreightTypeArray (string)
            FreightTypeArray2 = getCargoArray2(FreightTypeHeader, FreightTypeLenPos);
            if (FreightTypeArray2.Length == 0)
            {
                MessageBox.Show("Error reading file: cannot parse Freight Types.");
                return;
            }
            //else
            //{
            //    for (i = 0; i < numTargetCars; i++)
            //        if (FreightTypeArray2[i].Length > 4)
            //            cargoShortName[i] = Encoding.ASCII.GetString(FreightTypeArray2[i], 4, FreightTypeArray2[i].Length - 5);
            //}
            //Debug.WriteLine("Done Getting Cargo:");


            //display these in the list
            //for (int j = 0; j < numTargetCars; j++)
            //{
            //    string toadd = "";
            //    if (carShortName[j] != null && carShortName[j].Length > 0)
            //    {
            //        toadd = carShortName[j] + " ";
            //    }
            //    if (carShortNumber[j] != null && carShortNumber[j].Length > 0)
            //    {
            //        toadd += "#" + carShortNumber[j] + " ";
            //    }
            //    if (cargoShortName[j] != null && cargoShortName[j].Length > 0)
            //        toadd += "[" + cargoShortName[j] + "]";

            //    checkedListBox2.Items[j] = toadd + checkedListBox2.Items[j];
            //}

            //--------------------------------------------------------------------------------------------------------
            //SmokestackTypeArray (Int)
            SmokestackTypeArray2 = getIntArray2(SmokestackTypeHeader, SmokestackTypeLenPos, numTargetCars);
            if (SmokestackTypeArray2.Length == 0)
            {
                MessageBox.Show("Error reading file: cannot parse Smokestacks.");
                nearestBuggy = -1;
                return;
            }
            //--------------------------------------------------------------------------------------------------------
            //HeadlightTypeArray (Int)
            HeadlightTypeArray2 = getIntArray2(HeadlightTypeHeader, HeadlightTypeLenPos, numTargetCars);
            if (HeadlightTypeArray2.Length == 0)
            {
                MessageBox.Show("Error reading file: cannot parse Headlights.");
                nearestBuggy = -1;
                return;
            }
            //--------------------------------------------------------------------------------------------------------
            //PaintTypeArray (Int)
            PaintTypeArray2 = getIntArray2(PaintTypeHeader, PaintTypeLenPos, numTargetCars, true);
            if (PaintTypeArray2 == null || PaintTypeArray2.Length == 0)
            {
                this.Text = "Parsing File...";
                //old saves don't contain paint data so we'll just add this in manually.
                PaintTypeArray2 = new byte[numTargetCars][];
                for (int j = 0; j < numTargetCars; j++)
                {
                    PaintTypeArray2[j] = new byte[] { 0x00, 0x00, 0x00, 0x00 };
                }

                //MessageBox.Show("Error reading file: cannot parse PaintType.");
                //nearestBuggy = -1;
                //return;
            }
            //--------------------------------------------------------------------------------------------------------
            //BoilerFuelAmountArray (float)
            BoilerFuelAmountArray2 = getIntArray2(BoilerFuelAmountHeader, BoilerFuelAmountLenPos, numTargetCars);
            if (BoilerFuelAmountArray2.Length == 0)
            {
                MessageBox.Show("Error reading file: cannot parse BoilerFuelAmount.");
                nearestBuggy = -1;
                return;
            }

            //--------------------------------------------------------------------------------------------------------
            //BoilerFireTempArray (float)
            BoilerFireTempArray2 = getIntArray2(BoilerFireTempHeader, BoilerFireTempLenPos, numTargetCars);
            if (BoilerFireTempArray2.Length == 0)
            {
                MessageBox.Show("Error reading file: cannot parse BoilerFireTemp.");
                nearestBuggy = -1;
                return;
            }

            //--------------------------------------------------------------------------------------------------------
            //BoilerWaterTempArray (float)
            BoilerWaterTempArray2 = getIntArray2(BoilerWaterTempHeader, BoilerWaterTempLenPos, numTargetCars);
            if (BoilerWaterTempArray2.Length == 0)
            {
                MessageBox.Show("Error reading file: cannot parse BoilerWaterTemp.");
                nearestBuggy = -1;
                return;
            }

            //--------------------------------------------------------------------------------------------------------
            //BoilerWaterLevelArray (float)
            BoilerWaterLevelArray2 = getIntArray2(BoilerWaterLevelHeader, BoilerWaterLevelLenPos, numTargetCars);
            if (BoilerWaterLevelArray2.Length == 0)
            {
                MessageBox.Show("Error reading file: cannot parse BoilerWaterLevel.");
                nearestBuggy = -1;
                return;
            }

            //--------------------------------------------------------------------------------------------------------
            //BoilerPressureArray (float)
            BoilerPressureArray2 = getIntArray2(BoilerPressureHeader, BoilerPressureLenPos, numTargetCars);
            if (BoilerPressureArray2.Length == 0)
            {
                MessageBox.Show("Error reading file: cannot parse BoilerPressure.");
                nearestBuggy = -1;
                return;
            }

            //--------------------------------------------------------------------------------------------------------
            //HeadlightFrontStateArray (bool)
            HeadlightFrontStateArray2 = getBoolArray2(HeadlightFrontStateHeader, HeadlightFrontStateLenPos, numTargetCars);
            if (HeadlightFrontStateArray2.Length == 0)
            {
                MessageBox.Show("Error reading file: cannot parse HeadlightFrontState.");
                nearestBuggy = -1;
                return;
            }

            //--------------------------------------------------------------------------------------------------------
            //HeadlightRearStateArray (bool)
            HeadlightRearStateArray2 = getBoolArray2(HeadlightRearStateHeader, HeadlightRearStateLenPos, numTargetCars);
            if (HeadlightRearStateArray2.Length == 0)
            {
                MessageBox.Show("Error reading file: cannot parse HeadlightRearState.");
                nearestBuggy = -1;
                return;
            }

            //--------------------------------------------------------------------------------------------------------
            //CouplerFrontStateArray (bool)
            CouplerFrontStateArray2 = getBoolArray2(CouplerFrontStateHeader, CouplerFrontStateLenPos, numTargetCars);
            if (CouplerFrontStateArray2.Length == 0)
            {
                MessageBox.Show("Error reading file: cannot parse CouplerFrontState.");
                nearestBuggy = -1;
                return;
            }

            //--------------------------------------------------------------------------------------------------------
            //CouplerRearStateArray (bool)
            CouplerRearStateArray2 = getBoolArray2(CouplerRearStateHeader, CouplerRearStateLenPos, numTargetCars);
            if (CouplerRearStateArray2.Length == 0)
            {
                MessageBox.Show("Error reading file: cannot parse CouplerRearState.");
                nearestBuggy = -1;
                return;
            }

            //--------------------------------------------------------------------------------------------------------
            //TenderFuelAmountArray (float)
            TenderFuelAmountArray2 = getIntArray2(TenderFuelAmountHeader, TenderFuelAmountLenPos, numTargetCars);
            if (TenderFuelAmountArray2.Length == 0)
            {
                MessageBox.Show("Error reading file: cannot parse TenderFuelAmount.");
                nearestBuggy = -1;
                return;
            }

            //--------------------------------------------------------------------------------------------------------
            //TenderWaterAmountArray (float)
            TenderWaterAmountArray2 = getIntArray2(TenderWaterAmountHeader, TenderWaterAmountLenPos, numTargetCars);
            if (TenderWaterAmountArray2.Length == 0)
            {
                MessageBox.Show("Error reading file: cannot parse TenderWaterAmount.");
                nearestBuggy = -1;
                return;
            }


            //--------------------------------------------------------------------------------------------------------
            //CompressorAirPressureArray (float)
            CompressorAirPressureArray2 = getIntArray2(CompressorAirPressureHeader, CompressorAirPressureLenPos, numTargetCars);
            if (CompressorAirPressureArray2.Length == 0)
            {
                MessageBox.Show("Error reading file: cannot parse CompressorAirPressure.");
                nearestBuggy = -1;
                return;
            }


            //--------------------------------------------------------------------------------------------------------
            //MarkerLightsFrontRightStateArray (Int)
            MarkerLightsFrontRightStateArray2 = getIntArray2(MarkerLightsFrontRightStateHeader, MarkerLightsFrontRightStateLenPos, numTargetCars);
            if (MarkerLightsFrontRightStateArray2.Length == 0)
            {
                MessageBox.Show("Error reading file: cannot parse MarkerLightsFrontRightState.");
                nearestBuggy = -1;
                return;
            }

            //--------------------------------------------------------------------------------------------------------
            //MarkerLightsFrontLeftStateArray (Int)
            MarkerLightsFrontLeftStateArray2 = getIntArray2(MarkerLightsFrontLeftStateHeader, MarkerLightsFrontLeftStateLenPos, numTargetCars);
            if (MarkerLightsFrontLeftStateArray2.Length == 0)
            {
                MessageBox.Show("Error reading file: cannot parse MarkerLightsFrontLeftState.");
                nearestBuggy = -1;
                return;
            }

            //--------------------------------------------------------------------------------------------------------
            //MarkerLightsRearRightStateArray (Int)
            MarkerLightsRearRightStateArray2 = getIntArray2(MarkerLightsRearRightStateHeader, MarkerLightsRearRightStateLenPos, numTargetCars);
            if (MarkerLightsRearRightStateArray2.Length == 0)
            {
                MessageBox.Show("Error reading file: cannot parse MarkerLightsRearRightState.");
                nearestBuggy = -1;
                return;
            }

            //--------------------------------------------------------------------------------------------------------
            //MarkerLightsRearLeftStateArray (Int)
            MarkerLightsRearLeftStateArray2 = getIntArray2(MarkerLightsRearLeftStateHeader, MarkerLightsRearLeftStateLenPos, numTargetCars);
            if (MarkerLightsRearLeftStateArray2.Length == 0)
            {
                MessageBox.Show("Error reading file: cannot parse MarkerLightsRearLeftState.");
                nearestBuggy = -1;
                return;
            }

            //--------------------------------------------------------------------------------------------------------
            //MarkerLightsCenterStateArray (Int)
            MarkerLightsCenterStateArray2 = getIntArray2(MarkerLightsCenterStateHeader, MarkerLightsCenterStateLenPos, numTargetCars);
            if (MarkerLightsCenterStateArray2.Length == 0)
            {
                MessageBox.Show("Error reading file: cannot parse MarkerLightsCenterState.");
                nearestBuggy = -1;
                return;
            }

            //--------------------------------------------------------------------------------------------------------
            //FreightAmountArray (Int)
            FreightAmountArray2 = getIntArray2(FreightAmountHeader, FreightAmountLenPos, numTargetCars);
            if (FreightAmountArray2.Length == 0)
            {
                MessageBox.Show("Error reading file: cannot parse FreightAmount.");
                nearestBuggy = -1;
                return;
            }

            //--------------------------------------------------------------------------------------------------------
            //RegulatorValueArray (float)
            RegulatorValueArray2 = getIntArray2(RegulatorValueHeader, RegulatorValueLenPos, numTargetCars);
            if (RegulatorValueArray2.Length == 0)
            {
                MessageBox.Show("Error reading file: cannot parse RegulatorValue.");
                nearestBuggy = -1;
                return;
            }

            //--------------------------------------------------------------------------------------------------------
            //BrakeValueArray (float)
            BrakeValueArray2 = getIntArray2(BrakeValueHeader, BrakeValueLenPos, numTargetCars);
            if (BrakeValueArray2.Length == 0)
            {
                MessageBox.Show("Error reading file: cannot parse BrakeValue.");
                nearestBuggy = -1;
                return;
            }

            //--------------------------------------------------------------------------------------------------------
            //GeneratorValveValueArray (float)
            GeneratorValveValueArray2 = getIntArray2(GeneratorValveValueHeader, GeneratorValveValueLenPos, numTargetCars);
            if (GeneratorValveValueArray2.Length == 0)
            {
                MessageBox.Show("Error reading file: cannot parse GeneratorValveValue.");
                nearestBuggy = -1;
                return;
            }

            //--------------------------------------------------------------------------------------------------------
            //CompressorValveValueArray (float)
            CompressorValveValueArray2 = getIntArray2(CompressorValveValueHeader, CompressorValveValueLenPos, numTargetCars);
            if (CompressorValveValueArray2.Length == 0)
            {
                MessageBox.Show("Error reading file: cannot parse CompressorValveValue.");
                nearestBuggy = -1;
                return;
            }

            //--------------------------------------------------------------------------------------------------------
            //ReverserValueArray (float)
            ReverserValueArray2 = getIntArray2(ReverserValueHeader, ReverserValueLenPos, numTargetCars);
            if (ReverserValueArray2.Length == 0)
            {
                MessageBox.Show("Error reading file: cannot parse ReverserValue.");
                nearestBuggy = -1;
                return;
            }


            //--------------------------------------------------------------------------------------------------------
            //SanderAmountArray (float)
            SanderAmountArray2 = getIntArray2(SanderAmountHeader, SanderAmountLenPos, numTargetCars, true);
            if (SanderAmountArray2.Length == 0)
            {
                MessageBox.Show("Error reading file: cannot parse SanderAmount.");
                nearestBuggy = -1;
                return;
            }

            //showBuggiesOnMap();
            this.Text = "File Loaded. " + checkedListBox2.Items.Count.ToString() + " buggies found.";

            getTreeListFromMapSave();
            parseTracksNow();
        }


        byte[][] getIntArray2(byte[] header, int ign, int size, bool silent = false, bool debug = false)
        {
            byte[][] arrTemp = new byte[size][];
            //find location of trigger text in file, jump ahead by offset, gather each int
            int found = findBytesInBytes2(header, ign);
            if (found == -1)
            {
                if (!silent)
                {
                    MessageBox.Show("Unable to parse this save file. Make sure you dropped a valid save file here.");
                    this.Text = "Invalid File - Try Again";
                    checkedListBox2.Items.Clear();
                }
                return null; //new byte[0][];//an empty array is a problem reading the file and should stop the attempt to read
            }

            //need to construct an array of byte arrays. One byte array (Int32) for each car.
            int offset = header.Length + 4; //do we need an offset from the found position?

            int pos = found + offset;
            for (int i = 0; i < size; i++)
            { //for each car, check to see if there is a name or not
                arrTemp[i] = new byte[4];
                for (int j = 0; j < 4; j++)
                    arrTemp[i][j] = buff2[pos + j];
                if (debug) Debug.WriteLine("Checking @ " + pos.ToString() + " = " + BitConverter.ToSingle(arrTemp[i]));
                pos += 4;
            }
            return arrTemp;
        }


        byte[][] getVectorArray2(byte[] header, int offset, int size)
        {
            int ign = header.Length + 4;//just set ign to greater than the length... we're not searching for the entire header this time
            byte[][] arrTemp = new byte[size][];
            //find location of trigger text in file, jump ahead by offset, gather each int
            int found = findBytesInBytes2(header, ign);
            if (found == -1)
            {
                MessageBox.Show("Unable to parse this save file. Make sure you dropped a valid save file here.");
                this.Text = "Invalid File - Try Again";
                checkedListBox2.Items.Clear();
                return null; //new byte[0][];//an empty array is a problem reading the file and should stop the attempt to read
            }

            int pos = found + offset; //NOTE: the offset is a hardcoded offset from the header start to the data start
            for (int i = 0; i < size; i++)
            { //for each car, check to see if there is a name or not
                arrTemp[i] = new byte[12];
                for (int j = 0; j < 12; j++)
                    arrTemp[i][j] = buff2[pos + j];
                //if (debug) Debug.WriteLine("Checking @ " + pos.ToString() + " = " + BitConverter.ToSingle(arrTemp[i]));
                pos += 12;
            }
            return arrTemp;
        }

        byte[] getBoolArray2(byte[] header, int ign, int size, bool debug = false)
        {
            byte[] arrTemp = new byte[size];
            //find location of trigger text in file, jump ahead by offset, gather each int
            int found = findBytesInBytes2(header, ign);
            if (found == -1)
            {
                //MessageBox.Show("Unable to parse this save file. Make sure you dropped a valid save file here.");
                this.Text = "Invalid File - Try Again";
                checkedListBox2.Items.Clear();
                return null; //new byte[0][];//an empty array is a problem reading the file and should stop the attempt to read
            }

            //need to construct an array2. One byte for each car. 
            int offset = header.Length + 4; //do we need an offset from the found position?

            int pos = found + offset;
            for (int i = 0; i < size; i++)
            { //for each car, check to see if there is a name or not
                arrTemp[i] = buff2[pos];
                if (debug) Debug.WriteLine("Checking @ " + pos.ToString() + " = " + (arrTemp[i] == 1).ToString());
                pos++;
            }
            return arrTemp;
        }


        //Get a string array, note we don't need to have actual strings, just the bytes to be put back into the file
        //IF STRING PRESENT: 02 00 00 00 FF 01 00 00 00 (len + 1) 00 00 00 (stringchars 00)
        //IF NO STRING:      00 00 00 00 FF 00 00 00 00 
        byte[][] getStringArray2(byte[] header, int ign)
        {
            byte[][] arrTemp = new byte[numTargetCars][];
            //find location of trigger text in file, jump ahead by offset, gather each string
            int found = findBytesInBytes2(header, ign);
            if (found == -1)
            {
                //MessageBox.Show("Unable to parse this save file. Make sure you dropped a valid save file here.");
                this.Text = "Invalid File - Try Again";
                checkedListBox2.Items.Clear();
                return null; //new byte[0][];//an empty array is a problem reading the file and should stop the attempt to read
            }

            //need to construct an array of byte arrays. One byte array ("string") for each car.
            int offset = header.Length + 4; //do we need an offset from the found position?

            int pos = found + offset;
            for (int i = 0; i < numTargetCars; i++)
            { //for each car, check to see if there is a name or not
                Debug.WriteLine("Checking @ " + pos.ToString());
                int strlen = Convert.ToInt32(buff2[pos]);
                if (strlen == 0)
                {
                    arrTemp[i] = new byte[] { 0x00, 0x00, 0x00, 0x00, 0xFF, 0x00, 0x00, 0x00, 0x00 };
                    pos += 9;
                }
                else //this one has a string associated with it
                {
                    // check for a specific type of corrupted data that came with new saves in the beta:
                    if (Convert.ToInt32(buff2[pos]) == 1)
                    {
                        MessageBox.Show("Note: Possible Name corruption (buggy number " + i.ToString() + " of " + numTargetCars.ToString() + ") detected in save. If this operation fails you should load a copy of this save in ROSE to rename the buggies.\nLook for one that might have a <br> or other odd characters in it, or count in the list up from zero to the number supplied earlier.");
                        //find the next instance of "FF" and then back up 4 spaces:
                        for (int k = pos; k < buff2.Length; k++)
                        {
                            if (buff2[k] == 255)
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
                        int tlen = Convert.ToInt32(buff2[pos + 9]); //length of the string and trailer
                        arrTemp[i] = new byte[13 + tlen];
                        for (int j = 0; j < 13 + tlen; j++)
                            arrTemp[i][j] = buff2[pos + j];
                        pos += 13 + tlen;
                    }


                }

                //Debug.WriteLine(Encoding.ASCII.GetString(arrTemp[i]));



            }
            return arrTemp;
        }


        byte[][] getCargoArray2(byte[] header, int ign)
        {
            byte[][] arrTemp = new byte[numTargetCars][];
            //find location of trigger text in file, jump ahead by offset, gather each string
            int found = findBytesInBytes2(header, ign);
            if (found == -1)
            {
                //MessageBox.Show("Unable to parse this save file. Make sure you dropped a valid save file here.");
                this.Text = "Invalid File - Try Again";
                checkedListBox2.Items.Clear();
                return null; //new byte[0][];//an empty array is a problem reading the file and should stop the attempt to read
            }

            //need to construct an array of byte arrays. One byte array ("string") for each car.
            int offset = header.Length + 4; //do we need an offset from the found position?

            int pos = found + offset;
            for (int i = 0; i < numTargetCars; i++)
            { //for each car, check to see if there is a name or not
                //Debug.WriteLine("Checking @ " + pos.ToString());
                int strlen = Convert.ToInt32(buff2[pos]);
                if (strlen == 0)
                {
                    arrTemp[i] = new byte[] { 0x00, 0x00, 0x00, 0x00 };
                    pos += 4;
                }
                else //this one has a string associated with it
                {
                    //get the length of the string and all:
                    int tlen = Convert.ToInt32(buff2[pos]); //length of the string and trailer
                    arrTemp[i] = new byte[4 + tlen];
                    for (int j = 0; j < 4 + tlen; j++)
                        arrTemp[i][j] = buff2[pos + j];
                    pos += 4 + tlen;
                }

                //Debug.WriteLine(Encoding.ASCII.GetString(arrTemp[i]));



            }
            return arrTemp;
        }


        int findInBytes2(string pattern)
        {
            byte[] findme = Encoding.ASCII.GetBytes(pattern);
            //go through buff until we get to findme:
            int i = 0;
            int j = 0;
            while (i < buff2.Length)
            {
                if (buff2[i] == findme[j])
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

        //finds a string of bytes wtthin the file's bytes
        //we need to ignore the one section (4 bytes) in the header where it stores the length, because this will vary from file to file
        int findBytesInBytes2(byte[] findme, int lengthPos)
        {
            //go through buff until we get to findme:
            int i = 0;
            int j = 0;
            while (i < buff2.Length)
            {
                if (j >= lengthPos && j < lengthPos + 4)
                {
                    j++; //don't test these 4 bytes because they are different in each save
                }
                else
                {
                    if (buff2[i] == findme[j])
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
    }
}