using Microsoft.Azure.CognitiveServices.Vision.ComputerVision;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision.Models;
using Raylib_cs;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace AI_Labb_2.Core.cImage
{

    public static class Classify
    {
        static int NumPicture = 0;
        static Random rand = new Random();
        static char filename = 'a';

        public static ClassifiedImage ClassifyImageFakeData(string filepath)
        {

            Texture2D fullscreenimage = Raylib.LoadTexture(filepath);
            string filename = filepath.Split("\\").ToList().Last();

            Texture2D image;
            if (Utils.Utilities.ResizeImage(filepath, filename))
                image = Raylib.LoadTexture("image.png");
            else
                image = Raylib.LoadTexture(filepath);

            Utils.Utilities.ResizeImageToThumbnail(filepath, filename);

            Texture2D thumbnail = Raylib.LoadTexture("thumbnail.png");

            File.Delete("thumbnail.png");

            string imageCaption = "";
            List<ImageTag> imageTags = new List<ImageTag>();
            List<Tuple<DetectedObject, Rectangle>> imageObjects = new List<Tuple<DetectedObject, Rectangle>>();
            
            imageCaption = "Picture" + NumPicture++;

            for (int i = 0; i < 5; i++)
            {
                ImageTag tag = new ImageTag();
                tag.Name = ("Tag: " + i);
                tag.Confidence = rand.NextDouble();
                imageTags.Add(tag);
            }

            for (int i = 0; i < 5; i++)
            {
                DetectedObject detectedobject = new DetectedObject();
                detectedobject.Confidence = rand.NextDouble();
                detectedobject.ObjectProperty = "Object " + i;
                Rectangle rect = new Rectangle();


                rect.x = rand.Next(image.width);
                rect.y = rand.Next(image.height);

                rect.width = Math.Clamp(rand.Next(image.width - (int)rect.x), 0, image.width - (int)rect.x);
                rect.height = Math.Clamp(rand.Next(image.height - (int)rect.y), 0, image.height - (int)rect.y);


                Tuple<DetectedObject, Rectangle> o = new Tuple<DetectedObject, Rectangle>(detectedobject, rect);
                imageObjects.Add(o);
            }

            ClassifiedImage classifiedImage = new ClassifiedImage(image, thumbnail, fullscreenimage, imageCaption, imageTags, imageObjects);

            return classifiedImage;
        }

        public static async Task<ClassifiedImage> ClassifyImageFile(string filepath, Config config)
        {
            ApiKeyServiceClientCredentials creds = new ApiKeyServiceClientCredentials(config.imageKey);
            Texture2D image;
            Texture2D fullscreenimage;
            string filename = filepath.Split("\\").ToList().Last();

            FileStream imageData;

            if (Utils.Utilities.ResizeImage(filepath, filename))
            {
                image = Raylib.LoadTexture("resized" + filename);
                imageData = File.OpenRead("resized" + filename);
                //TODO: Fixa så att den laddar in originalbilden som fullscreenimage.
                fullscreenimage = Raylib.LoadTexture("resized" + filename);
                
            }
            else
            {
                image = Raylib.LoadTexture(filepath);
                imageData = File.OpenRead(filepath);
                fullscreenimage = Raylib.LoadTexture(filepath);
            }

            Utils.Utilities.ResizeImageToThumbnail(filepath, filename);
            Texture2D thumbnail = Raylib.LoadTexture("thumbnail" + filename);

            File.Delete("thumbnail" + filename);

            string imageCaption = "";
            List<ImageTag> imageTags = new List<ImageTag>();
            List<Tuple<DetectedObject, Rectangle>> imageObjects = new List<Tuple<DetectedObject, Rectangle>>();

            var client = new ComputerVisionClient(creds)
            {
                Endpoint = config.imageEndpoint,
            };



            List<VisualFeatureTypes?> features = new List<VisualFeatureTypes?>()
            {
                VisualFeatureTypes.Description,
                VisualFeatureTypes.Tags,
                VisualFeatureTypes.Objects
            };
            
            var analysis = await client.AnalyzeImageInStreamAsync(imageData, features);

            foreach (var caption in analysis.Description.Captions)
            {
                imageCaption = caption.Text;
            }

            if (analysis.Tags.Count > 0)
            {
                foreach (var tag in analysis.Tags)
                {
                    imageTags.Add(tag);
                }
            }

            if (analysis.Objects.Count > 0)
            {
                foreach (var detectedobject in analysis.Objects)
                {
                    Rectangle rect = new Rectangle();
                    rect.x = detectedobject.Rectangle.X;
                    rect.y = detectedobject.Rectangle.Y;
                    rect.width = detectedobject.Rectangle.W;
                    rect.height = detectedobject.Rectangle.H;

                    Tuple<DetectedObject, Rectangle> o = new Tuple<DetectedObject, Rectangle>(detectedobject, rect);
                    imageObjects.Add(o);
                }
            }
            


            ClassifiedImage classifiedImage = new ClassifiedImage(image, thumbnail, fullscreenimage, imageCaption, imageTags, imageObjects);

            return classifiedImage;
        }

        


    }
    
}
