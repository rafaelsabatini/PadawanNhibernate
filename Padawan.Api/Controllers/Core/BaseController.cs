using Flunt.Notifications;
using Microsoft.AspNetCore.Mvc;
using Padawan.Infra.Transations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Padawan.Api.Controllers.Core
{
    public class BaseController: Controller
    {
        protected readonly IUow _uow;

        public BaseController(IUow unitOfWork)
        {
            _uow = unitOfWork;
        }


        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<IActionResult> Response(object result)
        {
            return await Response(result, new List<Notification> ());
        }



        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<IActionResult> Response(object result, string property, string message)
        {
            return await Response(result, new List<Notification> { new Notification(property, message) });
        }


        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<IActionResult> Response(object result, IEnumerable<Notification> notifications)
        {
            if (!notifications.Any())
            {
                try
                {
                    bool ExistsNofications = false;

                    _uow.OpenTransaction();

                     await _uow.Commit();


                    return Ok(new
                    {
                        success = true,
                        data = result
                    });

                }
                catch (Exception ex)
                {
                    
                    return await TryErrors(ex);
                }
            }
            else
            {
                _uow.Rollback();
                return BadRequest(new { success = false, errors = notifications.GroupBy(x => new { x.Property, x.Message }).Select(x => x.FirstOrDefault()) });
            }
        }
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<IActionResult> TryErrors(Exception ex)
        {

            _uow.Rollback();

            //Podemos incluir o log do Elmah

            var erros = new List<Notification>();
            erros.Add(new Notification("BadRequest", "Ocorreu uma falha interna no servidor"));

            return BadRequest(new { success = false, errors = erros });
        }
    }
}
