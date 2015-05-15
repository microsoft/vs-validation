/********************************************************
*                                                        *
*   © Copyright (C) Microsoft. All rights reserved.      *
*                                                        *
*********************************************************/

namespace System.ComponentModel
{
    [AttributeUsage(AttributeTargets.All)]
    internal sealed class LocalizableAttribute : Attribute
    {
        public LocalizableAttribute(bool isLocalizable) { }
    }
}
