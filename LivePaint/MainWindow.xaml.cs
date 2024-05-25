using LivePaint.Models;
using Microsoft.Win32;
using System.ComponentModel;
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
using Xceed.Wpf.Toolkit;


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

        private DrawingAttributes highLighterAtt = new()
        {
            Color = Colors.Yellow,
            Height = 10,
            Width = 2,
            IsHighlighter = true,
            StylusTip = StylusTip.Rectangle
        };

        public List<DrawModel> drawModels = new List<DrawModel>();
        public enum Controls
        {
            Pen, HighLighter, Eraser
        }
        public MainWindow()
        {
            InitializeComponent();

            Canvas.DefaultDrawingAttributes = penAtt;

            Canvas.StrokeCollected += StrokeCollected;
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

            if (!string.IsNullOrWhiteSpace(fileName))
            {
                using (FileStream fileStream = new FileStream(filePath, FileMode.Create))
                {
                    pngBitmapEncoder.Save(fileStream);
                    System.Windows.MessageBox.Show("File saved successfully", "SUCCESS");
                } 
            }
            else
            {
                return;
            }
        }

        private void StrokeCollected(object sender, InkCanvasStrokeCollectedEventArgs e)
        {
            // Extract stroke information 
            Stroke stroke = e.Stroke;
            System.Windows.Point startPoint = stroke.StylusPoints[0].ToPoint();
            System.Windows.Point endPoint = stroke.StylusPoints[stroke.StylusPoints.Count - 1].ToPoint();
            System.Windows.Media.Color myColor = stroke.DrawingAttributes.Color;
            System.Drawing.Color convertedColor = System.Drawing.Color.FromArgb(myColor.A, myColor.R, myColor.G, myColor.B);

            DrawModel d = new DrawModel()
            {
                StartX = startPoint.X,
                StartY = startPoint.Y,
                EndX = endPoint.X,
                EndY = endPoint.Y,
                ColorCode = convertedColor.ToArgb(),
                Thickness = stroke.DrawingAttributes.Width
            };
            drawModels.Add(d);    
        }
    }
   
}   