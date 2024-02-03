using System;
using System.Collections.Generic;

using SkiaSharp;

using TouchTracking;
using static MasonVeteransMemorial.Controls.SkiaSharp.SkiaSharpCommon;

namespace MasonVeteransMemorial.Transforms
{
    class TouchManipulationBitmap
    {
        public SKBitmap bitmap;

        Dictionary<long, TouchManipulationInfo> touchDictionary =
            new Dictionary<long, TouchManipulationInfo>();

        public TouchManipulationBitmap(SKBitmap bitmap)
        {
            this.bitmap = bitmap;
            Matrix = SKMatrix.MakeIdentity();

            TouchManager = new TouchManipulationManager
            {
                Mode = TouchManipulationMode.ScaleRotate
            };
        }

        public TouchManipulationManager TouchManager { set; get; }

        public SKMatrix Matrix { set; get; }

        public void Paint(SKCanvas canvas)
        {
            canvas.Save();
            SKMatrix matrix = Matrix;
            canvas.Concat(ref matrix);

            canvas.DrawBitmap(bitmap, 0, 0);
            canvas.Restore();
        }

        public void PaintNavigate(SKCanvas canvas, MapArea area)
        {
            canvas.Save();

            var newMatrix = new SKMatrix()
            {
                Persp0 = 0,
                Persp1 = 0,
                Persp2 = 1,
                ScaleX = area.ScaleX,
                ScaleY = area.ScaleY,
                SkewX = 0,
                SkewY = 0,
                TransX = area.TransX,
                TransY = area.TransY
            };
            SKMatrix matrix = newMatrix;
            canvas.Concat(ref matrix);

            canvas.DrawBitmap(bitmap, 0, 0);
            canvas.Restore();
            Matrix = newMatrix;
        }

        public void PaintSimple(SKCanvas canvas)
        {
            canvas.Save();

            var newMatrix = new SKMatrix()
            {
                Persp0 = 0,
                Persp1 = 0,
                Persp2 = 1,
                ScaleX = Matrix.ScaleX,
                ScaleY = Matrix.ScaleY,
                SkewX = 0,
                SkewY = 0,
                TransX = Matrix.TransX,
                TransY = Matrix.TransY
            };
            SKMatrix matrix = newMatrix;
            canvas.Concat(ref matrix);

            canvas.DrawBitmap(bitmap, 0, 0);
            canvas.Restore();
            Matrix = newMatrix;
        }

        public void PaintReset(SKCanvas canvas)
        {
            canvas.Clear();
            var newMatrix = new SKMatrix()
            {
                Persp0 = 0,
                Persp1 = 0,
                Persp2 = 1,
                ScaleX = 1,
                ScaleY = 1,
                SkewX = 0,
                SkewY = 0,
                TransX = 0,
                TransY = 0
            };
            SKMatrix matrix = newMatrix;
            canvas.Concat(ref matrix);
            canvas.DrawBitmap(bitmap, 0, 0);
            //canvas.Restore();
            Matrix = newMatrix;
        }

        public bool HitTest(SKPoint location)
        {
            // Invert the matrix
            SKMatrix inverseMatrix;

            if (Matrix.TryInvert(out inverseMatrix))
            {
                // Transform the point using the inverted matrix
                SKPoint transformedPoint = inverseMatrix.MapPoint(location);

                // Check if it's in the untransformed bitmap rectangle
                SKRect rect = new SKRect(0, 0, bitmap.Width, bitmap.Height);
                return rect.Contains(transformedPoint);
            }
            return false;
        }

        public void ProcessTouchEvent(long id, TouchActionType type, SKPoint location)
        {
            switch (type)
            {
                case TouchActionType.Pressed:
                    touchDictionary.Add(id, new TouchManipulationInfo
                    {
                        PreviousPoint = location,
                        NewPoint = location
                    });
                    break;

                case TouchActionType.Moved:
                    TouchManipulationInfo info = touchDictionary[id];
                    info.NewPoint = location;
                    Manipulate();
                    info.PreviousPoint = info.NewPoint;
                    break;

                case TouchActionType.Released:
                    touchDictionary[id].NewPoint = location;
                    Manipulate();
                    touchDictionary.Remove(id);
                    break;

                case TouchActionType.Cancelled:
                    touchDictionary.Remove(id);
                    break;
            }
        }

        void Manipulate()
        {
            TouchManipulationInfo[] infos = new TouchManipulationInfo[touchDictionary.Count];
            touchDictionary.Values.CopyTo(infos, 0);
            SKMatrix touchMatrix = SKMatrix.MakeIdentity();

            if (infos.Length == 1)
            {
                SKPoint prevPoint = infos[0].PreviousPoint;
                SKPoint newPoint = infos[0].NewPoint;
                SKPoint pivotPoint = Matrix.MapPoint(bitmap.Width / 2, bitmap.Height / 2);

                touchMatrix = TouchManager.OneFingerManipulate(prevPoint, newPoint, pivotPoint);
            }
            else if (infos.Length >= 2)
            {
                int pivotIndex = infos[0].NewPoint == infos[0].PreviousPoint ? 0 : 1;
                SKPoint pivotPoint = infos[pivotIndex].NewPoint;
                SKPoint newPoint = infos[1 - pivotIndex].NewPoint;
                SKPoint prevPoint = infos[1 - pivotIndex].PreviousPoint;

                touchMatrix = TouchManager.TwoFingerManipulate(prevPoint, newPoint, pivotPoint);
            }

            SKMatrix matrix = Matrix;
            SKMatrix.PostConcat(ref matrix, touchMatrix);
            Matrix = matrix;
        }
    }


    class TouchManipulationCanvas
    {
        //SKBitmap bitmap;
        SKCanvas _canvas;
        SKSize _size;

        Dictionary<long, TouchManipulationInfo> touchDictionary =
            new Dictionary<long, TouchManipulationInfo>();

        public TouchManipulationCanvas(SKCanvas canvas, SKSize size)
        {
            _canvas = canvas;
            _size = size;
            Matrix = SKMatrix.MakeIdentity();

            TouchManager = new TouchManipulationManager
            {
                Mode = TouchManipulationMode.ScaleRotate
            };
        }

        public TouchManipulationManager TouchManager { set; get; }

        public SKMatrix Matrix { set; get; }

        public void Paint(SKCanvas canvas)
        {
            canvas.Save();
            SKMatrix matrix = Matrix;
            canvas.Concat(ref matrix);
            //canvas.DrawBitmap(bitmap, 0, 0);
            canvas.Restore();
        }

        public void PaintReset(SKCanvas canvas)
        {
            canvas.Clear();
            var newMatrix = new SKMatrix()
            {
                Persp0 = 0,
                Persp1 = 0,
                Persp2 = 1,
                ScaleX = 1,
                ScaleY = 1,
                SkewX = 0,
                SkewY = 0,
                TransX = 0,
                TransY = 0
            };
            SKMatrix matrix = newMatrix;
            canvas.Concat(ref matrix);
            //canvas.DrawBitmap(bitmap, 0, 0);
            //canvas.Restore();
            Matrix = newMatrix;
        }

        public bool HitTest(SKPoint location)
        {
            // Invert the matrix
            SKMatrix inverseMatrix;

            if (Matrix.TryInvert(out inverseMatrix))
            {
                // Transform the point using the inverted matrix
                SKPoint transformedPoint = inverseMatrix.MapPoint(location);

                // Check if it's in the untransformed bitmap rectangle
                SKRect rect = new SKRect(0, 0, _size.Width, _size.Height);
                return rect.Contains(transformedPoint);
            }
            return false;
        }

        public void ProcessTouchEvent(long id, TouchActionType type, SKPoint location)
        {
            switch (type)
            {
                case TouchActionType.Pressed:
                    touchDictionary.Add(id, new TouchManipulationInfo
                    {
                        PreviousPoint = location,
                        NewPoint = location
                    });
                    break;

                case TouchActionType.Moved:
                    TouchManipulationInfo info = touchDictionary[id];
                    info.NewPoint = location;
                    Manipulate();
                    info.PreviousPoint = info.NewPoint;
                    break;

                case TouchActionType.Released:
                    touchDictionary[id].NewPoint = location;
                    Manipulate();
                    touchDictionary.Remove(id);
                    break;

                case TouchActionType.Cancelled:
                    touchDictionary.Remove(id);
                    break;
            }
        }

        void Manipulate()
        {
            TouchManipulationInfo[] infos = new TouchManipulationInfo[touchDictionary.Count];
            touchDictionary.Values.CopyTo(infos, 0);
            SKMatrix touchMatrix = SKMatrix.MakeIdentity();

            if (infos.Length == 1)
            {
                SKPoint prevPoint = infos[0].PreviousPoint;
                SKPoint newPoint = infos[0].NewPoint;
                SKPoint pivotPoint = Matrix.MapPoint(_size.Width / 2, _size.Height / 2);

                touchMatrix = TouchManager.OneFingerManipulate(prevPoint, newPoint, pivotPoint);
            }
            else if (infos.Length >= 2)
            {
                int pivotIndex = infos[0].NewPoint == infos[0].PreviousPoint ? 0 : 1;
                SKPoint pivotPoint = infos[pivotIndex].NewPoint;
                SKPoint newPoint = infos[1 - pivotIndex].NewPoint;
                SKPoint prevPoint = infos[1 - pivotIndex].PreviousPoint;

                touchMatrix = TouchManager.TwoFingerManipulate(prevPoint, newPoint, pivotPoint);
            }

            SKMatrix matrix = Matrix;
            SKMatrix.PostConcat(ref matrix, touchMatrix);
            Matrix = matrix;
        }
    }

}
