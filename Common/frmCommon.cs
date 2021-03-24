using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Common.Select;

namespace Common
{
    [Serializable()]
    public partial class frmCommon : Form
    {
        public bool KAlt = false;
        public string SelectControl = "";
        public SelectControls SelItems = new SelectControls();
        public string BackImage = "";
        //public SelectControls tmpSel = new SelectControls();

        public int cx;
        public int cy;


        public frmCommon()
        {
            InitializeComponent();
        }
    }
}