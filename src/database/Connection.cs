﻿using SoborniyProject;
using System;
using System.Data.Common;

namespace SoborniyProject.database
{
    public class Connection: DbConnectionStringBuilder
    {

        public static string GetString()
        {
            Settings.Load();
            return String.Format(
                "Server={0};Port={1};UserId={2};Password={3};Database={4}",
                Settings.MYSQL_HOST,
                Settings.MYSQL_PORT,
                Settings.MYSQL_USER,
                Settings.MYSQL_PASSWORD,
                Settings.MYSQL_DATABASE
            );
        }
    }
}