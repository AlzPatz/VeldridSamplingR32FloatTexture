using System;
using System.IO;

namespace EffectDemo
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Demo to demonstrate potential problems sampling a texture of PixelFormat.R32_Float format");

            var demo = new Demo();

            demo.Run();
        }
    }
}