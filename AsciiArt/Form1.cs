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

namespace AsciiArt
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void btn_import_Click(object sender, EventArgs e)
        {
            string filePath = String.Empty;

            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.InitialDirectory = "%userprofile%/Desktop";
                openFileDialog.Filter = "Files|*.png;";
                openFileDialog.RestoreDirectory = true;

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    //Get the path of specified file
                    filePath = openFileDialog.FileName;

                    Bitmap myBitmap = new Bitmap(filePath);

                    BmpToAscii.bmp = myBitmap;
                    BmpToAscii.ConvertToAscii();
                }
            }
        }

        private void btn_export_Click(object sender, EventArgs e)
        {
            string filePath = String.Empty;

            using (SaveFileDialog saveFileDialog = new SaveFileDialog())
            {
                saveFileDialog.InitialDirectory = "%userprofile%/Desktop";
                saveFileDialog.Filter = "Text File (*.txt)|*.txt;";
                saveFileDialog.RestoreDirectory = true;

                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    File.WriteAllText(saveFileDialog.FileName, BmpToAscii.GetAsciiArt());
                }
            }
        }

        public static class BmpToAscii
        {
            private static int tileSizeHeight = 6;
            private static int tileSizeWidth = 3;

            private enum BrightnessMap
            {
                Lightest = 255,
                Lighter = 240,
                Light = 210,
                MediumLight = 180,
                Medium = 150,
                MediumDark = 120,
                Dark = 90,
                Darker = 60,
                Darkest = 30,
                Black = 0
            };

            private static Dictionary<BrightnessMap, string> BrightnessMapDict = new Dictionary<BrightnessMap, string>{
                {BrightnessMap.Lightest,    " "},
                {BrightnessMap.Lighter,     "."},
                {BrightnessMap.Light,       "-"},
                {BrightnessMap.MediumLight, "+"},
                {BrightnessMap.Medium,      "*"},
                {BrightnessMap.MediumDark,  "o"},
                {BrightnessMap.Dark,        "O"},
                {BrightnessMap.Darker,      "G"},
                {BrightnessMap.Darkest,     "#"},
                {BrightnessMap.Black,       "@"}
            };

            public static Bitmap bmp;

            private static string[,] asciiArt;

            public static void ConvertToAscii()
            {
                int slicesHorizontal = (int)Math.Ceiling((double)(bmp.Width / tileSizeWidth));
                int slicesVertical = (int)Math.Ceiling((double)(bmp.Height / tileSizeHeight));

                asciiArt = new string[slicesHorizontal, slicesVertical];

                for (int x = 0; x < slicesHorizontal; x++)
                {
                    for (int y = 0; y < slicesVertical; y++)
                    {
                        int[] horizontal = new int[] { x * tileSizeWidth, Math.Min((x + 1) * tileSizeWidth, bmp.Width) };
                        int[] vertical = new int[] { y * tileSizeHeight, Math.Min((y + 1) * tileSizeHeight, bmp.Height) };

                        int avgRGB = GetAverageRGBFromTile(horizontal, vertical);

                        asciiArt[x, y] = GetCharFromBrightness(avgRGB);
                    }
                }
            }

            private static int GetAverageRGBFromTile(int[] horizontal, int[] vertical)
            {
                int horizontalDiff = horizontal[1] - horizontal[0];
                int verticalDiff = vertical[1] - vertical[0];

                int[,] BrightnessMapTile = new int[verticalDiff, horizontalDiff];

                for (int x = horizontal[0]; x < horizontal[1]; x++)
                {
                    for (int y = vertical[0]; y < vertical[1]; y++)
                    {
                        BrightnessMapTile[y%tileSizeHeight, x%tileSizeWidth] = (int)Math.Round(bmp.GetPixel(x, y).GetBrightness()*255);
                    }
                }

                int sum = BrightnessMapTile.Cast<int>().Sum();

                return sum / BrightnessMapTile.Length;
            }

            private static string GetCharFromBrightness(int avgRGB)
            {
                foreach (BrightnessMap x in BrightnessMapDict.Keys)
                {
                    if (avgRGB >= (int)x)
                    {
                        return BrightnessMapDict[x];
                    }
                }

                return "";
            }

            public static string GetAsciiArt()
            {
                if (asciiArt == null) return "";

                string shit = "";

                for (int x = 0; x < asciiArt.GetLength(1); x++)
                {
                    for (int y = 0; y < asciiArt.GetLength(0); y++)
                    {
                        shit += asciiArt[y, x];
                    }
                    shit += "\n";
                }

                return shit;
            }
        }
    }
}
