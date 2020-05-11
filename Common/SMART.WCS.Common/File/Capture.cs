using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace SMART.WCS.Common.File
{
    public class Capture : DisposeClass
    {
        private static Canvas root;
        private static RenderTargetBitmap canvasImage;

        public static void SetRootElement(FrameworkElement element)
        {
            if (element is Canvas)
            {
                root = element as Canvas;
                canvasImage = ConverterBitmapImage(root);
            }
        }

        #region ElementToBitmapImage
        /// <summary>
        /// 현재 객체의 그래픽 요소를 비트맵으로 변환합니다.
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        public static RenderTargetBitmap ConverterBitmapImage(FrameworkElement element)
        {
            DrawingVisual drawingVisual = new DrawingVisual();
            DrawingContext drawingContext = drawingVisual.RenderOpen();

            // 해당 객체의 그래픽요소로 사각형의 그림을 그립니다.
            drawingContext.DrawRectangle(new VisualBrush(element), null, 
                //new Rect(new Point(0, 0), new Point(element.ActualWidth, element.ActualHeight)));
                new Rect(new Point(0, 0), new Point(500, 500)));
            drawingContext.Close();
            
            // 비트맵으로 변환합니다.
            RenderTargetBitmap target =
                //new RenderTargetBitmap((int)element.ActualWidth, (int)element.ActualHeight,
                new RenderTargetBitmap(500, 500,
                96, 96, System.Windows.Media.PixelFormats.Pbgra32);

            target.Render(drawingVisual);
            return target;
        }
        #endregion

        #region ElementToBitmapImage
        /// <summary>
        /// 부모 창에서 객체 부분만큼 비트맵으로 변환합니다.
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="element"></param>
        /// <returns></returns>
        public static BitmapSource ConverterBitmapImage2(FrameworkElement element)
        {
            byte[] bgPixels = CopyPixels((int)Canvas.GetLeft(element), 
                (int)Canvas.GetTop(element),
                (int)element.ActualWidth,
                (int)element.ActualHeight);

            int length = ((int)element.ActualWidth * (int)element.ActualHeight) * 4;
            byte[] elementPixels = new byte[length];

            BitmapSource source = ConverterBitmapImage(element);
            int stride = ((int)element.ActualWidth * canvasImage.Format.BitsPerPixel + 7) / 8;

            // 객체 영역 픽셀로 복사
            source.CopyPixels(elementPixels, stride, 0);

            // 객체 영역만 이미지로 만들기 위해 변경
            PixelTransform(bgPixels, elementPixels, length);

            return BitmapSource.Create((int)element.ActualWidth, (int)element.ActualHeight, 96, 96,
                PixelFormats.Pbgra32, null, bgPixels, stride);
        }
        #endregion

        #region CopyPixels
        public static byte[] CopyPixels(int x, int y, int width, int height)
        {
            byte[] pixels = new byte[width * height * 4];
            int stride = (width * canvasImage.Format.BitsPerPixel + 7) / 8;

            // Canvas 이미지에서 객체 역역만큼 픽셀로 복사
            canvasImage.CopyPixels(new Int32Rect(x, y, width, height), pixels, stride, 0);

            return pixels;
        }
        #endregion

        #region CutAreaToImage
        /// <summary>
        /// 선택 영역만큼 비트맵 이미지로 변환합니다.
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <returns></returns>
        public static BitmapSource CutAreaToImage(int x, int y, int width, int height)
        {
            //if (x < 0)
            //{
            //    width += x;
            //    x = 0;
            //}
            //if (y < 0)
            //{
            //    height += y;
            //    y = 0;

            //    width = (int)root.ActualWidth - x;
            //}
            //if (x + width > root.ActualWidth)
            //{
            //    width = (int)root.ActualWidth - x;
            //}
            //if (y + height > root.ActualHeight)
            //{
            //    height = (int)root.ActualHeight - y;
            //}



            byte[] pixels = CopyPixels(x, y, width, height);

            int stride = (width * canvasImage.Format.BitsPerPixel + 7) / 8;

           return BitmapSource.Create(width, height, 96, 96, PixelFormats.Pbgra32, null, pixels, stride);
        }
        #endregion

        #region Transform
        /// <summary>
        /// 객체 영역만 이미지로 사용할 픽셀로 변경합니다.
        /// </summary>
        /// <param name="pixels"></param>
        /// <param name="mask"></param>
        /// <param name="length"></param>
        private static void PixelTransform(byte[] pixels, byte[] element_pixels, int length)
        {
           for(int i=3; i<length; i+=4) 
           {
               if (element_pixels[i] == 0)
               {
                   pixels[i] = 0;
               }
               else
               {
                   if (pixels[i] > element_pixels[i])
                   {
                       pixels[i] = element_pixels[i];
                       pixels[i - 1] = element_pixels[i - 1];
                       pixels[i - 2] = element_pixels[i - 2];
                       pixels[i - 3] = element_pixels[i - 3];
                   }
               }
           }
        }
        #endregion

        #region Save
        /// <summary>
        /// 저장 포맷에 맞게 비트맵을 인코딩 후 저장합니다.
        /// </summary>
        /// <param name="source"></param>
        public static void Save(BitmapSource source, bool png)
        {
            var strFileName = "Test.jpg";
            FileStream stream = new FileStream(strFileName, FileMode.Create, FileAccess.Write);
            BitmapEncoder encoder = new PngBitmapEncoder();

            strFileName.ToCharArray(strFileName.Length - 3, 3);
                
            string upper = strFileName.ToUpper();
            char[] format = upper.ToCharArray(strFileName.Length - 3, 3);
            upper = new string(format);

            if (!png)
            {
                switch (upper.ToString())
                {
                    case "JPG":
                        encoder = new JpegBitmapEncoder();
                        break;

                    case "GIF":
                        encoder = new GifBitmapEncoder();
                        break;

                    case "BMP":
                        encoder = new BmpBitmapEncoder();
                        break;
                }
            }
                
            encoder.Frames.Add(BitmapFrame.Create(source));

            encoder.Save(stream);
            stream.Close();
        }
        #endregion
    }
}
