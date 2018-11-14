using System.Collections.Generic;
using System.Linq;
using ExaltedCharm.Api.Entities;
using ExaltedCharm.Api.Models;
using ExaltedCharm.Api.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ExaltedCharm.Api.Controllers
{
   
    [Route("api/charmTypes")]
    public class CharmTypeController : Controller
    {
        private readonly IRepository _charmTypeRepository;

        public CharmTypeController(IRepository charmTypeRepository)
        {
            _charmTypeRepository = charmTypeRepository;
        }

        [HttpGet()]
        public IActionResult GetCharmTypes()
        {

            var charmTypes = _charmTypeRepository.GetAll<CharmType>().ToList();

            var results = AutoMapper.Mapper.Map<IEnumerable<CharmTypeWithoutCharmsDto>>(charmTypes);

            return Ok(results);
        }

        [HttpGet("{id}")]
        public IActionResult GetCharmType(int id, bool includeCharms = false)
        {
            var charmType = _charmTypeRepository.GetFirst<CharmType>(x => x.Id == id, null, includeCharms ? "Charms" : null);
            if (charmType == null)
            {
                return NotFound();
            }

            if (includeCharms)
            {
                var charmTypeDto = AutoMapper.Mapper.Map<CharmTypeDto>(charmType);
                return Ok(charmTypeDto);
            }
            else
            {
                var charmTypeDto = AutoMapper.Mapper.Map<CharmTypeWithoutCharmsDto>(charmType);
                return Ok(charmTypeDto);
            }
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteCharmType(int id)
        {
            var charmType = _charmTypeRepository.GetAll<CharmType>().Include(x => x.Charms).SingleOrDefault(x => x.Id == id);
            if (charmType == null)
            {
                return NotFound();
            }
            _charmTypeRepository.Delete(charmType);
            if (!_charmTypeRepository.Save())
            {
                return StatusCode(500, "A problem happened while handling your request.");
            }

            return NoContent();
        }
    }
}
