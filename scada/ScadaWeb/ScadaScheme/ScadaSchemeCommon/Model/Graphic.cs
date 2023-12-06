using Scada.Scheme.Model.DataTypes;
using Scada.Scheme.Model.PropertyGrid;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Design;
using System.Runtime.InteropServices;
using System.Xml;
using CM = System.ComponentModel;
using Newtonsoft.Json;
using Utils;
using System.IO;

namespace Scada.Scheme.Model
{
    /// <summary>
    /// Scheme component that represents a chart
    /// <para>Компонент схемы, представляющий график</para>
    /// </summary>
    [Serializable]
    public class Chart : BaseComponent
    {
        /// <summary>
        /// Points of the chart
        /// </summary>
        public List<int> DataPoints { get; set; } = new List<int>();

        /// <summary>
        /// Constructor
        /// </summary>
        public Chart()
        {
            DataPoints = new List<int>() {10, 20, 30, 40, 50};
            CurveStyle = ChartCurveStyles.Smooth;
        }

        /// <summary>
        /// Get or set the type of the chart (e.g., line, bar, etc.)
        /// </summary>
        #region Attributes
        [DisplayName("Chart Type"), Category(Categories.Appearance)]
        [Description("The type of the chart.")]
        [CM.DefaultValue(ChartTypes.Line)]
        #endregion
        public ChartTypes ChartType { get; set; }

        public List<string> Dates { get; set; } = new List<string>();

        private string dataSource;

        public int MaxValue { get; set; } = 0;

        public int MinValue { get; set; } = 0;

        #region Attributes
        [DisplayName("Debug String"), Category(Categories.Data)]
        [Description("The data source for the chart.")]
        [CM.DefaultValue("")]
        #endregion
        public string LogString {get; set;} = "";

        /// <summary>
        /// Get or set the data source for the chart
        /// </summary>
        #region Attributes
        [DisplayName("Data Source"), Category(Categories.Data)]
        [Description("The data source for the chart.")]
        [CM.DefaultValue("")]
        #endregion
        public string DataSource
        {
            get { return dataSource; }
            set
            {
                if (dataSource != value)
                {
                    dataSource = value;

                    if (dataSource != "")
                    {
                        LogString = dataSource;

                        string path = Path.GetFullPath(dataSource);
                        string jsonString = File.ReadAllText(path);

                        List<Record> data = JsonConvert.DeserializeObject<List<Record>>(jsonString);
                        DataPoints = new List<int>();

                        MaxValue = data[0].tmax;
                        MinValue = data[0].tmin;

                        foreach (var item in data)
                        {
                            DataPoints.Add(MinValue + item.value);
                            Dates.Add(item.recordTime);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Get or set the curve style of the chart
        /// </summary>
        #region Attributes
        [DisplayName("Curve Style"), Category(Categories.LineChart)]
        [Description("The curve style of the chart.")]
        [CM.DefaultValue(ChartCurveStyles.Smooth)]
        #endregion
        public ChartCurveStyles CurveStyle { get; set; }

        public string ToJson()
        {
            return JsonConvert.SerializeObject(DataPoints);
        }

        /// <summary>
        /// Load component configuration from XML node
        /// </summary>
        public override void LoadFromXml(XmlNode xmlNode)
        {
            base.LoadFromXml(xmlNode);

            ChartType = xmlNode.GetChildAsEnum<ChartTypes>("ChartType");
            DataSource = xmlNode.GetChildAsString("DataSource");
            CurveStyle = xmlNode.GetChildAsEnum<ChartCurveStyles>("CurveStyle");
        }

        /// <summary>
        /// Save component configuration to XML node
        /// </summary>
        public override void SaveToXml(XmlElement xmlElem)
        {
            base.SaveToXml(xmlElem);

            xmlElem.AppendElem("ChartType", ChartType);
            xmlElem.AppendElem("DataSource", DataSource);
            xmlElem.AppendElem("CurveStyle", CurveStyle);
        }

    }

    /// <summary>
    /// Chart types
    /// <para>Типы графиков</para>
    /// </summary>
    public enum ChartTypes
    {
        /// <summary>
        /// Line chart type.
        /// <para>Линейный график</para>
        /// </summary>
        Line,

        /// <summary>
        /// Bar chart type.
        /// <para>Столбчатый график</para>
        /// </summary>
        Bar,

        /// <summary>
        /// Area chart type.
        /// <para>График области</para>
        /// </summary>
        Area,

        /// <summary>
        /// Pie chart type.
        /// <para>Круговая диаграмма</para>
        /// </summary>
        Pie,

        /// <summary>
        /// Donut chart type.
        /// <para>Донат-диаграмма</para>
        /// </summary>
        Donut,

        /// <summary>
        /// Radar chart type.
        /// <para>Радар-диаграмма</para>
        /// </summary>
        Radar,

        /// <summary>
        /// Scatter chart type.
        /// <para>Точечная диаграмма</para>
        /// </summary>
        Scatter,

        /// <summary>
        /// Bubble chart type.
        /// <para>Пузырьковая диаграмма</para>
        /// </summary>
        Bubble,

        /// <summary>
        /// Heatmap chart type.
        /// <para>Тепловая карта</para>
        /// </summary>
        Heatmap
    }

    /// <summary>
    /// Chart curve styles
    /// <para>Стили кривых графиков</para>
    /// </summary>
    public enum ChartCurveStyles
    {
        /// <summary>
        /// Straight line.
        /// <para>Прямая линия</para>
        /// </summary>
        Straight,

        /// <summary>
        /// Smooth curve.
        /// <para>Гладкая кривая</para>
        /// </summary>
        Smooth,

        /// <summary>
        /// Step curve.
        /// <para>Ступенчатая кривая</para>
        /// </summary>
        StepLine
    }

    public class Record
    {
        public int tmax { get; set; }
        public int tmin { get; set; }
        public string recordTime { get; set; }
        public int value { get; set; }
    }
}
