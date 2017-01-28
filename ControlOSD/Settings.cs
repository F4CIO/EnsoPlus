﻿using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Reflection;
using System.Xml.Serialization;


using Common;
using System.Drawing;
using System.Xml;
using ControlOSD;
using CraftSynth.BuildingBlocks.IO.Xml.CustomXmlSerializer;
using CustomXmlSerializerTester;

namespace OSD
{
    public class Settings
    {
        #region UserData
        
        //Put settings fields here with their default values

        //public string A = "[";

		public CraftSynth.BuildingBlocks.UI.WindowsForms.FormDisplayer.ShowHideEffect showHideEffect = (CraftSynth.BuildingBlocks.UI.WindowsForms.FormDisplayer.ShowHideEffect)DefaultSettings.Default.ShowHideEffect;
        public TimeSpan showHideEffectDuration = DefaultSettings.Default.ShowHideEffectDuration;
        public TimeSpan showHideEffectUpdateInterval = DefaultSettings.Default.ShowHideEffectUpdateInterval;

        public int margins = DefaultSettings.Default.Margins;

        public double backgroundOpacity = DefaultSettings.Default.BackgroundOpacity;
        public Color backgroundColor = DefaultSettings.Default.BackgroundColor;
        public int backgroundMarginsSize = DefaultSettings.Default.BackgroundMarginsSize;

        public Size size = DefaultSettings.Default.Size;
        public Point location = DefaultSettings.Default.Location;

        public FrameType frameType = (FrameType)DefaultSettings.Default.FrameType;
        public Color frameColor = DefaultSettings.Default.FrameColor;
        public int frameThickness = DefaultSettings.Default.FrameThickness;
        
        public SerializibleFont mainHeaderFont = new SerializibleFont(DefaultSettings.Default.MainHeaderFont);        
        public Color mainHeaderForeColor = DefaultSettings.Default.MainHeaderForeColor;
        public Color mainHeaderBackColor = DefaultSettings.Default.MainHeaderBackColor;
        public int mainHeaderHeight = DefaultSettings.Default.MainHeaderHeight;

        public SerializibleFont normalFont = new SerializibleFont(DefaultSettings.Default.NormalFont);        
        public Color normalTextForeColor = DefaultSettings.Default.NormalTextForeColor;
        public Color normalTextBackColor = DefaultSettings.Default.NormalTextBackColor;
        public Color selectedTextForeColor = DefaultSettings.Default.SelectedTextForeColor;
        public Color selectedTextBackColor = DefaultSettings.Default.SelectedTextBackColor;
        
        public ContentAlignment menuHeaderAlignment = DefaultSettings.Default.MenuHeaderAlignment;        
        public ContentAlignment menuItemsAlignment = DefaultSettings.Default.MenuItemsAlignment;
        public int menuItemHeight = DefaultSettings.Default.MenuItemHeight;
        public int menuMaximalDisplayedItemsCount = DefaultSettings.Default.MenuMaximalDisplayedItemsCount;
        
        public string menuArrowTopText = DefaultSettings.Default.MenuArrowTopText;
        public string menuArrowBottomText = DefaultSettings.Default.MenuArrowBottomText;
        public string menuArrowRightText = DefaultSettings.Default.MenuArrowLeftText;
  
        /// <summary>
        /// If settings file not exist this constructors set default values (after inline constrictors from above)
        /// </summary>
        #region Settings fields first-time-constructors

        //private void InitA()
        //{
        //    string val = null;
        //    try
        //    {                
        //        this.A = "aa";
        //    }
        //    catch(Exception exception)
        //    {
        //        ExceptionHandler.Handle(exception, ExceptionHandlingPolicies.Log_and_Block_Policy);
        //    }           
        //}
        #endregion 

        public Settings()
        {
            //this.InitA();
        }
        #endregion UserData

        private static Settings _current;
        public static Settings Current
        {
            get
            {
                if (_current == null)
                {
                    Initialize();
                }
                return _current;
            }
        }

        /// <summary>
        /// If settings exists loads it, if not loads default values.
        /// </summary>
        public static void Initialize()
        {
            if (!SettingsExist())
            {
                _current = new Settings();
                Save(_current);                
            }
            else
            {
                _current = Load();
            }
        }

        public static string FilePath = "OSD.settings";        

        /// <summary>
        /// Tells if settings are present in data source.
        /// </summary>
        /// <returns></returns>
        protected static bool SettingsExist()
        {
            return File.Exists(FilePath);
        }

        /// <summary>
        /// Loads settings from data source and returns it.
        /// </summary>
        /// <returns></returns>
        protected static Settings Load()
        {
            Settings settings = null;
            try
            {
                XmlDocument doc = new XmlDocument();
                doc.Load(Path.Combine(CraftSynth.BuildingBlocks.Common.Misc.ApplicationRootFolderPath, Settings.FilePath));
                settings = (Settings)CustomXmlDeserializer.Deserialize(doc.OuterXml, 1, new TestMeTypeConverter());  
                    //Helper.Deserialize(typeof(Settings), FilePath);                
            }
            catch (Exception exception)
            {
				Common.Logging.AddExceptionLog(exception);
            }
            return settings;
        }

        /// <summary>
        /// Saves current settings to data source.
        /// </summary>
        public static void Save()
        {
            Save(Settings.Current);
        }

        /// <summary>
        /// Saves provided settings to data source.
        /// </summary>
        protected static void Save(Settings settings)
        {
            try
            {
                XmlDocument doc = CustomXmlSerializer.Serialize(settings, 1, "Test1");
                doc.Save(Settings.FilePath);
                //Helper.Serialize(settings, FilePath);
            }
            catch (Exception exception)
            {
				Common.Logging.AddExceptionLog(exception);
            }
        }
    }
}
