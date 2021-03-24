using System;
using System.Collections.Generic;
using System.Text;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Windows.Forms;
using System.Collections;

namespace Common.Select
{
    [Serializable()]
    public class SelectControls : Collection<BaseControl>
    {
        public ArrayList arrControl = new ArrayList();
        public ArrayList arrType = new ArrayList(); 
        public UndoType m_strUndoTmp;
        
        public int m_PosCnt;

        public void LastSelect()
        {
            int i;

            if (this.Count > 0)
            {
                for (i = 0; i <= this.Count - 1; i++)
                {
                    if (i == (this.Count - 1))
                    {
                        this[i].Selected = true;
                    }
                    else
                    {
                        this[i].Selected = false;
                    }
                }
            }
        }

        //public BaseControl LastSelectControl()
        //{
        //    int i;
        //    BaseControl rtn = new BaseControl();

        //    if (this.Count > 0)
        //    {
        //        for (i = 0; i <= this.Count - 1; i++)
        //        {
        //            if (i == (this.Count - 1))
        //            {
        //                rtn = this[i];
        //                break;
        //            }
        //        }
        //    }

        //    return rtn;
        //}

        public void LastSelectFromMouse()
        {
            int i;
            int nidx=0; 
            if (this.Count > 0)
            {
                for (i = 0; i <= this.Count - 1; i++)
                {
                    this[i].AlignMode = false;

                    if (this[i].Selected == true)
                    {
                        nidx = i;
                    }                    
                }

                this[nidx].AlignMode = true;
                this[nidx].Invalidate();
            }
        }

        public void LastSelectClear()
        {
            int i;
            if (this.Count > 0)
            {
                for (i = 0; i <= this.Count - 1; i++)
                {
                    this[i].AlignMode = false;
                    //this[i].Invalidate();
                }
            }
        }

        public void NameSelect(string cName, bool AddMode)
        {
            int i;
            
            if (this.Count > 0)
            {
                for (i = 0; i <= this.Count - 1; i++)
                {
                    if (this[i].Name == cName)
                    {
                        this[i].Selected = true;
                    }
                    else
                    {
                        if (!AddMode)
                        {
                            this[i].Selected = false;
                        }
                    }
                }
            }
        }

        public int NameSelectNum(string cName, bool AddMode)
        {
            int i;
            int nidx = 0;
            if (this.Count > 0)
            {
                for (i = 0; i <= this.Count - 1; i++)
                {
                    if (this[i].Name == cName)
                    {
                        this[i].Selected = true;
                        nidx = i;
                    }
                    else
                    {
                        if (!AddMode)
                        {
                            this[i].Selected = false;
                        }
                    }
                }
            }

            return nidx;
        }

        public void LastSelectFromMouse(int Pos)
        {
            for (int i = 0; i <= this.Count - 1; i++)
            {
                this[i].AlignMode = false;
            }

            this[Pos].AlignMode = true;
        }

        public void LastSelectFromForm()
        {
            for (int i = 0; i <= this.Count - 1; i++)
            {
                if (this[i].Selected)
                {
                    this[i].AlignMode = true;
                    break;
                }
            }

        }

        public int SelectCount()
        {
            int i;
            int rtn;

            rtn = 0;

            if (this.Count > 0)
            {
                for (i = 0; i <= this.Count - 1; i++)
                {
                    if (this[i].Selected)
                    {
                        rtn = rtn + 1;
                    }
                }
            }

            return rtn;
        }

        public BaseControl SelectControl()
        {
            int i;
            BaseControl rtn = new BaseControl();

            for (i = 0; i <= this.Count - 1; i++)
            {
                if (this[i].Selected)
                {
                    rtn = this[i];
                    break; 
                }
            }

            return rtn;
        }

        public BaseControl LastSelectControl()
        {
            int i;
            BaseControl rtn = null;

            for (i = 0; i <= this.Count - 1; i++)
            {
                if (this[i].AlignMode)
                {
                    rtn = new BaseControl();
                    rtn = this[i];
                    break;
                }
            }

            if (rtn == null)
            {
                rtn = new BaseControl();
                rtn = this[0];
            }

            return rtn;
        }

        public void SelectClear()
        {
            int i;

            for (i = 0; i <= this.Count - 1; i++)
            {
                this[i].Selected = false;
            }
        }


        public void RectangleSelect(int lx, int ly, int rx, int ry)
        {

            int i;

            for (i = 0; i <= this.Count - 1; i++)
            {
                int inPoint = 0;

                Point p1 = new Point(this[i].Left, this[i].Top);
                Point p2 = new Point(this[i].Left + this[i].Width, this[i].Top);
                Point p3 = new Point(this[i].Left, this[i].Top + this[i].Height);
                Point p4 = new Point(this[i].Left + this[i].Width, this[i].Top + this[i].Height);

                if (lx < p1.X && rx > p1.X && ly < p1.Y && ry > p1.Y)
                {
                    inPoint = inPoint + 1;
                }
                if (lx < p2.X && rx > p2.X && ly < p2.Y && ry > p2.Y)
                {
                    inPoint = inPoint + 1;
                }
                if (lx < p3.X && rx > p3.X && ly < p3.Y && ry > p3.Y)
                {
                    inPoint = inPoint + 1;
                }
                if (lx < p4.X && rx > p4.X && ly < p4.Y && ry > p4.Y)
                {
                    inPoint = inPoint + 1;
                }

                if (inPoint >= 4)
                {
                    this[i].Selected = true;                    
                }
                else
                {
                    this[i].Selected = false;

                }

            }

        }

    }

    public struct UndoType
    {
        public Point m_oldPos;
        public Point m_WHPos;
        public Color BackColor;
        public Color ForeColor;
        public Image BackgroundImage;
        public ImageLayout BackgroundImageLayout;
        public BorderStyle BorderStyle;
        public Font Font;
        public bool OnTopMode;
    }
}
