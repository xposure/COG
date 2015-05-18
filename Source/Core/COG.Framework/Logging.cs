//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;

//namespace COG.Framework
//{
//    #region ILogger
//    public interface ILogger
//    {
//        void Info(string message);
//        void Info(string message, params object[] args);
//        void Info(SimpleUri uri, string message);
//        void Info(SimpleUri uri, string message, params object[] args);

//        void Warn(string message);
//        void Warn(string message, params object[] args);
//        void Warn(SimpleUri uri, string message);
//        void Warn(SimpleUri uri, string message, params object[] args);

//        void Error(string message);
//        void Error(string message, params object[] args);
//        void Error(SimpleUri uri, string message);
//        void Error(SimpleUri uri, string message, params object[] args);
//    }
//    #endregion

//    #region AbstractLogger
//    public abstract class AbstractLogger : ILogger
//    {
//        protected abstract void info(string message);

//        public void Info(string message)
//        {
//            Info(SimpleUri.NULL, message);
//        }

//        public void Info(string message, params object[] args)
//        {
//            Info(string.Format(message, args));
//        }

//        public void Info(SimpleUri uri, string message)
//        {
//            info(string.Format("INFO :: {0} -> {1}", uri, message));
//        }

//        public void Info(SimpleUri uri, string message, params object[] args)
//        {
//            Info(uri, string.Format(message, args));
//        }

//        protected abstract void warn(string message);

//        public void Warn(string message)
//        {
//            Warn(SimpleUri.NULL, message);
//        }

//        public void Warn(string message, params object[] args)
//        {
//            Warn(string.Format(message, args));
//        }

//        public void Warn(SimpleUri uri, string message)
//        {
//            warn(string.Format("WARN :: {0} -> {1}", uri, message));
//        }

//        public void Warn(SimpleUri uri, string message, params object[] args)
//        {
//            Warn(uri, string.Format(message, args));
//        }

//        protected abstract void error(string message);

//        public void Error(string message)
//        {
//            Error(SimpleUri.NULL, message);
//        }

//        public void Error(string message, params object[] args)
//        {
//            Error(string.Format(message, args));
//        }

//        public void Error(SimpleUri uri, string message)
//        {
//            error(string.Format("ERR  :: {0} -> {1}", uri, message));
//        }

//        public void Error(SimpleUri uri, string message, params object[] args)
//        {
//            Error(uri, string.Format(message, args));
//        }
//    }
//    #endregion
//}
