﻿using Scada.Scheme.Model.DataTypes;
using Scada.Scheme.Model.PropertyGrid;
using System;
using System.Collections.Generic;
using System.Drawing.Design;
using System.Xml;
using CM = System.ComponentModel;

namespace Scada.Scheme.Model
{
    [Serializable]
    public class ChartComponent : StaticText, IDynamicComponent
    {
        public ChartComponent()
            : base()
        {
            // Initialize properties
            Points = new List<Point>();
            ChartType = "line";
            Options = new Dictionary<string, object>();

            BackColorOnHover = "";
            BorderColorOnHover = "";
            ForeColorOnHover = "";
            UnderlineOnHover = false;
            Action = Actions.None;
            ShowValue = ShowValueKinds.ShowWithUnit;
            InCnlNum = 0;
            CtrlCnlNum = 0;
        }

        public List<Point> Points { get; set; }
        public string ChartType { get; set; }
        public Dictionary<string, object> Options { get; set; }

        // Implement required methods and properties



        /// <summary>
        /// Получить или установить цвет фона при наведении указателя мыши
        /// </summary>
        #region Attributes
        [DisplayName("Back color on hover"), Category(Categories.Behavior)]
        [Description("The background color of the component when user rests the pointer on it.")]
        [CM.Editor(typeof(ColorEditor), typeof(UITypeEditor))]
        #endregion
        public string BackColorOnHover { get; set; }

        /// <summary>
        /// Получить или установить цвет рамки при наведении указателя мыши
        /// </summary>
        #region Attributes
        [DisplayName("Border color on hover"), Category(Categories.Behavior)]
        [Description("The border color of the component when user rests the pointer on it.")]
        [CM.Editor(typeof(ColorEditor), typeof(UITypeEditor))]
        #endregion
        public string BorderColorOnHover { get; set; }

        /// <summary>
        /// Получить или установить цвет текста при наведени указателя мыши
        /// </summary>
        #region Attributes
        [DisplayName("Fore color on hover"), Category(Categories.Behavior)]
        [Description("The foreground color of the component, which is used to display text, when user rests the pointer on it.")]
        [CM.Editor(typeof(ColorEditor), typeof(UITypeEditor))]
        #endregion
        public string ForeColorOnHover { get; set; }

        /// <summary>
        /// Получить или установить признак подчёркивания при наведении указателя мыши
        /// </summary>
        #region Attributes
        [DisplayName("Underline on hover"), Category(Categories.Behavior)]
        [Description("Underline text when user rests the pointer on the component.")]
        [CM.DefaultValue(false), CM.TypeConverter(typeof(BooleanConverter))]
        #endregion
        public bool UnderlineOnHover { get; set; }

        /// <summary>
        /// Получить или установить действие
        /// </summary>
        #region Attributes
        [DisplayName("Action"), Category(Categories.Behavior)]
        [Description("The action executed by clicking the left mouse button on the component.")]
        [CM.DefaultValue(Actions.None)]
        #endregion
        public Actions Action { get; set; }

        /// <summary>
        /// Получить или установить признак вывода значения входного канала
        /// </summary>
        #region Attributes
        [DisplayName("Show value"), Category(Categories.Behavior)]
        [Description("Show a value of the input channel associated with the component.")]
        [CM.DefaultValue(ShowValueKinds.ShowWithUnit)]
        #endregion
        public ShowValueKinds ShowValue { get; set; }

        /// <summary>
        /// Получить или установить номер входного канала
        /// </summary>
        #region Attributes
        [DisplayName("Input channel"), Category(Categories.Data)]
        [Description("The input channel number associated with the component.")]
        [CM.DefaultValue(0)]
        #endregion
        public int InCnlNum { get; set; }

        /// <summary>
        /// Получить или установить номер канала управления
        /// </summary>
        #region Attributes
        [DisplayName("Output channel"), Category(Categories.Data)]
        [Description("The output channel number associated with the component.")]
        [CM.DefaultValue(0)]
        #endregion
        public int CtrlCnlNum { get; set; }


        /// <summary>
        /// Загрузить конфигурацию компонента из XML-узла
        /// </summary>
        public override void LoadFromXml(XmlNode xmlNode)
        {
            base.LoadFromXml(xmlNode);

            BackColorOnHover = xmlNode.GetChildAsString("BackColorOnHover");
            BorderColorOnHover = xmlNode.GetChildAsString("BorderColorOnHover");
            ForeColorOnHover = xmlNode.GetChildAsString("ForeColorOnHover");
            UnderlineOnHover = xmlNode.GetChildAsBool("UnderlineOnHover");
            Action = xmlNode.GetChildAsEnum<Actions>("Action");
            ShowValue = xmlNode.GetChildAsEnum<ShowValueKinds>("ShowValue");
            InCnlNum = xmlNode.GetChildAsInt("InCnlNum");
            CtrlCnlNum = xmlNode.GetChildAsInt("CtrlCnlNum");
        }

        /// <summary>
        /// Сохранить конфигурацию компонента в XML-узле
        /// </summary>
        public override void SaveToXml(XmlElement xmlElem)
        {
            base.SaveToXml(xmlElem);

            xmlElem.AppendElem("BackColorOnHover", BackColorOnHover);
            xmlElem.AppendElem("BorderColorOnHover", BorderColorOnHover);
            xmlElem.AppendElem("ForeColorOnHover", ForeColorOnHover);
            xmlElem.AppendElem("UnderlineOnHover", UnderlineOnHover);
            xmlElem.AppendElem("Action", Action);
            xmlElem.AppendElem("ShowValue", ShowValue);
            xmlElem.AppendElem("InCnlNum", InCnlNum);
            xmlElem.AppendElem("CtrlCnlNum", CtrlCnlNum);
        }
    }
}