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
    public class Matrix
    {
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
    }
}
