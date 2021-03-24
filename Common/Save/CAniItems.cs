using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Security;

namespace Common.Save
{
    [Serializable()] 
    public class CAniItems
    {
        private CompareType m_CType = CompareType.Equal;
        private Double m_Value;
        private Image m_image;
        private Color m_color;
        private Color m_Bcolor;
        private ActionType m_AType= ActionType.TextColor ;
        private Font m_Font;
        private bool m_Roop = true;
        private bool m_Visible = true;
        private string m_Text;

        public CAniItems()
        {

        }

        public CAniItems(CompareType CType, Double Value, Image Simage)
        {
            m_CType = CType;
            m_Value = Value;
            m_image = Simage;
            m_AType = ActionType.Animation;
        }

        public CAniItems(CompareType CType, Double Value, Image Simage, bool Roop)
        {
            m_CType = CType;
            m_Value = Value;
            m_image = Simage;
            m_AType = ActionType.Animation;
            m_Roop = Roop;
        }

        public CAniItems(CompareType CType, Double Value, Color Tcolor, Color Bcolor)
        {
            m_CType = CType;
            m_Value = Value;
            m_color = Tcolor;
            m_Bcolor = Bcolor;
            m_AType = ActionType.TextColor;
        }

        public CAniItems(CompareType CType, Double Value, Color Tcolor, Color Bcolor, Font TFont)
        {
            m_CType = CType;
            m_Value = Value;
            m_color = Tcolor;
            m_Bcolor = Bcolor;
            m_AType = ActionType.TextColor;
            m_Font = TFont;
        }

        public CAniItems(CompareType CType, Double Value, Color Tcolor, Color Bcolor, Font TFont, bool Roop)
        {
            m_CType = CType;
            m_Value = Value;
            m_color = Tcolor;
            m_Bcolor = Bcolor;
            m_AType = ActionType.TextColor;
            m_Font = TFont;
            m_Roop = Roop;
        }

        public CAniItems(CompareType CType, Double Value, Color Tcolor, Color Bcolor, Font TFont, bool Roop, string text, bool visible)
        {
            m_CType = CType;
            m_Value = Value;
            m_color = Tcolor;
            m_Bcolor = Bcolor;
            m_AType = ActionType.TextColor;
            m_Font = TFont;
            m_Roop = Roop;

            m_Text = text;
            m_Visible = visible;
        }

        public CompareType CType
        {
            get { return m_CType; }
            set { m_CType = value; }
        }

        public Double Value
        {
            get { return m_Value; }
            set { m_Value = value; }
        }

        public Image SImage
        {
            get { return m_image; }
            set { m_image = value; }
        }

        public Color TColor
        {
            get { return m_color; }
            set { m_color = value; }
        }

        public Color BColor
        {
            get { return m_Bcolor; }
            set { m_Bcolor = value; }
        }

        public ActionType AType
        {
            get { return m_AType; }
            set { m_AType = value; }
        }

        public Font TFont
        {
            get { return m_Font; }
            set { m_Font = value; }
        }

        public bool Roop
        {
            get { return m_Roop; }
            set { m_Roop = value; }
        }

        public bool Visible
        {
            get { return m_Visible; }
            set { m_Visible = value; }
        }

        public string DisplayText
        {
            get { return m_Text; }
            set { m_Text = value; }
        }
    }

    public enum CompareType
    {
        Equal,
        Bigger,
        Lower,
        OverBigger,
        OverLower
    }

    public enum ActionType
    {
        Animation,
        TextColor,
        DisplayText,
        Visible
    }


}
