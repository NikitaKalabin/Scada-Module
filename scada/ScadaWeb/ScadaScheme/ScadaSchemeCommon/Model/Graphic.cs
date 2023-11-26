using Scada.Scheme.Model.DataTypes;
using Scada.Scheme.Model.PropertyGrid;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Design;
using System.Runtime.InteropServices;
using System.Xml;
using CM = System.ComponentModel;
using System.Runtime.InteropServices;
using Newtonsoft.Json;

namespace Scada.Scheme.Model
{
    /// <summary>
    /// Scheme component that represents a chart
    /// <para>Компонент схемы, представляющий график</para>
    /// </summary>
    [Serializable]
    public class Chart : StaticText, IDynamicComponent // inheriting from StaticText for now, but you might want a different base class
    {
        /// <summary>
        /// Default chart title
        /// </summary>
        new public static readonly string DefaultText =
            Localization.UseRussian ? "График" : "Chart";

        /// <summary>
        /// Constructor
        /// </summary>
        // Sample data for the chart
        public List<PointF> DataPoints { get; set; } = new List<PointF>();



        public Chart()
        {
            // Sample data for demonstration
            DataPoints.Add(new PointF(10, 20));
            DataPoints.Add(new PointF(20, 30));
            DataPoints.Add(new PointF(30, 10));
        }

        // Override the render method to draw the chart

        /// <summary>
        /// Get or set the type of the chart (e.g., line, bar, etc.)
        /// </summary>
        #region Attributes
        [DisplayName("Chart Type"), Category(Categories.Behavior)]
        [Description("The type of the chart.")]
        [CM.DefaultValue(ChartTypes.Line)]
        #endregion
        public ChartTypes ChartType { get; set; }

        public void UpdateProperties()
        {
            // ... update properties based on user input ...
        }


        /// <summary>
        /// Get or set the data source for the chart
        /// </summary>
        #region Attributes
        [DisplayName("Data Source"), Category(Categories.Data)]
        [Description("The data source for the chart.")]
        [CM.DefaultValue("")]
        #endregion
        public string DataSource { get; set; }
        /// <summary>
        /// Gets or sets the action associated with the chart.
        /// <para>Получает или задает действие, связанное с графиком.</para>
        /// </summary>
        /// <exception cref="NotImplementedException">Thrown when trying to get or set the value.</exception>
        private Actions _action;
        public Actions Action
        {
            get => _action;
            set => _action = value;
        }

        /// <summary>
        /// Gets or sets the input channel number associated with the chart.
        /// <para>Получает или задает номер входного канала, связанного с графиком.</para>
        /// </summary>
        public int InCnlNum { get; set; }

        /// <summary>
        /// Gets or sets the control channel number associated with the chart.
        /// <para>Получает или задает номер канала управления, связанного с графиком.</para>
        /// </summary>
        public int CtrlCnlNum { get; set; }

        // Метод для обновления графика на основе пользовательского ввода
        public void UpdateChart(List<PointF> newPoints)
        {
            DataPoints = newPoints; // Обновите список точек
            UpdateChartJavaScript(); // Вызовите JavaScript метод для обновления графика
        }

        [DllImport("JavaScriptBridge.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern void CallJavaScriptFunction(string functionName, string argument);

        // Метод для вызова JavaScript метода обновления графика
        private void UpdateChartJavaScript()
        {
            CallJavaScriptFunction("updateChartJavaScript", this.ToString());
        }
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
            // ... load other properties ...
        }
        /// <summary>
        /// Save component configuration to XML node
        /// </summary>
        public override void SaveToXml(XmlElement xmlElem)
        {
            base.SaveToXml(xmlElem);

            xmlElem.AppendElem("ChartType", ChartType);
            xmlElem.AppendElem("DataSource", DataSource);
            // ... save other properties ...
        }

    }
    // Enum for chart types; expand as necessary
    /// <summary>
    /// Enumerates types of charts.
    /// <para>Перечисляет типы графиков.</para>
    /// </summary>
    public enum ChartTypes
    {
        /// <summary>
        /// Line chart type.
        /// <para>Тип линейного графика.</para>
        /// </summary>
        Line,

        /// <summary>
        /// Bar chart type.
        /// <para>Тип столбчатого графика.</para>
        /// </summary>
        Bar
    }
}
