using System.Text;
using System.Windows.Forms;
using System.Linq;

namespace BuggyExchange
{
    public partial class Form1 : Form
    {
        private void button1_Click(object sender, EventArgs e)
        {
            //User clicked to Add Buggies.
            //We need to write out the beginning and end of the file, inserting our altered list into it.

            bool dupeAnswered = false;
            bool keepDupes = false;
            //find any possible duplicates and handle them
            for (int i = 0; i < numSourceCars; i++)
                if (checkedListBox1.GetItemChecked(i) == true)
                {
                    //check for duplicates (by locations of less than 1 unit)
                    for (int j = 0; j < numTargetCars; j++)
                    {
                        float distX = framePositions2X[j] - framePositionsX[i];
                        float distY = framePositions2Y[j] - framePositionsY[i];
                        float distZ = framePositions2Z[j] - framePositionsZ[i];
                        double dist = Math.Sqrt(distX * distX + distY * distY + distZ * distZ);
                        if (dist < 2d)
                        {
                            if (!dupeAnswered)
                            {
                                DialogResult res = MessageBox.Show("Possible duplicate(s) found, do you want to add/keep buggies that are overlapping each other?", "Keep duplicates?", MessageBoxButtons.YesNoCancel);
                                if (res == DialogResult.Cancel) return;
                                dupeAnswered = true;
                                if (res == DialogResult.Yes) keepDupes = true;
                                if (res == DialogResult.No) keepDupes = false;
                            }

                            if (!keepDupes) //remove the duplicate: keep the one from the map file and uncheck the one from the add file
                                checkedListBox1.SetItemChecked(i, false);
                        }
                    }
                }

            //find out how many we have in total, we will need this number many times
            int numChecked = 0;
            for (int i = 0; i < numSourceCars; i++)
                if (checkedListBox1.GetItemChecked(i) == true)
                    numChecked++;


            if (numChecked == 0)
            {
                MessageBox.Show("There are no changes to make.", "Nothing happened.");
                return;
            }

            //alter numchecked because we now have the two combined as our total:
            numChecked += numTargetCars;

            //construct the new buggy segment of the file

            List<byte> outputBytes = new List<byte>(); //the entire output chunk for all cars, all arrays

            List<byte> tB = new List<byte>(); //the entire output chunk for this one array (for all cars) minus the header
            byte[] b;

            //add all target cars:
            for (int i = 0; i < numTargetCars; i++) //FrameTypeArray;
            {//collect all the frametypes from the ones checked in the list
                b = BitConverter.GetBytes(frameTypes2[i].Length);
                for (int k = 0; k < 4; k++) tB.Add(b[k]); //string length

                b = Encoding.ASCII.GetBytes(frameTypes2[i]);
                for (int j = 0; j < frameTypes2[i].Length; j++) tB.Add(b[j]); //string itself with a trailing zero
            }

            for (int i = 0; i < numSourceCars; i++) //FrameTypeArray;
            {//collect all the frametypes from the ones checked in the list

                if (checkedListBox1.GetItemChecked(i))
                {
                    b = BitConverter.GetBytes(frameTypes[i].Length);
                    for (int k = 0; k < 4; k++) tB.Add(b[k]); //string length

                    b = Encoding.ASCII.GetBytes(frameTypes[i]);
                    for (int j = 0; j < frameTypes[i].Length; j++) tB.Add(b[j]); //string itself with a trailing zero
                }
            }
            //now that we have the whole name block, we need to get its length and inject that into the header in the correct spot
            b = BitConverter.GetBytes(tB.Count + 4);
            for (int k = 0; k < 4; k++) FrameTypeHeader[FrameTypeLenPos + k] = b[k];

            for (int i = 0; i < FrameTypeHeader.Length; i++)
                outputBytes.Add(FrameTypeHeader[i]); //header is only added once for each segment

            b = BitConverter.GetBytes(numChecked); //next is the number of buggies:
            for (int k = 0; k < 4; k++) outputBytes.Add(b[k]);

            for (int i = 0; i < tB.Count; i++)//add this array to the output block
                outputBytes.Add(tB[i]);

            //---------------------------------------------------------------------------------------------------

            tB = new List<byte>(); //the entire output chunk for this one array (for all cars) minus the header

            for (int i = 0; i < numTargetCars; i++) //FrameLocationArray;
            {//collect all the locations from the ones checked in the list
                for (int k = 0; k < 12; k++) tB.Add(FrameLocationArray2[i][k]); //Each vector is 12 bytes long
            }

            for (int i = 0; i < numSourceCars; i++) //FrameLocationArray;
            {//collect all the locations from the ones checked in the list
                if (checkedListBox1.GetItemChecked(i))
                    for (int k = 0; k < 12; k++) tB.Add(FrameLocationArray[i][k]); //Each vector is 12 bytes long
            }
            //now that we have the whole vector block, we need to get its length and inject that into the header in the correct 2 spots:
            //(offset 115) #of cars * 12
            //(offset 41) 28 + #of cars * 12
            b = BitConverter.GetBytes(tB.Count);
            for (int k = 0; k < 4; k++) FrameLocationHeader[115 + k] = b[k];

            b = BitConverter.GetBytes(tB.Count + 82);
            for (int k = 0; k < 4; k++) FrameLocationHeader[41 + k] = b[k];

            b = BitConverter.GetBytes(numChecked); //next is the number of buggies, this is inside the header for location and rotation arrays
            for (int k = 0; k < 4; k++) FrameLocationHeader[69 + k] = b[k];

            for (int i = 0; i < FrameLocationHeader.Length; i++)
                outputBytes.Add(FrameLocationHeader[i]); //header is only added once for each segment

            for (int i = 0; i < tB.Count; i++)//add this array to the output block
                outputBytes.Add(tB[i]);

            //---------------------------------------------------------------------------------------------------

            tB = new List<byte>(); //the entire output chunk for this one array (for all cars) minus the header

            for (int i = 0; i < numTargetCars; i++) //FrameRotationArray;
            {//collect all the Rotations from the ones checked in the list
                for (int k = 0; k < 12; k++) tB.Add(FrameRotationArray2[i][k]); //Each vector is 12 bytes long
            }
            for (int i = 0; i < numSourceCars; i++) //FrameRotationArray;
            {//collect all the Rotations from the ones checked in the list
                if (checkedListBox1.GetItemChecked(i))
                    for (int k = 0; k < 12; k++) tB.Add(FrameRotationArray[i][k]); //Each vector is 12 bytes long
            }
            //now that we have the whole vector block, we need to get its length and inject that into the header in the correct 2 spots:
            //(offset 115) #of cars * 12
            //(offset 41) 28 + #of cars * 12
            b = BitConverter.GetBytes(tB.Count);
            for (int k = 0; k < 4; k++) FrameRotationHeader[115 + k] = b[k];

            b = BitConverter.GetBytes(tB.Count + 83);
            for (int k = 0; k < 4; k++) FrameRotationHeader[41 + k] = b[k];

            b = BitConverter.GetBytes(numChecked); //next is the number of buggies, this is inside the header for location and rotation arrays
            for (int k = 0; k < 4; k++) FrameRotationHeader[69 + k] = b[k];

            for (int i = 0; i < FrameRotationHeader.Length; i++)
                outputBytes.Add(FrameRotationHeader[i]); //header is only added once for each segment

            for (int i = 0; i < tB.Count; i++)//add this array to the output block
                outputBytes.Add(tB[i]);

            //---------------------------------------------------------------------------------------------------





            tB = new List<byte>(); //the entire output chunk for this one array (for all cars) minus the header

            for (int i = 0; i < numTargetCars; i++) //FrameNumberArray;
                for (int j = 0; j < FrameNumberArray2[i].Length; j++) tB.Add(FrameNumberArray2[i][j]); //string itself with a trailing zero

            for (int i = 0; i < numSourceCars; i++) //FrameNumberArray;
            {//collect all the frametypes from the ones checked in the list

                if (checkedListBox1.GetItemChecked(i))
                {
                    //b = BitConverter.GetBytes(FrameNumberArray[i].Length);
                    //for (int k = 0; k < 4; k++) tB.Add(b[k]); //string length


                    for (int j = 0; j < FrameNumberArray[i].Length; j++) tB.Add(FrameNumberArray[i][j]); //string itself with a trailing zero
                }
            }
            //now that we have the whole name block, we need to get its length and inject that into the header in the correct spot
            b = BitConverter.GetBytes(tB.Count + 4);
            for (int k = 0; k < 4; k++) FrameNumberHeader[FrameNumberLenPos + k] = b[k];

            for (int i = 0; i < FrameNumberHeader.Length; i++)
                outputBytes.Add(FrameNumberHeader[i]); //header is only added once for each segment

            b = BitConverter.GetBytes(numChecked); //next is the number of buggies:
            for (int k = 0; k < 4; k++) outputBytes.Add(b[k]);

            for (int i = 0; i < tB.Count; i++)//add this array to the output block
                outputBytes.Add(tB[i]);

            //---------------------------------------------------------------------------------------------------

            tB = new List<byte>(); //the entire output chunk for this one array (for all cars) minus the header

            for (int i = 0; i < numTargetCars; i++) //FrameNameArray;
                for (int j = 0; j < FrameNameArray2[i].Length; j++) tB.Add(FrameNameArray2[i][j]); //string itself with a trailing zero

            for (int i = 0; i < numSourceCars; i++) //FrameNameArray;
            {//collect all the frametypes from the ones checked in the list

                if (checkedListBox1.GetItemChecked(i))
                {
                    //b = BitConverter.GetBytes(FrameNameArray[i].Length);
                    //for (int k = 0; k < 4; k++) tB.Add(b[k]); //string length


                    for (int j = 0; j < FrameNameArray[i].Length; j++) tB.Add(FrameNameArray[i][j]); //string itself with a trailing zero
                }
            }
            //now that we have the whole name block, we need to get its length and inject that into the header in the correct spot
            b = BitConverter.GetBytes(tB.Count + 4);
            for (int k = 0; k < 4; k++) FrameNameHeader[FrameNameLenPos + k] = b[k];

            for (int i = 0; i < FrameNameHeader.Length; i++)
                outputBytes.Add(FrameNameHeader[i]); //header is only added once for each segment

            b = BitConverter.GetBytes(numChecked); //next is the number of buggies:
            for (int k = 0; k < 4; k++) outputBytes.Add(b[k]);

            for (int i = 0; i < tB.Count; i++)//add this array to the output block
                outputBytes.Add(tB[i]);

            //---------------------------------------------------------------------------------------------------


            tB = new List<byte>(); //the entire output chunk for this one array (for all cars) minus the header

            for (int i = 0; i < numTargetCars; i++) //SmokestackTypeArray;
                for (int j = 0; j < SmokestackTypeArray2[i].Length; j++) tB.Add(SmokestackTypeArray2[i][j]);

            for (int i = 0; i < numSourceCars; i++) //SmokestackTypeArray;
            {//collect all the frametypes from the ones checked in the list

                if (checkedListBox1.GetItemChecked(i))
                    for (int j = 0; j < SmokestackTypeArray[i].Length; j++) tB.Add(SmokestackTypeArray[i][j]);
            }
            //now that we have the whole block, we need to get its length and inject that into the header in the correct spot
            b = BitConverter.GetBytes(tB.Count + 4);
            for (int k = 0; k < 4; k++) SmokestackTypeHeader[SmokestackTypeLenPos + k] = b[k];

            for (int i = 0; i < SmokestackTypeHeader.Length; i++)
                outputBytes.Add(SmokestackTypeHeader[i]); //header is only added once for each segment

            b = BitConverter.GetBytes(numChecked); //next is the number of buggies:
            for (int k = 0; k < 4; k++) outputBytes.Add(b[k]);

            for (int i = 0; i < tB.Count; i++)//add this array to the output block
                outputBytes.Add(tB[i]);

            //---------------------------------------------------------------------------------------------------



            tB = new List<byte>(); //the entire output chunk for this one array (for all cars) minus the header

            for (int i = 0; i < numTargetCars; i++) //HeadlightTypeArray;
                for (int j = 0; j < HeadlightTypeArray2[i].Length; j++) tB.Add(HeadlightTypeArray2[i][j]);

            for (int i = 0; i < numSourceCars; i++) //HeadlightTypeArray;
            {//collect all the frametypes from the ones checked in the list

                if (checkedListBox1.GetItemChecked(i))
                    for (int j = 0; j < HeadlightTypeArray[i].Length; j++) tB.Add(HeadlightTypeArray[i][j]);
            }
            //now that we have the whole block, we need to get its length and inject that into the header in the correct spot
            b = BitConverter.GetBytes(tB.Count + 4);
            for (int k = 0; k < 4; k++) HeadlightTypeHeader[HeadlightTypeLenPos + k] = b[k];

            for (int i = 0; i < HeadlightTypeHeader.Length; i++)
                outputBytes.Add(HeadlightTypeHeader[i]); //header is only added once for each segment

            b = BitConverter.GetBytes(numChecked); //next is the number of buggies:
            for (int k = 0; k < 4; k++) outputBytes.Add(b[k]);

            for (int i = 0; i < tB.Count; i++)//add this array to the output block
                outputBytes.Add(tB[i]);

            //---------------------------------------------------------------------------------------------------


            tB = new List<byte>(); //the entire output chunk for this one array (for all cars) minus the header

            for (int i = 0; i < numTargetCars; i++) //PaintTypeArray;
                for (int j = 0; j < PaintTypeArray2[i].Length; j++) tB.Add(PaintTypeArray2[i][j]);

            for (int i = 0; i < numSourceCars; i++) //PaintTypeArray;
            {//collect all the frametypes from the ones checked in the list

                if (checkedListBox1.GetItemChecked(i))
                    for (int j = 0; j < PaintTypeArray[i].Length; j++) tB.Add(PaintTypeArray[i][j]);
            }
            //now that we have the whole block, we need to get its length and inject that into the header in the correct spot
            b = BitConverter.GetBytes(tB.Count + 4);
            for (int k = 0; k < 4; k++) PaintTypeHeader[PaintTypeLenPos + k] = b[k];

            for (int i = 0; i < PaintTypeHeader.Length; i++)
                outputBytes.Add(PaintTypeHeader[i]); //header is only added once for each segment

            b = BitConverter.GetBytes(numChecked); //next is the number of buggies:
            for (int k = 0; k < 4; k++) outputBytes.Add(b[k]);

            for (int i = 0; i < tB.Count; i++)//add this array to the output block
                outputBytes.Add(tB[i]);

            //---------------------------------------------------------------------------------------------------


            tB = new List<byte>(); //the entire output chunk for this one array (for all cars) minus the header

            for (int i = 0; i < numTargetCars; i++) //BoilerFuelAmountArray;
                for (int j = 0; j < BoilerFuelAmountArray2[i].Length; j++) tB.Add(BoilerFuelAmountArray2[i][j]);

            for (int i = 0; i < numSourceCars; i++) //BoilerFuelAmountArray;
            {//collect all the frametypes from the ones checked in the list

                if (checkedListBox1.GetItemChecked(i))
                    for (int j = 0; j < BoilerFuelAmountArray[i].Length; j++) tB.Add(BoilerFuelAmountArray[i][j]);
            }
            //now that we have the whole block, we need to get its length and inject that into the header in the correct spot
            b = BitConverter.GetBytes(tB.Count + 4);
            for (int k = 0; k < 4; k++) BoilerFuelAmountHeader[BoilerFuelAmountLenPos + k] = b[k];

            for (int i = 0; i < BoilerFuelAmountHeader.Length; i++)
                outputBytes.Add(BoilerFuelAmountHeader[i]); //header is only added once for each segment

            b = BitConverter.GetBytes(numChecked); //next is the number of buggies:
            for (int k = 0; k < 4; k++) outputBytes.Add(b[k]);

            for (int i = 0; i < tB.Count; i++)//add this array to the output block
                outputBytes.Add(tB[i]);

            //---------------------------------------------------------------------------------------------------



            tB = new List<byte>(); //the entire output chunk for this one array (for all cars) minus the header

            for (int i = 0; i < numTargetCars; i++) //BoilerFireTempArray;
                for (int j = 0; j < BoilerFireTempArray2[i].Length; j++) tB.Add(BoilerFireTempArray2[i][j]);

            for (int i = 0; i < numSourceCars; i++) //BoilerFireTempArray;
            {//collect all the frametypes from the ones checked in the list

                if (checkedListBox1.GetItemChecked(i))
                    for (int j = 0; j < BoilerFireTempArray[i].Length; j++) tB.Add(BoilerFireTempArray[i][j]);
            }
            //now that we have the whole block, we need to get its length and inject that into the header in the correct spot
            b = BitConverter.GetBytes(tB.Count + 4);
            for (int k = 0; k < 4; k++) BoilerFireTempHeader[BoilerFireTempLenPos + k] = b[k];

            for (int i = 0; i < BoilerFireTempHeader.Length; i++)
                outputBytes.Add(BoilerFireTempHeader[i]); //header is only added once for each segment

            b = BitConverter.GetBytes(numChecked); //next is the number of buggies:
            for (int k = 0; k < 4; k++) outputBytes.Add(b[k]);

            for (int i = 0; i < tB.Count; i++)//add this array to the output block
                outputBytes.Add(tB[i]);

            //---------------------------------------------------------------------------------------------------


            tB = new List<byte>(); //the entire output chunk for this one array (for all cars) minus the header

            for (int i = 0; i < numTargetCars; i++) //BoilerWaterTempArray;            
                for (int j = 0; j < BoilerWaterTempArray2[i].Length; j++) tB.Add(BoilerWaterTempArray2[i][j]);

            for (int i = 0; i < numSourceCars; i++) //BoilerWaterTempArray;
            {//collect all the frametypes from the ones checked in the list

                if (checkedListBox1.GetItemChecked(i))
                    for (int j = 0; j < BoilerWaterTempArray[i].Length; j++) tB.Add(BoilerWaterTempArray[i][j]);
            }
            //now that we have the whole block, we need to get its length and inject that into the header in the correct spot
            b = BitConverter.GetBytes(tB.Count + 4);
            for (int k = 0; k < 4; k++) BoilerWaterTempHeader[BoilerWaterTempLenPos + k] = b[k];

            for (int i = 0; i < BoilerWaterTempHeader.Length; i++)
                outputBytes.Add(BoilerWaterTempHeader[i]); //header is only added once for each segment

            b = BitConverter.GetBytes(numChecked); //next is the number of buggies:
            for (int k = 0; k < 4; k++) outputBytes.Add(b[k]);

            for (int i = 0; i < tB.Count; i++)//add this array to the output block
                outputBytes.Add(tB[i]);

            //---------------------------------------------------------------------------------------------------



            tB = new List<byte>(); //the entire output chunk for this one array (for all cars) minus the header

            for (int i = 0; i < numTargetCars; i++) //BoilerWaterLevelArray;
                for (int j = 0; j < BoilerWaterLevelArray2[i].Length; j++) tB.Add(BoilerWaterLevelArray2[i][j]);

            for (int i = 0; i < numSourceCars; i++) //BoilerWaterLevelArray;
            {//collect all the frametypes from the ones checked in the list

                if (checkedListBox1.GetItemChecked(i))
                    for (int j = 0; j < BoilerWaterLevelArray[i].Length; j++) tB.Add(BoilerWaterLevelArray[i][j]);
            }
            //now that we have the whole block, we need to get its length and inject that into the header in the correct spot
            b = BitConverter.GetBytes(tB.Count + 4);
            for (int k = 0; k < 4; k++) BoilerWaterLevelHeader[BoilerWaterLevelLenPos + k] = b[k];

            for (int i = 0; i < BoilerWaterLevelHeader.Length; i++)
                outputBytes.Add(BoilerWaterLevelHeader[i]); //header is only added once for each segment

            b = BitConverter.GetBytes(numChecked); //next is the number of buggies:
            for (int k = 0; k < 4; k++) outputBytes.Add(b[k]);

            for (int i = 0; i < tB.Count; i++)//add this array to the output block
                outputBytes.Add(tB[i]);

            //---------------------------------------------------------------------------------------------------




            tB = new List<byte>(); //the entire output chunk for this one array (for all cars) minus the header

            for (int i = 0; i < numTargetCars; i++) //BoilerPressureArray;
                for (int j = 0; j < BoilerPressureArray2[i].Length; j++) tB.Add(BoilerPressureArray2[i][j]);

            for (int i = 0; i < numSourceCars; i++) //BoilerPressureArray;
            {//collect all the frametypes from the ones checked in the list

                if (checkedListBox1.GetItemChecked(i))
                    for (int j = 0; j < BoilerPressureArray[i].Length; j++) tB.Add(BoilerPressureArray[i][j]);
            }
            //now that we have the whole block, we need to get its length and inject that into the header in the correct spot
            b = BitConverter.GetBytes(tB.Count + 4);
            for (int k = 0; k < 4; k++) BoilerPressureHeader[BoilerPressureLenPos + k] = b[k];

            for (int i = 0; i < BoilerPressureHeader.Length; i++)
                outputBytes.Add(BoilerPressureHeader[i]); //header is only added once for each segment

            b = BitConverter.GetBytes(numChecked); //next is the number of buggies:
            for (int k = 0; k < 4; k++) outputBytes.Add(b[k]);

            for (int i = 0; i < tB.Count; i++)//add this array to the output block
                outputBytes.Add(tB[i]);

            //---------------------------------------------------------------------------------------------------




            tB = new List<byte>(); //the entire output chunk for this one array (for all cars) minus the header


            for (int i = 0; i < numTargetCars; i++) //HeadlightFrontStateArray;
                tB.Add(HeadlightFrontStateArray2[i]);

            for (int i = 0; i < numSourceCars; i++) //HeadlightFrontStateArray;
            {//collect all the frametypes from the ones checked in the list

                if (checkedListBox1.GetItemChecked(i))
                    tB.Add(HeadlightFrontStateArray[i]);
            }
            //now that we have the whole block, we need to get its length and inject that into the header in the correct spot
            b = BitConverter.GetBytes(tB.Count + 4);
            for (int k = 0; k < 4; k++) HeadlightFrontStateHeader[HeadlightFrontStateLenPos + k] = b[k];

            for (int i = 0; i < HeadlightFrontStateHeader.Length; i++)
                outputBytes.Add(HeadlightFrontStateHeader[i]); //header is only added once for each segment

            b = BitConverter.GetBytes(numChecked); //next is the number of buggies:
            for (int k = 0; k < 4; k++) outputBytes.Add(b[k]);

            for (int i = 0; i < tB.Count; i++)//add this array to the output block
                outputBytes.Add(tB[i]);

            //---------------------------------------------------------------------------------------------------



            tB = new List<byte>(); //the entire output chunk for this one array (for all cars) minus the header

            for (int i = 0; i < numTargetCars; i++) //HeadlightRearStateArray;
                tB.Add(HeadlightRearStateArray2[i]);

            for (int i = 0; i < numSourceCars; i++) //HeadlightRearStateArray;
            {//collect all the frametypes from the ones checked in the list

                if (checkedListBox1.GetItemChecked(i))
                    tB.Add(HeadlightRearStateArray[i]);
            }
            //now that we have the whole block, we need to get its length and inject that into the header in the correct spot
            b = BitConverter.GetBytes(tB.Count + 4);
            for (int k = 0; k < 4; k++) HeadlightRearStateHeader[HeadlightRearStateLenPos + k] = b[k];

            for (int i = 0; i < HeadlightRearStateHeader.Length; i++)
                outputBytes.Add(HeadlightRearStateHeader[i]); //header is only added once for each segment

            b = BitConverter.GetBytes(numChecked); //next is the number of buggies:
            for (int k = 0; k < 4; k++) outputBytes.Add(b[k]);

            for (int i = 0; i < tB.Count; i++)//add this array to the output block
                outputBytes.Add(tB[i]);

            //---------------------------------------------------------------------------------------------------


            tB = new List<byte>(); //the entire output chunk for this one array (for all cars) minus the header

            for (int i = 0; i < numTargetCars; i++) //CouplerFrontStateArray;
                tB.Add(CouplerFrontStateArray2[i]);

            //now that we have the whole block, we need to get its length and inject that into the header in the correct spot
            b = BitConverter.GetBytes(tB.Count + 4);
            for (int k = 0; k < 4; k++) CouplerFrontStateHeader[CouplerFrontStateLenPos + k] = b[k];

            for (int i = 0; i < CouplerFrontStateHeader.Length; i++)
                outputBytes.Add(CouplerFrontStateHeader[i]); //header is only added once for each segment

            b = BitConverter.GetBytes(numChecked); //next is the number of buggies:
            for (int k = 0; k < 4; k++) outputBytes.Add(b[k]);

            for (int i = 0; i < tB.Count; i++)//add this array to the output block
                outputBytes.Add(tB[i]);

            //---------------------------------------------------------------------------------------------------





            tB = new List<byte>(); //the entire output chunk for this one array (for all cars) minus the header

            for (int i = 0; i < numTargetCars; i++) //CouplerRearStateArray;
                tB.Add(CouplerRearStateArray2[i]);

            for (int i = 0; i < numSourceCars; i++) //CouplerRearStateArray;
            {//collect all the frametypes from the ones checked in the list

                if (checkedListBox1.GetItemChecked(i))
                    tB.Add(CouplerRearStateArray[i]);
            }
            //now that we have the whole block, we need to get its length and inject that into the header in the correct spot
            b = BitConverter.GetBytes(tB.Count + 4);
            for (int k = 0; k < 4; k++) CouplerRearStateHeader[CouplerRearStateLenPos + k] = b[k];

            for (int i = 0; i < CouplerRearStateHeader.Length; i++)
                outputBytes.Add(CouplerRearStateHeader[i]); //header is only added once for each segment

            b = BitConverter.GetBytes(numChecked); //next is the number of buggies:
            for (int k = 0; k < 4; k++) outputBytes.Add(b[k]);

            for (int i = 0; i < tB.Count; i++)//add this array to the output block
                outputBytes.Add(tB[i]);

            //---------------------------------------------------------------------------------------------------



            tB = new List<byte>(); //the entire output chunk for this one array (for all cars) minus the header

            for (int i = 0; i < numTargetCars; i++) //TenderFuelAmountArray;
                for (int j = 0; j < TenderFuelAmountArray2[i].Length; j++) tB.Add(TenderFuelAmountArray2[i][j]);

            for (int i = 0; i < numSourceCars; i++) //TenderFuelAmountArray;
            {//collect all the frametypes from the ones checked in the list

                if (checkedListBox1.GetItemChecked(i))
                    for (int j = 0; j < TenderFuelAmountArray[i].Length; j++) tB.Add(TenderFuelAmountArray[i][j]);
            }
            //now that we have the whole block, we need to get its length and inject that into the header in the correct spot
            b = BitConverter.GetBytes(tB.Count + 4);
            for (int k = 0; k < 4; k++) TenderFuelAmountHeader[TenderFuelAmountLenPos + k] = b[k];

            for (int i = 0; i < TenderFuelAmountHeader.Length; i++)
                outputBytes.Add(TenderFuelAmountHeader[i]); //header is only added once for each segment

            b = BitConverter.GetBytes(numChecked); //next is the number of buggies:
            for (int k = 0; k < 4; k++) outputBytes.Add(b[k]);

            for (int i = 0; i < tB.Count; i++)//add this array to the output block
                outputBytes.Add(tB[i]);

            //---------------------------------------------------------------------------------------------------




            tB = new List<byte>(); //the entire output chunk for this one array (for all cars) minus the header

            for (int i = 0; i < numTargetCars; i++) //TenderWaterAmountArray;
                for (int j = 0; j < TenderWaterAmountArray2[i].Length; j++) tB.Add(TenderWaterAmountArray2[i][j]);

            for (int i = 0; i < numSourceCars; i++) //TenderWaterAmountArray;
            {//collect all the frametypes from the ones checked in the list

                if (checkedListBox1.GetItemChecked(i))
                    for (int j = 0; j < TenderWaterAmountArray[i].Length; j++) tB.Add(TenderWaterAmountArray[i][j]);
            }
            //now that we have the whole block, we need to get its length and inject that into the header in the correct spot
            b = BitConverter.GetBytes(tB.Count + 4);
            for (int k = 0; k < 4; k++) TenderWaterAmountHeader[TenderWaterAmountLenPos + k] = b[k];

            for (int i = 0; i < TenderWaterAmountHeader.Length; i++)
                outputBytes.Add(TenderWaterAmountHeader[i]); //header is only added once for each segment

            b = BitConverter.GetBytes(numChecked); //next is the number of buggies:
            for (int k = 0; k < 4; k++) outputBytes.Add(b[k]);

            for (int i = 0; i < tB.Count; i++)//add this array to the output block
                outputBytes.Add(tB[i]);

            //---------------------------------------------------------------------------------------------------


            tB = new List<byte>(); //the entire output chunk for this one array (for all cars) minus the header

            for (int i = 0; i < numTargetCars; i++) //CompressorAirPressureArray;
                for (int j = 0; j < CompressorAirPressureArray2[i].Length; j++) tB.Add(CompressorAirPressureArray2[i][j]);

            for (int i = 0; i < numSourceCars; i++) //CompressorAirPressureArray;
            {//collect all the frametypes from the ones checked in the list

                if (checkedListBox1.GetItemChecked(i))
                    for (int j = 0; j < CompressorAirPressureArray[i].Length; j++) tB.Add(CompressorAirPressureArray[i][j]);
            }
            //now that we have the whole block, we need to get its length and inject that into the header in the correct spot
            b = BitConverter.GetBytes(tB.Count + 4);
            for (int k = 0; k < 4; k++) CompressorAirPressureHeader[CompressorAirPressureLenPos + k] = b[k];

            for (int i = 0; i < CompressorAirPressureHeader.Length; i++)
                outputBytes.Add(CompressorAirPressureHeader[i]); //header is only added once for each segment

            b = BitConverter.GetBytes(numChecked); //next is the number of buggies:
            for (int k = 0; k < 4; k++) outputBytes.Add(b[k]);

            for (int i = 0; i < tB.Count; i++)//add this array to the output block
                outputBytes.Add(tB[i]);

            //---------------------------------------------------------------------------------------------------


            tB = new List<byte>(); //the entire output chunk for this one array (for all cars) minus the header

            for (int i = 0; i < numTargetCars; i++) //MarkerLightsFrontRightStateArray;
                for (int j = 0; j < MarkerLightsFrontRightStateArray2[i].Length; j++) tB.Add(MarkerLightsFrontRightStateArray2[i][j]);

            for (int i = 0; i < numSourceCars; i++) //MarkerLightsFrontRightStateArray;
            {//collect all the frametypes from the ones checked in the list

                if (checkedListBox1.GetItemChecked(i))
                    for (int j = 0; j < MarkerLightsFrontRightStateArray[i].Length; j++) tB.Add(MarkerLightsFrontRightStateArray[i][j]);
            }
            //now that we have the whole block, we need to get its length and inject that into the header in the correct spot
            b = BitConverter.GetBytes(tB.Count + 4);
            for (int k = 0; k < 4; k++) MarkerLightsFrontRightStateHeader[MarkerLightsFrontRightStateLenPos + k] = b[k];

            for (int i = 0; i < MarkerLightsFrontRightStateHeader.Length; i++)
                outputBytes.Add(MarkerLightsFrontRightStateHeader[i]); //header is only added once for each segment

            b = BitConverter.GetBytes(numChecked); //next is the number of buggies:
            for (int k = 0; k < 4; k++) outputBytes.Add(b[k]);

            for (int i = 0; i < tB.Count; i++)//add this array to the output block
                outputBytes.Add(tB[i]);

            //---------------------------------------------------------------------------------------------------


            tB = new List<byte>(); //the entire output chunk for this one array (for all cars) minus the header

            for (int i = 0; i < numTargetCars; i++) //MarkerLightsFrontLeftStateArray;
                for (int j = 0; j < MarkerLightsFrontLeftStateArray2[i].Length; j++) tB.Add(MarkerLightsFrontLeftStateArray2[i][j]);

            for (int i = 0; i < numSourceCars; i++) //MarkerLightsFrontLeftStateArray;
            {//collect all the frametypes from the ones checked in the list

                if (checkedListBox1.GetItemChecked(i))
                    for (int j = 0; j < MarkerLightsFrontLeftStateArray[i].Length; j++) tB.Add(MarkerLightsFrontLeftStateArray[i][j]);
            }
            //now that we have the whole block, we need to get its length and inject that into the header in the correct spot
            b = BitConverter.GetBytes(tB.Count + 4);
            for (int k = 0; k < 4; k++) MarkerLightsFrontLeftStateHeader[MarkerLightsFrontLeftStateLenPos + k] = b[k];

            for (int i = 0; i < MarkerLightsFrontLeftStateHeader.Length; i++)
                outputBytes.Add(MarkerLightsFrontLeftStateHeader[i]); //header is only added once for each segment

            b = BitConverter.GetBytes(numChecked); //next is the number of buggies:
            for (int k = 0; k < 4; k++) outputBytes.Add(b[k]);

            for (int i = 0; i < tB.Count; i++)//add this array to the output block
                outputBytes.Add(tB[i]);

            //---------------------------------------------------------------------------------------------------




            tB = new List<byte>(); //the entire output chunk for this one array (for all cars) minus the header

            for (int i = 0; i < numTargetCars; i++) //MarkerLightsRearRightStateArray;
                for (int j = 0; j < MarkerLightsRearRightStateArray2[i].Length; j++) tB.Add(MarkerLightsRearRightStateArray2[i][j]);

            for (int i = 0; i < numSourceCars; i++) //MarkerLightsRearRightStateArray;
            {//collect all the frametypes from the ones checked in the list

                if (checkedListBox1.GetItemChecked(i))
                    for (int j = 0; j < MarkerLightsRearRightStateArray[i].Length; j++) tB.Add(MarkerLightsRearRightStateArray[i][j]);
            }
            //now that we have the whole block, we need to get its length and inject that into the header in the correct spot
            b = BitConverter.GetBytes(tB.Count + 4);
            for (int k = 0; k < 4; k++) MarkerLightsRearRightStateHeader[MarkerLightsRearRightStateLenPos + k] = b[k];

            for (int i = 0; i < MarkerLightsRearRightStateHeader.Length; i++)
                outputBytes.Add(MarkerLightsRearRightStateHeader[i]); //header is only added once for each segment

            b = BitConverter.GetBytes(numChecked); //next is the number of buggies:
            for (int k = 0; k < 4; k++) outputBytes.Add(b[k]);

            for (int i = 0; i < tB.Count; i++)//add this array to the output block
                outputBytes.Add(tB[i]);

            //---------------------------------------------------------------------------------------------------




            tB = new List<byte>(); //the entire output chunk for this one array (for all cars) minus the header

            for (int i = 0; i < numTargetCars; i++) //MarkerLightsRearLeftStateArray;
                for (int j = 0; j < MarkerLightsRearLeftStateArray2[i].Length; j++) tB.Add(MarkerLightsRearLeftStateArray2[i][j]);

            for (int i = 0; i < numSourceCars; i++) //MarkerLightsRearLeftStateArray;
            {//collect all the frametypes from the ones checked in the list

                if (checkedListBox1.GetItemChecked(i))
                    for (int j = 0; j < MarkerLightsRearLeftStateArray[i].Length; j++) tB.Add(MarkerLightsRearLeftStateArray[i][j]);
            }
            //now that we have the whole block, we need to get its length and inject that into the header in the correct spot
            b = BitConverter.GetBytes(tB.Count + 4);
            for (int k = 0; k < 4; k++) MarkerLightsRearLeftStateHeader[MarkerLightsRearLeftStateLenPos + k] = b[k];

            for (int i = 0; i < MarkerLightsRearLeftStateHeader.Length; i++)
                outputBytes.Add(MarkerLightsRearLeftStateHeader[i]); //header is only added once for each segment

            b = BitConverter.GetBytes(numChecked); //next is the number of buggies:
            for (int k = 0; k < 4; k++) outputBytes.Add(b[k]);

            for (int i = 0; i < tB.Count; i++)//add this array to the output block
                outputBytes.Add(tB[i]);

            //---------------------------------------------------------------------------------------------------


            tB = new List<byte>(); //the entire output chunk for this one array (for all cars) minus the header

            for (int i = 0; i < numTargetCars; i++) //MarkerLightsCenterStateArray;
                for (int j = 0; j < MarkerLightsCenterStateArray2[i].Length; j++) tB.Add(MarkerLightsCenterStateArray2[i][j]);

            for (int i = 0; i < numSourceCars; i++) //MarkerLightsCenterStateArray;
            {//collect all the frametypes from the ones checked in the list

                if (checkedListBox1.GetItemChecked(i))
                    for (int j = 0; j < MarkerLightsCenterStateArray[i].Length; j++) tB.Add(MarkerLightsCenterStateArray[i][j]);
            }
            //now that we have the whole block, we need to get its length and inject that into the header in the correct spot
            b = BitConverter.GetBytes(tB.Count + 4);
            for (int k = 0; k < 4; k++) MarkerLightsCenterStateHeader[MarkerLightsCenterStateLenPos + k] = b[k];

            for (int i = 0; i < MarkerLightsCenterStateHeader.Length; i++)
                outputBytes.Add(MarkerLightsCenterStateHeader[i]); //header is only added once for each segment

            b = BitConverter.GetBytes(numChecked); //next is the number of buggies:
            for (int k = 0; k < 4; k++) outputBytes.Add(b[k]);

            for (int i = 0; i < tB.Count; i++)//add this array to the output block
                outputBytes.Add(tB[i]);

            //---------------------------------------------------------------------------------------------------



            //;
            tB = new List<byte>(); //the entire output chunk for this one array (for all cars) minus the header

            for (int i = 0; i < numTargetCars; i++) //FreightTypeArray;
                for (int j = 0; j < FreightTypeArray2[i].Length; j++) tB.Add(FreightTypeArray2[i][j]); //string itself with a trailing zero

            for (int i = 0; i < numSourceCars; i++) //FreightTypeArray;
            {//collect all the frametypes from the ones checked in the list

                if (checkedListBox1.GetItemChecked(i))
                {
                    //b = BitConverter.GetBytes(FreightTypeArray[i].Length);
                    //for (int k = 0; k < 4; k++) tB.Add(b[k]); //string length


                    for (int j = 0; j < FreightTypeArray[i].Length; j++) tB.Add(FreightTypeArray[i][j]); //string itself with a trailing zero
                }
            }
            //now that we have the whole name block, we need to get its length and inject that into the header in the correct spot
            b = BitConverter.GetBytes(tB.Count + 4);
            for (int k = 0; k < 4; k++) FreightTypeHeader[FreightTypeLenPos + k] = b[k];

            for (int i = 0; i < FreightTypeHeader.Length; i++)
                outputBytes.Add(FreightTypeHeader[i]); //header is only added once for each segment

            b = BitConverter.GetBytes(numChecked); //next is the number of buggies:
            for (int k = 0; k < 4; k++) outputBytes.Add(b[k]);

            for (int i = 0; i < tB.Count; i++)//add this array to the output block
                outputBytes.Add(tB[i]);

            //---------------------------------------------------------------------------------------------------


            tB = new List<byte>(); //the entire output chunk for this one array (for all cars) minus the header

            for (int i = 0; i < numTargetCars; i++) //FreightAmountArray;
                for (int j = 0; j < FreightAmountArray2[i].Length; j++) tB.Add(FreightAmountArray2[i][j]);

            for (int i = 0; i < numSourceCars; i++) //FreightAmountArray;
            {//collect all the frametypes from the ones checked in the list

                if (checkedListBox1.GetItemChecked(i))
                    for (int j = 0; j < FreightAmountArray[i].Length; j++) tB.Add(FreightAmountArray[i][j]);
            }
            //now that we have the whole block, we need to get its length and inject that into the header in the correct spot
            b = BitConverter.GetBytes(tB.Count + 4);
            for (int k = 0; k < 4; k++) FreightAmountHeader[FreightAmountLenPos + k] = b[k];

            for (int i = 0; i < FreightAmountHeader.Length; i++)
                outputBytes.Add(FreightAmountHeader[i]); //header is only added once for each segment

            b = BitConverter.GetBytes(numChecked); //next is the number of buggies:
            for (int k = 0; k < 4; k++) outputBytes.Add(b[k]);

            for (int i = 0; i < tB.Count; i++)//add this array to the output block
                outputBytes.Add(tB[i]);

            //---------------------------------------------------------------------------------------------------



            tB = new List<byte>(); //the entire output chunk for this one array (for all cars) minus the header

            for (int i = 0; i < numTargetCars; i++) //RegulatorValueArray;
                for (int j = 0; j < RegulatorValueArray2[i].Length; j++) tB.Add(RegulatorValueArray2[i][j]);

            for (int i = 0; i < numSourceCars; i++) //RegulatorValueArray;
            {//collect all the frametypes from the ones checked in the list

                if (checkedListBox1.GetItemChecked(i))
                    for (int j = 0; j < RegulatorValueArray[i].Length; j++) tB.Add(RegulatorValueArray[i][j]);
            }
            //now that we have the whole block, we need to get its length and inject that into the header in the correct spot
            b = BitConverter.GetBytes(tB.Count + 4);
            for (int k = 0; k < 4; k++) RegulatorValueHeader[RegulatorValueLenPos + k] = b[k];

            for (int i = 0; i < RegulatorValueHeader.Length; i++)
                outputBytes.Add(RegulatorValueHeader[i]); //header is only added once for each segment

            b = BitConverter.GetBytes(numChecked); //next is the number of buggies:
            for (int k = 0; k < 4; k++) outputBytes.Add(b[k]);

            for (int i = 0; i < tB.Count; i++)//add this array to the output block
                outputBytes.Add(tB[i]);

            //---------------------------------------------------------------------------------------------------


            tB = new List<byte>(); //the entire output chunk for this one array (for all cars) minus the header

            for (int i = 0; i < numTargetCars; i++) //BrakeValueArray;
                for (int j = 0; j < BrakeValueArray2[i].Length; j++) tB.Add(BrakeValueArray2[i][j]);

            for (int i = 0; i < numSourceCars; i++) //BrakeValueArray;
            {//collect all the frametypes from the ones checked in the list

                if (checkedListBox1.GetItemChecked(i))
                    for (int j = 0; j < BrakeValueArray[i].Length; j++) tB.Add(BrakeValueArray[i][j]);
            }
            //now that we have the whole block, we need to get its length and inject that into the header in the correct spot
            b = BitConverter.GetBytes(tB.Count + 4);
            for (int k = 0; k < 4; k++) BrakeValueHeader[BrakeValueLenPos + k] = b[k];

            for (int i = 0; i < BrakeValueHeader.Length; i++)
                outputBytes.Add(BrakeValueHeader[i]); //header is only added once for each segment

            b = BitConverter.GetBytes(numChecked); //next is the number of buggies:
            for (int k = 0; k < 4; k++) outputBytes.Add(b[k]);

            for (int i = 0; i < tB.Count; i++)//add this array to the output block
                outputBytes.Add(tB[i]);

            //---------------------------------------------------------------------------------------------------


            tB = new List<byte>(); //the entire output chunk for this one array (for all cars) minus the header

            for (int i = 0; i < numTargetCars; i++) //GeneratorValveValueArray;
                for (int j = 0; j < GeneratorValveValueArray2[i].Length; j++) tB.Add(GeneratorValveValueArray2[i][j]);

            for (int i = 0; i < numSourceCars; i++) //GeneratorValveValueArray;
            {//collect all the frametypes from the ones checked in the list

                if (checkedListBox1.GetItemChecked(i))
                    for (int j = 0; j < GeneratorValveValueArray[i].Length; j++) tB.Add(GeneratorValveValueArray[i][j]);
            }
            //now that we have the whole block, we need to get its length and inject that into the header in the correct spot
            b = BitConverter.GetBytes(tB.Count + 4);
            for (int k = 0; k < 4; k++) GeneratorValveValueHeader[GeneratorValveValueLenPos + k] = b[k];

            for (int i = 0; i < GeneratorValveValueHeader.Length; i++)
                outputBytes.Add(GeneratorValveValueHeader[i]); //header is only added once for each segment

            b = BitConverter.GetBytes(numChecked); //next is the number of buggies:
            for (int k = 0; k < 4; k++) outputBytes.Add(b[k]);

            for (int i = 0; i < tB.Count; i++)//add this array to the output block
                outputBytes.Add(tB[i]);

            //---------------------------------------------------------------------------------------------------




            tB = new List<byte>(); //the entire output chunk for this one array (for all cars) minus the header

            for (int i = 0; i < numTargetCars; i++) //CompressorValveValueArray;
                for (int j = 0; j < CompressorValveValueArray2[i].Length; j++) tB.Add(CompressorValveValueArray2[i][j]);

            for (int i = 0; i < numSourceCars; i++) //CompressorValveValueArray;
            {//collect all the frametypes from the ones checked in the list

                if (checkedListBox1.GetItemChecked(i))
                    for (int j = 0; j < CompressorValveValueArray[i].Length; j++) tB.Add(CompressorValveValueArray[i][j]);
            }
            //now that we have the whole block, we need to get its length and inject that into the header in the correct spot
            b = BitConverter.GetBytes(tB.Count + 4);
            for (int k = 0; k < 4; k++) CompressorValveValueHeader[CompressorValveValueLenPos + k] = b[k];

            for (int i = 0; i < CompressorValveValueHeader.Length; i++)
                outputBytes.Add(CompressorValveValueHeader[i]); //header is only added once for each segment

            b = BitConverter.GetBytes(numChecked); //next is the number of buggies:
            for (int k = 0; k < 4; k++) outputBytes.Add(b[k]);

            for (int i = 0; i < tB.Count; i++)//add this array to the output block
                outputBytes.Add(tB[i]);

            //---------------------------------------------------------------------------------------------------





            tB = new List<byte>(); //the entire output chunk for this one array (for all cars) minus the header

            for (int i = 0; i < numTargetCars; i++) //ReverserValueArray;
                for (int j = 0; j < ReverserValueArray2[i].Length; j++) tB.Add(ReverserValueArray2[i][j]);

            for (int i = 0; i < numSourceCars; i++) //ReverserValueArray;
            {//collect all the frametypes from the ones checked in the list

                if (checkedListBox1.GetItemChecked(i))
                    for (int j = 0; j < ReverserValueArray[i].Length; j++) tB.Add(ReverserValueArray[i][j]);
            }
            //now that we have the whole block, we need to get its length and inject that into the header in the correct spot
            b = BitConverter.GetBytes(tB.Count + 4);
            for (int k = 0; k < 4; k++) ReverserValueHeader[ReverserValueLenPos + k] = b[k];

            for (int i = 0; i < ReverserValueHeader.Length; i++)
                outputBytes.Add(ReverserValueHeader[i]); //header is only added once for each segment

            b = BitConverter.GetBytes(numChecked); //next is the number of buggies:
            for (int k = 0; k < 4; k++) outputBytes.Add(b[k]);

            for (int i = 0; i < tB.Count; i++)//add this array to the output block
                outputBytes.Add(tB[i]);

            //---------------------------------------------------------------------------------------------------




            tB = new List<byte>(); //the entire output chunk for this one array (for all cars) minus the header

            for (int i = 0; i < numTargetCars; i++) //SanderAmountArray;
                for (int j = 0; j < SanderAmountArray2[i].Length; j++) tB.Add(SanderAmountArray2[i][j]);

            for (int i = 0; i < numSourceCars; i++) //SanderAmountArray;
            {//collect all the frametypes from the ones checked in the list

                if (checkedListBox1.GetItemChecked(i))
                    for (int j = 0; j < SanderAmountArray[i].Length; j++) tB.Add(SanderAmountArray[i][j]);
            }
            //now that we have the whole block, we need to get its length and inject that into the header in the correct spot
            b = BitConverter.GetBytes(tB.Count + 4);
            for (int k = 0; k < 4; k++) SanderAmountHeader[SanderAmountLenPos + k] = b[k];

            for (int i = 0; i < SanderAmountHeader.Length; i++)
                outputBytes.Add(SanderAmountHeader[i]); //header is only added once for each segment

            b = BitConverter.GetBytes(numChecked); //next is the number of buggies:
            for (int k = 0; k < 4; k++) outputBytes.Add(b[k]);

            for (int i = 0; i < tB.Count; i++)//add this array to the output block
                outputBytes.Add(tB[i]);

            //---------------------------------------------------------------------------------------------------



            // At this point Outputbytes contains the entire segment with the cars, we need to add everything before, then this, then everything after:
            //let's get our total lengths, for which we need the file positions of sections A, B, and C
            int lengthA = findInBytes("FrameTypeArray") - 4;
            int startC = findInBytes("RemovedVegetationAssetsArray") - 4;
            if (startC <= 0)
            {
                MessageBox.Show("Error: invalid save file, does not contain RemovedVegetationAssetsArray!\nNew File not written.");
                return;
            }
            int lengthC = buff.Length - startC;
            byte[] finalOutput = new byte[lengthA + outputBytes.Count + lengthC];
            for (int i = 0; i < lengthA; i++) finalOutput[i] = (byte)(buff[i]);
            for (int i = 0; i < outputBytes.Count; i++) finalOutput[lengthA + i] = (byte)(outputBytes[i]);
            for (int i = 0; i < lengthC; i++) finalOutput[lengthA + outputBytes.Count + i] = (byte)(buff[i + startC]);

            //if (saveFileDialog1.FileName == null) { }

            string outfile = saveFileDialog1.InitialDirectory + @"\" + saveFileDialog1.FileName;

            if (File.Exists(outfile))
                if (MessageBox.Show("Overwrite File " + outfile, "Overwrite?", MessageBoxButtons.OKCancel) == DialogResult.Cancel) return;

            File.WriteAllBytes(outfile, finalOutput);
            MessageBox.Show(saveFileDialog1.FileName + "\nSaved!");

            colorSlots();





        }
    }
}