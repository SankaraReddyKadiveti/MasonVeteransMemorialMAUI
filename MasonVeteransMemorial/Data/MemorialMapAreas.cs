using System;
using System.Diagnostics;
using SkiaSharp;
//using SkiaSharp.Views.Forms;
//using Xamarin.Forms;
using static MasonVeteransMemorial.Controls.SkiaSharp.SkiaSharpCommon;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using MasonVeteransMemorial.Models;
using SkiaSharp.Views.Maui.Controls;

namespace MasonVeteransMemorial.Data
{
    public class MemorialMapAreas
    {
        public MemorialMapAreas()
        {
            GenerateAlphabetList();
        }

        #region Brick Properties
        //*** Brick Layout Info ******************************
        public static float Brick_ScaleXOffset = 1057; //1057
        public static float Brick_TopAngle = 20;
        public static float Brick_OppAdjRatio = -0.35f;//   0.368686868686869f; //.45f;
        public static float Brick_Opposite = 198; // 198
        public static float Brick_Adjacent = 53;
        public static float Brick_HR_Width = 1556f + 2 * 76.58823f; // 1056f // 706f + 2 * 46.58823f
        public static float Brick_HR_Height = 245;  //682 //235

        public static float Brick_HR_StartLeft = 16f + 135.64705f; //16f + 77.64705f //-40f + 0.64705f;
        public static float Brick_HR_StartTop = 910f; //671f //441f

        private static List<string> AlphaList = null;
        public static bool ShowHonorRollGridLines = false;
        public static float CircleRadiusHilight = 110;

        public static float BigToSmallSizeFactor = 1;
        public static float BigToSmallSizeRatio = 1;
        //****************************************************
        #endregion

        #region BitMap Settings
        public static bool UseNonDefaultBitmap = true;
        public const float Bitmap_Default_Width = 3998;
        public const float Bitmap_Default_Height = 1465;


        #endregion


        #region Canvases
        public const float Canvas_Default_X10_Width = 1125; // 1125
        public const float Canvas_Default_X10_Height = 1665;

        public static float Canvas_ScaleX_Factor = 1;
        public static float Canvas_ScaleY_Factor = 1;
        public static float Canvas_TransX_Factor = 0;
        public static float Canvas_TransY_Factor = 0;

        public static float Canvas_Current_Width = 0f;
        public static float Canvas_Current_Height = 0f;
        public static float BitMap_Current_Width = 0f;
        public static float BitMap_Current_Height = 0f;
        #endregion

        #region Paint and Stroke Properties
        static SKPaint textPaint = new SKPaint
        {
            //Style = SKPaintStyle.Stroke,
            Color = SKColors.White,
            StrokeWidth = .5f
        };

        static SKPaint paintLine = new SKPaint
        {
            Style = SKPaintStyle.Stroke,
            Color = SKColors.Black , // Color.Black.ToSKColor(),
            //IsAntialias = true,
            StrokeWidth = 1,
        };

        static SKPaint paintHiLite = new SKPaint
        {
            Style = SKPaintStyle.Fill,
            Color = new SKColor(255, 255, 0, 127), // Color.Yellow.ToSKColor().WithAlpha(127),
            IsAntialias = true

        };

        static SKPaint paintGridBackground = new SKPaint
        {
            Style = SKPaintStyle.Fill,
            Color = SKColors.Transparent.FromHex("#be5959"), //8e3f3f //be5959
            //IsAntialias = true,
        };

        static SKPaint paintHiLiteLine = new SKPaint
        {
            Style = SKPaintStyle.Stroke,
            Color = new SKColor(255, 255, 0), // Color.Yellow.ToSKColor(),
            StrokeWidth = 3,
            //IsAntialias = true,
        };

        static SKPaint paintHiLiteRed = new SKPaint
        {
            Style = SKPaintStyle.Fill,
            Color = new SKColor(255, 0, 0), // Color.Red.ToSKColor(),
            //IsAntialias = true,
        };
        #endregion

        #region Animation Properties
        public static float DefaultAnimateTime = 800; //1000 = 1 sec
        public static float DefaultAnimateTimeIncrement = 40; //in Milliseconds

        public static float DefaultZoomOutAnimateTime = 500; //1000 = 1 sec
        public static float DefaultZoomOutAnimateTimeIncrement = 40; //in Milliseconds

        public const int DefaultZoomOutPause = 1000; //1 sec

        public static float AnimateTime = 0;
        public static float AnimateTimeIncrement = 0;

        static Stopwatch stopwatch = new Stopwatch();
        public const double CycleTime = 1000;
        public static float TimeElapsed = 0;

        static bool timerBusy = false;
        static bool showHonorCircle = false;

        public static bool ZoomOut { get; set; }

        public static float ScaleX_Animate_Unit { get; set; }
        public static float ScaleY_Animate_Unit { get; set; }
        public static float TransX_Animate_Unit { get; set; }
        public static float TransY_Animate_Unit { get; set; }

        static bool flashBrickOn = false;
        static int skipBrickCount = 2;
        static int skipBrickCounter = 0;
        static int flashBrickLoop = 20;

        public static bool NotFirstClear { get; set; }

        #endregion

        #region Map Areas Properties
        public static MapArea MoveToMapArea = View_Full_Fit;
        public static MapArea View_Animate { get; set; }
        public static MapArea View_Animate_Dest { get; set; }
        public static MapArea View_Dest { get; set; }

        public static bool IsMoveToMapArea = false;
        public static bool IsMoveToMapAreaAnimate = false;
        #endregion

        public static float GetBrickRightTriangleOppositeSideFromAngle(float adjacent)
        {
            return adjacent * Brick_OppAdjRatio; //Opposite angle of 20 degrees
        }

        public static SKRect CreateRect(SKPoint start, float height, float width = 0)
        {
            var rect = new SKRect(start.X, start.Y, width <= 0 ? height : width, height);
            return rect;
        }

        public static void UpdateScaleFactorsForCanvasSize(float newWidth, float newHeight, float newBitMapWidth, float newBitMapHeight)
        {
            Canvas_Current_Width = newWidth;
            Canvas_Current_Height = newHeight;

            BitMap_Current_Width = newBitMapWidth;
            BitMap_Current_Height = newBitMapHeight;

            if (newWidth.Equals(Canvas_Default_X10_Width) && newHeight.Equals(Canvas_Default_X10_Height))
                return;

            Canvas_TransX_Factor = Canvas_Default_X10_Width - (newWidth / Canvas_Default_X10_Width * Canvas_Default_X10_Width);// Canvas_Default_X10_Width - newWidth;
            Canvas_TransY_Factor = Canvas_Default_X10_Height - (newHeight / Canvas_Default_X10_Height * Canvas_Default_X10_Height);//Canvas_Default_X10_Height - newHeight;

            CircleRadiusHilight = CircleRadiusHilight * newWidth / Canvas_Default_X10_Width;

            Canvas_ScaleX_Factor = newWidth / Canvas_Default_X10_Width;
            Canvas_ScaleY_Factor = newHeight / Canvas_Default_X10_Height;

            if (!newBitMapWidth.Equals(Bitmap_Default_Width) || !newBitMapHeight.Equals(Bitmap_Default_Height))
            {
                BigToSmallSizeFactor = Bitmap_Default_Height / newBitMapHeight;
                BigToSmallSizeRatio = newBitMapHeight / Bitmap_Default_Height;
                UseNonDefaultBitmap = true;
            }

            if (!UseNonDefaultBitmap)
            {
                Brick_HR_Width = 706f + 2 * 46.58823f;
                Brick_ScaleXOffset = 1057f;
            }
            else
            {
                Brick_HR_Width = (float)((1 * 706) + 2 * 46.58823f);
                Brick_ScaleXOffset = (float)(1057f * 1);
            }

            if (Device.RuntimePlatform == Device.iOS)
            {
                DefaultAnimateTime = 1800;
                DefaultZoomOutAnimateTime = 1000;
            }
            else
            {
                DefaultAnimateTime = 700;
                DefaultZoomOutAnimateTime = 400;
            }
        }

        public static void CreateHonorRollAngledBrickGrid(SKCanvas canvas, SKMatrix matrix, Brick searchForBrick = null, bool wingBricks = false)
        {
            if (flashBrickOn)
            {
                if (skipBrickCounter <= skipBrickCount)
                {
                    skipBrickCounter++;
                    return;
                }
                else
                {
                    skipBrickCounter = 0;
                }
            }

            var bricksInRowDivide = 38; //2 less than max //34
            var maxLayerCount = 40; //MM   48; //VV

            if (wingBricks)
            {
                bricksInRowDivide = 7;
                maxLayerCount = 14;
            }

            float areaWidth = 0;
            float areaHeight = 0;

            float startFrom = 0;
            float startFromY = 0;

            if (UseNonDefaultBitmap)
            {
                areaWidth = (Brick_HR_Width) * matrix.ScaleX * BigToSmallSizeRatio;
                areaHeight = (Brick_HR_Height) * matrix.ScaleY * BigToSmallSizeRatio;
                startFrom = ((Brick_HR_StartLeft + Brick_ScaleXOffset) * matrix.ScaleX * BigToSmallSizeRatio) + matrix.TransX;
                startFromY = ((Brick_HR_StartTop) * matrix.ScaleY * BigToSmallSizeRatio) + matrix.TransY;
            }
            else
            {
                areaWidth = (Brick_HR_Width) * matrix.ScaleX;
                areaHeight = (Brick_HR_Height) * matrix.ScaleY;
                startFrom = ((Brick_HR_StartLeft + Brick_ScaleXOffset) * matrix.ScaleX) + matrix.TransX;
                startFromY = ((Brick_HR_StartTop) * matrix.ScaleY) + matrix.TransY;
            }

            var modelWidth = areaWidth / bricksInRowDivide;
            var modelHeight = areaHeight / maxLayerCount;

            GenerateAlphabetList();

            var brickPalate = new BrickPalate
            {
                MaxLayerCount = maxLayerCount,
                MaxLayerBrickCount = bricksInRowDivide,
                StartFromY = startFromY,
                LayerNumber = 0,
                LayerWidth = areaWidth,
                WallHeight = areaHeight,
                StartFrom = startFrom,
                Layers = new SKPath(),
                model = CreateRect(new SKPoint(0, 0), modelHeight, modelWidth),
                BrickText = $"Rocky: ",
                Bricktionary = new List<BrickDetail>(),
                IsFirstLayer = true,
                CircleRadius = CircleRadiusHilight * matrix.ScaleX
            };

            if (UseNonDefaultBitmap)
                brickPalate.CircleRadius = CircleRadiusHilight * matrix.ScaleX * BigToSmallSizeRatio;

            //canvas.RotateDegrees((float)rotateSlider.Value, info.Width / 2, info.Height / 2);

            var brickCodeSelected = "";

            if (null != searchForBrick && searchForBrick.Section.ToUpper() == "HONOR" && showHonorCircle)
            {
                var position = searchForBrick.Position;
                //if (position == 33)
                //position = 32;

                if (searchForBrick.Position != 0 && searchForBrick.Section.ToUpper() == "Honor".ToUpper())
                {
                    brickCodeSelected = $"{searchForBrick.Location}-{position}";
                }
            }

            LayAngledBricksWithLinesORIG(canvas, brickPalate, brickCodeSelected); //searchForBrick.Location
            canvas.DrawPath(brickPalate.Layers, paintLine);
        }

        public static string Right(string value, int length)
        {
            return value.Substring(value.Length - length);
        }

        public static void LayAngledBricksWithLines(SKCanvas canvas, BrickPalate palate, string searchBrickCode)
        {
            bool StartFullBrick = palate.LayerNumber % 2 == 0, isFirstBrick = true; float currentBricksWidth = 0; bool skipText = false;
            int brickCount = 1;

            if (palate.IsFirstLayer)
            {
                palate.NextLayerTop -= palate.StartFromY;
                palate.DrawTo = 0;
                palate.IsFirstLayer = false;
            }
            else
            {
                palate.StartFrom += GetBrickRightTriangleOppositeSideFromAngle(palate.model.Height);
                palate.DrawTo += GetBrickRightTriangleOppositeSideFromAngle(palate.model.Height) / 2;
            }

            currentBricksWidth = palate.StartFrom;

            var currentLayerWidth = palate.StartFrom + palate.LayerWidth + palate.DrawTo;

            if (ShowHonorRollGridLines)
                canvas.DrawLine(palate.StartFrom, palate.NextLayerTop, currentLayerWidth, palate.NextLayerTop, paintLine);
            var brickWidth = currentLayerWidth / palate.MaxLayerBrickCount;//    palate.model.Width;

            while (currentBricksWidth <= currentLayerWidth)
            //while (currentBricksWidth <= palate.LayerWidth + palate.StartFrom)
            {

                skipText = false;
                if (isFirstBrick && !StartFullBrick)
                {
                    brickWidth = brickWidth / 2;
                    isFirstBrick = false;
                    skipText = true;
                }

                if (ShowHonorRollGridLines)
                    canvas.DrawLine(currentBricksWidth, palate.NextLayerTop, currentBricksWidth, palate.NextLayerTop /*+ palate.model.Height*/, paintLine);

                palate.TotalBrickCount++;

                if (!skipText)
                {
                    var loc = AlphaList[palate.MaxLayerCount - palate.LayerNumber - 1];
                    var pos = brickCount;

                    //var brickText = $"{palate.BrickText}{loc}-{brickCount.ToString()}";
                    var brickText = $"{loc}-{brickCount.ToString()}";
                    // Adjust TextSize property so text is 90% of screen width
                    float textWidth = textPaint.MeasureText(brickText + "brick");
                    textPaint.TextSize = .8f * brickWidth * textPaint.TextSize / textWidth;
                    textWidth = textPaint.MeasureText(brickText);
                    var textX = currentBricksWidth + (brickWidth / 2 - textWidth / 2);
                    var textY = (float)(palate.NextLayerTop + palate.model.Height / 2);


                    if (!string.IsNullOrEmpty(searchBrickCode))
                    {
                        var coords = searchBrickCode.Split('-');
                        if (null != coords && coords.Length == 2 && coords[0].ToUpper() == loc.ToUpper() && coords[1] == pos.ToString())
                        {
                            //palate.Layers.AddCircle(currentBricksWidth, palate.NextLayerTop, palate.CircleRadius);
                            canvas.DrawCircle(currentBricksWidth, palate.NextLayerTop, palate.CircleRadius, paintHiLite);// paintHiLiteLine);
                            //canvas.Restore();
                            //canvas.Save();
                            //canvas.Restore();
                        }
                        else
                        {
                            if (ShowHonorRollGridLines)
                                canvas.DrawText(brickText, textX, textY, textPaint);
                        }
                    }
                    else
                    {
                        if (ShowHonorRollGridLines)
                            canvas.DrawText(brickText, textX, textY, textPaint);
                        //canvas.Save();
                    }

                    brickCount++;
                }

                currentBricksWidth += brickWidth;
            }

            if (ShowHonorRollGridLines)
                canvas.DrawLine(currentBricksWidth, palate.NextLayerTop, currentBricksWidth, palate.NextLayerTop/* + palate.model.Height*/, paintLine);

            palate.LayerNumber++; palate.NextLayerTop -= palate.model.Height; canvas.Restore();

            if (palate.LayerNumber < palate.MaxLayerCount)
                LayAngledBricksWithLines(canvas, palate, searchBrickCode);
            else
            {
                if (ShowHonorRollGridLines)
                    canvas.DrawLine(palate.StartFrom, palate.NextLayerTop, palate.StartFrom + palate.LayerWidth + palate.DrawTo, palate.NextLayerTop, paintLine);
            }
        }


            /*      var brickText = $"{palate.BrickText}{loc}-{brickCount.ToString()}";
                    var brickText = $"{loc}-{brickCount.ToString()}";

                    // Generate the brick code based on the current layer letter and position
                    var brickText = $"{currentLayerLetter}{loc}-{brickCount.ToString()}";

                    // Adjust TextSize property so text is 90% of screen width
                    float textWidth = textPaint.MeasureText(brickText + "brick");
                    textPaint.TextSize = .8f * brickWidth * textPaint.TextSize / textWidth;
                    textWidth = textPaint.MeasureText(brickText);
                    var textX = currentBricksWidth + (brickWidth / 2 - textWidth / 2);
                    var textY = (float)(palate.NextLayerTop + palate.model.Height / 2);


                    if (!string.IsNullOrEmpty(searchBrickCode))
                    {
                        var coords = searchBrickCode.Split('-');
                        if (null != coords && coords.Length == 2 && coords[0].ToUpper() == loc.ToUpper() && coords[1] == pos.ToString())
                        {

                            storeAdjustPos = coords[1];

                            //palate.Layers.AddCircle(currentBricksWidth, palate.NextLayerTop, palate.CircleRadius);

                            if (int.Parse(storeAdjustPos) < 29)
                                canvas.DrawCircle(currentBricksWidth, palate.NextLayerTop, palate.CircleRadius, paintHiLite);// paintHiLiteLine);
                            //canvas.Restore();
                            //canvas.Save();

                            //var hilightBrick = new SKRect(currentBricksWidth, palate.NextLayerTop, currentBricksWidth + brickWidth, palate.model.Height + palate.NextLayerTop);
                            //canvas.DrawRect(hilightBrick, paintHiLite);

                            //canvas.Restore();

                            //paintLine.StrokeWidth = 0.5f;
                            //canvas.DrawText(brickText, textX - 5, textY + 5, paintLine);

                            //canvas.Restore();
                        }
                        else
                        {
                            if (ShowHonorRollGridLines)
                                canvas.DrawText(brickText, textX, textY, textPaint);
                        }
                    }
                    else
                    {
                        if (ShowHonorRollGridLines)
                            canvas.DrawText(brickText, textX, textY, textPaint);
                        //canvas.Save();
                    }

                    palate.Bricktionary.Add(new BrickDetail { Name = palate.BrickText, location = loc, position = pos });
                    brickCount++;
                }

                currentBricksWidth += brickWidth;
            }

            if (!string.IsNullOrEmpty(storeAdjustPos) && int.Parse(storeAdjustPos) >= 29)
                canvas.DrawCircle(currentLayerWidth - (palate.model.Width * 3), palate.NextLayerTop, palate.CircleRadius, paintHiLite);

            if (ShowHonorRollGridLines)
                canvas.DrawLine(currentBricksWidth, palate.NextLayerTop, currentBricksWidth, palate.NextLayerTop + palate.model.Height, paintLine);

            palate.LayerNumber++; palate.NextLayerTop += palate.model.Height; canvas.Restore();

            if (palate.LayerNumber < palate.MaxLayerCount)
                // LayAngledBricksWithLinesORIG(canvas, palate, searchBrickCode);
                LayAngledBricksWithLinesORIG(canvas, palate, nextLayerLetter.ToString(), searchBrickCode);
            else
            {
                if (ShowHonorRollGridLines)
                    canvas.DrawLine(palate.StartFrom, palate.NextLayerTop, palate.StartFrom + palate.LayerWidth + palate.DrawTo, palate.NextLayerTop, paintLine);
            }
        }
*/

        public static void LayAngledBricksWithLinesORIG(SKCanvas canvas, BrickPalate palate, string searchBrickCode)
        {
            bool StartFullBrick = palate.LayerNumber % 2 == 0, isFirstBrick = true; float currentBricksWidth = 0; bool skipText = false;
            int brickCount = 1;

            if (palate.IsFirstLayer)
            {
                palate.NextLayerTop += palate.StartFromY;
                palate.DrawTo = 0;
                palate.IsFirstLayer = false;
            }
            else
            {
                palate.StartFrom += GetBrickRightTriangleOppositeSideFromAngle(palate.model.Height);
                palate.DrawTo += GetBrickRightTriangleOppositeSideFromAngle(palate.model.Height) / 2;
            }

            currentBricksWidth = palate.StartFrom;

            var currentLayerWidth = palate.StartFrom + palate.LayerWidth + palate.DrawTo;

            if (ShowHonorRollGridLines)
                canvas.DrawLine(palate.StartFrom, palate.NextLayerTop, palate.StartFrom + palate.LayerWidth + palate.DrawTo, palate.NextLayerTop, paintLine);

            var storeAdjustPos = "";

            while ((currentBricksWidth + palate.model.Width) - palate.StartFrom <= palate.LayerWidth + palate.DrawTo)
            {
                var brickWidth = palate.model.Width;
                skipText = false;
                if (isFirstBrick && !StartFullBrick)
                {
                    brickWidth = palate.model.Width / 2;
                    isFirstBrick = false;
                    skipText = true;
                }

                if (ShowHonorRollGridLines)
                    canvas.DrawLine(currentBricksWidth, palate.NextLayerTop, currentBricksWidth, palate.NextLayerTop /*+ palate.model.Height*/, paintLine);

                palate.TotalBrickCount++;

                if (!skipText)
                {
                    var loc = AlphaList[palate.MaxLayerCount - palate.LayerNumber - 1];
                    var pos = brickCount;

                    //var brickText = $"{palate.BrickText}{loc}-{brickCount.ToString()}";
                    var brickText = $"{loc}-{brickCount.ToString()}";
                    // Adjust TextSize property so text is 90% of screen width
                    float textWidth = textPaint.MeasureText(brickText + "brick");
                    textPaint.TextSize = .8f * brickWidth * textPaint.TextSize / textWidth;
                    textWidth = textPaint.MeasureText(brickText);
                    var textX = currentBricksWidth + (brickWidth / 2 - textWidth / 2);
                    var textY = (float)(palate.NextLayerTop + palate.model.Height / 2);


                    if (!string.IsNullOrEmpty(searchBrickCode))
                    {
                        var coords = searchBrickCode.Split('-');
                        if (null != coords && coords.Length == 2 && coords[0].ToUpper() == loc.ToUpper() && coords[1] == pos.ToString())
                        {

                            storeAdjustPos = coords[1];

                            //palate.Layers.AddCircle(currentBricksWidth, palate.NextLayerTop, palate.CircleRadius);

                            if (int.Parse(storeAdjustPos) < 29)
                                canvas.DrawCircle(currentBricksWidth, palate.NextLayerTop, palate.CircleRadius, paintHiLite);// paintHiLiteLine);
                            //canvas.Restore();
                            //canvas.Save();

                            //var hilightBrick = new SKRect(currentBricksWidth, palate.NextLayerTop, currentBricksWidth + brickWidth, palate.model.Height + palate.NextLayerTop);
                            //canvas.DrawRect(hilightBrick, paintHiLite);

                            //canvas.Restore();

                            //paintLine.StrokeWidth = 0.5f;
                            //canvas.DrawText(brickText, textX - 5, textY + 5, paintLine);

                            //canvas.Restore();
                        }
                        else
                        {
                            if (ShowHonorRollGridLines)
                                canvas.DrawText(brickText, textX, textY, textPaint);
                        }
                    }
                    else
                    {
                        if (ShowHonorRollGridLines)
                            canvas.DrawText(brickText, textX, textY, textPaint);
                        //canvas.Save();
                    }

                    palate.Bricktionary.Add(new BrickDetail { Name = palate.BrickText, location = loc, position = pos });
                    brickCount++;
                }

                currentBricksWidth += brickWidth;
            }

            if (!string.IsNullOrEmpty(storeAdjustPos) && int.Parse(storeAdjustPos) >= 29)
                canvas.DrawCircle(currentLayerWidth - (palate.model.Width * 3), palate.NextLayerTop, palate.CircleRadius, paintHiLite);

            if (ShowHonorRollGridLines)
                canvas.DrawLine(currentBricksWidth, palate.NextLayerTop, currentBricksWidth, palate.NextLayerTop/* + palate.model.Height*/, paintLine);

            palate.LayerNumber++; palate.NextLayerTop -= palate.model.Height; canvas.Restore();
            
            if (palate.LayerNumber < palate.MaxLayerCount)
                LayAngledBricksWithLinesORIG(canvas, palate, searchBrickCode);
            else
            {
                if (ShowHonorRollGridLines)
                    canvas.DrawLine(palate.StartFrom, palate.NextLayerTop, palate.StartFrom + palate.LayerWidth + palate.DrawTo, palate.NextLayerTop, paintLine);
            }
        }


        public static void GenerateAlphabetList()
        {
            var exitOnLetter = "V";
            var alpha = "ABCDEFGHIJKLMNOPQRSTUVWXYZ".ToCharArray().Select(c => c.ToString()).ToList();
            var list = new List<string>();
            foreach (var letter in alpha)
            {
                list.Add(letter + letter);
                if (letter == exitOnLetter)
                    break;
            }

            alpha.AddRange(list);

            AlphaList = alpha;
            //AlphaList = alpha.Reverse();

        }

        public static void AnimateZoomTo(MapArea dest, SKCanvasView canvas)
        {
            View_Dest = dest;
            View_Animate_Dest = View_Full_Fit;

            Zoom(canvas);
        }

        public static void AnimateZoomToSimple(MapArea dest, SKCanvasView canvas)
        {
            View_Animate_Dest = dest;

            ZoomSimple(canvas);
        }

        public static void FlashHilight(SKCanvasView canvas)
        {
            flashBrickOn = true;

            skipBrickCounter = 0;

            //skipBrickCounter = skipBrickCount + 1;
            IsMoveToMapAreaAnimate = true;

            var animateX = flashBrickLoop;
            var animatex = 0;

            stopwatch.Start();

            Device.StartTimer(TimeSpan.FromMilliseconds(DefaultAnimateTimeIncrement), () =>
            {
                flashBrickOn = true;
                var stillBusy = false;
                timerBusy = true;
                IsMoveToMapAreaAnimate = true;

                TimeElapsed = (float)(stopwatch.Elapsed.TotalMilliseconds % CycleTime / CycleTime);

                if (animatex == animateX)
                {
                    stillBusy = false;
                    flashBrickOn = false;
                }
                else
                    stillBusy = true;

                animatex++;

                canvas.InvalidateSurface();

                if (!stillBusy)
                {
                    timerBusy = false;
                    IsMoveToMapAreaAnimate = false;
                    timerBusy = false;
                }

                if (!timerBusy)
                {
                    stopwatch.Stop();
                }
                return timerBusy;
            });
        }

        static async void Zoom(SkiaSharp.Views.Maui.Controls.SKCanvasView canvas, bool init = true)
        {

            if (!init)
            {
                showHonorCircle = true;
                AnimateTime = DefaultAnimateTime;
                AnimateTimeIncrement = DefaultAnimateTimeIncrement;
                await Task.Delay(DefaultZoomOutPause);
            }
            else
            {
                showHonorCircle = false;
                AnimateTime = DefaultZoomOutAnimateTime;
                AnimateTimeIncrement = DefaultZoomOutAnimateTimeIncrement;
            }

            //Calc
            var scaleXDiff = MoveToMapArea.ScaleX - View_Animate_Dest.ScaleX;
            var scaleYDiff = MoveToMapArea.ScaleY - View_Animate_Dest.ScaleY;
            var transXDiff = MoveToMapArea.TransX - View_Animate_Dest.TransX;
            var transYDiff = MoveToMapArea.TransY - View_Animate_Dest.TransY;


            if (scaleXDiff != 0 && scaleYDiff != 0)
            {
                ScaleX_Animate_Unit = scaleXDiff / (AnimateTime / AnimateTimeIncrement);
                ScaleY_Animate_Unit = scaleYDiff / (AnimateTime / AnimateTimeIncrement);
                TransX_Animate_Unit = transXDiff / (AnimateTime / AnimateTimeIncrement);
                TransY_Animate_Unit = transYDiff / (AnimateTime / AnimateTimeIncrement);

                timerBusy = true;
                IsMoveToMapAreaAnimate = true;

                View_Animate = new MapArea
                {
                    ScaleX = MoveToMapArea.ScaleX,
                    ScaleY = MoveToMapArea.ScaleY,
                    TransX = MoveToMapArea.TransX,
                    TransY = MoveToMapArea.TransY
                };

                stopwatch.Start();

                Device.StartTimer(TimeSpan.FromMilliseconds(DefaultAnimateTimeIncrement), () =>
               {
                   var stillBusy = false;

                   TimeElapsed = (float)(stopwatch.Elapsed.TotalMilliseconds % CycleTime / CycleTime);

                   View_Animate.ScaleX -= ScaleX_Animate_Unit;
                   View_Animate.ScaleY -= ScaleY_Animate_Unit;
                   View_Animate.TransX -= TransX_Animate_Unit;
                   View_Animate.TransY -= TransY_Animate_Unit;

                   //Scale X
                   if ((MoveToMapArea.ScaleX > View_Animate_Dest.ScaleX && View_Animate.ScaleX <= View_Animate_Dest.ScaleX) ||
                      (MoveToMapArea.ScaleX < View_Animate_Dest.ScaleX && View_Animate.ScaleX >= View_Animate_Dest.ScaleX))
                   {
                       View_Animate.ScaleX = View_Animate_Dest.ScaleX;
                   }
                   else
                       stillBusy = true;

                   //Scale Y
                   if ((MoveToMapArea.ScaleY > View_Animate_Dest.ScaleY && View_Animate.ScaleY <= View_Animate_Dest.ScaleY) ||
                      (MoveToMapArea.ScaleY < View_Animate_Dest.ScaleY && View_Animate.ScaleY >= View_Animate_Dest.ScaleY))
                   {
                       View_Animate.ScaleY = View_Animate_Dest.ScaleY;
                   }
                   else
                       stillBusy = true;

                   //Trans X
                   if ((MoveToMapArea.TransX > View_Animate_Dest.TransX && View_Animate.TransX <= View_Animate_Dest.TransX) ||
                      (MoveToMapArea.TransX < View_Animate_Dest.TransX && View_Animate.TransX >= View_Animate_Dest.TransX))
                   {
                       View_Animate.TransX = View_Animate_Dest.TransX;
                   }
                   else
                       stillBusy = true;

                   //Trans Y
                   if ((MoveToMapArea.TransY > View_Animate_Dest.TransY && View_Animate.TransY <= View_Animate_Dest.TransY) ||
                      (MoveToMapArea.TransY < View_Animate_Dest.TransY && View_Animate.TransY >= View_Animate_Dest.TransY))
                   {
                       View_Animate.TransY = View_Animate_Dest.TransY;
                   }
                   else
                       stillBusy = true;

                   if (!init && !stillBusy)
                       FlashHilight(canvas);

                   canvas.InvalidateSurface();

                   if (!stillBusy)
                   {
                       timerBusy = false;
                       IsMoveToMapAreaAnimate = false;
                       MoveToMapArea = View_Animate_Dest;
                       View_Animate_Dest = View_Dest;
                       if (init)
                       {
                           Zoom(canvas, false);
                       }
                   }

                   if (!timerBusy)
                   {
                       stopwatch.Stop();
                   }
                   return timerBusy;
               });
            }
            else
            {
                timerBusy = false;
                IsMoveToMapAreaAnimate = false;
                MoveToMapArea = View_Animate_Dest;
                View_Animate_Dest = View_Dest;
                if (init)
                {
                    Zoom(canvas, false);
                }

            }
        }

        static void ZoomSimple(SKCanvasView canvas)
        {
            showHonorCircle = true;
            AnimateTime = DefaultAnimateTime;
            AnimateTimeIncrement = DefaultAnimateTimeIncrement;

            //Calc
            var scaleXDiff = MoveToMapArea.ScaleX - View_Animate_Dest.ScaleX;
            var scaleYDiff = MoveToMapArea.ScaleY - View_Animate_Dest.ScaleY;
            var transXDiff = MoveToMapArea.TransX - View_Animate_Dest.TransX;
            var transYDiff = MoveToMapArea.TransY - View_Animate_Dest.TransY;


            if (scaleXDiff != 0 && scaleYDiff != 0)
            {
                ScaleX_Animate_Unit = scaleXDiff / (AnimateTime / AnimateTimeIncrement);
                ScaleY_Animate_Unit = scaleYDiff / (AnimateTime / AnimateTimeIncrement);
                TransX_Animate_Unit = transXDiff / (AnimateTime / AnimateTimeIncrement);
                TransY_Animate_Unit = transYDiff / (AnimateTime / AnimateTimeIncrement);

                timerBusy = true;
                IsMoveToMapAreaAnimate = true;

                View_Animate = new MapArea
                {
                    ScaleX = MoveToMapArea.ScaleX,
                    ScaleY = MoveToMapArea.ScaleY,
                    TransX = MoveToMapArea.TransX,
                    TransY = MoveToMapArea.TransY
                };

                stopwatch.Start();

                Device.StartTimer(TimeSpan.FromMilliseconds(DefaultAnimateTimeIncrement), () =>
                {
                    var stillBusy = false;

                    TimeElapsed = (float)(stopwatch.Elapsed.TotalMilliseconds % CycleTime / CycleTime);

                    View_Animate.ScaleX -= ScaleX_Animate_Unit;
                    View_Animate.ScaleY -= ScaleY_Animate_Unit;
                    View_Animate.TransX -= TransX_Animate_Unit;
                    View_Animate.TransY -= TransY_Animate_Unit;

                    //Scale X
                    if ((MoveToMapArea.ScaleX > View_Animate_Dest.ScaleX && View_Animate.ScaleX <= View_Animate_Dest.ScaleX) ||
                       (MoveToMapArea.ScaleX < View_Animate_Dest.ScaleX && View_Animate.ScaleX >= View_Animate_Dest.ScaleX))
                    {
                        View_Animate.ScaleX = View_Animate_Dest.ScaleX;
                    }
                    else
                        stillBusy = true;

                    //Scale Y
                    if ((MoveToMapArea.ScaleY > View_Animate_Dest.ScaleY && View_Animate.ScaleY <= View_Animate_Dest.ScaleY) ||
                       (MoveToMapArea.ScaleY < View_Animate_Dest.ScaleY && View_Animate.ScaleY >= View_Animate_Dest.ScaleY))
                    {
                        View_Animate.ScaleY = View_Animate_Dest.ScaleY;
                    }
                    else
                        stillBusy = true;

                    //Trans X
                    if ((MoveToMapArea.TransX > View_Animate_Dest.TransX && View_Animate.TransX <= View_Animate_Dest.TransX) ||
                       (MoveToMapArea.TransX < View_Animate_Dest.TransX && View_Animate.TransX >= View_Animate_Dest.TransX))
                    {
                        View_Animate.TransX = View_Animate_Dest.TransX;
                    }
                    else
                        stillBusy = true;

                    //Trans Y
                    if ((MoveToMapArea.TransY > View_Animate_Dest.TransY && View_Animate.TransY <= View_Animate_Dest.TransY) ||
                       (MoveToMapArea.TransY < View_Animate_Dest.TransY && View_Animate.TransY >= View_Animate_Dest.TransY))
                    {
                        View_Animate.TransY = View_Animate_Dest.TransY;
                    }
                    else
                        stillBusy = true;

                    if (!stillBusy)
                        FlashHilight(canvas);

                    canvas.InvalidateSurface();

                    if (!stillBusy)
                    {
                        timerBusy = false;
                        IsMoveToMapAreaAnimate = false;
                    }

                    if (!timerBusy)
                    {
                        stopwatch.Stop();
                    }
                    return timerBusy;
                });
            }
        }

        //public static void GetZoomToAnimationFromBrickCode(string code, SKCanvasView canvas)
        //{
        //    switch (code)
        //    {
        //        case "H":
        //            AnimateZoomTo(View_HR_Fit, canvas);
        //            break;
        //        case "O":
        //            AnimateZoomTo(View_Full_Fit, canvas);
        //            break;
        //        case "G":
        //            AnimateZoomTo(West_G, canvas);
        //            break;
        //        case "F":
        //            AnimateZoomTo(West_F, canvas);
        //            break;
        //        case "E":
        //            AnimateZoomTo(West_E, canvas);
        //            break;
        //        case "D":
        //            AnimateZoomTo(West_D, canvas);
        //            break;
        //        case "A":
        //            AnimateZoomTo(East_A, canvas);
        //            break;
        //        case "B":
        //            AnimateZoomTo(East_B, canvas);
        //            break;
        //        case "C":
        //            AnimateZoomTo(East_C, canvas);
        //            break;
        //        case "X":
        //            AnimateZoomTo(East_X, canvas);
        //            break;
        //    }
        //}


        public static void GetZoomToAnimationFromBrickCode(Brick brick, SKCanvasView canvas, bool FullZoomOut = false)
        {
            NotFirstClear = false;

            var locationArea = brick.Location;
            var section = brick.Section;

            if (FullZoomOut)
            {
                locationArea = "O";
            }
            else if (brick.Section == "Honor")
            {
                locationArea = "H";
            }
            else
            {
                locationArea = brick.Section.ToUpper() + locationArea;
            }

            MapArea sectionToFlash = null;

            switch (locationArea.ToUpper())
            {
                case "H":
                    AnimateZoomTo(View_HR_Fit, canvas);
                    sectionToFlash = View_HR_Fit;
                    break;
                case "O":
                    AnimateZoomTo(View_Full_Fit, canvas);
                    break;
                case "WESTD":
                    AnimateZoomTo(West_D, canvas);
                    break;
                case "WESTC":
                    AnimateZoomTo(West_C, canvas);
                    break;
                case "WESTB":
                    AnimateZoomTo(West_B, canvas);
                    break;
                case "WESTA":
                    AnimateZoomTo(West_A, canvas);
                    break;
                case "EASTA":
                    AnimateZoomTo(East_A, canvas);
                    // FlashHilight(canvas, East_A);
                    // sectionToFlash = East_A;
                    break;
                case "EASTB":
                    AnimateZoomTo(East_B, canvas);
                    break;
                case "EASTC":
                    AnimateZoomTo(East_C, canvas);
                    break;
                case "EASTD":
                    AnimateZoomTo(East_D, canvas);
                    break;
            }

          /*  if (sectionToFlash != null)
            {
                FlashHilight(canvas, sectionToFlash);
            }*/
        }

        public static void GetZoomToAnimationSimpleFromBrickCode(Brick brick, SKCanvasView canvas, bool FullZoomOut = false)
        {
            var locationArea = brick.Location;

            if (FullZoomOut)
            {
                locationArea = "O";
            }
            else if (brick.Section == "Honor")
            {
                locationArea = "H";
            }
            else
            {
                locationArea = brick.Section.ToUpper() + locationArea;
            }

            switch (locationArea.ToUpper())
            {
                case "H":
                    AnimateZoomToSimple(View_HR_Fit, canvas);
                    break;
                case "O":
                    AnimateZoomToSimple(View_Full_Fit, canvas);
                    break;
                case "WESTD":
                    AnimateZoomToSimple(West_D, canvas);
                    break;
                case "WESTC":
                    AnimateZoomToSimple(West_C, canvas);
                    break;
                case "WESTB":
                    AnimateZoomToSimple(West_B, canvas);
                    break;
                case "WESTA":
                    AnimateZoomToSimple(West_A, canvas);
                    break;
                case "EASTA":
                    AnimateZoomToSimple(East_A, canvas);
                    break;
                case "EASTB":
                    AnimateZoomToSimple(East_B, canvas);
                    break;
                case "EASTC":
                    AnimateZoomToSimple(East_C, canvas);
                    break;
                case "EASTD":
                    AnimateZoomToSimple(East_D, canvas);
                    break;
            }
        }

        public static async void NoZoomRepaint(Brick brick, SKCanvasView canvas)
        {
            NotFirstClear = false;

            var locationArea = brick.Location;

            if (brick.Section == "Honor")
            {
                locationArea = "H";
            }

            IsMoveToMapArea = true;
            canvas.InvalidateSurface();

            if (locationArea == "H")
            {
                await Task.Delay(500);
                FlashHilight(canvas);
            }
        }


        public static void GetZoomToDirectFromBrickCode(Brick brick, SKCanvasView canvas, bool FullZoomOut = false, bool DefaultNoZoom = false)
        {
            NotFirstClear = false;

            var locationArea = brick.Location;

            if (DefaultNoZoom)
            {
                locationArea = "DEFAULT";
            }
            else if (FullZoomOut)
            {
                locationArea = "O";
            }
            else if (brick.Section == "Honor")
            {
                locationArea = "H";
            }
            else
            {
                locationArea = brick.Section.ToUpper() + locationArea;
            }

            string flashSection = string.Empty;

            switch (locationArea.ToUpper())
            {
                case "DEFAULT":
                    MoveToMapArea = View_Default;
                    break;
                case "H":
                    MoveToMapArea = View_HR_Fit;
                    break;
                case "O":
                    MoveToMapArea = View_Full_Fit;
                    break;
                case "WESTD":
                    MoveToMapArea = West_D;
                    flashSection = "WESTD";
                    break;
                case "WESTC":
                    MoveToMapArea = West_C;
                    flashSection = "WESTC";
                    break;
                case "WESTB":
                    MoveToMapArea = West_B;
                    flashSection = "WESTB";
                    break;
                case "WESTA":
                    MoveToMapArea = West_A;
                    flashSection = "WESTA";
                    break;
                case "EASTA":
                    MoveToMapArea = East_A;
                    flashSection = "EASTA";
                    break;
                case "EASTB":
                    MoveToMapArea = East_B;
                    flashSection = "EASTB";
                    break;
                case "EASTC":
                    MoveToMapArea = East_C;
                    flashSection = "EASTC";
                    break;
                case "EASTD":
                    MoveToMapArea = East_D;
                    flashSection = "EASTD";
                    break;

            }

            IsMoveToMapArea = true;
            canvas.InvalidateSurface();

            if (locationArea == "H")
                FlashHilight(canvas);

            
            /*    if (!string.IsNullOrEmpty(flashSection))
                    FlashHilight(canvas, flashSection);*/
        }

        public static MapArea View_Default
        {
            get
            {
                var area = new MapArea
                {
                    ScaleX = 1f,
                    ScaleY = 1f,
                    TransX = 0f,
                    TransY = 0f
                };

                if (UseNonDefaultBitmap)
                {
                    area.ScaleX = 1f * BigToSmallSizeFactor;
                    area.ScaleY = 1f * BigToSmallSizeFactor;
                    area.TransX = 0f;
                    area.TransY = 0f;
                }

                return area;
            }
        }

        public static MapArea View_HR_Fit
        {
            get
            {
                var area = new MapArea
                {
                    ScaleX = 0.72f * Canvas_ScaleX_Factor,
                    ScaleY = 0.72f * Canvas_ScaleY_Factor,
                    TransX = -801f * Canvas_ScaleX_Factor, //-801f
                    TransY = 218f * Canvas_ScaleY_Factor //218f
                };

                if (UseNonDefaultBitmap)
                {
                    area.ScaleX = (float)(0.72f * Canvas_ScaleX_Factor * BigToSmallSizeFactor);
                    area.ScaleY = (float)(0.72f * Canvas_ScaleY_Factor * BigToSmallSizeFactor);
                    area.TransX = (float)(-801f * Canvas_ScaleX_Factor);
                    area.TransY = (float)(218f * Canvas_ScaleY_Factor);
                }

                return area;
            }
        }

        public static MapArea View_Full_Fit
        {
            get
            {
                var area = new MapArea
                {
                    ScaleX = 0.28f * Canvas_ScaleX_Factor,
                    ScaleY = 0.28f * Canvas_ScaleY_Factor,
                    TransX = 5f * Canvas_ScaleX_Factor, //15f
                    TransY = 555f * Canvas_ScaleY_Factor
                };

                if (UseNonDefaultBitmap)
                {
                    area.ScaleX = (float)(0.28f * Canvas_ScaleX_Factor * BigToSmallSizeFactor);
                    area.ScaleY = (float)(0.28f * Canvas_ScaleY_Factor * BigToSmallSizeFactor);
                    area.TransX = (float)(5f * Canvas_ScaleX_Factor);
                    area.TransY = (float)(555f * Canvas_ScaleY_Factor);
                }

                return area;
            }
        }

        public static MapArea West_D
        {
            get
            {
                var area = new MapArea
                {
                    ScaleX = 2.44f * Canvas_ScaleX_Factor * BigToSmallSizeFactor,
                    ScaleY = 2.44f * Canvas_ScaleY_Factor * BigToSmallSizeFactor,
                    TransX = -684f * Canvas_ScaleX_Factor, // 32
                    TransY = -1063f * Canvas_ScaleY_Factor // -2042
                };
                return area;
            }
        }

        public static MapArea West_C
        {
            get
            {
                var area = new MapArea
                {
                    ScaleX = 2.44f * Canvas_ScaleX_Factor * BigToSmallSizeFactor,
                    ScaleY = 2.44f * Canvas_ScaleY_Factor * BigToSmallSizeFactor,
                    TransX = -1184f * Canvas_ScaleX_Factor, // -684
                    TransY = -1127f * Canvas_ScaleY_Factor // -2097
                };
                return area;
            }
        }

        public static MapArea West_B
        {
            get
            {
                var area = new MapArea
                {
                    ScaleX = 2.44f * Canvas_ScaleX_Factor * BigToSmallSizeFactor,
                    ScaleY = 2.44f * Canvas_ScaleY_Factor * BigToSmallSizeFactor,
                    TransX = -1629f * Canvas_ScaleX_Factor, // -1469
                    TransY = -1228f * Canvas_ScaleY_Factor // -2229
                };
                return area;
            }
        }

        public static MapArea West_A
        {
            get
            {
                var area = new MapArea
                {
                    ScaleX = 2.44f * Canvas_ScaleX_Factor * BigToSmallSizeFactor,
                    ScaleY = 2.44f * Canvas_ScaleY_Factor * BigToSmallSizeFactor,
                    TransX = -2124f * Canvas_ScaleX_Factor, // -2204
                    TransY = -1253f * Canvas_ScaleY_Factor // -2253
                };
                return area;
            }
        }

        public static MapArea East_A
        {
            get
            {
                var area = new MapArea
                {
                    ScaleX = 2.44f * Canvas_ScaleX_Factor * BigToSmallSizeFactor,
                    ScaleY = 2.44f * Canvas_ScaleY_Factor * BigToSmallSizeFactor,
                    TransX = -4757f * Canvas_ScaleX_Factor, // -6451
                    TransY = -1238f * Canvas_ScaleY_Factor // -2084
                };
                return area;
            }
        }

        public static MapArea East_B
        {
            get
            {
                var area = new MapArea
                {
                    ScaleX = 2.44f * Canvas_ScaleX_Factor * BigToSmallSizeFactor,
                    ScaleY = 2.44f * Canvas_ScaleY_Factor * BigToSmallSizeFactor,
                    TransX = -5263f * Canvas_ScaleX_Factor, // -7217
                    TransY = -1218f * Canvas_ScaleY_Factor  // -2078
                };
                return area;
            }
        }

        public static MapArea East_C
        {
            get
            {
                var area = new MapArea
                {
                    ScaleX = 2.44f * Canvas_ScaleX_Factor * BigToSmallSizeFactor,
                    ScaleY = 2.44f * Canvas_ScaleY_Factor * BigToSmallSizeFactor,
                    TransX = -5787f * Canvas_ScaleX_Factor, // -7944
                    TransY = -1218f * Canvas_ScaleY_Factor // -2024
                };
                return area;
            }
        }

        public static MapArea East_D
        {
            get
            {
                var area = new MapArea
                {
                    ScaleX = 2.44f * Canvas_ScaleX_Factor * BigToSmallSizeFactor,
                    ScaleY = 2.44f * Canvas_ScaleY_Factor * BigToSmallSizeFactor,
                    TransX = -6277f * Canvas_ScaleX_Factor, // -8712
                    TransY = -1208f * Canvas_ScaleY_Factor // -1927
                };
                return area;
            }
        }


        /*public static void FlashHilight(SKCanvasView canvas, MapArea sectionToFlash)
        {
            flashBrickOn = true;
            skipBrickCounter = 0;
            IsMoveToMapAreaAnimate = true;

            var animateX = flashBrickLoop;
            var animatex = 0;

            stopwatch.Start();

            Device.StartTimer(TimeSpan.FromMilliseconds(DefaultAnimateTimeIncrement), () =>
            {
                flashBrickOn = true;
                var stillBusy = false;
                timerBusy = true;
                IsMoveToMapAreaAnimate = true;

                TimeElapsed = (float)(stopwatch.Elapsed.TotalMilliseconds % CycleTime / CycleTime);

                if (animatex == animateX)
                {
                    stillBusy = false;
                    flashBrickOn = false;
                }
                else
                    stillBusy = true;

                animatex++;

                canvas.InvalidateSurface();

                if (!stillBusy)
                {
                    timerBusy = false;
                    IsMoveToMapAreaAnimate = false;
                    timerBusy = false;
                }

                if (!timerBusy)
                {
                    stopwatch.Stop();
                }
                return timerBusy;
            });
        }

*/
        /* public static void FlashHilight(SKCanvasView canvas, string sectionToFlash)
         {
             flashBrickOn = true;
             skipBrickCounter = 0;
             IsMoveToMapAreaAnimate = true;

             var animateX = flashBrickLoop;
             var animatex = 0;

             stopwatch.Start();

             Device.StartTimer(TimeSpan.FromMilliseconds(DefaultAnimateTimeIncrement), () =>
             {
                 flashBrickOn = true;
                 var stillBusy = false;
                 timerBusy = true;
                 IsMoveToMapAreaAnimate = true;

                 TimeElapsed = (float)(stopwatch.Elapsed.TotalMilliseconds % CycleTime / CycleTime);

                 if (animatex == animateX)
                 {
                     stillBusy = false;
                     flashBrickOn = false;
                 }
                 else
                     stillBusy = true;

                 animatex++;

                 canvas.InvalidateSurface();

                 if (!stillBusy)
                 {
                     timerBusy = false;
                     IsMoveToMapAreaAnimate = false;
                     timerBusy = false;
                 }

                 if (!timerBusy)
                 {
                     stopwatch.Stop();
                 }
                 return timerBusy;
             });
         }

 */
        public static void CreateHonorRollBrickGrid(SKCanvas canvas, SKMatrix matrix, bool wingBricks = false)
        {
            var bricksInRowDivide = 36; //34
            var maxLayerCount = 39; //MM   48; //VV

            if (wingBricks)
            {
                bricksInRowDivide = 7;
                maxLayerCount = 14;
            }

            float areaWidth = Brick_HR_Width * matrix.ScaleX;
            float areaHeight = Brick_HR_Height * matrix.ScaleY;
            float startFrom = Brick_HR_StartLeft + Brick_ScaleXOffset;// + matrix.TransX;
            float startFromY = Brick_HR_StartTop;// + matrix.TransY;

            var modelWidth = areaWidth / bricksInRowDivide;
            var modelHeight = areaHeight / maxLayerCount;

            GenerateAlphabetList();

            var brickPalate = new BrickPalate
            {
                MaxLayerCount = maxLayerCount,
                StartFromY = startFromY,
                LayerNumber = 0,
                LayerWidth = areaWidth,
                WallHeight = areaHeight,
                StartFrom = startFrom,
                Layers = new SKPath(),
                //model = CreateRect(new SKPoint(0, 0), 30, 70),
                model = CreateRect(new SKPoint(0, 0), modelHeight, modelWidth),
                BrickText = $"Rocky: ",
                Bricktionary = new List<BrickDetail>(),
                IsFirstLayer = true
            };

            //Will not be compatible since will be at an angle -- Need to skew it
            var gridOutline = new SKRect(startFrom, startFromY, areaWidth, startFromY + areaHeight);
            //canvas.DrawRect(gridOutline, paintGridBackground);

            //canvas.RotateDegrees((float)rotateSlider.Value, info.Width / 2, info.Height / 2);

            canvas.Save();
            canvas.Restore();

            var brickCodeSelected = "";
            LayBricksWithLines(canvas, brickPalate, brickCodeSelected);

            canvas.DrawPath(brickPalate.Layers, paintLine);
        }

        public static void LayBricksWithLines(SKCanvas canvas, BrickPalate palate, string searchBrickCode)
        {
            bool StartFullBrick = palate.LayerNumber % 2 == 0, isFirstBrick = true; float currentBricksWidth = 0; bool skipText = false;
            int brickCount = 1;

            if (palate.IsFirstLayer)
            {
                palate.NextLayerTop += palate.StartFromY;
                palate.IsFirstLayer = false;
            }

            canvas.DrawLine(palate.StartFrom, palate.NextLayerTop, palate.LayerWidth, palate.NextLayerTop, paintLine);

            currentBricksWidth = palate.StartFrom;

            while ((currentBricksWidth + palate.model.Width) - palate.StartFrom < palate.LayerWidth)
            {
                var brickWidth = palate.model.Width;
                skipText = false;
                if (isFirstBrick && !StartFullBrick)
                {
                    brickWidth = palate.model.Width / 2;
                    isFirstBrick = false;
                    skipText = true;
                }

                //var nextBrick = CreateRect(new SKPoint(currentBricksWidth, palate.NextLayerTop), palate.model.Height + palate.NextLayerTop, brickWidth);
                canvas.DrawLine(currentBricksWidth, palate.NextLayerTop, currentBricksWidth, palate.NextLayerTop/* + palate.model.Height*/, paintLine);

                palate.TotalBrickCount++;

                if (!skipText)
                {
                    var loc = AlphaList[palate.MaxLayerCount - palate.LayerNumber - 1];
                    var pos = brickCount;

                    //var brickText = $"{palate.BrickText}{loc}-{brickCount.ToString()}";
                    var brickText = $"{loc}-{brickCount.ToString()}";
                    // Adjust TextSize property so text is 90% of screen width
                    float textWidth = textPaint.MeasureText(brickText + "brick");
                    textPaint.TextSize = .8f * brickWidth * textPaint.TextSize / textWidth;
                    textWidth = textPaint.MeasureText(brickText);
                    var textX = currentBricksWidth + (brickWidth / 2 - textWidth / 2);
                    var textY = (float)(palate.NextLayerTop + palate.model.Height / 2);


                    if (!string.IsNullOrEmpty(searchBrickCode))
                    {
                        var coords = searchBrickCode.Split('-');
                        if (null != coords && coords.Length == 2 && coords[0].ToUpper() == loc.ToUpper() && coords[1] == pos.ToString())
                        {

                            //palate.Layers.AddCircle(currentBricksWidth, palate.NextLayerTop, 50);
                            canvas.DrawCircle(currentBricksWidth, palate.NextLayerTop, 50, paintHiLiteLine);
                            canvas.Restore();
                            canvas.Save();

                            var hilightBrick = new SKRect(currentBricksWidth, palate.NextLayerTop, currentBricksWidth + brickWidth, /*palate.model.Height +*/ palate.NextLayerTop);
                            canvas.DrawRect(hilightBrick, paintHiLite);
                            //canvas.Save();
                            canvas.Restore();

                            paintLine.StrokeWidth = 0.5f;
                            canvas.DrawText(brickText, textX - 5, textY + 5, paintLine);
                            //canvas.Save();
                            canvas.Restore();
                        }
                        else
                            canvas.DrawText(brickText, textX, textY, textPaint);
                    }
                    else
                        canvas.DrawText(brickText, textX, textY, textPaint);
                    //canvas.Save();

                    palate.Bricktionary.Add(new BrickDetail { Name = palate.BrickText, location = loc, position = pos });
                    brickCount++;
                }

                currentBricksWidth += brickWidth;
            }

            canvas.DrawLine(currentBricksWidth, palate.NextLayerTop, currentBricksWidth, palate.NextLayerTop/* + palate.model.Height*/, paintLine);

            palate.LayerNumber++; palate.NextLayerTop += palate.model.Height; canvas.Restore();

            //if (palate.WallHeight > palate.NextLayerTop + palate.model.Height && palate.LayerNumber < palate.MaxLayerCount)
            if (palate.LayerNumber < palate.MaxLayerCount)
                LayBricksWithLines(canvas, palate, searchBrickCode);
            else
                canvas.DrawLine(palate.StartFrom, palate.NextLayerTop, palate.LayerWidth, palate.NextLayerTop, paintLine);

        }
    }
}
