using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Windows.Forms;

namespace BuggyExchange
{
    public partial class Form1 : Form
    {
        Vector3[] treePos;
        Vector2[] treeDraw;
        bool[] treeExists;
        void getAllTreesFromFile()
        {
            byte[] theFile = BuggyExchange.Properties.Resources.treevectors;
            treePos = new Vector3[theFile.Length / 12];
            treeDraw = new Vector2[treePos.Length];
            treeExists = new bool[treePos.Length];
            int p = 0;
            //mapScale = 4000f / 400000f;

            //float highest = 0f, lowest = 999999f ;


            for (int i = 0; i < theFile.Length; i += 12)
            {
                treePos[p].X = BitConverter.ToSingle(theFile, i);
                treePos[p].Y = BitConverter.ToSingle(theFile, i + 4);
                treePos[p].Z = BitConverter.ToSingle(theFile, i + 8);

                treeExists[p] = true;
                p++;
            }

            createTreeRegions();

            //Debug.WriteLine(highest);
            //Debug.WriteLine(lowest);


            ////generate a bitmap from the tree data that we can use for our map image.
            //Bitmap bmp = new Bitmap(4000, 4000);
            //using (Graphics g = Graphics.FromImage(bmp))
            //{
            //    g.Clear(Color.White);
            //    //if a pixel's elevation is within a tolerance from a multiple of 100 we can draw a relief line
            //    //float tol = 20f;
            //    //float[,] alt = new float[4000, 4000];

            //    for (int x = 0; x < 4000; x++)
            //    {
            //        for (int y = 0; y < 4000; y++) //for each xy position on the bitmap
            //        {
            //            float nearest = 999999f;
            //            int nIndex = 0;
            //            //find the nearest tree:
            //            for (int t = 0; t < treePos.Length; t++)
            //            {
            //                float dx = treeDraw[t].X - (float)x;
            //                float dy = treeDraw[t].Y - (float)y;
            //                float dist = (dx * dx) + (dy * dy);
            //                if (dist < nearest)
            //                {
            //                    nearest = dist;
            //                    nIndex = t;
            //                }
            //            }
            //            //alt[x, y] = (int)Math.Clamp((treePos[nIndex].Z / 123) - 32,0,255);
            //            int r = (int)Math.Clamp((treePos[nIndex].Z / 123) - 32, 0, 255);
            //            g.FillRectangle(new SolidBrush(Color.FromArgb(255, r, r, r)), x, y, 1, 1);
            //            //if (r % 16 == 0) g.FillRectangle(new SolidBrush(Color.Red), x, y, 1, 1);

            //            //alt[(int)x, (int)y] = treePos[nIndex].Z;
            //            //if (treePos[nIndex].Z % 1000 < tol)
            //            //{

            //            //    g.FillRectangle(new SolidBrush(Color.FromArgb(1, r, r, r)), x, y, 1, 1);
            //            //    //Debug.Write(treePos[nIndex].Z.ToString());
            //            //}
            //        }
            //        //Debug.WriteLine(x.ToString());
            //        if (x % 10 == 0)
            //            bmp.Save(@"E:\a\" + x.ToString() + ".png");
            //    }

            //    bmp.Save("reliefmap.png");
            //}


        }

        List<int>[] treeRegion = new List<int>[256];

        void createTreeRegions()
        {
            for (int i = 0; i < 256; i++) treeRegion[i] = new List<int>();

            //assume extents of tree X and Y positions are -400,000 to +400,000
            //divide regions up into 256 areas, 50,000 squared each
            for (int i = 0; i < treeExists.Length; i++)
            {
                int rY = (int)(treePos[i].Y + 200000f) / 25000; //y position in regions
                int region = (int)((treePos[i].X + 200000f) / 25000) + (rY * 16); // get our index, each Y is a row so we add 16 to go down one row
                treeRegion[region].Add(i); //add this tree to the appropriate region list.
                //if ((treePos[i].X > 200000f) || (treePos[i].X < -200000f)) MessageBox.Show("X outside region");
                //if ((treePos[i].Y > 200000f) || (treePos[i].Y < -200000f)) MessageBox.Show("Y outside region");
            }

            //for (int i = 0; i < 256; i++) Debug.WriteLine(i.ToString() + " = " + treeRegion[i].Count.ToString());
        }


        //clean up tracks
        void clearTreesFromTracks()
        {
            //for each piece of track we need to create a path from start to end using the spline handles/points
            for (int i = 0; i < trackStart.Length; i++)
            {
                //longest possible segment is distance between start and end * pi
                float dx, dy, dist;
                dx = trackStart[i].X - trackEnd[i].X;
                dy = trackStart[i].Y - trackEnd[i].Y;
                dist = 1f / ((float)Math.Sqrt(dx * dx + dy * dy) * 0.00628f);

                for (float w = 0f; w < 1f; w += dist)
                {
                    //get the spline point at this location:
                    float ax = trackStart[i].X + ((tangentStart[i].X - trackStart[i].X) * w);
                    float bx = tangentStart[i].X + ((tangentEnd[i].X - tangentStart[i].X) * w);
                    float cx = tangentEnd[i].X + ((trackEnd[i].X - tangentEnd[i].X) * w);
                    float ex = ax + ((bx - ax)*w);
                    float fx = bx + ((cx - bx) * w);
                    float gx = ex + ((fx - ex) * w);

                    float ay = trackStart[i].Y + ((tangentStart[i].Y - trackStart[i].Y) * w);
                    float by = tangentStart[i].Y + ((tangentEnd[i].Y - tangentStart[i].Y) * w);
                    float cy = tangentEnd[i].Y + ((trackEnd[i].Y - tangentEnd[i].Y) * w);
                    float ey = ay + ((by - ay) * w);
                    float fy = by + ((cy - by) * w);
                    float gy = ey + ((fy - ey) * w);

                    //each step along the path, do a clear radius.
                    removeRadiusTrees(gx, gy, trackBar1.Value * 100);
                }
            }


        }


        //find all trees within a radius from starting point and remove (or add) them
        void removeRadiusTrees(float x, float y, float radius, bool remove = true)
        {
            float x2, y2 = y;

            //determine which regions need to be searched, project out on all 8 directions to see if that region is overlapped by our radius
            HashSet<int> regions = new HashSet<int>();

            //right
            int rY = (int)(y2 + 200000f) / 25000;
            x2 = x + radius;
            regions.Add((int)((x2 + 200000f) / 25000) + (rY * 16));
            //left
            x2 = x - radius;
            regions.Add((int)((x2 + 200000f) / 25000) + (rY * 16));

            //top left
            y2 = y - radius;
            rY = (int)(y2 + 200000f) / 25000;
            regions.Add((int)((x2 + 200000f) / 25000) + (rY * 16));
            //top right
            x2 = x + radius;
            regions.Add((int)((x2 + 200000f) / 25000) + (rY * 16));
            //top center
            x2 = x;
            regions.Add((int)((x2 + 200000f) / 25000) + (rY * 16));

            //bottom left
            y2 = y + radius;
            rY = (int)(y2 + 200000f) / 25000;
            regions.Add((int)((x2 + 200000f) / 25000) + (rY * 16));
            //bottom right
            x2 = x + radius;
            regions.Add((int)((x2 + 200000f) / 25000) + (rY * 16));
            //bottom center
            x2 = x;
            regions.Add((int)((x2 + 200000f) / 25000) + (rY * 16));

            foreach (int i in regions)
            {
                float dist, dx, dy, minDist = radius * radius;
                //int fIndex = 0;
                foreach (int j in treeRegion[i])
                {
                    dx = x - treePos[j].X;
                    dy = y - treePos[j].Y;
                    dist = dx * dx + dy * dy;
                    if (dist < minDist)
                    {
                        if (remove) treeExists[j] = false;
                        else treeExists[j] = true;
                    }
                }

            }
        }

        // use a more efficient method to find the nearest tree so painting can be faster.
        // split trees into regions, sort list by region, and only search trees within the closest regions.
        // this is about 250 times faster than searching the whole list every time.
        void removeNearestTree(float x, float y, bool remove = true)
        {
            //find the region to search for tree:
            int rY = (int)(y + 200000f) / 25000; //y position in regions
            int region = (int)((x + 200000f) / 25000) + (rY * 16); // get our index, each Y is a row so we add 16 to go down one row

            //find the nearest tree in the region list and turn it false
            float dist, dx, dy, minDist = 9999999999f;
            int fIndex = 0;
            foreach (int j in treeRegion[region])
            {
                dx = x - treePos[j].X;
                dy = y - treePos[j].Y;
                dist = dx * dx + dy * dy;
                if (dist < minDist)
                {
                    minDist = dist;
                    fIndex = j;
                }
            }
            if (remove) treeExists[fIndex] = false;
            else treeExists[fIndex] = true;
        }

        void getTreeListFromMapSave()
        {
            //Find the header, start at offset, load in the vectors.
            //byte[] vegHeader = convertHeader("1D 00 00 00 52 65 6D 6F 76 65 64 56 65 67 65 74 61 74 69 6F 6E 41 73 73 65 74 73 41 72 72 61 79 00 0E");
            int found = findInBytes2("RemovedVegetationAssetsArray");
            if (found == -1)
            {
                MessageBox.Show("Note: The map save has no removed trees, or no tree data was found.");
                return;
            }
            for (int j = 0; j < treeExists.Length; j++) treeExists[j] = true;//restore all trees

            int size = BitConverter.ToInt32(buff2, found + 75); //how many vectors are included
            float tX, tY;
            int pos = found + 167; //NOTE: the offset is a hardcoded offset from the header start to the data start
            for (int i = 0; i < size; i++)
            {
                tX = BitConverter.ToSingle(buff2, pos);
                tY = BitConverter.ToSingle(buff2, pos + 4);
                pos += 12;

                removeNearestTree(tX, tY);
            }

            int removedCount = 0;
            for (int j = 0; j < treeExists.Length; j++)
                if (!treeExists[j]) removedCount++;
            label5.Text = removedCount.ToString() + " Removed";
        }

    }
}