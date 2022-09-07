
using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision.Models;




namespace AI_Labb_2
{

    class Program
    {

        static void Main(string[] args)
        {
            Core.Imager bot = new Core.Imager();

            bot.Run();
            bot.CloseImager();
        }
    }
}
