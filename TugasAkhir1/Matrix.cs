using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;


namespace TugasAkhir1
{
    /**
     * This class was used to do some matrix manipulation
     * 1. ConvertToVectorMatrix Class
     * 2. ConvertToBinaryVectorMatrix Class
     * 3. 1/3 Convolution Code
     * 
     * */
    public class Matrix
    {
        //Convert to 1 Dimensional Matrix fill with grayscale value
        public List<int> ConvertToVectorMatrix(Bitmap bmp)
        {
            List<int> m = new List<int>();
            int width = bmp.Width;
            int height = bmp.Height;
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    Color c = bmp.GetPixel(x, y);
                    int p = (c.R+c.G+c.B)/3;
                    m.Add(p);
                }
            }

            return m;
        }

        //Convert to 0 or 1 bit sequence , 1 denote to white space and 0 denote to black space
        public List<int> ConvertToBinaryVectorMatrix(List<int> vm)
        {
            List<int> bvm = new List<int>();
            foreach (int i in vm)
            {
                if (i >= 250)
                {
                    int b = 1;
                    bvm.Add(b);
                }
                else if (i <= 0)
                {
                    int b = 0;
                    bvm.Add(b);
                }
            }

            return bvm;    
        }


    }
}
