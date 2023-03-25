﻿using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;
using FluentNHibernate.Mapping;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHibernate.Tool.hbm2ddl;
using OrkadWeb.Domain.Exceptions;
using OrkadWeb.Infrastructure.Persistence;
using System.Threading;

namespace OrkadWeb.Tests.UnitTests.Persistence
{
    [TestClass]
    public class NHibernateDataServiceTest
    {
        private NHibernate.ISession session;

        private class TestEntity
        {
            public virtual int Id { get; init; }
            public virtual string Name { get; init; }
        }

        private class TestEntityMap : ClassMap<TestEntity>
        {
            public TestEntityMap()
            {
                Id(x => x.Id).GeneratedBy.Identity();
                Map(x => x.Name);
            }
        }

        [TestInitialize]
        public void Init()
        {
            var configuration = Fluently.Configure()
                .Database(SQLiteConfiguration.Standard.InMemory())
                .Mappings(mapping => mapping.FluentMappings.Add<TestEntityMap>())
                .BuildConfiguration();
            var sessionFactory = configuration.BuildSessionFactory();
            session = sessionFactory.OpenSession();
            new SchemaExport(configuration).Execute(false, true, false, session.Connection, null);
        }

        [TestMethod]
        public async Task TransactTest()
        {
            var service = new NHibernateDataService(session);
            await service.TransactAsync(async () =>
            {
                await service.InsertAsync(new TestEntity
                {
                    Name = "Test",
                });
                Check.That(service.Exists<TestEntity>(e => e.Name == "Test")).IsTrue();
            });
        }

        [TestMethod]
        public void TransactionFailTest()
        {
            var service = new NHibernateDataService(session);
            Check.ThatCode(async () =>
            {
                await service.TransactAsync(async () =>
                {
                    await service.InsertAsync(new TestEntity
                    {
                        Name = "Test",
                    });
                    throw new System.Exception("Fail");
                });
            }).ThrowsAny();
            Check.That(service.Exists<TestEntity>(e => e.Name == "Test")).IsFalse();
        }

        [TestMethod]
        public void NestedTransactionFailTest()
        {
            var service = new NHibernateDataService(session);
            Check.ThatCode(async () =>
            {
                await service.TransactAsync(async () =>
                {
                    await service.InsertAsync(new TestEntity
                    {
                        Name = "Test",
                    });
                    await service.TransactAsync(async () =>
                    {
                        await service.InsertAsync(new TestEntity
                        {
                            Name = "Test",
                        });
                    });

                    throw new System.Exception("Fail");
                });
            }).ThrowsAny();
            Check.That(service.Exists<TestEntity>(e => e.Name == "Test")).IsFalse();

        }
    }
}
