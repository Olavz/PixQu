using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PixQuView
{
    class Frame
    {
        private int duration = 1000;
        private string data = "";

        private string[,] dataMatrix = new string[40,60];

        public Frame(int duration, string data)
        {
            this.duration = duration;
            this.data = data;

            string[] lines = data.Split(new string[] { Environment.NewLine }, StringSplitOptions.None);
            for(int y=0; y<lines.Length; y++) {
                string line = lines[y];
                for(int x=0; x<line.Length; x++)
                {
                    char xy = line[x];
                    dataMatrix[y, x] = xy.ToString();
                }
            }

        }

        public string[,] getMatrix()
        {
            return dataMatrix;
        }

        public int getDuration()
        {
            return duration;
        }



    }
}
