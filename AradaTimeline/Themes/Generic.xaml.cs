using System;
using System.Drawing;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

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
            VideoStateAxisControl._axisCanvasMarker.Children.Remove(SelectedPath);
            var index = Array.FindIndex(VideoStateAxisControl.Markers, val => val.Name == SelectedPath.Name);
            VideoStateAxisControl.Markers[index] = null;
            SaveBtn.Visibility = Visibility.Hidden;
        }

        private void setStartingItm_Checked(object sender, RoutedEventArgs e)
        {
           var index= Array.FindIndex(VideoStateAxisControl.Markers,val=>val.Name== SelectedPath.Name);
            if(index!=-1)
            {
                if (index == 1)
                    VideoStateAxisControl.Markers[index - 1].IsStarting = false;
                else
                {
                    if(VideoStateAxisControl.Markers[1]!=null)
                        VideoStateAxisControl.Markers[index + 1].IsStarting = false;
                }
                VideoStateAxisControl.Markers[index].IsStarting = true;
            }
        }

        private void setStartingItm_Unchecked(object sender, RoutedEventArgs e)
        {
            var index = Array.FindIndex(VideoStateAxisControl.Markers, val => val.Name == SelectedPath.Name);
            if (index != -1)
            {
                VideoStateAxisControl.Markers[index].IsStarting = false;
            }
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

    }
}