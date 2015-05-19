using System;

namespace COG.Logging
{
    public abstract class Logger
    {
        private static Logger _instance;

        private string module;

        protected internal Logger()
        {
            if (_instance == null)
            {
                _instance = this;
            }
            else
            {
                throw new Exception("There was already an instance of logger initialized");
            }
        }

        private Logger(Type type)
        {
            module = type.Name;
        }

        #region Methods
        public void info(string message)
        {
            log(module, "info", message);
        }

        public void info(string message, params object[] args)
        {
            log(module, "info", string.Format(message, args));
        }

        public void warn(string message)
        {
            log(module, "warn", message);
        }

        public void warn(string message, params object[] args)
        {
            log(module, "warn", string.Format(message, args));
        }

        public void error(string message)
        {
            log(module, "error", message);
        }

        public void error(string message, params object[] args)
        {
            log(module, "error", string.Format(message, args));
        }

        protected abstract void log(string module, string type, string message);
        #endregion Methods

        public static Logger GetLogger(Type type)
        {
            return new LogWrapper(type);
        }

        private class LogWrapper : Logger
        {
            public LogWrapper(Type type) : base(type) { }

            protected override void log(string module, string type, string message)
            {
                if (_instance != null)
                    _instance.log(module, type, message);
            }
        }

    }
}
