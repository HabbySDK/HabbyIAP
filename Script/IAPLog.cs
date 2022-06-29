namespace Habby.Business
{
    public class IAPLog
    {
        public static string LogTag = "HabbyIAP";

        public static void LogJson(string pKey,object pobj)
        {
            HabbyLog.LogJson(LogTag,pKey, pobj);
        }
        public static void Log(object pobj)
        {
            HabbyLog.Log(LogTag, pobj);
        }

        public static void LogWarning(object pobj)
        {
            HabbyLog.LogWarning(LogTag, pobj);
        }

        public static void LogError(object pobj)
        {
            HabbyLog.LogError(LogTag, pobj);
        }

        public static void LogAssertion(object pobj)
        {
            HabbyLog.LogAssertion(LogTag, pobj);
        }

        public static void LogException(string msg, System.Exception error)
        {
            HabbyLog.LogException(LogTag, msg, error);
        }

        public static void LogFormat(string format, params object[] args)
        {
            HabbyLog.LogFormat(LogTag, format, args);
        }

        public static void LogWarningFormat(string format, params object[] args)
        {
            HabbyLog.LogWarningFormat(LogTag, format, args);
        }

        public static void LogErrorFormat(string format, params object[] args)
        {
            HabbyLog.LogErrorFormat(LogTag, format, args);
        }
    }
}