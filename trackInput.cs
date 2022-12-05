using System.Diagnostics;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Windows.Forms;

namespace BuggyExchange
{
    public partial class Form1 : Form
    {

        //this will read in track data from the Map file (buff2)
        //and use the functions from the "parseTargetFile" that were used to
        //parse out the different data types from the file (int, string, vector, etc)

        byte[] SplineTypeTrackHeader;
        byte[] SplineTrackLocationHeader;
        byte[] SplineTrackRotationHeader;
        byte[] SplineTrackStartPointHeader;
        byte[] SplineTrackEndPointHeader;
        byte[] SplineTrackStartTangentHeader;
        byte[] SplineTrackEndTangentHeader;
        byte[] SplineTrackSwitchStateHeader;
        byte[] SplineTrackPaintStyleHeader;

        int SplineTypeTrackLenPos = 68; //(43)
        int SplineTrackLocationLenPos = 127;
        int SplineTrackRotationLenPos = 127;
        int SplineTrackStartPointLenPos = 131;
        int SplineTrackEndPointLenPos = 127;
        int SplineTrackStartTangentLenPos = 135;
        int SplineTrackEndTangentLenPos = 131;
        int SplineTrackSwitchStateLenPos = 50;
        int SplineTrackPaintStyleLenPos = 49;

        int numTracks = 0;
        string[] trackTypes; //user-readable strings for the track types
        Vector2[] trackDrawStart, trackDrawEnd; //drawing positions of segments
        Vector3[] trackStart, trackEnd, tangentStart, tangentEnd, trackDrawLocation;
        Vector2[] tangentDrawStart, tangentDrawEnd;
        Vector3[] trackRotation, trackLocation;
        float[] trackGrade;
        int[] trackColor;
        int[] trackTypeColor;
        Pen[] gradeColor, tracktypePen;
        Vector2[] trackManualTest;

        byte[][] SplineTrackTypeArray;
        byte[][] SplineTrackLocationArray;
        byte[][] SplineTrackRotationArray;
        byte[][] SplineTrackStartPointArray;
        byte[][] SplineTrackEndPointArray;
        byte[][] SplineTrackStartTangentArray;
        byte[][] SplineTrackEndTangentArray;
        byte[][] SplineTrackSwitchStateArray;
        byte[][] SplineTrackPaintStyleArray;
        List<string> TrackSegmentNames = new List<string>();

        Vector3 getVectorFromBytes(byte[] bytes)
        {
            Vector3 vector = new Vector3();
            byte[] b = new byte[4];

            for (int i = 0; i < 4; i++) b[i] = bytes[i];
            vector.X = BitConverter.ToSingle(b);
            for (int i = 0; i < 4; i++) b[i] = bytes[i + 4];
            vector.Y = BitConverter.ToSingle(b);
            for (int i = 0; i < 4; i++) b[i] = bytes[i + 8];
            vector.Z = BitConverter.ToSingle(b);

            return vector;
        }
        Vector2 getVector2FromBytes(byte[] bytes)
        {
            Vector2 vector = new Vector2();
            byte[] b = new byte[4];

            for (int i = 0; i < 4; i++) b[i] = bytes[i];
            vector.X = BitConverter.ToSingle(b);
            for (int i = 0; i < 4; i++) b[i] = bytes[i + 4];
            vector.Y = BitConverter.ToSingle(b);

            return vector;
        }

        void parseTracksNow()
        {
            //need to find all of the Track Segments in the file, find "FrameTypeArray" in the file, number of cars in the save is at this position +58 bytes

            int found = findInBytes2("SplineTrackTypeArray");
            if (found == -1)
            {
                MessageBox.Show("Unable to find 'SplineTrackTypeArray' in this save file. Make sure you dropped a valid save file here.");
                this.Text = "Invalid File - Try Again";
                return;
            }

            //now we need to get the number of track segments
            numTracks = buff2[found + 64] + (buff2[found + 65] * 256);

            if (numTracks == 0)
            {
                MessageBox.Show("Note: this save does not contain any Tracks.");
                return;
            }

            trackTypes = new string[numTracks];
            trackLocation = new Vector3[numTracks]; //the unscaled xyz values
            trackDrawLocation = new Vector3[numTracks]; //the scaled xy values of the segment's center, for drawing on the map
            trackRotation = new Vector3[numTracks];
            trackGrade = new float[numTracks]; //stores the highest grade found within this segment of track
            trackColor = new int[numTracks]; //stores the drawing color for the track segments based on grade
            trackTypeColor = new int[numTracks]; //stores the drawing color for the track segments based on grade
            trackManualTest = new Vector2[numTracks * 100];

            trackDrawStart = new Vector2[numTracks];
            trackDrawEnd = new Vector2[numTracks];
            trackStart = new Vector3[numTracks];
            trackEnd = new Vector3[numTracks];

            tangentStart = new Vector3[numTracks];
            tangentEnd = new Vector3[numTracks];
            tangentDrawStart = new Vector2[numTracks];
            tangentDrawEnd = new Vector2[numTracks];



            // Track TYPES
            int i = 0;
            int pos = found + 72;
            int l;
            while (i < numTracks)
            {
                l = buff2[pos - 4]; //this is the length of the string we're getting
                trackTypes[i] = buff2ToString(pos, l);
                TrackSegmentNames.Add(trackTypes[i].Substring(0, trackTypes[i].Length - 1));
                pos += l + 4;
                i++;
            }
            //for now we won't be writing anything out so we don't need to store the binary data yet, only the strings



            //byte[][] SplineTrackSwitchStateArray;
            //byte[][] SplineTrackPaintStyleArray;

            //-------------------------------------------
            // LOCATIONS
            found = findInBytes2("SplineTrackLocationArray");
            if (found == -1)
            {
                MessageBox.Show("Unable to find 'SplineTrackLocationArray' in this save file. Make sure you dropped a valid save file here.");
                this.Text = "Invalid File - Try Again";
                TrackSegmentNames.Clear();
                return;
            }

            SplineTrackLocationArray = getVectorArray2(Encoding.ASCII.GetBytes("SplineTrackLocationArray"), 159, numTracks);
            //Debug.WriteLine("Locations:");
            pos = 0;
            for (int j = 0; j < numTracks; j++)
            {
                trackLocation[j] = getVectorFromBytes(SplineTrackLocationArray[j]);
                pos += 12;
                //Debug.WriteLine(trackLocation[j].ToString());
            }



            //-------------------------------------------
            // Rotations

            found = findInBytes2("SplineTrackRotationArray");
            if (found == -1)
            {
                MessageBox.Show("Unable to find 'SplineTrackRotationArray' in this save file. Make sure you dropped a valid save file here.");
                this.Text = "Invalid File - Try Again";
                TrackSegmentNames.Clear();
                return;
            }

            SplineTrackRotationArray = getVectorArray2(Encoding.ASCII.GetBytes("SplineTrackRotationArray"), 160, numTracks);
            //Debug.WriteLine("Rotations:");
            pos = 0;
            for (int j = 0; j < numTracks; j++)
            {
                trackRotation[j] = getVectorFromBytes(SplineTrackRotationArray[j]);
                pos += 12;
                //Debug.WriteLine(trackRotation[j].ToString());
            }


            //-------------------------------------------
            // StartPoints
            found = findInBytes2("SplineTrackStartPointArray");
            if (found == -1)
            {
                MessageBox.Show("Unable to find 'SplineTrackStartPointArray' in this save file. Make sure you dropped a valid save file here.");
                this.Text = "Invalid File - Try Again";
                TrackSegmentNames.Clear();
                return;
            }

            SplineTrackStartPointArray = getVectorArray2(Encoding.ASCII.GetBytes("SplineTrackStartPointArray"), 163, numTracks);
            //Debug.WriteLine("StartPoints:");
            pos = 0;
            for (int j = 0; j < numTracks; j++)
            {
                trackStart[j] = getVectorFromBytes(SplineTrackStartPointArray[j]);
                pos += 12;
                //Debug.WriteLine(trackStart[j].ToString());
            }



            //-------------------------------------------
            // EndPoints
            found = findInBytes2("SplineTrackEndPointArray");
            if (found == -1)
            {
                MessageBox.Show("Unable to find 'SplineTrackEndPointArray' in this save file. Make sure you dropped a valid save file here.");
                this.Text = "Invalid File - Try Again";
                TrackSegmentNames.Clear();
                return;
            }

            SplineTrackEndPointArray = getVectorArray2(Encoding.ASCII.GetBytes("SplineTrackEndPointArray"), 159, numTracks);
            //Debug.WriteLine("EndPoints:");
            pos = 0;
            for (int j = 0; j < numTracks; j++)
            {
                trackEnd[j] = getVectorFromBytes(SplineTrackEndPointArray[j]);
                pos += 12;
                //Debug.WriteLine(trackEnd[j].ToString());
            }




            //-------------------------------------------
            // StartTangents
            float tangentScale = -0.33333f;

            found = findInBytes2("SplineTrackStartTangentArray");
            if (found == -1)
            {
                MessageBox.Show("Unable to find 'SplineTrackStartTangentArray' in this save file. Make sure you dropped a valid save file here.");
                this.Text = "Invalid File - Try Again";
                TrackSegmentNames.Clear();
                return;
            }

            SplineTrackStartTangentArray = getVectorArray2(Encoding.ASCII.GetBytes("SplineTrackStartTangentArray"), 167, numTracks);
            //Debug.WriteLine("StartTangents:");
            pos = 0;
            for (int j = 0; j < numTracks; j++)
            {
                tangentStart[j] = getVectorFromBytes(SplineTrackStartTangentArray[j]);
                pos += 12;
                //Debug.WriteLine(tangentStart[j].ToString());
            }

            for (int c = 0; c < numTracks; c++)
            {
                //tangent lines are simple offsets from the start and end points, create an absolute not relative position so we can zoom/scale it
                tangentStart[c] = trackStart[c] - (tangentStart[c] * tangentScale);
            }

            //-------------------------------------------
            // EndTangents
            found = findInBytes2("SplineTrackEndTangentArray");
            if (found == -1)
            {
                MessageBox.Show("Unable to find 'SplineTrackEndTangentArray' in this save file. Make sure you dropped a valid save file here.");
                this.Text = "Invalid File - Try Again";
                TrackSegmentNames.Clear();
                return;
            }

            SplineTrackEndTangentArray = getVectorArray2(Encoding.ASCII.GetBytes("SplineTrackEndTangentArray"), 163, numTracks);
            //Debug.WriteLine("EndTangents:");
            pos = 0;
            for (int j = 0; j < numTracks; j++)
            {
                tangentEnd[j] = getVectorFromBytes(SplineTrackEndTangentArray[j]);
                pos += 12;
                //Debug.WriteLine(tangentEnd[j].ToString());
            }


            for (int c = 0; c < numTracks; c++)
            {
                //tangent lines are simple offsets from the start and end points
                tangentEnd[c] = trackEnd[c] + (tangentEnd[c] * tangentScale);
            }


            //Find the highest absolute grade for each track segment so we can draw it.
            //calculating the whole 3-d spline instead of just the Z axis,
            //if it's needed elsewhere we can use this code for it and put it in a function
            //also check track types and make them appropriate colors
            string[] ttList = { "rail_914\0", "rail_914_h01\0", "rail_914_h05\0", "rail_914_h10\0", "rail_914_bumper\0", "rail_914_trestle_wood_01\0", "rail_914_trestle_pile_01\0", "rail_914_trestle_steel_01\0", "rail_914_wall_01\0" };

            for (int t = 0; t < numTracks; t++)
            {
                float biggest = 0f;
                Vector3 prevPoint = trackStart[t];

                Vector3 aLen = tangentStart[t] - trackStart[t];
                Vector3 bLen = tangentEnd[t] - tangentStart[t];
                Vector3 cLen = trackEnd[t] - tangentEnd[t];

                //float nujg;
                //if (t == 283)
                //    nujg = 0f;

                for (float s = 0.01f; s <= 1.0f; s += 0.01f)
                {
                    //get the spline point at this distance along
                    Vector3 a = trackStart[t] + (aLen * s);
                    Vector3 b = tangentStart[t] + (bLen * s);
                    Vector3 c = tangentEnd[t] + (cLen * s);

                    Vector3 dLen = b - a;
                    Vector3 d = a + (dLen * s);
                    Vector3 eLen = c - b;
                    Vector3 e = b + (eLen * s);

                    Vector3 fLen = e - d;
                    Vector3 f = d + (fLen * s);

                    float dx = f.X - prevPoint.X;
                    float dy = f.Y - prevPoint.Y;
                    float run = (float)Math.Sqrt((dx * dx) + (dy * dy));
                    float grade = Math.Abs((f.Z - prevPoint.Z) / run) * 100f; //slope at this position (rise over run)
                    if (grade > biggest)
                    {
                        biggest = grade;
                    }
                    prevPoint = f;
                }
                //we've checked all points in the spline and have the largest grade.
                //decide which color to make it.
                trackGrade[t] = biggest; //this will always be zero or a positive number, never negative
                int colorIndex = (int)(biggest * 2f); //make sure the index is within bounds for our color list array
                if (colorIndex > 20) colorIndex = 20;
                trackColor[t] = colorIndex;

                //set the track type color for when it needs to be used for drawing:
                //looks for "rail_914_switch" and lump all of these together. No need to separate them by color

                trackTypeColor[t] = 0;//switches are all bundled as type 0, which is the default, so we don't need to search for them
                if ((trackTypes[t].Substring(0, 7) == "ballast") || (trackTypes[t] == "rail_914_wall_01_norail\0"))
                {
                    trackTypeColor[t] = 1; //brown
                }
                else
                {
                    for (int r = 0; r < ttList.Length; r++)
                    {
                        if (trackTypes[t] == ttList[r])
                        {
                            trackTypeColor[t] = r+2;
                            break;
                        }
                    }
                }


            }

            scaleChanged();
            showBuggiesOnMap();
            label4.Text = trackTypes.Length.ToString() + " Found";

            //this.Text = "File Loaded. " + trackTypes.Length.ToString() + " Track Segments Found.";
        }



        void getSplinePoint(int index, float x)
        {

        }



    }
}