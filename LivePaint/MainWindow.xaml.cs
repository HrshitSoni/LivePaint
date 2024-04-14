using Microsoft.Win32;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace LivePaint
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private DrawingAttributes penAtt = new()
        {
            Color = Colors.Black,
            Height = 2,
            Width = 2
        };

        private  DrawingAttributes highLighterAtt = new()
        {
            Color = Colors.Yellow,
            Height = 10,
            Width = 2,
            IsHighlighter = true,
            StylusTip = StylusTip.Rectangle
        };

        public enum Controls
        {
           Pen,HighLighter,Eraser
        }
        public MainWindow()
        {
            InitializeComponent();
            Canvas.DefaultDrawingAttributes = penAtt;
        }

        private void PenBtn_Click(object sender , RoutedEventArgs e)
        {
            setControl(Controls.Pen);
        }

        private void HighlighterBtn_Click(object sender,RoutedEventArgs e)
        {
            setControl(Controls.HighLighter);
        }

        private void EraserBtn_Click(object sender, RoutedEventArgs e)
        {
            setControl(Controls.Eraser);
        }

        private void setControl(Controls control)
        {
            
            PenBtn.IsChecked = false;
            HighlighterBtn.IsChecked = false;
            EraserBtn.IsChecked = false;

            switch (control)
            {
                case Controls.Pen:
                    PenBtn.IsChecked = true;
                    Canvas.EditingMode = InkCanvasEditingMode.Ink;
                    Canvas.DefaultDrawingAttributes = penAtt;
                    break;
                case Controls.HighLighter:
                    HighlighterBtn.IsChecked = true;
                    Canvas.EditingMode = InkCanvasEditingMode.Ink;
                    Canvas.DefaultDrawingAttributes = highLighterAtt;
                    break;
                case Controls.Eraser:
                    EraserBtn.IsChecked = true;
                    if (PartialStrokeRadio.IsChecked == true)
                    {
                        Canvas.EditingMode = InkCanvasEditingMode.EraseByPoint;
                    }
                    else
                    {
                        Canvas.EditingMode = InkCanvasEditingMode.EraseByStroke;
                    }
                    break;
                   
            }
        }

        private void PenColorPicker_SelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<System.Windows.Media.Color?> e)
        {
            if (IsLoaded)
                penAtt.Color = PenColorPicker.SelectedColor ?? Colors.Black;
        }

        private void ThicknessSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            penAtt.Height = ThicknessSlider.Value;
            penAtt.Width = ThicknessSlider.Value;
        }

        private void YellowRadio_Click(object sender , RoutedEventArgs e)
        {
            highLighterAtt.Color = Colors.Yellow;
        }

        private void CyanRadio_Click(object sender , RoutedEventArgs e)
        {
            highLighterAtt.Color = Colors.Cyan;
        }

        private void MagentaRadio_Click(object sender , RoutedEventArgs e)
        {
            highLighterAtt.Color = Colors.Magenta;
        }

        private void PartialStrokeRadio_Click(object sender,RoutedEventArgs e)
        {
            if(EraserBtn.IsChecked == true)
            {
                Canvas.EditingMode = InkCanvasEditingMode.EraseByPoint;
            }
        }

        private void FullStrokeRadio_Click(object sender,RoutedEventArgs e)
        {
            if(EraserBtn.IsChecked == true)
            {
                Canvas.EditingMode = InkCanvasEditingMode.EraseByStroke;
            }
        }

        private void SaveBtn_Click(object sender, RoutedEventArgs e)
        {
            string folderPath = @"C:\Practice Projects\LivePaint\LivePaint\SavedImages";
            string fileName = Microsoft.VisualBasic.Interaction.InputBox("Enter the file name:", "Save Image", "image.png");
            string filePath = System.IO.Path.Combine(folderPath, fileName);

            RenderTargetBitmap renderTarget = new RenderTargetBitmap((int)Canvas.ActualHeight, (int)Canvas.ActualWidth, 96d, 96d, PixelFormats.Pbgra32);
            renderTarget.Render(Canvas);

            PngBitmapEncoder pngBitmapEncoder = new PngBitmapEncoder();
            pngBitmapEncoder.Frames.Add(BitmapFrame.Create(renderTarget));



            using (FileStream fileStream = new FileStream(filePath, FileMode.Create))
            {
                pngBitmapEncoder.Save(fileStream);
                MessageBox.Show("File saved successfully,SUCCESS");
            }

        }

        private void OpenBtn_Click(object sender,RoutedEventArgs e)
        {
                OpenFileDialog openFileDialog = new OpenFileDialog();
                openFileDialog.InitialDirectory = @"C:\Practice Projects\LivePaint\LivePaint\SavedImages";
                openFileDialog.Filter = "All Files (*.*)|*.*";
                openFileDialog.FilterIndex = 1;
                openFileDialog.Multiselect = false;

                if (openFileDialog.ShowDialog() == true)
                {
                    // Get the selected file path
                    string filePath = openFileDialog.FileName;

                    // Create a bitmap from the selected file
                    BitmapImage bitmapImage = new BitmapImage();
                    bitmapImage.BeginInit();
                    bitmapImage.UriSource = new Uri(filePath);
                    bitmapImage.EndInit();

                    // Display the bitmap on the canvas
                    Image image = new Image();
                    image.Source = bitmapImage;
                    Canvas.Children.Add(image);
                }
           
        }
    } 
}   