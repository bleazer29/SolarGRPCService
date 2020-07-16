using SolarService.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SolarService.Database
{
    public class DatabaseData
    {
        private static DatabaseData instance;

        public SolarContext db;

        private DatabaseData()
        {
            db = new SolarContext();
        }

        public static DatabaseData GetInstance()
        {
            if (instance == null)
            {
                instance = new DatabaseData();
            }
            return instance;
        }
    }
}
