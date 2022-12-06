using Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class Cliente : AuditableBaseEntity
    {
        public string Nombre { get; set; }
        public string Apellido { get; set; }
        private int edad { get; set; }
        public string Telefono { get; set; } 
        public string Email { get; set; }
        public DateTime FechaNacimiento { get; set; }
        public string Direccion { get; set; }
        public int Edad 
        
        { get 
            
            { if (edad <= 0) 
                { 
                    edad = new DateTime(DateTime.Now.Subtract(this.FechaNacimiento).Ticks).Year - 1;
                }
                return edad;
            }
            set { edad = value;}
        }
    }
}
