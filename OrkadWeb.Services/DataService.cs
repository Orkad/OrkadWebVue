﻿using NHibernate;
using OrkadWeb.Services.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OrkadWeb.Services
{
    public class DataService : IDataService
    {
        private readonly ISession session;

        public DataService(ISession session)
        {
            this.session = session;
        }

        /// <summary>
        /// Récupération d'une l'entité.
        /// </summary>
        /// <typeparam name="T">type de l'entité</typeparam>
        /// <param name="id">identifiant unique de l'entité</param>
        /// <exception cref="NotFoundException{T}">Si l'entité n'existe pas</exception>
        public T Get<T>(object id) => session.Get<T>(id) ?? throw new NotFoundException<T>(id);

        /// <summary>
        /// Création de requète sur les entités du type fourni
        /// </summary>
        /// <typeparam name="T">type de l'entité</typeparam>
        public IQueryable<T> Query<T>() => session.Query<T>();

        public object Insert<T>(T obj) => session.Save(obj);
        public void Update<T>(T obj) => session.Update(obj);
        public void Delete<T>(T obj) => session.Delete(obj);
    }
}
