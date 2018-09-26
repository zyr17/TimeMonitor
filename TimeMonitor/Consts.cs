using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace TimeMonitor
{
    class Consts
    {
        public static TimeSpan ActiveTimeSpan = new TimeSpan(0, 0, 0, 10, 0);
        public static TimeSpan HookTimeSpan = new TimeSpan(0, 0, 5);
        public static TimeSpan CheckForegroundTimeSpan = new TimeSpan(0, 0, 0, 1, 0);
        public const string DateTimeFormatString = "yyyy-MM-dd HH:mm:ss";
        public const string CatchFishString = "摸鱼中...";
        static SolidColorBrush[] Colors = {
            Brushes.MediumSlateBlue,
            Brushes.LightSkyBlue,
            Brushes.LimeGreen,
            Brushes.Chocolate,
            Brushes.Orchid,
            Brushes.PaleGreen,
            Brushes.DodgerBlue,
            Brushes.Silver,
            Brushes.Red,
            Brushes.Plum,
            Brushes.DodgerBlue,
            Brushes.Orange,
            Brushes.DarkGray,
        };
        static int nowcolor = 0;
        public static SolidColorBrush NextBrush
        {
            get
            {
                return Colors[nowcolor++ % Colors.Length];
            }
        }

        public static ImageSource ChangeBitmapToImageSource(System.Drawing.Bitmap bitmap)
        {
            IntPtr hBitmap = bitmap.GetHbitmap();
            ImageSource wpfBitmap = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(
                hBitmap,
                IntPtr.Zero,
                System.Windows.Int32Rect.Empty,
                BitmapSizeOptions.FromEmptyOptions()
            );
            return wpfBitmap;
        }

        public static string Icon2Base64String(System.Drawing.Icon I)
        {
            MemoryStream ms = new MemoryStream();
            if (I != null) I.ToBitmap().Save(ms, ImageFormat.Png);
            byte[] arr = ms.GetBuffer();
            return Convert.ToBase64String(arr);
        }

        public static System.Drawing.Icon Base64String2Icon(string b64)
        {
            var bytes = Convert.FromBase64String(b64);
            var MS = new MemoryStream(bytes);
            var bitmapDecoder = BitmapDecoder.Create(
                MS,
                BitmapCreateOptions.PreservePixelFormat,
                BitmapCacheOption.OnLoad);

            var writable = new WriteableBitmap(bitmapDecoder.Frames.Single());
            writable.Freeze();

            var BS = writable as BitmapSource;
            System.Drawing.Bitmap bitmap;
            using (var outStream = new MemoryStream())
            {
                BitmapEncoder enc = new PngBitmapEncoder();
                enc.Frames.Add(BitmapFrame.Create(BS));
                enc.Save(outStream);
                bitmap = new System.Drawing.Bitmap(outStream);
            }
            Debug.Write(bitmap.PixelFormat);
            return System.Drawing.Icon.FromHandle(bitmap.GetHicon());
        }

        public static string IconClockStr
        {
            get
            {
                return "iVBORw0KGgoAAAANSUhEUgAAABAAAAAQCAYAAAAf8/9hAAAAAXNSR0IArs4c6QAAAARnQU1BAACxjwv8YQUAAAAJcEhZcwAADsMAAA7DAcdvqGQAAAIjSURBVDhPpZHfa1JhHMYPwxvF6sqK/oJBliLIRMLApojzF6g35SzOxZisvPXcDoR24ayLMRpjo9sFu9rlqLHdBM0da5KmJ3U7OqXa0rEb2c3b+xw8L0cquujAx/P6fp/n875wOELIf6H8hEIhLhKJcNFolIvFYlfi8fgEJUrhh2A9gRkyyKLzJ8H4E55PLa+vvymUy82T09MBwBp7mCHzN8H4zOyssCOKLanbJT/Oz8m73V3lDbCH2UwqJSA7IggGg1cfTU/P7RwctD4eHZFv/b5SyuVyTIC9T3S2Vyy2EsnkHDpM4Pf7nS9XVjYL9TqpDU8H2WyWrQFmYqNBltfWNtFhAq/X+/Dt/v4xBFoSicTIf5X3h4fH6DCB2+1+WpCkgRpQTwyHwyM36PZ6yrwsywN0mMDlcj0T63VF8H0YttvtJJPJEJ/PR+gnJDzPk3Q6TV5vbJBquz1AhwmcTufjPVGUtYKvzSZxOBzKW0WiFKpVUqxUZHSYgJ52/8XS0ha+AK6pXhkldQ0wK8kyebW6uoUOE9hstmsPJieFD6VSp312NlLSckJnn2u1jsfjEdBhAqvVypnN5rv+qakFBH5eXPxW7tE9qdHoBAKBBWTRYQI8BoOBM5lMNovFMp9bXNyuVKsdWrrsU75QaT6f36aleWSQxaMV6Cg3KLd1Ot09vV6fNBqNAuX5EAF7mNHMHcpNdLSCMYqRcp1y6x/gIGTHCCHcL/f4Zj19fvgZAAAAAElFTkSuQmCC";
            }
        }
        public static string IconFishStr
        {
            get
            {
                return "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";
            }
        }
        public static string IconXStr
        {
            get
            {
                return "iVBORw0KGgoAAAANSUhEUgAAADAAAAAwCAYAAABXAvmHAAAAAXNSR0IArs4c6QAAAARnQU1BAACxjwv8YQUAAAAJcEhZcwAADsMAAA7DAcdvqGQAAAPrSURBVGhD1Zq7ihRBFIZHzWQTQTE108B4wUvqU4hPIKzgZPoKgvsERhsoiE8g5goba2ziKibeWBBZ/b/ZOuWZ6tPV07O9rnXgg56qc6vuupyemdkYmc/nERtiU2yJHbEr9sS+OEhwTRt96KCLDbYdn8ciQaDL4r54KT6KX+L3iqD7SWCLD3wt+Z9MSsfiqngs3osouXXAFz7xvRTvSFI4Oy8eiCkTL8H3Q0GsHHst8Q7ENfFKREGPA2IRM+cwSpzhKXFbjLnrLNrvgkWLHXBNG32RTQR2xCaH1QdhyuKMuCe+iCiA54d4I7bFHXFTXBGXElzTRh866GIT+fIQmxzIZXgQpiQYNYZDQT6LJ+KWOCe8jxroYoMtPiLfBjmQS/1JWGeCR1e78z/Fc3Fd5LvjKSXSEdjiA1/4jGIBuZBTtu2I62Tx1Ob8B3FXnBXeLnkaltJO4Auf+I5iAjnlhb0k1ijYvmq7zTvBo/c2yct4Kf0IfBMjig3klrfYyAn7fGQIOF5/a6uI9ymIURsEOWZ9b8gp2Dd1eLRLd35q8b4FsfqmEznmE9sbcZRHBiwu5mfWDQJ2KCXS8QQ6xOxb2OR6aJcuKKb67j47RF6wQaDT4mKC69xn4ttEVd99Jiaxo5zIdVEAmjIVYaTIHs02t9Azsc+CBfVIvE1wfUF4HQ99pf7yopTYZ0HsvnOCnBfK1OSUtZESB03nFEyfuXskUNo8FdEgaKOv1MfH4kmYcJ0gNjmUNkDOGyjxYkE9XypwAuaF6yW1MQW4i6UdPBN+EFzTFuniA1/J+6HwOUEOUUXA+8QmCrwdRS8j1Cu5PPCS2moDABJmekBf8jA0AHJ4LUo7ct5CgVe8shMouhZOSkntfVPI8yIR9RmdKWRCW4JcItsdOnlPLTsoe6kcFw5KsXbRN69XZWm9lGLtglyiUnyXTmr1suObuCGGHENtftco10ny/ldcH7mQU+ljj06+Meh0CGr45Kor9DnGDmIweZPUTy7Rjd6nM3o0HBS8iCQ3sdDvYLEOzXdAJ+/9UJOkQy7RQXtAZ/MDaH4KNb+Im99Gmz/Imi8lmi/mmi+noekXGmj+lRKafqmH5r9WAX5ciIzg//xiq3DAgmrrq0UTaxTcgb6pBDxa5mde2MaqUtoJfOGzb9oAOeUZ0BHrSLT39TriFNr7gcPElAR3B8PakzAIMuYnJmqboZsDXwU5dKqBqpiyYNQ8utqaKKHsPbkf+UzMKMHiqe1OU0Oso2/Z3oFg++KcGPM0xoLvaX7oNvGOEpyCHOVTDgRf+MwnrDGZlI4FxRQVIWUttXn0UtQHuv/mzx5eyiCJE/y7zWz2Bz+9UAsvOoVXAAAAAElFTkSuQmCC";
            }
        }
    }
}
