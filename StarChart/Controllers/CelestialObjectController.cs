using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Conventions.Internal;
using StarChart.Data;
using StarChart.Models;

namespace StarChart.Controllers
{
    [Route("")]
    [ApiController]
    public class CelestialObjectController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public CelestialObjectController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("{id:int}", Name ="GetByID")]
        public IActionResult GetById(int id)
        {
            var celestialObject = _context.CelestialObjects.SingleOrDefault(s => s.Id == id);
            if (celestialObject == null)
                return NotFound();

         foreach (CelestialObject c in _context.CelestialObjects)
            {
                if (c.OrbitedObjectId == celestialObject.Id)
                {
                    celestialObject.Satellites = new List<CelestialObject>();
                    celestialObject.Satellites.Add(c);
                }
            }
            return Ok(celestialObject);
        }
        [HttpGet("{name}")]
        public IActionResult GetByName(string name)
        {
            var celestialObject = from c in _context.CelestialObjects
                                  where c.Name == name
                                  orderby c.Name
                                  select c;

            if (celestialObject.Count<CelestialObject>() == 0)
                return NotFound();

            foreach (CelestialObject c in celestialObject)
            {
                foreach (CelestialObject co in _context.CelestialObjects)
                {
                    if (co.OrbitedObjectId == c.Id)
                    {
                        if (c.Satellites == null)
                        {
                            c.Satellites = new List<CelestialObject>();
                            c.Satellites.Add(co);
                        }
                        else
                            c.Satellites.Add(co);
                    }
                }
            }
            return Ok(celestialObject);
        }
        [HttpGet]
        public IActionResult GetAll()
        {
            foreach(CelestialObject c in _context.CelestialObjects)
            {
                foreach (CelestialObject co in _context.CelestialObjects)
                {
                    if (co.OrbitedObjectId == c.Id)
                    {
                        if (c.Satellites == null)
                        {
                            c.Satellites = new List<CelestialObject>();
                            c.Satellites.Add(co);
                        }
                        else
                            c.Satellites.Add(co);
                    }
                }
            }
            return Ok(_context.CelestialObjects);
        }
    }
}
