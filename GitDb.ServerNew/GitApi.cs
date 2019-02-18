using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GitDb.Core.Interfaces;
using GitDb.Core.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GitDb.Server
{
    public class GitApiController : ControllerBase
    {
        readonly IGitDb _gitDb;

        public GitApiController(IGitDb gitDb)
        {
            _gitDb = gitDb;
        }

		[Route("{branch}/document/{*key}")]
		[HttpGet]
		//[Authorize(Roles = "admin, read")]
		public Task<IActionResult> Get(string branch, string key) =>
			Result(() => _gitDb.Get(branch, key));

		[Route("{branch}/documents/{*key}")]
        [HttpGet]
        //[Authorize(Roles = "admin, read")]
        public Task<IActionResult> GetFiles(string branch, string key) =>
			Result(() => _gitDb.GetFiles(branch, key));

        [Route("{branch}/document")]
        [HttpPost]
        //[Authorize(Roles = "admin,write")]
        public Task<IActionResult> Save(string branch, [FromBody] SaveRequest request) =>
			Result(() => _gitDb.Save(branch, request.Message, request.Document, request.Author));

        [Route("{branch}/document/delete")]
        [HttpPost]
        //[Authorize(Roles = "admin,write")]
        public Task<IActionResult> Delete(string branch, [FromBody] DeleteRequest request) =>
			Result(() => _gitDb.Delete(branch, request.Key, request.Message, request.Author));

        [Route("tag")]
        [HttpPost]
        //[Authorize(Roles = "admin,write")]
        public Task<IActionResult> Tag([FromBody] Reference reference) =>
			Result(() => _gitDb.Tag(reference));

        [Route("branch")]
        [HttpGet]
        //[Authorize(Roles = "admin,read")]
        public Task<IActionResult> GetBranches() =>
			Result(() => _gitDb.GetAllBranches());

        [Route("branch")]
        [HttpPost]
        //[Authorize(Roles = "admin,write")]
        public Task<IActionResult> CreateBranch([FromBody] Reference reference) =>
			Result(() => _gitDb.CreateBranch(reference));

        static readonly Dictionary<string, ITransaction> transactions = new Dictionary<string, ITransaction>();

        [Route("{branch}/transaction")]
        [HttpPost]
        //[Authorize(Roles = "admin,write")]
        public Task<IActionResult> CreateTransaction(string branch) =>
            Result(async () =>
            {
                var trans = await _gitDb.CreateTransaction(branch);
                var transactionId = Guid.NewGuid().ToString();
                transactions.Add(transactionId, trans);
                return transactionId;
            });

        [Route("{transactionId}/add")]
        [HttpPost]
        //[Authorize(Roles = "admin,write")]
        public Task<IActionResult> AddToTransaction(string transactionId, Document document) =>
			Result(() => transactions[transactionId].Add(document));

        [Route("{transactionId}/addmany")]
        [HttpPost]
        //[Authorize(Roles = "admin,write")]
        public Task<IActionResult> AddToTransaction(string transactionId, List<Document> documents) =>
			Result(() => transactions[transactionId].AddMany(documents));


        [Route("{transactionId}/delete/{key}")]
        [HttpPost]
        //[Authorize(Roles = "admin,write")]
        public Task<IActionResult> DeleteInTransaction(string transactionId, string key) =>
            Result(() => transactions[transactionId].Delete(key));

        [Route("{transactionId}/deleteMany")]
        [HttpPost]
        //[Authorize(Roles = "admin,write")]
        public Task<IActionResult> DeleteInTransaction(string transactionId, List<string> keys) =>
            Result(() => transactions[transactionId].DeleteMany(keys));


        [Route("{transactionId}/commit")]
        [HttpPost]
        //[Authorize(Roles = "admin,write")]
        public Task<IActionResult> CommitTransaction(string transactionId, [FromBody] CommitTransaction commit) =>
            Result(async () =>
            {
                var transaction = transactions[transactionId];
                var sha = await transaction.Commit(commit.Message, commit.Author);
                transactions.Remove(transactionId);
                return sha;
            });

        [Route("{transactionId}/abort")]
        [HttpPost]
        //[Authorize(Roles = "admin,write")]
        public Task<IActionResult> AbortTransaction(string transactionId) =>
            Result(async () =>
            {
                var transaction = transactions[transactionId];
                await transaction.Abort();
                transactions.Remove(transactionId);
            });


        async Task<IActionResult> Result<T>(Func<Task<T>> action)
        {
            try
            {
                return Ok(await action());
            }
            catch (ArgumentException ex)
            {
				return BadRequest(ex.Message);
            }
        }

        async Task<IActionResult> Result(Func<Task> action)
        {
            try
            {
                await action();
                return Ok();
            }
            catch (ArgumentException ex)
            {
				return BadRequest(ex.Message);
            }
        }

    }
}