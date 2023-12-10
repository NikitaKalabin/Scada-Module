using Scada.Scheme.Model;
using Scada.Scheme.Model.PropertyGrid;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Design;
using System.Xml;
using Scada.Scheme.Model.DataTypes;
using CollectionConverter = Scada.Scheme.Model.PropertyGrid.CollectionConverter;
using System.IO;
using Newtonsoft.Json;
using CM = System.ComponentModel;
using System.Linq;
using Newtonsoft.Json.Linq;

namespace Scada.Scheme.Model
{
    /// <summary>
    /// Scheme component that represents a table
    /// <para>Компонент схемы, представляющий таблицу</para>
    /// </summary>
    [Serializable]
    public class Table : BaseComponent, IDynamicComponent
    {
        new public static readonly string DefaultText =
            Localization.UseRussian ? "Таблица" : "Table";


        // Конструктор
        public Table()
            : base()
        {
            HeaderColor = "";
            RowColor = "";
            Cells = new List<TableCell>();
            CtrlCnlNum = 0;
            InCnlNum = 0;
            RowCount = 1;
            ColCount = 1;
        }

        // Свойства для настройки отображения таблицы
        #region Attributes
        [Scheme.Model.PropertyGrid.DisplayName("Header Color"), Scheme.Model.PropertyGrid.Category(Categories.Appearance)]
        [Scheme.Model.PropertyGrid.Description("The background color of the table header.")]
        [CM.Editor(typeof(ColorEditor), typeof(UITypeEditor))]
        #endregion
        public string HeaderColor { get; set; }

        // Свойства для хранения цвета каждой строк таблицы
        #region Attributes
        [Scheme.Model.PropertyGrid.DisplayName("Row Color"), Scheme.Model.PropertyGrid.Category(Categories.Appearance)]
        [Scheme.Model.PropertyGrid.Description("The background colors of the table rows.")]
        [CM.Editor(typeof(ColorEditor), typeof(UITypeEditor))]
        #endregion
        public string RowColor { get; set; }

        #region Attributes
        [Scheme.Model.PropertyGrid.DisplayName("Cells Font"), Scheme.Model.PropertyGrid.Category(Categories.Appearance)]
        [Scheme.Model.PropertyGrid.Description("The font used to display text in the cells.")]
        #endregion
        public Font CellsFont { get; set; }

        #region Attributes
        [Scheme.Model.PropertyGrid.DisplayName("Header Font"), Scheme.Model.PropertyGrid.Category(Categories.Appearance)]
        [Scheme.Model.PropertyGrid.Description("The font used to display text in the header of the table.")]
        #endregion
        public Font HeaderFont { get; set; }

        // Свойства для хранения данных ячеек таблицы
        #region Attributes
        [Scheme.Model.PropertyGrid.DisplayName("Cells"), Scheme.Model.PropertyGrid.Category("Data")]
        [Scheme.Model.PropertyGrid.Description("The data of the table cells.")]
        [TypeConverter(typeof(CollectionConverter))]
        [Editor(typeof(CollectionEditor), typeof(UITypeEditor))]
        #endregion
        public List<TableCell> Cells { get; set; }

        /// <summary>
        /// Получить или установить размер ячеек.
        /// </summary>
        #region Attributes
        [Scheme.Model.PropertyGrid.DisplayName("Size Of Cells"), Scheme.Model.PropertyGrid.Category(Categories.Appearance)]
        [Scheme.Model.PropertyGrid.Description("The size of the table cells in pixels.")]
        #endregion
        public Size SizeOfCells { get; set; }

        // Методы для работы с XML конфигурацией
        public override void LoadFromXml(XmlNode xmlNode)
        {
            base.LoadFromXml(xmlNode);
            RowColor = xmlNode.GetChildAsString("RowColor");
            RowCount = xmlNode.GetChildAsInt("RowCount");
            ColCount = xmlNode.GetChildAsInt("ColCount");
            CellsFont = Font.GetChildAsFont(xmlNode, "CellsFont");
            HeaderColor = xmlNode.GetChildAsString("HeaderColor");
            HeaderFont = Font.GetChildAsFont(xmlNode, "HeaderFont");
            InCnlNum = xmlNode.GetChildAsInt("InCnlNum");
            CtrlCnlNum = xmlNode.GetChildAsInt("CtrlCnlNum");

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

            xmlElem.AppendElem("RowColor", RowColor);
            xmlElem.AppendElem("RowCount", RowCount);
            xmlElem.AppendElem("ColCount", ColCount);
            xmlElem.AppendElem("CellsFont", CellsFont);
            xmlElem.AppendElem("HeaderColor", HeaderColor);
            xmlElem.AppendElem("HeaderFont", HeaderFont);
            xmlElem.AppendElem("InCnlNum", InCnlNum);
            xmlElem.AppendElem("CtrlCnlNum", CtrlCnlNum);
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
            clonedComponent.Cells = new List<TableCell>(Cells.ConvertAll(cell => (TableCell)cell.Clone()));
            clonedComponent.RowColor = RowColor;
            clonedComponent.RowCount = RowCount;
            clonedComponent.ColCount = ColCount;
            clonedComponent.CellsFont = CellsFont;
            clonedComponent.HeaderColor = HeaderColor;
            clonedComponent.HeaderFont = HeaderFont;
            return clonedComponent;
        }

        public Actions Action { get; set; }
        public int InCnlNum { get; set; }
        public int CtrlCnlNum { get; set; }
        public int RowCount { get; set; }

        public int ColCount { get; set; }
        public class Record
        {
            public KeyValuePair<int, int> machineid { get; set; }
            public KeyValuePair<int, string> timestart { get; set; }
            public KeyValuePair<int, string> timeend { get; set; }
            public KeyValuePair<int, int> tmin { get; set; }
            public KeyValuePair<int, int> taverage { get; set; }
            public KeyValuePair<int, int> tmax { get; set; }
        }

        private string dataSource;

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
                        string path = Path.GetFullPath(dataSource);
                        string jsonString = File.ReadAllText(path);

                        List<Record> data = JsonConvert.DeserializeObject<List<Record>>(jsonString);

                        Cells = new List<TableCell>
                        {
                            new TableCell("Machine ID", 0, 1),
                            new TableCell("Time Start", 0, 2),
                            new TableCell("Time End", 0, 3),
                            new TableCell("Tmin", 0, 4),
                            new TableCell("Taverage", 0, 5),
                            new TableCell("Tmax", 0, 6)
                        };
                        for (int i = 0; i < data.Count; i++)
                        {
                            Cells.Add(new TableCell(data[i].machineid.Value.ToString(), i + 1, data[i].machineid.Key));
                            Cells.Add(new TableCell(data[i].timestart.Value, i + 1, data[i].timestart.Key));
                            Cells.Add(new TableCell(data[i].timeend.Value, i + 1, data[i].timeend.Key));
                            Cells.Add(new TableCell(data[i].tmin.Value.ToString(), i + 1, data[i].tmin.Key));
                            Cells.Add(new TableCell(data[i].taverage.Value.ToString(), i + 1, data[i].taverage.Key));
                            Cells.Add(new TableCell(data[i].tmax.Value.ToString(), i + 1, data[i].tmax.Key));
                        }
                        TableCell cellWithMaxRowSpan = Cells.OrderByDescending(cell => cell.RowSpan).FirstOrDefault();
                        RowCount = cellWithMaxRowSpan.RowSpan;
                        ColCount = 6;
                    }
                }
            }
        }
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
        public TableCell(string text, int rowspan, int colspan)
        {
            Text = text;
            RowSpan = rowspan;
            ColSpan = colspan;
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