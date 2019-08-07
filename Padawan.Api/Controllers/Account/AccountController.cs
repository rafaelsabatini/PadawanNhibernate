using Microsoft.AspNetCore.Mvc;
using Padawan.Api.Controllers.Core;
using Padawan.Domain.Commands;
using Padawan.Domain.Entities;
using Padawan.Domain.Handlers;
using Padawan.Domain.Repositories;
using Padawan.Infra.Transations;
using Padawan.Shared;
using Padawan.Shared.Commands;
using Padawan.Shared.Entities;
using Padawan.Shared.Messages;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Padawan.Api.Controllers
{
    [Route(Constantes.API + "v1/cadastros")]
    public class AccountController : BaseController
    {
        private readonly IAccountRepository _repository;
        private readonly AccountHandler _handler;


        public AccountController(IAccountRepository repository, AccountHandler handler, IUow uow) : base(uow)
        {
            _repository = repository;
            _handler = handler;
        }

        [HttpGet]
        public async Task<IActionResult> GetList([FromBody] Filter filter)
        {
            try
            {
                List<Account> result = await _repository.GetList(filter.Page, filter.PageSize);
                if (result.Count == 0)
                    return Ok(new CommandResult(false, Messages.NO_RECORDS_FOUND, null));
                else
                    return Ok(new CommandResult(true, result.Count.ToString() + " Cadastro(s) encontrado(s).", result));
            }
            catch (Exception e)
            {
                return BadRequest(new CommandResult(false, e.Message, null));
            }
        }

        [HttpGet]
        [Route("{id}")]
        public async Task<IActionResult> GetById(long id)
        {
            try
            {
                Account account = await _repository.GetById(id);
                if (account == null)
                    return await Response(null, "id", Messages.NO_RECORDS_FOUND);

                return await Response(account.ToDto());
            }
            catch (Exception ex)
            {
                return await TryErrors(ex);
            }


        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody]CreateAccountCommand command)
        {
            try
            {
                _uow.OpenTransaction();
                var result = await _handler.Handle(command);

                return await Response(result, _handler.Notifications);
            }

            catch (Exception ex)
            {
                return await TryErrors(ex);
            }
        }


        [HttpPut]
        public async Task<IActionResult> Put([FromBody]EditAccountCommand command)
        {
            try
            {
                _uow.OpenTransaction();
                var result = await _handler.Handle(command);
                return await Response(result, _handler.Notifications);
            }
            catch (Exception ex)
            {
                return await TryErrors(ex);
            }
        }

        [HttpDelete]
        [Route("{id}")]
        public async Task<IActionResult> Delete(long id)
        {
            try
            {
                var command = new DeleteAccountCommand
                {
                    Id = id
                };
                _uow.OpenTransaction();

                var result = await _handler.Handle(command);
                return await Response(result, _handler.Notifications);

            }
            catch (Exception ex)
            {
                return await TryErrors(ex);
            }
        }
    }
}