using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Runtime.InteropServices;

namespace ChaosIslandHacking
{
    public partial class Form2 : Form
    {
        public Form2()
        {
            InitializeComponent();
        }

        ChaosImage.Sprite[] sprites;
        int[] loadedPalette = {};

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern int SendMessage(IntPtr hWnd, int wMsg, IntPtr wParam, IntPtr lParam);
        private const int WM_VSCROLL = 277;
        private const int SB_PAGEBOTTOM = 7;

        public static void ScrollToBottom(RichTextBox MyRichTextBox)
        {
            SendMessage(MyRichTextBox.Handle, WM_VSCROLL, (IntPtr)SB_PAGEBOTTOM, IntPtr.Zero);
        }

        //TODO: There's currently nothing associating the items with their original index once they get sorted. That means if you sort them and click on a file, it'll load the file that was ORIGINALLY in that position.
        private class ListViewItemComparer : System.Collections.IComparer
        {
            private int col;
            public ListViewItemComparer()
            {
                col = 0;
            }
            public ListViewItemComparer(int column)
            {
                col = column;
            }
            public int Compare(object x, object y)
            {
                return String.Compare(((ListViewItem)x).SubItems[col].Text, ((ListViewItem)y).SubItems[col].Text);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            openFileDialog1.FileName = "*.SPR"; 
            openFileDialog1.Filter = "SPR files|*.spr|All files|*.*";
            DialogResult dres = openFileDialog1.ShowDialog();

            //byte[][] wholeFile;
            if (dres == DialogResult.OK)
            {
                string[] allFiles = Directory.GetFiles(Path.GetDirectoryName(openFileDialog1.FileName), "*.SPR", SearchOption.TopDirectoryOnly);
                //wholeFile = new byte[allFiles.Length][];
                sprites = new ChaosImage.Sprite[allFiles.Length];

                //Load all the SPR files in that directory
                for (int x = 0; x < allFiles.Length; x++)
                {
                    int res = ChaosImage.loadSprite(allFiles[x], ref sprites[x]);
                    if (res == 1)
                    {
                        //Report invalid sprite file based on my silly assumptions
                        System.Diagnostics.Debugger.Break();
                        listView1.Items.Add(new ListViewItem(new string[] { allFiles[x], sprites[x].fType.ToString("X"), "NOT", "A", "SPR" }));
                    }
                    else if (res == 0)
                    {
                        //Everything's fiiine
                        listView1.Items.Add(new ListViewItem(new string[] { Path.GetFileNameWithoutExtension(allFiles[x]), sprites[x].fType.ToString("X"), sprites[x].width.ToString(), sprites[x].height.ToString(), sprites[x].subimages.ToString() }));
                    }
                    else
                    {
                        listView1.Items.Add(new ListViewItem(new string[] { allFiles[x], sprites[x].fType.ToString("X"), "UNKNOWN", "ERROR", "" }));
                    }
                }

                if (false)
                {
                    //For debug use only! Outputs *all* the sprites (though for some reason, you have to click in the ListView for it to activate).
                    for (int x = 0; x < listView1.Items.Count; x++)
                    {
                        listView1.Items[x].Selected = true;
                        Application.DoEvents();
                        listView1.Items[x].Selected = false;
                    }
                }
            }
        }

        private void listView1_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            listView1.ListViewItemSorter = new ListViewItemComparer(e.Column);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            openFileDialog1.FileName = "*.PAL";
            openFileDialog1.Filter = "PAL files|*.pal|All files|*.*";
            DialogResult dres = openFileDialog1.ShowDialog();

            if (dres == DialogResult.OK)
            {
                int res = ChaosImage.loadPalette(openFileDialog1.FileName, ref loadedPalette);
                if (res == 1)
                {
                    MessageBox.Show("Oh, you. That's not a Chaos Island PAL file, silly.");
                    return;
                }
                else if (res == 2)
                {
                    MessageBox.Show("The PAL file seems to be missing some data..");
                    return;
                }
                else if (res != 0)
                {
                    MessageBox.Show("Unknown error in PAL file."); //Just in case I forget if I add any more error codes to that function.
                    return;
                }

                //Draw it to a box
                Bitmap imfabulous = new Bitmap(16, 16);
                for (int x = 0; x < loadedPalette.Length && x < 256; x++)
                {
                    imfabulous.SetPixel(x % 16, x >> 4, Color.FromArgb(loadedPalette[x]));
                }
                pictureBox1.Image = imfabulous;
                pictureBox1.Refresh();
            }

        }

        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listView1.SelectedIndices.Count == 0) return;

            if (loadedPalette.Length == 0)
            {
                MessageBox.Show("Please load a palette first... just because.");
                return;
            }

            int t = listView1.SelectedIndices[0]; //For shorter lines of code
            pictureBox2.Width = sprites[t].dataWidth;
            pictureBox2.Height = sprites[t].height * sprites[t].subimages;
            pictureBox2.Image = new Bitmap(pictureBox2.Width, pictureBox2.Height);

            Bitmap[] subimages = new Bitmap[0];
            byte[] outputBuffer = new byte[0];

            int res = ChaosImage.decompressSpriteStage1(sprites[t], ref outputBuffer, loadedPalette, ref subimages);
            /* I used to have various code like this for outputting the data as hex to RichTextBoxes.
                if (textOutput == 2)
                {
                    int selectionStart = rtbOutput.TextLength;
                    rtbOutput.AppendText(outputBuffer2[outputOffset2 - 1].ToString("X2") + (outputColumn == 15 ? "\r\n" : (outputColumn == 7 ? "  " : " ")));
                    outputColumn = (outputColumn + 1) & 15; //Increment but keep between 0 and 7
                    rtbOutput.Select(selectionStart, rtbOutput.TextLength - selectionStart);
                    rtbOutput.SelectionColor = Color.White; //Literals (including anything that came from references in the previous pass) are white

                    ScrollToBottom(rtbInput);
                    ScrollToBottom(rtbOutput);
                }
             */
            res = ChaosImage.decompressSpriteStage2(sprites[t], outputBuffer, loadedPalette, ref subimages);

            //Draw all the subimages, however many there are.
            Graphics g = pictureBox2.CreateGraphics();
            for (int x = 0; x < sprites[t].subimages; x++)
            {
                g.DrawImageUnscaledAndClipped(subimages[x], new Rectangle(0, sprites[t].height * x, subimages[0].Width, subimages[0].Height));
            }
            g.Dispose();
            //pictureBox2.Refresh();
            //Can't do this anymore: ((Bitmap)pictureBox2.Image).Save("data/testOutput/" + listView1.SelectedItems[0].Text + ".png", System.Drawing.Imaging.ImageFormat.Png);
            //But you can totally save the frames from subimages[] if you want.

            //File.WriteAllBytes("data/testOutput/" + listView1.SelectedItems[0].Text + ".bin", outputBuffer2);
        }
    }
}
