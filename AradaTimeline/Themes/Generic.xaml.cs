using System;
using System.Drawing;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using Rectangle = System.Windows.Shapes.Rectangle;

namespace AradaTimeline
{
    public partial class Generic:ResourceDictionary
    {
        private Path SelectedPath { get; set; }
        public static Path SaveBtn { get; set; }
        private void Path_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            Path path = (Path)sender;
            path.Data = Geometry.Parse("M18.088,16.035C12,16,14,22,12,24c-2-2,0-8-6.088-7.965c-0.516,0.003-0.913-0.471-0.913-0.987 c-0.001-2.54,0-9.459,0.001-12.05C5,2.445,5.448,2,6,2h12c0.552,0,1,0.445,1,0.997c0,2.591,0.001,9.51,0.001,12.05 C19.001,15.564,18.604,16.037,18.088,16.035z");
        }
        public static string markedName = "Marker0";
        private void Path_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            Path path = (Path)sender;
            path.Data = Geometry.Parse("M17,4c0,2.69,0.001,7.441,0.001,10.086C14.13,14.387,12.74,15.951,12,17.611c-0.74-1.66-2.13-3.224-5.001-3.525 C6.999,11.442,6.999,6.69,7,4H17 M18,2H6C5.448,2,5,2.445,5,2.997c0,2.591-0.001,9.51-0.001,12.05c0,0.515,0.394,0.987,0.908,0.987 c0.002,0,0.003,0,0.005,0c0.017,0,0.035,0,0.052,0c6.024,0,4.041,5.971,6.036,7.966c1.994-1.994,0.01-7.966,6.036-7.966 c0.017,0,0.035,0,0.052,0c0.002,0,0.003,0,0.005,0c0.514,0,0.908-0.472,0.908-0.987c0.001-2.54,0-9.459-0.001-12.05 C19,2.445,18.552,2,18,2L18,2z");

        }

        private void Path_MouseRightButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            SelectedPath = (Path)sender;
            var marker = (ContextMenu)this["MarkerMenu"];
             marker.PlacementTarget = (Path)sender;
             marker.IsOpen = true;
        }
        private void removeItm_Click(object sender, RoutedEventArgs e)
        {

            int index = VideoStateAxisControl.Markers.FindIndex(val => val.Name == SelectedPath.Name);
            if (VideoStateAxisControl.Markers[0] != null)
            {
                VideoStateAxisControl._axisCanvasMarker.Children.Clear();
                VideoStateAxisControl._markerLine.Children.Clear();
                VideoStateAxisControl.Markers.Clear();
            }
            SaveBtn.Visibility = Visibility.Hidden;
        }

        private void joinBtn_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            VideoStateAxisControl.VideoControl.InvokeJoinButton();
        }

        private void joinBtn_Initialized(object sender, EventArgs e)
        {
            SaveBtn =(Path)sender;
            SaveBtn.Visibility = Visibility.Hidden;
        }

        private void joinBtn_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            SaveBtn.Fill = (Brush)(new BrushConverter().ConvertFrom("#FFFFFFFF"));
        }

        private void joinBtn_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
           SaveBtn.Fill=  (Brush)(new BrushConverter().ConvertFrom("#c8c7c3"));
        
        }
        private void Z_Parid__axisCanvasTimeText_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if(VideoStateAxisControl.VideoControl!=null)
            {
                VideoStateAxisControl.VideoControl._currentTime.Visibility = Visibility.Visible;
                VideoStateAxisControl.VideoControl._timePoint.CaptureMouse();
            }
        }

        private void Z_Parid__axisCanvasTimeText_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            VideoStateAxisControl.VideoControl._currentTime.Visibility = Visibility.Collapsed;
        }

        private void Line_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            VideoStateAxisControl.VideoControl._currentTime.Visibility = Visibility.Visible;
            VideoStateAxisControl.VideoControl._timePoint.CaptureMouse();
        }

        private void Line_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            VideoStateAxisControl.VideoControl._currentTime.Visibility = Visibility.Collapsed;
        }

        private void Path_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if(e.ClickCount>=2)
            {
                SelectedPath = (Path)sender;
                int index = 0;
                if (VideoStateAxisControl.Markers[0] != null)
                {
                    index = VideoStateAxisControl.Markers.FindIndex(val => val.Name == SelectedPath.Name);
                    VideoStateAxisControl._markerLine.Children.RemoveAt(index);
                }
                else
                {
                    index = 1;
                    VideoStateAxisControl._markerLine.Children.RemoveAt(0);
                }
                VideoStateAxisControl.Markers[index] = null;
                SaveBtn.Visibility = Visibility.Hidden;
                VideoStateAxisControl._axisCanvasMarker.Children.Remove(SelectedPath);
            }
        }
    }
}