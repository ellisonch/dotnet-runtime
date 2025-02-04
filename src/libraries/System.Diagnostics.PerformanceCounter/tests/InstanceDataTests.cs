// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Collections;
using System.Collections.Specialized;
using Xunit;

namespace System.Diagnostics.Tests
{
    public static class InstanceDataTests
    {
        [Fact]
        public static void InstanceData_CreateInstanceData_FromCounterSample()
        {
            long timestamp = DateTime.Now.ToFileTime();
            CounterSample cs = new CounterSample(1, 2, 3, 4, timestamp, timestamp, PerformanceCounterType.SampleFraction);

            InstanceData id = new InstanceData("foo", cs);
            Assert.Equal(cs.RawValue, id.Sample.RawValue);
            Assert.Equal(cs.CounterType, id.Sample.CounterType);
            Assert.Equal(1, id.RawValue);
        }

        [Fact]
        public static void InstanceDataCollection_GetItem_ExistingCounter()
        {
            InstanceDataCollection idc = GetInstanceDataCollection();

            InstanceData[] ids = new InstanceData[idc.Count];
            idc.CopyTo(ids, 0);

            Assert.Equal("% User Time", idc.CounterName);

            for (int i = 0; i < idc.Count; i++)
            {
                string instanceName = ids[i].InstanceName;
                Assert.Equal(instanceName, idc[instanceName].InstanceName);
                Assert.Equal(ids[i].RawValue, idc[instanceName].RawValue);
                Assert.True(idc.Contains(instanceName));
            }
        }

        [Fact]
        public static void InstanceDataCollection_NullTest()
        {
            InstanceDataCollection idc = GetInstanceDataCollection();

            Assert.Throws<ArgumentNullException>(() => idc[null]);
            Assert.Throws<ArgumentNullException>(() => idc.Contains(null));
        }

        [Fact]
        public static void InstanceDataCollection_GetKeys()
        {
            InstanceDataCollection idc = GetInstanceDataCollection();

            string[] keys = new string[idc.Count];
            idc.Keys.CopyTo(keys, 0);

            Assert.True(keys.Length > 0);
        }

        [Fact]
        public static void InstanceDataCollection_GetValues()
        {
            InstanceDataCollection idc = GetInstanceDataCollection();

            InstanceData[] values = new InstanceData[idc.Count];
            idc.Values.CopyTo(values, 0);

            Assert.True(values.Length > 0);
        }

        [Fact]
        public static void InstanceDataCollectionCollection_GetItem_Invalid()
        {
            InstanceDataCollectionCollection idcc = GetInstanceDataCollectionCollection();

            Assert.Throws<ArgumentNullException>(() => idcc[null]);
        }

        [Fact]
        public static void InstanceDataCollectionCollection_GetKeys()
        {
            InstanceDataCollectionCollection idcc = GetInstanceDataCollectionCollection();

            Assert.True(idcc.Keys.Count > 0);
        }

        [Fact]
        public static void InstanceDataCollectionCollection_GetValues()
        {
            InstanceDataCollectionCollection idcc = GetInstanceDataCollectionCollection();

            Assert.True(idcc.Values.Count > 0);
        }

        [Fact]
        public static void InstanceDataCollectionCollection_Contains_Valid()
        {
            InstanceDataCollectionCollection idcc = GetInstanceDataCollectionCollection();

            Assert.False(idcc.Contains("Not a real instance"));
        }

        [Fact]
        public static void InstanceDataCollectionCollection_Contains_inValid()
        {
            InstanceDataCollectionCollection idcc = GetInstanceDataCollectionCollection();

            Assert.Throws<ArgumentNullException>(() => idcc.Contains(null));
        }

        [Fact]
        public static void InstanceDataCollectionCollection_CopyTo()
        {
            InstanceDataCollectionCollection idcc = GetInstanceDataCollectionCollection();

            InstanceDataCollection[] idc = new InstanceDataCollection[idcc.Values.Count];
            idcc.CopyTo(idc, 0);
            Assert.True(idc.Length > 0);
        }

        public static InstanceDataCollectionCollection GetInstanceDataCollectionCollection()
        {
            PerformanceCounterCategory pcc = Helpers.RetryOnAllPlatforms(() => new PerformanceCounterCategory("Processor"));
            return Helpers.RetryOnAllPlatforms(() =>
            {
                var idcc = pcc.ReadCategory();
                Assert.InRange(idcc.Values.Count, 1, int.MaxValue);
                return idcc;
            });
        }

        public static InstanceDataCollection GetInstanceDataCollection()
        {
            InstanceDataCollectionCollection idcc = GetInstanceDataCollectionCollection();
            return idcc["% User Time"];
        }
    }
}
