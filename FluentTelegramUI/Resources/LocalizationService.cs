using System;
using System.Globalization;
using System.Resources;
using System.Threading;

namespace FluentTelegramUI.Resources
{
    public class LocalizationService
    {
        private static LocalizationService _instance;
        private ResourceManager _resourceManager;
        private CultureInfo _currentCulture;

        public static LocalizationService Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new LocalizationService();
                }
                return _instance;
            }
        }

        private LocalizationService()
        {
            _resourceManager = new ResourceManager("FluentTelegramUI.Resources.Strings", typeof(LocalizationService).Assembly);
            _currentCulture = CultureInfo.CurrentUICulture;
        }

        public void SetCulture(string cultureName)
        {
            try
            {
                _currentCulture = new CultureInfo(cultureName);
                Thread.CurrentThread.CurrentUICulture = _currentCulture;
            }
            catch (CultureNotFoundException)
            {
                // Fallback to default culture if specified culture is not found
                _currentCulture = CultureInfo.InvariantCulture;
            }
        }

        public string GetString(string key)
        {
            try
            {
                return _resourceManager.GetString(key, _currentCulture) ?? key;
            }
            catch
            {
                return key;
            }
        }

        public string GetString(string key, params object[] args)
        {
            try
            {
                string value = _resourceManager.GetString(key, _currentCulture);
                return value != null ? string.Format(value, args) : key;
            }
            catch
            {
                return key;
            }
        }
    }
} 