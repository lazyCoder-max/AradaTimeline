﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace AradaTimeline
{
    #region TemplatePart 模板元素声明

    [TemplatePart(Name = Parid_axisCanvas)]
    [TemplatePart(Name = Parid_timePoint)]
    [TemplatePart(Name = Parid_currentTime)]
    [TemplatePart(Name = Parid_timePanel)]
    [TemplatePart(Name = Parid_timeLine)]
    [TemplatePart(Name = Parid_scrollViewer)]
    [TemplatePart(Name = Parid_videoHistoryPanel)]
    [TemplatePart(Name = Parid__axisCanvasTimeText)]
    [TemplatePart(Name = Parid__axisCanvasMarker)]
    [TemplatePart(Name = Parid_zoomSlider)]
    [TemplatePart(Name = Parid_clipCanvas)]
    [TemplatePart(Name = Parid_clipStartBorder)]
    [TemplatePart(Name = Parid_clipAreaBorder)]
    [TemplatePart(Name = Parid_clipEndBorder)]
    [TemplatePart(Name = Parid_clip)]
    [TemplatePart(Name = Parid_clipOff)]
    [TemplatePart(Name = Parid_clipStateTimeTextBlock)]
    [TemplatePart(Name = Parid_clipEndTimeTextBlock)]
    [TemplatePart(Name = Parid_cameraListBox)]
    [TemplatePart(Name = Parid_downButtonListBox)]

    #endregion
    public class VideoStateAxisControl : Control
    {
        #region UIElement
        private ScrollViewer _scrollViewer;                 //Scroll view
        private Canvas _axisCanvas;                          //Time scale container
        private Canvas _axisCanvasTimeText;             //Timescale time text container
        public static Canvas _axisCanvasMarker;             //Marker container
        private Slider _zoomSlider;                          //Zoom timeline slider

        private TextBlock _currentTime;                    //Progress pointer time
        private Canvas _timePanel;                           //Progress container
        private Canvas _timeLine;                             //Progress pointer container
        private Grid _timePoint;                                //Progress pointer

        private Canvas _clipCanvas;                         //Clip control moving container
        private StackPanel _clip;              //Clip slider container

        private TextBlock _clipStateTimeTextBlock;     //Clip start time indicator
        private TextBlock _clipEndTimeTextBlock;      //Clip end time indicator

        private ListBox _cameraListBox;                    //Camera list
        private ListBox _downButtonListBox;             //Download list

        #endregion

        #region ConstString Template element name, Geometry image data

        private const string Parid_axisCanvas = "Z_Parid_axisCanvas";
        private const string Parid__axisCanvasTimeText = "Z_Parid__axisCanvasTimeText";
        private const string Parid__axisCanvasMarker = "Z_Parid__axisCanvasMarker";
        private const string Parid_timePoint = "Z_Parid_timePoint";
        private const string Parid_currentTime = "Z_Parid_currentTime";
        private const string Parid_timePanel = "Z_Parid_timePanel";
        private const string Parid_timeLine = "Z_Parid_timeLine";
        private const string Parid_scrollViewer = "Z_Parid_scrollViewer";
        private const string Parid_videoHistoryPanel = "Z_videoHistoryPanel";
        private const string Parid_zoomSlider = "Z_Parid_zoomSlider";
        private const string Parid_clipCanvas = "Z_Parid_clipCanvas";
        private const string Parid_clipStartBorder = "Z_Parid_clipStartBorder";
        private const string Parid_clipAreaBorder = "Z_Parid_clipAreaBorder";
        private const string Parid_clipEndBorder = "Z_Parid_clipEndBorder";
        private const string Parid_clip = "Z_Parid_clip";
        private const string Parid_clipOff = "Z_Parid_clipOff";
        private const string Parid_clipStateTimeTextBlock = "Z_Parid_clipStateTimeTextBlock";
        private const string Parid_clipEndTimeTextBlock = "Z_Parid_clipEndTimeTextBlock";
        private const string Parid_cameraListBox = "Z_Parid_cameraListBox";
        private const string Parid_downButtonListBox = "Z_Parid_downButtonListBox";
        private const string GeometryMarker = "M17,4c0,2.69,0.001,7.441,0.001,10.086C14.13,14.387,12.74,15.951,12,17.611c-0.74-1.66-2.13-3.224-5.001-3.525 C6.999,11.442,6.999,6.69,7,4H17 M18,2H6C5.448,2,5,2.445,5,2.997c0,2.591-0.001,9.51-0.001,12.05c0,0.515,0.394,0.987,0.908,0.987 c0.002,0,0.003,0,0.005,0c0.017,0,0.035,0,0.052,0c6.024,0,4.041,5.971,6.036,7.966c1.994-1.994,0.01-7.966,6.036-7.966 c0.017,0,0.035,0,0.052,0c0.002,0,0.003,0,0.005,0c0.514,0,0.908-0.472,0.908-0.987c0.001-2.54,0-9.459-0.001-12.05 C19,2.445,18.552,2,18,2L18,2z";

        #endregion

        #region DependencyProperty 

        // Using a DependencyProperty as the backing store for ScrollValue.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ScrollValueProperty =
            DependencyProperty.Register("ScrollValue", typeof(double), typeof(VideoStateAxisControl), new PropertyMetadata(12.0));
        public static readonly DependencyProperty StartTimeProperty = DependencyProperty.Register(
            "StartTime",
            typeof(TimeSpan),
            typeof(VideoStateAxisControl),
            new PropertyMetadata(OnTimeChanged));

        public static readonly DependencyProperty EndTimeProperty = DependencyProperty.Register(
            "EndTime",
            typeof(TimeSpan),
            typeof(VideoStateAxisControl),
            new PropertyMetadata(OnTimeChanged));

        public static readonly DependencyProperty AxisTimeProperty = DependencyProperty.Register(
            "AxisTime",
            typeof(TimeSpan),
            typeof(VideoStateAxisControl),
            new PropertyMetadata(OnAxisTimeChanged));
        #endregion

        #region Property 
        internal static VideoStateAxisControl VideoControl { get; set; }
        public Visibility SaveBtnVisibility { get; set; } = Visibility.Hidden;
        /// <summary>
        /// Get Markers Information
        /// </summary>
        public static Marker[] Markers { get; internal set; }
        public double ScrollValue
        {
            get { return (double)_zoomSlider.Value; }
            set { SetValue(ScrollValueProperty, value); }
        }
        /// <summary>
        /// Get the Seek Marker's current location 
        /// </summary>
        public double GetSeekMarkerPoint { get; private set; }
        private VideoEventArgs EventArgs { get; set; }
        /// <summary>
        /// Returns the drawer
        /// </summary>
        private DrawarType Drawar { get; set; }
        /// <summary>
        /// Search history video start time
        /// </summary>
        public TimeSpan StartTime
        {
            get { return (TimeSpan)GetValue(StartTimeProperty); }
            set { SetValue(StartTimeProperty, value); }
        }

        /// <summary>
        /// Search historical video end time
        /// </summary>
        public TimeSpan EndTime
        {
            get { return (TimeSpan)GetValue(EndTimeProperty); }
            set { SetValue(EndTimeProperty, value); }
        }
        /// <summary>
        /// Pointer time
        /// </summary>
        public TimeSpan AxisTime
        {
            get { return (TimeSpan)GetValue(AxisTimeProperty); }
            set { SetValue(AxisTimeProperty, value); }
        }
        /// <summary>
        /// Width of the time axis occupied per hour
        /// </summary>
        private double Dial_Cell_H
        {
            get { return _scrollViewer == null ? 0 : ((_scrollViewer.ActualWidth - 10) * Slider_Magnification) / EventArgs.Duration.TotalHours; }
        }
        private double Dial_Cell_M2
        {
            get { return _scrollViewer == null ? 0 : ((_scrollViewer.ActualWidth - 10) * Slider_Magnification) / 1444; }
        }

        /// <summary>
        /// The width of the time axis occupied per minute
        /// </summary>
        private double Dial_Cell_M
        {
            get { return Dial_Cell_H / 60; }
        }

        /// <summary>
        /// The width of the time axis per second
        /// </summary>
        private double Dial_Cell_S
        {
            get { return Dial_Cell_M / 60; }
        }
        /// <summary>
        /// The width of the time axis per milli-second
        /// </summary>
        private double Dial_Cell_MiS
        {
            get { return Dial_Cell_S / 1000; }
        }

        /// <summary>
        /// Time axis zoom ratio
        /// </summary>
        private double Slider_Magnification = 1;
        /// <summary>
        /// Slider Maximum Zoom
        /// </summary>
        public double ScrollMaximum { get; set; } = 60;
        /// <summary>
        /// Slider Current Zoom
        /// </summary>
        public TimeSpan VideoDuration { get; set; }
        #endregion

        #region RouteEvent 
        public static readonly RoutedEvent AxisDownRoutedEvent = EventManager.RegisterRoutedEvent(
            "AxisDown",
            RoutingStrategy.Bubble,
            typeof(EventHandler<VideoStateAxisRoutedEventArgs>),
            typeof(VideoStateAxisControl));

        public static readonly RoutedEvent DragTimeLineRoutedEvent = EventManager.RegisterRoutedEvent(
            "DragTimeLine",
            RoutingStrategy.Bubble,
            typeof(EventHandler<VideoStateAxisRoutedEventArgs>),
            typeof(VideoStateAxisControl));

        public event RoutedEventHandler AxisDown
        {
            add { this.AddHandler(AxisDownRoutedEvent, value); }
            remove { this.RemoveHandler(AxisDownRoutedEvent, value); }
        }
        /// <summary>
        /// Download routing events
        /// </summary>
        /// <summary>
        /// Pointer drag event
        /// </summary>
        public event RoutedEventHandler DragTimeLine
        {
            add { this.AddHandler(DragTimeLineRoutedEvent, value); }
            remove { this.RemoveHandler(DragTimeLineRoutedEvent, value); }
        }

        #endregion

        #region Method 
        internal void InvokeJoinButton()
        {
            OnJoinButtonClicked();
        }
        protected virtual void OnJoinButtonClicked()
        {
            JoinButtonClick?.Invoke(this, new MarkerEventArgs() { Markers = Markers });
        }
        public void LoadVideo(VideoEventArgs video)
        {
            VideoControl = this;
            VideoLoaded += VideoStateAxisControl_VideoLoaded;
            VideoLoaded?.Invoke(this, video);
        }
        private void VideoStateAxisControl_VideoLoaded(object sender, VideoEventArgs e)
        {
            EventArgs = e;
            RefreshTimeline(AssignDrawer(e.Duration),e);
        }

        /// <summary>
        /// Based on the video duration, it will assign line and text drawer
        /// </summary>
        /// <param name="timeSpan"></param>
        /// <returns name="DrawarType"></returns>
        private DrawarType AssignDrawer(TimeSpan timeSpan)
        {
            if(timeSpan.TotalHours>=1)
            {
                // Draw Hour
                Drawar = DrawarType.Hour;
            }
            else if(timeSpan.TotalMinutes>=1)
            {
                // Draw Minute
                Drawar = DrawarType.Minute;
            }
            else if (timeSpan.TotalSeconds >= 1)
            {
                // Draw Second
                Drawar = DrawarType.Second;
            }
            else if (timeSpan.TotalSeconds >= 1)
            {
                // Draw Millisecond
                Drawar = DrawarType.Millisecond;
            }
            else
            {
                Drawar = DrawarType.Unknown;
            }
            return Drawar;
        }
        /// <summary>
        /// Based on the video duration and zoom value, it will assign line and text drawer
        /// </summary>
        /// <returns></returns>
        
        /// <summary>
        /// Redraw the timeline
        /// </summary>
        public void RefreshTimeline()
        {
            Slider_Magnification = Math.Round(ScrollValue, 2);
            _zoomSlider.Value = ScrollValue;
            InitializeAxis();
        }
        private void RefreshTimeline(DrawarType drawarType,VideoEventArgs e)
        {
            switch (drawarType)
            {
                case DrawarType.Hour:
                    DrawHourText(Convert.ToInt32(e.Duration.TotalHours));
                    DrawHourLines(Convert.ToInt32(e.Duration.TotalHours));
                    break;
                case DrawarType.Minute:
                    DrawMinuteText(Convert.ToInt32(e.Duration.TotalMinutes));
                    DrawMinuiteLines(Convert.ToInt32(e.Duration.TotalMinutes));
                    break;
                case DrawarType.Second:
                    DrawSecondText(Convert.ToInt32(e.Duration.TotalSeconds));
                    DrawSecondLines(Convert.ToInt32(e.Duration.TotalSeconds));
                    break;
                case DrawarType.Millisecond:
                    DrawMillisecondText(Convert.ToInt32(e.Duration.TotalMilliseconds));
                    DrawMillisecodLines(Convert.ToInt32(e.Duration.TotalMilliseconds));
                    break;
            }
        }

        /// <summary>
        /// Historical query time-change
        /// </summary>
        /// <param name="d"></param>
        /// <param name="e"></param>
        private static void OnTimeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            VideoStateAxisControl AxisOb = d as VideoStateAxisControl;
            if (AxisOb != null)
            {
                AxisOb.InitializeAxis();
            }
        }
        /// <summary>
        /// Pointer time refresh pointer position
        /// </summary>
        /// <param name="d"></param>
        /// <param name="e"></param>
        private static void OnAxisTimeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            VideoStateAxisControl AxisOb = d as VideoStateAxisControl;
            if (AxisOb != null && e.NewValue != e.OldValue)
            {
                AxisOb.RefreshTimeLineLeft((TimeSpan)e.NewValue);
            }
        }

        /// <summary>
        /// The constructor initializes some properties and styles
        /// </summary>
        public VideoStateAxisControl()
        {
            Loaded += delegate
            {
                InitializeAxis();
                SizeChanged += delegate
                {
                    InitializeAxis();
                };
            };
        }

        /// <summary>
        /// Refresh pointer position
        /// </summary>
        /// <param name="dt"></param>
        private void RefreshTimeLineLeft(TimeSpan dt)
        {
            TimeSpan ts = dt - StartTime;
            if (_timeLine != null)
            {
                Canvas.SetLeft(_timeLine,
                    Dial_Cell_H * (ts.Days == 1 ? 23 : dt.Hours) +
                    Dial_Cell_M * (ts.Days == 1 ? 59 : dt.Minutes) +
                    Dial_Cell_S * (ts.Days == 1 ? 59 : dt.Seconds) +
                    Dial_Cell_MiS * (ts.Days == 1 ? 999 : dt.Milliseconds));
                _currentTime.Text = dt.ToString();
            }
        }
        /// <summary>
        /// Time zoom slider event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _zoomSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            Slider_Magnification = Math.Round(e.NewValue, 2);
            if(EventArgs==null)
                InitializeAxis();
            else
            {
                RefreshTimeline(Drawar, EventArgs);
                InitializationNewtTimeLine();
            }
        }

        /// <summary>
        /// Scroll to reset the timescale position
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void scrollViewer_Changed(object sender, ScrollChangedEventArgs e)
        {
            _timeLine.Margin = new Thickness(0, _scrollViewer.VerticalOffset, 0, 0);
            _axisCanvasTimeText.Margin = new Thickness(0, _scrollViewer.VerticalOffset, 0, 0);
            _axisCanvas.Margin = new Thickness(0, _scrollViewer.VerticalOffset, 0, 0);
            _clipCanvas.Margin = new Thickness(0, _scrollViewer.VerticalOffset, 0, 0);
        }


        /// <summary>
        ///   Pointer movement
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="s"></param>
        private void timePoint_MouseMove(object sender, MouseEventArgs s)
        {
            if (Mouse.LeftButton == MouseButtonState.Pressed)
            {
                double delta = s.GetPosition(_timePanel).X;
                double timePointMaxLeft = _timePanel.ActualWidth - _timePoint.ActualWidth;
                Canvas.SetLeft(_timeLine, delta = delta < 0 ? 0 : (delta > timePointMaxLeft ? timePointMaxLeft : delta));
                TimeLine_Resver(delta);
            }
        }

        /// <summary>
        /// Refresh time indicator coordinate position
        /// </summary>
        /// <param name="delta">The mouse is at Canvas coordinate X</param>
        private void TimeLine_Resver(double delta)
        {
            double timePointMaxLeft = _timePanel.ActualWidth - _timePoint.ActualWidth;
            double currentTimeMaxLeft = _timePanel.ActualWidth - _currentTime.ActualWidth;
            _currentTime.Text = (AxisTime = XToTimeSpan(delta < 0 ? 0 : (delta > timePointMaxLeft ? timePointMaxLeft : delta))).ToString();
            _currentTime.Margin = delta < currentTimeMaxLeft ?
                new Thickness(delta < 0 ? 10 : delta + 10, 2, 0, 0) :
                new Thickness(delta > timePointMaxLeft ? timePointMaxLeft - _currentTime.ActualWidth : delta - _currentTime.ActualWidth, 2, 0, 0);
            GetSeekMarkerPoint= _currentTime.Margin.Left;
        }
        /// <summary>
        /// Pointer down
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="s"></param>
        private void timePoint_MouseLeftButtonDown(object sender, MouseButtonEventArgs s)
        {
            _currentTime.Visibility = Visibility.Visible;
            _timePoint.CaptureMouse();
        }

        /// <summary>
        /// Post pointer drag routing event
        /// </summary>
        private void SendDragTimeLineRoutedEvent()
        {
            VideoStateAxisRoutedEventArgs args = new VideoStateAxisRoutedEventArgs(DragTimeLineRoutedEvent, this)
            {
                TimeLine = AxisTime
            };
            this.RaiseEvent(args);
        }
        /// <summary>
        /// Pointer bounce
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="s"></param>
        private void timePoint_MouseLeftButtonUp(object sender, MouseButtonEventArgs s)
        {
            _currentTime.Visibility = Visibility.Collapsed;
            _timePoint.ReleaseMouseCapture();
            SendDragTimeLineRoutedEvent();
        }

        /// <summary>
        /// Calculate the display time of the pointer drag
        /// </summary>
        /// <param name="x">The Left coordinate value of the pointer in the Canvas container</param>
        private TimeSpan XToTimeSpan(double point_x)
        {
            TimeSpan dt = StartTime;
            double time;
            int H, M, S,MS;
            time = point_x / Dial_Cell_H;
            H = (int)(time);
            time = (time - H) * 60;
            M = (int)(time);
            time = (time - M) * 60;
            S = (int)time;
            MS = (int)((time - S) * 1000);
            return dt.Add(new TimeSpan(0,H,M,S,MS));
        }

        /// <summary>
        /// initialization
        /// </summary>
        private void InitializeAxis()
        {
            AddTimeTextBlock();
            InitializationNewtTimeLine();
        }
        /// <summary>
        /// Initialize pointer position
        /// </summary>
        private void InitializationNewtTimeLine()
        {
            if (_timeLine != null && !double.IsNaN(Canvas.GetLeft(_timeLine)))
            {
                RefreshTimeLineLeft(AxisTime);
            }
        }

        /// <summary>
        /// Initialize timescale text
        /// </summary>
        /// <param name="HaveMathTextBlock">The amount of time text that needs to be filled</param>
        private void AddTimeTextBlock()
        {
            if (_axisCanvasTimeText != null)
            {
                _axisCanvasTimeText.Width = (_scrollViewer.ActualWidth - 10) * Slider_Magnification;
                _axisCanvasTimeText.Children.Clear();
                for (int i = 0; i < 24; i++)
                {
                    _axisCanvasTimeText.Children.Add((
                        new TextBlock()
                        {
                            Text = i.ToString().PadLeft(2, '0') + ":00:00:00",
                            Margin = new Thickness(Dial_Cell_H * i-30, 2, 0, 0)
                        }));
                }
            }
        }
        #region Drawers
        /// <summary>
        /// Initialize timescale text
        /// </summary>
        /// <param name="HaveMathTextBlock">The amount of time text that needs to be filled</param>
        private void DrawHourText(int totalDrawing)
        {
            if (_axisCanvasTimeText != null)
            {
                _axisCanvasTimeText.Width = (_scrollViewer.ActualWidth - 10) * Slider_Magnification;
                _axisCanvasTimeText.Children.Clear();
                var minute = (totalDrawing * 60);
                var minuteHalf = 30;
                for (int i = 0; i < totalDrawing; i++)
                {
                    _axisCanvasTimeText.Children.Add((
                        new TextBlock()
                        {
                            Text = i.ToString().PadLeft(2, '0') + ":00:00:00",
                            Margin = new Thickness(Dial_Cell_H * i - 30, 2, 0, 0)
                        }));
                    for (int j = i; j < minute; j++)
                    {
                        if (j == minuteHalf)
                        {
                            _axisCanvasTimeText.Children.Add((
                                new TextBlock()
                                {
                                    Text = $"{j.ToString("00")}",
                                    FontSize = 8,
                                    Margin = new Thickness(Dial_Cell_M * j - 10, 2, 0, 0)
                                }));
                            minuteHalf += 60;
                        }
                    }
                }
            }
        }
        private void DrawMinuteText(int totalDrawing)
        {
            if (_axisCanvasTimeText != null)
            {
                _axisCanvasTimeText.Width = (_scrollViewer.ActualWidth - 10) * Slider_Magnification;
                _axisCanvasTimeText.Children.Clear();
                var second = (totalDrawing * 60);
                var secondHalf = 30;
                for (int i = 0; i <= totalDrawing; i++)
                {
                    _axisCanvasTimeText.Children.Add((
                        new TextBlock()
                        {
                            Text = $"00:{i.ToString("00")}:00:00",
                            Margin = new Thickness(Dial_Cell_M * i - 30, 2, 0, 0)
                        }));
                    for (int j = i; j < second; j++)
                    {
                        if (j == secondHalf)
                        {
                            _axisCanvasTimeText.Children.Add((
                                new TextBlock()
                                {
                                    Text = $"{j.ToString("00")}",
                                    FontSize = 8,
                                    Margin = new Thickness(Dial_Cell_S * j - 10, 2, 0, 0)
                                }));
                            secondHalf += 60;
                        }
                    }
                }
            }
        }
        private void DrawSecondText(int totalDrawing)
        {
            if (_axisCanvasTimeText != null)
            {
                _axisCanvasTimeText.Width = (_scrollViewer.ActualWidth - 10) * Slider_Magnification;
                _axisCanvasTimeText.Children.Clear();
                var msecond = (totalDrawing * 1000);
                var msecondHalf = 500;
                for (int i = 0; i <= totalDrawing; i++)
                {
                    _axisCanvasTimeText.Children.Add((
                        new TextBlock()
                        {
                            Text = $"00:00:{i.ToString("00")}:00",
                            Margin = new Thickness(Dial_Cell_S * i - 30, 2, 0, 0)
                        }));
                    for (int j = i; j < msecond; j++)
                    {
                        if (j == msecondHalf)
                        {
                            _axisCanvasTimeText.Children.Add((
                                new TextBlock()
                                {
                                    Text = $"{j.ToString("00")}",
                                    FontSize = 8,
                                    Margin = new Thickness(Dial_Cell_MiS * j - 10, 2, 0, 0)
                                }));
                            msecondHalf += 1000;
                        }
                    }
                }
            }
        }
        private void DrawMillisecondText(int totalDrawing)
        {
            if (_axisCanvasTimeText != null)
            {
                _axisCanvasTimeText.Width = (_scrollViewer.ActualWidth - 10) * Slider_Magnification;
                _axisCanvasTimeText.Children.Clear();
                for (int i = 0; i <= totalDrawing; i++)
                {
                    _axisCanvasTimeText.Children.Add((
                        new TextBlock()
                        {
                            Text = $"00:00:00:{i.ToString("000")}",
                            Margin = new Thickness(Dial_Cell_MiS * i - 30, 2, 0, 0)
                        }));
                }
            }
        }
        /// <summary>
        /// Draws Hour Lines
        /// </summary>
        /// <param name="IsSmall"></param>
        private void DrawHourLines(int totalDrawing, bool IsSmall = false)
        {
            var y2 = 15;
            if (IsSmall)
                y2 = 15;
            else
                y2 = 25;
            _axisCanvas.Width = (_scrollViewer.ActualWidth - 10) * Slider_Magnification;
            _axisCanvas.Children.Clear();
            int minute = totalDrawing * 60;
            int minuteHalf = 30;
            for (int i = 0; i < totalDrawing; i++)
            {
                _axisCanvas.Children.Add(new Line()
                {
                    X1 = Dial_Cell_H * i,
                    Y1 = 0,
                    X2 = Dial_Cell_H * i,
                    Y2 = y2,
                    StrokeThickness = 1
                });
                for (int j = i; j < minute; j++)
                {
                    if (j == minuteHalf)
                    {
                        _axisCanvas.Children.Add(new Line()
                        {
                            X1 = Dial_Cell_M * j,
                            Y1 = 0,
                            X2 = Dial_Cell_M * j,
                            Y2 = 15,
                            StrokeThickness = 1
                        });
                        minuteHalf += 60;
                    }
                }
            }
        }
        /// <summary>
        /// Draws Second Lines
        /// </summary>
        /// <param name="IsSmall"></param>
        private void DrawSecondLines(int totalDrawing, bool IsSmall = false)
        {
            var y2 = 15;
            if (IsSmall)
                y2 = 15;
            else
                y2 = 25;
            _axisCanvas.Width = (_scrollViewer.ActualWidth - 10) * Slider_Magnification;
            _axisCanvas.Children.Clear();
            int mSecondHalf = 500;
            int mSecond = totalDrawing * 1000;
            for (int i = 0; i <= totalDrawing; i++)
            {
                _axisCanvas.Children.Add(new Line()
                {
                    X1 = Dial_Cell_S * i,
                    Y1 = 0,
                    X2 = Dial_Cell_S * i,
                    Y2 = y2,
                    StrokeThickness = 1
                });
                for (int j = i; j < mSecond; j++)
                {
                    if (j == mSecondHalf)
                    {
                        _axisCanvas.Children.Add(new Line()
                        {
                            X1 = Dial_Cell_MiS * j,
                            Y1 = 0,
                            X2 = Dial_Cell_MiS * j,
                            Y2 = 15,
                            StrokeThickness = 1
                        });
                        mSecondHalf += 1000;
                    }
                }

            }
        }
        /// <summary>
        /// Draws Minute Lines
        /// </summary>
        /// <param name="hourPoint"></param>
        /// <param name="IsSmall"></param>
        private void DrawMinuiteLines(int totalDrawing, bool IsSmall = false)
        {
            var y2 = 15;
            if (IsSmall)
                y2 = 15;
            else
                y2 = 25;
            _axisCanvas.Width = (_scrollViewer.ActualWidth - 10) * Slider_Magnification;
            _axisCanvas.Children.Clear();
            int SecondHalf = 30;
            int Second = totalDrawing * 60;
            for (int i = 0; i <= totalDrawing; i++)
            {
                _axisCanvas.Children.Add(new Line()
                {
                    X1 = Dial_Cell_M * i,
                    Y1 = 0,
                    X2 = Dial_Cell_M * i,
                    Y2 = y2,
                    StrokeThickness = 1
                });
                for (int j = i; j < Second; j++)
                {
                    if (j == SecondHalf)
                    {
                        _axisCanvas.Children.Add(new Line()
                        {
                            X1 = Dial_Cell_S * j,
                            Y1 = 0,
                            X2 = Dial_Cell_S * j,
                            Y2 = 15,
                            StrokeThickness = 1
                        });
                        SecondHalf += 60;
                    }
                }
            }
        }
        /// <summary>
        /// Draws Millisecond Lines
        /// </summary>
        /// <param name="IsSmall"></param>
        private void DrawMillisecodLines(int totalDrawing, bool IsSmall = false)
        {
            var y2 = 15;
            if (IsSmall)
                y2 = 15;
            else
                y2 = 25;
            _axisCanvas.Width = (_scrollViewer.ActualWidth - 10) * Slider_Magnification;
            _axisCanvas.Children.Clear();
            for (int i = 0; i <= totalDrawing; i++)
            {
                _axisCanvas.Children.Add(new Line()
                {
                    X1 = Dial_Cell_MiS * i,
                    Y1 = 0,
                    X2 = Dial_Cell_MiS * i,
                    Y2 = y2,
                    StrokeThickness = 1
                });
            }
        }
        /// <summary>
        /// Draw a Marker on a given point
        /// </summary>
        /// <param name="left"></param>
        public void DrawMarker(double left, bool IsStartingPoint = false)
        {
            if (Markers == null)
                Markers = new Marker[2];
            if (IsMarkerListFull == false && IsMarkerExist(left) == false)
            {
                var path = new Path()
                {
                    Data = Geometry.Parse(GeometryMarker),
                    Margin = new Thickness(left - 12, 2, 0, 0),
                    Name = $"Marker{GetEmptyMarkerIndex}"
                };
                _axisCanvasMarker.Children.Add(path);
                _axisCanvasMarker.RegisterName(path.Name, path);
                Marker marker = new Marker()
                {
                    Name = $"Marker{GetEmptyMarkerIndex}",
                    MarkerPoint = left,
                    IsStarting = IsStartingPoint,
                    Time = AxisTime
                };
                Markers[GetEmptyMarkerIndex] = marker;
                if (GetEmptyMarkerIndex == 0)
                    Generic.SaveBtn.Visibility = Visibility.Visible;
            }

        }
        public void DrawClip(List<Clip> clips)
        {
            _clipCanvas.Width = (_scrollViewer.ActualWidth - 10) * Slider_Magnification;
            _clip.Width = (_scrollViewer.ActualWidth - 10) * Slider_Magnification;
            _clip.Children.Clear();
            foreach(var clip in clips)
            {
                _clip.Children.Add(clip.Left);
                _clip.Children.Add(clip.Middle);
                _clip.Children.Add(clip.Right);
                ClipStartTimeChanged(clip.StartingTime);
                ClipEndTimeChanged(clip);
            }
        }

        #endregion
        #region Clip Setting
        /// <summary>
        /// Recalculate the left coordinate of the clip bar based on the clip time
        /// </summary>
        private void ClipStartTimeChanged(TimeSpan dt)
        {
            TimeSpan ts = dt - StartTime;
            if (ts.Days <= 1 && ts.Seconds >= 0 && _clip != null)
            {
                double left = Dial_Cell_H * (ts.Days == 1 ? 23 : dt.Hours) + Dial_Cell_M * (ts.Days == 1 ? 59 : dt.Minutes) + Dial_Cell_S * (ts.Days == 1 ? 59 : dt.Seconds) + Dial_Cell_MiS * (ts.Days == 1 ? 999 : dt.Milliseconds);
                _clip.Margin = new Thickness(left, 0, 0, 0);
            }
        }

        /// <summary>
        /// Recalculate the width of the clip bar based on the clip time
        /// </summary>
        /// <param name="dt"></param>
        private void ClipEndTimeChanged(Clip clip)
        {
            TimeSpan ts = clip.EndingTime - StartTime;
            if (ts.Days <= 1 && ts.Seconds >= 0 && clip.Middle != null)
            {
                double width = Dial_Cell_H * (ts.Days == 1 ? 23 : ts.Hours) + Dial_Cell_M * (ts.Days == 1 ? 59 : ts.Minutes) + Dial_Cell_S * (ts.Days == 1 ? 59 : ts.Seconds) + Dial_Cell_MiS * (ts.Days == 1 ? 999 : ts.Milliseconds);
                if(width>0)
                {
                    clip.Middle.Width = width - 10;
                    clip.Length = width - 10;
                }
                else
                {
                    clip.Middle.Width = width;
                    clip.Length = width;
                }
            }
        }
        #endregion
        #region Marker
        private bool IsMarkerListFull
        {
            get
            {
                if(Markers!=null)
                {
                    if (Markers[0] != null && Markers[1] != null)
                        return true;
                }
                return false;
            }
        }
        private bool IsMarkerExist(double left)
        {
            if(Markers!=null)
            {
                if(Markers[0]!=null)
                {
                    if (Markers[0].MarkerPoint == left)
                        return true;
                }
                else if(Markers[1] != null)
                {
                    if (Markers[1].MarkerPoint == left)
                        return true;
                }
                
            }
            return false;
        }
        private int GetEmptyMarkerIndex
        {
            get
            {
                if(Markers!=null)
                {
                    if (Markers[0] == null)
                        return 0;
                    else if (Markers[1] == null)
                        return 1;
                    else if(Markers[1]!=null && Markers[0]!=null)
                        return 0;
                }
                return -1;
            }
        }
        #endregion
        /// <summary>
        /// Calculate the intermittent time axis
        /// </summary>
        /// <param name="region"></param>
        /// <returns></returns>
        private Dictionary<KeyValuePair<int, int>, bool> MathToTimeSp(char[] region)
        {
            Dictionary<KeyValuePair<int, int>, bool> dic = new Dictionary<KeyValuePair<int, int>, bool>();
            string regStr = new string(region.Select(x => x == '\0' ? x = '0' : '1').ToArray());
            foreach (Match item in Regex.Matches(regStr, "(.)\\1*"))
            {
                if (item.Success)
                {
                    dic.Add(new KeyValuePair<int, int>(dic.Count + 1, item.Value.Length), item.Value.Contains('1') ? true : false);
                }
            }
            return dic;
        }
        /// <summary>
        /// Get instance items
        /// </summary>
        public override void OnApplyTemplate()
        {
            EventArgs = new VideoEventArgs()
            {
                Duration = new TimeSpan(0, 0, 1),
                FrameRate = 25
            };
            _timePanel = GetTemplateChild(Parid_timePanel) as Canvas;
            _timeLine = GetTemplateChild(Parid_timeLine) as Canvas;
            _axisCanvas = GetTemplateChild(Parid_axisCanvas) as Canvas;
            _axisCanvasTimeText = GetTemplateChild(Parid__axisCanvasTimeText) as Canvas;
            _axisCanvasMarker = GetTemplateChild(Parid__axisCanvasMarker) as Canvas;
            _clipCanvas = GetTemplateChild(Parid_clipCanvas) as Canvas;
            _clip = GetTemplateChild(Parid_clip) as StackPanel;
            if ((_zoomSlider = GetTemplateChild(Parid_zoomSlider) as Slider) != null)
            {
                _zoomSlider.ValueChanged += new RoutedPropertyChangedEventHandler<double>(_zoomSlider_ValueChanged);
            }
            if ((_timePoint = GetTemplateChild(Parid_timePoint) as Grid) != null)
            {
                _timePoint.MouseLeftButtonDown += new MouseButtonEventHandler(timePoint_MouseLeftButtonDown);
                _timePoint.MouseLeftButtonUp += new MouseButtonEventHandler(timePoint_MouseLeftButtonUp);
                _timePoint.MouseMove += new MouseEventHandler(timePoint_MouseMove);
            }
            if ((_scrollViewer = GetTemplateChild(Parid_scrollViewer) as ScrollViewer) != null)
            {
                _scrollViewer.ScrollChanged += new ScrollChangedEventHandler(scrollViewer_Changed);
            }
            if ((_currentTime = GetTemplateChild(Parid_currentTime) as TextBlock) != null)
            {
                _currentTime.Text = StartTime.ToString();
            }
            if ((_clipStateTimeTextBlock = GetTemplateChild(Parid_clipStateTimeTextBlock) as TextBlock) != null)
            {
                Binding binding = new Binding("ClipStartTime") { Source = this, StringFormat = " [ yyyy-MM-dd ] HH:mm:ss " };
                _clipStateTimeTextBlock.SetBinding(TextBlock.TextProperty, binding);
            }
            if ((_clipEndTimeTextBlock = GetTemplateChild(Parid_clipEndTimeTextBlock) as TextBlock) != null)
            {
                Binding binding = new Binding("ClipEndTime") { Source = this, StringFormat = " [ yyyy-MM-dd ] HH:mm:ss " };
                _clipEndTimeTextBlock.SetBinding(TextBlock.TextProperty, binding);
            }
            if ((_cameraListBox = GetTemplateChild(Parid_cameraListBox) as ListBox) != null)
            {
                Binding binding = new Binding("HisVideoSources") { Source = this };
                _cameraListBox.SetBinding(ListBox.ItemsSourceProperty, binding);
            }
            if ((_downButtonListBox = GetTemplateChild(Parid_downButtonListBox) as ListBox) != null)
            {
                Binding binding = new Binding("HisVideoSources") { Source = this };
                _downButtonListBox.SetBinding(ListBox.ItemsSourceProperty, binding);
            }
        }

        /// <summary>
        /// Overwrite the style of the original control
        /// </summary>
        static VideoStateAxisControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(VideoStateAxisControl), new FrameworkPropertyMetadata(typeof(VideoStateAxisControl)));
        }

        #endregion

        #region Enumirator
        
        enum DrawarType
        {
            Hour,
            Minute,
            Second,
            Millisecond,
            Unknown
        };
        #endregion
        #region Event
        private event EventHandler<VideoEventArgs> VideoLoaded;
        public event EventHandler<MarkerEventArgs> JoinButtonClick;
        #endregion
    }

    /// <summary>
    /// Timeline event parameter class
    /// </summary>
    public class VideoStateAxisRoutedEventArgs : RoutedEventArgs
    {
        /// <summary>
        /// Base class constructor
        /// </summary>
        /// <param name="routedEvent"></param>
        /// <param name="source"></param>
        public VideoStateAxisRoutedEventArgs(RoutedEvent routedEvent, object source) : base(routedEvent, source) { }

        /// <summary>
        /// Pointer time
        /// </summary>
        public TimeSpan TimeLine { get; set; }
    }
    /// <summary>
    /// Video parameter class
    /// </summary>
    public class VideoEventArgs:EventArgs
    {
        public TimeSpan Duration { get; set; }
        public int FrameRate { get; set; }
        public string VideoTitle { get; set; }
    }
    public class MarkerEventArgs: EventArgs
    {
        public Marker[] Markers { get; set; }
    }
    public class Marker
    {
        public string Name { get; set; }
        public double MarkerPoint { get; set; } = 0;
        public TimeSpan Time { get; set; }
        public bool IsStarting { get; set; } = false;
    }
    /// <summary>
    /// Timeline control event type
    /// </summary>

    /// <summary>
    /// Timeline object
    /// </summary>
    public class VideoStateItem : INotifyPropertyChanged
    {
        private string _cameraName;
        public string CameraName
        {
            get => _cameraName;
            set { _cameraName = value; OnPropertyChanged("CameraName"); }
        }

        private bool _cameraChedcked;
        /// <summary>
        /// Is the camera selected
        /// </summary>
        public bool CameraChecked
        {
            get => _cameraChedcked;
            set { _cameraChedcked = value; OnPropertyChanged("CameraChecked"); }
        }

        private char[] _axisHistoryTimeList;
        /// <summary>
        /// Camera history video video time set
        /// </summary>
        public char[] AxisHistoryTimeList
        {
            get => _axisHistoryTimeList;
            set { _axisHistoryTimeList = value; OnPropertyChanged("AxisHistoryTimeList"); }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = this.PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
    public class Clip
    {
        internal double Length { get; set; }
        /// <summary>
        /// Clip Starting Time
        /// </summary>
        public TimeSpan StartingTime { get; set; }
        /// <summary>
        /// Clip Ending Time
        /// </summary>
        public TimeSpan EndingTime { get; set; }
        internal Border Left { get; private set; }
        internal Border Right { get; private set; }
        internal Border Middle { get; private set; }
        public Clip(double lenght=50)
        {
            Length = lenght;
            Left = new Border()
            {
                Width = 5,
                Background = (Brush)(new BrushConverter().ConvertFrom("#FF3AFF00")),
                BorderBrush = (Brush)(new BrushConverter().ConvertFrom("#FF000000")),
                BorderThickness = new Thickness(0, 0, 1, 0),
                CornerRadius = new CornerRadius(2, 0, 0, 2),
                Margin = new Thickness(0, 50, 0, 0)
            };
            Right = new Border()
            {
                Width = 5,
                Background = (Brush)(new BrushConverter().ConvertFrom("#FF4500")),
                BorderBrush = (Brush)(new BrushConverter().ConvertFrom("#FF000000")),
                BorderThickness = new Thickness(1, 0, 0, 0),
                CornerRadius = new CornerRadius(0, 2, 2, 0),
                Margin = new Thickness(0, 50, 0, 0)
            };
            Middle = new Border()
            {
                Width = Length,
                Background = (Brush)(new BrushConverter().ConvertFrom("#FF777777")),
                BorderBrush = (Brush)(new BrushConverter().ConvertFrom("#FF000000")),
                BorderThickness = new Thickness(0, 0, 1, 0),
                CornerRadius = new CornerRadius(2, 0, 0, 2),
                Margin = new Thickness(0, 50, 0, 0)
            };
        }

    }
}
