using Scada.Scheme.Model;
using Scada.Scheme.Model.PropertyGrid;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Design;
using System.Xml;
using Scada.Scheme.Model.DataTypes;
using CollectionConverter = Scada.Scheme.Model.PropertyGrid.CollectionConverter;

namespace Scada.Web.Plugins.SchBasicComp
{
   [Serializable]
    public class Table : BaseComponent, IDynamicComponent
    {
        public static readonly Size DefaultSize = new Size(200, 100);

        // Конструктор
        public Table()
            : base()
        {
            HeaderColor = "LightGray";
            RowColors = new List<string> { "White" };
            Cells = new List<TableCell>();
            Size = DefaultSize;
        }

        // Свойства для настройки отображения таблицы
        #region Attributes
        [Scheme.Model.PropertyGrid.DisplayName("Header Color"), Scheme.Model.PropertyGrid.Category("Appearance")]
        [Scheme.Model.PropertyGrid.Description("The background color of the table header.")]
        #endregion
        public string HeaderColor { get; set; }

        // Свойства для хранения цвета каждой строки таблицы
        #region Attributes
        [Scheme.Model.PropertyGrid.DisplayName("Row Colors"), Scheme.Model.PropertyGrid.Category("Appearance")]
        [Scheme.Model.PropertyGrid.Description("The background colors of the table rows.")]
        [TypeConverter(typeof(CollectionConverter))]
        [Editor(typeof(CollectionEditor), typeof(UITypeEditor))]
        #endregion
        public List<string> RowColors { get; set; }

        // Свойства для хранения данных ячеек таблицы
        #region Attributes
        [Scheme.Model.PropertyGrid.DisplayName("Cells"), Scheme.Model.PropertyGrid.Category("Data")]
        [Scheme.Model.PropertyGrid.Description("The data of the table cells.")]
        [TypeConverter(typeof(CollectionConverter))]
        [Editor(typeof(CollectionEditor), typeof(UITypeEditor))]
        #endregion
        public List<TableCell> Cells { get; set; }

        // Методы для работы с XML конфигурацией
        public override void LoadFromXml(XmlNode xmlNode)
        {
            base.LoadFromXml(xmlNode);

            HeaderColor = xmlNode.GetChildAsString("HeaderColor");

            RowColors.Clear();
            XmlNode rowsColorNode = xmlNode.SelectSingleNode("RowColors");
            if (rowsColorNode != null)
            {
                foreach (XmlNode colorNode in rowsColorNode.SelectNodes("Color"))
                {
                    RowColors.Add(colorNode.InnerText);
                }
            }

            Cells.Clear();
            XmlNode cellsNode = xmlNode.SelectSingleNode("Cells");
            if (cellsNode != null)
            {
                foreach (XmlNode cellNode in cellsNode.SelectNodes("Cell"))
                {
                    TableCell cell = new TableCell();
                    cell.LoadFromXml(cellNode);
                    Cells.Add(cell);
                }
            }
        }

        public override void SaveToXml(XmlElement xmlElem)
        {
            base.SaveToXml(xmlElem);

            xmlElem.AppendElem("HeaderColor", HeaderColor);

            XmlElement rowColorsElem = xmlElem.AppendElem("RowColors");
            foreach (string color in RowColors)
            {
                rowColorsElem.AppendElem("Color", color);
            }

            XmlElement cellsElem = xmlElem.AppendElem("Cells");
            foreach (TableCell cell in Cells)
            {
                XmlElement cellElem = cellsElem.AppendElem("Cell");
                cell.SaveToXml(cellElem);
            }
        }

        public override BaseComponent Clone()
        {
            Table clonedComponent = (Table)base.Clone();
            clonedComponent.RowColors = new List<string>(RowColors);
            clonedComponent.Cells = new List<TableCell>(Cells.ConvertAll(cell => (TableCell)cell.Clone()));
            return clonedComponent;
        }

        public Actions Action { get; set; }
        public int InCnlNum { get; set; }
        public int CtrlCnlNum { get; set; }
    }

    [Serializable]
    public class TableCell : ICloneable
    {
        public string Text { get; set; }
        public int RowSpan { get; set; }
        public int ColSpan { get; set; }

        public TableCell()
        {
            Text = "";
            RowSpan = 1;
            ColSpan = 1;
        }

        public void LoadFromXml(XmlNode xmlNode)
        {
            Text = xmlNode.GetChildAsString("Text");
            RowSpan = xmlNode.GetChildAsInt("RowSpan");
            ColSpan = xmlNode.GetChildAsInt("ColSpan");
        }

        public void SaveToXml(XmlElement xmlElem)
        {
            xmlElem.AppendElem("Text", Text);
            xmlElem.AppendElem("RowSpan", RowSpan);
            xmlElem.AppendElem("ColSpan", ColSpan);
        }

        public object Clone()
        {
            return MemberwiseClone();
        }
    }
}