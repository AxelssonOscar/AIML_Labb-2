using Microsoft.Azure.CognitiveServices.Vision.ComputerVision.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Raylib_cs;
using System.Numerics;

namespace AI_Labb_2.Core.cImage
{
    public class ClassifiedImage
    {
        private const int OBJECT_TEXT_SPACING = 20;


        public ClassifiedImage(Texture2D image, Texture2D thumbnail, Texture2D fullscreentexture, string caption, List<ImageTag> tags, List<Tuple<DetectedObject, Raylib_cs.Rectangle>> objects)
        {
            texture = image;
            this.thumbnail = thumbnail;
            this.fullscreentexture = fullscreentexture;
            this.caption = caption;
            this.tags = tags;
            taggedObjects = objects;
            valid = false;
        }

        public bool valid;

        public Texture2D texture;
        public Texture2D thumbnail;
        public Texture2D fullscreentexture;
        public string caption;
        public List<ImageTag> tags;
        public List<Tuple<DetectedObject, Raylib_cs.Rectangle>> taggedObjects;

        public int ObjectsTextLength;

    }
}
