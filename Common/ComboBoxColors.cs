using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using System.Reflection;

namespace Common
{
    public class ComboBoxColors : ComboBox
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public ComboBoxColors()
        {
            // Setup the drawing modes
            DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            DrawItem += new System.Windows.Forms.DrawItemEventHandler(OnDrawItem);

            // Load the colours so the user can show the selected colour at design time
            // However this means the ALL colours will appear in the Windows Form Designer generated code
            // which is a bad thing.
            // LoadAllWindowsColors();
        }


        /// <summary>
        /// Process drawing requests to draw the colour marker and text
        /// </summary>
        private void OnDrawItem(object sender, System.Windows.Forms.DrawItemEventArgs e)
        {
            // Fill in the background
            e.Graphics.FillRectangle(new SolidBrush(e.BackColor), e.Bounds);
            if (e.Index < 0)
                return;

            // Work out where every thing goes
            int nX = e.Bounds.Left;
            int nY = e.Bounds.Top;
            int nMarg = 2;
            int nH = e.Bounds.Height - (2 * nMarg);
            string sText = Items[e.Index].ToString();

            // Draw the Colour Gymph
            Pen penFore = new Pen(e.ForeColor);
            Rectangle rectGymph = new Rectangle(nX + nMarg, nY + nMarg, nH, nH);
            e.Graphics.FillRectangle(new SolidBrush(Color.FromName(sText)), rectGymph);
            e.Graphics.DrawRectangle(penFore, rectGymph);

            // Draw the text
            e.Graphics.DrawString(
                sText, e.Font, new SolidBrush(e.ForeColor),
                nX + nH + (2 * nMarg), e.Bounds.Top);
        }

        /// <summary>
        /// Colour setting and getting Property
        /// </summary>
        public string SelectedColorText
        {
            get
            {
                if (base.SelectedIndex >= 0)
                    return Items[base.SelectedIndex].ToString();
                return "";
            }
            set
            {
                int nIndex = FindStringExact(value);
                if (nIndex >= 0)
                    SelectedIndex = nIndex;
            }
        }

        /// <summary>
        /// Colour setting using a Color object
        /// </summary>
        public Color SelectedColor
        {
            get { return Color.FromName(SelectedColorText); }
            set
            {
                int nIndex = FindStringExact(value.Name);
                if (nIndex >= 0)
                    SelectedIndex = nIndex;
            }
        }

        /// <summary>
        /// Load all the simple colours. 
        /// </summary>
        public void LoadBaseColors()
        {
            Items.Clear();
            PropertyInfo[] aryPI = typeof(Color).GetProperties(BindingFlags.Public | BindingFlags.Static);
            foreach (PropertyInfo pi in aryPI)
                Items.Add(pi.Name);
        }

        /// <summary>
        /// Load all the know colours, this includes windows named colours such as ActiveCaption 
        /// </summary>
        public void LoadAllWindowsColors()
        {
            Items.Clear();
            FieldInfo[] aryPI = typeof(KnownColor).GetFields(BindingFlags.Public | BindingFlags.Static);
            foreach (FieldInfo pi in aryPI)
                Items.Add(pi.Name);
        }
    }
}
