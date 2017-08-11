using System;

namespace SolisSearch.Log
{
    public interface ILogFacade
    {
        void AddLogentry(SolisSearch.Log.Enum.LogLevel level, string value, Exception ex = null);
    }
}
