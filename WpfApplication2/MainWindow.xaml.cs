using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace WpfApplication2
{


    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        private DispatcherTimer updateTimer;

        public MainWindow()
        {
            InitializeComponent();
            updateTimer = new System.Windows.Threading.DispatcherTimer();
            updateTimer.Tick += new EventHandler(DrawingAY);
            updateTimer.Interval = new TimeSpan(0, 0, 0, 0, 1000 / 60);
            updateTimer.Start();
        }

        private void DrawingAY(object sender, EventArgs e)
        {
            AyKeyFrame();
        }

        private void Cav_MouseMove(object sender, MouseEventArgs e)
        {
            var ui = e.GetPosition(Cav);
            mousePoint.x = ui.X;
            mousePoint.y = ui.Y;

        }

        private void Cav_MouseLeave(object sender, MouseEventArgs e)
        {
            mousePoint.x = null;
            mousePoint.y = null;
        }


        List<GrainBase> grains = new List<GrainBase>();
        //List<GrainBase> grainsEqual = new List<GrainBase>();
        GrainBase mousePoint = new GrainBase();
        Random rand = new Random();
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            mousePoint.max = 20000;
            grains.Add(mousePoint);
       
            //// 添加粒子
            //// x，y为粒子坐标，xa, ya为粒子xy轴加速度，max为连线的最大距离     
            for (int i = 0; i <100; i++)
            {
                GrainBase gb = new GrainBase();
                gb.x = rand.NextDouble() * Cav.ActualWidth;
                gb.y = rand.NextDouble() * Cav.ActualHeight;
                gb.xa = rand.NextDouble() * 2 - 1;
                gb.ya = rand.NextDouble() * 2 - 1;
                gb.max = 8000;
                grains.Add(gb);
            }
            //grainsEqual.Add(mousePoint);
            //grains.ForEach(i => grainsEqual.Add(i));

        }

        public void AyKeyFrame()
        {
            Cav.Children.Clear();

            for (int i = 0; i < grains.Count; i++)
            {
                GrainBase dot = grains[i];
                if (dot.x == null || dot.y == null) continue;
                #region 创建碰撞粒子
                // 粒子位移
                dot.x += dot.xa;
                dot.y += dot.ya;
                // 遇到边界将加速度反向
                dot.xa *= (dot.x.Value > Cav.ActualWidth || dot.x.Value < 0) ? -1 : 1;
                dot.ya *= (dot.y.Value > Cav.ActualHeight || dot.y.Value < 0) ? -1 : 1;
                // 绘制点
                Ellipse elip = new Ellipse();
                elip.Width = 2;
                elip.Height = 2;
                Canvas.SetLeft(elip, dot.x.Value - 0.5);
                Canvas.SetTop(elip, dot.y.Value - 0.5);
                elip.Fill = new SolidColorBrush(Colors.White);
                Cav.Children.Add(elip);
                #endregion

            
                //判断是不是最后一个，就不用两两比较了
                int endIndex = i + 1;
                if(endIndex == grains.Count) continue;
                for (int j = endIndex; j < grains.Count; j++)
                {
                    GrainBase d2 = grains[j];
                    double xc = dot.x.Value - d2.x.Value;
                    double yc = dot.y.Value - d2.y.Value;
                    // 两个粒子之间的距离
                    double dis = xc * xc + yc * yc;
                    // 距离比
                    double ratio;
                    // 如果两个粒子之间的距离小于粒子对象的max值，则在两个粒子间画线
                    if (dis < d2.max)
                    {
                        // 计算距离比
                        ratio = (d2.max - dis) / d2.max;
                        Line line = new Line();
                        double opacity = ratio + 0.2;
                        if (opacity > 1) { opacity = 1; }
                        byte ar = (byte)(opacity * 255);
                        line.Stroke = new SolidColorBrush(Color.FromArgb(ar, 255, 255, 255));
                        line.StrokeThickness = ratio / 2;
                        line.X1 = dot.x.Value;
                        line.Y1 = dot.y.Value;
                        line.X2 = d2.x.Value;
                        line.Y2 = d2.y.Value;
                        Cav.Children.Add(line);
                    }
                }
            }
       
        }

        private void Cav_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.MouseDevice.LeftButton == MouseButtonState.Pressed) {
                GrainBase gb = new GrainBase();
                var ui = e.GetPosition(Cav);
                gb.x = ui.X;
                gb.y = ui.Y;
               
                gb.xa = rand.NextDouble() * 2 - 1;
                gb.ya = rand.NextDouble() * 2 - 1;
                gb.max = 8000;
                grains.Add(gb);
            }
        }

      
    }
}
