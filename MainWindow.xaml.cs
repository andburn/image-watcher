using System;
using System.Windows.Media.Imaging;
using System.Windows.Forms;
using System.Windows;
using System.IO;

namespace ImageWatcher
{
	public partial class MainWindow : Window
	{
		private double _width;
		private BitmapImage _image;
		private FileSystemWatcher _watcher;

		public MainWindow()
		{
			InitializeComponent();
		}

		private void StartWatching(string filePath)
		{
			var path = Path.GetDirectoryName(filePath);
			var file = Path.GetFileName(filePath);

			_watcher = new FileSystemWatcher(path, file);

			_watcher.Changed += (sender, e) =>
			{
				this.Dispatcher.Invoke(new Action(() =>
				{
					LoadImage(filePath);
				}));
			};

			_watcher.EnableRaisingEvents = true;
		}

		private void UpdateStatus()
		{
			if (ImageBox.Source == null)
				return;

			_width = ImageBox.ActualWidth;
			var zoom = (int)(_width / _image.Width * 100);
			ZoomText.Text = $"{zoom}%";
		}

		private void LoadImage(string filePath)
		{
			if (!string.IsNullOrEmpty(filePath) && File.Exists(filePath))
			{
				try
				{
					byte[] buffer = File.ReadAllBytes(filePath);
					var ms = new System.IO.MemoryStream(buffer);
					var image = new BitmapImage();
					image.BeginInit();
					image.CacheOption = BitmapCacheOption.OnLoad;
					image.StreamSource = ms;
					image.EndInit();
					image.Freeze();
					// update
					_image = image;
					ImageBox.Source = _image;
					UpdateStatus();
				}
				catch
				{
					// TODO error
				}
			}
		}

		private void OpenBtnClick(object sender, RoutedEventArgs e)
		{
			OpenFileDialog dialog = new OpenFileDialog();

			dialog.InitialDirectory = @"C:\";
			dialog.Filter = "Image files(*.bmp;*.jpg;*.png)|*.bmp;*.jpg;*.png|All files (*.*)|*.*";
			dialog.FilterIndex = 1;
			dialog.Multiselect = false;

			if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
			{
				LoadImage(dialog.FileName);
				StartWatching(dialog.FileName);
			}
		}

		private void FitButtonClick(object sender, RoutedEventArgs e)
		{
			if (ImageBox.Stretch == System.Windows.Media.Stretch.None)
			{
				ImageBox.Stretch = System.Windows.Media.Stretch.Uniform;
				FitButton.Content = "\ue98a";
			}	
			else
			{
				ImageBox.Stretch = System.Windows.Media.Stretch.None;
				FitButton.Content = "\ue989";
			}			
		}

		private void ImageLayoutUpdated(object sender, EventArgs e)
		{
			UpdateStatus();
		}
	}
}