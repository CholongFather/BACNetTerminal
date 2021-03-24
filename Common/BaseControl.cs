using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using Common.Save;
using System.Drawing.Drawing2D;
using System.Threading;

namespace Common
{
    [Serializable()]
    public partial class BaseControl : UserControl
    {
        private Thread DataManager;

        public BaseControl()
        {
            InitializeComponent();

        }

        public BaseControl(ControlType CoType)
        {
            // 이 호출은 Windows Form 디자이너에 필요합니다. 
            InitializeComponent();

            SetStyle(ControlStyles.UserPaint, true);
            SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            SetStyle(ControlStyles.DoubleBuffer, true);
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);

            // InitializeComponent() 호출 뒤에 초기화 코드를 추가하십시오. 
            m_ConType = CoType;
            InitControl();

            this.ForeColor = Color.Black;

            switch (CoType)
            {
                case ControlType.LinkBox:
                case ControlType.LinkButton:
                    this.Cursor = Cursors.Hand;
                    break;
                default:
                    break;
            }
        }

        public Color m_FillColor = Color.GreenYellow;

        private bool m_Selected = false;
        private bool m_AlignMode = false;
        private bool m_Mode = false;
        private bool m_AniMode = false;

        private bool m_DisplayMode = false;
        private bool m_TooltipMode = false;

        private ControlType m_ConType;
        private ContentAlignment m_ValueAlign = ContentAlignment.TopLeft;
        private CAniCollection m_aitems = new CAniCollection();

        private string m_Value = "";
        private string m_PLink = "";
        private string m_Kind_Code = "";
        private string m_tag_id = "";
        private string m_tag_name = "";
        private string m_tag_desc = "";
        private string m_tag_unit = "";
        private string m_tag_type = "";
        private string m_sif_id = "";
        private string m_tag_ttype = "";
        private string m_tag_objid = "";
        private string m_tag_objid2 = "";
        private string m_tag_objid3 = "";
        private string m_tag_objid4 = "";
        private string m_tag_value1 = "";
        private string m_tag_value2 = "";
        private string m_tag_value3 = "";
        private string m_tag_lo = "";
        private string m_tag_lolo = "";
        private string m_tag_hi = "";
        private string m_tag_hihi = "";
        private string m_tag_on = "";
        private string m_tag_off = "";
        private string m_tag_limithi = "";
        private string m_tag_limitlo = "";
        private string m_tag_addr = "";
        private string m_tag_initval = "";
        private string m_tag_use_cos = "";
        private string m_tag_use_trend = "";

        private bool m_tag_is_single = false;
        private string m_tag_single_value = "OFF";

        private ActionModeType m_ActType;
        private GifImage m_img;
        private Image bmpGIF;
        private bool m_ContinueMode = true;

        private bool m_Protect;

        private bool m_OnTop = false;

        private bool m_bRound = false;

        private string m_sConn = "";
        private string m_sServerIP = "";
        private string m_sDatabase = "";
        private string m_sUser = "";
        private string m_sPassword = "";

        private ConnectType m_CType = ConnectType.Table;
        private SearchType m_SType = SearchType.Day;

        private string m_sTableName = "";
        private string m_sQuery = "";
        private string m_sDateField = "";
        private string m_sDateFormat = "yyyy-MM-dd";
        private string m_sOrderby = "";

        private PartTrendChart m_TrendChart = new PartTrendChart();

        public string sConn
        {
            get { return m_sConn = "server=" + m_sServerIP + ";database=" + m_sDatabase + ";uid=" + m_sUser + ";pwd=" + m_sPassword + ";"; }
        }
        public ConnectType CType
        {
            get { return m_CType; }
            set { m_CType = value; }
        }
        public SearchType SType
        {
            get { return m_SType; }
            set { m_SType = value; }
        }

        public string sServerIP
        {
            get { return m_sServerIP; }
            set { m_sServerIP = value; }
        }
        public string sDatabase
        {
            get { return m_sDatabase; }
            set { m_sDatabase = value; }
        }
        public string sUser
        {
            get { return m_sUser; }
            set { m_sUser = value; }
        }
        public string sPassword
        {
            get { return m_sPassword; }
            set { m_sPassword = value; }
        }
        public string sTableName
        {
            get { return m_sTableName; }
            set { m_sTableName = value; }
        }
        public string sQuery
        {
            get { return m_sQuery; }
            set { m_sQuery = value; }
        }
        public string sDateField
        {
            get { return m_sDateField; }
            set { m_sDateField = value; }
        }
        public string sDateFormat
        {
            get { return m_sDateFormat; }
            set { m_sDateFormat = value; }
        }
        public string sOrderby
        {
            get { return m_sOrderby; }
            set { m_sOrderby = value; }
        }

        public bool TagInfoClear()
        {
            m_Value = "";
            m_PLink = "";
            m_Kind_Code = "";
            m_tag_id = "";
            m_tag_name = "";
            m_tag_desc = "";
            m_tag_unit = "";
            m_tag_type = "";
            m_sif_id = "";
            m_tag_ttype = "";
            m_tag_objid = "";
            m_tag_objid2 = "";
            m_tag_objid3 = "";
            m_tag_objid4 = "";
            m_tag_value1 = "";
            m_tag_value2 = "";
            m_tag_value3 = "";
            m_tag_lo = "";
            m_tag_lolo = "";
            m_tag_hi = "";
            m_tag_hihi = "";
            m_tag_on = "";
            m_tag_off = "";
            m_tag_limithi = "";
            m_tag_limitlo = "";
            m_tag_addr = "";
            m_tag_initval = "";
            m_tag_use_cos = "";
            m_tag_use_trend = "";

            m_tag_is_single = false;
            m_tag_single_value = "";

            return true;
        }

        public bool OnTop
        {
            get { return m_OnTop; }
            set { m_OnTop = value; }
        }

        public bool Protect
        {
            get { return m_Protect; }
            set { m_Protect = value; }
        }

        public string tag_initval
        {
            get { return m_tag_initval; }
            set { m_tag_initval = value; }
        }

        public string Hi
        {
            get { return m_tag_hi; }
            set { m_tag_hi = value; }
        }

        public string Lo
        {
            get { return m_tag_lo; }
            set { m_tag_lo = value; }
        }

        public string Hihi
        {
            get { return m_tag_hihi; }
            set { m_tag_hihi = value; }
        }

        public string Lolo
        {
            get { return m_tag_lolo; }
            set { m_tag_lolo = value; }
        }

        public string On
        {
            get { return m_tag_on; }
            set { m_tag_on = value; }
        }

        public string Off
        {
            get { return m_tag_off; }
            set { m_tag_off = value; }
        }

        public string LimitHi
        {
            get { return m_tag_limithi; }
            set { m_tag_limithi = value; }
        }

        public string LimitLo
        {
            get { return m_tag_limitlo; }
            set { m_tag_limitlo = value; }
        }

        public string Addr
        {
            get { return m_tag_addr; }
            set { m_tag_addr = value; }
        }

        public bool DisignMode
        {
            get { return m_Mode; }
            set { m_Mode = value; }
        }

        public string Kind_Code
        {
            get { return m_Kind_Code; }
            set { m_Kind_Code = value; }
        }

        public string tag_id
        {
            get { return m_tag_id; }
            set { m_tag_id = value; }
        }

        public string tag_name
        {
            get { return m_tag_name; }
            set { m_tag_name = value; }
        }

        public string tag_desc
        {
            get { return m_tag_desc; }
            set { m_tag_desc = value; }
        }

        public string tag_unit
        {
            get { return m_tag_unit; }
            set { m_tag_unit = value; }
        }

        public string tag_type
        {
            get { return m_tag_type; }
            set
            {
                m_tag_type = value;

                if (!m_Mode)
                {
                    if (m_tag_type == "02" || m_tag_type == "04")
                        this.Cursor = Cursors.Hand;
                }
            }
        }

        public string tag_ttype
        {
            get { return m_tag_ttype; }
            set { m_tag_ttype = value; }
        }

        public string sif_id
        {
            get { return m_sif_id; }
            set { m_sif_id = value; }
        }

        public string tag_objid
        {
            get { return m_tag_objid; }
            set { m_tag_objid = value; }
        }

        public string tag_objid2
        {
            get { return m_tag_objid2; }
            set { m_tag_objid2 = value; }
        }

        public string tag_objid3
        {
            get { return m_tag_objid3; }
            set { m_tag_objid3 = value; }
        }

        public string tag_objid4
        {
            get { return m_tag_objid4; }
            set { m_tag_objid4 = value; }
        }

        public string tag_value1
        {
            get { return m_tag_value1; }
            set { m_tag_value1 = value; }
        }

        public string Format
        {
            get { return m_tag_value1; }
            set { m_tag_value1 = value; }
        }

        public string tag_value2
        {
            get { return m_tag_value2; }
            set { m_tag_value2 = value; }
        }

        public string tag_value3
        {
            get { return m_tag_value3; }
            set { m_tag_value3 = value; }
        }

        public string tag_use_cos
        {
            get { return m_tag_use_cos; }
            set { m_tag_use_cos = value; }
        }

        public string tag_use_trend
        {
            get { return m_tag_use_trend; }
            set { m_tag_use_trend = value; }
        }

        public bool tag_is_single
        {
            get { return m_tag_is_single; }
            set { m_tag_is_single = value; }
        }

        public string tag_single_value
        {
            get { return m_tag_single_value; }
            set { m_tag_single_value = value; }
        }

        public string PageLink
        {
            get { return m_PLink; }
            set { m_PLink = value; }
        }

        public CAniCollection AItems
        {
            get { return m_aitems; }
            set { m_aitems = value; }
        }

        public ContentAlignment ValueAlign
        {
            get { return m_ValueAlign; }
            set { m_ValueAlign = value; }
        }

        public string Value
        {
            get { return m_Value; }
            set
            {
                if (value != m_Value)
                {
                    m_Value = value;

                    if (m_Mode == false)
                    {
                        if (ConType != ControlType.LevelGuage && ConType != ControlType.LevelGuage_X)
                            AppendDoAction(); // DoAction(); // 

                        //if (ConType != ControlType.LevelGuage && ConType != ControlType.LevelGuage_X)
                        //    DoAction(); // 
                        AnimateImage();
                        this.Invalidate();
                    }

                    this.Invalidate();
                }
            }
        }

        public delegate void UpdateDoAction();

        private void AppendDoAction()
        {
            if (InvokeRequired)
            {
                this.BeginInvoke(new UpdateDoAction(DoAction), null);
            }
            else
            {
                // This is the main thread which created this control, hence update it
                // directly 
                DoAction();
            }
        }

        public ActionModeType ActType
        {
            get { return m_ActType; }
            set { m_ActType = value; }
        }

        public bool Selected
        {
            get { return m_Selected; }
            set
            {
                m_Selected = value;
                //if( m_Mode == false)
                //this.Invalidate();
            }
        }

        public bool AlignMode
        {
            get { return m_AlignMode; }
            set
            {
                m_AlignMode = value;
                if (m_Mode == false)
                    this.Invalidate();
            }
        }

        public bool TooltipMode
        {
            get { return this.m_TooltipMode; }
            set { this.m_TooltipMode = value; }
        }

        public ControlType ConType
        {
            get { return m_ConType; }
            set { m_ConType = value; }
        }

        public bool AniMode
        {
            get { return m_AniMode; }
            set { m_AniMode = value; }
        }

        public void NextImageDisplay()
        {
            if (!m_Mode && m_img != null)
            {
                //if (m_img.ContinueMode == true)
                {
                    this.BackgroundImage = m_img.GetNextFrame();
                    this.Invalidate();
                }
            }
        }

        public bool SetRound
        {
            get { return m_bRound; }
            set
            {
                if (m_bRound != value)
                {
                    m_bRound = value;
                    this.Invalidate();
                }
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public PartTrendChart TrendChart
        {
            get
            {
                if (this.m_TrendChart == null)
                {
                    this.m_TrendChart = new PartTrendChart();
                }
                return this.m_TrendChart;
            }
        }

        private void InitControl()
        {
            switch (ConType)
            {
                case ControlType.Box:
                case ControlType.PointText:
                case ControlType.LevelGuage:
                case ControlType.LevelGuage_X:
                case ControlType.LinkButton:
                case ControlType.LinkBox:
                    this.BorderStyle = BorderStyle.FixedSingle;
                    break;
                case ControlType.Label:
                case ControlType.Picture:
                case ControlType.PointPicture:
                    this.BorderStyle = BorderStyle.None;
                    break;
            }

            m_ValueAlign = ContentAlignment.MiddleCenter;

        }


        public void AnimateImage()
        {
            if (bmpGIF != null && m_Mode == false)
            {
                m_img = new GifImage(bmpGIF);
                m_img.ContinueMode = m_ContinueMode;

                if (this.AniMode == false && m_img.FrameCount >= 1)
                {
                    this.BackgroundImage = m_img.GetNextFrame();//.GetFrame(0);
                    this.AniMode = true;

                    this.Invalidate();
                }
                else
                {
                    if (this.AniMode == true)
                    {
                        this.BackgroundImage = bmpGIF;
                        this.AniMode = false;
                    }
                }
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            if (m_Mode)
            {
                // 객체에 속성을 상단에 이미지로 표현 한다. 선택, 태그연결, 컨드롤 타입 등등
                // 0 : 선택
                // 1 : 컨트롤 타입
                // 2 : 링크, 태그 연결(AI, AO, DI, DO)
                // type 0
                if (this.m_Selected == true)
                {
                    // 선택 여부 기준 여부~
                    if (this.m_AlignMode == true)
                        e.Graphics.DrawImage(SIResource.Pchecked, this.Width - SIResource.Pchecked.Width - 4, 2);
                    else
                        e.Graphics.DrawImage(SIResource.Checked, this.Width - SIResource.Checked.Width - 4, 2);
                }

                // Type 1
                // Control Type
                Image img = null;
                switch (m_ConType)
                {
                    case ControlType.Label: img = SIResource.label; break;
                    case ControlType.Box: img = SIResource.box; break;
                    case ControlType.Picture: img = SIResource.picture; break;
                    case ControlType.PointText: img = SIResource.pointtext; break;
                    case ControlType.LinkBox: img = SIResource.pointbox; break;
                    case ControlType.PointPicture: img = SIResource.pointpicture; break;
                    case ControlType.LevelGuage: img = SIResource.Level; break;
                    case ControlType.LevelGuage_X: img = SIResource.level_x; break;
                    case ControlType.LinkButton:
                        DrawButton(e.Graphics);
                        img = SIResource.btn; break;

                    // 수정
                    case ControlType.iexplorer: img = SIResource.btn; break;
                    case ControlType.ListCTL: img = SIResource.btn; break;
                    case ControlType.TrendChart: img = SIResource.TrendChart; break;
                    default: img = SIResource.box; break;
                }

                e.Graphics.DrawImage(img, 2, 2);

                // type 2                                
                switch (m_ConType)
                {
                    case ControlType.Box:
                    case ControlType.Picture:
                    case ControlType.Label:
                    case ControlType.ListCTL:
                        break;
                    case ControlType.LinkButton:
                    case ControlType.LinkBox:
                    case ControlType.iexplorer:
                        // 링크 상태 표시
                        if (this.m_PLink.Length > 0)
                            e.Graphics.DrawImage(SIResource.PageLink, 2, 2);
                        else
                            e.Graphics.DrawImage(SIResource.unlinked, 2, 2);
                        break;
                    case ControlType.PointPicture:
                    case ControlType.PointText:
                    case ControlType.LevelGuage:
                    case ControlType.LevelGuage_X:
                    case ControlType.TrendChart:
                        // 태그타입별
                        if (this.m_tag_id.Length > 0)
                        {
                            switch (this.m_tag_type.ToString())
                            {
                                case "01": img = SIResource.AI; break;
                                case "02": img = SIResource.AO; break;
                                case "03": img = SIResource.DI; break;
                                case "04": img = SIResource.DO; break;
                                case "05":
                                default: img = SIResource.etc; break;
                            }
                            e.Graphics.DrawImage(img, 14, 2);
                        }
                        else
                            e.Graphics.DrawImage(SIResource.unlinked, 14, 2);

                        break;
                }

                DrawValue(e.Graphics);
            }
            else
            {
                // 데이터 draw
                switch (m_ConType)
                {
                    case ControlType.Box:
                    case ControlType.Label:
                    case ControlType.Picture:
                    case ControlType.LinkBox:
                        DrawValue(e.Graphics);
                        break;
                    case ControlType.PointText:
                        if (!string.IsNullOrEmpty(m_tag_id))
                            DrawValue(e.Graphics);
                        break;
                    case ControlType.LinkButton:
                        DrawButton(e.Graphics);
                        DrawValue(e.Graphics);
                        break;
                    case ControlType.LevelGuage:
                        DrawGuage(e.Graphics);
                        DrawValue(e.Graphics);
                        break;
                    case ControlType.LevelGuage_X:
                        DrawGuage_X(e.Graphics);
                        DrawValue(e.Graphics);
                        break;
                    case ControlType.PointPicture:
                    case ControlType.iexplorer:
                    case ControlType.ListCTL:
                        break;
                    case ControlType.TrendChart:
                        this.TrendChart.DrawChart(e.Graphics, this.ClientRectangle, this.tag_name);
                        break;
                }

                if (m_bRound)
                {
                    DrawRound(e.Graphics);
                }
            }

            base.OnPaint(e);
        }

        private void DrawValue(Graphics g)
        {
            if (m_ActType == ActionModeType.DigitalImage)
                return;

            Rectangle rect = new Rectangle();
            StringFormat sFormat = new StringFormat();

            Pen mypen = new Pen(this.ForeColor);
            if (this.BackgroundImage == null)
            {
                //g.Clear(this.BackColor);
            }

            rect.X = 0;
            rect.Y = 0;
            rect.Width = this.Width;
            rect.Height = this.Height;

            switch (this.m_ValueAlign)
            {
                case ContentAlignment.TopLeft:
                    sFormat.LineAlignment = StringAlignment.Near;
                    sFormat.Alignment = StringAlignment.Near;
                    break;
                case ContentAlignment.TopCenter:
                    sFormat.LineAlignment = StringAlignment.Near;
                    sFormat.Alignment = StringAlignment.Center;
                    break;
                case ContentAlignment.TopRight:
                    sFormat.LineAlignment = StringAlignment.Near;
                    sFormat.Alignment = StringAlignment.Far;
                    break;
                case ContentAlignment.MiddleLeft:
                    sFormat.LineAlignment = StringAlignment.Center;
                    sFormat.Alignment = StringAlignment.Near;
                    break;
                case ContentAlignment.MiddleCenter:
                    sFormat.LineAlignment = StringAlignment.Center;
                    sFormat.Alignment = StringAlignment.Center;
                    break;
                case ContentAlignment.MiddleRight:
                    sFormat.LineAlignment = StringAlignment.Center;
                    sFormat.Alignment = StringAlignment.Far;
                    break;
                case ContentAlignment.BottomLeft:
                    sFormat.LineAlignment = StringAlignment.Far;
                    sFormat.Alignment = StringAlignment.Near;
                    break;
                case ContentAlignment.BottomCenter:
                    sFormat.LineAlignment = StringAlignment.Far;
                    sFormat.Alignment = StringAlignment.Center;
                    break;
                case ContentAlignment.BottomRight:
                    sFormat.LineAlignment = StringAlignment.Far;
                    sFormat.Alignment = StringAlignment.Far;
                    break;
            }

            string szText = m_Value;

            if (!string.IsNullOrEmpty(this.LimitLo) && this.LimitLo != "0" && !string.IsNullOrEmpty(m_Value))
            {
                double fVal = Convert.ToDouble(m_Value) / Convert.ToDouble(this.LimitLo);
                m_Value = fVal.ToString();
                szText = m_Value;
            }

            if (!string.IsNullOrEmpty(this.LimitHi) && this.LimitHi != "0" && !string.IsNullOrEmpty(m_Value))
            {
                double fVal = Convert.ToDouble(m_Value) * Convert.ToDouble(this.LimitHi);
                m_Value = fVal.ToString();
                szText = m_Value;
            }

            if (this.Format.Length > 0 && !string.IsNullOrEmpty(m_Value))
            {
                double fVal = Convert.ToDouble(m_Value);
                m_Value = fVal.ToString("F" + this.Format);
                szText = m_Value;
            }

            if (!string.IsNullOrEmpty(this.tag_unit) && !string.IsNullOrEmpty(m_Value))
                szText = m_Value + " " + this.tag_unit + " ";

            if (m_DisplayMode)
                szText = this.Text;

            g.DrawString(szText, this.Font, mypen.Brush, rect, sFormat);

        }

        private void DrawGuage(Graphics g)
        {
            if (m_ActType == ActionModeType.DigitalImage)
                return;

            Rectangle rect = new Rectangle();
            StringFormat sFormat = new StringFormat();

            Pen mypen = new Pen(this.ForeColor);

            rect.X = 0;
            rect.Y = 0;
            rect.Width = this.Width;
            rect.Height = this.Height;

            GetGuageFillColor();

            SolidBrush mBry = new SolidBrush(this.m_FillColor);

            Double Tmp = 0;

            if (m_Value.Length == 0)
                m_Value = "0";

            Tmp = Convert.ToDouble(m_Value);
            if (Tmp < 0)
                Tmp = 0;
            if (Tmp > 100)
                Tmp = 100;

            int Val = 0;

            if (!string.IsNullOrEmpty(m_tag_limithi) && m_tag_limithi != "0")
                Val = Convert.ToInt32((Convert.ToDouble(m_Value) / Convert.ToInt32(m_tag_limithi)) * Convert.ToDouble(this.Height));
            else
                Val = Convert.ToInt32((Convert.ToDouble(m_Value) / 100.0d) * Convert.ToDouble(this.Height));

            rect.Y = this.Height - Val;
            rect.Height = Val;

            //if (!string.IsNullOrEmpty(m_tag_limithi) && m_tag_limithi != "0")
            //    Val = Convert.ToInt32((Convert.ToDouble(m_Value) / Convert.ToInt32(m_tag_limithi)) * Convert.ToDouble(this.Width));
            //else
            //    Val = Convert.ToInt32((Convert.ToDouble(m_Value) / 100.0d) * Convert.ToDouble(this.Width));

            //rect.X = 0;
            //rect.Width = Val;

            g.FillRectangle(mBry, rect);
        }

        private void DrawRound(Graphics g)
        {
            //if (m_ActType == ActionModeType.DigitalImage)
            //    return;

            Rectangle rect = new Rectangle();
            StringFormat sFormat = new StringFormat();

            Pen mypen = new Pen(Color.Chartreuse, (float)2);

            rect.X = 0;
            rect.Y = 0;
            rect.Width = this.Width;
            rect.Height = this.Height;

            g.DrawRectangle(mypen, rect);
        }

        private void DrawGuage_X(Graphics g)
        {
            if (m_ActType == ActionModeType.DigitalImage)
                return;

            Rectangle rect = new Rectangle();
            StringFormat sFormat = new StringFormat();

            Pen mypen = new Pen(this.ForeColor);

            rect.X = 0;
            rect.Y = 0;
            rect.Width = this.Width;
            rect.Height = this.Height;

            GetGuageFillColor();

            SolidBrush mBry = new SolidBrush(this.m_FillColor);

            Double Tmp = 0;

            if (m_Value.Length == 0)
                m_Value = "0";

            Tmp = Convert.ToDouble(m_Value);
            if (Tmp < 0)
                Tmp = 0;
            if (Tmp > 100)
                Tmp = 100;

            int Val = 0;

            if (!string.IsNullOrEmpty(m_tag_limithi) && m_tag_limithi != "0")
                Val = Convert.ToInt32((Convert.ToDouble(m_Value) / Convert.ToInt32(m_tag_limithi)) * Convert.ToDouble(this.Width));
            else
                Val = Convert.ToInt32((Convert.ToDouble(m_Value) / 100.0d) * Convert.ToDouble(this.Width));

            rect.X = 0;
            rect.Width = Val;

            g.FillRectangle(mBry, rect);
        }

        private void GetGuageFillColor()
        {
            Color fillcolor = m_FillColor;

            if (m_tag_type == "01" || m_tag_type == "02")
            {
                if (m_tag_hihi == "0" && m_tag_hi == "0" && m_tag_lo == "0" && m_tag_lolo == "0")
                    return;

                // 데이터 Setting
                if (m_ActType == ActionModeType.AnalogHiLo)
                {
                    foreach (CAniItems ca in AItems)
                    {
                        if (ca.CType == CompareType.OverLower)
                        {
                            if (Convert.ToDouble(m_Value) <= ca.Value)
                            {
                                this.m_FillColor = ca.BColor;
                                this.ForeColor = ca.TColor;
                                this.Font = ca.TFont;
                                break;
                            }
                        }

                        if (ca.CType == CompareType.Lower)
                        {
                            if (Convert.ToDouble(m_Value) < ca.Value)
                            {
                                this.m_FillColor = Color.Honeydew;
                                this.ForeColor = ca.TColor;
                                this.Font = ca.TFont;
                                break;
                            }
                        }

                        if (ca.CType == CompareType.OverBigger)
                        {
                            if (Convert.ToDouble(m_Value) >= ca.Value)
                            {
                                this.m_FillColor = ca.BColor;
                                this.ForeColor = ca.TColor;
                                this.Font = ca.TFont;
                                break;
                            }
                        }

                        if (ca.CType == CompareType.Equal)
                        {
                            if (Convert.ToDouble(m_Value) == ca.Value)
                            {
                                this.m_FillColor = ca.BColor;
                                this.ForeColor = ca.TColor;
                                this.Font = ca.TFont;
                                this.Text = ca.DisplayText;
                                this.Visible = ca.Visible;
                                break;
                            }
                        }
                    }
                }
                else
                {
                    foreach (CAniItems ca in AItems)
                    {
                        if (ca.CType == CompareType.OverLower)
                        {
                            if (Convert.ToDouble(m_Value) <= ca.Value)
                            {
                                this.m_FillColor = ca.BColor;
                                this.ForeColor = ca.TColor;
                                this.Font = ca.TFont;
                                break;
                            }
                        }

                        if (ca.CType == CompareType.Bigger)
                        {
                            if (Convert.ToDouble(m_Value) > ca.Value)
                            {
                                this.m_FillColor = ca.BColor;
                                this.ForeColor = ca.TColor;
                                this.Font = ca.TFont;
                                break;
                            }
                        }

                        if (ca.CType == CompareType.Equal)
                        {
                            if (Convert.ToDouble(m_Value) == ca.Value)
                            {
                                this.m_FillColor = ca.BColor;
                                this.ForeColor = ca.TColor;
                                this.Font = ca.TFont;
                                this.Text = ca.DisplayText;
                                this.Visible = ca.Visible;
                                break;
                            }
                        }
                    }
                }
            }
            //else
            //{
            //    if (m_ActType == ActionModeType.DigitalText)
            //    {
            //        foreach (CAniItems ca in AItems)
            //        {
            //            if (ca.CType == CompareType.OverLower)
            //            {
            //                if (Convert.ToDouble(m_Value) <= ca.Value)
            //                {
            //                    this.m_FillColor = ca.BColor;
            //                    break;
            //                }
            //            }

            //            if (ca.CType == CompareType.Equal)
            //            {
            //                if (Convert.ToDouble(m_Value) == ca.Value)
            //                {
            //                    this.m_FillColor = ca.BColor;
            //                    break;
            //                }
            //            }

            //            if (ca.CType == CompareType.OverBigger)
            //            {
            //                if (Convert.ToDouble(m_Value) >= ca.Value)
            //                {
            //                    this.m_FillColor = ca.BColor;
            //                    break;
            //                }

            //            }
            //        }
            //    }
            //}
        }

        private void BaseControl_Paint(object sender, PaintEventArgs e)
        {
        }

        private void BaseControl_Resize(object sender, EventArgs e)
        {
            this.Invalidate();
        }

        private void DoAction()
        {
            if (m_tag_type == "01" || m_tag_type == "02")
            {
                //if (m_tag_hihi == "0" && m_tag_hi == "0" && m_tag_lo == "0" && m_tag_lolo == "0")
                //    return;

                foreach (CAniItems ca in AItems)
                {
                    // Normal
                    if (ca.CType == CompareType.Lower)
                    {
                        if (Convert.ToDouble(m_Value) < ca.Value)
                        {
                            this.ForeColor = ca.TColor;
                            this.BackColor = ca.BColor;
                            this.Font = ca.TFont;

                            if (ca.AType == ActionType.DisplayText)
                            {
                                m_DisplayMode = true;
                                this.Text = ca.DisplayText;
                            }
                            else
                                m_DisplayMode = false;

                            if (ca.AType == ActionType.Visible)
                                this.Visible = ca.Visible;

                            if (ca.AType == ActionType.Animation)
                            {
                                this.BackgroundImage = ca.SImage;
                                m_ActType = ActionModeType.DigitalImage;
                            }

                            break;
                        }
                    }

                    if (ca.CType == CompareType.OverLower)
                    {
                        if (Convert.ToDouble(m_Value) <= ca.Value)
                        {
                            this.ForeColor = ca.TColor;
                            this.BackColor = ca.BColor;
                            this.Font = ca.TFont;

                            if (ca.AType == ActionType.DisplayText)
                            {
                                m_DisplayMode = true;
                                this.Text = ca.DisplayText;
                            }
                            else
                                m_DisplayMode = false;

                            if (ca.AType == ActionType.Visible)
                                this.Visible = ca.Visible;

                            if (ca.AType == ActionType.Animation)
                            {
                                this.BackgroundImage = ca.SImage;
                                m_ActType = ActionModeType.DigitalImage;
                            }

                            break;
                        }
                    }

                    if (ca.CType == CompareType.OverBigger)
                    {
                        if (Convert.ToDouble(m_Value) >= ca.Value)
                        {
                            this.ForeColor = ca.TColor;
                            this.BackColor = ca.BColor;
                            this.Font = ca.TFont;

                            if (ca.AType == ActionType.DisplayText)
                            {
                                m_DisplayMode = true;
                                this.Text = ca.DisplayText;
                            }
                            else
                                m_DisplayMode = false;

                            if (ca.AType == ActionType.Visible)
                                this.Visible = ca.Visible;

                            if (ca.AType == ActionType.Animation)
                            {
                                this.BackgroundImage = ca.SImage;
                                m_ActType = ActionModeType.DigitalImage;
                            }

                            break;
                        }
                    }

                    if (ca.CType == CompareType.Bigger)
                    {
                        if (Convert.ToDouble(m_Value) > ca.Value)
                        {
                            this.ForeColor = ca.TColor;
                            this.BackColor = ca.BColor;
                            this.Font = ca.TFont;

                            if (ca.AType == ActionType.DisplayText)
                            {
                                m_DisplayMode = true;
                                this.Text = ca.DisplayText;
                            }
                            else
                                m_DisplayMode = false;

                            if (ca.AType == ActionType.Visible)
                                this.Visible = ca.Visible;

                            if (ca.AType == ActionType.Animation)
                            {
                                this.BackgroundImage = ca.SImage;
                                m_ActType = ActionModeType.DigitalImage;
                            }

                            break;
                        }
                    }

                    if (ca.CType == CompareType.Equal)
                    {
                        if (Convert.ToDouble(m_Value) == ca.Value)
                        {
                            this.m_FillColor = ca.BColor;
                            this.ForeColor = ca.TColor;
                            this.Font = ca.TFont;

                            if (ca.AType == ActionType.DisplayText)
                            {
                                m_DisplayMode = true;
                                this.Text = ca.DisplayText;
                            }
                            else
                                m_DisplayMode = false;

                            if (ca.AType == ActionType.Visible)
                                this.Visible = ca.Visible;

                            if (ca.AType == ActionType.Animation)
                            {
                                this.BackgroundImage = ca.SImage;
                                m_ActType = ActionModeType.DigitalImage;
                            }

                            break;
                        }
                    }
                }
            }
            else
            {
                if (m_ActType == ActionModeType.DigitalText)
                {
                    foreach (CAniItems ca in AItems)
                    {
                        if (ca.CType == CompareType.Lower)
                        {
                            if (Convert.ToDouble(m_Value) < ca.Value)
                            {
                                this.ForeColor = ca.TColor;
                                this.BackColor = ca.BColor;
                                this.Font = ca.TFont;

                                if (ca.AType == ActionType.DisplayText)
                                {
                                    m_DisplayMode = true;
                                    this.Text = ca.DisplayText;
                                }
                                else
                                    m_DisplayMode = false;

                                if (ca.AType == ActionType.Visible)
                                    this.Visible = ca.Visible;

                                if (ca.AType == ActionType.Animation)
                                {
                                    this.BackgroundImage = ca.SImage;
                                    m_ActType = ActionModeType.DigitalImage;
                                }

                                break;
                            }
                        }

                        if (ca.CType == CompareType.OverLower)
                        {
                            if (Convert.ToDouble(m_Value) <= ca.Value)
                            {
                                this.ForeColor = ca.TColor;
                                this.BackColor = ca.BColor;
                                this.Font = ca.TFont;

                                if (ca.AType == ActionType.DisplayText)
                                {
                                    m_DisplayMode = true;
                                    this.Text = ca.DisplayText;
                                }
                                else
                                    m_DisplayMode = false;

                                if (ca.AType == ActionType.Visible)
                                    this.Visible = ca.Visible;

                                if (ca.AType == ActionType.Animation)
                                {
                                    this.BackgroundImage = ca.SImage;
                                    m_ActType = ActionModeType.DigitalImage;
                                }

                                break;
                            }
                        }

                        if (ca.CType == CompareType.Equal)
                        {
                            if (Convert.ToDouble(m_Value) == ca.Value)
                            {
                                this.m_FillColor = ca.BColor;
                                this.ForeColor = ca.TColor;
                                this.Font = ca.TFont;

                                if (ca.AType == ActionType.DisplayText)
                                {
                                    m_DisplayMode = true;
                                    this.Text = ca.DisplayText;
                                }
                                else
                                    m_DisplayMode = false;

                                if (ca.AType == ActionType.Visible)
                                    this.Visible = ca.Visible;

                                if (ca.AType == ActionType.Animation)
                                {
                                    this.BackgroundImage = ca.SImage;
                                    m_ActType = ActionModeType.DigitalImage;
                                }

                                break;
                            }
                        }

                        if (ca.CType == CompareType.OverBigger)
                        {
                            if (Convert.ToDouble(m_Value) >= ca.Value)
                            {
                                this.ForeColor = ca.TColor;
                                this.BackColor = ca.BColor;
                                this.Font = ca.TFont;

                                if (ca.AType == ActionType.DisplayText)
                                {
                                    m_DisplayMode = true;
                                    this.Text = ca.DisplayText;
                                }
                                else
                                    m_DisplayMode = false;

                                if (ca.AType == ActionType.Visible)
                                    this.Visible = ca.Visible;

                                if (ca.AType == ActionType.Animation)
                                {
                                    this.BackgroundImage = ca.SImage;
                                    m_ActType = ActionModeType.DigitalImage;
                                }

                                break;
                            }
                        }

                        if (ca.CType == CompareType.Bigger)
                        {
                            if (Convert.ToDouble(m_Value) > ca.Value)
                            {
                                this.ForeColor = ca.TColor;
                                this.BackColor = ca.BColor;
                                this.Font = ca.TFont;

                                if (ca.AType == ActionType.DisplayText)
                                {
                                    m_DisplayMode = true;
                                    this.Text = ca.DisplayText;
                                }
                                else
                                    m_DisplayMode = false;

                                if (ca.AType == ActionType.Visible)
                                    this.Visible = ca.Visible;

                                if (ca.AType == ActionType.Animation)
                                {
                                    this.BackgroundImage = ca.SImage;
                                    m_ActType = ActionModeType.DigitalImage;
                                }

                                break;
                            }
                        }
                    }
                }
                else if (m_ActType == ActionModeType.DigitalImage)
                {
                    foreach (CAniItems ca in AItems)
                    {
                        if (ca.CType == CompareType.Equal)
                        {
                            if (Convert.ToDouble(m_Value) == ca.Value)
                            {
                                //if (bmpGIF != ca.SImage)
                                {
                                    //this.BackgroundImage = ca.SImage;
                                    bmpGIF = ca.SImage;
                                    m_ContinueMode = ca.Roop;
                                    this.AniMode = false;
                                    //AnimateImage();
                                }

                                break;
                            }
                        }
                    }
                }
                else
                {
                    foreach (CAniItems ca in AItems)
                    {
                        if (ca.CType == CompareType.Equal)
                        {
                            if (Convert.ToDouble(m_Value) == ca.Value)
                            {
                                this.m_FillColor = ca.BColor;
                                this.ForeColor = ca.TColor;
                                this.Font = ca.TFont;

                                if (ca.AType == ActionType.DisplayText)
                                {
                                    m_DisplayMode = true;
                                    this.Text = ca.DisplayText;
                                }
                                else
                                    m_DisplayMode = false;

                                if (ca.AType == ActionType.Visible)
                                    this.Visible = ca.Visible;

                                if (ca.AType == ActionType.Animation)
                                {
                                    this.BackgroundImage = ca.SImage;
                                    m_ActType = ActionModeType.DigitalImage;
                                }

                                break;
                            }
                        }
                    }
                }
            }

            AnimateImage();

            //this.Invalidate();
        }


        private void DrawButton(Graphics g)
        {
            int x1, y1, x2, y2;
            Pen pOldPen = new Pen(Color.Black);

            x1 = 0;
            y1 = this.Height;
            x2 = this.Width;
            y2 = 0;

            pOldPen.Color = Color.White;
            pOldPen.Width = 2;
            g.DrawLine(pOldPen, x1 + 1, y2 + 1, x2 - 1, y2 + 1);
            g.DrawLine(pOldPen, x1 + 1, y2 + 1, x1 + 1, y1 - 2);

            pOldPen.Color = Color.Black;
            g.DrawLine(pOldPen, x1 + 2, y1 - 2, x2 - 1, y1 - 1);
            g.DrawLine(pOldPen, x2 - 1, y2 + 1, x2 - 2, y1 - 2);

            //this.BackColor = Color.Gray;
        }

        public void SetClearTagInfo()
        {
            tag_id = "";
            tag_name = "";
            tag_desc = "";
            tag_unit = "";
            tag_type = "";
            sif_id = "";
            Lo = "";
            Lolo = "";
            Hi = "";
            Hihi = "";
            Kind_Code = "";
            On = "";
            Off = "";
            LimitHi = "";
            LimitLo = "";

            tag_ttype = "";
            tag_objid = "";
            tag_objid2 = "";
            tag_objid3 = "";
            tag_objid4 = "";
            tag_value1 = "";
            tag_value2 = "";
            tag_value3 = "";
            Addr = "";
            tag_initval = "";

            tag_use_cos = "";
            tag_use_trend = "";

            tag_is_single = false;
            tag_single_value = "";

            //AItems.Clear();
        }

        private void BaseControl_Load(object sender, EventArgs e)
        {

        }

        /// <summary>
        ///    Clean up any resources being used.
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (components != null)
                {
                    components.Dispose();
                }
            }
            base.Dispose(disposing);
        }

        //public void StartReadVal()
        //{
        //    DataManager = new Thread(new ThreadStart(ReadVal));
        //    DataManager.Name = "DataManager";
        //    DataManager.IsBackground = true;
        //    DataManager.Start();
        //}

        //private void ReadVal()
        //{   
        //    TagValue tVal = new TagValue();

        //    for (; ; )
        //    {
        //        int i;
        //        double retVal = 0;

        //        //for (i = 0; i < SelItems.Count; i++)
        //        {
        //            if (!string.IsNullOrEmpty(this.tag_id))
        //            {
        //                retVal = tVal.ReadMemByTagId(Convert.ToInt32(this.tag_id));

        //                if (retVal == -88888 || retVal == -99999)
        //                    break;

        //                if (this.Format.Length > 0)
        //                    this.Value = retVal.ToString(this.Format);
        //                else
        //                {
        //                    if (this.tag_type == "01" || this.tag_type == "02")
        //                        this.Value = retVal.ToString("###,##0.0");
        //                    else
        //                        this.Value = retVal.ToString();
        //                }

        //                //// 화씨 -> 섭씨
        //                //if (SelItems[i].tag_value2.Length > 0)
        //                //{
        //                //    tVal.WriteMemByTagId(Convert.ToInt32(SelItems[i].tag_value2), Convert.ToDouble(SelItems[i].Value) - 32 / 1.8);
        //                //} 
        //            }
        //        }

        //        Thread.Sleep(1000);
        //    }
        //}

    }

    public enum ControlType
    {
        Label,
        Box,
        Picture,
        PointText,
        LinkBox,
        PointPicture,
        SinglePointPicture,
        LinkButton,
        LevelGuage,
        LevelGuage_X,
        iexplorer,
        Cam,
        ListCTL,
        TrendChart
    }

    // table 테이블 면 입력
    // Query 쿼리 직접 입력, 프로시져명 입력
    public enum ConnectType
    {
        Table,
        QueryExec
    }

    // Day 날짜 검색
    // Period 기간 검색
    // Word 특정 단어 검색
    public enum SearchType
    {
        Day,
        Period,
        Word
    }

}
