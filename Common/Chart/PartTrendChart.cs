using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using System.Runtime.Serialization;

namespace Common
{
    [Serializable()]
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class PartTrendChart
    {
        // public 속성

        public string ChartTitle
        {
            get { return m_ChartTitle; }
            set { m_ChartTitle = value; }
        }

        public Color ChartTitleColor
        {
            get { return m_ChartTitleColor; }
            set { m_ChartTitleColor = value; }
        }

        public Font ChartTitleFont
        {
            get { return m_ChartTitleFont; }
            set { m_ChartTitleFont = value; }
        }

        public Color ChartAreaColor
        {
            get { return m_ChartAreaColor; }
            set { m_ChartAreaColor = value; }
        }

        public Padding ChartAreaMargin
        {
            get { return m_ChartAreaMargin; }
            set { m_ChartAreaMargin = value; }
        }

        public Padding ChartAreaPadding
        {
            get { return m_ChartAreaPadding; }
            set { m_ChartAreaPadding = value; }
        }

        public Color ValueLineColor
        {
            get { return m_ValueLineColor; }
            set { m_ValueLineColor = value; }
        }

        public int ValueLineWidth
        {
            get { return m_ValueLineWidth; }
            set { m_ValueLineWidth = value; }
        }

        public Color LegendTextColor
        {
            get { return m_LegendTextColor; }
            set { m_LegendTextColor = value; }
        }

        public Font LegendTextFont
        {
            get { return m_LegendTextFont; }
            set { m_LegendTextFont = value; }
        }

        public Color GridLineColor
        {
            get { return m_GridLineColor; }
            set { m_GridLineColor = value; }
        }

        public int GridLineWidth
        {
            get { return m_GridLineWidth; }
            set { m_GridLineWidth = value; }
        }

        public Color GridTextColor
        {
            get { return m_GridTextColor; }
            set { m_GridTextColor = value; }
        }

        public Font GridTextFont
        {
            get { return m_GridTextFont; }
            set { m_GridTextFont = value; }
        }

        public Color TimeAxisColor
        {
            get { return m_TimeAxisColor; }
            set { m_TimeAxisColor = value; }
        }

        public int TimeAxisWidth
        {
            get { return m_TimeAxisWidth; }
            set { m_TimeAxisWidth = value; }
        }

        public Color TimeTextColor
        {
            get { return m_TimeTextColor; }
            set { m_TimeTextColor = value; }
        }

        public Font TimeTextFont
        {
            get { return m_TimeTextFont; }
            set { m_TimeTextFont = value; }
        }

        public TimeSpan TimeRange
        {
            get { return m_TimeRange; }
            set { m_TimeRange = value; }
        }

        public bool IsRealTimeChart
        {
            get { return m_IsRealTimeChart; }
            set { m_IsRealTimeChart = value; }
        }

        // public 메서드

        public object GetSynsObject()
        {
            return this.SyncReadDataWait;
        }

        public bool CheckNeedData()
        {
            lock (this.SyncData)
            {
                DateTime nowTime = DateTime.Now;
                TimeSpan timeDiff = nowTime - this.m_LastTime;

                // 시간 변경 등의 상황으로 현재 시각이 LastTime 보다 이전 시각이 된 경우, 오랜시간 값을 읽지 못하는 문제 방지

                if (this.m_IsRealTimeChart == true)
                {
                    if (timeDiff.TotalSeconds >= 1 | timeDiff.TotalSeconds < 0)
                    {
                        this.m_LastCheckNeedTime = nowTime;
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    // 이전 읽기 시도 후 일정시간 읽기 방지
                    TimeSpan checkTimeDiff = nowTime - this.m_LastCheckNeedTime;

                    if (checkTimeDiff.TotalMinutes >= 1 | checkTimeDiff.TotalMinutes < 0)
                    {
                        if (timeDiff.TotalMinutes >= 15 | timeDiff.TotalMinutes < 0)
                        {
                            this.m_LastCheckNeedTime = nowTime;
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }
                    else
                    {
                        return false;
                    }
                }
            }
        }

        public void SetRealTimeData(double value)
        {
            DateTime lastTime = DateTime.Now;
            DateTime firstTime = lastTime.Add(-this.m_TimeRange);

            lock (this.SyncData)
            {
                ListData.Add(new ClsTimeAndValue(lastTime, value));
                ListData.Sort(ClsTimeAndValue.CompareTime);

                // 기간 초과 삭제

                m_LastTime = lastTime;

                while (ListData.Count > 0)
                {
                    if (ListData[0].Time < firstTime)
                    {
                        this.ListData.RemoveAt(0);
                    }
                    else
                    {
                        break;
                    }
                }
            }
        }

        public void SetHistoryTrendData(Dictionary<DateTime, double> data)
        {
            List<DateTime> listTime = new List<DateTime>();

            foreach (DateTime nowTime in data.Keys)
            {
                listTime.Add(nowTime);
            }

            listTime.Sort();

            if (listTime.Count > 0)
            {
                DateTime lastTime = listTime[listTime.Count - 1];
                DateTime firstTime = lastTime.Add(-this.m_TimeRange);

                List<ClsTimeAndValue> listData = new List<ClsTimeAndValue>();
                foreach (DateTime nowTime in listTime)
                {
                    if (nowTime >= firstTime)
                    {
                        listData.Add(new ClsTimeAndValue(nowTime, data[nowTime]));
                    }
                }

                lock (this.SyncData)
                {
                    this.m_LastTime = lastTime;
                    this.ListData.Clear();
                    this.ListData.AddRange(listData);
                }
            }
            else
            {
                lock (this.SyncData)
                {
                    this.m_LastTime = DateTime.MinValue;
                    this.ListData.Clear();
                }
            }
        }

        public void DrawChart(Graphics g, Rectangle clientRectangle, string legendText)
        {
            int valueAreaTopPadding = 3;
            int valueAreaBottomPadding = 5;
            int gridTextSpace = 2;
            int timeGridTextSpace = 4;
            int legendTextSpace = 4;

            Font nowGridTextFont = this.m_GridTextFont ?? new Font("굴림", 9f, FontStyle.Regular);
            Font nowTimeTextFont = this.m_TimeTextFont ?? new Font("굴림", 9f, FontStyle.Regular);
            Font nowLegentTextFont = this.m_LegendTextFont ?? new Font("굴림", 9f, FontStyle.Regular);
            Font nowChartTitleFont = this.m_ChartTitleFont ?? new Font("굴림", 9f, FontStyle.Regular);

            DateTime lastTime;
            ClsTimeAndValue[] data;
            lock (this.SyncData)
            {
                lastTime = m_LastTime;
                data = ListData.ToArray();
            }

            DateTime firstTime;
            if (lastTime > DateTime.MinValue.Add(this.m_TimeRange))
            {
                firstTime = lastTime.Add(-this.m_TimeRange);
            }
            else
            {
                firstTime = DateTime.MinValue;
            }

            double minValue = double.MaxValue;
            double maxValue = double.MinValue;
            #region 최대, 최소값 검색
            foreach (ClsTimeAndValue nowTimeValue in data)
            {
                if (nowTimeValue.Value > maxValue)
                {
                    maxValue = nowTimeValue.Value;
                }

                if (nowTimeValue.Value < minValue)
                {
                    minValue = nowTimeValue.Value;
                }
            }
            #endregion

            if (data.Length == 0)
            {
                minValue = 0;
                maxValue = 10;
            }

            bool hasZero = true;
            #region 0 축 포함 여부 확인
            if ((maxValue >= 0 & minValue <= 0) | maxValue == minValue)
            {
                // 최대 >= 0 >= 최소 or 최대 == 최소
                hasZero = true;
            }
            else if ((maxValue - minValue) <= Math.Max(Math.Abs(maxValue), Math.Abs(minValue)) / 10)
            {
                // 값 범위가 좁은 경우
                hasZero = false;
            }
            #endregion

            double valueRange = 0;
            #region 값 간격 확인 min-max or 0-max or 0-min
            if (hasZero == true)
            {
                valueRange = Math.Max(Math.Abs(maxValue), Math.Abs(minValue));
            }
            else
            {
                valueRange = Math.Abs(maxValue - minValue);
            }
            #endregion

            double gridSize = 1;
            if (valueRange >= 1)
            {
                #region 값 간격에 따라, Grid 간격 결정(1 이상)

                int digitCount = Convert.ToInt32(Math.Ceiling(valueRange)).ToString().Length;

                foreach (double nowSize in new double[] { 5.0, 2.5, 2, 1, 0.5, 0.25, 0.2, 0.1 })
                {
                    if (valueRange >= nowSize * Math.Pow(10, digitCount - 1) * 2)
                    {
                        gridSize = nowSize * Math.Pow(10, digitCount - 1);
                        break;
                    }
                }
                #endregion
            }
            else
            {
                #region 값 간격에 따라, Grid 간격 결정(1 미만)

                double[] arraySize = { 0.5, 0.25, 0.2, 0.1, 0.05, 0.025, 0.02, 0.01, 0.005, 0.002, 0.001, };
                foreach (double nowSize in arraySize)
                {
                    if (valueRange >= nowSize * 2)
                    {
                        gridSize = nowSize;
                        break;
                    }
                }
                #endregion
            }

            double maxCeilingValue;
            double minFloorValue;
            #region ValueLine 영역 위쪽 값, 아래쪽 값 결정

            if (hasZero == true & maxValue == 0 & minValue == 0)
            {
                maxCeilingValue = gridSize;
                minFloorValue = 0;
            }
            else
            {
                maxCeilingValue = Math.Ceiling(maxValue * 1000) / 1000;
                minFloorValue = Math.Floor(minValue * 1000) / 1000;

                if (hasZero == true & minValue >= 0)
                {
                    minFloorValue = 0;
                }
                else if (hasZero == true & maxValue <= 0)
                {
                    maxCeilingValue = 0;
                }
            }
            #endregion

            List<double> listGridValue = new List<double>();
            #region Grid 표시 값 확인
            {
                double gridLowValue = Math.Floor(minFloorValue / gridSize) * gridSize;
                double gridHighValue = Math.Ceiling(maxCeilingValue / gridSize) * gridSize;
                for (double nowGridValue = gridLowValue; nowGridValue <= gridHighValue; nowGridValue += gridSize)
                {
                    if (nowGridValue >= minFloorValue & nowGridValue <= maxCeilingValue)
                    {
                        listGridValue.Add(nowGridValue);
                    }
                }
            }
            #endregion

            TimeSpan timeSize = TimeSpan.FromSeconds(1);
            #region TimeGrid 간격 결정
            {
                TimeSpan[] arrayTimeSize = { 
                            TimeSpan.FromDays(1),
                            TimeSpan.FromHours(8), TimeSpan.FromHours(4), TimeSpan.FromHours(2), TimeSpan.FromHours(1),
                            TimeSpan.FromMinutes(30), TimeSpan.FromMinutes(15), TimeSpan.FromMinutes(5), TimeSpan.FromMinutes(3), TimeSpan.FromMinutes(1), 
                            TimeSpan.FromSeconds(30), TimeSpan.FromSeconds(20), TimeSpan.FromSeconds(10), TimeSpan.FromSeconds(1),
                        };

                foreach (TimeSpan nowTimeSize in arrayTimeSize)
                {
                    if (this.m_TimeRange.TotalSeconds >= nowTimeSize.TotalSeconds * 3)
                    {
                        timeSize = nowTimeSize;
                        break;
                    }
                }
            }
            #endregion

            List<DateTime> listTimeGridValue = new List<DateTime>();
            #region TimeGrid 표시 값 확인
            {
                DateTime timeGridHighValue = lastTime.Date.AddSeconds(Math.Floor(lastTime.TimeOfDay.TotalSeconds / timeSize.TotalSeconds) * timeSize.TotalSeconds);
                DateTime timeGridLowValue = firstTime.Date.AddSeconds(Math.Ceiling(firstTime.TimeOfDay.TotalSeconds / timeSize.TotalSeconds) * timeSize.TotalSeconds);

                for (DateTime nowTimeGridValue = timeGridLowValue; nowTimeGridValue <= timeGridHighValue; nowTimeGridValue = nowTimeGridValue.Add(timeSize))
                {
                    if (nowTimeGridValue >= firstTime & nowTimeGridValue <= lastTime)
                    {
                        listTimeGridValue.Add(nowTimeGridValue);
                    }
                }
            }
            #endregion

            List<string> listTimeGridText = new List<string>();
            #region TimeGrid 표시 텍스트 확인
            foreach (DateTime nowTimeGridValue in listTimeGridValue)
            {
                string nowTimeText = nowTimeGridValue.ToString("m");

                TimeSpan timeOfDay = nowTimeGridValue.TimeOfDay;
                if (timeOfDay.TotalSeconds == 0)
                {

                }
                else if (timeOfDay.Seconds == 0)
                {
                    nowTimeText += "\r\n";
                    nowTimeText += nowTimeGridValue.ToShortTimeString();
                }
                else
                {
                    nowTimeText += "\r\n";
                    nowTimeText += nowTimeGridValue.ToLongTimeString();
                }
                listTimeGridText.Add(nowTimeText);
            }
            #endregion

            // TODO : 영역 크기와 패딩 크기 비교하여 그리기 가능한지 확인

            // 패딩 포함 Chart 영역
            Rectangle chartArea = new Rectangle();
            chartArea.X = this.m_ChartAreaMargin.Left;
            chartArea.Y = this.m_ChartAreaMargin.Top;

            chartArea.Width = clientRectangle.Width - this.m_ChartAreaMargin.Left - this.m_ChartAreaMargin.Right;
            chartArea.Height = clientRectangle.Height - this.m_ChartAreaMargin.Top - this.m_ChartAreaMargin.Bottom;

            if (chartArea.Width > 0 & chartArea.Height > 0)
            {
                using (Bitmap bufferImage = new Bitmap(chartArea.Width, chartArea.Height))
                {
                    using (Graphics nowGraphics = Graphics.FromImage(bufferImage))
                    {
                        int valueTextWidth = 0;
                        #region Grid 표시 텍스트 최대 chartWidth 확인
                        foreach (double nowGridValue in listGridValue)
                        {
                            string nowText = nowGridValue.ToString();
                            SizeF size = nowGraphics.MeasureString(nowText, nowGridTextFont);

                            int nowWidth = Convert.ToInt32(Math.Ceiling(size.Width));
                            if (nowWidth > valueTextWidth)
                            {
                                valueTextWidth = nowWidth;
                            }
                        }
                        #endregion

                        int timeTextHeight = 0;
                        #region TimeGrid 표시 텍스트 최대 chartHeight 확인
                        foreach (string nowTimeText in listTimeGridText)
                        {
                            SizeF size = nowGraphics.MeasureString(nowTimeText, nowTimeTextFont);

                            int nowHeight = Convert.ToInt32(Math.Ceiling(size.Height));
                            if (nowHeight > timeTextHeight)
                            {
                                timeTextHeight = nowHeight;
                            }
                        }
                        #endregion

                        if (this.m_ChartAreaColor != Color.Transparent)
                        {
                            // ChartArea 배경색 칠하기
                            nowGraphics.Clear(this.m_ChartAreaColor);
                        }

                        Size legendTextSize;
                        {
                            SizeF nowSize = nowGraphics.MeasureString(legendText, nowLegentTextFont);
                            legendTextSize = new Size(Convert.ToInt32(Math.Ceiling(nowSize.Width)), Convert.ToInt32(Math.Ceiling(nowSize.Height)));
                        }

                        // 패딩 제외 Chart 영역
                        Rectangle chartInnerArea = new Rectangle();
                        chartInnerArea.X = this.m_ChartAreaPadding.Left;
                        chartInnerArea.Y = this.m_ChartAreaPadding.Top;
                        chartInnerArea.Width = chartArea.Width - this.m_ChartAreaPadding.Left - this.m_ChartAreaPadding.Right;
                        chartInnerArea.Height = chartArea.Height - this.m_ChartAreaPadding.Top - this.m_ChartAreaPadding.Bottom;

                        // LegendText 영역
                        Rectangle legendTextArea = new Rectangle();
                        legendTextArea.X = chartInnerArea.Right - legendTextSize.Width - legendTextSize.Height;
                        legendTextArea.Y = chartInnerArea.Top;
                        legendTextArea.Width = legendTextSize.Width;
                        legendTextArea.Height = legendTextSize.Height;

                        // 값 Path 영역
                        Rectangle valueArea = new Rectangle();
                        valueArea.X = chartInnerArea.X + valueTextWidth + gridTextSpace;
                        valueArea.Y = legendTextArea.Bottom + legendTextSpace;
                        valueArea.Width = chartInnerArea.Width - valueTextWidth - gridTextSpace;
                        valueArea.Height = chartInnerArea.Height - legendTextArea.Height - legendTextSpace - timeTextHeight - timeGridTextSpace;

                        // 값 Path 패딩 제외 영역
                        Rectangle valueInnerArea = new Rectangle();
                        valueInnerArea.X = valueArea.X;
                        valueInnerArea.Y = valueArea.Y + valueAreaTopPadding;
                        valueInnerArea.Width = valueArea.Width;
                        valueInnerArea.Height = valueArea.Height - valueAreaTopPadding - valueAreaBottomPadding;

                        // Legend 그리기
                        using (SolidBrush legendTextBrush = new SolidBrush(this.m_LegendTextColor))
                        {
                            nowGraphics.DrawString(legendText, nowLegentTextFont, legendTextBrush, legendTextArea);
                        }

                        using (Pen legendLinePen = new Pen(this.m_ValueLineColor, this.m_ValueLineWidth))
                        {
                            int legendLineX = legendTextArea.X - legendTextArea.Height - legendTextSpace;
                            int legendLineY = legendTextArea.Y + (legendTextArea.Height / 2) - this.m_ValueLineWidth * 2;
                            nowGraphics.DrawLine(legendLinePen, legendLineX, legendLineY, legendLineX + legendTextArea.Height, legendLineY);
                        }

                        // TimeAxis 그리기
                        using (Pen timeAsixPen = new Pen(this.m_TimeAxisColor, this.m_TimeAxisWidth))
                        {
                            nowGraphics.DrawLine(timeAsixPen, valueArea.Left, valueArea.Bottom, valueArea.Right, valueArea.Bottom);
                        }

                        using (Pen gridLinePen = new Pen(this.m_GridLineColor, this.m_GridLineWidth))
                        {
                            // Grid 수평 그리기
                            using (SolidBrush gridTextBrush = new SolidBrush(this.m_GridTextColor))
                            using (StringFormat gridTextFormat = new StringFormat())
                            {
                                gridTextFormat.Alignment = StringAlignment.Far;
                                gridTextFormat.LineAlignment = StringAlignment.Center;

                                foreach (double nowGridValue in listGridValue)
                                {
                                    int valueY = Convert.ToInt32(valueInnerArea.Top + (valueInnerArea.Height * (nowGridValue - maxCeilingValue)) / (minFloorValue - maxCeilingValue));

                                    nowGraphics.DrawLine(gridLinePen, valueArea.Left, valueY, valueArea.Right, valueY);
                                    nowGraphics.DrawString(nowGridValue.ToString(), nowGridTextFont, gridTextBrush, valueArea.X - gridTextSpace, valueY + this.m_GridLineWidth, gridTextFormat);
                                }
                            }

                            using (SolidBrush timeTextBrush = new SolidBrush(this.m_TimeTextColor))
                            using (StringFormat timeGridTextFormat = new StringFormat())
                            {
                                timeGridTextFormat.Alignment = StringAlignment.Center;
                                timeGridTextFormat.LineAlignment = StringAlignment.Near;

                                for (int idxTimeGrid = 0; idxTimeGrid < listTimeGridValue.Count; idxTimeGrid++)
                                {
                                    #region TimeGrid 그리기

                                    DateTime nowTimeGridValue = listTimeGridValue[idxTimeGrid];
                                    string nowTimeGridText = listTimeGridText[idxTimeGrid];

                                    double nowTimeGridDistance = (nowTimeGridValue - firstTime).TotalSeconds;
                                    int valueX = Convert.ToInt32(valueInnerArea.Left + ((valueInnerArea.Width * nowTimeGridDistance) / m_TimeRange.TotalSeconds));

                                    #endregion
                                    nowGraphics.DrawLine(gridLinePen, valueX, valueArea.Top, valueX, valueArea.Bottom - 1);
                                    nowGraphics.DrawString(nowTimeGridText, nowGridTextFont, timeTextBrush, valueX, valueArea.Bottom + timeGridTextSpace, timeGridTextFormat);
                                }
                            }

                            if (data.Length > 1)
                            {
                                List<Point> listPoint = new List<Point>();
                                foreach (ClsTimeAndValue nowTimeAndValue in data)
                                {
                                    #region ValueLine Path 계산

                                    double nowGridValue = nowTimeAndValue.Value;
                                    DateTime nowTimeGridValue = nowTimeAndValue.Time;

                                    int valueY = Convert.ToInt32(valueInnerArea.Top + (valueInnerArea.Height * (nowGridValue - maxCeilingValue)) / (minFloorValue - maxCeilingValue));

                                    double nowTimeGridDistance = (nowTimeGridValue - firstTime).TotalSeconds;
                                    int valueX = Convert.ToInt32(valueInnerArea.Left + ((valueInnerArea.Width * nowTimeGridDistance) / m_TimeRange.TotalSeconds));

                                    #endregion
                                    listPoint.Add(new Point(valueX, valueY));
                                }

                                using (Pen valueLinePen = new Pen(this.m_ValueLineColor, this.m_ValueLineWidth))
                                {
                                    nowGraphics.DrawLines(valueLinePen, listPoint.ToArray());
                                }
                            }
                        }
                    }

                    g.DrawImage(bufferImage, chartArea);

                    if (String.IsNullOrEmpty(this.m_ChartTitle) != true)
                    {
                        using (SolidBrush chartTitleBrush = new SolidBrush(this.m_ChartTitleColor))
                        using (StringFormat chartTitleFormat = new StringFormat())
                        {
                            chartTitleFormat.Alignment = StringAlignment.Center;
                            chartTitleFormat.LineAlignment = StringAlignment.Center;

                            Rectangle rect = new Rectangle(clientRectangle.X, clientRectangle.Y, clientRectangle.Width, this.m_ChartAreaMargin.Top);
                            g.DrawString(this.m_ChartTitle, nowChartTitleFont, chartTitleBrush, rect, chartTitleFormat);
                        }
                    }
                }
            }
        }

        // public static 메서드

        public static void CopyProperty(PartTrendChart sourceControl, PartTrendChart targetControl)
        {
            if (sourceControl == null)
            {
                sourceControl = new PartTrendChart();
            }

            targetControl.m_ChartTitle = sourceControl.m_ChartTitle;
            targetControl.m_ChartTitleColor = sourceControl.m_ChartTitleColor;
            targetControl.m_ChartTitleFont = sourceControl.m_ChartTitleFont;
            targetControl.m_ChartAreaColor = sourceControl.m_ChartAreaColor;
            targetControl.m_ValueLineColor = sourceControl.m_ValueLineColor;
            targetControl.m_ValueLineWidth = sourceControl.m_ValueLineWidth;
            targetControl.m_LegendTextColor = sourceControl.m_LegendTextColor;
            targetControl.m_LegendTextFont = sourceControl.m_LegendTextFont;
            targetControl.m_GridLineColor = sourceControl.m_GridLineColor;
            targetControl.m_GridLineWidth = sourceControl.m_GridLineWidth;
            targetControl.m_GridTextColor = sourceControl.m_GridTextColor;
            targetControl.m_GridTextFont = sourceControl.m_GridTextFont;
            targetControl.m_TimeAxisColor = sourceControl.m_TimeAxisColor;
            targetControl.m_TimeAxisWidth = sourceControl.m_TimeAxisWidth;
            targetControl.m_TimeTextColor = sourceControl.m_TimeTextColor;
            targetControl.m_TimeTextFont = sourceControl.m_TimeTextFont;
            targetControl.m_ChartAreaMargin = sourceControl.m_ChartAreaMargin;
            targetControl.m_ChartAreaPadding = sourceControl.m_ChartAreaPadding;
            targetControl.m_TimeRange = sourceControl.m_TimeRange;
            targetControl.m_IsRealTimeChart = sourceControl.m_IsRealTimeChart;
        }

        // 생성자

        public PartTrendChart()
        {

        }

        // private 속성용 멤버변수

        private string m_ChartTitle;

        private Color m_ChartTitleColor = Color.White;

        private Font m_ChartTitleFont = new Font("굴림", 9f, FontStyle.Regular);

        private Color m_ChartAreaColor = Color.Black;

        private Padding m_ChartAreaMargin = new Padding(10, 30, 10, 30);

        private Padding m_ChartAreaPadding = new Padding(5);

        private Color m_ValueLineColor = Color.Yellow;

        private int m_ValueLineWidth = 1;

        private Color m_LegendTextColor = Color.Silver;

        private Font m_LegendTextFont = new Font("굴림", 9f, FontStyle.Regular);

        private Color m_GridLineColor = Color.Gray;

        private int m_GridLineWidth = 1;

        private Color m_GridTextColor = Color.Silver;

        private Font m_GridTextFont = new Font("굴림", 9f, FontStyle.Regular);

        private Color m_TimeAxisColor = Color.Silver;

        private int m_TimeAxisWidth = 1;

        private Color m_TimeTextColor = Color.Silver;

        private Font m_TimeTextFont = new Font("굴림", 9f, FontStyle.Regular);

        private TimeSpan m_TimeRange = new TimeSpan(24, 0, 0);

        private bool m_IsRealTimeChart = false;

        // private 속성 멤버변수

        private object SyncReadDataWait
        {
            get
            {
                lock (this)
                {
                    if (m_SyncReadDataWait == null)
                    {
                        m_SyncReadDataWait = new object();
                    }
                }

                return m_SyncReadDataWait;
            }
        }

        private object SyncData
        {
            get
            {
                lock (this)
                {
                    if (m_SyncData == null)
                    {
                        m_SyncData = new object();
                    }
                }

                return m_SyncData;
            }
        }

        private List<ClsTimeAndValue> ListData
        {
            get
            {
                lock (this)
                {
                    if (m_ListData == null)
                    {
                        m_ListData = new List<ClsTimeAndValue>();
                    }
                }

                return m_ListData;
            }
        }

        // private 멤버변수

        [NonSerialized]
        private object m_SyncReadDataWait;

        [NonSerialized]
        private object m_SyncData;

        [NonSerialized]
        private DateTime m_LastTime = DateTime.MinValue;

        [NonSerialized]
        private DateTime m_LastCheckNeedTime = DateTime.MinValue;

        [NonSerialized]
        private List<ClsTimeAndValue> m_ListData;

        // 내부 class

        private class ClsTimeAndValue
        {
            // public 속성

            public DateTime Time
            {
                get { return this.m_Time; }
            }

            public double Value
            {
                get { return this.m_Value; }
            }

            // public static 메서드

            public static int CompareTime(ClsTimeAndValue objA, ClsTimeAndValue objB)
            {
                return DateTime.Compare(objA.Time, objB.Time);
            }

            // 생성자

            public ClsTimeAndValue(DateTime time, double value)
            {
                this.m_Time = time;
                this.m_Value = value;
            }

            // private 멤버변수

            private DateTime m_Time;

            private double m_Value;
        }
    }
}
