using System;
using System.Collections.Generic;
//using SkiaSharp.Views.Forms;
//using Xamarin.Forms;
using SkiaSharp.Views.Maui;
using SkiaSharp;
using System.Linq;
using System.Reflection;
using System.IO;
using MasonVeteransMemorial.Transforms;
using TouchTracking;
using MasonVeteransMemorial.Controls.SkiaSharp;
using static MasonVeteransMemorial.Controls.SkiaSharp.SkiaSharpCommon;
using MasonVeteransMemorial.Data;
using System.Diagnostics;
using MasonVeteransMemorial.Models;
using SkiaSharp.Views.Maui.Controls;

namespace MasonVeteransMemorial.Pages
{
    public partial class SkiaToolsPage : ContentPage
    {
        double pageWidth = 0;
        double pageHeight = 0;

        SKBitmap bitmap;
        TouchManipulationBitmap tBitmap;
        TouchManipulationCanvas tCanvas;

        CanvasDimension canvases;
        CanvasType canvasType = CanvasType.Image;

        MatrixDisplay matrixDisplay = new MatrixDisplay();
        MatrixDisplay matrixCanvasDisplay = new MatrixDisplay();
        List<long> touchIds = new List<long>();
        List<long> touchCanvasIds = new List<long>();

        bool lockScale = false;

        bool modeIs3DControl = false;

        bool IS_HONOR_ROLL_IMAGE = true;

        bool isClearAll = false;
        bool isClear3D = false;
        bool isClear3DCanvas = false;
        bool isSyncGrid = false;
        bool isUseFingerPaint = false;

        Dictionary<long, SKPath> inProgressPaths = new Dictionary<long, SKPath>();
        List<SKPath> completedPaths = new List<SKPath>();

        SKPaint paintFinger = new SKPaint
        {
            Style = SKPaintStyle.Stroke,
            Color = SKColors.Blue,
            StrokeWidth = 10,
            StrokeCap = SKStrokeCap.Round,
            StrokeJoin = SKStrokeJoin.Round
        };


        SKPaint paint = new SKPaint
        {
            Style = SKPaintStyle.Stroke,
            Color = Colors.Red.ToSKColor(),
            StrokeWidth = 25
        };

        SKPaint paintLine = new SKPaint
        {
            Style = SKPaintStyle.Stroke,
            Color = Colors.Black.ToSKColor(),
            //IsAntialias = true,
            StrokeWidth = 1,
            //StrokeJoin = SKStrokeJoin.Miter,
            //StrokeCap = SKStrokeCap.Square,
            //IsStroke = true
        };

        SKPaint paintHiLite = new SKPaint
        {
            Style = SKPaintStyle.Fill,
            Color = Colors.Yellow.ToSKColor(),
            IsAntialias = true,
        };

        SKPaint paintGridBackground = new SKPaint
        {
            Style = SKPaintStyle.Fill,
            Color = SKColors.Transparent.FromHex("#be5959"), //8e3f3f //be5959
            //IsAntialias = true,
        };


        SKPaint paintHiLiteLine = new SKPaint
        {
            Style = SKPaintStyle.Stroke,
            Color = Colors.White.ToSKColor(),
            StrokeWidth = 5,
            //IsAntialias = true,
        };

        SKPaint paintHiLiteRed = new SKPaint
        {
            Style = SKPaintStyle.Fill,
            Color = Colors.Red.ToSKColor(),
            //IsAntialias = true,
        };

        // Create an SKPaint object to display the text
        SKPaint textPaint = new SKPaint
        {
            //Style = SKPaintStyle.Stroke,
            Color = SKColors.White,
            StrokeWidth = .5f
            //IsAntialias = true,
        };

        SKPaint textYellowPaint = new SKPaint
        {
            //Style = SKPaintStyle.Stroke,
            Color = SKColors.Yellow,
            StrokeWidth = .5f
            //IsAntialias = true,
        };

        private List<string> AlphaList = null;

        public SkiaToolsPage()
        {
            InitializeComponent();

            var path = "MasonVeteransMemorial.Resources.Images.drone_pu_4_overhead_view.png";

            if (!IS_HONOR_ROLL_IMAGE)
                path = "MasonVeteransMemorial.Resources.Images.drone_pu_4_overhead_view.png";

            var assembly = IntrospectionExtensions.GetTypeInfo(typeof(SkiaToolsPage)).Assembly;
            Stream stream = assembly.GetManifestResourceStream(path);

            Stream stream2 = assembly.GetManifestResourceStream(path);

            //string resourceID = "SkiaSharpTemplate.Media.H_smaller_rotated.png";
            //Assembly assembly = GetType().GetTypeInfo().Assembly;

            //using (Stream stream = assembly.GetManifestResourceStream(resourceID))
            using (SKManagedStream skStream = new SKManagedStream(stream))
            {
                //Standard Bitmap
                bitmap = SKBitmap.Decode(skStream);
            }

            using (SKManagedStream skStream2 = new SKManagedStream(stream2))
            {
                //Touch Bitmap
                SKBitmap bitmapProcess = SKBitmap.Decode(skStream2);
                this.tBitmap = new TouchManipulationBitmap(bitmapProcess);
                this.tBitmap.TouchManager.Mode = TouchManipulationMode.ScaleRotate;
            }

            Title = "Tools";
            // Icon = "ic_map";
            btnImgSelect.BackgroundColor = Colors.Lime;

            canvases = new CanvasDimension
            {
                Image = CreateDimensionDetails(),
                Map = CreateDimensionDetails(),
                TImage = CreateDimensionDetails()
            };

            btnShowHideMap.Clicked += (obj, args) =>
            {
                canvasLayoutMap.IsVisible = !canvasLayoutMap.IsVisible;

                if (canvasLayoutMap.IsVisible)
                {
                    btnShowHideMap.Text = "xMap";
                    btnShowHideMap.TextColor = Colors.Red;
                }
                else
                {
                    btnShowHideMap.Text = "!Map";
                    btnShowHideMap.TextColor = Colors.Green;
                }
            };

            btnShowHideImage.Clicked += (obj, args) =>
            {
                canvasLayout.IsVisible = !canvasLayout.IsVisible;

                if (canvasLayout.IsVisible)
                {
                    btnShowHideImage.Text = "xImg";
                    btnShowHideImage.TextColor = Colors.Red;
                }
                else
                {
                    btnShowHideImage.Text = "!Img";
                    btnShowHideImage.TextColor = Colors.Green;
                }
            };

            btnShowHideTImage.Clicked += (obj, args) =>
            {
                canvasLayoutTImage.IsVisible = !canvasLayoutTImage.IsVisible;

                if (canvasLayoutTImage.IsVisible)
                {
                    btnShowHideTImage.Text = "xTImg";
                    btnShowHideTImage.TextColor = Colors.Red;
                }
                else
                {
                    btnShowHideTImage.Text = "!TImg";
                    btnShowHideTImage.TextColor = Colors.Green;
                }
            };

            btnShowHideTCanvas.Clicked += (obj, args) =>
            {
                canvasLayoutTCanvas.IsVisible = !canvasLayoutTCanvas.IsVisible;

                if (canvasLayoutTCanvas.IsVisible)
                {
                    btnShowHideTCanvas.Text = "xTCvs";
                    btnShowHideTCanvas.TextColor = Colors.Red;
                }
                else
                {
                    btnShowHideTCanvas.Text = "!TCvs";
                    btnShowHideTCanvas.TextColor = Colors.Green;
                }
            };

            btnEnableFingerToggle.Clicked += (obj, args) =>
            {
                isUseFingerPaint = !isUseFingerPaint;

                if (!isUseFingerPaint)
                {
                    btnEnableFingerToggle.Text = "!Fing";
                    btnEnableFingerToggle.TextColor = Colors.Green;
                }
                else
                {
                    btnEnableFingerToggle.Text = "xFing";
                    btnEnableFingerToggle.TextColor = Colors.Red;
                }
            };


            btnClearAll.Clicked += (sender, e) =>
            {
                isClearAll = true;
                skCanvas.InvalidateSurface();
            };

            btnClear3DSkew.Clicked += (sender, e) =>
            {
                isClear3D = true;
                skCanvasTImage.InvalidateSurface();
            };

            btnLockScale.Clicked += (obj, args) =>
            {
                lockScale = !lockScale;

                btnLockScale.Text = lockScale ? "Unlock" : "Lock";
            };

            btnBackToStandardControls.Clicked += (obj, args) =>
            {
                //if (!modeIs3DControl)
                //{
                rotation3DGrid.IsVisible = false;
                controllerLayout.IsVisible = true;
                modeIs3DControl = false;
                //}
            };

            btnShow3DGrid.Clicked += (obj, args) =>
            {
                rotation3DGrid.IsVisible = true;

                controllerLayout.IsVisible = false;

                modeIs3DControl = true;
            };

            btnWriteToConsole.Clicked += (sender, e) =>
            {
                WritePointsToConsole();
            };

            if (modeIs3DControl)
            {
                rotation3DGrid.IsVisible = true;
                controllerLayout.IsVisible = false;
            }
            else
            {
                rotation3DGrid.IsVisible = false;
                controllerLayout.IsVisible = true;
            }



            InitializeResets();
        }

        protected DimensionDetails CreateDimensionDetails()
        {
            var ds = new DimensionDetails();

            //ds.Add(new DimensionDetail { Type = DimensionType.Scale_XY, Value = 100 });
            ds.Add(new DimensionDetail { Type = DimensionType.Scale_X, Value = 100 });
            ds.Add(new DimensionDetail { Type = DimensionType.Scale_Y, Value = 100 });
            ds.Add(new DimensionDetail { Type = DimensionType.Scale_XY, Value = 100 });
            ds.Add(new DimensionDetail { Type = DimensionType.PerspectiveH, Value = 0 });
            ds.Add(new DimensionDetail { Type = DimensionType.PerspectiveV, Value = 0 });
            ds.Add(new DimensionDetail { Type = DimensionType.PanH, Value = 0 });
            ds.Add(new DimensionDetail { Type = DimensionType.PanV, Value = 0 });
            ds.Add(new DimensionDetail { Type = DimensionType.Rotate_Deg, Value = 0 });
            return ds;
        }

        protected void OnTouchModePickerSelectedIndexChanged(object sender, EventArgs args)
        {
            if (bitmap != null)
            {
                Picker picker = (Picker)sender;
                TouchManipulationMode mode;
                Enum.TryParse(picker.Items[picker.SelectedIndex], out mode);
                tBitmap.TouchManager.Mode = mode;
            }
        }

        SKPoint ConvertToPixel(Point pt)
        {
            return new SKPoint((float)(skCanvas.CanvasSize.Width * pt.X / skCanvas.Width),
                               (float)(skCanvas.CanvasSize.Height * pt.Y / skCanvas.Height));
        }

        SKPoint ConvertToPixelCustom(Point pt, SKCanvasView canvas)
        {
            return new SKPoint((float)(canvas.CanvasSize.Width * pt.X / canvas.Width),
                               (float)(canvas.CanvasSize.Height * pt.Y / canvas.Height));
        }

        bool isFirstDrawPoint = true;
        SKPoint firstDrawPoint;

        void OnTouchEffectActionFinger(object sender, TouchActionEventArgs args)
        {
            switch (args.Type)
            {
                case TouchActionType.Pressed:
                    if (!inProgressPaths.ContainsKey(args.Id))
                    {
                        SKPath path = new SKPath();
                        var point = ConvertToPixel(args.Location);
                        path.MoveTo(point);
                        inProgressPaths.Add(args.Id, path);
                        skCanvas.InvalidateSurface();

                        if (isFirstDrawPoint)
                        {
                            firstDrawPoint = point;
                            isFirstDrawPoint = false;
                        }
                    }
                    break;

                case TouchActionType.Moved:
                    if (inProgressPaths.ContainsKey(args.Id))
                    {
                        SKPath path = inProgressPaths[args.Id];
                        path.LineTo(ConvertToPixel(args.Location));
                        skCanvas.InvalidateSurface();
                    }
                    break;

                case TouchActionType.Released:
                    if (inProgressPaths.ContainsKey(args.Id))
                    {
                        completedPaths.Add(inProgressPaths[args.Id]);
                        inProgressPaths.Remove(args.Id);
                        skCanvas.InvalidateSurface();
                    }
                    break;

                case TouchActionType.Cancelled:
                    if (inProgressPaths.ContainsKey(args.Id))
                    {
                        inProgressPaths.Remove(args.Id);
                        skCanvas.InvalidateSurface();
                    }
                    break;
            }
        }

        protected void OnTouchEffectAction(object sender, TouchActionEventArgs args)
        {
            if (!isUseFingerPaint)
            {
                // Convert Xamarin.Forms point to pixels
                Point pt = args.Location;
                SKPoint point =
                    new SKPoint((float)(skCanvasTImage.CanvasSize.Width * pt.X / skCanvasTImage.Width),
                                (float)(skCanvasTImage.CanvasSize.Height * pt.Y / skCanvasTImage.Height));

                switch (args.Type)
                {
                    case TouchActionType.Pressed:
                        if (tBitmap.HitTest(point))
                        {
                            touchIds.Add(args.Id);
                            tBitmap.ProcessTouchEvent(args.Id, args.Type, point);
                            break;
                        }
                        break;

                    case TouchActionType.Moved:
                        if (touchIds.Contains(args.Id))
                        {
                            tBitmap.ProcessTouchEvent(args.Id, args.Type, point);
                            skCanvasTImage.InvalidateSurface();
                        }
                        break;

                    case TouchActionType.Released:
                    case TouchActionType.Cancelled:
                        if (touchIds.Contains(args.Id))
                        {
                            tBitmap.ProcessTouchEvent(args.Id, args.Type, point);
                            touchIds.Remove(args.Id);
                            skCanvasTImage.InvalidateSurface();
                        }
                        break;
                }
            }
            else
            {
                switch (args.Type)
                {
                    case TouchActionType.Pressed:
                        if (!inProgressPaths.ContainsKey(args.Id))
                        {
                            SKPath path = new SKPath();
                            var point = ConvertToPixelCustom(args.Location, skCanvasTImage);
                            path.MoveTo(point);
                            inProgressPaths.Add(args.Id, path);
                            skCanvasTImage.InvalidateSurface();

                            if (isFirstDrawPoint)
                            {
                                firstDrawPoint = point;
                                isFirstDrawPoint = false;
                            }
                        }
                        break;

                    case TouchActionType.Moved:
                        if (inProgressPaths.ContainsKey(args.Id))
                        {
                            SKPath path = inProgressPaths[args.Id];
                            path.LineTo(ConvertToPixelCustom(args.Location, skCanvasTImage));
                            skCanvasTImage.InvalidateSurface();
                        }
                        break;

                    case TouchActionType.Released:
                        if (inProgressPaths.ContainsKey(args.Id))
                        {
                            completedPaths.Add(inProgressPaths[args.Id]);
                            inProgressPaths.Remove(args.Id);
                            skCanvasTImage.InvalidateSurface();
                        }
                        break;

                    case TouchActionType.Cancelled:
                        if (inProgressPaths.ContainsKey(args.Id))
                        {
                            inProgressPaths.Remove(args.Id);
                            skCanvasTImage.InvalidateSurface();
                        }
                        break;
                }
            }
        }

        protected void OnTouchCanvasEffectAction(object sender, TouchActionEventArgs args)
        {
            // Convert Xamarin.Forms point to pixels
            Point pt = args.Location;
            SKPoint point =
                new SKPoint((float)(skCanvasTCanvas.CanvasSize.Width * pt.X / skCanvasTCanvas.Width),
                            (float)(skCanvasTCanvas.CanvasSize.Height * pt.Y / skCanvasTCanvas.Height));

            switch (args.Type)
            {
                case TouchActionType.Pressed:
                    if (tCanvas.HitTest(point))
                    {
                        touchCanvasIds.Add(args.Id);
                        tCanvas.ProcessTouchEvent(args.Id, args.Type, point);
                        break;
                    }
                    break;

                case TouchActionType.Moved:
                    if (touchCanvasIds.Contains(args.Id))
                    {
                        tCanvas.ProcessTouchEvent(args.Id, args.Type, point);
                        skCanvasTCanvas.InvalidateSurface();
                    }
                    break;

                case TouchActionType.Released:
                case TouchActionType.Cancelled:
                    if (touchCanvasIds.Contains(args.Id))
                    {
                        tCanvas.ProcessTouchEvent(args.Id, args.Type, point);
                        touchCanvasIds.Remove(args.Id);
                        skCanvasTCanvas.InvalidateSurface();
                    }
                    break;
            }
        }

        void WritePointsToConsole()
        {
            Console.WriteLine("Completed Paths:v");
            foreach (var points in completedPaths)
            {
                Console.WriteLine(points.GetPoint(0).X);
            }
        }

        protected void skCanvasTImage_OnPaintSurface(object sender, SKPaintSurfaceEventArgs args)
        {
            SKImageInfo info = args.Info;
            SKSurface surface = args.Surface;
            SKCanvas canvas = surface.Canvas;

            canvas.Clear();


            // Display the bitmap
            if (MemorialMapAreas.IsMoveToMapAreaAnimate)
            {
                tBitmap.PaintNavigate(canvas, MemorialMapAreas.View_Animate);
            }
            else if (MemorialMapAreas.IsMoveToMapArea)
            {
                tBitmap.PaintNavigate(canvas, MemorialMapAreas.MoveToMapArea);
                MemorialMapAreas.IsMoveToMapArea = false;
            }
            else if (isClear3D)
            {
                isClear3D = false;
                //matrixDisplay = new MatrixDisplay();
                // matrixDisplay.Reset(canvas, tBitmap.Matrix, new SKPoint(0, 0));
                tBitmap.PaintReset(canvas);
                //return;
            }
            else
            {
                tBitmap.PaintSimple(canvas);

                MemorialMapAreas.MoveToMapArea.ScaleX = tBitmap.Matrix.ScaleX;
                MemorialMapAreas.MoveToMapArea.ScaleY = tBitmap.Matrix.ScaleY;
                MemorialMapAreas.MoveToMapArea.TransX = tBitmap.Matrix.TransX;
                MemorialMapAreas.MoveToMapArea.TransY = tBitmap.Matrix.TransY;
                //tBitmap.Paint(canvas);
            }

            // Display the matrix in the lower-right corner
            SKSize matrixSize = matrixDisplay.Measure(tBitmap.Matrix);

            matrixDisplay.Paint(canvas, tBitmap.Matrix,
                new SKPoint(info.Width - matrixSize.Width,
                            info.Height - matrixSize.Height));

            if (isUseFingerPaint)
            {
                foreach (SKPath path in completedPaths)
                {
                    canvas.DrawPath(path, paintFinger);
                    var point = completedPaths.FirstOrDefault().GetPoint(0);
                    drawPointsLabel.Text = $"From: {point.X},{point.Y}";

                }

                foreach (SKPath path in inProgressPaths.Values)
                {
                    canvas.DrawPath(path, paintFinger);
                    var point = inProgressPaths.Values.FirstOrDefault().GetPoint(0);
                    //drawPointsLabel.Text = $"From: {point.X},{point.Y}";
                }
            }

            ////BRICKS
            MemorialMapAreas.CreateHonorRollAngledBrickGrid(canvas, tBitmap.Matrix);


            //isSyncGrid = true;
            //skCanvasGrid.InvalidateSurface();

            //////////////////////////////////////////////////////////////////////

            // Calculate perspective matrix
            //SKMatrix perspectiveMatrix = SKMatrix.MakeIdentity();

            //perspectiveMatrix.Persp0 = tBitmap.Matrix.Persp0 / 100;
            //perspectiveMatrix.Persp1 = -1 * tBitmap.Matrix.Persp1 / 100;

            //// Center of screen
            //float xCenter = info.Width / 2;
            //float yCenter = info.Height / 2;

            //SKMatrix matrix = SKMatrix.MakeTranslation(-xCenter, -yCenter);
            //SKMatrix.PostConcat(ref matrix, perspectiveMatrix);
            //SKMatrix.PostConcat(ref matrix, SKMatrix.MakeTranslation(xCenter, yCenter));

            //// Coordinates to center of canvas to page
            //float x = xCenter - info.Width / 2;
            //float y = yCenter - info.Height / 2;

            //canvas.SetMatrix(matrix);

            //canvas.Scale(tBitmap.Matrix.ScaleX, tBitmap.Matrix.ScaleY);
            //canvas.Translate(tBitmap.Matrix.TransX, 0);
            //canvas.Translate(0, tBitmap.Matrix.TransY);

        }

        protected void skCanvasTCanvas_OnPaintSurface(object sender, SKPaintSurfaceEventArgs args)
        {
            SKImageInfo info = args.Info;
            SKSurface surface = args.Surface;
            SKCanvas canvas = surface.Canvas;

            canvas.Clear();

            if (tCanvas == null)
            {
                tCanvas = new TouchManipulationCanvas(canvas, new SKSize(info.Width, info.Height));
            }

            // Display the bitmap


            if (isClear3DCanvas)
            {
                isClear3DCanvas = false;
                //matrixDisplay = new MatrixDisplay();
                // matrixDisplay.Reset(canvas, tBitmap.Matrix, new SKPoint(0, 0));
                tCanvas.PaintReset(canvas);
                //return;
            }
            else
                tCanvas.Paint(canvas);


            // Display the matrix in the lower-right corner
            SKSize matrixSize = matrixCanvasDisplay.Measure(tCanvas.Matrix);

            matrixCanvasDisplay.Paint(canvas, tCanvas.Matrix,
                                      new SKPoint((float)(pageWidth - matrixSize.Width),
                                        (float)(pageHeight - matrixSize.Height)));

        }


        //***** BITMAP
        protected void skCanvas_OnPaintSurface(object sender, SKPaintSurfaceEventArgs e)
        {
            var info = e.Info;
            var surface = e.Surface;
            var canvas = surface.Canvas;

            canvas.Clear();

            //canvas.Save();
            //canvas.Restore();

            // Calculate perspective matrix
            SKMatrix perspectiveMatrix = SKMatrix.MakeIdentity();

            //perspectiveMatrix.Persp0 = (float)persp0Slider.Value / 100;
            //perspectiveMatrix.Persp1 = (float)persp1Slider.Value / 100;

            perspectiveMatrix.Persp0 = (float)persp0Slider.Value / 100;
            perspectiveMatrix.Persp1 = (float)persp1Slider.Value / 100;
            //perspectiveMatrix.Persp1 = (float)persp5Slider.Value / 100;


            // Center of screen
            float xCenter = info.Width / 2;
            float yCenter = info.Height / 2;

            //Center of Page

            SKMatrix matrix = SKMatrix.MakeTranslation(-xCenter, -yCenter);
            SKMatrix.PostConcat(ref matrix, perspectiveMatrix);
            SKMatrix.PostConcat(ref matrix, SKMatrix.MakeTranslation(xCenter, yCenter));

            // Coordinates to center bitmap on canvas
            float x = xCenter - bitmap.Width / 2;
            float y = yCenter - bitmap.Height / 2;

            //float x = xCenter + info.Width / 2;
            //float y = yCenter + info.Height / 2;

            canvas.SetMatrix(matrix);
            //canvas.DrawBitmap(bitmap, x, y);

            //canvas.Skew(xCenter, yCenter);

            //canvas.Scale((float)(persp2Slider.Value / 100), (float)(persp5Slider.Value / 100));
            //canvas.Scale((float)(persp2Slider.Value / 100));
            canvas.Scale((float)(persp2Slider.Value / 100), (float)(persp5Slider.Value / 100));

            canvas.Translate((float)persp3Slider.Value, 0);

            canvas.Translate(0, (float)persp4Slider.Value);

            canvas.RotateDegrees((float)rotateSlider.Value, info.Width / 2, info.Height / 2);
            ///////////////////////////////////////

            //canvas.DrawCircle(info.Width / 2, info.Height / 2, 100, paint);

            paint.Style = SKPaintStyle.Fill;
            paint.Color = SKColors.DodgerBlue;
            //canvas.DrawCircle(info.Width / 2, info.Height / 2, 100, paint);

            canvas.Restore();
            canvas.Save();
            canvas.Restore();


            //3d rotation feature
            if (modeIs3DControl)
            {

                // Translate center to origin
                SKMatrix matrix3D = SKMatrix.MakeTranslation(-xCenter, -yCenter);

                // Use 3D matrix for 3D rotations and perspective
                SKMatrix44 matrix44 = SKMatrix44.CreateIdentity();
                matrix44.PostConcat(SKMatrix44.CreateRotationDegrees(1, 0, 0, (float)xRotateSlider.Value));
                matrix44.PostConcat(SKMatrix44.CreateRotationDegrees(0, 1, 0, (float)yRotateSlider.Value));
                matrix44.PostConcat(SKMatrix44.CreateRotationDegrees(0, 0, 1, (float)zRotateSlider.Value));

                SKMatrix44 perspectiveMatrix3D = SKMatrix44.CreateIdentity();
                perspectiveMatrix3D[3, 2] = -1 / (float)depthSlider.Value;
                matrix44.PostConcat(perspectiveMatrix3D);

                // Concatenate with 2D matrix
                SKMatrix.PostConcat(ref matrix3D, matrix44.Matrix);

                // Translate back to center
                SKMatrix.PostConcat(ref matrix3D,
                    SKMatrix.MakeTranslation(xCenter, yCenter));

                // Set the matrix and display the bitmap
                canvas.SetMatrix(matrix3D);
                float xBitmap = xCenter - bitmap.Width / 2;
                float yBitmap = yCenter - bitmap.Height / 2;
                canvas.DrawBitmap(bitmap, xBitmap, yBitmap);

            }
            else
            {
                canvas.DrawBitmap(bitmap, x, y);
            }

            if (isClearAll)
            {
                isClearAll = false;
                completedPaths.Clear();
                inProgressPaths.Clear();
                drawPointsLabel.Text = "";
                return;
            }
            else
            {
                foreach (SKPath path in completedPaths)
                {
                    canvas.DrawPath(path, paintFinger);
                    var point = completedPaths.FirstOrDefault().GetPoint(0);
                    drawPointsLabel.Text = $"From: {point.X},{point.Y}";
                }

                foreach (SKPath path in inProgressPaths.Values)
                {
                    canvas.DrawPath(path, paintFinger);
                    var point = inProgressPaths.Values.FirstOrDefault().GetPoint(0);
                    //drawPointsLabel.Text = $"From: {point.X},{point.Y}";
                }
            }

            //DrawLineGrid(canvas, info.Width, info.Height);
        }

        //***** GRID ********//
        protected void skCanvasGrid_OnPaintSurface(object sender, SKPaintSurfaceEventArgs e)
        {
            var info = e.Info;
            var surface = e.Surface;
            var canvas = surface.Canvas;

            canvas.Clear();

            //canvas.Save();
            //canvas.Restore();


            // Calculate perspective matrix
            SKMatrix perspectiveMatrix = SKMatrix.MakeIdentity();

            //perspectiveMatrix.Persp0 = (float)persp0Slider.Value / 100;
            //perspectiveMatrix.Persp1 = (float)persp1Slider.Value / 100;

            perspectiveMatrix.Persp0 = (float)persp0Slider.Value / 100;
            perspectiveMatrix.Persp1 = -1 * (float)persp1Slider.Value / 100;
            //perspectiveMatrix.Persp1 = (float)persp5Slider.Value / 100;


            // Center of screen
            float xCenter = info.Width / 2;
            float yCenter = info.Height / 2;

            //Center of Page
            //float xCenter = (float)(pageWidth / 2);
            //float yCenter = (float)(pageHeight / 2);

            SKMatrix matrix = SKMatrix.MakeTranslation(-xCenter, -yCenter);
            SKMatrix.PostConcat(ref matrix, perspectiveMatrix);
            SKMatrix.PostConcat(ref matrix, SKMatrix.MakeTranslation(xCenter, yCenter));

            // Coordinates to center bitmap on canvas
            //float x = xCenter - bitmap.Width / 2;
            //float y = yCenter - bitmap.Height / 2;

            // Coordinates to center of canvas to page
            float x = xCenter - info.Width / 2;
            float y = yCenter - info.Height / 2;

            //float x = xCenter + info.Width / 2;
            //float y = yCenter + info.Height / 2;

            canvas.SetMatrix(matrix);
            //canvas.DrawBitmap(bitmap, x, y);

            //canvas.Skew(xCenter, yCenter);

            //Scale X,Y
            canvas.Scale((float)(persp2Slider.Value / 100), (float)(persp5Slider.Value / 100));
            //canvas.Scale((float)(persp2Slider.Value / 100), (float)(persp5Slider.Value / 100));

            //Trans X
            canvas.Translate((float)persp3Slider.Value, 0);

            //Trans Y
            canvas.Translate(0, (float)persp4Slider.Value);

            if (isSyncGrid)
            {
                canvas.Scale(tBitmap.Matrix.ScaleX, tBitmap.Matrix.ScaleY);
                canvas.Translate(tBitmap.Matrix.TransX, 0);
                canvas.Translate(0, tBitmap.Matrix.TransY);
                isSyncGrid = false;
            }

            ///////////////////////////////////////

            //canvas.DrawCircle(info.Width / 2, info.Height / 2, 100, paint);

            paint.Style = SKPaintStyle.Fill;
            paint.Color = SKColors.DodgerBlue;
            //canvas.DrawCircle(info.Width / 2, info.Height / 2, 100, paint);

            canvas.Restore();



            var section = "";
            var location = "";
            var position = 0;

            if (!string.IsNullOrEmpty(txtSearch.Text))
            {
                var search = txtSearch.Text.Split('-');

                section = search[0];
                location = search[1];
                position = int.Parse(search[2]);
            }

            var brick = new Brick()
            {
                Section = section,
                Location = location,
                Position = position
            };

            MemorialMapAreas.CreateHonorRollAngledBrickGrid(canvas, matrix, brick);

            //// 7,1182
            //// 859,1182

            //// 281,342
            //// 810,342

            ////Width = 859-7= 852
            ////Height = 1182-342 = 840

            //var bricksInRowDivide = 34;
            //var maxLayerCount = 48;

            //if (!IS_HONOR_ROLL_IMAGE)
            //{
            //    bricksInRowDivide = 7;
            //    maxLayerCount = 14;
            //}

            //float areaWidth = (float)(info.Width * .8);
            //float areaHeight = (float)(info.Height * .8);


            //areaWidth = (float)(info.Width);
            //areaHeight = (float)(info.Height);

            //areaWidth = 1182;
            //areaHeight = 840;

            //float startFromY = 342;

            //var modelWidth = areaWidth / bricksInRowDivide;
            //var modelHeight = modelWidth / 2;

            //float startFrom = (info.Width - areaWidth) / 2;
            //startFrom = 7;

            //GenerateAlphabetList();

            //var brickPalate = new BrickPalate
            //{
            //    MaxLayerCount = maxLayerCount,
            //    StartFromY = startFromY,
            //    LayerNumber = 0,
            //    LayerWidth = areaWidth,
            //    WallHeight = areaHeight,
            //    StartFrom = startFrom,
            //    Layers = new SKPath(),
            //    //model = CreateRect(new SKPoint(0, 0), 30, 70),
            //    model = CreateRect(new SKPoint(0, 0), modelHeight, modelWidth),
            //    BrickText = $"Rocky: ",
            //    Bricktionary = new List<BrickDetail>(),
            //    IsFirstLayer = true
            //};

            ////var gridOutline = CreateRect(new SKPoint(0, 0), modelHeight * brickPalate.MaxLayerCount, info.Width);
            //gridOutline = new SKRect(startFrom, startFromY, areaWidth, startFromY + (modelHeight * (brickPalate.MaxLayerCount)));

            //canvas.RotateDegrees((float)rotateSlider.Value, info.Width / 2, info.Height / 2);

            //canvas.DrawRect(gridOutline, paintGridBackground);
            //canvas.Save();
            //canvas.Restore();

            ////LayBricks(canvas, brickPalate);
            //LayBricksWithLines(canvas, brickPalate);

            //canvas.DrawPath(brickPalate.Layers, paintLine);

            ////DrawLineGrid(canvas, info.Width, info.Height);

        }

        void LoadFromSettings()
        {
            //if (!string.IsNullOrEmpty(Settings.WestBlockHImageSettings))
            //{
            //    //    canvases = Newtonsoft.Json.JsonConvert.DeserializeObject<CanvasDimension>(Settings.WestBlockHImageSettings);
            //    //    OnBtnImgSelectClicked(this, new EventArgs());
            //}
        }

        void SaveToSettings()
        {
            //var json = Newtonsoft.Json.JsonConvert.SerializeObject(canvases);
            //Settings.WestBlockHImageSettings = json;

        }

        void OnSearchCompletedAnimate(object sender, System.EventArgs e)
        {
            var control = sender as Entry;
            if (null == control)
                return;

            //MemorialMapAreas.GetZoomToAnimationFromBrickCode(control.Text.ToUpper(), skCanvasTImage);

            control.Focus();
        }

        void OnSearchCompleted(object sender, System.EventArgs e)
        {
            var control = sender as Entry;
            if (null == control)
                return;

            MemorialMapAreas.GetZoomToDirectFromBrickCode(new Brick { Section = control.Text.ToUpper() }, skCanvasTImage);
        }

        protected void GenerateAlphabetList()
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
        }

        protected SKRect CreateRect(SKPoint start, float height, float width = 0)
        {
            var rect = new SKRect(start.X, start.Y, width <= 0 ? height : width, height);
            return rect;
        }

        protected void DrawPathGrid(SKCanvas canvas, int width, int height, int colDividerCount = 10)
        {
            var gridUnit = width / colDividerCount;
            int rowDividerCount = (int)(height / gridUnit);

            var dynPoint = new SKPoint(0, 0);

            var path = new SKPath();

            for (var i = 1; i <= colDividerCount; i++)
            {
                canvas.DrawLine(gridUnit * i, 0, gridUnit * i, height, paintLine);
                canvas.Restore();
            }

            for (var i = 1; i <= rowDividerCount; i++)
            {
                canvas.DrawLine(0, gridUnit * i, width, gridUnit * i, paintLine);
                canvas.Restore();
            }
        }

        protected void DrawLineGrid(SKCanvas canvas, int width, int height, int colDividerCount = 10)
        {
            var gridUnit = width / colDividerCount;
            int rowDividerCount = (int)(height / gridUnit);

            for (var i = 1; i <= colDividerCount; i++)
            {
                canvas.DrawLine(gridUnit * i, 0, gridUnit * i, height, paintLine);
                canvas.Restore();
            }

            for (var i = 1; i <= rowDividerCount; i++)
            {
                canvas.DrawLine(0, gridUnit * i, width, gridUnit * i, paintLine);
                canvas.Restore();
            }
        }

        public class BrickPalate
        {
            public int MaxLayerCount { get; set; }
            public int LayerNumber { get; set; }
            public float LayerWidth { get; set; }
            public float WallHeight { get; set; }
            public float StartFromY { get; set; }
            public float StartFrom { get; set; }
            public float NextLayerTop { get; set; }
            public int PreviousLayerBrickCount { get; set; }
            public SKPath Layers { get; set; }
            public SKRect model { get; set; }
            public string BrickText { get; set; }
            public int TotalBrickCount { get; set; }
            public List<BrickDetail> Bricktionary { get; set; }
            public bool IsFirstLayer { get; set; }
        }

        public class BrickDetail
        {
            public string Name { get; set; }
            public string location { get; set; }
            public int position { get; set; }
        }

        //public enum TouchManipulationMode
        //{
        //    None,
        //    PanOnly,
        //    IsotropicScale,     // includes panning
        //    AnisotropicScale,   // includes panning
        //    ScaleRotate,        // implies isotropic scaling
        //    ScaleDualRotate     // adds one-finger rotation
        //}

        public enum DimensionType
        {
            Scale_XY,
            Scale_X,
            Scale_Y,
            PanH,
            PanV,
            PerspectiveH,
            PerspectiveV,
            Rotate_Deg
        }

        public enum CanvasType
        {
            Image,
            Map,
            TImage
        }

        public class DimensionDetail
        {
            public DimensionType Type { get; set; }
            public float Value { get; set; }
        }

        public class CanvasDimension
        {
            public DimensionDetails Image { get; set; }
            public DimensionDetails Map { get; set; }
            public DimensionDetails TImage { get; set; }
            public DimensionDetails ImageRelative { get; set; }
            public DimensionDetails MapRelative { get; set; }
        }

        public class DimensionDetails : List<DimensionDetail>
        {
            public float GetValue(DimensionType type)
            {
                return this.FirstOrDefault(x => x.Type == type).Value;
            }

            public void SetValue(DimensionType type, float value)
            {
                var item = this.FirstOrDefault(x => x.Type == type);
                if (null == item)
                    return;
                item.Value = value;
            }

            public int Width { get; set; }
            public int Height { get; set; }

            public int WidthNew { get; set; }
            public int HeightNew { get; set; }

            public void UpdateRelativeDimensions()
            {
                var w_ratio = (double)WidthNew / Width;
                var h_ratio = (double)HeightNew / Height;

                //ds.Add(new DimensionDetail { Type = DimensionType.Scale_X, Value = 100 });
                //ds.Add(new DimensionDetail { Type = DimensionType.Scale_Y, Value = 100 });
                //ds.Add(new DimensionDetail { Type = DimensionType.Scale_XY, Value = 100 });
                //ds.Add(new DimensionDetail { Type = DimensionType.PerspectiveH, Value = 0 });
                //ds.Add(new DimensionDetail { Type = DimensionType.PerspectiveV, Value = 0 });
                //ds.Add(new DimensionDetail { Type = DimensionType.PanH, Value = 0 });
                //ds.Add(new DimensionDetail { Type = DimensionType.PanV, Value = 0 });
            }
        }

        protected override SizeRequest OnMeasure(double widthConstraint, double heightConstraint)
        {
            if (heightConstraint > 0 && pageHeight <= 0)
            {
                pageHeight = heightConstraint;
                pageWidth = widthConstraint;
            }

            return base.OnMeasure(widthConstraint, heightConstraint);
        }

        void OnSliderValueChanged(object sender, ValueChangedEventArgs args)
        {
            if (canvasType == CanvasType.Image)
            {
                skCanvas.InvalidateSurface();
            }
            else if (canvasType == CanvasType.TImage)
            {
                skCanvasTImage.InvalidateSurface();
            }
            else
            {
                skCanvasGrid.InvalidateSurface();
            }
        }

        void OnPersp0SliderValueChanged(object sender, ValueChangedEventArgs args)
        {
            Slider slider = (Slider)sender;

            persp0Label.Text = String.Format("PERS. V = {0:F4}", slider.Value / 100);

            if (canvasType == CanvasType.Image)
            {
                canvases.Image.SetValue(DimensionType.PerspectiveV, (float)slider.Value);
                skCanvas.InvalidateSurface();
            }
            else if (canvasType == CanvasType.TImage)
            {
                canvases.TImage.SetValue(DimensionType.PerspectiveV, (float)slider.Value);
                skCanvasTImage.InvalidateSurface();
            }
            else
            {
                canvases.Map.SetValue(DimensionType.PerspectiveV, (float)slider.Value);
                skCanvasGrid.InvalidateSurface();
            }
        }

        void OnPersp1SliderValueChanged(object sender, ValueChangedEventArgs args)
        {
            Slider slider = (Slider)sender;

            persp1Label.Text = String.Format("PERS. H = {0:F4}", slider.Value / 100);

            if (canvasType == CanvasType.Image)
            {
                canvases.Image.SetValue(DimensionType.PerspectiveH, (float)slider.Value);
                skCanvas.InvalidateSurface();
            }
            else if (canvasType == CanvasType.TImage)
            {
                canvases.TImage.SetValue(DimensionType.PerspectiveH, (float)slider.Value);
                skCanvasTImage.InvalidateSurface();
            }
            else
            {
                canvases.Map.SetValue(DimensionType.PerspectiveH, (float)slider.Value);
                skCanvasGrid.InvalidateSurface();
            }
        }

        void OnPersp2SliderValueChanged(object sender, ValueChangedEventArgs args)
        {
            if (null == persp2Label)
                return;

            Slider slider = (Slider)sender;

            persp2Label.Text = String.Format("SCALE X= {0:F4}", slider.Value / 100);

            if (lockScale)
                persp5Slider.Value = slider.Value;

            if (canvasType == CanvasType.Image)
            {
                canvases.Image.SetValue(DimensionType.Scale_X, (float)slider.Value);
                if (lockScale)
                    canvases.Image.SetValue(DimensionType.Scale_Y, (float)slider.Value);
                skCanvas.InvalidateSurface();
            }
            else if (canvasType == CanvasType.TImage)
            {
                canvases.TImage.SetValue(DimensionType.Scale_X, (float)slider.Value);
                if (lockScale)
                    canvases.TImage.SetValue(DimensionType.Scale_Y, (float)slider.Value);
                skCanvasTImage.InvalidateSurface();
            }
            else
            {
                canvases.Map.SetValue(DimensionType.Scale_X, (float)slider.Value);
                if (lockScale)
                    canvases.Map.SetValue(DimensionType.Scale_Y, (float)slider.Value);
                skCanvasGrid.InvalidateSurface();
            }
        }

        void OnPersp3SliderValueChanged(object sender, ValueChangedEventArgs args)
        {
            if (null == persp3Label)
                return;

            Slider slider = (Slider)sender;

            persp3Label.Text = String.Format("H. PAN = {0:F4}", slider.Value);

            if (canvasType == CanvasType.Image)
            {
                canvases.Image.SetValue(DimensionType.PanH, (float)slider.Value);
                skCanvas.InvalidateSurface();
            }
            else if (canvasType == CanvasType.TImage)
            {
                canvases.TImage.SetValue(DimensionType.PanH, (float)slider.Value);
                skCanvasTImage.InvalidateSurface();
            }
            else
            {
                canvases.Map.SetValue(DimensionType.PanH, (float)slider.Value);
                skCanvasGrid.InvalidateSurface();
            }
        }

        void OnPersp4SliderValueChanged(object sender, ValueChangedEventArgs args)
        {
            if (null == persp4Label)
                return;

            Slider slider = (Slider)sender;

            persp4Label.Text = String.Format("V. PAN = {0:F4}", slider.Value);

            if (canvasType == CanvasType.Image)
            {
                canvases.Image.SetValue(DimensionType.PanV, (float)slider.Value);
                skCanvas.InvalidateSurface();
            }
            else if (canvasType == CanvasType.TImage)
            {
                canvases.TImage.SetValue(DimensionType.PanV, (float)slider.Value);
                skCanvasTImage.InvalidateSurface();
            }
            else
            {
                canvases.Map.SetValue(DimensionType.PanV, (float)slider.Value);
                skCanvasGrid.InvalidateSurface();
            }
        }

        void OnPersp5SliderValueChanged(object sender, ValueChangedEventArgs args)
        {
            if (null == persp5Label)
                return;

            Slider slider = (Slider)sender;
            persp5Label.Text = String.Format("SCALE Y = {0:F4}", slider.Value / 100);

            if (canvasType == CanvasType.Image)
            {
                canvases.Image.SetValue(DimensionType.Scale_Y, (float)slider.Value);
                skCanvas.InvalidateSurface();
            }
            else if (canvasType == CanvasType.TImage)
            {
                canvases.TImage.SetValue(DimensionType.Scale_Y, (float)slider.Value);
                skCanvasTImage.InvalidateSurface();
            }
            else
            {
                canvases.Map.SetValue(DimensionType.Scale_Y, (float)slider.Value);
                skCanvasGrid.InvalidateSurface();
            }
        }

        void OnRotateSliderValueChanged(object sender, ValueChangedEventArgs args)
        {
            if (null == persp5Label)
                return;

            Slider slider = (Slider)sender;
            rotateLabel.Text = String.Format("ROTATE Deg. = {0:F4}", slider.Value / 100);

            if (canvasType == CanvasType.Image)
            {
                canvases.Image.SetValue(DimensionType.Rotate_Deg, (float)slider.Value);
                skCanvas.InvalidateSurface();
            }
            else if (canvasType == CanvasType.TImage)
            {
                canvases.TImage.SetValue(DimensionType.Rotate_Deg, (float)slider.Value);
                skCanvasTImage.InvalidateSurface();
            }
            else
            {
                canvases.Map.SetValue(DimensionType.Rotate_Deg, (float)slider.Value);
                skCanvasGrid.InvalidateSurface();
            }
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
        }

        void OnSearchButtonClicked(object sender, System.EventArgs e)
        {
            skCanvas.InvalidateSurface();
        }

        void InitializeResets()
        {
            persp2Label.Tapped += (sender, e) => { persp2Slider.Value = 100; };
            persp0Label.Tapped += (sender, e) => { persp0Slider.Value = 0; };
            persp1Label.Tapped += (sender, e) => { persp1Slider.Value = 0; };
            persp3Label.Tapped += (sender, e) => { persp3Slider.Value = 0; };
            persp4Label.Tapped += (sender, e) => { persp4Slider.Value = 0; };
            persp5Label.Tapped += (sender, e) => { persp5Slider.Value = 100; };
            rotateLabel.Tapped += (sender, e) => { rotateSlider.Value = 0; };
        }

        void OnBtnImgSelectClicked(object sender, System.EventArgs e)
        {
            canvasType = CanvasType.Image;
            persp2Slider.Value = canvases.Image.GetValue(DimensionType.Scale_X);
            persp5Slider.Value = canvases.Image.GetValue(DimensionType.Scale_Y);
            persp0Slider.Value = canvases.Image.GetValue(DimensionType.PerspectiveV);
            persp1Slider.Value = canvases.Image.GetValue(DimensionType.PerspectiveH);
            persp3Slider.Value = canvases.Image.GetValue(DimensionType.PanH);
            persp4Slider.Value = canvases.Image.GetValue(DimensionType.PanV);
            rotateSlider.Value = canvases.Image.GetValue(DimensionType.Rotate_Deg);
            Title = "Image Settings";
            btnImgSelect.BackgroundColor = Colors.Lime;
            btnMapSelect.BackgroundColor = Colors.Transparent;
            btnTImgSelect.BackgroundColor = Colors.Transparent;
        }

        void OnBtnMapSelectClicked(object sender, System.EventArgs e)
        {
            canvasType = CanvasType.Map;
            persp2Slider.Value = canvases.Map.GetValue(DimensionType.Scale_X);
            persp5Slider.Value = canvases.Map.GetValue(DimensionType.Scale_Y);
            persp0Slider.Value = canvases.Map.GetValue(DimensionType.PerspectiveV);
            persp1Slider.Value = canvases.Map.GetValue(DimensionType.PerspectiveH);
            persp3Slider.Value = canvases.Map.GetValue(DimensionType.PanH);
            persp4Slider.Value = canvases.Map.GetValue(DimensionType.PanV);
            rotateSlider.Value = canvases.Map.GetValue(DimensionType.Rotate_Deg);
            Title = "Map Settings";
            btnImgSelect.BackgroundColor = Colors.Transparent;
            btnTImgSelect.BackgroundColor = Colors.Transparent;
            btnMapSelect.BackgroundColor = Colors.Lime;
        }

        void OnBtnTImgSelectClicked(object sender, System.EventArgs e)
        {
            canvasType = CanvasType.TImage;
            persp2Slider.Value = canvases.TImage.GetValue(DimensionType.Scale_X);
            persp5Slider.Value = canvases.TImage.GetValue(DimensionType.Scale_Y);
            persp0Slider.Value = canvases.TImage.GetValue(DimensionType.PerspectiveV);
            persp1Slider.Value = canvases.TImage.GetValue(DimensionType.PerspectiveH);
            persp3Slider.Value = canvases.TImage.GetValue(DimensionType.PanH);
            persp4Slider.Value = canvases.TImage.GetValue(DimensionType.PanV);
            rotateSlider.Value = canvases.TImage.GetValue(DimensionType.Rotate_Deg);
            Title = "TImage Settings";
            btnImgSelect.BackgroundColor = Colors.Transparent;
            btnMapSelect.BackgroundColor = Colors.Transparent;
            btnTImgSelect.BackgroundColor = Colors.Lime;
        }

        void OnBtnShowHideControlsClicked(object sender, System.EventArgs e)
        {
            var control = sender as Button;

            if (null == control)
                return;

            if (modeIs3DControl)
                rotation3DGrid.IsVisible = !rotation3DGrid.IsVisible;
            else
                controllerLayout.IsVisible = !controllerLayout.IsVisible;

            btnShowHideControls.Text = controllerLayout.IsVisible ? "^" : "v";
        }

    }
}
