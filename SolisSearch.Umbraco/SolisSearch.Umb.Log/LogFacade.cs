using log4net;
using log4net.Config;
using SolisSearch.Log;
using System;

namespace SolisSearch.Umb.Log
{
    public class LogFacade : ILogFacade
    {
        private ILog log;

        public LogFacade(Type type)
        {
            this.InitLog(type);
        }

        public void AddLogentry(SolisSearch.Log.Enum.LogLevel level, string value, Exception ex = null)
        {
            switch (level)
            {
                case SolisSearch.Log.Enum.LogLevel.Debug:
                    if (!this.log.IsDebugEnabled)
                        break;
                    this.log.Debug((object)value);
                    break;
                case SolisSearch.Log.Enum.LogLevel.Info:
                    if (!this.log.IsInfoEnabled)
                        break;
                    this.log.Info((object)value);
                    break;
                case SolisSearch.Log.Enum.LogLevel.Warn:
                    if (!this.log.IsWarnEnabled)
                        break;
                    if (ex != null)
                    {
                        this.log.Warn((object)value, ex);
                        break;
                    }
                    this.log.Warn((object)value);
                    break;
                case SolisSearch.Log.Enum.LogLevel.Error:
                    if (!this.log.IsErrorEnabled)
                        break;
                    if (ex != null)
                    {
                        this.log.Error((object)value, ex);
                        break;
                    }
                    this.log.Error((object)value);
                    break;
                case SolisSearch.Log.Enum.LogLevel.Fatal:
                    if (!this.log.IsFatalEnabled)
                        break;
                    this.log.Fatal((object)value);
                    break;
                default:
                    throw new ArgumentOutOfRangeException("level");
            }
        }

        public void InitLog(Type type)
        {
            XmlConfigurator.Configure();
            this.log = LogManager.GetLogger(type);
        }
    }
}
