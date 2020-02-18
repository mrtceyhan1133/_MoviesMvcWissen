﻿using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading;
using System.Web;
using _036_MoviesMvcWissen.Entities;

namespace _036_MoviesMvcWissen.Contexts
{
    public class MoviesContext : DbContext
    {
        public MoviesContext() : base("MoviesContext")
        {
            //Disable initializer
            Database.SetInitializer<MoviesContext>(null);
        }

        public virtual DbSet<Movie> Movies { get; set; }
        public virtual DbSet<Director> Directors { get; set; }
        public virtual DbSet<Review> Reviews { get; set; }
        public virtual DbSet<MovieDirector> MovieDirectors { get; set; }
        public virtual DbSet<User> Users { get; set; }

        public virtual DbSet<Role> Roles { get; set; }
        public virtual DbSet<vwUser> vwUsers { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            //modelBuilder.Ignore<vwUser>();
        }
        
    }
    //birden fazla dbcontext varsa alttaki komutlarla contexti belirtmemiz gerekiyo
    //enable-migrations -ContextTypeName _036....
    //add-migration -ConfigurationTypeName _036_Movies....Configuration
    //update-database -SourceMigration namespace.migrationadi
}