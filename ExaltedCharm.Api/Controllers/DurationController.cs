using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using ExaltedCharm.Api.Entities;
using ExaltedCharm.Api.Models;
using ExaltedCharm.Api.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ExaltedCharm.Api.Controllers
{
    [Route("api/Durations")]
    public class DurationController : Controller
    {
        private readonly IRepository _repository;

        public DurationController(IRepository repository)
        {
            _repository = repository;
        }

        [HttpGet]
        public IActionResult GetDurations()
        {
            var durations = _repository.GetAll<Duration>();
            return Ok(AutoMapper.Mapper.Map<IEnumerable<DurationDto>>(durations));
        }

        [HttpGet("{id}", Name = "GetDuration")]
        public IActionResult GetDuration(int id)
        {
            var duration = _repository.GetAll<Duration>().SingleOrDefault(x => x.Id == id);
            if (duration == null)
            {
                return NotFound();
            }

            return Ok(AutoMapper.Mapper.Map<DurationDto>(duration));
        }

        [HttpGet("{id}/Charms")]
        public IActionResult GetCharmsForDuration(int id)
        {
            var duration = _repository.GetAll<Duration>().Include(x => x.Charms).SingleOrDefault(x => x.Id == id);
            if (duration == null)
            {
                return NotFound();
            }

            var charms = AutoMapper.Mapper.Map<IEnumerable<CharmDto>>(duration.Charms);
            return Ok(charms);
        }

        [HttpPost]
        public IActionResult CreateDuration([FromBody] SaveDurationDto duration)
        {

            if (duration == null)
            {
                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var durationEntity = AutoMapper.Mapper.Map<Duration>(duration);
            _repository.Create(durationEntity);
            return !_repository.Save() ?
                StatusCode(500, "A problem happened while handling your request.") :
                CreatedAtRoute("GetDuration", new { id = durationEntity.Id }, AutoMapper.Mapper.Map<DurationDto>(durationEntity));
        }

        [HttpPost("{id}")]
        public IActionResult BlockDurationCreation(int id)
        {
            var duration = _repository.GetAll<Duration>().SingleOrDefault(x => x.Id == id);
            if (duration == null)
            {
                return NotFound();
            }

            return new StatusCodeResult(StatusCodes.Status409Conflict);
        }

        [HttpPut("{id}")]
        public IActionResult UpdateDuration(int id, [FromBody] DurationForUpdate duration)
        {
            if (duration == null)
            {
                return BadRequest();
            }

            var durationEntity = _repository.GetAll<Duration>().SingleOrDefault(x => x.Id == id);
            if (durationEntity == null)
            {
                return NotFound();
            }

            Mapper.Map(duration, durationEntity);

            _repository.Update(durationEntity);

            if (!_repository.Save())
            {
                return StatusCode(500, "A problem happened while handling your request.");
            }

            return NoContent();
        }

        [HttpPatch("{id}")]
        public IActionResult PartiallyUpdateDuration(int id,
            [FromBody] JsonPatchDocument<DurationForUpdate> patchDoc)
        {
            if (patchDoc == null)
            {
                return BadRequest();
            }

            var duration = _repository.GetAll<Duration>().SingleOrDefault(x => x.Id == id);

            if (duration == null)
            {
                var durationDto = new DurationForUpdate();
                patchDoc.ApplyTo(durationDto);
                var durationToAdd = Mapper.Map<Duration>(durationDto);
                durationToAdd.Id = id;
                return !_repository.Save()
                    ? StatusCode(500, "A problem happened while handling your request.")
                    : CreatedAtRoute("GetDuration", new { id = durationToAdd.Id },
                        AutoMapper.Mapper.Map<DurationDto>(durationToAdd));
            }

            var durationToPath = AutoMapper.Mapper.Map<DurationForUpdate>(duration);

            patchDoc.ApplyTo(durationToPath);

            Mapper.Map(durationToPath, duration);

            _repository.Update(duration);

            if (!_repository.Save())
            {
                return StatusCode(500, "A problem happened while handling your request.");
            }

            return NoContent();

        }
    }
}