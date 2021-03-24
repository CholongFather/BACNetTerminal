using System;
using System.Collections.Generic;
using System.Text;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Windows.Forms;
using Common;
using Common.Save;

namespace GraphicBuilder.Save
{
    [Serializable()]
    public class Cform : Collection<Citems> 
    {
        private string m_Text;
        private Color m_BackColor;
        private Image m_BackgroundImage;
        private ImageLayout m_BackgroundImageLayout;
        private int m_Height;
        private int m_Width;

        public Cform()
        {

        }
        
        public Cform(frmCommon basefrom)
        {
            m_Text = basefrom.Text;
            m_BackColor = basefrom.BackColor;
            m_BackgroundImage = basefrom.BackgroundImage;
            m_BackgroundImageLayout = basefrom.BackgroundImageLayout;
            Height = basefrom.Height;
            Width = basefrom.Width;
        }

        public void GetForm(frmCommon basefrom)
        {
            //basefrom.Text = m_Text;
            basefrom.BackColor = m_BackColor;
            basefrom.BackgroundImage = m_BackgroundImage;
            basefrom.BackgroundImageLayout = m_BackgroundImageLayout;
            basefrom.Height = Height;
            basefrom.Width = Width;
        }

        public int Height
        {
            get { return m_Height; }
            set { m_Height = value; }   
        }

        public int Width
        {
            get { return m_Width; }
            set { m_Width = value; }
        }
    }
}
