using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ChaosIslandHacking
{
    public partial class Form4 : Form
    {
        ChaosLevel tm;
        int[,] resupplyParams = new int[8 + 5 + 8, 4];
        int previousResupplyParamsIndex = -1;

        public Form4()
        {
            InitializeComponent();
        }

        public void prepareData(ref ChaosLevel theMap)
        {
            //TODO: Make all controls match the given map data
            chkUseTileset.Checked = tm.useTileset;

            tm = theMap;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            //TODO: Save all changes to "tm"
            tm.useTileset = chkUseTileset.Checked;

            Close();
        }

        private void cboResupplyParamsForWhom_Validating(object sender, CancelEventArgs e)
        {
            //TODO: Temporarily store changes for previousResupplyParamsIndex
            //TODO: Display data for cboResupplyParamsForWhom.SelectedIndex (0-7 is rogue dinos, 8-12 is hunters, 13+ is hunter dinos)
        }
    }
}
