using Raylib_cs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

using AI_Labb_2.Core.cImage;

namespace AI_Labb_2.Core
{

    public enum CursorStates
    {
        STOPPED,
        ACTIVE,
        HOLDING
    }


    public class Cursor
    {
        private const float GOAL_HELD_MAX_SIZE = 1f;

        private const float GOAL_MAX_SIZE = 0.6f;
        private const float GOAL_MIN_SIZE = 0.15f;

        public List<CursorImage> CursorImages = new List<CursorImage>();
        public CursorImage HeldImage = null;

        const int CircleRadius = 350;
        private float firstimageangle = 0;

        public bool stopped = false;

        private Vector2 CurrentMousePos;
        private Vector2 StoppedMousePos;


        public CursorStates state;

        private bool ImageSpinLerping = true;
        private bool ImageSizeLerping = true;
        private bool ImageCenterLerping = true;

        private bool ImageHeldSizeLerping = false;

        private float CurrentCursorImageSize = GOAL_MIN_SIZE;
        private float GoalCursorImageSize = GOAL_MAX_SIZE;
        private float GoalHeldImageSize;

        private float HeldImageSize;

        private Vector2 MiddleOfScreen = new Vector2(1920 / 2, 1080 / 2);
        private float circle = 0;

        public Cursor()
        {
            state = CursorStates.ACTIVE;
        }

        public void UpdateCursor(Vector2 mousePos)
        {
            CurrentMousePos = mousePos;
        }

        public void UpdateHeldImagePosition(Vector2 mousePos)
        {
            HeldImage.coords = mousePos;
        }

        public void StateCursor(Vector2 camera)
        {
            if(state == CursorStates.STOPPED)
            {
                state = CursorStates.ACTIVE;
                ImageSpinLerping = false;
                ImageSizeLerping = false;
                ImageCenterLerping = false;
                ImageHeldSizeLerping = false;
                GoalCursorImageSize = GOAL_MIN_SIZE;
                MiddleOfScreen.X = 1920 / 2;
                MiddleOfScreen.Y = 1080 / 2;
                firstimageangle = 0;
            }
            else if(state == CursorStates.ACTIVE && CursorImages.Count > 0)
            {
                state = CursorStates.STOPPED;
                ImageSpinLerping = false;
                ImageSizeLerping = false;
                ImageCenterLerping = false;
                ImageHeldSizeLerping = false;
                GoalCursorImageSize = GOAL_MAX_SIZE;
                StoppedMousePos = CurrentMousePos - camera;
                MiddleOfScreen -= camera;
                if (firstimageangle > 360 / 2)
                    circle = 360;
                else
                    circle = 0;
            }
            else if(state == CursorStates.HOLDING)
            {
                state = CursorStates.ACTIVE;
                ImageSpinLerping = false;
                ImageSizeLerping = false;
                ImageCenterLerping = false;
                ImageHeldSizeLerping = false;
                GoalCursorImageSize = GOAL_MIN_SIZE;
                MiddleOfScreen.X = 1920 / 2;
                MiddleOfScreen.Y = 1080 / 2;
                firstimageangle = 0;
            }
        }


        public void AddImagesToCursor(Texture2D image, int id, float scale)
        {
            CursorImage cImage = new CursorImage();
            cImage.texture = image;
            cImage.Id = id;
            cImage.scale = scale;
            CursorImages.Add(cImage);
        }

        public void GrabImage()
        {
            if(state == CursorStates.STOPPED)
            {
                int i = 0;
                foreach(CursorImage image in CursorImages)
                {
                    Rectangle rect = new Rectangle { x = image.coords.X, y = image.coords.Y, width = image.texture.width * GOAL_MAX_SIZE, height = image.texture.height * GOAL_MAX_SIZE};
                    
                    if(Raylib.CheckCollisionPointRec(CurrentMousePos, rect))
                    {
                        HeldImage = image;
                        HeldImageSize = CurrentCursorImageSize;
                        ImageHeldSizeLerping = false;
                        state = CursorStates.HOLDING;
                        GoalHeldImageSize = GOAL_HELD_MAX_SIZE * image.scale;
                        break;
                    }
                    i++;
                }
            }
        }

        public void DrawHeldImage()
        {
            if(state == CursorStates.HOLDING)
            {
                if(HeldImage != null)
                {
                    Vector2 correctPos = new Vector2 { X = CurrentMousePos.X - HeldImage.texture.width * 0.5f, Y = CurrentMousePos.Y - HeldImage.texture.height * 0.5f };
                    //Console.WriteLine("HeldImage X, Y, W, H: [" + CurrentMousePos.X + "," + CurrentMousePos.Y + "," + HeldImage.texture.width + "," + HeldImage.texture.height + "]");
                    Raylib.DrawTextureEx(HeldImage.texture, correctPos, 0, 1, Color.WHITE);
                    //Raylib.DrawTexture(HeldImage.texture, (int)correctPos.X, (int)correctPos.Y, Color.WHITE);

                    //Raylib.DrawText("HeldImage X, Y, W, H: [" + CurrentMousePos.X + "," + CurrentMousePos.Y + "," + HeldImage.texture.width + "," + HeldImage.texture.height + "]", 5, 110, 24, Color.BLACK);
                    //Raylib.DrawCircle((int)(CurrentMousePos.X - HeldImage.texture.width / 2), (int)(CurrentMousePos.Y - HeldImage.texture.height / 2), 10, Color.BLUE);
                    //Raylib.DrawText("HeldImage [" + HeldImage.coords.X + "," + HeldImage.coords.Y + "]", (int)HeldImage.coords.X, (int)HeldImage.coords.Y, 22, Color.GREEN);
                }
            }
        }

        public CursorImage AddHeldImageToWorld()
        {
            if (state == CursorStates.HOLDING)
            {
                return HeldImage;
            }
            else return null;
        }

        public void RemoveHeldImageFromCursor()
        {
            state = CursorStates.ACTIVE;
            ImageSpinLerping = false;
            ImageSizeLerping = false;
            ImageCenterLerping = false;
            ImageHeldSizeLerping = false;
            GoalCursorImageSize = GOAL_MIN_SIZE;
            MiddleOfScreen.X = 1920 / 2;
            MiddleOfScreen.Y = 1080 / 2;
            firstimageangle = 0;
            HeldImage = null;
        }

        public void RemoveHeldImage()
        {
            CursorImages.Remove(HeldImage);
            state = CursorStates.ACTIVE;
            HeldImage = null;
        }

        public void CalculateImagePositions(Vector2 camera)
        {
            if (CursorImages.Count > 0 && state != CursorStates.HOLDING)
            {
                Vector2 ImagePos = new Vector2();

                double angle;

                double anglebetweenimages = 360 / CursorImages.Count;

                int numpicture = 0;

                for (int i = 0; i < 360; i += (int)anglebetweenimages)
                {
                    angle = i;
                    angle *= Math.PI / 180;

                    if (state == CursorStates.STOPPED)
                    {
                        ImagePos.X = (float)(StoppedMousePos.X + (Math.Sin(angle) * CircleRadius));
                        ImagePos.Y = (float)(StoppedMousePos.Y - (Math.Cos(angle) * CircleRadius));
                        ImagePos.X += camera.X;
                        ImagePos.Y += camera.Y;
                    }
                    else if (state == CursorStates.ACTIVE)
                    {
                        ImagePos.X = (float)(CurrentMousePos.X + (Math.Sin(angle) * CircleRadius));
                        ImagePos.Y = (float)(CurrentMousePos.Y - (Math.Cos(angle) * CircleRadius));
                    }
                    
                    //Raylib.DrawCircleLines((int)StoppedMousePos.X, (int)StoppedMousePos.Y, CircleRadius, Color.RED);
                    //Raylib.DrawCircle((int)ImagePos.X, (int)ImagePos.Y, 10, Color.RED);
                    //Raylib.DrawText("Angle [" + firstimageangle + "]", 5, 90, 24, Color.BLACK);
                   
                    
                    Vector2 correctPos = new Vector2 { X = (int)(ImagePos.X - CursorImages[numpicture].texture.width * CurrentCursorImageSize / 2), Y = (int)(ImagePos.Y - CursorImages[numpicture].texture.width * CurrentCursorImageSize / 2) };
                    CursorImages[numpicture].coords = correctPos;
                    numpicture++;
                    if (numpicture >= CursorImages.Count)
                        break;
                }
            }
        }

        public void DrawCursorImagesAroundMouse()
        {
            if(state != CursorStates.HOLDING)
            {
                foreach (CursorImage image in CursorImages)
                {
                    Raylib.DrawTextureEx(image.texture, image.coords, 0, CurrentCursorImageSize, Color.WHITE);
                }
            }
        }

        public void LerpSmall()
        {
            if (!ImageSpinLerping)
            {
                var rate = (-100f * Raylib.GetFrameTime()) * (-100f * Raylib.GetFrameTime());
                var asd = (float)(-rate * Math.Log2(0.75));
                firstimageangle = Utils.Utilities.Lerp(firstimageangle, circle, asd);
                if (firstimageangle < 1)
                {
                    CurrentCursorImageSize = GoalCursorImageSize;
                    ImageSpinLerping = true;
                }
            }

            if(!ImageSizeLerping)
            {
                var rate = (-100f * Raylib.GetFrameTime()) * (-100f * Raylib.GetFrameTime());
                var asd = (float)(-rate * Math.Log2(0.75));
                CurrentCursorImageSize = Utils.Utilities.Lerp(CurrentCursorImageSize, GoalCursorImageSize, asd);
                
                if (CurrentCursorImageSize == GoalCursorImageSize)
                {
                    CurrentCursorImageSize = GoalCursorImageSize;
                    ImageSizeLerping = true;
                }
            }

            if(!ImageCenterLerping)
            {
                var rate = (-100f * Raylib.GetFrameTime()) * (-100f * Raylib.GetFrameTime());
                var asd = (float)(-rate * Math.Log2(0.75));
                StoppedMousePos = Vector2.Lerp(StoppedMousePos, MiddleOfScreen, asd);
                if (StoppedMousePos == MiddleOfScreen)
                {
                    CurrentCursorImageSize = GoalCursorImageSize;
                    ImageCenterLerping = true;
                }
            }

            if(!ImageHeldSizeLerping)
            {
                var rate = (-100f * Raylib.GetFrameTime()) * (-100f * Raylib.GetFrameTime());
                var asd = (float)(-rate * Math.Log2(0.5));
                HeldImageSize = Utils.Utilities.Lerp(HeldImageSize, GoalHeldImageSize, asd);
                if (HeldImageSize == GOAL_HELD_MAX_SIZE)
                {
                    HeldImageSize = GOAL_HELD_MAX_SIZE;
                    ImageHeldSizeLerping = true;
                }
            }

        }

        public void LerpBig()
        {

        }
    }
}
