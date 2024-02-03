using System;
using System.Diagnostics;

namespace MasonVeteransMemorial.BusinessServices.Base
{
    [DebuggerStepThrough]
    public abstract class SingletonBase<T, TI> where T : TI, new() where TI : class
    {
        //
        // Static Fields
        //
        protected static readonly object _ConcurrencyLock = new object();

        private static volatile TI _Current;

        //
        // Static Properties
        //
        public static TI Current
        {
            get
            {
                bool flag = SingletonBase<T, TI>._Current == null;
                if (flag)
                {
                    object concurrencyLock = SingletonBase<T, TI>._ConcurrencyLock;
                    lock (concurrencyLock)
                    {
                        bool flag3 = SingletonBase<T, TI>._Current == null;
                        if (flag3)
                        {
                            SingletonBase<T, TI>._Current = (TI)((object)Activator.CreateInstance<T>());
                        }
                    }
                }
                return SingletonBase<T, TI>._Current;
            }
            set
            {
                object concurrencyLock = SingletonBase<T, TI>._ConcurrencyLock;
                lock (concurrencyLock)
                {
                    SingletonBase<T, TI>._Current = value;
                }
            }
        }
    }
}
