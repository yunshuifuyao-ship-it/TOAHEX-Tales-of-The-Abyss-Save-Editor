using System;

namespace TOAHEX
{
    public enum Language
    {
        CN = 0,
        JP = 1
    }

    public static class LanguageConfig
    {
        private static Language _current = Language.CN;

        public static Language Current
        {
            get => _current;
            set
            {
                if (_current != value)
                {
                    _current = value;
                    LanguageChanged?.Invoke(null, EventArgs.Empty);
                }
            }
        }

        public static event EventHandler LanguageChanged;
    }
}
