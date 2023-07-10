﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Crestron.SimplSharp;

/* 
This is a pretty minimal file, I was just doing it to see if the compiler being invoked would optimize
the delegate call we see below

csc v3.5 included with msbuild 3.5 / visual studio 2008 outputs this, which is acceptable:
2008 happily compiles the below to something like this, which is a-OK:

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

in our targets file we happily 
*/
namespace CrestronSandboxProgramLibrarySample
{
    public delegate void DelegateNoParametersEvent(EventArgs args);

    public class Class1
    {
        internal event DelegateNoParametersEvent RecoverNotificationEvent;
        public Class1()
        {
            CrestronConsole.PrintLine("Hello World from the program library!");
            this.RecoverNotificationEvent += new DelegateNoParametersEvent(this.RecoverNotificationEventHandler);
        }

        private void RecoverNotificationEventHandler(EventArgs args)
        {

        }
    }
}