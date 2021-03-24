using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;

namespace Common
{
    public class GifImage
    {
        private Image gifImage;
        private FrameDimension dimension;
        private int frameCount;
        private int currentFrame = -1;
        private bool reverse;
        private int step = 1;
        private int m_RoopCnt = 1;
        private bool m_ContinueMode = true;

        public bool ContinueMode 
        {
            get { return m_ContinueMode; }
            set { m_ContinueMode = value; }
        }

        public int FrameCount
        {
            get { return frameCount; }
            set { frameCount = value;
                  m_RoopCnt = value; }
        }

        public GifImage(string path)
        {
            gifImage = Image.FromFile(path); //initialize
            dimension = new FrameDimension(gifImage.FrameDimensionsList[0]); //gets the GUID
            frameCount = gifImage.GetFrameCount(dimension); //total frames in the animation

            m_RoopCnt = frameCount;
        }

        public GifImage(Image img)
        {
            gifImage = img; 
            dimension = new FrameDimension(gifImage.FrameDimensionsList[0]); //gets the GUID
            frameCount = gifImage.GetFrameCount(dimension); //total frames in the animation

            m_RoopCnt = frameCount;
        }

        public bool ReverseAtEnd //whether the gif should play backwards when it reaches the end
        {
            get { return reverse; }
            set { reverse = value; }
        }

        public Image GetNextFrame()
        {
            currentFrame += step;          //if the animation reaches a boundary...

            if (currentFrame >= frameCount )//|| currentFrame < 1)
            {
                if (reverse)
                {
                    step *= -1; //...reverse the count
                    currentFrame += step; //apply it
                }
                else
                {
                    if (m_ContinueMode == false)
                    {
                        currentFrame = frameCount - 1; //...or start over

                        //if (m_RoopCnt > 1)
                        //    m_RoopCnt--;
                    }
                    else
                        currentFrame = 0; //...or start over
                }
            }
            
            return GetFrame(currentFrame);
        }

        public Image GetFrame(int index)
        {
            gifImage.SelectActiveFrame(dimension, index); //find the frame
            return (Image)gifImage.Clone(); //return a copy of it
        }

    }
}
