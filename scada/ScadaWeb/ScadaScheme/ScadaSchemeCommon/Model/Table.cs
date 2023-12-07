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

        }

        // Свойства для настройки отображения таблицы
        #region Attributes
        [Scheme.Model.PropertyGrid.DisplayName("Header Color"), Scheme.Model.PropertyGrid.Category(Categories.Appearance)]
        [Scheme.Model.PropertyGrid.Description("The background color of the table header.")]
        [CM.Editor(typeof(ColorEditor), typeof(UITypeEditor))]
        #endregion
        public string HeaderColor { get; set; }

        // Свойства для хранения цвета каждой строки таблицы
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

        // Методы для работы с XML конфигурацией
        public override void LoadFromXml(XmlNode xmlNode)
        {
            base.LoadFromXml(xmlNode);

            HeaderColor = xmlNode.GetChildAsString("HeaderColor");


            RowColor = xmlNode.GetChildAsString("RowColor");

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

            xmlElem.AppendElem("RowColor", RowColor);

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
            clonedComponent.RowColor = string.Copy(RowColor);
            clonedComponent.Cells = new List<TableCell>(Cells.ConvertAll(cell => (TableCell)cell.Clone()));
            return clonedComponent;
        }

        public Actions Action { get; set; }
        public int InCnlNum { get; set; } = 1;//число строк
        public int CtrlCnlNum { get; set; } = 1;//количество столбцов

        public class Record
        {
            public KeyValuePair<int, int> machineid { get; set; }
            public KeyValuePair<int, string> timestart { get; set; }
            public KeyValuePair<int, string> timeend { get; set; }
            public KeyValuePair<int, int> tmin { get; set; }
            public KeyValuePair<int, int> tavaerage { get; set; }
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
                        Cells = new List<TableCell>();

                        for (int i = 0; i < data.Count; i++)
                        {
                            Cells.Add(new TableCell(data[i].machineid.Value.ToString(), i + 1, data[i].machineid.Key));
                            Cells.Add(new TableCell(data[i].timestart.Value, i + 1, data[i].timestart.Key));
                            Cells.Add(new TableCell(data[i].timeend.Value, i + 1, data[i].timeend.Key));
                            Cells.Add(new TableCell(data[i].tmin.Value.ToString(), i + 1, data[i].tmin.Key));
                            Cells.Add(new TableCell(data[i].tavaerage.Value.ToString(), i + 1, data[i].tavaerage.Key));
                            Cells.Add(new TableCell(data[i].tmax.Value.ToString(), i + 1, data[i].tmax.Key));
                        }
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