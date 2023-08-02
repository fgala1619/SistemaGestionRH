using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.DotNet.Scaffolding.Shared.Messaging;
using Microsoft.EntityFrameworkCore;
using SistemaGestionRH.Models;

namespace SistemaGestionRH.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TrabajadorController : ControllerBase
    {
        private readonly RRHHContext _context;

        public TrabajadorController(RRHHContext context)
        {
            _context = context;
        }

        // GET: api/Trabajador
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Trabajador>>> GetTrabajadors()
        {
          if (_context.Trabajadors == null)
          {
              return NotFound();
          }
            return await _context.Trabajadors.ToListAsync();
        }

        // GET: api/Trabajador/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Trabajador>> GetTrabajador(int id)
        {
          if (_context.Trabajadors == null)
          {
              return NotFound();
          }
            var trabajador = await _context.Trabajadors.FindAsync(id);

            if (trabajador == null)
            {
                return NotFound();
            }

            return trabajador;
        }

        // PUT: api/Trabajador/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutTrabajador(int id, Trabajador trabajador)
        {
            if (id != trabajador.Id)
            {
                return BadRequest();
            }

            _context.Entry(trabajador).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TrabajadorExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Trabajador
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Trabajador>> PostTrabajador(Trabajador trabajador)
        {
          if (_context.Trabajadors == null)
          {
              return Problem("Entity set 'RRHHContext.Trabajadors'  is null.");
          }
            _context.Trabajadors.Add(trabajador);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetTrabajador", new { id = trabajador.Id }, trabajador);
        }

        // DELETE: api/Trabajador/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTrabajador(int id)
        {
            if (_context.Trabajadors == null)
            {
                return NotFound();
            }
            var trabajador = await _context.Trabajadors.FindAsync(id);
            if (trabajador == null)
            {
                return NotFound();
            }

            _context.Trabajadors.Remove(trabajador);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool TrabajadorExists(int id)
        {
            return (_context.Trabajadors?.Any(e => e.Id == id)).GetValueOrDefault();
        }

        [HttpGet("trabajadoresRoles")]
        public async Task<IActionResult> TrabajadoresRoles()
        {
            if (_context.Trabajadors == null)
            {
                return NotFound();
            }

            var trabajores_roles = from t in _context.Trabajadors join r in _context.Rols on t.IdRol equals r.IdRol
                                   select new {

                                       Nombre = t.Nombre,
                                       Apellido = t.Apellidos,
                                       Rol = r.Descripcion

                                   };

            return Ok(trabajores_roles);
            
        }

        [HttpPost("recalcularSalario")]
        public async Task<IActionResult> RecalcularSalario(string ultimaRev)
        {           

            DateTime fecha_actual = DateTime.Now;

            DateTime fechaRev = DateTime.Parse(ultimaRev);

            DateTime fechaRevMes = fechaRev.AddMonths(3);

            var msj = "";

            decimal salario = 0;
            

            int fechaResultado = fecha_actual.CompareTo(fechaRevMes);

            if (fechaResultado < 0)
            {
                msj = "No se puede realizar la operacion. Las revisiones se realizan cada tres meses.";
                return Ok(msj);
            }
            else
            {
                var trabajadores = _context.Trabajadors.ToList();

                foreach (var item in trabajadores)
                {
                    DateTime fechaContrat = item.FechaContratacion.AddMonths(3);
                    int fechaAccion = fechaContrat.CompareTo(fecha_actual);

                    if (fechaAccion <= 0)
                    {
                        if (item.IdRol == 1)
                        {
                            item.SalarioIncrementado = item.SalarioInicial + (item.SalarioInicial * 5 / 100);
                            item.FechaIncrementoSalarial = fecha_actual;
                            item.FechaContratacion = fecha_actual;
                            item.SalarioInicial = (decimal)item.SalarioIncrementado;
                            await PutTrabajador(item.Id, item);
                        }
                        else if (item.IdRol == 2)
                        {
                            item.SalarioIncrementado = item.SalarioInicial + (item.SalarioInicial * 8 / 100);
                            item.FechaIncrementoSalarial = fecha_actual;
                            item.FechaContratacion = fecha_actual;
                            item.SalarioInicial = (decimal)item.SalarioIncrementado;
                            await PutTrabajador(item.Id, item);
                        }
                        else
                        {
                            item.SalarioIncrementado = item.SalarioInicial + (item.SalarioInicial * 12 / 100);
                            item.FechaIncrementoSalarial = fecha_actual;
                            item.FechaContratacion = fecha_actual;
                            item.SalarioInicial = (decimal)item.SalarioIncrementado;
                            await PutTrabajador(item.Id, item);
                        }
                    }
                }

            }

            msj = "Los salarios de los trabajadores fueron calculados con éxito.";
            return Ok(msj);
        });

        }

        [HttpGet("historialSalario")]
        public async Task<IActionResult> HistorialSalario(string nombre, string apellidos)
        {
            if (_context.Trabajadors == null)
            {
                return NotFound();
            }

            var listaTrabajadoresNombre = from trab in _context.Trabajadors
                                          where trab.Nombre == nombre &&
                                          trab.Apellidos == apellidos
                                          select new
                                          {
                                              Monto = trab.SalarioIncrementado,
                                              FechaIncremento = trab.FechaIncrementoSalarial
                                          };            

            return Ok(listaTrabajadoresNombre);
        }

    }
}
