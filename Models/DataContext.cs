using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace inmobiliariaAST.Models
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options) { 
            
            
        }
        public DbSet<Propietario> Propietario { get; set; }

        public DbSet<Inmueble> Inmueble { get; set; }
    }
    
}
