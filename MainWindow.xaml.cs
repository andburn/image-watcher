using System;
using System.Windows.Media.Imaging;
using System.Windows.Forms;
using System.Windows;

namespace ImageWatcher
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		public MainWindow()
		{
			InitializeComponent();
		}

		private void Button_Click(object sender, RoutedEventArgs e)
		{
			OpenFileDialog dialog = new OpenFileDialog();

			dialog.InitialDirectory = @"C:\";
			dialog.Filter = "Image files(*.bmp;*.jpg;*.png)|*.bmp;*.jpg;*.png|All files (*.*)|*.*";
			dialog.FilterIndex = 2;
			//dialog.RestoreDirectory = true;
			dialog.Multiselect = false;

			if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
			{
				imagebox.Source = new BitmapImage(new Uri(dialog.FileName));
			}
		}
	}
}