using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using Crestron.SimplSharp;

/* 
This is a pretty minimal file, I was just doing it to see if the compiler being invoked would optimize
the delegate call we see below

csc v3.5 included with msbuild 3.5 / visual studio 2008 outputs this, which is acceptable:
2008 happily compiles the below class to something like this, which is a-OK:

[MethodImpl(MethodImplOptions.Synchronized)]
internal void add_RecoverNotificationEvent(DelegateNoParametersEvent value)
{
    this.RecoverNotificationEvent = (DelegateNoParametersEvent)Delegate.Combine(this.RecoverNotificationEvent, value);
}

Whereas *any* newer csc (starting at 4.0) or Roslyn outputs this, 
which utilizes Interlocked and that is not sandbox compatible:
[CompilerGenerated]
internal void add_RecoverNotificationEvent(DelegateNoParametersEvent value)
{
    DelegateNoParametersEvent delegateNoParametersEvent = this.RecoverNotificationEvent;
    DelegateNoParametersEvent delegateNoParametersEvent2;
    do
    {
        delegateNoParametersEvent2 = delegateNoParametersEvent;
        DelegateNoParametersEvent delegateNoParametersEvent3 = (DelegateNoParametersEvent)Delegate.Combine(delegateNoParametersEvent2, value);
        delegateNoParametersEvent = Interlocked.CompareExchange<DelegateNoParametersEvent>(ref this.RecoverNotificationEvent, delegateNoParametersEvent3, delegateNoParametersEvent2);
    }
    while (delegateNoParametersEvent != delegateNoParametersEvent2);
}

*/
namespace CrestronSandboxProgramSample 
{
    public delegate void DelegateNoParametersEvent(EventArgs args);

    public class Class1
    {
        /*
        private DelegateNoParametersEvent _delegateNoParametersEvent;
        public event DelegateNoParametersEvent RecoverNotificationEvent
        {
            [MethodImpl(MethodImplOptions.Synchronized)]
            add => _delegateNoParametersEvent += (DelegateNoParametersEvent)Delegate.Combine(_delegateNoParametersEvent, value);
            [MethodImpl(MethodImplOptions.Synchronized)]
            remove => _delegateNoParametersEvent -= (DelegateNoParametersEvent)Delegate.Remove(_delegateNoParametersEvent, value);
        }
        */
        public event DelegateNoParametersEvent RecoverNotificationEvent;

        public Class1()
        {
            CrestronConsole.PrintLine("Hello World from the library!");
            this.RecoverNotificationEvent += new DelegateNoParametersEvent(this.RecoverNotificationEventHandler);
        }

        public void TestMethod()
        {
            CrestronConsole.PrintLine("Hello from TestMethod!");
            //_delegateNoParametersEvent?.Invoke(EventArgs.Empty);
            RecoverNotificationEvent?.Invoke(EventArgs.Empty);
        }

        private void RecoverNotificationEventHandler(EventArgs args)
        {
            CrestronConsole.PrintLine("RecoverNotificationEventHandler");
        }
    }
}