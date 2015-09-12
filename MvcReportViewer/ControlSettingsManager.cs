using Microsoft.Reporting.WebForms;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Web.UI.WebControls;

namespace MvcReportViewer
{
    internal class ControlSettingsManager
    {
        private static readonly Dictionary<string, PropertyInfo> UriParameters = new Dictionary<string, PropertyInfo>();

        private static readonly Dictionary<PropertyInfo, string> Properties = new Dictionary<PropertyInfo, string>();

        private readonly bool _isEncrypted;

        static ControlSettingsManager()
        {
            var type = typeof(ControlSettings);
            foreach(var property in type.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
            {
                var uriAttr = property.GetCustomAttributes(typeof(UriParameterAttribute), false)
                                      .FirstOrDefault() as UriParameterAttribute;
                if (uriAttr != null)
                {
                    UriParameters.Add(uriAttr.Name, property);
                    Properties.Add(property, uriAttr.Name);
                }
            }
        }

        public ControlSettingsManager(bool isEncrypted = false)
        {
            _isEncrypted = isEncrypted;
        }

        public IDictionary<string, string> Serialize(ControlSettings settings)
        {
            var activeSettings = new Dictionary<string, string>();
            if (settings == null)
            {
                return activeSettings;
            }

            foreach(var settingsInfo in Properties)
            {
                var property = settingsInfo.Key;
                var value = property.GetValue(settings, null);

                if (value == null)
                {
                    continue;
                }
                
                var uriKey = settingsInfo.Value;
                string serializedSetting;
                if (property.PropertyType == typeof (Color?))
                {
                    serializedSetting = ((Color?) value).Value
                                                        .ToArgb()
                                                        .ToString(CultureInfo.CurrentCulture);
                }
                else
                {
                    serializedSetting = value.ToString();
                }

                activeSettings.Add(uriKey, serializedSetting);
            }

            return activeSettings;
        }

        public bool IsControlSetting(string key)
        {
            return UriParameters.ContainsKey(key);
        }

        public ControlSettings Deserialize(NameValueCollection queryString)
        {
            var settings = new ControlSettings();
            if (queryString == null)
            {
                return settings;
            }

            var actualSettings = queryString.AllKeys.Where(IsControlSetting);
            foreach (var setting in actualSettings)
            {
                var property = UriParameters[setting];
                var value = queryString[setting];
                if (_isEncrypted)
                {
                    value = SecurityUtil.Decrypt(value);
                }

                DeserializeValue(settings, property, value);
            }

            return settings;
        }

        private void DeserializeValue(ControlSettings settings, PropertyInfo property, string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return;
            }

            if (property.PropertyType == typeof (string))
            {
                property.SetValue(settings, value, null);
            }
            else if (property.PropertyType == typeof (int?))
            {
                int numValue;
                if (int.TryParse(value, out numValue))
                {
                    property.SetValue(settings, numValue, null);
                }
            }
            else if (property.PropertyType == typeof (bool?))
            {
                bool boolValue;
                if (bool.TryParse(value, out boolValue))
                {
                    property.SetValue(settings, boolValue, null);
                }
            }
            else if (property.PropertyType == typeof (Color?))
            {
                int argbColor;
                if (int.TryParse(value, out argbColor))
                {
                    var color = Color.FromArgb(argbColor);
                    property.SetValue(settings, color, null);
                }
            }
            else if (property.PropertyType == typeof (Unit?))
            {
                var unit = new Unit(value);
                property.SetValue(settings, unit, null);
            }
            else if (property.PropertyType == typeof (BorderStyle?))
            {
                BorderStyle style;
                if (Enum.TryParse(value, true, out style))
                {
                    property.SetValue(settings, style, null);
                }
            }
            else if (property.PropertyType == typeof (ZoomMode?))
            {
                ZoomMode mode;
                if (Enum.TryParse(value, true, out mode))
                {
                    property.SetValue(settings, mode, null);
                }
            }
            else
            {
                throw new ArgumentException(
                        string.Format("Unknown property {0} type: {1}", property.Name, property.PropertyType));
            }
        }
    }
}
