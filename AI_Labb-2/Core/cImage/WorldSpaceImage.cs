using Raylib_cs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace AI_Labb_2.Core.cImage
{
    public class Corner
    {
        public bool Taken;
        public Vector2 Position;

        public Corner(bool taken, Vector2 pos)
        {
            Taken = taken;
            Position = pos;
        }

    }

    public class WorldSpaceImage
    {

        public WorldSpaceImage(ClassifiedImage image, string name)
        {
            this.image = image;
            Name = name;

            GoalColor = new Color(255, 0, 0, 255);
            CurrentColor = new Color(255, 0, 0, 255);
            colorList = Utils.Utilities.RandomColorsInList(image.taggedObjects.Count);
            CalculateFullscreenSize();
            Id++;
        }

        public ClassifiedImage image;
        public int Id;
        public string Name;

        public Vector2 Coordinates;
        public Rectangle Hitbox;

        public Vector2 FullscreenCoordinates;
        public Rectangle FullscreenHitbox;
        public float FullscreenScale;


        public float scale;
        public int TotalWidth;
        public int TotalHeight;

        public int alphaValue;

        public Color GoalColor;
        public Color CurrentColor;

        public List<Corner> Corners = new List<Corner>(3);


        private List<Color> colorList;

        public bool LerpingDone = true;


        public void CalculateCornerPositions()
        {
            Corners.Add(new Corner(false, new Vector2 { X = Coordinates.X, Y = Coordinates.Y }));
            Corners.Add(new Corner(false, new Vector2 { X = Coordinates.X + image.texture.width, Y = Coordinates.Y }));
            Corners.Add(new Corner(false, new Vector2 { X = Coordinates.X, Y = Coordinates.Y + image.texture.height }));
        }

        public void CalculateHitbox()
        {
            Rectangle temp = new Rectangle();
            temp.x = Coordinates.X;
            temp.y = Coordinates.Y;
            temp.width = image.texture.width;
            temp.height = image.texture.height;
            Hitbox = temp;
        }

        public void CalculateFullscreenSize()
        {
            FullscreenScale = Math.Min((float)1080 / (float)image.fullscreentexture.width, (float)(1080 / (float)image.fullscreentexture.height));
            
            Rectangle rect = new Rectangle();
            rect.x = 1920 / 2 - (image.fullscreentexture.width / 2) * FullscreenScale;
            rect.y = (1080 / 2) - ((image.fullscreentexture.height / 2) * FullscreenScale);
            
            rect.height = image.fullscreentexture.height * FullscreenScale;
            rect.width = image.fullscreentexture.width * FullscreenScale;
            
            FullscreenHitbox = rect;
        }



        public void FlipCorner(int num)
        {
            if (num == 0)
                Corners[0].Taken = true;
            if (num == 1)
                Corners[1].Taken = true;
            if (num == 2)
                Corners[2].Taken = true;
            
        }

        public void DrawCornerPositions(Vector2 camera)
        {
            for(int i = 0; i < Corners.Count; i++)
            {
                string text = "Corner [" + Corners[i].Position.X + "," + Corners[i].Position.Y + "]" + Corners[i].Taken;
                int textlenght = Raylib.MeasureText(text, 22);

                if (i == 0)
                    Raylib.DrawText(text, (int)(Corners[i].Position.X + camera.X), (int)(Corners[i].Position.Y + camera.Y), 22, Color.RED);
                else if(i == 1)
                    Raylib.DrawText(text, (int)(Corners[i].Position.X - textlenght + camera.X), (int)(Corners[i].Position.Y + camera.Y), 22, Color.RED);
                else if(i == 2)
                    Raylib.DrawText(text, (int)(Corners[i].Position.X + camera.X), (int)(Corners[i].Position.Y - 22 + camera.Y), 22, Color.RED);
            }
        }

        public void DrawImages(Vector2 camera, float scale)
        {
            Raylib.DrawTextureEx(image.texture, Coordinates * scale + camera, 0, scale, Color.WHITE);
        }


        public void DrawFullscreenImage()
        {

            Raylib.DrawRectangle(0, 0, 1920, 1080, new Color(100, 100, 100, 100));



            if (FullscreenScale > 1)
                Raylib.DrawTextureEx(image.fullscreentexture, new Vector2(FullscreenHitbox.x, FullscreenHitbox.y), 0, FullscreenScale, Color.WHITE);
            else
                Raylib.DrawTextureEx(image.fullscreentexture, new Vector2(FullscreenHitbox.x, FullscreenHitbox.y), 0, FullscreenScale, Color.WHITE);


            int row = (int)FullscreenHitbox.y;

            Raylib.DrawText(image.caption, (int)(FullscreenHitbox.x + FullscreenHitbox.width), row, 30, Color.BLACK);

            row += 50;

            for (int i = 0; i < image.tags.Count; i++)
            {
                string text = image.tags[i].Name + " (" + Math.Round(image.tags[i].Confidence * 100, 2) + "%)";
                Raylib.DrawText(text, (int)(FullscreenHitbox.x + FullscreenHitbox.width), row, 25, Color.BLACK);
                row += 25;
            }

            row += 40;

            for (int i = 0; i < image.taggedObjects.Count; i++)
            {
                Rectangle rect = image.taggedObjects[i].Item2;
                rect.x *= FullscreenScale;
                rect.y *= FullscreenScale;
                rect.x += FullscreenHitbox.x;
                rect.y += FullscreenHitbox.y;
                if(FullscreenScale > 1)
                {
                    rect.width *= FullscreenScale;
                    rect.height *= FullscreenScale;
                }
                else
                {
                    rect.width /= FullscreenScale;
                    rect.height /= FullscreenScale;
                }

                string text = image.taggedObjects[i].Item1.ObjectProperty + " (" + Math.Round(image.taggedObjects[i].Item1.Confidence * 100, 2) + "%)";
                
                Raylib.DrawRectangleLinesEx(rect, 4f, colorList[i]);

                Raylib.DrawText(text,(int)(FullscreenHitbox.x + FullscreenHitbox.width), row, 25, colorList[i]);

                row += 25;
            }

            

        }



    }

}
