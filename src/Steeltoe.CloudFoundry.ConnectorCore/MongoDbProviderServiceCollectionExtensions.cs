﻿// Copyright 2017 the original author or authors.
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
// http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Steeltoe.CloudFoundry.Connector.Services;
using Steeltoe.Common.HealthChecks;
using System;

namespace Steeltoe.CloudFoundry.Connector.MongoDb
{
    public static class MongoDbProviderServiceCollectionExtensions
    {
        /// <summary>
        /// Add MongoDb to a ServiceCollection
        /// </summary>
        /// <param name="services">Service collection to add to</param>
        /// <param name="config">App configuration</param>
        /// <param name="contextLifetime">Lifetime of the service to inject</param>
        /// <returns>IServiceCollection for chaining</returns>
        public static IServiceCollection AddMongoClient(this IServiceCollection services, IConfiguration config, ServiceLifetime contextLifetime = ServiceLifetime.Singleton)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            if (config == null)
            {
                throw new ArgumentNullException(nameof(config));
            }

            MongoDbServiceInfo info = config.GetSingletonServiceInfo<MongoDbServiceInfo>();

            DoAdd(services, info, config, contextLifetime);
            return services;
        }

        /// <summary>
        /// Add MongoDb to a ServiceCollection
        /// </summary>
        /// <param name="services">Service collection to add to</param>
        /// <param name="config">App configuration</param>
        /// <param name="serviceName">cloud foundry service name binding</param>
        /// <param name="contextLifetime">Lifetime of the service to inject</param>
        /// <returns>IServiceCollection for chaining</returns>
        public static IServiceCollection AddMongoClient(this IServiceCollection services, IConfiguration config, string serviceName, ServiceLifetime contextLifetime = ServiceLifetime.Singleton)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            if (string.IsNullOrEmpty(serviceName))
            {
                throw new ArgumentNullException(nameof(serviceName));
            }

            if (config == null)
            {
                throw new ArgumentNullException(nameof(config));
            }

            MongoDbServiceInfo info = config.GetRequiredServiceInfo<MongoDbServiceInfo>(serviceName);

            DoAdd(services, info, config, contextLifetime);
            return services;
        }

        private static void DoAdd(IServiceCollection services, MongoDbServiceInfo info, IConfiguration config, ServiceLifetime contextLifetime)
        {
            Type mongoClient = MongoDbTypeLocator.MongoClient;
            var mongoOptions = new MongoDbConnectorOptions(config);
            var clientFactory = new MongoDbConnectorFactory(info, mongoOptions, mongoClient);
            services.Add(new ServiceDescriptor(MongoDbTypeLocator.IMongoClient, clientFactory.Create, contextLifetime));
            services.Add(new ServiceDescriptor(mongoClient, clientFactory.Create, contextLifetime));
            services.Add(new ServiceDescriptor(typeof(IHealthContributor), ctx => new MongoDbHealthContributor(clientFactory, ctx.GetService<ILogger<MongoDbHealthContributor>>()), ServiceLifetime.Singleton));

            Type mongoInfo = ConnectorHelpers.FindType(MongoDbTypeLocator.Assemblies, MongoDbTypeLocator.MongoConnectionInfo);
            var urlFactory = new MongoDbConnectorFactory(info, mongoOptions, mongoInfo);
            services.Add(new ServiceDescriptor(mongoInfo, urlFactory.Create, contextLifetime));
        }
    }
}
