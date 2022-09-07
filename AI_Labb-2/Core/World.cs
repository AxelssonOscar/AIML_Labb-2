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
    public class World
    {
        private Vector2 camera;

        private const int MAXIMUM_IMAGES_WIDTH = 5;
        private const int WINDOW_HEIGHT = 1080;
        private const int WINDOW_WIDTH = 1920;

        private List<WorldSpaceImage> DisplayedImages;
        private List<WorldSpaceImage> LoadedImages;
        private WorldSpaceImage FullscreenImage;


        private bool switchTarget = false;

        private Vector2 CurrentMousePosition;
        private Vector2 PreviousMousePosition;

        private float scale = 1f;

        private WorldState state = WorldState.IMAGES;

        private Cursor mouseCursor;

        private int NumPicture = 0;

        private bool firstImage = true;

        bool newinput = false;

        private enum WorldState
        {
            FULLSCREEN_IMAGE,
            IMAGES
        }

        public World()
        {
            DisplayedImages = new List<WorldSpaceImage>();
            LoadedImages = new List<WorldSpaceImage>();
            PreviousMousePosition = Raylib.GetMousePosition();
            mouseCursor = new Cursor();
        }


        public void AddImageToWorld(WorldSpaceImage worldImage)
        {
            worldImage.Id = NumPicture;
            LoadedImages.Add(worldImage);
            float diff = worldImage.image.texture.height / worldImage.image.thumbnail.height;

            mouseCursor.AddImagesToCursor(worldImage.image.thumbnail, NumPicture, diff);
            NumPicture++;
        }

        public bool AddImageToDisplay(CursorImage cursorImage)
        {
            WorldSpaceImage worldImage;
            foreach(var image in LoadedImages)
            {
                if(image.Id == cursorImage.Id)
                {
                    if(firstImage)
                    {
                        worldImage = image;

                        Vector2 tagsCoordinates = new Vector2(CurrentMousePosition.X - camera.X - worldImage.image.texture.width / 2, CurrentMousePosition.Y - camera.Y - worldImage.image.texture.height / 2);
                        Vector2 imageCoordinates = new Vector2(CurrentMousePosition.X - camera.X - worldImage.image.texture.width / 2, CurrentMousePosition.Y - camera.Y - worldImage.image.texture.height / 2);

                        worldImage.Coordinates = imageCoordinates;

                        worldImage.CalculateCornerPositions();
                        worldImage.CalculateHitbox();

                        DisplayedImages.Add(worldImage);
                        firstImage = false;
                        return true;
                        
                    }
                    else
                    {
                        var closestImage = FindClosestCorner(image.image.texture.width, image.image.texture.height);
                        if (closestImage == null)
                        {
                            return false;
                        }
                        worldImage = image;
                        if (closestImage.Item3 == 0)
                        {
                            worldImage.Coordinates.X = closestImage.Item1.Coordinates.X;
                            worldImage.Coordinates.Y = closestImage.Item1.Coordinates.Y - cursorImage.texture.height * 2;
                        }

                        if (closestImage.Item3 == 1)
                        {
                            worldImage.Coordinates.X = closestImage.Item1.Coordinates.X + closestImage.Item1.image.texture.width;
                            worldImage.Coordinates.Y = closestImage.Item1.Coordinates.Y;
                        }

                        if (closestImage.Item3 == 2)
                        {
                            worldImage.Coordinates.X = closestImage.Item1.Coordinates.X;
                            worldImage.Coordinates.Y = closestImage.Item1.Coordinates.Y + closestImage.Item1.image.texture.height;
                        }
                        worldImage.CalculateCornerPositions();
                        worldImage.CalculateHitbox();
                        DisplayedImages.Add(worldImage);
                        break;
                    }
                    
                }
            }
            mouseCursor.StateCursor(camera);
            return true;
        }

        private bool CheckImageHitbox(Vector2 coordinate, int width, int height)
        {
            Rectangle temp = new Rectangle();

            temp.x = coordinate.X;
            temp.y = coordinate.Y;
            temp.width = width;
            temp.height = height;

            foreach(WorldSpaceImage rect in DisplayedImages)
            {
                if(Raylib.CheckCollisionRecs(temp, rect.Hitbox))
                {
                    return true;
                }
            }
            return false;
        }

        private bool CheckFullscreenHitbox()
        {
            if(FullscreenImage != null)
            {
                if (Raylib.CheckCollisionPointRec(CurrentMousePosition, FullscreenImage.FullscreenHitbox))
                    return false;
                else
                    return true;
            }
            return true;
            
        }

        public Tuple<WorldSpaceImage, Vector2, int> FindClosestCorner(int width, int height)
        {
            Vector2 imageTopLeft = Vector2.Zero;

            Vector2 closestCornerVector = Vector2.Zero;
            float closestCornerDistance = 100000;

            var foundImage = DisplayedImages[0];
            bool foundCorner = false;

            int numCorner = 0;


            foreach (var image in DisplayedImages)
            {
                for (int i = 0; i < image.Corners.Count; i++)
                {
                    Corner corner = image.Corners[i];
                    bool legal = CheckImageHitbox(corner.Position, width, height);

                    if (corner.Taken == false && !legal)
                    {
                        Vector2 correct = corner.Position;
                        correct.X += camera.X;
                        correct.Y += camera.Y;

                        float distance = Vector2.Distance(correct, mouseCursor.HeldImage.coords);

                        if (distance < closestCornerDistance && distance < 500)
                        {
                            foreach (var picture in LoadedImages)
                            {
                                if (picture.Id == mouseCursor.HeldImage.Id)
                                {
                                    closestCornerDistance = distance;
                                    closestCornerVector = corner.Position;
                                    foundImage = image;
                                    numCorner = i;
                                    foundCorner = true;
                                }
                            }
                        }
                    }
                }
            }

            if(!foundCorner)
            {
                return null;
            }

            foreach (var picture in LoadedImages)
            {
                if (picture.Id == mouseCursor.HeldImage.Id)
                {
                    width = picture.image.texture.width;
                    height = picture.image.texture.height;
                    foundImage.FlipCorner(numCorner);

                    return new Tuple<WorldSpaceImage, Vector2, int>(foundImage, closestCornerVector, numCorner);
                }
            }

            return null;
        }


        public void DrawClosestCorner()
        {
            Vector2 imageTopLeft = Vector2.Zero;

            Vector2 closestCornerVector = Vector2.Zero;
            float closestCornerDistance = 100000;

            int width = 0;
            int height = 0;
            Vector2 DrawFromCorner = Vector2.Zero;
            bool FoundCorner = false;

            foreach (var image in DisplayedImages)
            {
                for(int i = 0; i < image.Corners.Count; i++)
                {
                    Corner corner = image.Corners[i];
                    bool legal = CheckImageHitbox(corner.Position, width, height);

                    if (corner.Taken == false && !legal)
                    {
                        Vector2 correct = corner.Position;
                        correct.X += camera.X;
                        correct.Y += camera.Y;

                        float distance = Vector2.Distance(correct, mouseCursor.HeldImage.coords);

                        if(distance < closestCornerDistance && distance < 500)
                        {
                            foreach (var picture in LoadedImages)
                            {
                                if(picture.Id == mouseCursor.HeldImage.Id)
                                {
                                    closestCornerDistance = distance;
                                    closestCornerVector = corner.Position;
                                    width = picture.image.texture.width;
                                    height = picture.image.texture.height;
                                    FoundCorner = true;

                                    DrawFromCorner = correct;
                                    
                                    //Raylib.DrawCircle((int)(correct.X), (int)(correct.Y), 10, Color.RED);
                                    //Raylib.DrawLine((int)correct.X, (int)correct.Y, (int)mouseCursor.HeldImage.coords.X, (int)mouseCursor.HeldImage.coords.Y, Color.RED);
                                }
                            }
                        }
                    }
                }
            }
            if(FoundCorner)
            {
                Raylib.DrawRectangle((int)(DrawFromCorner.X), (int)(DrawFromCorner.Y), width, height, new Color(0, 255, 0, 30));
            }
        }

        public void Update()
        {
            if(switchTarget)
            {
                if(state == WorldState.FULLSCREEN_IMAGE)
                {
                    foreach (WorldSpaceImage image in DisplayedImages)
                    {
                        //image.GoalCoordinates = image.CoordinatesWithInfo;
                        //image.GoalColor.a = 255;
                    }
                }
                else
                {
                    foreach(WorldSpaceImage image in DisplayedImages)
                    {
                        //image.GoalCoordinates = image.CoordinatesWithoutInfo;
                        //image.GoalColor.a = 0;
                    }
                }

                switchTarget = false;
                
            }

            mouseCursor.UpdateCursor(CurrentMousePosition);

            if(mouseCursor.state == CursorStates.HOLDING)
            {
                mouseCursor.UpdateHeldImagePosition(CurrentMousePosition);
            }

            mouseCursor.CalculateImagePositions(camera);
            mouseCursor.LerpSmall();
            mouseCursor.LerpBig();

        }

        public void HandleInput()
        {
            newinput = false;
            if (Raylib.IsKeyDown(KeyboardKey.KEY_UP)) camera.Y -= 100 * Raylib.GetFrameTime();
            if (Raylib.IsKeyDown(KeyboardKey.KEY_LEFT)) camera.X -= 100 * Raylib.GetFrameTime();
            if (Raylib.IsKeyDown(KeyboardKey.KEY_DOWN)) camera.Y += 100 * Raylib.GetFrameTime();
            if (Raylib.IsKeyDown(KeyboardKey.KEY_RIGHT)) camera.X += 100 * Raylib.GetFrameTime();

            if (Raylib.IsKeyPressed(KeyboardKey.KEY_KP_ADD)) scale += 0.05f;
            if (Raylib.IsKeyPressed(KeyboardKey.KEY_KP_SUBTRACT)) scale -= 0.05f;


            if(Raylib.IsKeyPressed(KeyboardKey.KEY_LEFT_CONTROL) && state == WorldState.IMAGES)
            {
                mouseCursor.StateCursor(camera);
            }

            if (Raylib.IsMouseButtonPressed(0) && state != WorldState.IMAGES && !newinput)
            {
                if (CheckFullscreenHitbox())
                    ChangeState();
                newinput = true;
            }

            CurrentMousePosition = Raylib.GetMousePosition();

            Vector2 delta = PreviousMousePosition - CurrentMousePosition;

            PreviousMousePosition = CurrentMousePosition;

            if (Raylib.IsMouseButtonDown(0) && mouseCursor.state != CursorStates.STOPPED && state != WorldState.FULLSCREEN_IMAGE && !newinput)
                camera -= delta;


            if (Raylib.IsMouseButtonPressed(0) && mouseCursor.state == CursorStates.ACTIVE && state == WorldState.IMAGES && DisplayedImages.Count > 0 && !newinput)
            {
                DetailedImageView();
                newinput = true;
            }
            
            
            if(Raylib.IsMouseButtonReleased(0) && mouseCursor.state == CursorStates.STOPPED)
                mouseCursor.GrabImage();

            if (Raylib.IsMouseButtonPressed(MouseButton.MOUSE_BUTTON_LEFT) && mouseCursor.state == CursorStates.HOLDING)
            {
                if (AddImageToDisplay(mouseCursor.AddHeldImageToWorld()))
                {
                    mouseCursor.RemoveHeldImage();
                }
            }
            else if(Raylib.IsMouseButtonPressed(MouseButton.MOUSE_BUTTON_RIGHT) && mouseCursor.state == CursorStates.HOLDING)
            {
                mouseCursor.RemoveHeldImageFromCursor();
            }
            
        }

        public void RenderWorld(bool loadingImage)
        {
            Raylib.BeginDrawing();
            Raylib.ClearBackground(Color.DARKGRAY);

            
            

            if (DisplayedImages.Count > 0 && mouseCursor.state == CursorStates.HOLDING)
            {
                DrawClosestCorner();
            }

            
            if (DisplayedImages.Count > 0)
            {

                if (state == WorldState.IMAGES)
                {
                    foreach (WorldSpaceImage image in DisplayedImages)
                    {
                        image.DrawImages(camera, scale);
                        //image.DrawCornerPositions(camera);
                    }
                }

                if (state == WorldState.FULLSCREEN_IMAGE && FullscreenImage != null)
                {
                    FullscreenImage.DrawFullscreenImage();
                }

            }
            else if(DisplayedImages.Count == 0 && mouseCursor.CursorImages.Count == 0)
            {
                Raylib.DrawText("Drag a image file into the window", 500, 1080/2, 50, Color.RED);
            }

            if(state != WorldState.FULLSCREEN_IMAGE)
            {
                mouseCursor.DrawCursorImagesAroundMouse();
                mouseCursor.DrawHeldImage();
            }


            //Raylib.DrawFPS(0, 0);
            //Raylib.DrawText("ScreenCursor [" + CurrentMousePosition.X + "," + CurrentMousePosition.Y + "]", 5, 20, 24, Color.BLACK);
            //Raylib.DrawText("Camera [" + camera.X + "," + camera.Y + "]", 5, 44, 24, Color.BLACK);
            //Raylib.DrawText("WorldCursor [" + (CurrentMousePosition.X - camera.X) + "," + (CurrentMousePosition.Y - camera.Y) + "]", 5, 68, 24, Color.BLACK);

            
            Raylib.DrawText("Drag files into the window to load them in.", 5, 1080 - 5 * 22, 22, Color.BLACK);
            Raylib.DrawText("Click left ctrl to bring the images into clicking mode.", 5, 1080 - 4 * 22, 22, Color.BLACK);
            Raylib.DrawText("Click one of the images to hold onto it, then click it onto the surface.", 5, 1080 - 3 * 22, 22, Color.BLACK);
            Raylib.DrawText("If there already is an image, snap your held image to one of its sides.", 5, 1080 - 2 * 22, 22, Color.BLACK);
            Raylib.DrawText("Click on an image on the surface to get more information about it.", 5, 1080 - 1 * 22, 22, Color.BLACK);
            Raylib.DrawText("Click and drag the surface to move the world around.", 5, 1080 - 0 * 22, 22, Color.BLACK);

            Raylib.EndDrawing();
        }

        public void ChangeState()
        {
            foreach(WorldSpaceImage image in DisplayedImages)
            {
                image.LerpingDone = false;
            }

            if (state == WorldState.FULLSCREEN_IMAGE)
                state = WorldState.IMAGES;
            else
                state = WorldState.FULLSCREEN_IMAGE;

            switchTarget = true;
        }

        public void DetailedImageView()
        {
            Vector2 mousepos = CurrentMousePosition;

            foreach(WorldSpaceImage image in DisplayedImages)
            {
                Rectangle temp = image.Hitbox;
                temp.x += camera.X;
                temp.y += camera.Y;
                
                if(Raylib.CheckCollisionPointRec(mousepos, temp))
                {
                    FullscreenImage = image;
                    ChangeState();
                    break;
                }
            }
        }

    }
}
