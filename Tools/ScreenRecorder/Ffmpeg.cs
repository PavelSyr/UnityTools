using System;
using System.Diagnostics;
using System.Text;


namespace com.ich.Tools.ScreenRecorder
{
    /**
     * Example ffmpeg -r 30 -f image2 -i frame%d.bmp -vcodec libx264 -crf 21 -pix_fmt yuv420p output.mp4
     * */
    [Serializable]
    class Ffmpeg
    {
        public string ToolPath { get; private set; }
        public string OutputPath { get; private set; }
        public int FrameRate { get; set; } = 30;
        public string InputFileName { get; set; } = "frame%d.bmp";
        public string Codec { get; set; } = "libx264";
        public int crf { get; set; } = 21;
        public string PixFormat { get; set; } = "yuv420p";
        public string output { get; set; } = "output_{0}.mp4";
        public bool CreateNoWindow { get; set; } = false;

        public Ffmpeg(string toolPath, string outputPath)
        {
            OutputPath = outputPath;
            ToolPath = toolPath;
        }

        public void Run()
        {
            var pi = new ProcessStartInfo(ToolPath);
            pi.CreateNoWindow = CreateNoWindow;
            pi.WindowStyle = ProcessWindowStyle.Normal;

            StringBuilder args = new StringBuilder(string.Empty);
            args.Append("-f image2");

            if (FrameRate > 0)
            {
                args.Append(" -r ");
                args.Append(FrameRate);
            }

            if (!string.IsNullOrEmpty(InputFileName))
            {
                args.Append(" -i ");
                args.Append(InputFileName);
            }

            if (!string.IsNullOrEmpty(Codec))
            {
                args.Append(" -vcodec ");
                args.Append(Codec);
            }

            args.Append(" -crf ");
            args.Append(crf);

            if (!string.IsNullOrEmpty(PixFormat))
            {
                args.Append(" -pix_fmt ");
                args.Append(PixFormat);
            }

            if (string.IsNullOrEmpty(output))
            {
                output = "Output_{0}.mp4";
            }

            args.Append(" ");
            args.Append(string.Format(output, DateTime.Now.ToString("yyyyMMdd_HHmmss")));

            args.Append(" -y");

            pi.Arguments = args.ToString();

            UnityEngine.Debug.Log("ffmpeg " + pi.Arguments);

            var process = Process.Start(pi);
            process.WaitForExit();
        }
    }
}
