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

        public CelestialObjectController(ApplicationDbContext applicationDbContext)
        {
            _context = applicationDbContext;
        }

        [HttpGet("{id:int}", Name ="GetByID")]
        public IActionResult GetById(int id)
        {
            var celestialObject = _context.CelestialObjects.SingleOrDefault(s => s.Id == id);
            if (celestialObject == null)
                return NotFound();

            celestialObject.Satellites = (List<CelestialObject>)_context.CelestialObjects.Select(s => s.OrbitedObjectId == id);
            return Ok(celestialObject);
        }
        [HttpGet("{name}")]
        public IActionResult GetByName(string name)
        {
            var celestialObject = _context.CelestialObjects.SingleOrDefault(s => s.Name == name);
            if (celestialObject == null)
                return NotFound();
            celestialObject.Satellites = (List<CelestialObject>)_context.CelestialObjects.Select(s => s.OrbitedObjectId == celestialObject.Id);
            return Ok(celestialObject);

        }
    }
}
