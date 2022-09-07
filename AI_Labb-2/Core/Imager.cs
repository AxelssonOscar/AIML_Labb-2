using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using AI_Labb_2.Core.cImage;
using Raylib_cs;

namespace AI_Labb_2.Core
{
    public class Imager
    {
        private const int WINDOW_HEIGHT = 1080;
        private const int WINDOW_WIDTH = 1920;

        private List<string> FilesToBeLoaded;

        private World world;

        

        private bool LoadingImage = false;
        private Config config;


        public Imager()
        {
            config = new Config();
            Raylib.InitWindow(WINDOW_WIDTH, WINDOW_HEIGHT, "Imager");
            Raylib.SetTargetFPS(144);
            //Raylib.DisableCursor();

            world = new World();
            FilesToBeLoaded = new List<string>();
        }
        
        
        public void CloseImager()
        {
            Raylib.CloseWindow();
        }

        public void Run()
        {


        string[] loadedFilePaths;

            while(!Raylib.WindowShouldClose())
            {
                if(Raylib.IsFileDropped())
                {
                    loadedFilePaths = Raylib.GetDroppedFiles();

                    foreach(string file in loadedFilePaths)
                    {
                        LoadImage(file, false, null);
                    }


                    Raylib.ClearDroppedFiles();

                }

                world.Update();
                world.HandleInput();
                world.RenderWorld(LoadingImage);

            }
        }

        public void LoadImage(string filepath, bool fake, string name)
        {
            if(filepath.Contains(".png"))
            {
                LoadImageFromFile(filepath, fake, name);
            }
            else
            {
                Console.WriteLine("Wrong file loaded");
            }
        }


        public async void LoadImageFromFile(string filepath, bool fake, string name)
        {
            ClassifiedImage image;

            if (fake)
                image = Classify.ClassifyImageFakeData(filepath);
            else
            {
                LoadingImage = true;
                image = await Classify.ClassifyImageFile(filepath, config);
                LoadingImage = false;

            }
            WorldSpaceImage wsImage = new WorldSpaceImage(image, name);

            world.AddImageToWorld(wsImage);
        }

       
    }


}
