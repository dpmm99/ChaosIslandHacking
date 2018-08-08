using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace ChaosIslandHacking
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private struct HeaderNode
        {
            public int possiblyNodeType;
            public int fileLoc;
            public string fileID;
        }

        private int getInt(byte[] arr, ref int nPos)
        {
            int res = (int)arr[nPos] + ((int)arr[nPos + 1] << 8) + ((int)arr[nPos + 2] << 16) + ((int)arr[nPos + 3] << 24);
            nPos += 4;
            return res;
        }

        private string getString(byte[] arr, ref int nPos)
        {
            string res = "";
            if (arr[nPos] == '\0') return ""; //Short-circuit if it's a zero-length string.
            do
            {
                res += Convert.ToChar(arr[nPos]).ToString();
            } while (arr[++nPos] != '\0');
            nPos++; //Increment it once more after you find the \0
            return res;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            DialogResult dres = openFileDialog1.ShowDialog();

            if (dres == DialogResult.OK)
            {
                //Load whole file (even though it may be 300 MB! People with garbage PCs beware)
                byte[] wholeFile = File.ReadAllBytes(openFileDialog1.FileName);
                if (wholeFile[0] != 'F' || wholeFile[1] != 'S' || wholeFile[2] != 'H' || wholeFile[3] != '2')
                {
                    MessageBox.Show("Oh, you. That's not a Chaos Island CFS file, silly.");
                    return;
                }

                int nPos = 4;
                //TODO: Root node is different. May be multiple types of nodes.
                #region RootNode
                int blockLen = getInt(wholeFile, ref nPos);
                int numberOfFiles = getInt(wholeFile, ref nPos); //C5 02 00 00 for root node in disk2, B9 in disk1. Probably the number of files (I'm guessing).

                int possiblyFileTypeA = getInt(wholeFile, ref nPos); //0s for root node
                int possiblyFileTypeB = getInt(wholeFile, ref nPos); //0s for root node
                int unknown1 = getInt(wholeFile, ref nPos); //0s for root node

                int reserved = getInt(wholeFile, ref nPos); //01 00 00 00 for root node
                string nodeName = getString(wholeFile, ref nPos); //"Root\0" for root node
                int unknown2 = getInt(wholeFile, ref nPos); //61 6D 5C 6C for root node in disk2, 0s in disk1
                #endregion

                HeaderNode[] endNodes = {};
                while (nPos < wholeFile.Length && endNodes.Length < numberOfFiles)
                {
                    Array.Resize(ref endNodes, endNodes.Length + 1);
                    blockLen = getInt(wholeFile, ref nPos);
                    //Here, if you add blockLen to nPos, you get the position of the next block's blockLen.
                    endNodes[endNodes.Length - 1].possiblyNodeType = getInt(wholeFile, ref nPos);
                    if (endNodes[endNodes.Length - 1].possiblyNodeType != 2) System.Diagnostics.Debugger.Break(); //All 2s in both disk1 and disk2
                    endNodes[endNodes.Length - 1].fileLoc = getInt(wholeFile, ref nPos); //Location of the file descriptor within this file
                    endNodes[endNodes.Length - 1].fileID = getString(wholeFile, ref nPos);
                }

                toolStripStatusLabel1.Text = "Read " + endNodes.Length + " nodes, stopped at " + nPos;

                //Get the actual file data
                for (int x = 0; x < endNodes.Length; x++)
                {
                    nPos = endNodes[x].fileLoc; //Locate the file descriptor
                    blockLen = getInt(wholeFile, ref nPos); //Number of bytes in the descriptor (before the actual file data starts)
                    int fileDataStart = getInt(wholeFile, ref nPos); //File data starts at the location listed here
                    int fileDataEnd = getInt(wholeFile, ref nPos); //First byte after the file data ends, so you can subtract those directly to get the size (no "- 1" or anything)
                    possiblyFileTypeA = getInt(wholeFile, ref nPos); //00 4A 43 39 for the first AUD in disk1, for example
                    possiblyFileTypeB = getInt(wholeFile, ref nPos); //5C BA BC 01 for the frist AUD in disk1, for example
                    reserved = getInt(wholeFile, ref nPos); //All 0s in both disk1 and disk2
                    int possiblyNodeTypeAgain = getInt(wholeFile, ref nPos); //All 2s in both disk1 and disk2
                    string fileName = getString(wholeFile, ref nPos);
                    int reserved2 = getInt(wholeFile, ref nPos); //Mostly 0s, some weird data.

                    //Display it!
                    listView1.Items.Add(new ListViewItem(new string[] { fileName, possiblyFileTypeA.ToString("X"), possiblyFileTypeB.ToString("X"), reserved2.ToString("X")}));
                    //Extract it!
                    byte[] outFile = new byte[fileDataEnd - fileDataStart];
                    Array.Copy(wholeFile, fileDataStart, outFile, 0, fileDataEnd - fileDataStart);
                    File.WriteAllBytes("Data/" + fileName, outFile);
                    //Done with this one!
                }

            }
        }

        //TODO: Make a CFS re-archiver. You'll find out sooner or later if all that unknown data in the file descriptors and such is actually important.

        private void Form1_Resize(object sender, EventArgs e)
        {
            listView1.Width = this.ClientSize.Width - 24;
            listView1.Height = statusStrip1.Top - listView1.Top - 16;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            (new Form2()).Show();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            (new Form3()).Show();
        }
    }
}
