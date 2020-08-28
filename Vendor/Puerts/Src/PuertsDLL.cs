﻿/*
* Tencent is pleased to support the open source community by making Puerts available.
* Copyright (C) 2020 THL A29 Limited, a Tencent company.  All rights reserved.
* Puerts is licensed under the BSD 3-Clause License, except for the third-party components listed in the file 'LICENSE' which may be subject to their corresponding license terms. 
* This file is subject to the terms and conditions defined in file 'LICENSE', which is part of this source code package.
*/

using System;
using System.Runtime.InteropServices;
using System.Text;

namespace Puerts
{
#pragma warning disable 414
    public class MonoPInvokeCallbackAttribute : System.Attribute
    {
        private Type type;
        public MonoPInvokeCallbackAttribute(Type t)
        {
            type = t;
        }
    }
#pragma warning restore 414

    //[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void V8FunctionCallback(IntPtr isolate, IntPtr info, IntPtr self, int paramLen, long data);

    //[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate IntPtr V8ConstructorCallback(IntPtr isolate, IntPtr info, int paramLen, long data);

    //[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void V8DestructorCallback(IntPtr self, long data);

    public delegate void V8IndexedGetterCallback(IntPtr isolate, IntPtr info, IntPtr self, uint index, long data);

    public delegate void V8IndexedSetterCallback(IntPtr isolate, IntPtr info, IntPtr self, uint index, IntPtr value, long data);

    public delegate void LogCallback(string content);

    [Flags]
    public enum JsValueType
    {
        NullOrUndefined = 1,
        BigInt = 2,
        Number = 4,
        String = 8,
        Boolean = 16,
        NativeObject = 32,
        JsObject = 64,
        Array = 128,
        Function = 256,
        Date = 512,
        Unknow = 1024,
        Any = NullOrUndefined | BigInt | Number | String | Boolean | NativeObject | Array | Function | Date,
    };

    public class PuertsDLL
    {
#if (UNITY_IPHONE || UNITY_TVOS || UNITY_WEBGL || UNITY_SWITCH) && !UNITY_EDITOR
        const string DLLNAME = "__Internal";
#else
        const string DLLNAME = "puerts";
#endif

        [DllImport(DLLNAME, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr CreateJSEngine();

        [DllImport(DLLNAME, CallingConvention = CallingConvention.Cdecl)]
        public static extern void DestroyJSEngine(IntPtr isolate);

        [DllImport(DLLNAME, CallingConvention = CallingConvention.Cdecl)]
        public static extern void SetGlobalFunction(IntPtr isolate, string name, V8FunctionCallback v8FunctionCallback, long data);

        private static string GetStringFromNative(IntPtr str, int strlen)
        {
            if (str != IntPtr.Zero)
            {
#if XLUA_GENERAL || (UNITY_WSA && !UNITY_EDITOR)
                int len = strlen.ToInt32();
                byte[] buffer = new byte[len];
                Marshal.Copy(str, buffer, 0, len);
                return Encoding.UTF8.GetString(buffer);
#else
                string ret = Marshal.PtrToStringAnsi(str, strlen);
                if (ret == null)
                {
                    int len = strlen;
                    byte[] buffer = new byte[len];
                    Marshal.Copy(str, buffer, 0, len);
                    return Encoding.UTF8.GetString(buffer);
                }
                return ret;
#endif
            }
            else
            {
                return null;
            }
        }

        [DllImport(DLLNAME, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr GetLastExceptionInfo(IntPtr isolate, out int strlen);

        public static string GetLastExceptionInfo(IntPtr isolate)
        {
            int strlen;
            IntPtr str = GetLastExceptionInfo(isolate, out strlen);
            return GetStringFromNative(str, strlen);
        }

        [DllImport(DLLNAME, CallingConvention = CallingConvention.Cdecl)]
        public static extern void LowMemoryNotification(IntPtr isolate);

        [DllImport(DLLNAME, CallingConvention = CallingConvention.Cdecl)]
        public static extern void SetGeneralDestructor(IntPtr isolate, V8DestructorCallback generalDestructor);


        [DllImport(DLLNAME, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr Eval(IntPtr isolate, string code, string path);

        [DllImport(DLLNAME, CallingConvention = CallingConvention.Cdecl)]
        public static extern int RegisterClass(IntPtr isolate, int BaseTypeId, string fullName, V8ConstructorCallback constructor, V8DestructorCallback destructor, long data);

        [DllImport(DLLNAME, CallingConvention = CallingConvention.Cdecl)]
        public static extern int RegisterStruct(IntPtr isolate, int BaseTypeId, string fullName, V8ConstructorCallback constructor, V8DestructorCallback destructor, long data, int size);

        //切记注册的回调不能抛C#异常，必须先catch，然后转js异常
        [DllImport(DLLNAME, CallingConvention = CallingConvention.Cdecl)]
        public static extern bool RegisterFunction(IntPtr isolate, int classID, string name, bool isStatic, V8FunctionCallback callback, long data);

        [DllImport(DLLNAME, CallingConvention = CallingConvention.Cdecl)]
        public static extern bool RegisterProperty(IntPtr isolate, int classID, string name, bool isStatic, V8FunctionCallback getter, long getterData, V8FunctionCallback setter, long setterData);

        [DllImport(DLLNAME, CallingConvention = CallingConvention.Cdecl)]
        public static extern bool RegisterIndexedProperty(IntPtr isolate, int classID, V8IndexedGetterCallback getter, V8IndexedSetterCallback setter, long data);

        [DllImport(DLLNAME, CallingConvention = CallingConvention.Cdecl)]
        public static extern void ReturnClass(IntPtr isolate, IntPtr info, int classID);

        [DllImport(DLLNAME, CallingConvention = CallingConvention.Cdecl)]
        public static extern void ReturnObject(IntPtr isolate, IntPtr info, int classID, IntPtr self);

        [DllImport(DLLNAME, CallingConvention = CallingConvention.Cdecl)]
        public static extern void ReturnNumber(IntPtr isolate, IntPtr info, double number);

        [DllImport(DLLNAME, CallingConvention = CallingConvention.Cdecl)]
        public static extern void ReturnString(IntPtr isolate, IntPtr info, string str);

        [DllImport(DLLNAME, CallingConvention = CallingConvention.Cdecl)]
        public static extern void ReturnBigInt(IntPtr isolate, IntPtr info, long number);

        [DllImport(DLLNAME, CallingConvention = CallingConvention.Cdecl)]
        public static extern void ReturnBoolean(IntPtr isolate, IntPtr info, bool b);

        [DllImport(DLLNAME, CallingConvention = CallingConvention.Cdecl)]
        public static extern void ReturnDate(IntPtr isolate, IntPtr info, double date);

        [DllImport(DLLNAME, CallingConvention = CallingConvention.Cdecl)]
        public static extern void ReturnNull(IntPtr isolate, IntPtr info);

        [DllImport(DLLNAME, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr GetArgumentValue(IntPtr info, int index);

        [DllImport(DLLNAME, CallingConvention = CallingConvention.Cdecl)]
        public static extern JsValueType GetJsValueType(IntPtr isolate, IntPtr value, bool isByRef);

        [DllImport(DLLNAME, CallingConvention = CallingConvention.Cdecl)]
        public static extern JsValueType GetArgumentType(IntPtr isolate, IntPtr info, int index, bool isByRef);

        [DllImport(DLLNAME, CallingConvention = CallingConvention.Cdecl)]
        public static extern double GetNumberFromValue(IntPtr isolate, IntPtr value, bool isByRef);

        [DllImport(DLLNAME, CallingConvention = CallingConvention.Cdecl)]
        public static extern double GetDateFromValue(IntPtr isolate, IntPtr value, bool isByRef);

        [DllImport(DLLNAME, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr GetStringFromValue(IntPtr isolate, IntPtr value, out int len, bool isByRef);

        public static string GetStringFromValue(IntPtr isolate, IntPtr value, bool isByRef)
        {
            int strlen;
            IntPtr str = GetStringFromValue(isolate, value, out strlen, isByRef);
            return GetStringFromNative(str, strlen);
        }

        [DllImport(DLLNAME, CallingConvention = CallingConvention.Cdecl)]
        public static extern bool GetBooleanFromValue(IntPtr isolate, IntPtr value, bool isByRef);

        [DllImport(DLLNAME, CallingConvention = CallingConvention.Cdecl)]
        public static extern long GetBigIntFromValue(IntPtr isolate, IntPtr value, bool isByRef);

        [DllImport(DLLNAME, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr GetObjectFromValue(IntPtr isolate, IntPtr value, bool isByRef);

        [DllImport(DLLNAME, CallingConvention = CallingConvention.Cdecl)]
        public static extern int GetTypeIdFromValue(IntPtr isolate, IntPtr value, bool isByRef);

        [DllImport(DLLNAME, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr GetFunctionFromValue(IntPtr isolate, IntPtr value, bool isByRef);

        [DllImport(DLLNAME, CallingConvention = CallingConvention.Cdecl)]
        public static extern void SetNumberToOutValue(IntPtr isolate, IntPtr value, double number);

        [DllImport(DLLNAME, CallingConvention = CallingConvention.Cdecl)]
        public static extern void SetDateToOutValue(IntPtr isolate, IntPtr value, double date);

        [DllImport(DLLNAME, CallingConvention = CallingConvention.Cdecl)]
        public static extern void SetStringToOutValue(IntPtr isolate, IntPtr value, string str);

        [DllImport(DLLNAME, CallingConvention = CallingConvention.Cdecl)]
        public static extern void SetBooleanToOutValue(IntPtr isolate, IntPtr value, bool b);

        [DllImport(DLLNAME, CallingConvention = CallingConvention.Cdecl)]
        public static extern void SetBigIntToOutValue(IntPtr isolate, IntPtr value, long bigInt);

        [DllImport(DLLNAME, CallingConvention = CallingConvention.Cdecl)]
        public static extern void SetObjectToOutValue(IntPtr isolate, IntPtr value, int classId, IntPtr ptr);

        [DllImport(DLLNAME, CallingConvention = CallingConvention.Cdecl)]
        public static extern void SetNullToOutValue(IntPtr isolate, IntPtr value);

        [DllImport(DLLNAME, CallingConvention = CallingConvention.Cdecl)]
        public static extern void ThrowException(IntPtr isolate, string message);

        //begin cs call js
        [DllImport(DLLNAME, CallingConvention = CallingConvention.Cdecl)]
        public static extern void PushNullForJSFunction(IntPtr function);

        [DllImport(DLLNAME, CallingConvention = CallingConvention.Cdecl)]
        public static extern void PushDateForJSFunction(IntPtr function, double dateValue);

        [DllImport(DLLNAME, CallingConvention = CallingConvention.Cdecl)]
        public static extern void PushBooleanForJSFunction(IntPtr function, bool b);

        [DllImport(DLLNAME, CallingConvention = CallingConvention.Cdecl)]
        public static extern void PushBigIntForJSFunction(IntPtr function, long l);

        [DllImport(DLLNAME, CallingConvention = CallingConvention.Cdecl)]
        public static extern void PushStringForJSFunction(IntPtr function, string str);

        [DllImport(DLLNAME, CallingConvention = CallingConvention.Cdecl)]
        public static extern void PushNumberForJSFunction(IntPtr function, double d);

        [DllImport(DLLNAME, CallingConvention = CallingConvention.Cdecl)]
        public static extern void PushObjectForJSFunction(IntPtr function, int classId, IntPtr objectId);

        [DllImport(DLLNAME, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr InvokeJSFunction(IntPtr function, bool hasResult);

        [DllImport(DLLNAME, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr GetFunctionLastExceptionInfo(IntPtr function, out int len);

        [DllImport(DLLNAME, CallingConvention = CallingConvention.Cdecl)]
        public static extern void ReleaseJSFunction(IntPtr isolate, IntPtr function);

        public static string GetFunctionLastExceptionInfo(IntPtr function)
        {
            int strlen;
            IntPtr str = GetFunctionLastExceptionInfo(function, out strlen);
            return GetStringFromNative(str, strlen);
        }

        //保守方案
        [DllImport(DLLNAME, CallingConvention = CallingConvention.Cdecl)]
        public static extern JsValueType GetResultType(IntPtr resultInfo);

        [DllImport(DLLNAME, CallingConvention = CallingConvention.Cdecl)]
        public static extern double GetNumberFromResult(IntPtr resultInfo);

        [DllImport(DLLNAME, CallingConvention = CallingConvention.Cdecl)]
        public static extern double GetDateFromResult(IntPtr resultInfo);

        [DllImport(DLLNAME, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr GetStringFromResult(IntPtr resultInfo, out int len);

        public static string GetStringFromResult(IntPtr resultInfo)
        {
            int strlen;
            IntPtr str = GetStringFromResult(resultInfo, out strlen);
            return GetStringFromNative(str, strlen);
        }

        [DllImport(DLLNAME, CallingConvention = CallingConvention.Cdecl)]
        public static extern bool GetBooleanFromResult(IntPtr resultInfo);

        [DllImport(DLLNAME, CallingConvention = CallingConvention.Cdecl)]
        public static extern long GetBigIntFromResult(IntPtr resultInfo);

        [DllImport(DLLNAME, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr GetObjectFromResult(IntPtr resultInfo);

        [DllImport(DLLNAME, CallingConvention = CallingConvention.Cdecl)]
        public static extern int GetTypeIdFromResult(IntPtr resultInfo);

        [DllImport(DLLNAME, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr GetFunctionFromResult(IntPtr resultInfo);

        [DllImport(DLLNAME, CallingConvention = CallingConvention.Cdecl)]
        public static extern void ResetResult(IntPtr resultInfo);

        [DllImport(DLLNAME, CallingConvention = CallingConvention.Cdecl)]
        public static extern void PropertyReturnObject(IntPtr isolate, IntPtr info, int classID, IntPtr self);

        [DllImport(DLLNAME, CallingConvention = CallingConvention.Cdecl)]
        public static extern void PropertyReturnNumber(IntPtr isolate, IntPtr info, double number);

        [DllImport(DLLNAME, CallingConvention = CallingConvention.Cdecl)]
        public static extern void PropertyReturnString(IntPtr isolate, IntPtr info, string str);

        [DllImport(DLLNAME, CallingConvention = CallingConvention.Cdecl)]
        public static extern void PropertyReturnBigInt(IntPtr isolate, IntPtr info, long number);

        [DllImport(DLLNAME, CallingConvention = CallingConvention.Cdecl)]
        public static extern void PropertyReturnBoolean(IntPtr isolate, IntPtr info, bool b);

        [DllImport(DLLNAME, CallingConvention = CallingConvention.Cdecl)]
        public static extern void PropertyReturnDate(IntPtr isolate, IntPtr info, double date);

        [DllImport(DLLNAME, CallingConvention = CallingConvention.Cdecl)]
        public static extern void PropertyReturnNull(IntPtr isolate, IntPtr info);

        //end cs call js

        [DllImport(DLLNAME, CallingConvention = CallingConvention.Cdecl)]
        public static extern void CreateInspector(IntPtr isolate, int port);

        [DllImport(DLLNAME, CallingConvention = CallingConvention.Cdecl)]
        public static extern void DestroyInspector(IntPtr isolate);

        [DllImport(DLLNAME, CallingConvention = CallingConvention.Cdecl)]
        public static extern void InspectorTick(IntPtr isolate);

        [DllImport(DLLNAME, CallingConvention = CallingConvention.Cdecl)]
        public static extern void SetLogCallback(LogCallback log, LogCallback logWarning, LogCallback logError);
    }
}


