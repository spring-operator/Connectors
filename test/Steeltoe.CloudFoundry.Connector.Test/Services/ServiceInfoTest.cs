﻿//
// Copyright 2015 the original author or authors.
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
// https://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
//

using Steeltoe.CloudFoundry.Connector.App;
using Steeltoe.CloudFoundry.Connector.App.Test;
using System;
using Xunit;

namespace Steeltoe.CloudFoundry.Connector.Services.Test
{
    public class ServiceInfoTest
    {
        [Fact]
        public void Constructor_ThrowsIfIdNull()
        {
            string id = null;

            // Act and Assert
            var ex = Assert.Throws<ArgumentNullException>(() => new TestServiceInfo(id));
            Assert.Contains(nameof(id), ex.Message);
        }

        [Fact]
        public void Constructor_InitializesValues()
        {
            ApplicationInstanceInfo info = new ApplicationInstanceInfo(ApplicationInstanceInfoTest.MakeCloudFoundryApplicationOptions());
            var si = new TestServiceInfo("id", info);
            Assert.Equal("id", si.Id);
            Assert.Equal(info, si.ApplicationInfo);
        }

    }

    class TestServiceInfo : ServiceInfo
    {
        public TestServiceInfo(string id, ApplicationInstanceInfo info) : base(id, info)
        {

        }
        public TestServiceInfo(string id) : base(id, null)
        {

        }
    }
}
