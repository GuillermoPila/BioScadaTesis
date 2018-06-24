using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
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
using Telerik.Windows.Controls;
using Telerik.Windows.Controls.Charting;
using WpfComponent.Common;
using Telerik.Windows.Data;

namespace WpfComponent.Component.Visuals
{
    /// <summary>
    /// Interaction logic for Chart.xaml
    /// </summary>
    public partial class Chart : UserControl, IObjectReceiver
    {
        private LinesCollection Lines = new LinesCollection();

        private const int queueCapacity = 30;
        private Random r = new Random();
        private Queue<LinesCollection> data = new Queue<LinesCollection>(queueCapacity);
        private DateTime nowTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second);

        public Chart()
        {
            InitializeComponent();

            Lines.Var0 = new DataLine() { Active = true, VariableName = "P00", IsInput = true };
            Lines.Var1 = new DataLine() { Active = true, VariableName = "P01", IsInput = true };
            Lines.Var2 = new DataLine() { Active = true, VariableName = "T0000", IsInput = false };
            Lines.Var3 = new DataLine() { Active = true, VariableName = "P44", IsInput = true };
            Lines.Var4 = new DataLine() { Active = true, VariableName = "P46", IsInput = true };
            Lines.Var5 = new DataLine() { Active = true, VariableName = "P47", IsInput = true };
            Lines.UpdateReference();

            FillChartDataLive();
            SetUpYAxis();
            SetUpYAxisInput();
            SetUpXAxis();
            FillDataLine();

            Init();
            SetUpTimer();
        }



        private void Init()
        {
            ScadaDataSource dataSource = new ScadaDataSource("Exp1");

            dataSource.RefreshFrequency = 500;
            dataSource.Receivers.Add(new ObjectTransmitter.ReceiverItem() { Receiver = this });
            dataSource.Active = true;
        }
        public void Receive(Dictionary<string, object> Objects)
        {
            nowTime = nowTime.AddMilliseconds(500);
            LinesCollection temp = new LinesCollection();
            Type t = temp.GetType();
            DataLine aux = null;

            foreach (var line in Lines.Reference.Keys)
                if (Objects.ContainsKey(line))
                {
                    //RadChart1.Dispatcher.BeginInvoke(DispatcherPriority.Normal,
                    //                     (ThreadStart)(() => UpdateProperty(t, ref aux, line, Objects, nowTime)));

                    string varProp = "Var" + Lines.Reference[line];
                    aux = (t.GetProperty(varProp).GetValue(temp, null) as DataLine);
                    aux.Data = Convert.ToDouble(Objects[line]);
                    aux.Time = nowTime;
                }

            UpdateDataLive(temp);

            //RadChart1.Dispatcher.BeginInvoke(DispatcherPriority.Normal,
            //                             (ThreadStart)(() => SetUpAxisXRange(nowTime)));
            //RadChart1.Dispatcher.BeginInvoke(DispatcherPriority.Normal,
            //                             (ThreadStart)(() => RadChart1.ItemsSource = null));
            //RadChart1.Dispatcher.BeginInvoke(DispatcherPriority.Normal,
            //                             (ThreadStart)(() => RadChart1.ItemsSource = data));

        }

        private void UpdateProperty(Type t, ref DataLine aux, string line, Dictionary<string, object> Objects, DateTime now)
        {
            string varProp = "Var"+Lines.Reference[line];
            aux = (t.GetProperty(varProp).GetValue(Lines, null) as DataLine);
            aux.Data = Convert.ToDouble(Objects[line]);
            aux.Time = now;
        }

        private void SetUpTimer()
        {
            DispatcherTimer timer1 = new DispatcherTimer();
            timer1.Interval = TimeSpan.FromMilliseconds(1000);
            timer1.Tick += timer1_Tick;
            timer1.Start();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            SetUpAxisXRange(nowTime);
            RadChart1.ItemsSource = null;
            RadChart1.ItemsSource = data;
        }

        private void SetUpAxisXRange(DateTime now)
        {
            RadChart1.DefaultView.ChartArea.AxisX.MinValue = now.AddSeconds(-14.5).ToOADate();
            RadChart1.DefaultView.ChartArea.AxisX.MaxValue = now.ToOADate();
            RadChart1.DefaultView.ChartArea.AxisX.Step = 1.0 / 24.0 / 3600.0 / 2.0;
        }

        private void UpdateDataLive(LinesCollection temp)
        {
            if (this.data.Count >= queueCapacity)
                this.data.Dequeue();

            this.data.Enqueue(temp);
        }

        //private void UpdateData(DateTime now)
        //{
        //    if (this.data.Count >= queueCapacity)
        //        this.data.Dequeue();

        //    LinesCollection temp = new LinesCollection();
        //    for (int i = 0; i < Lines.Lines.Count; i++)
        //    {
        //        if (Lines.Lines[i].Active)
        //        {
        //            temp.Lines[i].Data = r.Next(0, 500);
        //            temp.Lines[i].Time = now;
        //        }
        //    }
        //    this.data.Enqueue(temp);
        //}

        private void FillChartDataLive()
        {
            RadChart1.DefaultView.ChartTitle.Content = "BioScada Live";
            RadChart1.DefaultView.ChartLegend.Header = "Variables";

            RadChart1.DefaultView.ChartArea.NoDataString = "Waiting for data...";
            RadChart1.DefaultView.ChartArea.EnableAnimations = false;


        }

        private void FillDataLine()
        {
            Type t = Lines.GetType();
            DataLine aux = new DataLine();
            for (int i = 0; i < Lines.Reference.Count; i++)
            {
                string propName = string.Format("Var{0}", Lines.Reference.Values.ElementAt(i));
                aux = (t.GetProperty(propName).GetValue(Lines, null) as DataLine);
                if (aux.Active)
                {
                    SeriesMapping lineDataMapping = new SeriesMapping();
                    lineDataMapping.LegendLabel = aux.VariableName;
                    //  lineDataMapping.CollectionIndex = i;
                    lineDataMapping.SeriesDefinition = new LineSeriesDefinition();
                    (lineDataMapping.SeriesDefinition as LineSeriesDefinition).ShowPointMarks = false;
                    (lineDataMapping.SeriesDefinition as LineSeriesDefinition).ShowItemLabels = false;
                    if (aux.IsInput)
                        lineDataMapping.SeriesDefinition.AxisName = "Input";
                    string xValue = propName + ".Time";
                    string yValue = propName + ".Data";
                    lineDataMapping.ItemMappings.Add(new ItemMapping(xValue, DataPointMember.XValue));
                    lineDataMapping.ItemMappings.Add(new ItemMapping(yValue, DataPointMember.YValue));
                    RadChart1.SeriesMappings.Add(lineDataMapping);
                }
            }
        }
        private void SetUpYAxisInput()
        {
            AxisY ramAxis = new AxisY();
            ramAxis.AxisName = "Input";
            ramAxis.Title = "Inputs";
            ramAxis.AutoRange = false;
            ramAxis.MinValue = 0;
            ramAxis.MaxValue = 2;
            ramAxis.Step = 1;
            ramAxis.DefaultLabelFormat = "#VAL";
            RadChart1.DefaultView.ChartArea.AdditionalYAxes.Add(ramAxis);
        }

        private void SetUpYAxis()
        {
            RadChart1.DefaultView.ChartArea.AxisY.AutoRange = true;
            RadChart1.DefaultView.ChartArea.AxisY.Step = 20;

            RadChart1.DefaultView.ChartArea.AxisY.DefaultLabelFormat = "#VAL{N0}";
            RadChart1.DefaultView.ChartArea.AxisY.Title = "Variable Value";

        }

        private void SetUpXAxis()
        {
            RadChart1.DefaultView.ChartArea.AxisX.DefaultLabelFormat = "#VAL{hh:mm:ss}";
            RadChart1.DefaultView.ChartArea.AxisX.LabelRotationAngle = 90;
            RadChart1.DefaultView.ChartArea.AxisX.LabelStep = 2;
            RadChart1.DefaultView.ChartArea.AxisX.Title = "Time";
            RadChart1.DefaultView.ChartArea.AxisX.LayoutMode = AxisLayoutMode.Normal;
            RadChart1.DefaultView.ChartArea.AxisX.AutoRange = false;
        }

        private void HistorialButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("El historico todavía no está hecho");
        }
    }

    public class LinesCollection
    {
        //public DateTime Time { get; set; }

        private DataLine var0;
        public DataLine Var0 { get { return var0; } set { var0 = value; } }

        private DataLine var1;
        public DataLine Var1 { get { return var1; } set { var1 = value; } }

        private DataLine var2;
        public DataLine Var2 { get { return var2; } set { var2 = value; } }

        private DataLine var3;
        public DataLine Var3 { get { return var3; } set { var3 = value; } }

        private DataLine var4;
        public DataLine Var4 { get { return var4; } set { var4 = value; } }

        private DataLine var5;
        public DataLine Var5 { get { return var5; } set { var5 = value; } }

        private DataLine var6;
        public DataLine Var6 { get { return var6; } set { var6 = value; } }

        private DataLine var7;
        public DataLine Var7 { get { return var7; } set { var7 = value; } }

        public Dictionary<string, int> Reference;

        public LinesCollection()
        {
            Var0 = new DataLine();
            Var1 = new DataLine();
            Var2 = new DataLine();
            Var3 = new DataLine();
            Var4 = new DataLine();
            Var5 = new DataLine();
            Var6 = new DataLine();
            Var7 = new DataLine();


        }

        public void UpdateReference()
        {
            Type t = this.GetType();
            PropertyInfo[] propertys = t.GetProperties();
            Reference = new Dictionary<string, int>();

            for (int i = 0; i < propertys.Length; i++)
            {
                var gh = (propertys[i].GetValue(this, null) as DataLine);
                if (gh.VariableName != null)
                    Reference.Add(gh.VariableName, i);
            }
        }
    }
    public class DataLine
    {
        public bool Active { get; set; }
        public bool IsInput { get; set; }
        public string VariableName { get; set; }
        public double Data { get; set; }
        public DateTime Time { get; set; }
    }


}
