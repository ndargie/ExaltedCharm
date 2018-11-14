using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using ExaltedCharm.Api.Entities;
using ExaltedCharm.Api.Models;
using ExaltedCharm.Api.Services;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;

namespace ExaltedCharm.Api.Controllers
{
    [Route("api/keywords")]
    public class KeywordController : Controller
    {
        private readonly IRepository _repository;

        public KeywordController(IRepository repository)
        {
            _repository = repository;
        }

        [HttpGet]
        public IActionResult GetKeywords()
        {
            var keywords = _repository.GetAll<Keyword>().ToList();
            var keywordDtos = AutoMapper.Mapper.Map<IEnumerable<KeywordDto>>(keywords);
            return Ok(keywordDtos);
        }

        [HttpGet("{id}", Name = "GetKeyword")]
        public IActionResult GetKeyword(int id)
        {
            var keyword = _repository.GetFirst<Keyword>(x => x.Id == id);
            if (keyword == null)
            {
                return NotFound();
            }
            var keywordDto = AutoMapper.Mapper.Map<KeywordDto>(keyword);
            return Ok(keywordDto);
        }

        [HttpPost]
        public IActionResult CreateKeyword([FromBody] KeywordCreationDto keyword)
        {
            if (keyword == null)
            {
                return BadRequest();
            }
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var finalKeyword = AutoMapper.Mapper.Map<Keyword>(keyword);

            _repository.Create(finalKeyword);
            if (!_repository.Save())
            {
                return StatusCode(500, "A problem happened while handling your request.");
            }

            return CreatedAtRoute("GetKeyword", new { id = finalKeyword.Id }, keyword);
        }

        [HttpPut("{id}")]
        public IActionResult UpdateKeyword(int id,
            [FromBody] KeywordUpdateDto keyword)
        {
            if (keyword == null)
            {
                return BadRequest();
            }
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (!_repository.GetExists<Keyword>(x => x.Id == id))
            {
                return NotFound();
            }

            var keywordEntity = _repository.GetFirst<Keyword>(x => x.Id == id);
            AutoMapper.Mapper.Map(keyword, keywordEntity);
            _repository.Update(keywordEntity);
            if (!_repository.Save())
            {
                return StatusCode(500, "A problem happened while handling your request.");
            }

            return NoContent();
        }

        [HttpPatch("{id}")]
        public IActionResult PatchKeyword(int id,
            [FromBody] JsonPatchDocument<KeywordUpdateDto> patchDocument)
        {
            if (patchDocument == null)
            {
                return BadRequest();
            }

            var keywordEntity = _repository.GetFirst<Keyword>(x => x.Id == id);
            if (keywordEntity == null)
            {
                return NotFound();
            }

            var keywordToPatch = AutoMapper.Mapper.Map<KeywordUpdateDto>(keywordEntity);
            patchDocument.ApplyTo(keywordToPatch, ModelState);

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            TryValidateModel(keywordToPatch);

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            AutoMapper.Mapper.Map(keywordToPatch, keywordEntity);
            _repository.Update(keywordEntity);
            if (!_repository.Save())
            {
                return StatusCode(500, "A problem happened while handling your request.");
            }

            return NoContent();
        }
    }
}