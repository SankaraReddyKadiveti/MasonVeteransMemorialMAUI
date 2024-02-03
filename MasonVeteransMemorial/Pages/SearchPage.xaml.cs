using MasonVeteransMemorial.Controls;
using MasonVeteransMemorial.Controls.SkiaSharp;
using MasonVeteransMemorial.Data;
using MasonVeteransMemorial.Models;
using MasonVeteransMemorial.NativeImplementations;
using MasonVeteransMemorial.Transforms;
using MasonVeteransMemorial.ViewModels;
using SkiaSharp;
using SkiaSharp.Views.Maui;
using SkiaSharp.Views.Maui.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using TouchTracking;

namespace MasonVeteransMemorial.Pages
{
	public partial class SearchPage : ContentPage, ISearchViewModelDelegate
	{
		#region SkiaSharp Variables

		TouchManipulationBitmap _searchResultsBitMap;
		SKBitmap _initialGroundsViewBitMap;
		SKBitmap _backgroundImage;

		MatrixDisplay matrixDisplay = new MatrixDisplay();
		List<long> touchIds = new List<long>();
		List<long> touchCanvasIds = new List<long>();

		bool _initialDisplay = true;

		bool isClear3D = false;
		bool isShowMatrixValues = false;
		bool isUseFingerPaint = false;

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
			Color = SKColors.Red,// Color.Red.ToSKColor(),
			StrokeWidth = 25
		};

		SKPaint paintLine = new SKPaint
		{
			Style = SKPaintStyle.Stroke,
			Color = SKColors.Black,// Color.Black.ToSKColor(),
			StrokeWidth = 1,
		};

		SKPaint paintHiLite = new SKPaint
		{
			Style = SKPaintStyle.Fill,
			Color = new SKColor(255, 255, 0), //Color.Yellow.ToSKColor(),
											  //IsAntialias = true,
		};

		SKPaint paintGridBackground = new SKPaint
		{
			Style = SKPaintStyle.Fill,
			Color = SKColors.Transparent.FromHex("#be5959"), //8e3f3f //be5959
		};

		SKPaint paintHiLiteLine = new SKPaint
		{
			Style = SKPaintStyle.Stroke,
			Color = SKColors.White,// Color.White.ToSKColor(),
			StrokeWidth = 5,
		};

		SKPaint paintHiLiteRed = new SKPaint
		{
			Style = SKPaintStyle.Fill,
			Color = SKColors.Red,// Color.Red.ToSKColor(),
		};

		// Create an SKPaint object to display the text
		SKPaint textPaint = new SKPaint
		{
			Color = SKColors.White,
			StrokeWidth = .5f
		};

		SKPaint textYellowPaint = new SKPaint
		{
			Color = SKColors.Yellow,
			StrokeWidth = .5f
		};

		Dictionary<long, SKPath> inProgressPaths = new Dictionary<long, SKPath>();
		List<SKPath> completedPaths = new List<SKPath>();

		#endregion
		public SearchViewModel ViewModel => BindingContext as SearchViewModel;
		public SearchPage()
		{
			InitializeComponent();
			BindingContext = new SearchViewModel();
			ViewModel.Delegate = this;

			Title = "Search";
			txtSearch.Keyboard = Keyboard.Create(KeyboardFlags.CapitalizeSentence | KeyboardFlags.Spellcheck);

			#region SkiaSharp Logic

			var assembly = IntrospectionExtensions.GetTypeInfo(typeof(SearchPage)).Assembly;

			string initialImagePath = "", backgroundImagepath = "", zoomImagePath = "";

			//*** Handle Bitmap Size Adjustment
			var overridDefaultBitmap = false;
			if (Device.RuntimePlatform == Device.Android)
				overridDefaultBitmap = true;
			else
				overridDefaultBitmap = false; //*** FOR NOW ALSO USE SMALLER IMAGES FOR IOS

			if (overridDefaultBitmap)
			{
				//AbsoluteLayout.SetLayoutBounds(slSelectedBrickDetail, new Rectangle(1, 1, .5, .20)); 
				slSelectedBrickDetail.Padding = new Thickness(0, 2, 0, 2); 
				slSelectedBrickDetail.Margin = new Thickness(0, -2, 0, 0); 

				initialImagePath = "MasonVeteransMemorial.Resources.Images.initial_startup_grounds.png";
				backgroundImagepath = "MasonVeteransMemorial.Resources.Images.background_for_blur.png";
				zoomImagePath = "MasonVeteransMemorial.Resources.Images.overhead_1161x425.png";
				// zoomImagePath = "MasonVeteransMemorial.Data.overhead_1161x425.png";

				initialImagePath = "MasonVeteransMemorial.Resources.Images.initial_startup_grounds_v2_620.png";
				backgroundImagepath = "MasonVeteransMemorial.Resources.Images.mason_background_2.png";
				zoomImagePath = "MasonVeteransMemorial.Resources.Images.overhead_1161x425.png";
				// zoomImagePath = "MasonVeteransMemorial.Data.overhead_1161x425.png";

			}
			else
			{
				//*** Use Small images for background things to help with resources

				initialImagePath = "MasonVeteransMemorial.Resources.Images.initial_startup_grounds.png";
				backgroundImagepath = "MasonVeteransMemorial.Resources.Images.background_for_blur.png";
				zoomImagePath = "MasonVeteransMemorial.Resources.Images.drone_pu_4_overhead_view.png";
			}

			//*** Initial Grounds View
			var path = initialImagePath;
			Stream stream = assembly.GetManifestResourceStream(path);
			using (SKManagedStream skStream = new SKManagedStream(stream))
			{
				_initialGroundsViewBitMap = SKBitmap.Decode(skStream);
			}

			//*** Search Results View (zoom)
			path = zoomImagePath;
			Stream stream2 = assembly.GetManifestResourceStream(path);
			using (SKManagedStream skStream2 = new SKManagedStream(stream2))
			{
				SKBitmap bitmapProcess = SKBitmap.Decode(skStream2);
				this._searchResultsBitMap = new TouchManipulationBitmap(bitmapProcess);
				this._searchResultsBitMap.TouchManager.Mode = TouchManipulationMode.ScaleRotate;
			}


			//*** Background Image View
			path = backgroundImagepath;
			Stream stream3 = assembly.GetManifestResourceStream(path);
			using (SKManagedStream skStream3 = new SKManagedStream(stream3))
			{
				_backgroundImage = SKBitmap.Decode(skStream3);
			}

			//_backgroundImage.ExtractAlpha()

			ViewModel.IsVisibleSearchResults = true;
			ShowCanvases(false);
			skCanvasStatic.InvalidateSurface();

			#endregion
		}

		protected void OnSearchTextChanged(object sender, TextChangedEventArgs e)
		{
			if (!Settings.UseMapPage)
				ViewModel.IsVisibleSearchResults = false;

			//ShowCanvases(false); to do
			ViewModel.SearchCommand.Execute(null);
		}

		protected void OnSearchTextFocused(object sender, FocusEventArgs e)
		{
			if (e.IsFocused && !Settings.UseMapPage)
			{
				//ShowCanvases(false); to do
				ViewModel.IsVisibleSearchResults = false;
				lvSearchResults.SelectedItem = null;
			}
		}

		public void OnSearchButtonPressed(object sender, EventArgs e)
		{
			ViewModel.SearchCommand.Execute(null);
		}

		void OnSearchButtonClicked(object sender, System.EventArgs e)
		{
			if (!Settings.UseMapPage)
			{
				lvSearchResults.SelectedItem = null;
				ViewModel.SearchCommand.Execute(null);
			}
		}
		void OnShowToolsMenuClicked(object sender, System.EventArgs e)
		{
			//ShowCanvases(true); to do
		}
		public void OnSearchCancelButtonPressed(object sender, EventArgs e)
		{
		}

		protected void OnSearchItemSelected(object sender, SelectedItemChangedEventArgs e)
		{
			var selected = e.SelectedItem as Brick;

			if (null == selected)
				return;

			var list = sender as ListView;
			var parent = this.Parent as MainAppTabContainer;
			var page = parent.Pages.FirstOrDefault(x => x.GetType() == typeof(MemorialMapPage));

			DependencyService.Get<IKeyboardHelper>().HideKeyboard();

			if (!Settings.UseMapPage)
			{
				ViewModel.SelectedBrick = selected;
				ViewModel.IsVisibleSearchResults = true;
				//ShowCanvases(true); to do

				MemorialMapAreas.GetZoomToAnimationFromBrickCode(selected, skCanvasStatic);
			}
			else
			{
				if (null == parent || null == list || null == page)
					return;

				var map = page as MemorialMapPage;
				map.MasonBrick = selected;

				parent.CurrentPage = map;
			}
		}

		SKPoint ConvertToPixel(Point pt)
		{
			return new SKPoint((float)(skCanvasStatic.CanvasSize.Width * pt.X / skCanvasStatic.Width),
							   (float)(skCanvasStatic.CanvasSize.Height * pt.Y / skCanvasStatic.Height));
		}

		SKPoint ConvertToPixelCustom(Point pt, SKCanvasView canvas)
		{
			return new SKPoint((float)(canvas.CanvasSize.Width * pt.X / canvas.Width),
							   (float)(canvas.CanvasSize.Height * pt.Y / canvas.Height));
		}

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
						skCanvasStatic.InvalidateSurface();

					}
					break;

				case TouchActionType.Moved:
					if (inProgressPaths.ContainsKey(args.Id))
					{
						SKPath path = inProgressPaths[args.Id];
						path.LineTo(ConvertToPixel(args.Location));
						skCanvasStatic.InvalidateSurface();
					}
					break;

				case TouchActionType.Released:
					if (inProgressPaths.ContainsKey(args.Id))
					{
						completedPaths.Add(inProgressPaths[args.Id]);
						inProgressPaths.Remove(args.Id);
						skCanvasStatic.InvalidateSurface();
					}
					break;

				case TouchActionType.Cancelled:
					if (inProgressPaths.ContainsKey(args.Id))
					{
						inProgressPaths.Remove(args.Id);
						skCanvasStatic.InvalidateSurface();
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
					new SKPoint((float)(skCanvasStatic.CanvasSize.Width * pt.X / skCanvasStatic.Width),
								(float)(skCanvasStatic.CanvasSize.Height * pt.Y / skCanvasStatic.Height));

				switch (args.Type)
				{
					case TouchActionType.Pressed:
						if (_searchResultsBitMap.HitTest(point))
						{
							touchIds.Add(args.Id);
							_searchResultsBitMap.ProcessTouchEvent(args.Id, args.Type, point);
							break;
						}
						break;

					case TouchActionType.Moved:
						if (touchIds.Contains(args.Id))
						{
							_searchResultsBitMap.ProcessTouchEvent(args.Id, args.Type, point);
							skCanvasStatic.InvalidateSurface();
						}
						break;

					case TouchActionType.Released:
					case TouchActionType.Cancelled:
						if (touchIds.Contains(args.Id))
						{
							_searchResultsBitMap.ProcessTouchEvent(args.Id, args.Type, point);
							touchIds.Remove(args.Id);
							skCanvasStatic.InvalidateSurface();
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
							var point = ConvertToPixelCustom(args.Location, skCanvasStatic);
							path.MoveTo(point);
							inProgressPaths.Add(args.Id, path);
							skCanvasStatic.InvalidateSurface();
						}
						break;

					case TouchActionType.Moved:
						if (inProgressPaths.ContainsKey(args.Id))
						{
							SKPath path = inProgressPaths[args.Id];
							path.LineTo(ConvertToPixelCustom(args.Location, skCanvasStatic));
							skCanvasStatic.InvalidateSurface();
						}
						break;

					case TouchActionType.Released:
						if (inProgressPaths.ContainsKey(args.Id))
						{
							completedPaths.Add(inProgressPaths[args.Id]);
							inProgressPaths.Remove(args.Id);
							skCanvasStatic.InvalidateSurface();
						}
						break;

					case TouchActionType.Cancelled:
						if (inProgressPaths.ContainsKey(args.Id))
						{
							inProgressPaths.Remove(args.Id);
							skCanvasStatic.InvalidateSurface();
						}
						break;
				}
			}
		}

		void ShowCanvases(bool showSearchResultsCanvases)
		{
			if (showSearchResultsCanvases)
			{
				if (_initialDisplay)
				{
					//*** On first search results, set initial image to full zoom out
					MemorialMapAreas.GetZoomToDirectFromBrickCode(new Brick(), skCanvasStatic, false, true);
					//MemorialMapAreas.GetZoomToDirectFromBrickCode(new Brick(), skCanvasStatic, true);
				}
				_initialDisplay = false;

				 slCanvasButtonBar.IsVisible = slSelectedBrickDetail.IsVisible = true; 
				 slSelectedBrickDetail.HorizontalOptions = LayoutOptions.FillAndExpand;
			}
			else
			{
				slCanvasButtonBar.IsVisible = slSelectedBrickDetail.IsVisible = false; 
			}
		}

		//***** BITMAP Static
		protected void skCanvasStatic_OnPaintSurface(object sender, SKPaintSurfaceEventArgs e)
		{
			var info = e.Info;
			var surface = e.Surface;
			var canvas = surface.Canvas;

			if (!MemorialMapAreas.NotFirstClear)
			{
				canvas.Clear();
				MemorialMapAreas.NotFirstClear = true;
			}


			// Center of screen
			float xCenter = (float)((float)info.Width / 2.5);
			float yCenter = (float)((float)info.Height / 2.5);


			if (_initialDisplay)
			{
				//Hi Res Intro Image
				// Coordinates to center bitmap on canvas
				float x = xCenter - _initialGroundsViewBitMap.Width / 2;
				float y = yCenter - _initialGroundsViewBitMap.Height / 2;
				var newHeight = info.Width * _initialGroundsViewBitMap.Height / _initialGroundsViewBitMap.Width;

				//Initial image
				SKRect bitmapRect = new SKRect(0, 0, info.Width, newHeight);
				canvas.DrawBitmap(_initialGroundsViewBitMap, bitmapRect);

				MemorialMapAreas.UpdateScaleFactorsForCanvasSize(info.Width, info.Height, _searchResultsBitMap.bitmap.Width, _searchResultsBitMap.bitmap.Height);
			}
			else
			{
				if (null != _initialGroundsViewBitMap)
				{
					_initialGroundsViewBitMap.Dispose();
					_initialGroundsViewBitMap = null;
				}

				// Apply Blur Mask
				var filter = SKImageFilter.CreateBlur(5, 5);

				var paintMask = new SKPaint();
				paintMask.ImageFilter = filter;

				// Background image
				SKRect bitmapRect = new SKRect(0, 0, info.Width, info.Height);
				canvas.DrawBitmap(_backgroundImage, bitmapRect, paintMask);


				// Display the bitmap
				if (MemorialMapAreas.IsMoveToMapAreaAnimate)
				{
					_searchResultsBitMap.PaintNavigate(canvas, MemorialMapAreas.View_Animate);
				}
				else if (MemorialMapAreas.IsMoveToMapArea)
				{
					_searchResultsBitMap.PaintNavigate(canvas, MemorialMapAreas.MoveToMapArea);
					MemorialMapAreas.IsMoveToMapArea = false;
				}
				else if (isClear3D)
				{
					isClear3D = false;
					_searchResultsBitMap.PaintReset(canvas);
				}
				else
				{
					_searchResultsBitMap.PaintSimple(canvas);

					MemorialMapAreas.MoveToMapArea.ScaleX = _searchResultsBitMap.Matrix.ScaleX;
					MemorialMapAreas.MoveToMapArea.ScaleY = _searchResultsBitMap.Matrix.ScaleY;
					MemorialMapAreas.MoveToMapArea.TransX = _searchResultsBitMap.Matrix.TransX;
					MemorialMapAreas.MoveToMapArea.TransY = _searchResultsBitMap.Matrix.TransY;
				}

				//*** Display the matrix in the lower-right corner
				if (isShowMatrixValues)
				{
					slSelectedBrickDetail.IsVisible = false;

					SKSize matrixSize = matrixDisplay.Measure(_searchResultsBitMap.Matrix);

					matrixDisplay.Paint(canvas, _searchResultsBitMap.Matrix,
						new SKPoint(info.Width - matrixSize.Width,
									info.Height - matrixSize.Height));
				}

				////HONOR ROLL BRICKS
				MemorialMapAreas.CreateHonorRollAngledBrickGrid(canvas, _searchResultsBitMap.Matrix, ViewModel.SelectedBrick);
			}
		}

		void ReplayZoom_Clicked(object sender, System.EventArgs e)
		{
			MemorialMapAreas.GetZoomToAnimationFromBrickCode(ViewModel.SelectedBrick, skCanvasStatic);
		}

		void FullZoomOut_Clicked(object sender, System.EventArgs e)
		{
			MemorialMapAreas.GetZoomToAnimationSimpleFromBrickCode(ViewModel.SelectedBrick, skCanvasStatic, true);
		}

	   

		protected override void OnAppearing()
		{
			base.OnAppearing();
			ViewModel.LoadCommand.Execute(null);
		}

		protected void OnLogoClicked(object sender, EventArgs e)
		{
			Browser.OpenAsync(new Uri(Settings.MasonHomePageUrl), BrowserLaunchMode.SystemPreferred);
		}
		protected void OnHomeClicked(object sender, EventArgs e)
		{
			Shell.Current.GoToAsync($"//{nameof(MainPage)}");
		}

		protected void OnSearchClicked(object sender, EventArgs e)
		{
			//Shell.Current.GoToAsync(nameof(SearchPage));
		}

		protected void OnAboutClicked(object sender, EventArgs e)
		{
			Browser.OpenAsync(new Uri(Settings.MasonAboutPageUrl), BrowserLaunchMode.SystemPreferred);
		}

		public void OnLoadSuccess()
		{
		}

		public void OnSearchComplete()
		{
		}
		public void OnLoadFailure(string title, string message)
		{
		}
	}
}


/*using MasonVeteransMemorial.Controls;
using MasonVeteransMemorial.Controls.SkiaSharp;
using MasonVeteransMemorial.Data;
using MasonVeteransMemorial.Models;
using MasonVeteransMemorial.NativeImplementations;
using MasonVeteransMemorial.Transforms;
using MasonVeteransMemorial.ViewModels;
using SkiaSharp;
using SkiaSharp.Views.Maui.Controls;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
// System.Windows.Controls;
using TouchTracking;
using Point = Microsoft.Maui.Graphics.Point;

namespace MasonVeteransMemorial.Pages
{
	public partial class SearchPage : ContentPage, ISearchViewModelDelegate
	{

		#region SkiaSharp Variables

		TouchManipulationBitmap _searchResultsBitMap;
		SKBitmap _initialGroundsViewBitMap;
		SKBitmap _backgroundImage;

		MatrixDisplay matrixDisplay = new MatrixDisplay();
		List<long> touchIds = new List<long>();
		List<long> touchCanvasIds = new List<long>();

		bool _initialDisplay = true;

		bool isClear3D = false;
		bool isShowMatrixValues = false;
		bool isUseFingerPaint = false;

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
			Color = SKColors.Red,// Color.Red.ToSKColor(),
			StrokeWidth = 25
		};

		SKPaint paintLine = new SKPaint
		{
			Style = SKPaintStyle.Stroke,
			Color = SKColors.Black,// Color.Black.ToSKColor(),
			StrokeWidth = 1,
		};

		SKPaint paintHiLite = new SKPaint
		{
			Style = SKPaintStyle.Fill,
			Color = new SKColor(255, 255, 0), //Color.Yellow.ToSKColor(),
			//IsAntialias = true,
		};

		SKPaint paintGridBackground = new SKPaint
		{
			Style = SKPaintStyle.Fill,
			Color = SKColors.Transparent.FromHex("#be5959"), //8e3f3f //be5959
		};

		SKPaint paintHiLiteLine = new SKPaint
		{
			Style = SKPaintStyle.Stroke,
			Color = SKColors.White,// Color.White.ToSKColor(),
			StrokeWidth = 5,
		};

		SKPaint paintHiLiteRed = new SKPaint
		{
			Style = SKPaintStyle.Fill,
			Color = SKColors.Red,// Color.Red.ToSKColor(),
		};

		// Create an SKPaint object to display the text
		SKPaint textPaint = new SKPaint
		{
			Color = SKColors.White,
			StrokeWidth = .5f
		};

		SKPaint textYellowPaint = new SKPaint
		{
			Color = SKColors.Yellow,
			StrokeWidth = .5f
		};

		Dictionary<long, SKPath> inProgressPaths = new Dictionary<long, SKPath>();
		List<SKPath> completedPaths = new List<SKPath>();

		#endregion

		public SearchViewModel ViewModel => BindingContext as SearchViewModel;

		public SearchPage()
		{
			InitializeComponent();
			Title = "Search";
			//Icon = "ic_search";
			BindingContext = new SearchViewModel();
			//ViewModel.Delegate = this;

			txtSearch.Keyboard = Keyboard.Create(KeyboardFlags.CapitalizeSentence | KeyboardFlags.Spellcheck);

			#region SkiaSharp Logic

			var assembly = IntrospectionExtensions.GetTypeInfo(typeof(SearchPage)).Assembly;

			string initialImagePath = "", backgroundImagepath = "", zoomImagePath = "";

			//*** Handle Bitmap Size Adjustment
			var overridDefaultBitmap = false;
			if (Device.RuntimePlatform == Device.Android)
				overridDefaultBitmap = true;
			else
				overridDefaultBitmap = false; //*** FOR NOW ALSO USE SMALLER IMAGES FOR IOS

			if (overridDefaultBitmap)
			{
				//AbsoluteLayout.SetLayoutBounds(slSelectedBrickDetail, new Rectangle(1, 1, .5, .20));
				slSelectedBrickDetail.Padding = new Thickness(0, 2, 0, 2);
				slSelectedBrickDetail.Margin = new Thickness(0, -2, 0, 0);

				initialImagePath = "MasonVeteransMemorial.Data.initial_startup_grounds.png";
				backgroundImagepath = "MasonVeteransMemorial.Data.background_for_blur.png";
				zoomImagePath = "MasonVeteransMemorial.Data.overhead_1161x425.png";
				// zoomImagePath = "MasonVeteransMemorial.Data.overhead_1161x425.png";

				initialImagePath = "MasonVeteransMemorial.Data.initial_startup_grounds_v2_620.png";
				backgroundImagepath = "MasonVeteransMemorial.Data.mason_background_2.png";
				zoomImagePath = "MasonVeteransMemorial.Data.overhead_1161x425.png";
				// zoomImagePath = "MasonVeteransMemorial.Data.overhead_1161x425.png";

			}
			else
			{
				//*** Use Small images for background things to help with resources

				initialImagePath = "MasonVeteransMemorial.Data.initial_startup_grounds.png";
				backgroundImagepath = "MasonVeteransMemorial.Data.background_for_blur.png";
				zoomImagePath = "MasonVeteransMemorial.Data.drone_pu_4_overhead_view.png";
			}

			//*** Initial Grounds View
			var path = initialImagePath;
			Stream stream = assembly.GetManifestResourceStream(path);
			using (SKManagedStream skStream = new SKManagedStream(stream))
			{
				_initialGroundsViewBitMap = SKBitmap.Decode(skStream);
			}

			//*** Search Results View (zoom)
			path = zoomImagePath;
			Stream stream2 = assembly.GetManifestResourceStream(path);
			using (SKManagedStream skStream2 = new SKManagedStream(stream2))
			{
				SKBitmap bitmapProcess = SKBitmap.Decode(skStream2);
				this._searchResultsBitMap = new TouchManipulationBitmap(bitmapProcess);
				this._searchResultsBitMap.TouchManager.Mode = TouchManipulationMode.ScaleRotate;
			}


			//*** Background Image View
			path = backgroundImagepath;
			Stream stream3 = assembly.GetManifestResourceStream(path);
			using (SKManagedStream skStream3 = new SKManagedStream(stream3))
			{
				_backgroundImage = SKBitmap.Decode(skStream3);
			}

			//_backgroundImage.ExtractAlpha()

			ViewModel.IsVisibleSearchResults = true;
			ShowCanvases(false);
			skCanvasStatic.InvalidateSurface();

			#endregion
		}


		#region SkiaSharp Methods

		SKPoint ConvertToPixel(Point pt)
		{
			return new SKPoint((float)(skCanvasStatic.CanvasSize.Width * pt.X / skCanvasStatic.Width),
							   (float)(skCanvasStatic.CanvasSize.Height * pt.Y / skCanvasStatic.Height));
		}

		SKPoint ConvertToPixelCustom(Point pt, SKCanvasView canvas)
		{
			return new SKPoint((float)(canvas.CanvasSize.Width * pt.X / canvas.Width),
							   (float)(canvas.CanvasSize.Height * pt.Y / canvas.Height));
		}

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
						skCanvasStatic.InvalidateSurface();

					}
					break;

				case TouchActionType.Moved:
					if (inProgressPaths.ContainsKey(args.Id))
					{
						SKPath path = inProgressPaths[args.Id];
						path.LineTo(ConvertToPixel(args.Location));
						skCanvasStatic.InvalidateSurface();
					}
					break;

				case TouchActionType.Released:
					if (inProgressPaths.ContainsKey(args.Id))
					{
						completedPaths.Add(inProgressPaths[args.Id]);
						inProgressPaths.Remove(args.Id);
						skCanvasStatic.InvalidateSurface();
					}
					break;

				case TouchActionType.Cancelled:
					if (inProgressPaths.ContainsKey(args.Id))
					{
						inProgressPaths.Remove(args.Id);
						skCanvasStatic.InvalidateSurface();
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
					new SKPoint((float)(skCanvasStatic.CanvasSize.Width * pt.X / skCanvasStatic.Width),
								(float)(skCanvasStatic.CanvasSize.Height * pt.Y / skCanvasStatic.Height));

				switch (args.Type)
				{
					case TouchActionType.Pressed:
						if (_searchResultsBitMap.HitTest(point))
						{
							touchIds.Add(args.Id);
							_searchResultsBitMap.ProcessTouchEvent(args.Id, args.Type, point);
							break;
						}
						break;

					case TouchActionType.Moved:
						if (touchIds.Contains(args.Id))
						{
							_searchResultsBitMap.ProcessTouchEvent(args.Id, args.Type, point);
							skCanvasStatic.InvalidateSurface();
						}
						break;

					case TouchActionType.Released:
					case TouchActionType.Cancelled:
						if (touchIds.Contains(args.Id))
						{
							_searchResultsBitMap.ProcessTouchEvent(args.Id, args.Type, point);
							touchIds.Remove(args.Id);
							skCanvasStatic.InvalidateSurface();
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
							var point = ConvertToPixelCustom(args.Location, skCanvasStatic);
							path.MoveTo(point);
							inProgressPaths.Add(args.Id, path);
							skCanvasStatic.InvalidateSurface();
						}
						break;

					case TouchActionType.Moved:
						if (inProgressPaths.ContainsKey(args.Id))
						{
							SKPath path = inProgressPaths[args.Id];
							path.LineTo(ConvertToPixelCustom(args.Location, skCanvasStatic));
							skCanvasStatic.InvalidateSurface();
						}
						break;

					case TouchActionType.Released:
						if (inProgressPaths.ContainsKey(args.Id))
						{
							completedPaths.Add(inProgressPaths[args.Id]);
							inProgressPaths.Remove(args.Id);
							skCanvasStatic.InvalidateSurface();
						}
						break;

					case TouchActionType.Cancelled:
						if (inProgressPaths.ContainsKey(args.Id))
						{
							inProgressPaths.Remove(args.Id);
							skCanvasStatic.InvalidateSurface();
						}
						break;
				}
			}
		}

		void ShowCanvases(bool showSearchResultsCanvases)
		{
			if (showSearchResultsCanvases)
			{
				if (_initialDisplay)
				{
					//*** On first search results, set initial image to full zoom out
					MemorialMapAreas.GetZoomToDirectFromBrickCode(new Brick(), skCanvasStatic, false, true);
					//MemorialMapAreas.GetZoomToDirectFromBrickCode(new Brick(), skCanvasStatic, true);
				}
				_initialDisplay = false;

				slCanvasButtonBar.IsVisible = slSelectedBrickDetail.IsVisible = true;
				slSelectedBrickDetail.HorizontalOptions = LayoutOptions.FillAndExpand;
			}
			else
			{
				slCanvasButtonBar.IsVisible = slSelectedBrickDetail.IsVisible = false;
			}
		}

		//***** BITMAP Static
		protected void skCanvasStatic_OnPaintSurface(object sender, SkiaSharp.Views.Maui.SKPaintSurfaceEventArgs e)
		{
			var info = e.Info;
			var surface = e.Surface;
			var canvas = surface.Canvas;

			if (!MemorialMapAreas.NotFirstClear)
			{
				canvas.Clear();
				MemorialMapAreas.NotFirstClear = true;
			}


			// Center of screen
			float xCenter = (float)((float)info.Width / 2.5);
			float yCenter = (float)((float)info.Height / 2.5);


			if (_initialDisplay)
			{
				//Hi Res Intro Image
				// Coordinates to center bitmap on canvas
				float x = xCenter - _initialGroundsViewBitMap.Width / 2;
				float y = yCenter - _initialGroundsViewBitMap.Height / 2;
				var newHeight = info.Width * _initialGroundsViewBitMap.Height / _initialGroundsViewBitMap.Width;

				//Initial image
				SKRect bitmapRect = new SKRect(0, 0, info.Width, newHeight);
				canvas.DrawBitmap(_initialGroundsViewBitMap, bitmapRect);

				MemorialMapAreas.UpdateScaleFactorsForCanvasSize(info.Width, info.Height, _searchResultsBitMap.bitmap.Width, _searchResultsBitMap.bitmap.Height);
			}
			else
			{
				if (null != _initialGroundsViewBitMap)
				{
					_initialGroundsViewBitMap.Dispose();
					_initialGroundsViewBitMap = null;
				}

				// Apply Blur Mask
				var filter = SKImageFilter.CreateBlur(5, 5);

				var paintMask = new SKPaint();
				paintMask.ImageFilter = filter;

				// Background image
				SKRect bitmapRect = new SKRect(0, 0, info.Width, info.Height);
				canvas.DrawBitmap(_backgroundImage, bitmapRect, paintMask);


				// Display the bitmap
				if (MemorialMapAreas.IsMoveToMapAreaAnimate)
				{
					_searchResultsBitMap.PaintNavigate(canvas, MemorialMapAreas.View_Animate);
				}
				else if (MemorialMapAreas.IsMoveToMapArea)
				{
					_searchResultsBitMap.PaintNavigate(canvas, MemorialMapAreas.MoveToMapArea);
					MemorialMapAreas.IsMoveToMapArea = false;
				}
				else if (isClear3D)
				{
					isClear3D = false;
					_searchResultsBitMap.PaintReset(canvas);
				}
				else
				{
					_searchResultsBitMap.PaintSimple(canvas);

					MemorialMapAreas.MoveToMapArea.ScaleX = _searchResultsBitMap.Matrix.ScaleX;
					MemorialMapAreas.MoveToMapArea.ScaleY = _searchResultsBitMap.Matrix.ScaleY;
					MemorialMapAreas.MoveToMapArea.TransX = _searchResultsBitMap.Matrix.TransX;
					MemorialMapAreas.MoveToMapArea.TransY = _searchResultsBitMap.Matrix.TransY;
				}

				//*** Display the matrix in the lower-right corner
				if (isShowMatrixValues)
				{
					slSelectedBrickDetail.IsVisible = false;

					SKSize matrixSize = matrixDisplay.Measure(_searchResultsBitMap.Matrix);

					matrixDisplay.Paint(canvas, _searchResultsBitMap.Matrix,
						new SKPoint(info.Width - matrixSize.Width,
									info.Height - matrixSize.Height));
				}

				////HONOR ROLL BRICKS
				MemorialMapAreas.CreateHonorRollAngledBrickGrid(canvas, _searchResultsBitMap.Matrix, ViewModel.SelectedBrick);
			}
		}

		#endregion




		protected override void OnAppearing()
		{
			base.OnAppearing();
			ViewModel.LoadCommand.Execute(null);
		}
		protected void OnLogoClicked(object sender, EventArgs e)
		{
		Browser.OpenAsync(new Uri(Settings.MasonHomePageUrl), BrowserLaunchMode.SystemPreferred);  	
		} 
		protected void OnHomeClicked(object sender, EventArgs e)
		{
			Shell.Current.GoToAsync($"//{nameof(MainPage)}");
		}

		protected void OnSearchClicked(object sender, EventArgs e)
		{
			Shell.Current.GoToAsync(nameof(SearchPage));
		}

		protected void OnAboutClicked(object sender, EventArgs e)
		{
			Browser.OpenAsync(new Uri(Settings.MasonAboutPageUrl), BrowserLaunchMode.SystemPreferred);
		}

		public void OnLoadSuccess()
		{

		}

		public void OnSearchComplete()
		{

		}
		public void OnLoadFailure(string title, string message)
		{
		}

		public void OnSearchCancelButtonPressed(object sender, EventArgs e)
		{

		}	       

		void ReplayZoom_Clicked(object sender, System.EventArgs e)
		{
			MemorialMapAreas.GetZoomToAnimationFromBrickCode(ViewModel.SelectedBrick, skCanvasStatic);
		}

		void FullZoomOut_Clicked(object sender, System.EventArgs e)
		{
			MemorialMapAreas.GetZoomToAnimationSimpleFromBrickCode(ViewModel.SelectedBrick, skCanvasStatic, true);
		}

		void OnShowToolsMenuClicked(object sender, System.EventArgs e)
		{
			ShowCanvases(true);
		}

		public void OnSearchButtonPressed(object sender, EventArgs e)
		{
			ViewModel.SearchCommand.Execute(null);
		}

		protected void OnSearchTextChanged(object sender, Microsoft.Maui.Controls.TextChangedEventArgs e)
		{
			if (!Settings.UseMapPage)
				ViewModel.IsVisibleSearchResults = false;

			ShowCanvases(false);
			ViewModel.SearchCommand.Execute(null);
		}

		protected void OnSearchTextFocused(object sender, FocusEventArgs e)
		{
			if (e.IsFocused && !Settings.UseMapPage)
			{
				ShowCanvases(false);
				ViewModel.IsVisibleSearchResults = false;
				lvSearchResults.SelectedItem = null;
			}
		}


		void OnSearchButtonClicked(object sender, System.EventArgs e)
		{
			if (!Settings.UseMapPage)
			{
				lvSearchResults.SelectedItem = null;
				ViewModel.SearchCommand.Execute(null);
			}
		}

		protected void OnSearchItemSelected(object sender, SelectedItemChangedEventArgs e)
		{
			var selected = e.SelectedItem as Brick;

			if (null == selected)
				return;

			var list = sender; // as ListView;
			var parent = this.Parent as MainAppTabContainer;
			var page = parent.Pages.FirstOrDefault(x => x.GetType() == typeof(MemorialMapPage));

			DependencyService.Get<IKeyboardHelper>().HideKeyboard();

			if (!Settings.UseMapPage)
			{
				ViewModel.SelectedBrick = selected;
				ViewModel.IsVisibleSearchResults = true;
				ShowCanvases(true);

				MemorialMapAreas.GetZoomToAnimationFromBrickCode(selected, skCanvasStatic);
			}
			else
			{
				if (null == parent || null == list || null == page)
					return;

				var map = page as MemorialMapPage;
				map.MasonBrick = selected;

				parent.CurrentPage = map;
			}
		}

	}
}*/