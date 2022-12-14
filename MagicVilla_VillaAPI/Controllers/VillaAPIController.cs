using MagicVilla_VillaAPI.Data;
using MagicVilla_VillaAPI.Models;
using MagicVilla_VillaAPI.Models.Dto;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MagicVilla_VillaAPI.Controllers
{
    [ApiController]
    [Route("api/VillaAPI")]
    public class VillaAPIController : ControllerBase
    {
        private readonly ApplicationDbContext _db;

        public VillaAPIController(ApplicationDbContext db)
        {
            _db = db;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public ActionResult<IEnumerable<VillaDTO>> GetVillas()
        {
            return Ok(_db.Villas.ToList());
        }

        [HttpGet("{id:int}", Name = nameof(GetVilla))]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<VillaDTO> GetVilla(int id)
        {
            if (id == 0)
            {
                return BadRequest();
            }

            var villa = _db.Villas.FirstOrDefault(v => v.Id == id);
            if (villa is null)
            {
                return NotFound();
            }

            return Ok(villa);
        }

        [HttpGet("occupancy/{occupancy:int}", Name = nameof(GetVillasByOccupancy))]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<VillaDTO> GetVillasByOccupancy(int occupancy)
        {
            if (occupancy <= 0)
            {
                return BadRequest();
            }

            var villasByOccupancy = new List<VillaDTO>();
            var villasFromDb = _db.Villas.ToList();
            foreach (var villa in villasFromDb)
            {
                if (villa.Occupancy == occupancy)
                {
                    villasByOccupancy.Add(new VillaDTO
                    {
                        Id = villa.Id,
                        Area = villa.Area,
                        Amenity = villa.Amenity,
                        Details = villa.Details,
                        Name = villa.Name,
                        ImageUrl = villa.ImageUrl,
                        Rate = villa.Rate,
                        Occupancy = villa.Occupancy
                    });
                }
            }

            if (villasByOccupancy is null)
            {
                return NotFound();
            }

            return Ok(villasByOccupancy);
        }

        [HttpGet("occupancy/{occupancy:int}/area/{area:int}", Name = nameof(GetVillasByOccupancyAndArea))]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<VillaDTO> GetVillasByOccupancyAndArea(int occupancy, int area)
        {
            if (occupancy <= 0 && area <= 0)
            {
                return BadRequest();
            }

            var villasByOccupancyAndArea = new List<VillaDTO>();
            var villasFromDb = _db.Villas.ToList();
            foreach (var villa in villasFromDb)
            {
                if (villa.Occupancy == occupancy && villa.Area == area)
                {
                    villasByOccupancyAndArea.Add(new VillaDTO
                    {
                        Id = villa.Id,
                        Area = villa.Area,
                        Amenity = villa.Amenity,
                        Details = villa.Details,
                        Name = villa.Name,
                        ImageUrl = villa.ImageUrl,
                        Rate = villa.Rate,
                        Occupancy = villa.Occupancy
                    });
                }
            }

            if (villasByOccupancyAndArea is null)
            {
                return NotFound();
            }

            return Ok(villasByOccupancyAndArea);
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult<VillaDTO> CreateVilla([FromBody] VillaDTO villaDTO)
        {
            if (villaDTO is null)
            {
                return BadRequest(villaDTO);
            }

            if (_db.Villas.FirstOrDefault(v => v.Name.ToLower() == villaDTO.Name.ToLower()) is not null)
            {
                ModelState.AddModelError("CustomError", "Villa already exists!");
                return BadRequest(ModelState);
            }

            if (villaDTO.Id > 0)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }

            var villa = new Villa
            {
                Area = villaDTO.Area,
                Amenity = villaDTO.Amenity,
                Details = villaDTO.Details,
                Name = villaDTO.Name,
                ImageUrl = villaDTO.ImageUrl,
                Rate = villaDTO.Rate,
                Occupancy = villaDTO.Occupancy,
                CreatedDate = DateTime.Now,
            };

            _db.Villas.Add(villa);
            _db.SaveChanges();

            return CreatedAtRoute(nameof(GetVilla), new { id = villaDTO.Id }, villaDTO);
        }

        [HttpDelete("{id:int}", Name = nameof(DeleteVilla))]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult DeleteVilla(int id)
        {
            if (id == 0)
            {
                return BadRequest();
            }

            var villa = _db.Villas.FirstOrDefault(v => v.Id == id);
            if (villa is null)
            {
                return NotFound();
            }

            _db.Villas.Remove(villa);
            _db.SaveChanges();

            return NoContent();
        }

        [HttpPut("{id:int}", Name = nameof(UpdateVilla))]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult UpdateVilla(int id, [FromBody] VillaDTO villaDTO)
        {
            if (villaDTO is null || id != villaDTO.Id)
            {
                return BadRequest();
            }

            var villa = _db.Villas.FirstOrDefault(v => v.Id == id);
            if (villa is null)
            {
                return NotFound();
            }
            villa.Id = id;
            villa.Area = villaDTO.Area;
            villa.Amenity = villaDTO.Amenity;
            villa.Details = villaDTO.Details;
            villa.Name = villaDTO.Name;
            villa.ImageUrl = villaDTO.ImageUrl;
            villa.Rate = villaDTO.Rate;
            villa.Occupancy = villaDTO.Occupancy;
            villa.UpdateDate = DateTime.Now;

            _db.Villas.Update(villa);
            _db.SaveChanges();

            return NoContent();
        }

        [HttpPatch("{id:int}", Name = nameof(UpdatePartialVilla))]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult UpdatePartialVilla(int id, JsonPatchDocument<VillaDTO> patchVillaDTO)
        {
            if (patchVillaDTO is null || id == 0)
            {
                return BadRequest();
            }

            var villa = _db.Villas.AsNoTracking().FirstOrDefault(v => v.Id == id);
            if (villa is null)
            {
                return NotFound();
            }

            var villaDTO = new VillaDTO
            {
                Id = villa.Id,
                Area = villa.Area,
                Amenity = villa.Amenity,
                Details = villa.Details,
                Name = villa.Name,
                ImageUrl = villa.ImageUrl,
                Rate = villa.Rate,
                Occupancy = villa.Occupancy
            };
            patchVillaDTO.ApplyTo(villaDTO, ModelState);
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            var partialUpdatedVilla = new Villa
            {
                Id = villaDTO.Id,
                Area = villaDTO.Area,
                Amenity = villaDTO.Amenity,
                Details = villaDTO.Details,
                Name = villaDTO.Name,
                ImageUrl = villaDTO.ImageUrl,
                Rate = villaDTO.Rate,
                Occupancy = villaDTO.Occupancy,
                UpdateDate = DateTime.Now,
                CreatedDate = villa.CreatedDate
            };

            _db.Villas.Update(partialUpdatedVilla);
            _db.SaveChanges();

            return NoContent();
        }
    }
}
