using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace QBClient
{
    class TimeoutClass
    {
        //wait for timeout seconds, if waitCondition is still satisfied, return true;
        //if this condition is broken, return false
        //somehow a ref must be an assignable variable
        public static bool isTimeout(int timeout, ref bool waitCondition)
        {
            int count = 0;
            int checkInterval = 100;
            while (count < timeout)
            {
                if (waitCondition)
                {
                    count += 100;
                    Thread.Sleep(checkInterval);
                }
                else
                {
                    return false;
                }
            }
            return true;
        }
    }
}
