using LivePaint.Server.Models;
using Microsoft.AspNetCore.SignalR.Client;
using System.ComponentModel;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Xceed.Wpf.Toolkit;

namespace LivePaint
{
    public partial class MainWindow : Window
    {
        private readonly DrawingAttributes penAtt = new()
        {
            Color = Colors.Black,
            Height = 2,
            Width = 2
        };

        private readonly DrawingAttributes highLighterAtt = new()
        {
            Color = Colors.Yellow,
            Height = 10,
            Width = 2,
            IsHighlighter = true,
        };

        private Point lastPoint;
        private HubConnection hubConnection;
        public enum Controls
        {
            Pen, HighLighter, Eraser
        }

        public MainWindow()
        {
            InitializeComponent();
            InitializeSignalR();
            Canvas.DefaultDrawingAttributes = penAtt;

            Canvas.StrokeCollected += Canvas_StrokeCollected;
            Canvas.MouseMove += Canvas_MouseMove;
            Canvas.StylusDown += Canvas_StylusDown;
        }

        private async void InitializeSignalR()
        {
            hubConnection = new HubConnectionBuilder()
                .WithUrl("https://localhost:7223/DrawHub")
                .WithAutomaticReconnect()
                .Build();

            hubConnection.On<DrawModel>("ReceiveDrawing", (drawModel) =>
            {
                Application.Current.Dispatcher.Invoke(() => UpdateCanvas(drawModel));
            });

            await hubConnection.StartAsync();
        }

        private async void SendDrawing(DrawModel drawModel)
        {
            await hubConnection.InvokeAsync("SendDrawing", drawModel);
        }

        private void Canvas_StylusDown(object sender, StylusDownEventArgs e)
        {
            lastPoint = e.GetPosition(Canvas);
        }

        private void Canvas_StrokeCollected(object sender, InkCanvasStrokeCollectedEventArgs e)
        {
            var stroke = e.Stroke;
            var points = stroke.StylusPoints;

            if (points.Count < 2) return;

            var drawModel = new DrawModel
            {
                startX = points[0].X,
                startY = points[0].Y,
                currX = points[points.Count - 1].X,
                currY = points[points.Count - 1].Y,
                color = stroke.DrawingAttributes.Color.ToString(),
                thickness = stroke.DrawingAttributes.Width
            };

            SendDrawing(drawModel);
        }

        private void Canvas_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                var position = e.GetPosition(Canvas);
                var drawModel = new DrawModel
                {
                    startX = lastPoint.X,
                    startY = lastPoint.Y,
                    currX = position.X,
                    currY = position.Y,
                    color = penAtt.Color.ToString(),
                    thickness = penAtt.Width
                };

                SendDrawing(drawModel);

                lastPoint = position; // Update lastPoint to current position for next segment
            }
        }

        private void UpdateCanvas(DrawModel drawModel)
        {
            var startPoint = new StylusPoint(drawModel.startX, drawModel.startY);
            var endPoint = new StylusPoint(drawModel.currX, drawModel.currY);
            var points = new StylusPointCollection { startPoint, endPoint };
            var stroke = new Stroke(points)
            {
                DrawingAttributes = new DrawingAttributes
                {
                    Color = (Color)ColorConverter.ConvertFromString(drawModel.color),
                    Width = drawModel.thickness,
                    Height = drawModel.thickness
                }
            };

            Canvas.Strokes.Add(stroke);
        }

        private void PenBtn_Click(object sender, RoutedEventArgs e)
        {
            setControl(Controls.Pen);
        }

        private void HighlighterBtn_Click(object sender, RoutedEventArgs e)
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

        private void YellowRadio_Click(object sender, RoutedEventArgs e)
        {
            highLighterAtt.Color = Colors.Yellow;
        }

        private void CyanRadio_Click(object sender, RoutedEventArgs e)
        {
            highLighterAtt.Color = Colors.Cyan;
        }

        private void MagentaRadio_Click(object sender, RoutedEventArgs e)
        {
            highLighterAtt.Color = Colors.Magenta;
        }

        private void PartialStrokeRadio_Click(object sender, RoutedEventArgs e)
        {
            if (EraserBtn.IsChecked == true)
            {
                Canvas.EditingMode = InkCanvasEditingMode.EraseByPoint;
            }
        }

        private void FullStrokeRadio_Click(object sender, RoutedEventArgs e)
        {
            if (EraserBtn.IsChecked == true)
            {
                Canvas.EditingMode = InkCanvasEditingMode.EraseByStroke;
            }
        }

        private void SaveBtn_Click(object sender, RoutedEventArgs e)
        {
            string folderPath = @"C:\Practice Projects\LivePaint\LivePaint\SavedImages";
            string fileName = Microsoft.VisualBasic.Interaction.InputBox("Enter the file name:", "Save Image", "image.png");
            string filePath = System.IO.Path.Combine(folderPath, fileName);

            RenderTargetBitmap renderTarget = new RenderTargetBitmap((int)Canvas.ActualWidth, (int)Canvas.ActualHeight, 96d, 96d, PixelFormats.Pbgra32);
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
        }
    }
}
