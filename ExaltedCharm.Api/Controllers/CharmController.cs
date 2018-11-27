using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using ExaltedCharm.Api.Entities;
using ExaltedCharm.Api.Models;
using ExaltedCharm.Api.Services;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace ExaltedCharm.Api.Controllers
{
    [Route("api/charmTypes")]
    public class CharmController : Controller
    {
        private readonly ILogger<CharmController> _logger;
        private readonly IMailService _localMailService;
        private readonly IRepository _repository;

        public CharmController(ILogger<CharmController> logger,
            IMailService localMailService,
            IRepository repository)
        {
            _logger = logger;
            _localMailService = localMailService;
            _repository = repository;
        }

        [HttpGet("{charmTypeId}/charms")]
        public IActionResult GetCharms(int charmTypeId)
        {
            try
            {
                if (!_repository.GetExists<CharmType>(x => x.Id == charmTypeId))
                {
                    _logger.LogInformation($"Charmtype with id {charmTypeId} wasn't found when accessing charms");
                    return NotFound();
                }
                var charms = _repository.GetAll<Charm>()
                    .Where(x => x.CharmTypeId == charmTypeId).ToList();
                var charmsForResult = AutoMapper.Mapper.Map<IEnumerable<CharmDto>>(charms);
                return Ok(charmsForResult);
            }
            catch (Exception ex)
            {
                _logger.LogCritical($"Exception while getting charms for charmtype {charmTypeId}", ex);
                return StatusCode(500, "A problem happened while handling your request.");
            }
        }

        [HttpGet("{charmTypeId}/charms/{id}", Name = "GetCharm")]
        public IActionResult GetCharm(int charmTypeId, int id)
        {
            if (!_repository.GetExists<CharmType>(x => x.Id == charmTypeId))
            {
                _logger.LogInformation($"Charmtype with id {charmTypeId} wasn't found when accessing charms");
                return NotFound();
            }

            var charm = _repository.GetAll<Charm>().SingleOrDefault(x => x.CharmTypeId == charmTypeId && x.Id == id);
            if (charm == null)
            {
                return NotFound();
            }

            var charmDto = AutoMapper.Mapper.Map<CharmDto>(charm);
            return Ok(charmDto);
        }

        [HttpPost("{charmTypeId}/charms")]
        public IActionResult CreateCharm(int charmTypeId,
            [FromBody] CharmCreationDto charm)
        {
            if (charm == null)
            {
                return BadRequest();
            }

            if (charm.Name == charm.Description)
            {
                ModelState.AddModelError("Description", "The provided description should be different from the name");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (!_repository.GetExists<CharmType>(x => x.Id == charmTypeId))
            {
                return NotFound();
            }

            var finalCharm = AutoMapper.Mapper.Map<Charm>(charm);

            var charmType = _repository.GetFirst<CharmType>(x => x.Id == charmTypeId);

            charmType.Charms.Add(finalCharm);
            if (!_repository.Save())
            {
                StatusCode(500, "A problem happened while handling your request.");
            }

            return CreatedAtRoute("GetCharm", new { charmTypeId, id = finalCharm.Id }, finalCharm);
        }

        [HttpPut("{charmTypeId}/charms/{id}", Name = "UpdateCharm")]
        public IActionResult UpdateCharm(int charmTypeId, int id, [FromBody] CharmUpdateDto charm)
        {
            if (charm == null)
            {
                return BadRequest();
            }

            if (charm.Name == charm.Description)
            {
                ModelState.AddModelError("Description", "The provided description should be different from the name");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (!_repository.GetExists<CharmType>(x => x.Id == charmTypeId))
            {
                return NotFound();
            }

            var charmEntity = _repository.GetAll<Charm>().SingleOrDefault(x => x.CharmTypeId == charmTypeId && x.Id == id);
            if (charmEntity == null)
            {
                var charmToAdd = Mapper.Map<Charm>(charm);
                charmToAdd.Id = id;
                var charmType = _repository.GetAll<CharmType>().Single(x => x.Id == charmTypeId);
                charmType.Charms.Add(charmToAdd);
                return _repository.Save() ? StatusCode(500, "A problem happend while handling your request") : CreatedAtRoute("GetCharm", new { charmTypeId, id = charmToAdd.Id }, Mapper.Map<CharmDto>(charmToAdd));
            }

            Mapper.Map(charm, charmEntity);
            _repository.Update(charmEntity);
            if (!_repository.Save())
            {
                return StatusCode(500, "A problem happend while handling your request");
            }

            return NoContent();
        }

        [HttpPatch("{charmTypeId}/charms/{id}", Name = "PartiallyUpdateCharm")]
        public IActionResult PartiallyUpdateCharm(int charmTypeId, int id, [FromBody] JsonPatchDocument<CharmUpdateDto> patchDocument)
        {
            if (patchDocument == null)
            {
                return BadRequest();
            }

            if (!_repository.GetExists<CharmType>(x => x.Id == charmTypeId))
            {
                return NotFound();
            }

            var charmEntity = _repository.GetAll<Charm>().SingleOrDefault(x => x.CharmTypeId == charmTypeId && x.Id == id);
            if (charmEntity == null)
            {
                return NotFound();
            }

            var charmToPatch = AutoMapper.Mapper.Map<CharmUpdateDto>(charmEntity);

            patchDocument.ApplyTo(charmToPatch, ModelState);

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (charmToPatch.Description == charmToPatch.Name)
            {
                ModelState.AddModelError("Description", "The provided description should be different from the name");
            }

            TryValidateModel(charmToPatch);

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            AutoMapper.Mapper.Map(charmToPatch, charmEntity);
            _repository.Update(charmEntity);
            if (!_repository.Save())
            {
                return StatusCode(500, "A problem happened while handling your request");
            }

            return NoContent();
        }

        [HttpDelete("{charmTypeId}/charms/{id}", Name = "DeleteCharm")]
        public IActionResult DeleteCharm(int charmTypeId, int id)
        {
            if (!_repository.GetExists<CharmType>(x => x.Id == charmTypeId))
            {
                return NotFound();
            }

            var charmEntity = _repository.GetAll<Charm>().SingleOrDefault(x => x.CharmTypeId == charmTypeId && x.Id == id);
            if (charmEntity == null)
            {
                return NotFound();
            }

            _repository.Delete(charmEntity); ;
            if (!_repository.Save())
            {
                return StatusCode(500, "A problem happened while handling your request");
            }

            _localMailService.Send("Charm deleted.",
                $"Charm {charmEntity.Name} with id {charmEntity.Id} was deleted.");

            return NoContent();
        }

        [HttpGet("{charmTypeId}/charms/{id}/keywords")]
        public IActionResult GetKeywords(int charmTypeId, int id)
        {
            if (!_repository.GetExists<CharmType>(x => x.Id == charmTypeId))
            {
                return NotFound();
            }

            var charm = _repository.GetFirst<Charm>(x => x.Id == id && x.CharmTypeId == charmTypeId, null, "Keywords.Keyword");
            if (charm == null)
            {
                return NotFound();
            }

            var keywords = AutoMapper.Mapper.Map<IEnumerable<KeywordDto>>(charm.Keywords.Select(x => x.Keyword));
            return Ok(keywords);
        }

        [HttpGet("{charmTypeId}/charms/{id}/keywords/{keywordId}", Name = "GetCharmKeyword")]
        public IActionResult GetKeyword(int charmTypeId, int id, int keywordId)
        {
            if (!_repository.GetExists<CharmType>(x => x.Id == charmTypeId))
            {
                return NotFound();
            }
            var charm = _repository.GetFirst<Charm>(x => x.Id == id && x.CharmTypeId == charmTypeId, null, "Keywords.Keyword");
            if (charm == null)
            {
                return NotFound();
            }

            if (charm.Keywords.All(x => x.KeywordId != keywordId))
            {
                return NotFound();
            }

            var keyword =
                AutoMapper.Mapper.Map<KeywordDto>(charm.Keywords.Single(x => x.KeywordId == keywordId).Keyword);
            return Ok(keyword);
        }

        [HttpPost("{charmTypeId}/charms/{id}/keywords/{keywordId}")]
        public IActionResult AddKeywordToCharm(int charmTypeId, int id, int keywordId)
        {
            if (!_repository.GetExists<CharmType>(x => x.Id == charmTypeId))
            {
                return NotFound();
            }

            var charm = _repository.GetFirst<Charm>(x => x.Id == id && x.CharmTypeId == charmTypeId, null, "Keywords.Keyword");
            if (charm == null)
            {
                return NotFound();
            }

            var keyword = _repository.GetFirst<Keyword>(x => x.Id == keywordId);

            if (keyword == null)
            {
                return NotFound();
            }
          
            charm.AddKeyword(keyword);
            _repository.Update(charm);
            if (!_repository.Save())
            {
                return StatusCode(500, "A problem happened while handling your request");
            }

            return CreatedAtRoute("GetCharmKeyword", new {charmTypeId, id, keywordId}, keyword);
        }

        [HttpDelete("{charmTypeId}/charms/{id}/keywords/{keywordId}")]
        public IActionResult RemoveKeyword(int charmTypeId, int id, int keywordId)
        {
            if (!_repository.GetExists<CharmType>(x => x.Id == charmTypeId))
            {
                return NotFound();
            }

            var charm = _repository.GetFirst<Charm>(x => x.Id == id && x.CharmTypeId == charmTypeId, null, "Keywords.Keyword");
            if (charm == null)
            {
                return NotFound();
            }

            var keyword = _repository.GetFirst<Keyword>(x => x.Id == keywordId);

            if (keyword == null)
            {
                return NotFound();
            }

            charm.RemoveKeyword(keyword);
            _repository.Update(charm);
            if (!_repository.Save())
            {
                return StatusCode(500, "A problem happened while handling your request");
            }

            return NoContent();
        }
    }
}