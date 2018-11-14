using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using ExaltedCharm.Api.Entities;
using ExaltedCharm.Api.Helpers;
using ExaltedCharm.Api.Models;
using ExaltedCharm.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace ExaltedCharm.Api.Controllers
{
    [Route("api/CharmCollection")]
    public class CharmCollectionsController : Controller
    {
        private readonly IRepository _repository;

        public CharmCollectionsController(IRepository repository)
        {
            _repository = repository;
        }

        [HttpPost("{charmTypeId}")]
        public IActionResult CreateCharmCollection(int charmTypeId, 
            [FromBody] IEnumerable<CharmCreationDto> charmCollection)
        {
            if (charmCollection == null)
            {
                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var charmType = _repository.GetAll<CharmType>().SingleOrDefault(x => x.Id == charmTypeId);

            if (charmType == null)
            {
                return NotFound();
            }

            var charms = AutoMapper.Mapper.Map<IEnumerable<Charm>>(charmCollection);
            foreach (var charm in charms)
            {
                charmType.Charms.Add(charm);
            }

            _repository.Update(charmType);

            if (!_repository.Save())
            {
                throw new Exception("Creating an charm collection failed on save.");
            }

            var charmsCollectionToReturn = Mapper.Map<IEnumerable<CharmDto>>(charms);

            var ids = string.Join(",", charmsCollectionToReturn.Select(a => a.Id));

            return CreatedAtRoute("GetCharmCollection", new {ids = ids}, charmsCollectionToReturn);
        }

        [HttpGet("{ids}", Name = "GetCharmCollection")]
        public IActionResult GetCharmCollection([ModelBinder(BinderType = typeof(ArrayModelBinder))] IEnumerable<int> ids)
        {
            if (ids == null)
            {
                return BadRequest();
            }

            var charms = _repository.GetAll<Charm>().Where(x => ids.Contains(x.Id));

            if (ids.Count() != charms.Count())
            {
                return NotFound();
            }

            var charmsToReturn = AutoMapper.Mapper.Map<IEnumerable<CharmDto>>(charms);
            return Ok(charmsToReturn);
        }
    }
}