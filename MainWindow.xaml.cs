using System;
using System.IO;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Media.Imaging;

namespace ImageWatcher
{
	public partial class MainWindow : Window
	{
		private FileSystemWatcher _watcher;

		private string _filePath;

		public MainWindow()
		{
			InitializeComponent();
		}

		private void StartWatching(string filePath)
		{
			_filePath = filePath;
			var path = Path.GetDirectoryName(filePath);
			var file = Path.GetFileName(filePath);

			_watcher = new FileSystemWatcher(path, file);

			_watcher.Changed += (sender, e) =>
			{
				this.Dispatcher.Invoke(new Action(() =>
				{
					LoadImage(_filePath);
				}));
			};

			_watcher.EnableRaisingEvents = true;
		}

		private void UpdateWatcher(string filePath)
		{
			if (_watcher == null)
				StartWatching(filePath);

			_filePath = filePath;
			var path = Path.GetDirectoryName(filePath);
			var file = Path.GetFileName(filePath);

			_watcher.Path = path;
			_watcher.Filter = file;
		}

		private void UpdateStatus()
		{
			if (ImageBox.Source == null)
				return;

			var width = ImageBox.ActualWidth;
			var bmp = ImageBox.Source as BitmapImage;
			if (bmp != null)
			{
				var zoom = (int)(width / bmp.Width * 100);
				StatusText.Text = $"{zoom}%";
			}
		}

		private void LoadImage(string filePath)
		{
			if (!string.IsNullOrEmpty(filePath) && File.Exists(filePath))
			{
				try
				{
					// create the bitmap from a memory stream to avoid file locks
					byte[] buffer = File.ReadAllBytes(filePath);
					var ms = new MemoryStream(buffer);
					var image = new BitmapImage();
					image.BeginInit();
					image.CacheOption = BitmapCacheOption.OnLoad;
					image.StreamSource = ms;
					image.EndInit();
					image.Freeze();
					// update the image and watcher
					ImageBox.Source = image;
					UpdateStatus();
					UpdateWatcher(filePath);
				}
				catch
				{
					StatusText.Text = "Error Loading File";
				}
			}
		}

		private void ImageDrop(object sender, System.Windows.DragEventArgs e)
		{
			if (e.Data.GetDataPresent(System.Windows.DataFormats.FileDrop))
			{
				string[] files = e.Data.GetData(System.Windows.DataFormats.FileDrop) as string[];

				if (files != null && files.Length > 0)
				{
					LoadImage(files[0]);
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
			}
		}

		private void FitButtonClick(object sender, RoutedEventArgs e)
		{
			if (ImageBox.Source == null)
				return;

			if (ImageBox.Stretch == System.Windows.Media.Stretch.None)
			{
				ImageBox.Stretch = System.Windows.Media.Stretch.Uniform;
				FitButton.Content = "\ue98a";
				SizeToContent = SizeToContent.Manual;
			}
			else
			{
				ImageBox.Stretch = System.Windows.Media.Stretch.None;
				FitButton.Content = "\ue989";
				SizeToContent = SizeToContent.WidthAndHeight;
			}
		}

		private void ImageLayoutUpdated(object sender, EventArgs e)
		{
			UpdateStatus();
		}
	}
}