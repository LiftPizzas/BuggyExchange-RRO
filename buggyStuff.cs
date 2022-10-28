using System.Text;
using System.Windows.Forms;
using System.Linq;

namespace BuggyExchange
{
    public partial class Form1 : Form
    {


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
            buggyDrawX = new float[numSourceCars];
            buggyDrawY = new float[numSourceCars];
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

            FrameLocationArray = getVectorArray(Encoding.ASCII.GetBytes("FrameLocationArray"), 147);

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

            FrameRotationArray = getVectorArray(Encoding.ASCII.GetBytes("FrameRotationArray"), 148);

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

            scaleChanged();
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


    }
}