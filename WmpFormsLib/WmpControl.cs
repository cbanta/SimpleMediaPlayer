using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace WmpFormsLib
{
    public partial class WmpControl : UserControl
    {
        public WmpControl()
        {
            InitializeComponent();
        }

        private void player_Enter(object sender, EventArgs e)
        {
            this.Parent.Focus();
        }
    }
}