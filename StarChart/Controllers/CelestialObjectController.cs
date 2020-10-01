using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Reflection.Emit;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Diagnostics;
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

        [HttpGet("{id:int}",Name = "GetById")]
        public IActionResult GetById(int id)
        {
            var celestialObject = _context.CelestialObjects.SingleOrDefault(s => s.Id == id);
            if (celestialObject == null)
                return NotFound();

            celestialObject.Satellites = _context.CelestialObjects.Where(c => c.OrbitedObjectId == id).ToList();

            return Ok(celestialObject);
        }
        [HttpGet("{name}")]
        public IActionResult GetByName(string name)
        {
            var celestialObjects = from c in _context.CelestialObjects
                                   where c.Name == name
                                   orderby c.Name
                                   select c;

            if (!celestialObjects.Any())
                return NotFound();

            foreach (CelestialObject co in celestialObjects)
            {
                co.Satellites = _context.CelestialObjects.Where(c => c.OrbitedObjectId == co.OrbitedObjectId).ToList();
            }
            return Ok(celestialObjects);
        }
        [HttpGet]
        public IActionResult GetAll()
        {
            var celestialObjects = _context.CelestialObjects.ToList();
            foreach (CelestialObject co in celestialObjects)
            {
                co.Satellites = _context.CelestialObjects.Where(c => c.OrbitedObjectId == co.OrbitedObjectId).ToList();
            }
            return Ok(celestialObjects);
        }

        [HttpPost]
        public IActionResult Create([FromBody] CelestialObject celestialObject)
        {
            _context.CelestialObjects.Add(celestialObject);
            return CreatedAtRoute("GetById", new { id = celestialObject.Id }, celestialObject);
        }

        [HttpPut("{id}")]
        public IActionResult Update(int id, CelestialObject celestialObject)
        {
            var co = _context.CelestialObjects.SingleOrDefault(c => c.Id == id);
            if (co == null)
                return NotFound();
            
            co.Name = celestialObject.Name;
            co.OrbitalPeriod = celestialObject.OrbitalPeriod;
            co.OrbitedObjectId = celestialObject.OrbitedObjectId;

            _context.CelestialObjects.Update(co);
            _context.SaveChanges();
            
            return NoContent();
        }

        [HttpPatch("{id}/{name}")]
        public IActionResult RenameObject(int id, string name)
        {
            var co = _context.CelestialObjects.SingleOrDefault(c => c.Id == id);
            if (co == null)
                return NotFound();
            
            co.Name = name;
            _context.CelestialObjects.Update(co);
            _context.SaveChanges();
            
            return NoContent();
        }
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var celestialObject = _context.CelestialObjects.Where(c => c.Id == id || c.OrbitedObjectId == id).ToList();
            if (!celestialObject.Any())
                return NotFound();

            _context.CelestialObjects.RemoveRange(celestialObject);
            _context.SaveChanges();

            return NoContent();
        }
    }

}
