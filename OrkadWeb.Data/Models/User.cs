﻿using FluentNHibernate.Mapping;
using System.Collections.Generic;

namespace OrkadWeb.Data.Models
{
    /// <summary>
    /// Représente un utilisateur de "OrkadWeb"
    /// </summary>
    public class User
    {
        /// <summary>
        /// Identifiant unique de l'utilisateur
        /// </summary>
        public virtual int Id { get; set; }

        /// <summary>
        /// Nom d'utilisateur
        /// </summary>
        public virtual string Username { get; set; }

        /// <summary>
        /// Mot de passe de l'utilisateur (crypté)
        /// </summary>
        public virtual string Password { get; set; }

        /// <summary>
        /// Adresse email de contact de l'utilisateur
        /// </summary>
        public virtual string Email { get; set; }

        /// <summary>
        /// Association des partages utilisateurs
        /// </summary>
        public virtual ISet<UserShare> UserShares { get; set; }
    }

    public class UserMap : ClassMap<User>
    {
        public UserMap()
        {
            Table("user");
            Id(x => x.Id, "id");
            Map(x => x.Username).Column("username");
            Map(x => x.Password).Column("password");
            Map(x => x.Email).Column("email");
            HasMany(x => x.UserShares).KeyColumn("user_id");
        }
    }
}