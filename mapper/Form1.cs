using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Drawing.Imaging;
using System.Reflection;
using System.Diagnostics;
using System.Xml;
using System.IO.Compression;

namespace mapper
{
    public partial class Form1 : Form
    {
        bool didITell = false;
        Random rnd = new Random();

        public Form1()
        {
            InitializeComponent();
        }

        private string getMapName(int x, int y)
        {
            return "..\\..\\evdEn\\evdEnContent\\GAME\\MAPS\\WORLD\\" + y.ToString("000") + "-" + x.ToString("000") + ".tmx";
        }

        private void btnBrowse_Click(object sender, EventArgs e)
        {
            OpenFileDialog fd = new OpenFileDialog();

            fd.FileName = edFile.Text;
            fd.Multiselect = false;
            fd.Filter = "PNG files (*.png)|*.png|All files (*.*)|*.*";
            fd.FilterIndex = 1;
            fd.RestoreDirectory = true;


            if (fd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                edFile.Text = fd.FileName;
                pictureBox.Image = new Bitmap(edFile.Text);
            }
        }

        private int randomize(int type)
        {
            switch (type)
            {
                case 1:
                case 65:
                //case 129:
                case 161:
                case 33:
                    int r = rnd.Next(100);
                    if (r < 86)
                        return type;
                    else if (r < 92)
                        return type + 13;
                    else if (r < 97)
                        return type + 14;
                    else
                        return type + 15;
                    break;
                default:
                    return type;
                    break;
            }
        }

        private int ConvertColor(Color clr)
        {
            if (clr == Color.FromArgb(0, 0, 255)) //ocean
            {
                return 0;
            }
            else if (clr == Color.FromArgb(255, 255, 0)) //sand
            {
                return 65;
            }
            else if (clr == Color.FromArgb(0, 255, 0)) //plains
            {
                return 1;
            }
            else if (clr == Color.FromArgb(0, 110, 0)) //woods
            {
                return 129;
            }
            else if (clr == Color.FromArgb(0, 255, 255)) //rivers
            {
                return 241;
            }
            else if (clr == Color.FromArgb(96, 57, 19)) //mountains
            {
                return 97;
            }
            else if (clr == Color.FromArgb(255, 255, 255)) //snow/ice
            {
                return 161;
            }
            else if (clr == Color.FromArgb(183, 183, 183)) //cities/roads
            {
                return 33;
            }
            return 0;
        }

        private int Dominator(int[,] bse, int x, int y)
        {
            int grasses = 0;
            int rivers = 0;
            int sands = 0;
            int ices = 0;

            if (x > 0)
            {
                switch (bse[x - 1, y])
                {
                    case 65: //sand
                        sands++;
                        break;
                    case 1: //grass
                        grasses++;
                        break;
                    case 241: //river
                        rivers++;
                        break;
                    case 161: //ice
                        ices++;
                        break;
                }
            }

            if (x < 63)
            {
                switch (bse[x + 1, y])
                {
                    case 65: //sand
                        sands++;
                        break;
                    case 1: //grass
                        grasses++;
                        break;
                    case 241: //river
                        rivers++;
                        break;
                    case 161: //ice
                        ices++;
                        break;
                }
            }

            if (y > 0)
            {
                switch (bse[x, y - 1])
                {
                    case 65: //sand
                        sands++;
                        break;
                    case 1: //grass
                        grasses++;
                        break;
                    case 241: //river
                        rivers++;
                        break;
                    case 161: //ice
                        ices++;
                        break;
                }
            }

            if (y < 63)
            {
                switch (bse[x, y + 1])
                {
                    case 65: //sand
                        sands++;
                        break;
                    case 1: //grass
                        grasses++;
                        break;
                    case 241: //river
                        rivers++;
                        break;
                    case 161: //ice
                        ices++;
                        break;
                }
            }

            if (x > 0 && y > 0)
            {
                switch (bse[x - 1, y - 1])
                {
                    case 65: //sand
                        sands++;
                        break;
                    case 1: //grass
                        grasses++;
                        break;
                    case 241: //river
                        rivers++;
                        break;
                    case 161: //ice
                        ices++;
                        break;
                }
            }

            if (x > 0 && y < 63)
            {
                switch (bse[x - 1, y + 1])
                {
                    case 65: //sand
                        sands++;
                        break;
                    case 1: //grass
                        grasses++;
                        break;
                    case 241: //river
                        rivers++;
                        break;
                    case 161: //ice
                        ices++;
                        break;
                }

            }

            if (x < 63 && y < 63)
            {
                switch (bse[x + 1, y + 1])
                {
                    case 65: //sand
                        sands++;
                        break;
                    case 1: //grass
                        grasses++;
                        break;
                    case 241: //river
                        rivers++;
                        break;
                    case 161: //ice
                        ices++;
                        break;
                }
            }

            if (x < 63 && y > 0)
            {
                switch (bse[x + 1, y - 1])
                {
                    case 65: //sand
                        sands++;
                        break;
                    case 1: //grass
                        grasses++;
                        break;
                    case 241: //river
                        rivers++;
                        break;
                    case 161: //ice
                        ices++;
                        break;
                }
            }

            if (sands == 0 && ices == 0 && rivers == 0 && grasses == 0) return bse[x, y];

            int z = Math.Max(Math.Max(sands, grasses), Math.Max(rivers, ices));
            if (z == grasses) return 1;
            if (z == rivers) return 241;
            if (z == sands) return 65;
            if (z == ices) return 161;

            return -1;
        }

        private bool isShore(int[,] bse, int x, int y)
        {
            if (!(bse[x, y] == 0 || bse[x, y] == 241)) return false;

            if (x > 0)
                if (bse[x - 1, y] > 0) return true;

            if (x < 63)
                if (bse[x + 1, y] > 0) return true;

            if (y > 0)
                if (bse[x, y - 1] > 0) return true;

            if (y < 63)
                if (bse[x, y + 1] > 0) return true;

            if (x > 0 && y > 0)
                if (bse[x - 1, y - 1] > 0) return true;

            if (x > 0 && y < 63)
                if (bse[x - 1, y + 1] > 0) return true;

            if (x < 63 && y < 63)
                if (bse[x + 1, y + 1] > 0) return true;

            if (x < 63 && y > 0)
                if (bse[x + 1, y - 1] > 0) return true;

            return false;
        }

        private void btnDo_Click(object sender, EventArgs e)
        {
            bool hasStuff;
            btnBrowse.Enabled = false;
            btnDo.Enabled = false;

            if (MessageBox.Show("To do, Yes, No?", "We about", MessageBoxButtons.YesNo) == DialogResult.No) return;

            using (Bitmap img = new Bitmap(edFile.Text),
                          roads = new Bitmap(Path.GetFileNameWithoutExtension(edFile.Text) + ".roads.png"))
            {
                int width = img.Width / 64;
                int height = img.Height / 64;

                for (int yy = 0; yy < height; yy++)
                {
                    for (int xx = 0; xx < width; xx++)
                    {
                        hasStuff = ProcessChunk(img, roads, yy, xx);
                        if (!hasStuff && xx != 0 && yy != 0)
                        {
                            string fileName = getMapName(xx, yy);
                            File.Delete(fileName);
                        }
                    }
                }
            }
            btnBrowse.Enabled = true;
            btnDo.Enabled = true;
            MessageBox.Show("Done");
        }

        private bool ProcessChunkIfNone(Bitmap img, Bitmap roads, int yy, int xx)
        {
            string fileName = getMapName(xx, yy);
            if (!File.Exists(fileName))
            {
                return ProcessChunk(img, roads, yy, xx);
            }
            return true;
        }

        private bool ProcessChunk(Bitmap img, Bitmap roads, int yy, int xx)
        {
            string fileName = getMapName(xx, yy); 
            bool hasStuff = false;

            int[,] under = new int[64, 64];
            int[,] ground = new int[64, 64];
            int[,] on = new int[64, 64];
            int[,] bse = new int[64, 64];
            int[,] coll = new int[64, 64];

            using (var file = File.CreateText(fileName))
            {
                WriteHeader(file, 64, 64, false);

                #region Under
                file.WriteLine("<layer name=\"under\" width=\"64\" height=\"64\">");
                file.WriteLine("<data>");

                int type;

                for (int y = 0; y < 64; y++)
                {
                    for (int x = 0; x < 64; x++)
                    {
                        type = ConvertColor(img.GetPixel((xx * 64 + x), (yy * 64 + y)));

                        if (type > 0) hasStuff = true;

                        bse[x, y] = randomize(type);

                        if (0 == type || 129 == type || 97 == type || -1 == type)
                        {
                            coll[x, y] = 256;
                        }
                        else if (241 == type)
                        {
                            coll[x, y] = 255;
                        }
                        else
                        {
                            coll[x, y] = 0;
                        }
                    }
                }

                for (int y = 0; y < 64; y++)
                {
                    for (int x = 0; x < 64; x++)
                    {
                        if (bse[x, y] == 241)
                        {
                            under[x, y] = 241;
                            ground[x, y] = 0;
                        }
                        else
                        {
                            under[x, y] = 0;
                            ground[x, y] = bse[x, y];
                        }

                        if (isShore(bse, x, y)) under[x, y] = 241;
                        //else under[x, y] = 0;

                        //if (bse[x, y] == 33) // road
                        //{
                        //    int z = Dominator(bse, x, y);
                        //    if (z > 0)
                        //    {
                        //        under[x, y] = z;
                        //        ground[x, y] = 33;
                        //    }
                        //}


                        type = ConvertColor(roads.GetPixel((xx * 64 + x), (yy * 64 + y)));

                        if (type == 33)
                        {
                            hasStuff = true;
                            //under[x, y] = ground[x, y];
                            //ground[x, y] = 225;
                            on[x, y] = 225;
                            coll[x, y] = 0;
                        }
                        else
                        {
                            on[x, y] = 0;
                        }

                    }
                }

                for (int y = 0; y < 64; y++)
                {
                    for (int x = 0; x < 64; x++)
                    {
                        file.WriteLine("<tile gid=\"" + under[x, y].ToString() + "\"/>");
                    }
                }
                file.WriteLine("</data>");
                file.WriteLine("</layer>");
                #endregion

                #region Ground
                file.WriteLine("<layer name=\"ground\" width=\"64\" height=\"64\">");
                file.WriteLine("<data>");
                for (int y = 0; y < 64; y++)
                {
                    for (int x = 0; x < 64; x++)
                    {
                        file.WriteLine("<tile gid=\"" + ground[x, y].ToString() + "\"/>");
                    }
                }
                file.WriteLine("</data>");
                file.WriteLine("</layer>");
                #endregion

                #region On
                file.WriteLine("<layer name=\"on\" width=\"64\" height=\"64\">");
                file.WriteLine("<data>");

                for (int y = 0; y < 64; y++)
                {
                    for (int x = 0; x < 64; x++)
                    {
                        file.WriteLine("<tile gid=\"" + on[x, y].ToString() + "\"/>");
                    }
                }
                file.WriteLine("</data>");
                file.WriteLine("</layer>");
                #endregion

                #region Over
                file.WriteLine("<layer name=\"over\" width=\"64\" height=\"64\">");
                file.WriteLine("<data>");

                for (int y = 0; y < 64; y++)
                {
                    for (int x = 0; x < 64; x++)
                    {
                        file.WriteLine("<tile gid=\"0\"/>");
                    }
                }
                file.WriteLine("</data>");
                file.WriteLine("</layer>");
                #endregion

                #region Collision
                file.WriteLine("<layer name=\"collision\" width=\"64\" height=\"64\">");
                file.WriteLine("<data>");
                for (int y = 0; y < 64; y++)
                {
                    for (int x = 0; x < 64; x++)
                    {
                        file.WriteLine("<tile gid=\"" + coll[x, y].ToString() + "\"/>");
                    }
                }


                file.WriteLine("</data>");
                file.WriteLine("</layer>");
                #endregion

                #region Objects
                file.WriteLine("<objectgroup name=\"objects\"></objectgroup>");
                #endregion

                file.WriteLine("</map>");

            }

            return hasStuff;
        }

        private void WriteHeader(StreamWriter file, int width, int height, bool isTemp)
        {
            file.WriteLine("<map version=\"1.0\" orientation=\"orthogonal\" width=\"" + width.ToString() + "\" height=\"" + height.ToString() + "\" tilewidth=\"64\" tileheight=\"64\">");
            file.WriteLine("<tileset firstgid=\"1\" name=\"tile0002s\" tilewidth=\"64\" tileheight=\"64\">");
            if (isTemp)
            {
                file.WriteLine("<image source=\"tile0002s.png\" width=\"1024\" height=\"1024\"/>");
            }
            else
            {
                file.WriteLine("<image source=\"..\\..\\..\\tiles\\tile0002s.png\" width=\"1024\" height=\"1024\"/>");
            }
            file.WriteLine("</tileset>");
        }


        PropertyInfo imageRectangleProperty = typeof(PictureBox).GetProperty("ImageRectangle", BindingFlags.GetProperty | BindingFlags.NonPublic | BindingFlags.Instance);

        private int[] getThemMapCoords(MouseEventArgs e)
        {
            int[] them = new int[2];
            them[0] = -1; them[1] = -1;

            Rectangle rectangle = (Rectangle)imageRectangleProperty.GetValue(pictureBox, null);
            if (rectangle.Contains(e.Location))
            {
                float newX = e.X - rectangle.X;
                float newY = e.Y - rectangle.Y;

                float scale = (float)(pictureBox.Image.Width) / (float)(rectangle.Width);

                them[0] = (int)(newX * scale) / 64;
                them[1] = (int)(newY * scale) / 64;
            }

            return them;
        }


        private void pictureBox_DoubleClick(object sender, EventArgs e)
        {
            TellCheck();

            if (pictureBox.Image == null) return;

            int[] them = getThemMapCoords((MouseEventArgs)e);
            if (them[0] != -1)
            {
                int xx = them[0];
                int yy = them[1];


                int width;
                int height;

                string fileName;

                using (Bitmap img = new Bitmap(edFile.Text),
                           roads = new Bitmap(Path.GetFileNameWithoutExtension(edFile.Text) + ".roads.png"))
                {
                    width = img.Width / 64;
                    height = img.Height / 64;
                    for (int y = yy > 0 ? yy - 1 : 0; y <= (yy < height - 2 ? yy + 1 : height - 1); y++)
                    {
                        for (int x = xx > 0 ? xx - 1 : 0; x <= (xx < width - 2 ? xx + 1 : width - 1); x++)
                        {
                            ProcessChunkIfNone(img, roads, y, x);
                        }
                    }
                }

                int[,] under = new int[64, 64];
                int[,] ground = new int[64, 64];
                int[,] on = new int[64, 64];
                int[,] over = new int[64, 64];
                int[,] coll = new int[64, 64];

                int[,] aunder = new int[128, 128];
                int[,] aground = new int[128, 128];
                int[,] aon = new int[128, 128];
                int[,] aover = new int[128, 128];
                int[,] acoll = new int[128, 128];


                #region Filling
                for (int y = yy > 0 ? yy - 1 : 0; y <= (yy < height - 2 ? yy + 1 : height - 1); y++)
                {
                    for (int x = xx > 0 ? xx - 1 : 0; x <= (xx < width - 2 ? xx + 1 : width - 1); x++)
                    {
                        fileName = getMapName(x, y);
                        int modx = (x < xx) ? 0 : ((x > xx) ? 2 : 1);
                        int mody = (y < yy) ? 0 : ((y > yy) ? 2 : 1);

                        int mod = mody * 3 + modx;

                        LoadMap(fileName, under, ground, on, over, coll);

                        switch (mod)
                        {
                            case 0: //top left
                                for (int j = 32; j < 64; j++)
                                {
                                    for (int i = 32; i < 64; i++)
                                    {
                                        aunder[i - 32, j - 32] = under[i, j];
                                        aground[i - 32, j - 32] = ground[i, j];
                                        aon[i - 32, j - 32] = on[i, j];
                                        aover[i - 32, j - 32] = over[i, j];
                                        acoll[i - 32, j - 32] = coll[i, j];
                                    }
                                }
                                break;
                            case 1: //top
                                for (int j = 32; j < 64; j++)
                                {
                                    for (int i = 0; i < 64; i++)
                                    {
                                        aunder[i + 32, j - 32] = under[i, j];
                                        aground[i + 32, j - 32] = ground[i, j];
                                        aon[i + 32, j - 32] = on[i, j];
                                        aover[i + 32, j - 32] = over[i, j];
                                        acoll[i + 32, j - 32] = coll[i, j];
                                    }
                                }
                                break;
                            case 2: //top right
                                for (int j = 32; j < 64; j++)
                                {
                                    for (int i = 0; i < 32; i++)
                                    {
                                        aunder[i + 96, j - 32] = under[i, j];
                                        aground[i + 96, j - 32] = ground[i, j];
                                        aon[i + 96, j - 32] = on[i, j];
                                        aover[i + 96, j - 32] = over[i, j];
                                        acoll[i + 96, j - 32] = coll[i, j];
                                    }
                                }
                                break;
                            case 3: //left
                                for (int j = 0; j < 64; j++)
                                {
                                    for (int i = 32; i < 64; i++)
                                    {
                                        aunder[i - 32, j + 32] = under[i, j];
                                        aground[i - 32, j + 32] = ground[i, j];
                                        aon[i - 32, j + 32] = on[i, j];
                                        aover[i - 32, j + 32] = over[i, j];
                                        acoll[i - 32, j + 32] = coll[i, j];
                                    }
                                }
                                break;
                            case 4: //dead on
                                for (int j = 0; j < 64; j++)
                                {
                                    for (int i = 0; i < 64; i++)
                                    {
                                        aunder[i + 32, j + 32] = under[i, j];
                                        aground[i + 32, j + 32] = ground[i, j];
                                        aon[i + 32, j + 32] = on[i, j];
                                        aover[i + 32, j + 32] = over[i, j];
                                        acoll[i + 32, j + 32] = coll[i, j];
                                    }
                                }
                                break;
                            case 5: //right
                                for (int j = 0; j < 64; j++)
                                {
                                    for (int i = 0; i < 32; i++)
                                    {
                                        aunder[i + 96, j + 32] = under[i, j];
                                        aground[i + 96, j + 32] = ground[i, j];
                                        aon[i + 96, j + 32] = on[i, j];
                                        aover[i + 96, j + 32] = over[i, j];
                                        acoll[i + 96, j + 32] = coll[i, j];
                                    }
                                }
                                break;
                            case 6: //bottom left
                                for (int j = 0; j < 32; j++)
                                {
                                    for (int i = 32; i < 64; i++)
                                    {
                                        aunder[i - 32, j + 96] = under[i, j];
                                        aground[i - 32, j + 96] = ground[i, j];
                                        aon[i - 32, j + 96] = on[i, j];
                                        aover[i - 32, j + 96] = over[i, j];
                                        acoll[i - 32, j + 96] = coll[i, j];
                                    }
                                }
                                break;
                            case 7: //bottom
                                for (int j = 0; j < 32; j++)
                                {
                                    for (int i = 0; i < 64; i++)
                                    {
                                        aunder[i + 32, j + 96] = under[i, j];
                                        aground[i + 32, j + 96] = ground[i, j];
                                        aon[i + 32, j + 96] = on[i, j];
                                        aover[i + 32, j + 96] = over[i, j];
                                        acoll[i + 32, j + 96] = coll[i, j];
                                    }
                                }
                                break;
                            case 8: //bottom right
                                for (int j = 0; j < 32; j++)
                                {
                                    for (int i = 0; i < 32; i++)
                                    {
                                        aunder[i + 96, j + 96] = under[i, j];
                                        aground[i + 96, j + 96] = ground[i, j];
                                        aon[i + 96, j + 96] = on[i, j];
                                        aover[i + 96, j + 96] = over[i, j];
                                        acoll[i + 96, j + 96] = coll[i, j];
                                    }
                                }
                                break;
                            default:
                                MessageBox.Show("Why?");
                                break;
                        } //switch
                    }
                }
                #endregion

                SaveMap("temp.tmx", aunder, aground, aon, aover, acoll, 128);

                //fileName = yy.ToString("000") + "-" + xx.ToString("000") + ".tmx";
                var myProcess = new Process { StartInfo = new ProcessStartInfo("C:\\Program Files (x86)\\Tiled\\tiled.exe", "temp.tmx") };
                myProcess.Start();
                myProcess.WaitForExit();

                LoadTempMap("temp.tmx", aunder, aground, aon, aover, acoll);

                #region Unfilling
                for (int y = yy > 0 ? yy - 1 : 0; y <= (yy < height - 2 ? yy + 1 : height - 1); y++)
                {
                    for (int x = xx > 0 ? xx - 1 : 0; x <= (xx < width - 2 ? xx + 1 : width - 1); x++)
                    {
                        fileName = getMapName(x, y);
                        int modx = (x < xx) ? 0 : ((x > xx) ? 2 : 1);
                        int mody = (y < yy) ? 0 : ((y > yy) ? 2 : 1);

                        int mod = mody * 3 + modx;

                        LoadMap(fileName, under, ground, on, over, coll);

                        switch (mod)
                        {
                            case 0: //top left
                                for (int j = 32; j < 64; j++)
                                {
                                    for (int i = 32; i < 64; i++)
                                    {
                                        under[i, j] = aunder[i - 32, j - 32];
                                        ground[i, j] = aground[i - 32, j - 32];
                                        on[i, j] = aon[i - 32, j - 32];
                                        over[i, j] = aover[i - 32, j - 32];
                                        coll[i, j] = acoll[i - 32, j - 32];
                                    }
                                }
                                break;
                            case 1: //top
                                for (int j = 32; j < 64; j++)
                                {
                                    for (int i = 0; i < 64; i++)
                                    {
                                        under[i, j] = aunder[i + 32, j - 32];
                                        ground[i, j] = aground[i + 32, j - 32];
                                        on[i, j] = aon[i + 32, j - 32];
                                        over[i, j] = aover[i + 32, j - 32];
                                        coll[i, j] = acoll[i + 32, j - 32];
                                    }
                                }
                                break;
                            case 2: //top right
                                for (int j = 32; j < 64; j++)
                                {
                                    for (int i = 0; i < 32; i++)
                                    {
                                        under[i, j] = aunder[i + 96, j - 32];
                                        ground[i, j] = aground[i + 96, j - 32];
                                        on[i, j] = aon[i + 96, j - 32];
                                        over[i, j] = aover[i + 96, j - 32];
                                        coll[i, j] = acoll[i + 96, j - 32];
                                    }
                                }
                                break;
                            case 3: //left
                                for (int j = 0; j < 64; j++)
                                {
                                    for (int i = 32; i < 64; i++)
                                    {
                                        under[i, j] = aunder[i - 32, j + 32];
                                        ground[i, j] = aground[i - 32, j + 32];
                                        on[i, j] = aon[i - 32, j + 32];
                                        over[i, j] = aover[i - 32, j + 32];
                                        coll[i, j] = acoll[i - 32, j + 32];
                                    }
                                }
                                break;
                            case 4: //dead on
                                for (int j = 0; j < 64; j++)
                                {
                                    for (int i = 0; i < 64; i++)
                                    {
                                        under[i, j] = aunder[i + 32, j + 32];
                                        ground[i, j] = aground[i + 32, j + 32];
                                        on[i, j] = aon[i + 32, j + 32];
                                        over[i, j] = aover[i + 32, j + 32];
                                        coll[i, j] = acoll[i + 32, j + 32];
                                    }
                                }
                                break;
                            case 5: //right
                                for (int j = 0; j < 64; j++)
                                {
                                    for (int i = 0; i < 32; i++)
                                    {
                                        under[i, j] = aunder[i + 96, j + 32];
                                        ground[i, j] = aground[i + 96, j + 32];
                                        on[i, j] = aon[i + 96, j + 32];
                                        over[i, j] = aover[i + 96, j + 32];
                                        coll[i, j] = acoll[i + 96, j + 32];
                                    }
                                }
                                break;
                            case 6: //bottom left
                                for (int j = 0; j < 32; j++)
                                {
                                    for (int i = 32; i < 64; i++)
                                    {
                                        under[i, j] = aunder[i - 32, j + 96];
                                        ground[i, j] = aground[i - 32, j + 96];
                                        on[i, j] = aon[i - 32, j + 96];
                                        over[i, j] = aover[i - 32, j + 96];
                                        coll[i, j] = acoll[i - 32, j + 96];
                                    }
                                }
                                break;
                            case 7: //bottom
                                for (int j = 0; j < 32; j++)
                                {
                                    for (int i = 0; i < 64; i++)
                                    {
                                        under[i, j] = aunder[i + 32, j + 96];
                                        ground[i, j] = aground[i + 32, j + 96];
                                        on[i, j] = aon[i + 32, j + 96];
                                        over[i, j] = aover[i + 32, j + 96];
                                        coll[i, j] = acoll[i + 32, j + 96];
                                    }
                                }
                                break;
                            case 8: //bottom right
                                for (int j = 0; j < 32; j++)
                                {
                                    for (int i = 0; i < 32; i++)
                                    {
                                        under[i, j] = aunder[i + 96, j + 96];
                                        ground[i, j] = aground[i + 96, j + 96];
                                        on[i, j] = aon[i + 96, j + 96];
                                        over[i, j] = aover[i + 96, j + 96];
                                        coll[i, j] = acoll[i + 96, j + 96];
                                    }
                                }
                                break;
                            default:
                                MessageBox.Show("Why?");
                                break;
                        } //switch

                        SaveJustMapData(fileName, under, ground, on, over, coll);
                    }
                }
                #endregion

            }

        }

        private void TellCheck()
        {
            if (!didITell)
            {
                didITell = true;
                MessageBox.Show("Just FYI: Only map-map layers (under, ground, on, and collision) will be propagated, to do objects and such use Right-Click");
            }
        }

        private void SaveJustMapData(string fileName, int[,] under, int[,] ground, int[,] on, int[,] over, int[,] coll)
        {
            XmlDocument xdoc = new XmlDocument();
            xdoc.Load(fileName);
            xdoc.Save(fileName+".bak");

            foreach (XmlElement elem in xdoc.GetElementsByTagName("layer"))
            {
                string name = elem.GetAttribute("name");

                foreach (XmlElement eee in elem.GetElementsByTagName("data"))
                {
                    if (name.Equals("under"))
                    {
                        eee.RemoveAll();
                        for (int y = 0; y < 64; y++)
                        {
                            for (int x = 0; x < 64; x++)
                            {
                                var z = xdoc.CreateElement("tile");
                                z.SetAttribute("gid",under[x, y].ToString());
                                eee.AppendChild(z);
                            }
                        }
                    }
                    else if (name.Equals("ground"))
                    {
                        eee.RemoveAll();
                        for (int y = 0; y < 64; y++)
                        {
                            for (int x = 0; x < 64; x++)
                            {
                                var z = xdoc.CreateElement("tile");
                                z.SetAttribute("gid", ground[x, y].ToString());
                                eee.AppendChild(z);
                            }
                        }
                    }
                    else if (name.Equals("on"))
                    {
                        eee.RemoveAll();
                        for (int y = 0; y < 64; y++)
                        {
                            for (int x = 0; x < 64; x++)
                            {
                                var z = xdoc.CreateElement("tile");
                                z.SetAttribute("gid", on[x, y].ToString());
                                eee.AppendChild(z);
                            }
                        }
                    }
                    else if (name.Equals("over"))
                    {
                        eee.RemoveAll();
                        for (int y = 0; y < 64; y++)
                        {
                            for (int x = 0; x < 64; x++)
                            {
                                var z = xdoc.CreateElement("tile");
                                z.SetAttribute("gid", over[x, y].ToString());
                                eee.AppendChild(z);
                            }
                        }
                    }
                    else if (name.Equals("collision"))
                    {
                        eee.RemoveAll();
                        for (int y = 0; y < 64; y++)
                        {
                            for (int x = 0; x < 64; x++)
                            {
                                if (coll[x, y] != 0 && coll[x, y] != 255 && coll[x, y] != 256) coll[x, y] = 0;
                                var z = xdoc.CreateElement("tile");
                                z.SetAttribute("gid", coll[x, y].ToString());
                                eee.AppendChild(z);
                            }
                        }
                        
                    } 
                }
            }
            xdoc.Save(fileName);
        }

        private void LoadMap(string fileName, int[,] under, int[,] ground, int[,] on, int[,] over, int[,] coll)
        {
            XmlDocument xdoc = new XmlDocument();
            xdoc.Load(fileName);

            foreach (XmlElement elem in xdoc.GetElementsByTagName("layer"))
            {
                string name = elem.GetAttribute("name");

                int i = 0;
                int j = 0;

                foreach (XmlElement eee in elem.GetElementsByTagName("data"))
                {
                    LoadThatData(under, ground, on, over, coll, name, ref i, ref j, eee, 64);
                }
            }
        }

        private void LoadThatData(int[,] under, int[,] ground, int[,] on, int[,] over, int[,] coll, string name, ref int i, ref int j, XmlElement eee, int width)
        {
            string baseEncode = eee.GetAttribute("encoding");

            if (string.IsNullOrEmpty(baseEncode))
            {
                foreach (XmlElement ee in eee.GetElementsByTagName("tile"))
                {
                    string s = ee.GetAttribute("gid");
                    if (name.Equals("under"))
                    {
                        under[i, j] = int.Parse(s);
                    }
                    else if (name.Equals("ground"))
                    {
                        ground[i, j] = int.Parse(s);
                    }
                    else if (name.Equals("on"))
                    {
                        on[i, j] = int.Parse(s);
                    }
                    else if (name.Equals("over"))
                    {
                        over[i, j] = int.Parse(s);
                    }
                    else if (name.Equals("collision"))
                    {
                        coll[i, j] = int.Parse(s);
                        if (coll[i, j] != 0 && coll[i, j] != 255 && coll[i, j] != 256) coll[i, j] = 0;
                    }

                    i++;
                    if (i >= width)
                    {
                        i = 0;
                        j++;
                    }
                }
            }
            else if (baseEncode.Equals("base64"))
            {
                string compression = eee.GetAttribute("compression");
                string ss = eee.InnerText;
                Byte[] data = Convert.FromBase64String(ss);

                if (string.IsNullOrEmpty(compression))
                {
                    for (int iData = 0; iData < data.Length; iData += sizeof(UInt32))
                    {
                        if (name.Equals("under"))
                        {
                            under[i, j] = BitConverter.ToInt32(data, iData);
                        }
                        else if (name.Equals("ground"))
                        {
                            ground[i, j] = BitConverter.ToInt32(data, iData);
                        }
                        else if (name.Equals("on"))
                        {
                            on[i, j] = BitConverter.ToInt32(data, iData);
                        }
                        else if (name.Equals("over"))
                        {
                            over[i, j] = BitConverter.ToInt32(data, iData);
                        }
                        else if (name.Equals("collision"))
                        {
                            coll[i, j] = BitConverter.ToInt32(data, iData);
                            if (coll[i, j] != 0 && coll[i, j] != 255 && coll[i, j] != 256) coll[i, j] = 0;
                        }

                        i++;
                        if (i >= width)
                        {
                            i = 0;
                            j++;
                        }
                    }
                }
                else if (compression.Equals("gzip"))
                {
                    GZipStream gz = new GZipStream(new MemoryStream(data), CompressionMode.Decompress);
                    Byte[] buffer = new Byte[sizeof(UInt32)];
                    while (gz.Read(buffer, 0, buffer.Length) == buffer.Length)
                    {
                        if (name.Equals("under"))
                        {
                            under[i, j] = BitConverter.ToInt32(buffer, 0);
                        }
                        else if (name.Equals("ground"))
                        {
                            ground[i, j] = BitConverter.ToInt32(buffer, 0);
                        }
                        else if (name.Equals("on"))
                        {
                            on[i, j] = BitConverter.ToInt32(buffer, 0);
                        }
                        else if (name.Equals("over"))
                        {
                            over[i, j] = BitConverter.ToInt32(buffer, 0);
                        }
                        else if (name.Equals("collision"))
                        {
                            coll[i, j] = BitConverter.ToInt32(buffer, 0);
                            if (coll[i, j] != 0 && coll[i, j] != 255 && coll[i, j] != 256) coll[i, j] = 0;
                        }

                        i++;
                        if (i >= width)
                        {
                            i = 0;
                            j++;
                        }
                    }
                }
                else if (compression.Equals("zlib"))
                {
                    // zlib data - first two bytes zlib specific and not part of deflate
                    MemoryStream ms = new MemoryStream(data);
                    ms.ReadByte();
                    ms.ReadByte();
                    DeflateStream gz = new DeflateStream(ms, CompressionMode.Decompress);
                    Byte[] buffer = new Byte[sizeof(UInt32)];
                    while (gz.Read(buffer, 0, buffer.Length) == buffer.Length)
                    {
                        if (name.Equals("under"))
                        {
                            under[i, j] = BitConverter.ToInt32(buffer, 0);
                        }
                        else if (name.Equals("ground"))
                        {
                            ground[i, j] = BitConverter.ToInt32(buffer, 0);
                        }
                        else if (name.Equals("on"))
                        {
                            on[i, j] = BitConverter.ToInt32(buffer, 0);
                        }
                        else if (name.Equals("over"))
                        {
                            over[i, j] = BitConverter.ToInt32(buffer, 0);
                        }
                        else if (name.Equals("collision"))
                        {
                            coll[i, j] = BitConverter.ToInt32(buffer, 0);
                            if (coll[i, j] != 0 && coll[i, j] != 255 && coll[i, j] != 256) coll[i, j] = 0;
                        }

                        i++;
                        if (i >= width)
                        {
                            i = 0;
                            j++;
                        }
                    }

                }
                else
                {
                    MessageBox.Show("Great unknown \"" + compression + "\" for compression");
                }
            }
            else
            {
                MessageBox.Show("Great unknown \"" + baseEncode + "\" for encoding");
            }
        }

        private void LoadTempMap(string fileName, int[,] under, int[,] ground, int[,] on, int[,] over, int[,] coll)
        {
            XmlDocument xdoc = new XmlDocument();
            xdoc.Load(fileName);

            foreach (XmlElement elem in xdoc.GetElementsByTagName("layer"))
            {
                string name = elem.GetAttribute("name");

                int i = 0;
                int j = 0;

                foreach (XmlElement eee in elem.GetElementsByTagName("data"))
                {
                    LoadThatData(under, ground, on, over, coll, name, ref i, ref j, eee, 128);
                }
            }
        }

        private void SaveMap(string fileName, int[,] under, int[,] ground, int[,] on, int[,] over, int[,] coll, int width)
        {
            using (var file = File.CreateText(fileName))
            {
                WriteHeader(file, width, width, width>64);

                #region Under
                file.WriteLine("<layer name=\"under\" width=\"" + width.ToString() + "\" height=\"" + width.ToString()+ "\">");
                file.WriteLine("<data>");

                for (int y = 0; y < width; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        file.WriteLine("<tile gid=\"" + under[x, y].ToString() + "\"/>");
                    }
                }
                file.WriteLine("</data>");
                file.WriteLine("</layer>");
                #endregion

                #region Ground
                file.WriteLine("<layer name=\"ground\" width=\"" + width.ToString() + "\" height=\"" + width.ToString()+ "\">");
                file.WriteLine("<data>");
                for (int y = 0; y < width; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        file.WriteLine("<tile gid=\"" + ground[x, y].ToString() + "\"/>");
                    }
                }
                file.WriteLine("</data>");
                file.WriteLine("</layer>");
                #endregion

                #region On
                file.WriteLine("<layer name=\"on\" width=\"" + width.ToString() + "\" height=\"" + width.ToString()+ "\">");
                file.WriteLine("<data>");
                for (int y = 0; y < width; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        file.WriteLine("<tile gid=\"" + on[x, y].ToString() + "\"/>");
                    }
                }
                file.WriteLine("</data>");
                file.WriteLine("</layer>");
                #endregion

                #region Over
                file.WriteLine("<layer name=\"over\" width=\"" + width.ToString() + "\" height=\"" + width.ToString()+ "\">");
                file.WriteLine("<data>");
                for (int y = 0; y < width; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        file.WriteLine("<tile gid=\"" + over[x, y].ToString() + "\"/>");
                    }
                }
                file.WriteLine("</data>");
                file.WriteLine("</layer>");
                #endregion

                #region Collision
                file.WriteLine("<layer name=\"collision\" width=\"" + width.ToString() + "\" height=\"" + width.ToString()+ "\">");
                file.WriteLine("<data>");
                for (int y = 0; y < width; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        file.WriteLine("<tile gid=\"" + coll[x, y].ToString() + "\"/>");
                    }
                }
                file.WriteLine("</data>");
                file.WriteLine("</layer>");
                #endregion

                file.WriteLine("</map>");

            }
        }

        private void pictureBox_MouseMove(object sender, MouseEventArgs e)
        {
            if (pictureBox.Image == null) return;

            int[] them = getThemMapCoords(e);
            if (them[0] != -1)
            {
                int xx = them[0];
                int yy = them[1];
                string fileName = getMapName(xx, yy);
                toolLabel.Text = fileName;
            }
        }

        private void pictureBox_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                if (pictureBox.Image == null) return;

                int[] them = getThemMapCoords(e);
                if (them[0] != -1)
                {
                    int xx = them[0];
                    int yy = them[1];

                    string fileName = getMapName(xx, yy);
                    var myProcess = new Process { StartInfo = new ProcessStartInfo("C:\\Program Files (x86)\\Tiled\\tiled.exe", fileName) };
                    myProcess.Start();
                    myProcess.WaitForExit();
                }
            }
        }

    }
}
