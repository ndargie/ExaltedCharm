using System.Collections.Generic;
using System.Linq;
using ExaltedCharm.Api.Entities;
using ExaltedCharm.Api.Helpers;
using ExaltedCharm.Api.Models;
using ExaltedCharm.Api.Services;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ExaltedCharm.Api.Controllers
{
   
    [Route("api/charmTypes")]
    public class CharmTypeController : Controller
    {
        private readonly IRepository _charmTypeRepository;
        private readonly IUrlHelper _urlHelper;

        public CharmTypeController(IRepository charmTypeRepository, IUrlHelper urlHelper)
        {
            _charmTypeRepository = charmTypeRepository;
            _urlHelper = urlHelper;
        }

        [HttpGet(Name = "GetCharmTypes")]
        public IActionResult GetCharmTypes()
        {
            var charmTypes = _charmTypeRepository.GetAll<CharmType>().ToList();

            var results = AutoMapper.Mapper.Map<IEnumerable<CharmTypeWithoutCharmsDto>>(charmTypes).Select(x =>
                {
                    x = x.GenerateLinks(_urlHelper);
                    return x;
                });

            return Ok(results);
        }

        [HttpGet("{id}", Name = "GetCharmType")]
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
                return Ok(charmTypeDto.GenerateLinks(_urlHelper));
            }
            else
            {
                var charmTypeDto = AutoMapper.Mapper.Map<CharmTypeWithoutCharmsDto>(charmType)
                    .GenerateLinks(_urlHelper);
                return Ok(charmTypeDto);
            }
        }

        [HttpPost(Name = "CreateCharmType")]
        public IActionResult CreateCharmType([FromBody] CharmTypeCreationDto charmType)
        {
            if (charmType == null)
            {
                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                return new UnprocessableEntityObjectResult(ModelState);
            }

            var charmTypeEntity = AutoMapper.Mapper.Map<CharmType>(charmType);
            _charmTypeRepository.Create(charmTypeEntity);

            return !_charmTypeRepository.Save()
                ? StatusCode(500, "A problem happened while handling your request.")
                : CreatedAtRoute("GetCharmType", new {id = charmTypeEntity.Id},
                    AutoMapper.Mapper.Map<CharmTypeDto>(charmTypeEntity).GenerateLinks(_urlHelper));
        }

        [HttpPut("{id}", Name = "UpdateCharmType")]
        public IActionResult UpdateCharmType(int id, CharmTypeUpdateDto charmType)
        {
            if (charmType == null)
            {
                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                return new UnprocessableEntityObjectResult(ModelState);
            }

            var charmTypeEntity = _charmTypeRepository.GetAll<CharmType>().SingleOrDefault(x => x.Id == id);
            if (charmTypeEntity == null)
            {
                return NotFound();
            }

            AutoMapper.Mapper.Map(charmType, charmTypeEntity);

            _charmTypeRepository.Update(charmTypeEntity);

            if (!_charmTypeRepository.Save())
            {
                return StatusCode(500, "A problem happened while handling your request.");
            }

            return NoContent();
        }

        [HttpPatch("{id}", Name = "PartiallyUpdateCharmType")]
        public IActionResult PartiallyUpdateCharmType(int id, [FromBody] JsonPatchDocument<CharmTypeUpdateDto> patchDoc)
        {
            if (patchDoc == null)
            {
                return BadRequest();
            }

            var charmType = _charmTypeRepository.GetAll<CharmType>().SingleOrDefault(x => x.Id == id);

            if (charmType == null)
            {
                var charmTypeDto = new CharmTypeUpdateDto();
                patchDoc.ApplyTo(charmTypeDto, ModelState);

                TryValidateModel(charmTypeDto);

                if (!ModelState.IsValid)
                {
                    return new UnprocessableEntityObjectResult(ModelState);
                }

                var charmTypeToAdd = AutoMapper.Mapper.Map<CharmType>(charmTypeDto);
                charmTypeToAdd.Id = id;

                return !_charmTypeRepository.Save()
                    ? StatusCode(500, "A problem happened while handling your request.")
                    : CreatedAtRoute("GetCharmType", new {id = charmTypeToAdd.Id},
                        AutoMapper.Mapper.Map<CharmTypeDto>(charmTypeToAdd).GenerateLinks(_urlHelper));
            }

            var charmTypeToPatch = AutoMapper.Mapper.Map<CharmTypeUpdateDto>(charmType);

            patchDoc.ApplyTo(charmTypeToPatch, ModelState);

            TryValidateModel(charmTypeToPatch);

            if (!ModelState.IsValid)
            {
                return new UnprocessableEntityObjectResult(ModelState);
            }

            AutoMapper.Mapper.Map(charmTypeToPatch, charmType);

            _charmTypeRepository.Update(charmType);

            if (!_charmTypeRepository.Save())
            {
                return StatusCode(500, "A problem happened while handling your request.");
            }

            return NoContent();
        }

        [HttpDelete("{id}", Name = "DeleteCharmType")]
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
